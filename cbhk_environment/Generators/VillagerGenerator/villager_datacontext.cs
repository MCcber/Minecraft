﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using cbhk_environment.Generators.VillagerGenerator.Components;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Threading.Tasks;
using cbhk_environment.GeneralTools.Displayer;
using cbhk_environment.CustomControls;
using Newtonsoft.Json.Linq;
using cbhk_environment.GeneralTools;

namespace cbhk_environment.Generators.VillagerGenerator
{
    public class villager_datacontext:ObservableObject
    {
        #region 处理拖拽
        public static bool IsGrabingItem = false;
        Image drag_source = null;
        Image GrabedImage = null;
        #endregion

        #region 生成、返回、保存指令
        public RelayCommand RunCommand { get; set; }
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand Save { get; set; }
        public RelayCommand ImportFromClipboard { get; set; }
        public RelayCommand ImportFromFile { get; set; }
        #endregion

        #region 版本源
        private string selectedVersion = "";
        public string SelectedVersion
        {
            get
            {
                return selectedVersion;
            }
            set
            {
                selectedVersion = value;
                if ((CanEditBrain || CanEditGossips) && selectedVersion == "1.13-")
                    CanEditBrain = CanEditGossips = false;
                CanTouchBrain = CanTouchGossips = selectedVersion != "1.13-";
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> VersionSource = new ObservableCollection<string> { "1.13-","1.14+" };
        #endregion

        #region 是否显示结果
        public bool ShowResult { get; set; }
        #endregion

        #region 存储生成结果
        public string Result { get; set; }
        #endregion

        #region 字段与引用
        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconVillagers.png";

        //左侧交易项数据源
        public ObservableCollection<TransactionItems> transactionItems { get; set; } = new ObservableCollection<TransactionItems> { };
        //言论数据源
        public ObservableCollection<GossipsItems> gossipItems { get; set; } = new ObservableCollection<GossipsItems> { };

        //是否显示物品数据页
        bool IsDisplayItemInfoWindow = false;
        //物品数据页
        public TransactionItemDataForm transactionItemDataForm = new TransactionItemDataForm();
        //物品数据页容器
        public static Popup popup = new()
        {
            AllowDrop = false,
            IsOpen = false
        };
        //当前选中的物品
        public TransactionItems CurrentItem = null;
        #endregion

        #region 言论面板收放
        private Visibility isEditGossips = Visibility.Collapsed;
        public Visibility IsEditGossips
        {
            get { return isEditGossips; }
            set
            {
                isEditGossips = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 是否可以编辑言论
        private bool canEditGossips = false;
        public bool CanEditGossips
        {
            get { return canEditGossips; }
            set
            {
                canEditGossips = value;
                OnPropertyChanged();
                IsEditGossips = CanEditGossips ? Visibility.Visible:Visibility.Collapsed;
                //恢复所有交易项的价格
                if (!CanEditGossips)
                    transactionItems.All(item => { item.HideDiscountData();return true; });
                else
                    transactionItems.All(item => { item.HideDiscountData(false); return true; });
                OnlyEditItem = !CanEditBrain && !CanEditGossips ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        #region 是否可以点击言论
        private bool canTouchGossips = true;
        public bool CanTouchGossips
        {
            get { return canTouchGossips; }
            set
            {
                canTouchGossips = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 是否可以点击记忆
        private bool canTouchBrain = true;
        public bool CanTouchBrain
        {
            get { return canTouchBrain; }
            set
            {
                canTouchBrain = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 记忆面板收放
        private Visibility isEditBrain = Visibility.Collapsed;
        public Visibility IsEditBrain
        {
            get { return isEditBrain; }
            set
            {
                isEditBrain = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 是否可以编辑记忆
        private bool canEditBrain = false;
        public bool CanEditBrain
        {
            get { return canEditBrain; }
            set
            {
                canEditBrain = value;
                OnPropertyChanged();
                IsEditBrain = CanEditBrain ? Visibility.Visible : Visibility.Collapsed;
                OnlyEditItem = !CanEditBrain && !CanEditGossips ?Visibility.Collapsed:Visibility.Visible;
            }
        }
        #endregion

        #region 言论与记忆面板收放
        private Visibility onlyEditItem = Visibility.Visible;
        public Visibility OnlyEditItem
        {
            get { return onlyEditItem; }
            set
            {
                onlyEditItem = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 已选中的成员
        private ItemStructure selectedItem = null;
        public ItemStructure SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 搜索内容
        private string searchText = "";
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                BagViewSource?.View.Refresh();
            }
        }
        #endregion

        #region 村民数据

        #region Offers
        private string Offers
        {
            get
            {
                if (transactionItems.Count == 0) return "";
                string result = "Offers:{Recipes:[";
                string transactionItemData = string.Join("", transactionItems.Select(item => item.TransactionItemData + ","));
                result += transactionItemData.TrimEnd(',') + "]},";
                return result;
            }
        }
        #endregion

        #region Gossips
        private string Gossips
        {
            get
            {
                if (!CanEditGossips || OnlyEditItem == Visibility.Collapsed || gossipItems.Count == 0) return "";
                string result = "Gossips:[";
                result += string.Join("", gossipItems.Select(item => "{" + item.GossipData + "},"));
                result = result.TrimEnd(',') + "],";
                return result;
            }
        }
        #endregion

        #region Brain

        #region 聚集点
        private string meeting_pointX = "";
        public string MeetingPointX
        {
            set { meeting_pointX = value; OnPropertyChanged(); }
            get{ return meeting_pointX; }
        }
        private string meeting_pointY = "";
        public string MeetingPointY
        {
            set { meeting_pointY = value; OnPropertyChanged(); }
            get
            {
                return meeting_pointY;
            }
        }
        private string meeting_pointZ = "";
        public string MeetingPointZ
        {
            set { meeting_pointZ = value; OnPropertyChanged(); }
            get
            {
                return meeting_pointZ;
            }
        }
        private string meetingPointDimension = null;
        public string MeetingPointDimension
        {
            get { return meetingPointDimension; }
            set
            {
                meetingPointDimension = value;
                OnPropertyChanged();
            }
        }
        private string MeetingPointDimensionString
        {
            get
            {
                string DimensionId = DimensionDataBase.Where(item => item.Value == MeetingPointDimension).First().Key;
                return MeetingPointDimension.Trim() != "" ? "dimension:\"minecraft:" + DimensionId + "\"" : "";
            }
        }
        private string MeetingPoint
        {
            get
            {
                string result = "meeting_point:{";
                string pos = MeetingPointX.Trim() != "" && MeetingPointY.Trim() != "" && MeetingPointZ.Trim() != "" ? "pos:[" + MeetingPointX + "," + MeetingPointY + "," + MeetingPointZ + "]," : "";
                if(MeetingPointDimensionString != "" && pos != "")
                result += pos + MeetingPointDimensionString;
                if (result.Trim() == "meeting_point:{") return "";
                return result.TrimEnd(',') + "},";
            }
        }
        #endregion

        #region 床位置
        private string homeX = "";
        public string HomeX
        {
            set { homeX = value; OnPropertyChanged(); }
            get
            {
                return homeX;
            }
        }
        private string homeY = "";
        public string HomeY
        {
            set { homeY = value; OnPropertyChanged(); }
            get
            {
                return homeY;
            }
        }
        private string homeZ = "";
        public string HomeZ
        {
            set { homeZ = value; OnPropertyChanged(); }
            get
            {
                return homeZ;
            }
        }
        private string homeDimension = null;
        public string HomeDimension
        {
            get { return homeDimension; }
            set
            {
                homeDimension = value;
                OnPropertyChanged();
            }
        }
        private string HomeDimensionString
        {
            get
            {
                string DimensionId = DimensionDataBase.Where(item=>item.Value == HomeDimension).First().Key;
                return HomeDimension.Trim() != "" ? "dimension:\"minecraft:" + DimensionId + "\"" : "";
            }
        }
        private string Home
        {
            get
            {
                string result = "home:{";
                string pos = HomeX.Trim() != "" && HomeY.Trim() != "" && HomeZ.Trim() != "" ? "pos:[" + HomeX + "," + HomeY + "," + HomeZ + "]," : "";
                if(pos != "" && HomeDimensionString != "")
                result += pos + HomeDimensionString;
                if (result.Trim() == "home:{") return "";
                return result.TrimEnd(',') + "},";
            }
        }
        #endregion

        #region 工作站点
        private string job_siteX = "";
        public string JobSiteX
        {
            set { job_siteX = value; OnPropertyChanged(); }
            get
            {
                return job_siteX;
            }
        }
        private string job_siteY = "";
        public string JobSiteY
        {
            set { job_siteY = value; OnPropertyChanged(); }
            get
            {
                return job_siteY;
            }
        }
        private string job_siteZ = "";
        public string JobSiteZ
        {
            set { job_siteZ = value; OnPropertyChanged(); }
            get
            {
                return job_siteZ;
            }
        }
        private string jobSiteDimension = null;
        public string JobSiteDimension
        {
            get { return jobSiteDimension; }
            set
            {
                jobSiteDimension = value;
                OnPropertyChanged();
            }
        }
        private string JobSiteDimensionString
        {
            get
            {
                string DimensionId = DimensionDataBase.Where(item => item.Value == jobSiteDimension).First().Key;
                return JobSiteDimension.Trim() !=""? "dimension:\"minecraft:" + DimensionId + "\",":"";
            }
        }
        private string JobSite
        {
            get
            {
                string result = "job_site:{";
                string pos = JobSiteX.Trim() != "" && JobSiteY.Trim() !="" && JobSiteZ.Trim() != ""? "pos:[" + JobSiteX + "," + JobSiteY + "," + JobSiteZ + "],":"";
                if(pos != "" && JobSiteDimensionString != "")
                result += pos + JobSiteDimensionString;
                if (result.Trim() == "job_site:{") return "";
                return result.TrimEnd(',')+"},";
            }
        }
        #endregion

        #region 记忆
        private string Brain
        {
            get
            {
                if (!CanEditBrain || OnlyEditItem == Visibility.Collapsed) return "";
                string memoriesContent = MeetingPoint + Home + JobSite;
                string result = "Brain:{memories:{" + memoriesContent.TrimEnd(',') + "}},";
                if (result.Trim() == "Brain:{memories:{}},") return "";
                return result;
            }
        }
        #endregion

        #endregion

        #region 村民属性

        #region 数据源
        string VillagerTypeSourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Villager\\data\\VillagerTypes.ini";
        string VillagerProfessionsSourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Villager\\data\\VillagerProfessionTypes.ini";
        string VillagerLevelSourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Villager\\data\\VillagerLevels.ini";

        public Dictionary<string, string> VillagerTypeDataBase = new Dictionary<string, string> { };
        public Dictionary<string, string> VillagerProfessionTypeDataBase = new Dictionary<string, string> { };

        ObservableCollection<string> VillagerTypeSource = new ObservableCollection<string> { };
        ObservableCollection<string> VillagerProfessionTypeSource = new ObservableCollection<string> { };
        ObservableCollection<string> VillagerLevelSource = new ObservableCollection<string> { };
        #endregion

        #region 村民种类
        private string villagerType = "";
        public string VillagerType
        {
            get
            {
                return villagerType;
            }
            set
            {
                villagerType = value;
                OnPropertyChanged();
            }
        }
        private string VillagerTypeString
        {
            get
            {
                string result = "type:\"minecraft:";
                result = result + VillagerTypeDataBase.Where(item => item.Value == VillagerType).First().Key + "\",";
                return result;
            }
        }
        #endregion

        #region 村民职业
        private string villagerProfessionType = "";
        public string VillagerProfessionType
        {
            get { return villagerProfessionType; }
            set
            {
                villagerProfessionType = value;
                OnPropertyChanged();
            }
        }
        private string VillagerProfessionTypeString
        {
            get
            {
                string result = "profession:\"minecraft:";
                result = result + VillagerProfessionTypeDataBase.Where(item => item.Value == VillagerProfessionType).First().Key + "\",";
                return result;
            }
        }
        #endregion

        #region 村民交易等级
        private string villagerLevel = "0";
        public string VillagerLevel
        {
            get { return villagerLevel; }
            set
            {
                villagerLevel = value;
                OnPropertyChanged();
            }
        }
        private string VillagerLevelString
        {
            get
            {
                string result = "level:";
                result = result + VillagerLevel + ",";
                return result;
            }
        }

        #endregion

        #region 村民数据
        private string VillagerData
        {
            get
            {
                string result = "VillagerData:{";
                result += VillagerTypeString + VillagerProfessionTypeString + VillagerLevelString;
                result = result.TrimEnd(',') + "},";
                return result;
            }
        }
        #endregion

        #endregion

        #region 是否愿意交配
        private bool willing = false;
        public bool Willing
        {
            get { return willing; }
            set
            {
                willing = value;
                OnPropertyChanged();
            }
        }
        private string WillingString
        {
            get
            {
                return "Willing:" + (Willing ?1:0) + "b,";
            }
        }
        #endregion

        #region 此村民最后一次前往工作站点重新供应交易的刻
        private double lastRestock = 0;
        public double LastRestock
        {
            get { return lastRestock; }
            set { lastRestock = value; OnPropertyChanged(); }
        }
        private string LastRestockString
        {
            get
            {
                return LastRestock.ToString().Trim() != ""? "LastRestock:" +LastRestock+",":"";
            }
        }
        #endregion

        #region 此村民当前的经验值
        private double xp = 1;
        public double Xp
        {
            get { return xp; }
            set { xp = value; OnPropertyChanged(); }
        }
        private string XpString
        {
            get
            {
                return Xp.ToString().Trim() != "" ? "Xp:" + Xp + "," : "";
            }
        }
        #endregion

        #endregion

        #region 黑白画刷
        private SolidColorBrush whiteBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        private SolidColorBrush blackBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
        #endregion

        #region 添加与清空交易项
        public RelayCommand AddTransactionItem { get; set; }
        public RelayCommand ClearTransactionItem { get; set; }
        #endregion

        #region 添加与清空言论
        public RelayCommand AddGossipItem { get; set; }
        public RelayCommand ClearGossipItem { get; set; }
        #endregion

        #region 字段与引用
        //言论搜索目标引用
        TextBox GossipSearchTarget = null;
        //言论搜索类型引用
        ComboBox GossipSearchType = null;
        //言论数据源所在视图引用
        ScrollViewer GossipViewer = null;
        //原版物品库
        public ObservableCollection<ItemStructure> BagItems { get; set; } = new ObservableCollection<ItemStructure>();
        public ObservableCollection<ItemStructure> CustomItems { get; set; } = new ObservableCollection<ItemStructure>();
        //物品描述引用
        public ObservableCollection<string> BagItemToolTips { get; set; } = new ObservableCollection<string> { };
        //言论搜索类型数据源
        ObservableCollection<string> gossipSearchType = new ObservableCollection<string> { };
        //言论搜索类型配置文件路径
        string gossipSearchTypeFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Villager\\data\\GossipSearchTypes.ini";
        //维度数据源
        ObservableCollection<string> DimensionTypeSource = new ObservableCollection<string> { };
        //维度数据源配置文件路径
        string dimensionTypeFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Villager\\data\\DimensionTypes.ini";
        //维度类型数据库
        Dictionary<string, string> DimensionDataBase = new Dictionary<string, string> { };
        //物品加载进程锁
        object itemLoadLock = new object();

        /// <summary>
        /// 对象数据源
        /// </summary>
        private CollectionViewSource BagViewSource = null;

        /// <summary>
        /// 表示是否已订阅搜索事件
        /// </summary>
        bool GotFocused = false;

        //背包引用
        ListView Bag = null;
        ListView CustomBag = null;
        #endregion

        public villager_datacontext()
        {
            #region 链接指令
            RunCommand = new RelayCommand(run_command);
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            AddTransactionItem = new RelayCommand(AddTransactionItemCommand);
            ClearTransactionItem = new RelayCommand(ClearTransactionItemCommand);
            AddGossipItem = new RelayCommand(AddGossipItemCommand);
            ClearGossipItem = new RelayCommand(ClearGossipItemCommand);
            Save = new RelayCommand(SaveCommand);
            ImportFromFile = new RelayCommand(ImportFromFileCommand);
            ImportFromClipboard = new RelayCommand(ImportFromClipboardCommand);
            #endregion

            #region 把交易数据页放入容器中，用于定位出现位置
            popup.Child = transactionItemDataForm;
            popup.Placement = PlacementMode.Mouse;
            popup.PlacementTarget = CurrentItem;
            #endregion

            #region 异步载入原版物品序列
            BindingOperations.EnableCollectionSynchronization(BagItems, itemLoadLock);
            //加载物品集合
            Task.Run(() =>
            {
                lock (itemLoadLock)
                {
                    string uriDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
                    string urlPath = "";
                    foreach (var item in MainWindow.ItemIdSource)
                    {
                        urlPath = uriDirectoryPath + item.ComboBoxItemId + ".png";
                        if (File.Exists(urlPath))
                            BagItems.Add(new ItemStructure(new Uri(urlPath, UriKind.Absolute), item.ComboBoxItemId + ":" + item.ComboBoxItemText,"{id:\"minecraft:"+item.ComboBoxItemId+"\",Count:1b}"));
                    }
                }
            });
            #endregion

            #region 异步载入自定义物品序列
            BindingOperations.EnableCollectionSynchronization(CustomItems, itemLoadLock);
            //加载物品集合
            Task.Run(() =>
            {
                lock (itemLoadLock)
                {
                    string uriDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\saves\\Item\\";
                    string[] itemFileList = Directory.GetFiles(uriDirectoryPath);
                    foreach (var item in itemFileList)
                    {
                        if (File.Exists(item))
                        {
                            string nbt = ExternalDataImportManager.GetItemDataHandler(item);
                            if (nbt.Length > 0)
                            {
                                JObject data = JObject.Parse(nbt);
                                JToken id = data.SelectToken("id");
                                if (id == null) continue;
                                string itemID = id.ToString().Replace("\"","").Replace("minecraft:","");
                                string iconPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + itemID + ".png";
                                string itemName = MainWindow.ItemIdSource.Where(item => item.ComboBoxItemId == itemID).Select(item => item.ComboBoxItemText).First();
                                CustomItems.Add(new ItemStructure(new Uri(iconPath, UriKind.Absolute), itemID + ":" + itemName, nbt));
                            }
                        }
                    }
                }
            });
            #endregion
        }

        /// <summary>
        /// 从文件导入
        /// </summary>
        private void ImportFromFileCommand()
        {
            Microsoft.Win32.OpenFileDialog dialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                RestoreDirectory = true,
                DefaultExt = ".command",
                Multiselect = false,
                Title = "请选择一个Minecraft村民数据文件"
            };
            if (dialog.ShowDialog().Value && File.Exists(dialog.FileName))
                ExternalDataImportManager.ImportVillagerDataHandler(dialog.FileName, this);
        }

        /// <summary>
        /// 从剪切板导入
        /// </summary>
        private void ImportFromClipboardCommand()
        {
            ExternalDataImportManager.ImportVillagerDataHandler(Clipboard.GetText(),this,false);
        }

        /// <summary>
        /// 保存村民
        /// </summary>
        private void SaveCommand()
        {
            run_command(false);
            Microsoft.Win32.SaveFileDialog saveFileDialog = new()
            {
                Title = "请选择村民保存路径",
                AddExtension = true,
                DefaultExt = ".command",
                Filter = "Command文件|*.command;",
                CheckPathExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                RestoreDirectory = true
            };
            if(saveFileDialog.ShowDialog().Value)
            {
                _ = File.WriteAllTextAsync(saveFileDialog.FileName, Result);
                _ = File.WriteAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + "resources\\saves\\Villager\\" + Path.GetFileName(saveFileDialog.FileName), Result);
            }
        }

        /// <summary>
        /// 获取数据源引用，订阅过滤事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(!GotFocused)
            {
                TextBox textBox = sender as TextBox;
                Window parent = Window.GetWindow(textBox);
                BagViewSource = parent.FindResource("BagItemSource") as CollectionViewSource;
                BagViewSource.Filter += CollectionViewSource_Filter;
                GotFocused = true;
            }
        }

        /// <summary>
        /// 过滤与搜索内容不相关的成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (SearchText.Trim().Length > 0)
            {
                e.Accepted = false;
                ItemStructure itemStructure = e.Item as ItemStructure;
                string currentItemID = Path.GetFileNameWithoutExtension(itemStructure.ImagePath.ToString());
                string IDAndName = itemStructure.IDAndName;

                if ((currentItemID.Contains(SearchText) && IDAndName.Contains(SearchText)) || (IDAndName.Contains(SearchText) && IDAndName.StartsWith(currentItemID)))
                    e.Accepted = true;
            }
            else
                e.Accepted = true;
        }

        public void VersionLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.ItemsSource = VersionSource;
        }

        /// <summary>
        /// 载入村民种类数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VillagerTypeLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            if(File.Exists(VillagerTypeSourceFilePath))
            {
                string[] data = File.ReadAllLines(VillagerTypeSourceFilePath);
                for (int i = 0; i < data.Length; i++)
                {
                    string[] item = data[i].Split('.');
                    string id = item[0];
                    string name = item[1];

                    if(!VillagerTypeDataBase.ContainsKey(id))
                    VillagerTypeDataBase.Add(id,name);

                    VillagerTypeSource.Add(name);
                }
                box.ItemsSource = VillagerTypeSource;
                box.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 载入村民职业种类数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VillagerProfessionTypeLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            if (File.Exists(VillagerProfessionsSourceFilePath))
            {
                string[] data = File.ReadAllLines(VillagerProfessionsSourceFilePath);
                for (int i = 0; i < data.Length; i++)
                {
                    string[] item = data[i].Split('.');
                    string id = item[0];
                    string name = item[1];

                    if(!VillagerProfessionTypeDataBase.ContainsKey(id))
                    VillagerProfessionTypeDataBase.Add(id, name);

                    VillagerProfessionTypeSource.Add(name);
                }
                box.ItemsSource = VillagerProfessionTypeSource;
                box.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 载入村民等级数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VillagerLevelLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            if (File.Exists(VillagerLevelSourceFilePath))
            {
                int level = int.Parse(File.ReadAllText(VillagerLevelSourceFilePath));
                for (int i = 1; i <= level; i++)
                {
                    VillagerLevelSource.Add(i.ToString());
                }
                box.ItemsSource = VillagerLevelSource;
                box.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 载入维度种类数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DimensionTypeLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            if (DimensionTypeSource.Count == 0)
            {
                if (File.Exists(dimensionTypeFilePath))
                {
                    string[] data = File.ReadAllLines(dimensionTypeFilePath);
                    for (int i = 0; i < data.Length; i++)
                    {
                        string[] item = data[i].Split('.');
                        string id = item[0];
                        string name = item[1];

                        if (!DimensionDataBase.ContainsKey(id))
                            DimensionDataBase.Add(id, name);

                        DimensionTypeSource.Add(name);
                    }
                }
            }
            box.ItemsSource = DimensionTypeSource;
        }

        /// <summary>
        /// 获取言论所在的视图引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GossipViewerLoaded(object sender, RoutedEventArgs e)
        {
            GossipViewer = sender as ScrollViewer;
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            Villager.cbhk.Topmost = true;
            Villager.cbhk.WindowState = WindowState.Normal;
            Villager.cbhk.Show();
            Villager.cbhk.Topmost = false;
            Villager.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 为保存行为执行生成
        /// </summary>
        /// <param name="showResult"></param>
        private void run_command(bool showResult)
        {
            Result = "";
            Result += WillingString + VillagerData + Offers + Gossips + Brain + LastRestockString + XpString;
            Result = "/summon villager ~ ~1 ~ {" + Result.TrimEnd(',') + "}";

            if (showResult)
            {
                GenerateResultDisplayer.Displayer displayer = GenerateResultDisplayer.Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "村民", icon_path);
                displayer.Show();
            }
            else
                Clipboard.SetText(Result);
        }

        /// <summary>
        /// 执行生成
        /// </summary>
        private void run_command()
        {
            Result = "";
            Result += WillingString + VillagerData + Offers + Gossips + Brain + LastRestockString + XpString;
            Result = "/summon villager ~ ~1 ~ {" + Result.TrimEnd(',') + "}";

            if (ShowResult)
            {
                GenerateResultDisplayer.Displayer displayer = GenerateResultDisplayer.Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "村民", icon_path);
                displayer.Show();
            }
            else
                Clipboard.SetText(Result);
        }

        /// <summary>
        /// 同步物品数据值
        /// </summary>
        private void ValueBinder()
        {
            #region 属性绑定器
            Binding rewardExpBinder = new Binding()
            {
                Path = new PropertyPath("RewardExp"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = false
            };
            Binding maxUsesBinder = new Binding()
            {
                Path = new PropertyPath("MaxUses"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            Binding usesBinder = new Binding()
            {
                Path = new PropertyPath("Uses"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            Binding xpBinder = new Binding()
            {
                Path = new PropertyPath("Xp"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            Binding demandBinder = new Binding()
            {
                Path = new PropertyPath("Demand"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            Binding specialPriceBinder = new Binding()
            {
                Path = new PropertyPath("SpecialPrice"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            Binding priceMultiplierBinder = new Binding()
            {
                Path = new PropertyPath("PriceMultiplier"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            #endregion

            BindingOperations.SetBinding(transactionItemDataForm.rewardExp, ToggleButton.IsCheckedProperty, rewardExpBinder);
            BindingOperations.SetBinding(transactionItemDataForm.maxUses, RangeBase.ValueProperty, maxUsesBinder);
            BindingOperations.SetBinding(transactionItemDataForm.uses, RangeBase.ValueProperty, usesBinder);
            BindingOperations.SetBinding(transactionItemDataForm.xp, RangeBase.ValueProperty, xpBinder);
            BindingOperations.SetBinding(transactionItemDataForm.demand, RangeBase.ValueProperty, demandBinder);
            BindingOperations.SetBinding(transactionItemDataForm.specialPrice, RangeBase.ValueProperty, specialPriceBinder);
            BindingOperations.SetBinding(transactionItemDataForm.priceMultiplier, RangeBase.ValueProperty, priceMultiplierBinder);
        }

        /// <summary>
        /// 添加一个交易项控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddTransactionItemCommand()
        {
            TransactionItems transaction = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            transaction.MouseLeftButtonDown += BuyItemDataUpdater;
            transactionItems.Add(transaction);
        }

        /// <summary>
        /// 清空交易项控件
        /// </summary>
        private void ClearTransactionItemCommand()
        {
            transactionItems.Clear();
            popup.IsOpen = false;
        }

        /// <summary>
        /// 弹出物品数据更新窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyItemDataUpdater(object sender, MouseButtonEventArgs e)
        {
            TransactionItems item = sender as TransactionItems;
            IsDisplayItemInfoWindow = !IsDisplayItemInfoWindow;
            if (CurrentItem != item && CurrentItem != null)
                IsDisplayItemInfoWindow = true;
            CurrentItem = item;

            //绑定交易项数据
            transactionItemDataForm.DataContext = CurrentItem;
            ValueBinder();

            if (IsDisplayItemInfoWindow)
            {
                popup.IsOpen = true;
                item.border.BorderBrush = whiteBrush;
            }
            else
            {
                popup.IsOpen = false;
                item.border.BorderBrush = blackBrush;
            }
        }

        /// <summary>
        /// 添加一个言论控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddGossipItemCommand()
        {
            GossipsItems gossipsItem = new GossipsItems()
            {
                Width = 315,
                Margin = new Thickness(0, 0, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            gossipsItem.MouseLeftButtonDown += GossipsItemMouseLeftButtonDown;
            gossipItems.Add(gossipsItem);
        }

        /// <summary>
        /// 清空言论控件
        /// </summary>
        private void ClearGossipItemCommand()
        {
            gossipItems.Clear();
        }

        /// <summary>
        /// 载入物品库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ItemsLoaded(object sender, RoutedEventArgs e)
        {
            Bag = ((sender as TabControl).Items[0] as TextTabItems).Content as ListView;
            CustomBag = ((sender as TabControl).Items[1] as TextTabItems).Content as ListView;

            Bag.DataContext = this;
            CustomBag.DataContext = this;

            Bag.PreviewMouseLeftButtonDown += SelectItemClickDown;
            Bag.MouseMove += Bag_MouseMove;
            Bag.MouseLeave += ListBox_MouseLeave;

            CustomBag.PreviewMouseLeftButtonDown += SelectItemClickDown;
            CustomBag.MouseMove += Bag_MouseMove;
            CustomBag.MouseLeave += ListBox_MouseLeave;
        }

        /// <summary>
        /// 获取言论搜索目标的引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GossipSearchTargetLoaded(object sender, RoutedEventArgs e)
        {
            GossipSearchTarget = sender as TextBox;
        }

        /// <summary>
        /// 获取言论搜索类型的引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GossipSearchTypeLoaded(object sender, RoutedEventArgs e)
        {
            GossipSearchType = sender as ComboBox;
            if(File.Exists(gossipSearchTypeFilePath))
            {
                string[] types = File.ReadAllLines(gossipSearchTypeFilePath);
                for (int i = 0; i < types.Length; i++)
                {
                    gossipSearchType.Add(types[i]);
                }
            }
            GossipSearchType.ItemsSource = gossipSearchType;
            GossipSearchType.SelectedIndex = 0;
        }

        /// <summary>
        /// 查找目标言论并更新价格数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SearchGossipsTextChanged(object sender, TextChangedEventArgs e)
        {
            if(CanEditGossips)
            {
                string current_type = GossipSearchType.SelectedItem as string;
                List<GossipsItems> target_gossip = gossipItems.Where(gossip =>
                {
                    string type = gossip.Type.SelectedItem as string;
                    if (gossip.Target.Text == GossipSearchTarget.Text.Trim() && type == current_type)
                        return true;
                    else
                        return false;
                }).ToList();

                if (target_gossip.Count > 0)
                    GeneralTools.ScrollToSomeWhere.Scroll(target_gossip[0], GossipViewer);
            }
        }

        /// <summary>
        /// 左击言论成员后计算左侧所有物品的价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GossipsItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GossipsItems current_item = sender as GossipsItems;

            string current_type = current_item.Type.SelectedItem.ToString();

            if (current_type != "minor_negative" && current_type != "trading")
                return;

            string search_type = current_type == "minor_negative" ? "trading" : current_type;

            string compare_type = "";

            List<GossipsItems> another_items = gossipItems.Where(item =>
            {
                compare_type = item.Type.SelectedItem as string;
                if (item.Target.Text.Trim() == current_item.Target.Text.Trim() && search_type == compare_type)
                    return true;
                else
                    return false;
            }).ToList();

            if(another_items.Count > 0)
            if(another_items.First().Value.Value.ToString().Trim() != "" && current_item.Value.Value.ToString().Trim() != "")
            {
                int minor_negative = current_type == "minor_negative" ? int.Parse(another_items.First().Value.Value.ToString()) : int.Parse(current_item.Value.Value.ToString());
                int trading = current_type != "trading" ? int.Parse(another_items.First().Value.Value.ToString()) : int.Parse(current_item.Value.Value.ToString());
                transactionItems.All(item => { item.UpdateDiscountData(minor_negative, trading); return true; });
            }
        }

        /// <summary>
        /// 鼠标移入背包后选中对应的成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Bag_MouseMove(object sender, MouseEventArgs e)
        {
            ListView listView = sender as ListView;
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(listView, Mouse.GetPosition(listView));

            if (hitTestResult != null)
            {
                var item = hitTestResult.VisualHit;

                while (item != null && item is not ListViewItem)
                    item = VisualTreeHelper.GetParent(item);

                if (item != null)
                {
                    int i = listView.Items.IndexOf(((ListViewItem)item).DataContext);
                    if (i >= 0 && i < listView.Items.Count)
                        listView.SelectedIndex = i;
                }
            }
        }

        public void ListBox_MouseLeave(object sender, MouseEventArgs e)
        {
            SelectedItem = null;
        }

        /// <summary>
        /// 处理开始拖拽物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelectItemClickDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Image)
                SelectedItem = null;
            if (SelectedItem != null)
            {
                IsGrabingItem = true;

                DependencyObject obj = (DependencyObject)e.OriginalSource;
                while (obj != null && obj is not ListViewItem)
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }
                ListViewItem item = obj as ListViewItem;
                ItemStructure itemStructure = item.Content as ItemStructure;

                drag_source = new Image
                {
                    Source = new BitmapImage(SelectedItem.ImagePath),
                    Tag = itemStructure
                };
                GrabedImage = drag_source;

                if (IsGrabingItem && drag_source != null)
                {
                    DataObject dataObject = new(typeof(Image), GrabedImage);
                    if (dataObject != null)
                        DragDrop.DoDragDrop(drag_source, dataObject, DragDropEffects.Move);
                    IsGrabingItem = false;
                }
            }
        }
    }
}

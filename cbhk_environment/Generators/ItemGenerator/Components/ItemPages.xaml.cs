using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.GenerateResultDisplayer;
using cbhk_environment.Generators.ItemGenerator.Components.SpecialNBT;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// ItemPages.xaml 的交互逻辑
    /// </summary>
    public partial class ItemPages : UserControl
    {
        #region 返回、运行和保存等指令
        public RelayCommand Run { get; set; }
        public RelayCommand Save { get; set; }
        public RelayCommand ClearUnnecessaryData { get; set; }
        #endregion

        #region 已选择的版本
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
            }
        }
        private ObservableCollection<string> VersionSource = new ObservableCollection<string> { "1.12-", "1.13+" };
        #endregion

        #region 保存物品ID
        private IconComboBoxItem selected_item_id;
        public IconComboBoxItem SelectedItemId
        {
            get { return selected_item_id; }
            set
            {
                selected_item_id = value;
                UpdateUILayOut();
            }
        }

        public string SelectedItemIdString
        {
            get
            {
                string key = MainWindow.ItemDataBase.Where(item => Regex.Match(item.Key, @"[\u4e00-\u9fa5]+").ToString() == SelectedItemId.ComboBoxItemText).First().Key;
                string result = key != "" ? Regex.Match(key, @"[a-zA-Z_]+").ToString() : "";
                return result;
            }
        }
        #endregion

        #region 版本
        private bool behavior_lock = true;
        private bool version1_12 = false;
        public bool Version1_12
        {
            get { return version1_12; }
            set
            {
                version1_12 = value;
                if (behavior_lock)
                {
                    behavior_lock = false;
                    Version1_13 = !version1_12;
                    behavior_lock = true;
                }
            }
        }
        private bool version1_13 = true;
        public bool Version1_13
        {
            get { return version1_13; }
            set
            {
                version1_13 = value;
                if (behavior_lock)
                {
                    behavior_lock = false;
                    Version1_12 = !version1_13;
                    behavior_lock = true;
                }
            }
        }
        #endregion

        #region 显示结果
        private bool showGeneratorResult = false;
        public bool ShowGeneratorResult
        {
            get
            {
                return showGeneratorResult;
            }
            set
            {
                showGeneratorResult = value;
            }
        }
        #endregion

        #region 生成方式
        bool generatorLock = true;
        #region 召唤
        private bool summon = false;
        public bool Summon
        {
            get { return summon; }
            set
            {
                summon = value;
                if (generatorLock)
                {
                    generatorLock = false;
                    GiveMode.IsChecked = !summon;
                    generatorLock = true;
                }
            }
        }
        #endregion
        #region 怪物蛋
        private bool give = true;
        public bool Give
        {
            get { return give; }
            set
            {
                give = value;
                if (generatorLock)
                {
                    generatorLock = false;
                    SummonMode.IsChecked = !give;
                    generatorLock = true;
                }
            }
        }
        #endregion
        #endregion

        #region 是否作为工具
        public bool UseForTool { get; set; } = false;
        #endregion

        #region 字段与引用
        string SpecialNBTStructureFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\SpecialTags.json";
        //特指结果集合
        public Dictionary<string, ObservableCollection<NBTDataStructure>> SpecialTagsResult { get; set; } = new();
        //特殊实体特指标签字典,用于动态切换内容
        Dictionary<string, Grid> specialDataDictionary = new();
        public ObservableCollection<IconComboBoxItem> ItemIds { get; set; } = new();
        //白色画刷
        SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        //黑色画刷
        SolidColorBrush blackBrush = new((Color)ColorConverter.ConvertFromString("#000000"));
        //橙色画刷
        SolidColorBrush orangeBrush = new((Color)ColorConverter.ConvertFromString("#FFE5B663"));
        //存储外部读取进来的实体数据
        public JObject ExternallyReadEntityData { get; set; } = null;
        string itemImageFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
        string itemImageFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
        string buttonNormalImage = "pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonNormal.png";
        string buttonPressedImage = "pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonPressed.png";
        ImageBrush buttonNormalBrush;
        ImageBrush buttonPressedBrush;
        #endregion

        #region 是否同步到文件
        public bool SyncToFile { get; set; }
        public string ExternFilePath { get; set; }
        #endregion

        #region 标记当前是否为导入外部数据的模式
        private bool importMode = false;
        public bool ImportMode
        {
            get
            {
                return importMode;
            }
            set
            {
                importMode = value;

            }
        }
        #endregion

        //最终结果
        public string Result { get; set; }

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconItems.png";
        public ItemPages()
        {
            InitializeComponent();
            DataContext = this;

            #region 连接指令
            Run = new RelayCommand(run_command);
            Save = new RelayCommand(SaveCommand);
            ClearUnnecessaryData = new RelayCommand(ClearUnnecessaryDataCommand);
            #endregion

            #region 初始化字段
            buttonNormalBrush = new ImageBrush(new BitmapImage(new Uri(buttonNormalImage, UriKind.RelativeOrAbsolute)));
            buttonPressedBrush = new ImageBrush(new BitmapImage(new Uri(buttonPressedImage, UriKind.RelativeOrAbsolute)));
            string SpecialData = File.ReadAllText(SpecialNBTStructureFilePath);
            JArray specialArray = JArray.Parse(SpecialData);
            ItemIds = MainWindow.ItemIdSource;
            SelectedItemId = ItemIds[0];
            #endregion
        }

        /// <summary>
        /// 最终结算
        /// </summary>
        /// <param name="MultipleMode"></param>
        private void FinalSettlement(object MultipleOrExtern)
        {
            StringBuilder nbt = new();
            if (SpecialTagsResult.Count > 0)
            {
                ObservableCollection<NBTDataStructure> SpecialData = SpecialTagsResult[SelectedItemIdString];
                nbt.Append(string.Join(',', SpecialData.Select(item => item.Result)));
            }
            nbt.Append(Common.Result + Function.Result + Data.Result);
            while (nbt.ToString().StartsWith(','))
                nbt.Remove(0, 1);
            while (nbt.ToString().EndsWith(','))
                nbt.Remove(nbt.ToString().Length - 1, 1);
            if (nbt.ToString().Length > 0)
            {
                nbt.Insert(0, '{');
                nbt.Append('}');
            }
            if (SelectedVersion == "1.12-")
                Result = "give @p " + SelectedItemIdString + " " + Data.ItemCount.Value + " " + Data.ItemDamage.Value + " " + nbt;
            else
                Result = "give @p " + SelectedItemIdString + nbt + " " + Data.ItemCount.Value;

            if (bool.Parse(MultipleOrExtern.ToString()))
            {
                Displayer displayer = Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "实体", icon_path);
                displayer.Topmost = true;
                displayer.Show();
                displayer.Topmost = false;
            }
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="MultipleMode"></param>
        public void GetResult(bool MultipleOrExtern = false)
        {
            Thread settlement = new(new ParameterizedThreadStart(FinalSettlement));
            settlement.Start(MultipleOrExtern);
        }

        /// <summary>
        /// 保存指令
        /// </summary>
        private void SaveCommand()
        {
            //执行生成
            FinalSettlement(false);
            Microsoft.Win32.SaveFileDialog saveFileDialog = new()
            {
                AddExtension = true,
                RestoreDirectory = true,
                CheckPathExists = true,
                DefaultExt = "command",
                Filter = "Command files (*.command;)|*.command;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Title = "保存为命令文件"
            };
            if (saveFileDialog.ShowDialog().Value && Directory.Exists(Path.GetDirectoryName(saveFileDialog.FileName)))
                File.WriteAllText(saveFileDialog.FileName, Result);
        }

        /// <summary>
        /// 执行生成
        /// </summary>
        private void run_command()
        {
            StringBuilder nbt = new();
            if(SpecialTagsResult.Count > 0)
            {
                ObservableCollection<NBTDataStructure> SpecialData = SpecialTagsResult[SelectedItemIdString];
                nbt.Append(string.Join(',', SpecialData.Select(item => item.Result)));
            }
            nbt.Append(Common.Result + Function.Result + Data.Result + (Summon && SelectedVersion == "1.13+" && Data.ItemDamage.Value > 0 ? "Damage:" + Data.ItemDamage.Value: ""));
            while (nbt.ToString().StartsWith(','))
                nbt.Remove(0, 1);
            while (nbt.ToString().EndsWith(','))
                nbt.Remove(nbt.ToString().Length - 1, 1);
            if (Summon)
            {
                nbt.Insert(0,"id:\"minecraft:"+ SelectedItemIdString + "\",tag:{");
                nbt.Append("},Count:" + Data.ItemCount.Value + "b");
            }
            if (nbt.ToString().Length > 0)
            {
                nbt.Insert(0, '{');
                nbt.Append('}');
            }
            if (UseForTool)
            {
                Result = nbt.ToString();
                Item item = Window.GetWindow(this) as Item;
                item.DialogResult = true;
                return;
            }
            if (Give)
            {
                if (SelectedVersion == "1.12-")
                    Result = "give @p " + SelectedItemIdString + " " + Data.ItemCount.Value + " " + Data.ItemDamage.Value + " " + nbt;
                else
                    Result = "give @p " + SelectedItemIdString + nbt + " " + Data.ItemCount.Value;
            }
            else
                Result = "summon item" + " ~ ~ ~ {Item:" + nbt + "}";

            if (ShowGeneratorResult)
            {
                Displayer displayer = Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "物品", icon_path);
                displayer.Show();
            }
            else
                Clipboard.SetText(Result);
        }

        public string run_command(bool showResult)
        {
            StringBuilder nbt = new();
            if (SpecialTagsResult.Count > 0)
            {
                ObservableCollection<NBTDataStructure> SpecialData = SpecialTagsResult[SelectedItemIdString];
                nbt.Append(string.Join(',', SpecialData.Select(item => item.Result)));
            }
            nbt.Append(Common.Result + Function.Result + Data.Result + (Summon && SelectedVersion == "1.13+" && Data.ItemDamage.Value > 0 ? "Damage:" + Data.ItemDamage.Value : ""));
            while (nbt.ToString().StartsWith(','))
                nbt.Remove(0, 1);
            while (nbt.ToString().EndsWith(','))
                nbt.Remove(nbt.ToString().Length - 1, 1);
            if (Summon)
            {
                nbt.Insert(0, "id:\"minecraft:" + SelectedItemIdString + "\",tag:{");
                nbt.Append("},Count:" + Data.ItemCount.Value + "b");
            }
            if (nbt.ToString().Length > 0)
            {
                nbt.Insert(0, '{');
                nbt.Append('}');
            }
            if (Give)
            {
                if (SelectedVersion == "1.12-")
                    Result = "give @p " + SelectedItemIdString + " " + Data.ItemCount.Value + " " + Data.ItemDamage.Value + " " + nbt;
                else
                    Result = "give @p " + SelectedItemIdString + nbt + " " + Data.ItemCount.Value;
            }
            else
                Result = "summon item" + " ~ ~ ~ {Item:" + nbt + "}";

            if (showResult)
            {
                Displayer displayer = Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "物品", icon_path);
                displayer.Show();
            }
            else
                Clipboard.SetText(Result);
            return Result;
        }

        /// <summary>
        /// 清除不需要的特指数据
        /// </summary>
        private void ClearUnnecessaryDataCommand()
        {
            if (specialDataDictionary.ContainsKey(SelectedItemIdString))
            {
                Grid grid = specialDataDictionary[SelectedItemIdString];
                specialDataDictionary.Clear();
                specialDataDictionary.Add(SelectedItemIdString, grid);
            }
        }

        /// <summary>
        /// 载入版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VersionLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.ItemsSource = VersionSource;
        }

        /// <summary>
        /// 载入物品id列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ItemIdsLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            comboBoxs.ItemsSource = MainWindow.ItemIdSource;
        }

        /// <summary>
        /// 根据需求构造控件
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        private List<FrameworkElement> ComponentsGenerator(ComponentData Request)
        {
            List<FrameworkElement> result = new();
            TextBlock displayText = new()
            {
                Uid = Request.nbtType,
                Text = Request.description.TrimEnd('。').TrimEnd('.'),
                Foreground = whiteBrush,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (Request.toolTip.Length > 0)
            {
                displayText.ToolTip = Request.toolTip;
                ToolTipService.SetInitialShowDelay(displayText, 0);
                ToolTipService.SetBetweenShowDelay(displayText, 0);
            }

            result.Add(displayText);
            ComponentEvents componentEvents = new();
            switch (Request.dataType)
            {
                case "TAG_List":
                    {
                        if (Request.dependency != null && Request.dependency.Length > 0)
                        {
                            switch (Request.dependency)
                            {
                                case "InlineItems":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = "Items",
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(10, 2, 10, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            JToken data = ExternallyReadEntityData.SelectToken(key);
                                            if (data != null)
                                            {
                                                JArray Items = JArray.Parse(data.ToString());
                                                string imagePath = "";
                                                for (int i = 0; i < Items.Count; i++)
                                                {
                                                    string itemID = JObject.Parse(Items[i].ToString())["id"].ToString();
                                                    Image image = new() { Tag = new NBTDataStructure() { Result = Items[i].ToString(), Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                                                    imagePath = itemImageFilePath + itemID + ".png";
                                                    if (File.Exists(imagePath))
                                                        image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                                                    InlineItems itemBag = new();
                                                    (itemBag.DisplayItem.Child as Image).Source = image.Source;
                                                    itemPanel.Children.Add(itemBag);
                                                }
                                                itemAccordion.Focus();
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "StoredEnchantments":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new();
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            JArray data = ExternallyReadEntityData.SelectToken(key) as JArray;
                                            if (data != null)
                                            {
                                                foreach (JObject item in data.Cast<JObject>())
                                                {
                                                    EnchantmentItems enchantment = new();
                                                    JToken idObj = item["id"];
                                                    JToken lvlObj = item["lvl"];
                                                    if (idObj != null)
                                                    {
                                                        string idString = idObj.ToString();
                                                        if(idString.Contains(':'))
                                                        idString = idString[(idString.IndexOf(':') + 1)..];
                                                        string id = MainWindow.EnchantmentDataBase[idString];
                                                        enchantment.Id.SelectedIndex = MainWindow.EnchantmentIdSource.IndexOf(Regex.Replace(id, @"\d+", ""));
                                                    }
                                                    if(lvlObj != null)
                                                    enchantment.Level.Value = double.Parse(lvlObj.ToString());
                                                    itemPanel.Children.Add(enchantment);
                                                }
                                                componentEvents.StoredEnchantments_LostFocus(itemAccordion,null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "NameSpaceReference":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(10, 2, 10, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray data)
                                            {
                                                foreach (JValue item in data.Cast<JValue>())
                                                {
                                                    NameSpaceReference nameSpaceReference = new();
                                                    nameSpaceReference.ReferenceBox.Text = item.Value<string>();
                                                    itemPanel.Children.Add(nameSpaceReference);
                                                }
                                                componentEvents.NameSpaceReference_LostFocus(itemAccordion,null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "MapDecorations":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new();
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray data)
                                            {
                                                foreach (JObject item in data.Cast<JObject>())
                                                {
                                                    MapDecorations mapDecorations = new();
                                                    JToken idObj = item["id"];
                                                    JToken typeObj = item["type"];
                                                    JToken xObj = item["x"];
                                                    JToken zObj = item["z"];
                                                    JToken rotObj = item["rot"];
                                                    if (idObj != null)
                                                        mapDecorations.Uid = idObj.ToString();
                                                    if (typeObj != null)
                                                        mapDecorations.type.SelectedIndex = byte.Parse(typeObj.ToString());
                                                    if (xObj != null)
                                                        mapDecorations.pos.number0.Value = double.Parse(xObj.ToString());
                                                    if (zObj != null)
                                                        mapDecorations.pos.number2.Value = double.Parse(zObj.ToString());
                                                    if (rotObj != null)
                                                        mapDecorations.rot.Value = double.Parse(rotObj.ToString());
                                                    itemPanel.Children.Add(mapDecorations);
                                                }
                                                componentEvents.MapDecorations_LostFocus(itemAccordion,null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "CustomPotionEffectList":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new();
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray Effects)
                                            {
                                                for (int i = 0; i < Effects.Count; i++)
                                                {
                                                    componentEvents.AddCustomPotionEffectCommand(itemAccordion);
                                                    CustomPotionEffects customPotionEffects = itemPanel.Children[i] as CustomPotionEffects;
                                                    StackPanel contentPanel = customPotionEffects.EffectListPanel;
                                                    #region 提取数据
                                                    JToken Ambient = Effects[i]["Ambient"];
                                                    JToken Amplifier = Effects[i]["Amplifier"];
                                                    JToken Duration = Effects[i]["Duration"];
                                                    JToken Id = Effects[i]["Id"];
                                                    JToken ShowIcon = Effects[i]["ShowIcon"];
                                                    JToken ShowParticles = Effects[i]["ShowParticles"];
                                                    JToken effect_changed_timestamp = Effects[i].SelectToken("FactorCalculationData.effect_changed_timestamp");
                                                    JToken factor_current = Effects[i].SelectToken("FactorCalculationData.factor_current");
                                                    JToken factor_previous_frame = Effects[i].SelectToken("FactorCalculationData.factor_previous_frame");
                                                    JToken factor_start = Effects[i].SelectToken("FactorCalculationData.factor_start");
                                                    JToken factor_target = Effects[i].SelectToken("FactorCalculationData.factor_target");
                                                    JToken had_effect_last_tick = Effects[i].SelectToken("FactorCalculationData.had_effect_last_tick");
                                                    JToken padding_duration = Effects[i].SelectToken("FactorCalculationData.padding_duration");
                                                    #endregion
                                                    #region 应用数据
                                                    if (Ambient != null)
                                                        contentPanel.FindChild<TextCheckBoxs>("Ambient").IsChecked = Ambient.ToString() == "1";
                                                    if (Amplifier != null)
                                                        contentPanel.FindChild<Slider>("Amplifier").Value = byte.Parse(Amplifier.ToString());
                                                    if (Duration != null)
                                                        contentPanel.FindChild<Slider>("Duration").Value = int.Parse(Duration.ToString());
                                                    if (Id != null)
                                                    {
                                                        string id = Id.ToString().Replace("minecraft:", "");
                                                        string currentID = MainWindow.MobEffectDataBase.Keys.Where(item => item == id).First();
                                                        ComboBox idBox = contentPanel.FindChild<ComboBox>("Id");
                                                        idBox.ItemsSource = MainWindow.MobEffectIdSource;
                                                        idBox.SelectedValuePath = "ComboBoxItemText";
                                                        idBox.SelectedValue = Regex.Replace(MainWindow.MobEffectDataBase[currentID], @"\d+", "");
                                                    }
                                                    if (ShowIcon != null)
                                                        contentPanel.FindChild<TextCheckBoxs>("ShowIcon").IsChecked = ShowIcon.ToString() == "1";
                                                    if (ShowParticles != null)
                                                        contentPanel.FindChild<TextCheckBoxs>("ShowParticles").IsChecked = ShowParticles.ToString() == "1";
                                                    Grid grid = customPotionEffects.FactorCalculationDataGrid;
                                                    if (effect_changed_timestamp != null)
                                                        grid.FindChild<Slider>("effect_changed_timestamp").Value = int.Parse(effect_changed_timestamp.ToString());
                                                    if (factor_current != null)
                                                        grid.FindChild<Slider>("factor_current").Value = int.Parse(factor_current.ToString());
                                                    if (factor_previous_frame != null)
                                                        grid.FindChild<Slider>("factor_previous_frame").Value = int.Parse(factor_previous_frame.ToString());
                                                    if (factor_start != null)
                                                        grid.FindChild<Slider>("factor_start").Value = int.Parse(factor_start.ToString());
                                                    if (factor_target != null)
                                                        grid.FindChild<Slider>("factor_target").Value = int.Parse(factor_target.ToString());
                                                    if (had_effect_last_tick != null)
                                                        grid.FindChild<TextCheckBoxs>("had_effect_last_tick").IsChecked = had_effect_last_tick.ToString() == "1";
                                                    if (padding_duration != null)
                                                        grid.FindChild<Slider>("padding_duration").Value = int.Parse(padding_duration.ToString());
                                                    #endregion
                                                }
                                                componentEvents.CustomPotionEffects_LostFocus(itemAccordion,null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "StewEffectList":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(10, 2, 10, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new();
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray data)
                                            {
                                                foreach (JObject item in data.Cast<JObject>())
                                                {
                                                    JToken durationObj = item["EffectDuration"];
                                                    JToken idObj = item["EffectId"];

                                                    string id = idObj.ToString();
                                                    var currentID = MainWindow.MobEffectDataBase.Where(item => Regex.Match(item.Value, @"\d+").ToString() == id).First();
                                                    string currentIDString = Regex.Replace(currentID.Value, @"\d+", "");
                                                    IconComboBoxItem targetItem = MainWindow.MobEffectIdSource.Where(item => item.ComboBoxItemText == currentIDString).First();
                                                    SuspiciousStewEffects suspiciousStewEffects = new();
                                                    itemPanel.Children.Add(suspiciousStewEffects);
                                                    if (durationObj != null)
                                                        suspiciousStewEffects.EffectDuration.Value = int.Parse(durationObj.ToString());
                                                    if (idObj != null)
                                                        suspiciousStewEffects.EffectID.SelectedIndex = MainWindow.MobEffectIdSource.IndexOf(targetItem);
                                                }
                                                componentEvents.StewEffectList_LostFocus(itemAccordion,null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case "TAG_Compound":
                    {
                        if (Request.dependency != null && Request.dependency.Length > 0)
                        {
                            switch (Request.dependency)
                            {
                                case "InlineItems":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = "Items",
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(0, 0, 0, 2),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            JToken data = ExternallyReadEntityData.SelectToken(key);
                                            if (data != null)
                                            {
                                                JArray Items = JArray.Parse(data.ToString());
                                                string imagePath = "";
                                                for (int i = 0; i < Items.Count; i++)
                                                {
                                                    string itemID = JObject.Parse(Items[i].ToString())["id"].ToString();
                                                    Image image = new() { Tag = new NBTDataStructure() { Result = Items[i].ToString(), Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                                                    imagePath = itemImageFilePath + itemID + ".png";
                                                    if (File.Exists(imagePath))
                                                        image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                                                    InlineItems itemBag = new();
                                                    (itemBag.DisplayItem.Child as Image).Source = image.Source;
                                                    itemPanel.Children.Add(itemBag);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "LodestonePos":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        UUIDOrPosGroup uUIDOrPosGroup = new() { IsUUID = false };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            Content = uUIDOrPosGroup,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if(ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            JToken data = ExternallyReadEntityData.SelectToken(key);
                                            if (data != null)
                                            {
                                                JToken x = data["X"];
                                                JToken y = data["Y"];
                                                JToken z = data["Z"];
                                                uUIDOrPosGroup.EnableButton.IsChecked = true;
                                                if (x != null)
                                                uUIDOrPosGroup.number0.Value = int.Parse(x.ToString());
                                                if(y != null)
                                                uUIDOrPosGroup.number1.Value = int.Parse(y.ToString());
                                                if(z != null)
                                                uUIDOrPosGroup.number2.Value = int.Parse(z.ToString());
                                                componentEvents.LodestonePos_LostFocus(itemAccordion, null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "DebugProperty":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel stackPanel = new();
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            Content = stackPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JObject data)
                                            {
                                                List<JProperty> properties = data.Properties().ToList();
                                                for (int i = 0; i < properties.Count; i++)
                                                {
                                                    string currentKey = MainWindow.BlockDataBase.Where(item => item.Key[..item.Key.IndexOf(':')] == properties[i].Name).First().Key;
                                                    currentKey = currentKey[(currentKey.IndexOf(':') + 1)..];
                                                    DebugProperties debugProperties = new();
                                                    debugProperties.BlockId.SelectedValuePath = "ComboBoxItemText";
                                                    debugProperties.BlockId.SelectedValue = currentKey;
                                                    debugProperties.BlockProperty.SelectedValue = properties[i].Value.ToString();
                                                    stackPanel.Children.Add(debugProperties);
                                                }
                                                componentEvents.DebugProperties_LostFocus(itemAccordion, null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "MapDisplay":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new();
                                        MapDisplay mapDisplay = new MapDisplay();
                                        mapDisplay.EnableButton.IsChecked = true;
                                        itemPanel.Children.Add(mapDisplay);
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JObject data)
                                            {
                                                JToken colorObj = data["MapColor"];
                                                if (colorObj != null)
                                                {
                                                    mapDisplay.color.Value = int.Parse(colorObj.ToString());
                                                    componentEvents.MapDisplay_LostFocus(itemAccordion, null);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "BannerBlockEntityTag":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.dependency,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel stackPanel = new();
                                        ShieldBlockEntityTag shieldBlockEntityTag = new();
                                        stackPanel.Children.Add(shieldBlockEntityTag);
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            Content = stackPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Summon)
                                                key = "Item." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JObject data)
                                            {
                                                JToken baseObj = ExternallyReadEntityData.SelectToken("Base");
                                                if (baseObj != null)
                                                    shieldBlockEntityTag.Base.SelectedIndex = int.Parse(baseObj.ToString());
                                                StackPanel bannerPanel = (shieldBlockEntityTag.BannerAccordion.Content as ScrollViewer).Content as StackPanel;
                                                BannerBlockEntityTag bannerBlockEntityTag = new();
                                                JToken customname = data["CustomName"];
                                                if (customname != null)
                                                {
                                                    if (customname is JValue)
                                                    {
                                                        JObject customNameObj = JObject.Parse((customname as JValue).Value<string>());
                                                        bannerBlockEntityTag.CustomName.Text = customNameObj["text"].ToString();
                                                    }
                                                    else
                                                        bannerBlockEntityTag.CustomName.Text = Regex.Match(customname.ToString(), @"[a-zA-Z_]+").ToString();
                                                }

                                                if (data["Patterns"] is JArray PatternList)
                                                {
                                                    StackPanel patternListPanel = (bannerBlockEntityTag.BannerPatternAccordion.Content as ScrollViewer).Content as StackPanel;
                                                    foreach (JObject Apattern in PatternList.Cast<JObject>())
                                                    {
                                                        BannerPatterns bannerPatterns = new();
                                                        patternListPanel.Children.Add(bannerPatterns);
                                                        JToken color = Apattern["Color"];
                                                        JToken pattern = Apattern["Pattern"];
                                                        if (color != null)
                                                            bannerPatterns.Color.SelectedIndex = int.Parse(color.ToString());
                                                        if (pattern != null)
                                                        {
                                                            bannerPatterns.Pattern.SelectedValuePath = "ComboBoxItemText";
                                                            bannerPatterns.Pattern.SelectedValue = pattern.ToString();
                                                        }
                                                    }
                                                }
                                                bannerPanel.Children.Add(bannerBlockEntityTag);
                                                componentEvents.ShieldBlockEntityTag_LostFocus(itemAccordion,null);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case "TAG_Byte":
                case "TAG_Int":
                case "TAG_Float":
                case "TAG_Double":
                case "TAG_Short":
                case "TAG_Pos":
                case "TAG_UUID":
                    {
                        double minValue = 0;
                        double maxValue = 0;
                        if (Request.dataType == "TAG_Byte")
                        {
                            minValue = byte.MinValue;
                            maxValue = byte.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Int" || Request.dataType == "TAG_UUID")
                        {
                            minValue = int.MinValue;
                            maxValue = int.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Float")
                        {
                            minValue = float.MinValue;
                            maxValue = float.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Double" || Request.dataType == "TAG_Pos")
                        {
                            minValue = double.MinValue;
                            maxValue = double.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Short")
                        {
                            minValue = short.MinValue;
                            maxValue = short.MaxValue;
                        }

                        if (Request.dataType == "TAG_Pos" || Request.dataType == "TAG_UUID")
                        {
                            UUIDOrPosGroup uUIDOrPosGroup = new()
                            {
                                Uid = Request.nbtType,
                                Name = Request.key,
                                IsUUID = Request.dataType == "TAG_UUID",
                                Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                            };
                            result.Add(uUIDOrPosGroup);
                            uUIDOrPosGroup.GotFocus += componentEvents.ValueChangedHandler;
                            #region 分析是否需要代入导入的数据
                            if (ImportMode)
                            {
                                string key = Request.key;
                                if (Summon)
                                    key = "Item." + key;
                                JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                                if (currentObj != null)
                                {
                                    JArray dataArray = JArray.Parse(currentObj.ToString());
                                    uUIDOrPosGroup.number0.Value = int.Parse(dataArray[0].ToString());
                                    uUIDOrPosGroup.number1.Value = int.Parse(dataArray[1].ToString());
                                    uUIDOrPosGroup.number2.Value = int.Parse(dataArray[2].ToString());
                                    uUIDOrPosGroup.number3.Value = int.Parse(dataArray[3].ToString());
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            Slider numberBox1 = new()
                            {
                                Name = Request.key,
                                Uid = Request.nbtType,
                                Minimum = minValue,
                                Maximum = maxValue,
                                Value = 0,
                                Style = Application.Current.Resources["NumberBoxStyle"] as Style,
                                Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                            };
                            numberBox1.GotFocus += componentEvents.ValueChangedHandler;
                            result.Add(numberBox1);
                            #region 分析是否需要代入导入的数据
                            if (ImportMode)
                            {
                                string key = Request.key;
                                if (Summon)
                                    key = "Item." + key;
                                JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                                if (currentObj != null)
                                {
                                    numberBox1.Value = int.Parse(currentObj.ToString());
                                    componentEvents.NumberBoxValueChanged(numberBox1, null);
                                }
                            }
                            #endregion
                        }
                    }
                    break;
                case "TAG_String_List":
                case "TAG_String":
                case "TAG_Long":
                    {
                        TextBox stringBox = new() { BorderBrush = blackBrush, Foreground = whiteBrush, Uid = Request.nbtType, Name = Request.key, Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                        stringBox.GotFocus += componentEvents.ValueChangedHandler;
                        if (Request.dataType == "TAG_String_List")
                        {
                            string NewToolTip = Request.toolTip + "(以,分割成员,请遵守NBT语法)";
                            displayText.ToolTip = NewToolTip;
                        }
                        result.Add(stringBox);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Summon)
                                key = "Item." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if (currentObj != null)
                            {
                                stringBox.Text = currentObj.ToString().Replace("\"", "");
                                if (Request.dataType == "TAG_String")
                                    componentEvents.StringBox_LostFocus(stringBox, null);
                                else
                                if (Request.dataType == "TAG_String_List")
                                    componentEvents.StringListBox_LostFocus(stringBox, null);
                                else
                                if (Request.dataType == "TAG_Long")
                                    componentEvents.LongNumberBox_LostFocus(stringBox, null);
                            }
                        }
                        #endregion
                    }
                    break;
                case "TAG_NameSpaceReference":
                    {
                        Grid grid = new() { Uid = Request.nbtType, Name = Request.key, Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                        TextBox stringBox = new() { IsReadOnly = true, CaretBrush = whiteBrush, BorderBrush = blackBrush, Foreground = whiteBrush };
                        IconTextButtons textButtons = new()
                        {
                            Style = Application.Current.Resources["IconTextButton"] as Style,
                            Background = buttonNormalBrush,
                            PressedBackground = buttonPressedBrush,
                            Content = "设置引用",
                            Padding = new Thickness(5, 2, 5, 2)
                        };
                        grid.Children.Add(stringBox);
                        grid.Children.Add(textButtons);
                        Grid.SetColumn(stringBox, 0);
                        Grid.SetColumn(textButtons, 1);
                        grid.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(grid);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Summon)
                                key = "Item." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if (currentObj != null)
                            {
                                stringBox.Text = currentObj.ToString().Replace("\"", "");
                                componentEvents.NameSpaceReference_LostFocus(stringBox, null);
                            }
                        }
                        #endregion
                    }
                    break;
                case "TAG_Boolean":
                    {
                        TextCheckBoxs textCheckBoxs = new()
                        {
                            Uid = Request.nbtType,
                            Name = Request.key,
                            Foreground = whiteBrush,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            HeaderWidth = 20,
                            HeaderHeight = 20,
                            Style = Application.Current.Resources["TextCheckBox"] as Style,
                            Content = Request.description,
                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                        };
                        Grid.SetColumnSpan(textCheckBoxs, 2);
                        if (Request.toolTip.Length > 0)
                        {
                            textCheckBoxs.ToolTip = Request.toolTip;
                            ToolTipService.SetBetweenShowDelay(textCheckBoxs, 0);
                            ToolTipService.SetInitialShowDelay(textCheckBoxs, 0);
                        }
                        result.Remove(displayText);
                        textCheckBoxs.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(textCheckBoxs);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Summon)
                                key = "Item." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if (currentObj != null)
                            {
                                textCheckBoxs.IsChecked = currentObj.ToString() == "1" || currentObj.ToString() == "true";
                                componentEvents.CheckBox_Checked(textCheckBoxs, null);
                            }
                        }
                        #endregion
                    }
                    break;
                case "TAG_Enum":
                    {
                        MatchCollection matchCollection = Regex.Matches(Request.toolTip, @"[a-zA-Z_]+");
                        List<string> enumValueList = matchCollection.ToList().ConvertAll(item => item.ToString());
                        if(enumValueList.Count == 0)
                            enumValueList = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\"+Request.dependency+".ini").ToList();
                        ComboBox comboBox = new()
                        {
                            ItemsSource = enumValueList,
                            Height = 25,
                            Uid = Request.nbtType,
                            Foreground = whiteBrush,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Style = Application.Current.Resources["TextComboBoxStyle"] as Style,
                            Name = Request.key,
                            SelectedIndex = 0,
                            Tag = new NBTDataStructure() { resultType = Request.resultType, Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                        };
                        comboBox.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(comboBox);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Summon)
                                key = "Item." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if (currentObj != null)
                            {
                                comboBox.SelectedIndex = enumValueList.IndexOf(currentObj.ToString());
                                componentEvents.EnumBox_SelectionChanged(comboBox, null);
                            }
                        }
                        #endregion
                    }
                    break;
            }

            #region 删除已读取的键
            if (ImportMode)
                ExternallyReadEntityData.Remove(Request.key);
            if (ExternallyReadEntityData != null)
            {
                string RemainData = ExternallyReadEntityData.ToString();
                if (RemainData == "[]" || RemainData == "{}")
                {
                    ExternallyReadEntityData = null;
                    ImportMode = false;
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// JSON转控件
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="nbtStructure"></param>
        private List<FrameworkElement> JsonToComponentConverter(JObject nbtStructure, string NBTType = "")
        {
            string tag = JArray.Parse(nbtStructure["tag"].ToString())[0].ToString();
            JToken resultObj = nbtStructure["resultType"];
            string result = resultObj != null ?resultObj.ToString():"";
            string key = nbtStructure["key"].ToString();
            JToken children = nbtStructure["children"];
            JToken descriptionObj = nbtStructure["description"];
            string description = descriptionObj != null ? descriptionObj.ToString() : "";
            JToken toolTipObj = nbtStructure["toolTip"];
            string toolTip = toolTipObj != null ? toolTipObj.ToString() : "";
            JToken dependencyObj = nbtStructure["dependency"];
            string dependency = dependencyObj != null ? dependencyObj.ToString() : "";
            ComponentData componentData = new()
            {
                dataType = tag,
                key = key,
                resultType = result,
                toolTip = toolTip,
                description = description,
                dependency = dependency,
                nbtType = NBTType
            };
            if (children != null)
                componentData.children = children.ToString();
            List<FrameworkElement> componentGroup = ComponentsGenerator(componentData);
            return componentGroup;
        }

        /// <summary>
        /// 根据物品ID实时显示隐藏特指数据
        /// </summary>
        private void UpdateUILayOut()
        {
            string data = File.ReadAllText(SpecialNBTStructureFilePath);
            JArray array = JArray.Parse(data);

            #region 搜索当前物品ID对应的JSON对象
            List<JToken> targetList = array.Where(item =>
            {
                JObject currentObj = item as JObject;
                if (currentObj["type"].ToString() == SelectedItemIdString)
                    return true;
                return false;
            }).ToList();
            #endregion

            if (targetList.Count > 0)
            {
                JObject targetObj = targetList.First() as JObject;
                #region 处理特指NBT
                if (!specialDataDictionary.ContainsKey(SelectedItemIdString))
                {
                    JArray children = JArray.Parse(targetObj["children"].ToString());
                    List<FrameworkElement> components = new();
                    Grid newGrid = new();
                    newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    #region 更新控件集合
                    foreach (JObject nbtStructure in children.Cast<JObject>())
                    {
                        List<FrameworkElement> result = JsonToComponentConverter(nbtStructure);
                        components.AddRange(result);
                    }
                    #endregion
                    #region 应用更新后的集合
                    bool LeftIndex = true;
                    for (int j = 0; j < components.Count; j++)
                    {
                        if (LeftIndex || newGrid.RowDefinitions.Count == 0)
                            newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                        newGrid.Children.Add(components[j]);
                        if (components[j] is Accordion || components[j] is TextCheckBoxs)
                        {
                            Grid.SetRow(components[j], newGrid.RowDefinitions.Count - 1);
                            Grid.SetColumn(components[j], 0);
                            Grid.SetColumnSpan(components[j], 2);
                            LeftIndex = true;
                        }
                        else
                        {
                            Grid.SetRow(components[j], newGrid.RowDefinitions.Count - 1);
                            Grid.SetColumn(components[j], LeftIndex ? 0 : 1);
                            LeftIndex = !LeftIndex;
                        }
                    }
                    #endregion

                    if (!specialDataDictionary.ContainsKey(SelectedItemIdString))
                        specialDataDictionary.Add(SelectedItemIdString, newGrid);
                    SpecialViewer.Content = newGrid;
                }
                else
                {
                    Grid cacheGrid = specialDataDictionary[SelectedItemIdString];
                    SpecialViewer.Content = cacheGrid;
                }
                #endregion
            }
        }
    }

    /// <summary>
    /// 动态控件数据结构
    /// </summary>
    public class ComponentData
    {
        public string children { get; set; }
        public string dataType { get; set; }
        public string resultType { get; set; }
        public string nbtType { get; set; }
        public string key { get; set; }
        public string description { get; set; }
        public string toolTip { get; set; }
        public string dependency { get; set; }
    }
}

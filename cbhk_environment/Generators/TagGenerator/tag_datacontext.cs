using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cbhk_environment.Generators.TagGenerator
{
    public partial class tag_datacontext: ObservableObject
    {
        #region 生成、返回等指令
        public RelayCommand RunCommand { get; set; }

        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand ImportFromClipboard { get; set; }
        public RelayCommand ImportFromFile { get; set; }
        #endregion

        #region 替换
        private bool replace;
        public bool Replace
        {
            get { return replace; }
            set
            {
                replace = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 存储最终生成的列表
        public List<string> Blocks = new();
        public List<string> Items = new();
        public List<string> Entities = new();
        public List<string> GameEvent = new();
        public List<string> Biomes = new();
        #endregion

        #region 搜索内容
        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                TagViewSource?.View.Refresh();
            }
        }
        #endregion

        #region 所有标签成员
        private ObservableCollection<TagItemTemplate> tagItems = new(){ };
        public ObservableCollection<TagItemTemplate> TagItems
        {
            get
            {
                return tagItems;
            }
            set
            {
                tagItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        //载入进程锁
        object tagItemsLock = new();

        //标签容器
        ListView TagZone = null;

        #region 当前选中的值成员
        private TagItemTemplate selectedItem = null;
        public TagItemTemplate SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
            }
        }
        private int LastSelectedIndex = 0;
        #endregion

        #region 当前选中的类型成员
        private string selectedTypeItem = null;
        public string SelectedTypeItem
        {
            get
            {
                return selectedTypeItem;
            }
            set
            {
                selectedTypeItem = value;
                OnPropertyChanged();
                TypeSelectionChanged();
            }
        }
        #endregion

        #region 全选或反选
        private bool selectedAll = false;
        public bool SelectedAll
        {
            get
            {
                return selectedAll;
            }
            set
            {
                selectedAll = value;
                if (TagViewSource != null)
                {
                    foreach (TagItemTemplate tagItemTemplate in TagViewSource.View)
                    {
                        if (tagItemTemplate.DisplayText.Trim().Length > 0)
                        {
                            string itemString = tagItemTemplate.DisplayText.Contains(' ')? tagItemTemplate.DisplayText[..tagItemTemplate.DisplayText.IndexOf(' ')]: tagItemTemplate.DisplayText;
                            tagItemTemplate.BeChecked = selectedAll;

                            if (tagItemTemplate.BeChecked.Value)
                            {
                                if (tagItemTemplate.DataType == "Item" && !Items.Contains("\"minecraft:" + itemString + "\","))
                                    Items.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Entity" && !Entities.Contains("\"minecraft:" + itemString + "\","))
                                    Entities.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Block&Item" && !Blocks.Contains("\"minecraft:" + itemString + "\","))
                                    Blocks.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Biome" && !Biomes.Contains("\"minecraft:" + itemString + "\","))
                                    Biomes.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "GameEvent" && !GameEvent.Contains("\"minecraft:" + itemString + "\","))
                                    GameEvent.Add("\"minecraft:" + itemString + "\",");
                            }
                            else
                            {
                                if (tagItemTemplate.DataType == "Item" && Items.Contains("\"minecraft:" + itemString + "\","))
                                    Items.Remove("\"minecraft:" + itemString + "\",");
                                if (tagItemTemplate.DataType == "Entity" && Entities.Contains("\"minecraft:" + itemString + "\","))
                                    Entities.Remove("\"minecraft:" + itemString + "\",");
                                if (tagItemTemplate.DataType == "Block&Item" && Blocks.Contains("\"minecraft:" + itemString + "\","))
                                    Blocks.Remove("\"minecraft:" + itemString + "\",");
                                if (tagItemTemplate.DataType == "Biome" && Biomes.Contains("\"minecraft:" + itemString + "\","))
                                    Biomes.Remove("\"minecraft:" + itemString + "\",");
                                if (tagItemTemplate.DataType == "GameEvent" && GameEvent.Contains("\"minecraft:" + itemString + "\","))
                                    GameEvent.Remove("\"minecraft:" + itemString + "\",");
                            }
                        }
                    }
                }
            }
        }
        private bool reverseAll = false;
        public bool ReverseAll
        {
            get
            {
                return reverseAll;
            }
            set
            {
                reverseAll = value;
                if (TagViewSource != null)
                {
                    foreach (TagItemTemplate tagItemTemplate in TagViewSource.View)
                    {
                        if (tagItemTemplate.DisplayText.Trim().Length > 0)
                        {
                            string itemString = tagItemTemplate.DisplayText.Contains(' ') ? tagItemTemplate.DisplayText[..tagItemTemplate.DisplayText.IndexOf(' ')] : tagItemTemplate.DisplayText;
                            tagItemTemplate.BeChecked = !tagItemTemplate.BeChecked.Value;
                            if (tagItemTemplate.BeChecked.Value)
                            {
                                if (tagItemTemplate.DataType == "Item" && !Items.Contains("\"minecraft:" + itemString + "\","))
                                    Items.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Entity" && !Entities.Contains("\"minecraft:" + itemString + "\","))
                                    Entities.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Block&Item" && !Blocks.Contains("\"minecraft:" + itemString + "\","))
                                    Blocks.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Biome" && !Biomes.Contains("\"minecraft:" + itemString + "\","))
                                    Biomes.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "GameEvent" && !GameEvent.Contains("\"minecraft:" + itemString + "\","))
                                    GameEvent.Add("\"minecraft:" + itemString + "\",");
                            }
                            else
                            {
                                if (tagItemTemplate.DataType == "Item")
                                    Items.Remove("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Entity")
                                    Entities.Remove("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Block&Item")
                                    Blocks.Remove("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Biome")
                                    Biomes.Remove("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "GameEvent")
                                    GameEvent.Remove("\"minecraft:" + itemString + "\",");
                            }
                        }
                    }
                }
            }
        }
        #endregion

        //对象数据源
        CollectionViewSource TagViewSource = null;
        //标签生成器的过滤类型数据源
        public ObservableCollection<string> TypeItemSource = new();

        /// <summary>
        /// 生物群系配置文件路径
        /// </summary>
        private string BiomesFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\Biomes.ini";
        /// <summary>
        /// 游戏事件配置文件路径
        /// </summary>
        private string GameEventFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\GameEventTags.ini";
        /// <summary>
        /// 物品配置文件路径
        /// </summary>
        private string ItemFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\items.json";
        /// <summary>
        /// 方块配置文件路径
        /// </summary>
        private string BlockFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\blocks.json";
        /// <summary>
        /// 实体配置文件路径
        /// </summary>
        private string EntityFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entities.json";
        /// <summary>
        /// 物品、方块、实体的图像目录
        /// </summary>
        private string ImageFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
        //标记载入状态
        bool ItemsLoaded = false;

        [GeneratedRegex("[a-zA-z_]+")]
        private static partial Regex GetDisplayText();

        public tag_datacontext()
        {
            #region 链接指令
            RunCommand = new RelayCommand(run_command);
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            ImportFromClipboard = new(ImportFromClipboardCommand);
            ImportFromFile = new(ImportFromFileCommand);
            #endregion

            #region 异步载入标签成员
            BindingOperations.EnableCollectionSynchronization(TagItems, tagItemsLock);
            Task.Run(() =>
            {
                lock(tagItemsLock)
                {
                    #region 物品
                    if (File.Exists(ItemFilePath))
                    {
                        string items = File.ReadAllText(ItemFilePath);
                        JArray itemArray = JArray.Parse(items);
                        foreach (JObject item in itemArray.Cast<JObject>())
                        {
                            string id = item["id"].ToString();
                            string name = item["name"].ToString();
                            string iconPath = ImageFolderPath + id + ".png";
                            Uri uri = new(iconPath,UriKind.Absolute);
                            TagItems.Add(new TagItemTemplate()
                            {
                                Icon = uri,
                                DisplayText = id + " " + name,
                                DataType = "Item",
                                BeChecked = false
                            });
                        }
                    }
                    #endregion
                    #region 方块
                    if (File.Exists(BlockFilePath))
                    {
                        string blocks = File.ReadAllText(BlockFilePath);
                        JArray blockArray = JArray.Parse(blocks);
                        foreach (JObject block in blockArray.Cast<JObject>())
                        {
                            string id = block["id"].ToString();
                            string name = block["name"].ToString();
                            string iconPath = ImageFolderPath + id + ".png";
                            Uri uri = new(iconPath, UriKind.Absolute);
                            TagItems.Add(new TagItemTemplate()
                            {
                                Icon = uri,
                                DisplayText = id + " " + name,
                                DataType = "Block&Item",
                                BeChecked = false
                            });
                        }
                    }
                    #endregion
                    #region 实体
                    if (File.Exists(EntityFilePath))
                    {
                        string entities = File.ReadAllText(EntityFilePath);
                        JArray entityArray = JArray.Parse(entities);
                        foreach (JObject entity in entityArray.Cast<JObject>())
                        {
                            string id = entity["id"].ToString();
                            string name = entity["name"].ToString();
                            string iconPath = ImageFolderPath + id + "_spawner_egg.png";
                            if (!File.Exists(ImageFolderPath + id + ".png"))
                                iconPath = ImageFolderPath + id + ".png";
                            Uri uri = new(iconPath, UriKind.Absolute);
                            TagItems.Add(new TagItemTemplate()
                            {
                                Icon = uri,
                                DisplayText = id + " " + name,
                                DataType = "Entity",
                                BeChecked = false
                            });
                        }
                    }
                    #endregion
                    #region 生物群系
                    if (File.Exists(BiomesFilePath))
                    {
                        string[] biomes = File.ReadAllLines(BiomesFilePath);
                        foreach (string biome in biomes)
                        {
                            TagItems.Add(new TagItemTemplate()
                            {
                                DisplayText = biome,
                                DataType = "Biome",
                                BeChecked = false
                            });
                        }
                    }
                    #endregion
                    #region 游戏事件
                    if (File.Exists(GameEventFilePath))
                    {
                        string[] gameEvents = File.ReadAllLines(GameEventFilePath);
                        foreach (string gameEvent in gameEvents)
                        {
                            TagItems.Add(new TagItemTemplate()
                            {
                                DisplayText = gameEvent,
                                DataType = "GameEvent",
                                BeChecked = false
                            });
                        }
                    }
                    #endregion
                    //载入完毕
                    ItemsLoaded = true;
                }
            });
            #endregion
        }

        /// <summary>
        /// 从剪切板导入标签数据
        /// </summary>
        private void ImportFromClipboardCommand()
        {
            ObservableCollection<TagItemTemplate> items = TagItems;
            tag_datacontext context = this;
            ExternalDataImportManager.ImportTagDataHandler(Clipboard.GetText(),ref items,ref context,false);
        }

        /// <summary>
        /// 从文件导入标签数据
        /// </summary>
        private void ImportFromFileCommand()
        {
            ObservableCollection<TagItemTemplate> items = TagItems;
            tag_datacontext context = this;
            OpenFileDialog dialog = new()
            {
                Filter = "Json文件|*.json;",
                AddExtension = true,
                DefaultExt = ".json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Multiselect = false,
                RestoreDirectory = true,
                Title = "请选择一个标签文件"
            };
            if (dialog.ShowDialog().Value && File.Exists(dialog.FileName))
                ExternalDataImportManager.ImportTagDataHandler(dialog.FileName, ref items, ref context);
        }

        /// <summary>
        /// 过滤与搜索内容不相关的成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            #region 执行过滤
            TagItemTemplate currentItem = e.Item as TagItemTemplate;
            bool needDisplay = e.Accepted = currentItem.DataType.Contains(SelectedTypeItem) || SelectedTypeItem == "All";
            #endregion
            #region 执行搜索
            if (needDisplay)
            {
                if (SearchText.Trim().Length > 0)
                {
                    string currentItemIDAndName = currentItem.DisplayText;
                    e.Accepted = currentItemIDAndName.Contains(SearchText);
                }
                else
                    e.Accepted = true;
            }
            #endregion
        }

        /// <summary>
        /// 载入类型过滤列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TypeFilterLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            #region 加载过滤类型
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Tag\\data\\TypeFilter.ini"))
            {
                string[] Types = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Tag\\data\\TypeFilter.ini");
                for (int i = 0; i < Types.Length; i++)
                {
                    TypeItemSource.Add(Types[i]);
                }
            }
            comboBoxs.ItemsSource = TypeItemSource;
            comboBoxs.SelectedIndex = 0;
            #endregion
        }

        /// <summary>
        /// 更新过滤类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TypeSelectionChanged()
        {
            if(ItemsLoaded)
            TagViewSource?.View.Refresh();
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            Tag.cbhk.Topmost = true;
            Tag.cbhk.WindowState = WindowState.Normal;
            Tag.cbhk.Show();
            Tag.cbhk.Topmost = false;
            Tag.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 执行生成
        /// </summary>
        private void run_command()
        {
            string result = (string.Join("\r\n",Items) + "\r\n" + string.Join("\r\n",Blocks) + "\r\n" + string.Join("\r\n",Entities) + "\r\n" + string.Join("\r\n",Biomes) + "\r\n" + string.Join("\r\n",GameEvent)).TrimEnd().TrimEnd(',');
            result = "{\r\n  \"replace\": " + Replace.ToString().ToLower() + ",\r\n\"values\": [\r\n" + result.Trim('\n').Trim('\r') + "\r\n]\r\n}";
            SaveFileDialog saveFileDialog = new()
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "json",
                Filter = "JSON files (*.json;)|*.json;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Title = "保存为JSON文件"
            };
            if (saveFileDialog.ShowDialog().Value)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(saveFileDialog.FileName));
                File.WriteAllText(saveFileDialog.FileName, result);
                OpenFolderThenSelectFiles.ExplorerFile(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// 左击选中成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ListBoxClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Border)
            {
                LastSelectedIndex = TagZone.SelectedIndex;
                SelectedItem = null;
            }
            else
                if(SelectedItem == null && LastSelectedIndex > 0 && LastSelectedIndex < TagZone.Items.Count)
                SelectedItem = TagZone.Items[LastSelectedIndex] as TagItemTemplate;
            if (SelectedItem != null)
            {
                ReverseValue(SelectedItem);
            }
        }

        /// <summary>
        /// 背包视图载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            TagZone = sender as ListView;
            #region 获取数据源引用，订阅过滤事件
            Window parent = Window.GetWindow(TagZone);
            TagViewSource = parent.FindResource("TagItemSource") as CollectionViewSource;
            TagViewSource.Filter += CollectionViewSource_Filter;
            #endregion
        }

        /// <summary>
        /// 反转目标成员的值
        /// </summary>
        /// <param name="CurrentItem"></param>
        private void ReverseValue(TagItemTemplate CurrentItem)
        {
            if (TagZone.ItemContainerGenerator.ContainerFromItem(CurrentItem) is ListViewItem listViewItem)
            {
                ContentPresenter contentPresenter = ChildrenHelper.FindVisualChild<ContentPresenter>(listViewItem);
                RichCheckBoxs iconCheckBoxs = contentPresenter.ContentTemplate.FindName("checkbox", contentPresenter) as RichCheckBoxs;
                string displayText = CurrentItem.DisplayText;
                string itemString;
                if (displayText.Trim().Length > 0)
                {
                    itemString = displayText.Contains(' ') ? displayText[..displayText.IndexOf(' ')] : displayText;
                    if (GetDisplayText().IsMatch(displayText.Trim()))
                    {
                        iconCheckBoxs.IsChecked = !iconCheckBoxs.IsChecked.Value;
                        if (iconCheckBoxs.IsChecked.Value)
                        {
                            if (CurrentItem.DataType == "Item" && !Items.Contains("\"minecraft:" + itemString + "\","))
                                Items.Add("\"minecraft:" + itemString + "\",");
                            else
                            if (CurrentItem.DataType == "Entity" && !Entities.Contains("\"minecraft:" + itemString + "\","))
                                Entities.Add("\"minecraft:" + itemString + "\",");
                            else
                            if (CurrentItem.DataType == "Block&Item" && !Blocks.Contains("\"minecraft:" + itemString + "\","))
                                Blocks.Add("\"minecraft:" + itemString + "\",");
                            else
                            if (CurrentItem.DataType == "Biome" && !Biomes.Contains("\"minecraft:" + itemString + "\","))
                                Biomes.Add("\"minecraft:" + itemString + "\",");
                            else
                            if (CurrentItem.DataType == "GameEvent" && !GameEvent.Contains("\"minecraft:" + itemString + "\","))
                                GameEvent.Add("\"minecraft:" + itemString + "\",");
                        }
                        else
                        {
                            if (CurrentItem.DataType == "Item" && Items.Contains("\"minecraft:" + itemString + "\","))
                                Items.Remove("\"minecraft:" + itemString + "\",");
                            if (CurrentItem.DataType == "Entity" && Entities.Contains("\"minecraft:" + itemString + "\","))
                                Entities.Remove("\"minecraft:" + itemString + "\",");
                            if (CurrentItem.DataType == "Block&Item" && Blocks.Contains("\"minecraft:" + itemString + "\","))
                                Blocks.Remove("\"minecraft:" + itemString + "\",");
                            if (CurrentItem.DataType == "Biome" && Biomes.Contains("\"minecraft:" + itemString + "\","))
                                Biomes.Remove("\"minecraft:" + itemString + "\",");
                            if (CurrentItem.DataType == "GameEvent" && GameEvent.Contains("\"minecraft:" + itemString + "\","))
                                GameEvent.Remove("\"minecraft:" + itemString + "\",");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 载入成员的属性模板
    /// </summary
    public class TagItemTemplate:ObservableObject
    {
        public Uri Icon { get; set; }
        public string DataType { get; set; }
        public string DisplayText { get; set; }

        private bool? beChecked = false;
        public bool? BeChecked
        {
            get
            {
                return beChecked;
            }
            set
            {
                beChecked = value;
                OnPropertyChanged();
            }
        }
    }
}
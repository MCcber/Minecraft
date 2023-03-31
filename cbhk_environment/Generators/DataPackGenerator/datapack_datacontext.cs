using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools.ScrollViewerHelper;
using cbhk_environment.Generators.DataPackGenerator.Components;
using cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSScriptControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.AI.MachineLearning;
using WK.Libraries.BetterFolderBrowserNS;

namespace cbhk_environment.Generators.DataPackGenerator
{
    public class datapack_datacontext:ObservableObject
    {
        //存储语法树数据
        Dictionary<string, ObservableCollection<string>> grammaticalTree = new Dictionary<string, ObservableCollection<string>> { };
        //存储所有指令的部首
        Dictionary<string,int> grammaticalRadical = new Dictionary<string, int> { };
        //语法树文件路径
        string grammaticalFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\grammatical_structure.json";

        //近期内容文件列表路径
        public static string recentContentsFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_contents";

        //近期模板文件列表路径
        public static string recentTemplateFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_templates";

        //存储包过滤器的成员
        public static List<PackFilter> packFilterList = new List<PackFilter> { };

        //保存所使用的所有图标引用
        public static ResourceDictionary IconDictionary = null;

        //获取近期内容视图引用
        TreeView recentContentView = null;

        //近期内容前景色
        SolidColorBrush RecentContentForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        //树视图样式引用
        public static Style RichTreeViewItemStyle = null;

        //指定新建内容成员的类型
        static ContentReader.ContentType contentType = ContentReader.ContentType.DataPack;

        //获取近期内容搜索框引用
        TextBox RecentItemTextBox = null;

        //数据包所对应游戏版本数据库
        public static Dictionary<string, string> DatapackVersionDatabase = new Dictionary<string, string> { };
        //数据包所对应游戏版本配置文件路径
        public static string DatapackVersionFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\datapackVersion.json";

        #region 存储所有类型的模板标签
        public static List<string> SelectedTemplateTypeTagList = new List<string> { };
        #endregion

        /// <summary>
        /// 模板元数据存放路径
        /// </summary>
        public static string TemplateMetaDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\templates\\introductions";

        /// <summary>
        /// 存放文件类型列表
        /// </summary>
        public static ObservableCollection<string> FileTypeList { get; set; } = new ObservableCollection<string> { };

        /// <summary>
        /// 存放功能类型列表
        /// </summary>
        public static ObservableCollection<string> FunctionTypeList { get; set; } = new ObservableCollection<string> { };

        #region 模板图标文件路径
        public static string TemplateIconFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\images";
        #endregion

        /// <summary>
        /// 模板成员集合
        /// </summary>
        public static ObservableCollection<TemplateItems> TemplateList { get; set; } = new ObservableCollection<TemplateItems>();

        //未搜索到结果文本的前景色
        SolidColorBrush searchResultIsNullBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        #region 主页，模板设置页，属性设置页，数据上下文的引用
        /// <summary>
        /// 主页
        /// </summary>
        private static HomePage homePage = new HomePage();
        /// <summary>
        /// 属性设置页
        /// </summary>
        private static DatapackGenerateSetupPage datapackGenerateSetupPage = null;
        /// <summary>
        /// 模板选择页
        /// </summary>
        private static TemplateSelectPage templateSelectPage = null;
        /// <summary>
        /// 编辑页
        /// </summary>
        private static EditPage editPage = null;
        /// <summary>
        /// 页面容器
        /// </summary>
        private static Frame PageFrame = new Frame()
        {
            NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden
        };
        #endregion

        #region 初始化页面右侧按钮的指令列表
        public RelayCommand OpenLocalProject { get; set; }
        public RelayCommand OpenLocalFolder { get; set; }
        public RelayCommand OpenLocalFile { get; set; }
        public RelayCommand CreateLocalDataPack { get; set; }
        #endregion

        #region 近期内容父级日期节点
        public static List<RichTreeViewItems> recentContentList = new List<RichTreeViewItems>()
                {
                    new RichTreeViewItems() { Header = "已固定",Tag = "Fixed",IsExpanded = true,Visibility = Visibility.Collapsed},
                    new RichTreeViewItems() { Header = "今天",Tag = "ToDay",IsExpanded = true},
                    new RichTreeViewItems() { Header = "昨天",Tag = "Yesterday",IsExpanded = true },
                    new RichTreeViewItems() { Header = "这周",Tag = "ThisWeek",IsExpanded = true },
                    new RichTreeViewItems() { Header = "一周内",Tag = "LastWeek",IsExpanded = true },
                    new RichTreeViewItems() { Header = "这个月",Tag = "ThisMonth",IsExpanded = true },
                    new RichTreeViewItems() { Header = "一月内",Tag = "LastMonth",IsExpanded = true },
                    new RichTreeViewItems() { Header = "今年",Tag = "ThisYear",IsExpanded = true },
                    new RichTreeViewItems() { Header = "一年内",Tag = "LastYear",IsExpanded = true }
                };
        #endregion

        #region 保存新增的节点对象
        public static List<RichTreeViewItems> newTreeViewItems = new List<RichTreeViewItems> { };
        #endregion

        #region 近期内容的所有交互对象(用于搜索)
        public static List<RecentItems> RecentItemList = new List<RecentItems> { };
        #endregion

        #region 绑定近期内容的搜索结果
        public ObservableCollection<RecentItems> RecentItemSearchResults { get; set; } = new ObservableCollection<RecentItems> { };
        ItemsControl RecentItemSearchPanel = null;
        #endregion

        #region 近期内容树视图可见性
        Visibility recentItemTreeViewVisibility = Visibility.Visible;
        public Visibility RecentItemTreeViewVisibility
        {
            get { return recentItemTreeViewVisibility; }
            set
            {
                recentItemTreeViewVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 近期内容搜索视图可见性
        Visibility recentItemSearchPanelVisibility = Visibility.Collapsed;
        public Visibility RecentItemSearchPanelVisibility
        {
            get { return recentItemSearchPanelVisibility; }
            set
            {
                recentItemSearchPanelVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 函数文本框可见性
        Visibility functionEditorZoneVisibility = Visibility.Collapsed;
        public Visibility FunctionEditorZoneVisibility
        {
            get { return functionEditorZoneVisibility; }
            set
            {
                functionEditorZoneVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 初始化页面可见性
        Visibility initPageVisibility = Visibility.Visible;
        public Visibility InitPageVisibility
        {
            get { return initPageVisibility; }
            set
            {
                initPageVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region js脚本执行者
        string js_file = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js";
        static string language = "javascript";
        public static ScriptControlClass json_parser = new ScriptControlClass()
        {
            Language = language
        };
        public static object JsonScript(string JScript)
        {
            object Result = null;
            try
            {
                Result = json_parser.Eval(JScript);
            }
            catch (Exception ex)
            {
                return ex.Source + "\n" + ex.Message;
            }
            return Result;
        }
        #endregion

        #region 覆盖生成
        private bool overLying;
        public bool OverLying
        {
            get { return overLying; }
            set
            {
                overLying = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public datapack_datacontext()
        {
            #region 链接指令
            OpenLocalProject = new RelayCommand(OpenLocalProjectCommand);
            OpenLocalFolder = new RelayCommand(OpenLocalFolderCommand);
            CreateLocalDataPack = new RelayCommand(CreateLocalDataPackCommand);
            OpenLocalFile = new RelayCommand(OpenLocalFileCommand);
            #endregion

            #region 读取语法树所需数据并解析每个指令的数据
            if (File.Exists(grammaticalFilePath))
            {
                string grammaticalJson = File.ReadAllText(grammaticalFilePath);

                //开始存储部首
                if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && 
                    grammaticalRadical.Count == 0)
                {
                    JsonScript(js_file);

                    JsonScript("var data = " + grammaticalJson);
                    int.TryParse(JsonScript("data.length").ToString(), out int item_count);

                    for (int i = 0; i < item_count; i++)
                    {
                        string radicalString = JsonScript("data[" + i + "].radical").ToString();
                        //存储指令部首和对应的索引
                        grammaticalRadical.Add(radicalString, i);
                    }
                }
            }
            #endregion

            #region 载入字典引用
            if (IconDictionary == null)
                IconDictionary = Application.LoadComponent(new Uri("/cbhk_environment;component/Generators/DataPackGenerator/Dictionaries/Icons.xaml",
                                            UriKind.RelativeOrAbsolute)) as ResourceDictionary;
            #endregion

            #region 读取并设置版本映射关系
            if (File.Exists(DatapackVersionFilePath) && DatapackVersionDatabase.Count == 0)
            {
                #region 解析并设置版本配置文件
                string VersionFile = File.ReadAllText(DatapackVersionFilePath);
                string[] versionStringList = VersionFile.Replace("{", "").Replace("}", "").Replace("\"", "").Split(',');

                foreach (string versionItem in versionStringList)
                {
                    string[] itemData = versionItem.Split(':');
                    string display = itemData[0].Trim();
                    string value = itemData[1].Trim();
                    DatapackVersionDatabase.Add(display, value);
                }
                #endregion
            }
            #endregion
        }

        public DatapackGenerateSetupPage GetDatapackGenerateSetupPage()
        {
            return datapackGenerateSetupPage;
        }

        public EditPage GetEditPage()
        {
            return editPage;
        }

        public void DataPackClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            newTreeViewItems.Clear();
        }

        /// <summary>
        /// 初始化数据包生成器的主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadHomePage(object sender, RoutedEventArgs e)
        {
            ContentControl contentControl = sender as ContentControl;
            if(homePage.DataContext == null)
            homePage.DataContext = this;
            PageFrame.Content = homePage;
            contentControl.Content = PageFrame;
        }

        /// <summary>
        /// 打开本地文件
        /// </summary>
        private void OpenLocalFileCommand()
        {
            Microsoft.Win32.OpenFileDialog fileBrowser = new Microsoft.Win32.OpenFileDialog()
            {
                Multiselect = true,
                RestoreDirectory = true,
                Title = "请选择一个或多个与Minecraft相关的文件",
                Filter = "Minecraft函数文件|*.mcfunction;|JSON文件|*.json"
            };

            if (fileBrowser.ShowDialog() == true)
            {
                foreach (string FileName in fileBrowser.FileNames)
                {
                    contentType = ContentReader.ContentType.File;
                    //解析mcmeta文件数据
                    ContentReader.DataPackMetaStruct metaStruct = new ContentReader.DataPackMetaStruct()
                    {
                    };
                    RichTreeViewItems contentNodes = ContentReader.ReadTargetContent(FileName, contentType, metaStruct);

                    if (contentNodes != null)
                    {
                        newTreeViewItems.Add(contentNodes);
                        InitPageVisibility = Visibility.Collapsed;
                        FunctionEditorZoneVisibility = Visibility.Visible;

                        //添加进近期使用内容链表
                        string folderName = Path.GetFileNameWithoutExtension(FileName);
                        File.WriteAllText(recentContentsFolderPath + "\\" + folderName + ".content", FileName);
                    }
                }
                editPage = new EditPage
                {
                    DataContext = new EditDataContext()
                };
                PageFrame.Content = editPage;
            }
        }

        /// <summary>
        ///  创建本地数据包
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateLocalDataPackCommand()
        {
            #region 载入模板选择窗体
            if(templateSelectPage == null)
                templateSelectPage = new TemplateSelectPage();
            if(templateSelectPage.DataContext == null)
            templateSelectPage.DataContext = new TemplateSelectDataContext();
            PageFrame.Content = templateSelectPage;
            #endregion
        }

        /// <summary>
        /// 打开本地文件夹
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OpenLocalFolderCommand()
        {
            BetterFolderBrowser betterFolderBrowser = new BetterFolderBrowser()
            {
                Multiselect = true,
                RootFolder = @"C:",
                Title = "请选择要编辑的Minecraft相关文件夹"
            };
            if(betterFolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string FilePath in betterFolderBrowser.SelectedPaths)
                {
                    contentType = ContentReader.ContentType.Folder;
                    //解析mcmeta文件数据
                    ContentReader.DataPackMetaStruct metaStruct = new ContentReader.DataPackMetaStruct()
                    {
                    };
                    RichTreeViewItems contentNodes = ContentReader.ReadTargetContent(FilePath, contentType,metaStruct);

                    if (contentNodes != null)
                    {
                        newTreeViewItems.Add(contentNodes);
                        InitPageVisibility = Visibility.Collapsed;
                        FunctionEditorZoneVisibility = Visibility.Visible;

                        //添加进近期使用内容链表
                        string folderName = Path.GetFileNameWithoutExtension(FilePath);
                        File.WriteAllText(recentContentsFolderPath + "\\" + folderName + ".content", FilePath);
                    }
                }
                editPage = new EditPage
                {
                    DataContext = new EditDataContext()
                };
                PageFrame.Content = editPage;
            }
        }

        /// <summary>
        /// 打开本地项目
        /// </summary>
        private void OpenLocalProjectCommand()
        {
            BetterFolderBrowser betterFolderBrowser = new BetterFolderBrowser()
            {
                Multiselect = true,
                RootFolder = @"C:",
                Title = "请选择要编辑的项目路径"
            };

            if (betterFolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                foreach (string FilePath in betterFolderBrowser.SelectedPaths)
                {
                    contentType = ContentReader.ContentType.DataPack;
                    //解析mcmeta文件数据
                    ContentReader.DataPackMetaStruct metaStruct = new ContentReader.DataPackMetaStruct()
                    {
                    };
                    RichTreeViewItems contentNodes = ContentReader.ReadTargetContent(FilePath, contentType, metaStruct);

                    if (contentNodes != null)
                    {
                        newTreeViewItems.Add(contentNodes);
                        InitPageVisibility = Visibility.Collapsed;
                        FunctionEditorZoneVisibility = Visibility.Visible;

                        //添加进近期使用内容链表
                        string folderName = Path.GetFileNameWithoutExtension(FilePath);
                        File.WriteAllText(recentContentsFolderPath + "\\" + folderName + ".content", FilePath);
                    }
                }
                editPage = new EditPage
                {
                    DataContext = new EditDataContext()
                };
                PageFrame.Content = editPage;
            }
        }

        /// <summary>
        /// 获取搜索结果面板引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RecentItemSearchPanelLoaded(object sender, RoutedEventArgs e)
        {
            RecentItemSearchPanel = sender as ItemsControl;
        }

        /// <summary>
        /// 获取树视图样式引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RichTreeViewItemStyleLoaded(object sender, RoutedEventArgs e)
        {
            RichTreeViewItemStyle = (sender as RichTreeViewItems).Style;
        }

        /// <summary>
        /// 获取模板容器引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TemplateListViewerLoaded(object sender, RoutedEventArgs e)
        {
            SelectedTemplateTypeTagList.Clear();

            #region 文件类型、功能类型的默认选中项
            if (FileTypeList.Count == 0)
                FileTypeList.Add("所有文件类型");
            if (FunctionTypeList.Count == 0)
                FunctionTypeList.Add("所有功能类型");
            #endregion

            if (Directory.Exists(TemplateMetaDataFilePath) && File.Exists(js_file) && TemplateList.Count == 0)
            {
                string js_file = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js");
                List<string> templateList = Directory.GetFiles(TemplateMetaDataFilePath).ToList();
                JsonScript(js_file);
                //白色刷子
                SolidColorBrush whiteBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                foreach (string template in templateList)
                {
                    string templateData = File.ReadAllText(template);
                    JsonScript("var data =" + templateData);

                    string FileName = Path.GetFileNameWithoutExtension(template);
                    string Description = JsonScript("data.Description").ToString();
                    string TemplateName = JsonScript("data.Name").ToString();
                    string FileType = JsonScript("data.FileType").ToString();
                    string FunctionType = JsonScript("data.FunctionType").ToString();
                    string fileNameSpace = JsonScript("data.NameSpace").ToString();

                    #region 载入文件类型和功能类型
                    if (!FileTypeList.Contains(FileType))
                    {
                        FileTypeList.Add(FileType);
                    }
                    if (!FunctionTypeList.Contains(FunctionType))
                    {
                        FunctionTypeList.Add(FunctionType);

                        #region 处理新建文件窗体的左侧类型树
                        RichTreeViewItems richTreeViewItems = new RichTreeViewItems()
                        {
                            Header = FunctionType,
                            Foreground = whiteBrush
                        };
                        if (EditDataContext.TemplateTypeItemList.Count == 0)
                        {
                            RichTreeViewItems FirstRichTreeViewItems = new RichTreeViewItems()
                            {
                                Header = FunctionType,
                                Foreground = whiteBrush
                            };
                            FirstRichTreeViewItems.Header = "全部";
                            FirstRichTreeViewItems.PreviewMouseLeftButtonDown += SwitchNewFileFunctionType;
                            EditDataContext.TemplateTypeItemList.Add(FirstRichTreeViewItems);
                        }
                        richTreeViewItems.PreviewMouseLeftButtonDown += SwitchNewFileFunctionType;
                        EditDataContext.TemplateTypeItemList.Add(richTreeViewItems);
                        #endregion
                    }
                    #endregion

                    #region 实例化模板成员
                    TemplateItems templateItem = new TemplateItems
                    {
                        TemplateID = FileName,
                        FileType = FileType,
                        FunctionType = FunctionType,
                        FileNameSpace = fileNameSpace
                    };
                    templateItem.MouseLeftButtonDown += UpdateNewFileFormData;

                    if (TemplateName != null)
                        templateItem.TemplateName.Text = TemplateName;
                    if (Description != null)
                        templateItem.TemplateDescription.Text = Description;

                    //判断获取的文件名是否有对应的图标文件
                    if (FileName != null && File.Exists(TemplateIconFilePath + "\\" + FileName + ".png"))
                        templateItem.TemplateImage.Source = new BitmapImage(
                            new Uri(TemplateIconFilePath + "\\" + FileName + ".png", UriKind.Absolute));

                    //添加文件类型和功能类型标签
                    if (FileType != null)
                        templateItem.TemplateTypeTagPanel.Children.Add(new TemplateTypeTag(FileType));
                    if (FunctionType != null)
                        templateItem.TemplateTypeTagPanel.Children.Add(new TemplateTypeTag(FunctionType));
                    TemplateList.Add(templateItem);
                    #endregion
                }
            }

            #region 设置版本、文件类型、功能类型默认选中项
            Page page = (sender as ScrollViewer).FindParent<Page>();
            TemplateSelectDataContext templateSelectDataContext = null;
            //模板选择页
            if (page != null)
            {
                TemplateList.All(item =>
                {
                    item.TemplateDescription.Visibility = Visibility.Visible;
                    return true;
                });
                templateSelectDataContext = page.DataContext as TemplateSelectDataContext;
                templateSelectDataContext.DefaultFileType = FileTypeList[1];
                templateSelectDataContext.SelectedFunctionType = FunctionTypeList.First();
            }
            else//新建文件窗体内更新右侧数据
            {
                TemplateList.All(item =>
                {
                    item.TemplateDescription.Visibility = Visibility.Collapsed;
                    return true;
                });
            }
            #endregion
        }

        /// <summary>
        /// 更新新建文件窗体数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateNewFileFormData(object sender, MouseButtonEventArgs e)
        {
            TemplateItems templateItems = sender as TemplateItems;

            #region 只允许选中一个模板
            if (EditDataContext.LastSelectedItems != null && EditDataContext.LastSelectedItems != templateItems)
                EditDataContext.LastSelectedItems.TemplateSelector.IsChecked = false;
            EditDataContext.LastSelectedItems = templateItems;
            #endregion

            #region 切换新建文件窗体右侧文件类型与描述
            EditDataContext editDataContext = Window.GetWindow(templateItems).DataContext as EditDataContext;
            if (templateItems != null && editDataContext != null)
            {
                editDataContext.RightSideFileType = templateItems.FileType;
                editDataContext.RightSideFileDescription = templateItems.TemplateDescription.Text;

                #region 更新新建文件窗体下方的文件名
                editDataContext.NewFileName = templateItems.TemplateID + templateItems.FileType;
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 新建文件窗体左侧树视图点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchNewFileFunctionType(object sender, MouseButtonEventArgs e)
        {
            RichTreeViewItems richTreeViewItems = sender as RichTreeViewItems;
            if(richTreeViewItems != null)
            {
                if (richTreeViewItems.Header.ToString() == "全部")
                    TemplateList.All(item =>
                    {
                        item.Visibility = Visibility.Visible;
                        return true;
                    });
                else
                    foreach (var item in TemplateList)
                    {
                        if (item.FunctionType != richTreeViewItems.Header.ToString())
                            item.Visibility = Visibility.Collapsed;
                        else
                            item.Visibility = Visibility.Visible;
                    }
            }
        }

        /// <summary>
        /// 开始搜索近期内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RecentItemSearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (RecentItemTextBox == null)
                RecentItemTextBox = sender as TextBox;

            //为空则返回
            if (RecentItemTextBox.Text == "")
            {
                foreach (RichTreeViewItems dateItem in recentContentList)
                {
                    foreach (RichTreeViewItems item in dateItem.Items)
                    {
                        RecentItems contentItem = item.Header as RecentItems;
                        if(RecentItemSearchPanel.Items.Contains(contentItem))
                        {
                            RecentItems newItem = new RecentItems();
                            newItem.MouseLeftButtonUp += AnalysisRecentItemData;
                            newItem.Tag = contentItem.Tag;
                            newItem.FileIcon.Source = new BitmapImage(new Uri(contentItem.CurrentFileIconPath, UriKind.Absolute));
                            newItem.FileName.Text = contentItem.FileName.Text;
                            newItem.FilePath.Text = contentItem.FilePath.Text;
                            newItem.FileModifyDateTime.Text = contentItem.FileModifyDateTime.Text;
                            newItem.CurrentTime = contentItem.CurrentTime;
                            item.Header = newItem;
                        }
                    }
                }

                RecentItemSearchPanelVisibility = Visibility.Collapsed;
                RecentItemTreeViewVisibility = Visibility.Visible;
                return;
            }

            RecentItemSearchPanel.ItemsSource = null;
            RecentItemSearchResults.Clear();

            //设置显示数据源
            RecentItemList.All(item =>
            {
                string searchText = RecentItemTextBox.Text.ToLower().Trim();
                string itemFileName = item.FileName.Text.ToLower();
                if (itemFileName.Contains(searchText))
                {
                    RecentItemSearchResults.Add(item);
                    if (RecentItemList.Contains(item))
                    return true;
                }
                return true;
            });

            //如果没找到内容则显示"为搜索到...相关的结果"
            if(RecentItemSearchResults.Count == 0)
            {
                RecentItemSearchPanel.Items.Add(new TextBlock() { Text = "未找到\"" + RecentItemTextBox.Text + "\"的结果", Foreground = searchResultIsNullBrush, FontSize = 12 });
            }
            else
            {
                RecentItemSearchPanel.ItemsSource = null;
                RecentItemSearchPanel.Items.Clear();
                RecentItemSearchPanel.ItemsSource = RecentItemSearchResults;
            }

            //切换近期内容视图和搜索面板可见性
            RecentItemSearchPanelVisibility = Visibility.Visible;
            RecentItemTreeViewVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 获取近期内容视图引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RecentFilesViewLoaded(object sender, RoutedEventArgs e)
        {
            recentContentView = sender as TreeView;

            //读取近期使用的内容
            if (Directory.Exists(recentContentsFolderPath))
            {
                UpdateRecentFileList();
                recentContentView.Items.Clear();
                foreach (RichTreeViewItems item in recentContentList)
                {
                    if (!Equals(item.Parent, null))
                    {
                        TreeView last_parent = item.Parent as TreeView;
                        last_parent.Items.Clear();
                    }
                }

                #region 添加日期
                //添加固定节点
                RichTreeViewItems fixNode = recentContentList.First();
                if (fixNode.Parent == null)
                    recentContentView.Items.Add(fixNode);

                if (recentContentView.Items.Count < 2)
                    foreach (RichTreeViewItems item in recentContentList)
                    {
                        if (item.Items.Count > 0 && item.Parent == null)
                        {
                            recentContentView.Items.Add(item);
                        }
                    }
                #endregion
            }
        }

        /// <summary>
        /// 更新近期内容链表
        /// </summary>
        private void UpdateRecentFileList()
        {
            string[] recent_contents = Directory.GetFiles(recentContentsFolderPath);

            //设置已固定节点的前景色
            recentContentList.First().Foreground = RecentContentForeground;

            //判断日期并添加成员
            foreach (string a_file in recent_contents)
            {
                RecentItems recentItems = new RecentItems(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\images\\icon.png", a_file);
                recentItems.MouseLeftButtonUp += AnalysisRecentItemData;
                string dateData = recentItems.CalculationDateInterval();

                //加入内容列表
                bool HadContent = false;
                foreach (RecentItems content in RecentItemList)
                {
                    if(content.FilePath.Text == recentItems.FilePath.Text)
                    {
                        HadContent = true;
                        break;
                    }
                }

                //去重
                if(!HadContent)
                {
                    RecentItemList.Add(recentItems);

                    List<RichTreeViewItems> targetNodeList = recentContentList.Where(item => item.Tag.ToString() == dateData).ToList();

                    if(targetNodeList.Count > 0)
                    {
                        int CurrentDateNodeIndex = recentContentList.IndexOf(targetNodeList[0]);
                        recentItems.Tag = CurrentDateNodeIndex;

                        targetNodeList[0].Foreground = RecentContentForeground;
                        RichTreeViewItems contentItem = new RichTreeViewItems
                        {
                            Header = recentItems,
                            Margin = new Thickness(0, 0, 0, 10)
                        };
                        targetNodeList[0].Items.Add(contentItem);
                    }
                }
            }
        }

        /// <summary>
        /// 解析近期内容数据并打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisRecentItemData(object sender, MouseButtonEventArgs e)
        {
            RecentItems item = sender as RecentItems;

            //如果在使用锚点则返回
            if (item.UsingThumbTack) return;

            string CurrentContentFilePath = item.FilePath.Text;

            if (File.Exists(CurrentContentFilePath) && File.Exists(js_file))
            {
                //读取内容文件
                string FilePath = File.ReadAllText(CurrentContentFilePath);

                //近期内容类型为未知
                contentType = ContentReader.GetTargetContentType(FilePath);
                //解析mcmeta文件数据
                ContentReader.DataPackMetaStruct metaStruct = ContentReader.McmetaParser(item.FilePath.Text);
                RichTreeViewItems contentItem = ContentReader.ReadTargetContent(FilePath,contentType, metaStruct);

                if(contentItem != null)
                {
                    newTreeViewItems.Add(contentItem);
                    InitPageVisibility = Visibility.Collapsed;
                    FunctionEditorZoneVisibility = Visibility.Visible;
                    editPage = new EditPage
                    {
                        DataContext = new EditDataContext()
                    };
                    PageFrame.Content = editPage;
                }
                else
                if (MessageBox.Show("当前所选内容不存在或无法识别,是否删除引用?", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RichTreeViewItems parent = item.Parent as RichTreeViewItems;
                    RichTreeViewItems dateItem = parent.Parent as RichTreeViewItems;
                    dateItem.Items.Remove(parent);
                    dateItem.Visibility = dateItem.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                    File.Delete(CurrentContentFilePath);
                }
            }
        }
    }
}

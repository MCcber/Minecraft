using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools.Time;
using cbhk_environment.Generators.DataPackGenerator.Components.HomePage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms
{
    public class HomePageDataContext:ObservableObject
    {
        #region 字段
        /// <summary>
        /// 白色
        /// </summary>
        private SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        /// <summary>
        /// 近期内容文件列表路径
        /// </summary>
        public string recentContentsFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_contents";

        /// <summary>
        /// 近期模板文件列表路径
        /// </summary>
        public string recentTemplateFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_templates";

        /// <summary>
        /// 近期被固定的解决方案
        /// </summary>
        public string StableRecentSolutionsFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_contents\\stable";

        /// <summary>
        /// 近期的非固定解决方案
        /// </summary>
        public string RecentSolutionsFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_contents";
        #endregion

        #region 搜索历史解决方案文本
        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                SetProperty(ref searchText, value);
                #region 搜索具有指定标题的近期解决方案
                if(SearchText.Length > 0)
                Task.Run(() =>
                {
                    dataPack.Dispatcher.InvokeAsync(() =>
                    {
                        RecentItemSearchResults.Clear();
                        foreach (var headItem in RecentContentDateItemList)
                        {
                            foreach(RichTreeViewItems contentItem in headItem.Items)
                            {
                                RecentTreeItem recentTreeItem = contentItem.Header as RecentTreeItem;
                                RecentTreeItem newRecentTreeItem = new();
                                newRecentTreeItem.pinBox.Margin = new Thickness(0,0,5,0);
                                newRecentTreeItem.Icon.Source = recentTreeItem.Icon.Source;
                                newRecentTreeItem.Title.Text = recentTreeItem.Title.Text;
                                newRecentTreeItem.Path.Text = recentTreeItem.Path.Text;
                                newRecentTreeItem.ModifyDate.Text = recentTreeItem.ModifyDate.Text;

                                string currentValue = recentTreeItem.Title.Text;
                                if (currentValue == SearchText || currentValue.StartsWith(SearchText) || Regex.IsMatch(currentValue, SearchText,RegexOptions.IgnoreCase))
                                {
                                    RichTreeViewItems richTreeViewItems = new()
                                    {
                                        Margin = new Thickness(0,0,0,10),
                                        MinHeight = 35,
                                        Header = newRecentTreeItem,
                                        Tag = contentItem.Tag,
                                        Uid = contentItem.Uid,
                                        Foreground = contentItem.Foreground,
                                        ToolTip = contentItem.ToolTip
                                    };
                                    ToolTipService.SetBetweenShowDelay(richTreeViewItems, 0);
                                    ToolTipService.SetInitialShowDelay(richTreeViewItems, 0);
                                    RecentItemSearchResults.Add(richTreeViewItems);
                                }
                            }
                        }
                        //如果搜索到了结果，则隐藏近期解决方案视图并且开启搜索视图
                        //转换两个视图的可见性
                        RecentItemTreeViewVisibility = Visibility.Collapsed;
                        SearchResultViewerVisibility = Visibility.Visible;
                        if (RecentItemSearchResults.Count == 0)
                            RecentItemSearchResults.Add(new TextBlock() { Text = $"未找到\"{SearchText}\"相关的结果", Foreground = whiteBrush,TextWrapping = TextWrapping.WrapWithOverflow });
                    });
                });
                else
                {
                    //转换两个视图的可见性
                    RecentItemTreeViewVisibility = Visibility.Visible;
                    SearchResultViewerVisibility = Visibility.Collapsed;
                }
                #endregion
            }
        }
        #endregion

        #region 初始化页面右侧按钮的指令列表
        public RelayCommand OpenLocalProject { get; set; }
        public RelayCommand OpenLocalFolder { get; set; }
        public RelayCommand OpenLocalFile { get; set; }
        public RelayCommand CreateLocalDataPack { get; set; }
        #endregion

        #region 近期内容父级日期节点
        public ObservableCollection<RichTreeViewItems> RecentContentDateItemList { get; set; } = new()
                {
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "已固定",Tag = "Fixed",IsExpanded = true,Visibility = Visibility.Collapsed},
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "一天内",Tag = "ToDay",IsExpanded = true,Visibility = Visibility.Collapsed},         
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "一天前",Tag = "Yesterday",IsExpanded = true,Visibility = Visibility.Collapsed },        
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "七天内",Tag = "ThisWeek",IsExpanded = true,Visibility = Visibility.Collapsed },        
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "上周",Tag = "LastWeek",IsExpanded = true , Visibility = Visibility.Collapsed},              
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "一月内",Tag = "ThisMonth",IsExpanded = true , Visibility = Visibility.Collapsed},       
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "上个月",Tag = "LastMonth",IsExpanded = true , Visibility = Visibility.Collapsed},       
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "一年内",Tag = "ThisYear",IsExpanded = true , Visibility = Visibility.Collapsed},             
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "去年",Tag = "LastYear",IsExpanded = true , Visibility = Visibility.Collapsed},              
                    new RichTreeViewItems() { Margin = new Thickness(0,2,0,0), Header = "很久前",Tag = "LongTime",IsExpanded = true , Visibility = Visibility.Collapsed}
                };
        #endregion

        #region 近期内容载入逻辑锁
        object RecentContentLoadLock = new();
        #endregion

        #region 搜索结果数据源
        private ObservableCollection<FrameworkElement> recentItemSearchResults = new();
        public ObservableCollection<FrameworkElement> RecentItemSearchResults
        {
            get => recentItemSearchResults;
            set => SetProperty(ref recentItemSearchResults, value);
        }
        #endregion

        #region 历史内容节点视图可见性
        private Visibility recentItemTreeViewVisibility = Visibility.Visible;
        public Visibility RecentItemTreeViewVisibility
        {
            get => recentItemTreeViewVisibility;
            set => SetProperty(ref recentItemTreeViewVisibility,value);
        }
        #endregion

        #region 搜索结果视图可见性
        private Visibility searchResultViewerVisibility = Visibility.Collapsed;
        public Visibility SearchResultViewerVisibility
        {
            get => searchResultViewerVisibility;
            set => SetProperty(ref searchResultViewerVisibility, value);
        }
        #endregion

        #region 主窗体引用
        DataPack dataPack = null;
        #endregion

        public HomePageDataContext()
        {
            #region 链接指令
            OpenLocalProject = new RelayCommand(OpenLocalProjectCommand);
            OpenLocalFolder = new RelayCommand(OpenLocalFolderCommand);
            CreateLocalDataPack = new RelayCommand(CreateLocalDataPackCommand);
            OpenLocalFile = new RelayCommand(OpenLocalFileCommand);
            #endregion
        }

        /// <summary>
        /// 使用右下角箭头获取数据包所在窗体的调度器以执行异步任务：加载固定和非固定的近期解决方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void RightArrowLoaded(object sender,RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            dataPack = Window.GetWindow(textBlock) as DataPack;
            await RecentSolutionsLoaded();
        }

        /// <summary>
        /// 异步载入历史解决方案节点链表
        /// </summary>
        /// <returns></returns>
        private async Task RecentSolutionsLoaded()
        {
            BindingOperations.EnableCollectionSynchronization(RecentContentDateItemList, RecentContentLoadLock);
            await Task.Run(() =>
            {
                lock (RecentContentLoadLock)
                {
                    #region 读取近期固定解决方案
                    if (Directory.Exists(StableRecentSolutionsFolderPath))
                    {
                        string[] stableContents = Directory.GetFiles(StableRecentSolutionsFolderPath);
                        dataPack.Dispatcher.InvokeAsync(() =>
                        {
                            RecentContentDateItemList.All(item =>
                            {
                                item.Foreground = whiteBrush;
                                item.Style = Application.Current.Resources["RichTreeViewItems"] as Style;
                                return true;
                            });
                            for (int i = 0; i < stableContents.Length; i++)
                            {
                                RecentTreeItem recentTreeItem = new()
                                {
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top
                                };
                                RotateTransform rotateTransform = recentTreeItem.pinBox.RenderTransform as RotateTransform;
                                rotateTransform.Angle = 0;
                                recentTreeItem.Title.Text = Path.GetFileNameWithoutExtension(stableContents[i]);
                                recentTreeItem.Path.Text = stableContents[i];
                                recentTreeItem.ModifyDate.Text = File.GetLastWriteTime(stableContents[i]).ToString("yyyy/M/d HH:mm");
                                RichTreeViewItems newNode = new()
                                {
                                    Margin = new Thickness(0, 0, 0, 10),
                                    Style = Application.Current.Resources["RichTreeViewItems"] as Style,
                                    Header = recentTreeItem,
                                    Foreground = whiteBrush,
                                    ToolTip = "打开解决方案\r\n" + stableContents[i],
                                    Uid = stableContents[i]
                                };
                                ToolTipService.SetBetweenShowDelay(newNode, 0);
                                ToolTipService.SetInitialShowDelay(newNode, 0);
                                RecentContentDateItemList[0].Items.Add(newNode);
                            }
                            RecentContentDateItemList[0].Visibility = Visibility.Visible;
                        });
                    }
                    #endregion
                    #region 读取近期非固定解决方案
                    if (Directory.Exists(RecentSolutionsFolderPath))
                    {
                        string[] Contents = Directory.GetFiles(RecentSolutionsFolderPath);
                        dataPack.Dispatcher.InvokeAsync(() =>
                        {
                            for (int i = 0; i < Contents.Length; i++)
                            {
                                RecentTreeItem recentTreeItem = new()
                                {
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top
                                };
                                recentTreeItem.Title.Text = Path.GetFileNameWithoutExtension(Contents[i]);
                                recentTreeItem.Path.Text = Contents[i];
                                recentTreeItem.ModifyDate.Text = File.GetLastWriteTime(Contents[i]).ToString("yyyy/M/d HH:mm");
                                RichTreeViewItems newNode = new()
                                {
                                    Margin = new Thickness(0, 0, 0, 10),
                                    Style = Application.Current.Resources["RichTreeViewItems"] as Style,
                                    Header = recentTreeItem,
                                    Foreground = whiteBrush,
                                    ToolTip = "打开解决方案\r\n" + Contents[i],
                                    Uid = Contents[i]
                                };
                                ToolTipService.SetBetweenShowDelay(newNode, 0);
                                ToolTipService.SetInitialShowDelay(newNode, 0);
                                string timeMarker = TimeDifferenceCalculater.Calculate(File.GetLastWriteTime(Contents[i]));
                                RecentContentDateItemList.All(item =>
                                {
                                    if (item.Header.ToString() == timeMarker)
                                    {
                                        item.Visibility = Visibility.Visible;
                                        item.Items.Add(newNode);
                                    }
                                    return true;
                                });
                            }
                        });
                    }
                    #endregion
                }
            });
        }

        /// <summary>
        /// 鼠标离开近期解决方案视图后取消选中成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeView_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((sender as TreeView).SelectedItem is RichTreeViewItems richTreeViewItems)
                richTreeViewItems.IsSelected = false;
        }

        /// <summary>
        /// 打开本地文件
        /// </summary>
        private void OpenLocalFileCommand()
        {
            Microsoft.Win32.OpenFileDialog fileBrowser = new()
            {
                Multiselect = true,
                RestoreDirectory = true,
                Title = "请选择一个或多个与Minecraft相关的文件",
                Filter = "Minecraft函数文件|*.mcfunction;|JSON文件|*.json"
            };

            if (fileBrowser.ShowDialog() == true)
            {
            }
        }

        /// <summary>
        ///  创建本地数据包
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateLocalDataPackCommand()
        {
            //templateSelectPage ??= new TemplateSelectPage();
            //templateSelectPage.DataContext ??= new TemplateSelectDataContext();
            //PageFrame.Content = templateSelectPage;
        }

        /// <summary>
        /// 打开本地文件夹
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OpenLocalFolderCommand()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new()
            {
                Description = "请选择要编辑的Minecraft相关文件夹",
                UseDescriptionForTitle = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                ShowHiddenFiles = true,
                ShowNewFolderButton = true
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && Directory.Exists(folderBrowserDialog.SelectedPath))
            {
            }
        }

        /// <summary>
        /// 打开本地项目
        /// </summary>
        private void OpenLocalProjectCommand()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new()
            {
                Description = "请选择要编辑的项目路径",
                UseDescriptionForTitle = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                ShowHiddenFiles = true,
                ShowNewFolderButton = true
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && Directory.Exists(folderBrowserDialog.SelectedPath))
            {
            }
        }
    }
}

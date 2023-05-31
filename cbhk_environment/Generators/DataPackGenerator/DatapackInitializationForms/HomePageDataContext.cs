using cbhk_environment.CustomControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms
{
    public class HomePageDataContext:ObservableObject
    {
        /// <summary>
        /// 白色
        /// </summary>
        SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        /// <summary>
        /// 近期内容文件列表路径
        /// </summary>
        public string recentContentsFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_contents";

        /// <summary>
        /// 近期模板文件列表路径
        /// </summary>
        public string recentTemplateFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_templates";

        /// <summary>
        /// 模板元数据存放路径
        /// </summary>
        public static string TemplateMetaDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\templates\\introductions";

        #region 搜索历史解决方案文本
        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }
        #endregion

        #region 初始化页面右侧按钮的指令列表
        public RelayCommand OpenLocalProject { get; set; }
        public RelayCommand OpenLocalFolder { get; set; }
        public RelayCommand OpenLocalFile { get; set; }
        public RelayCommand CreateLocalDataPack { get; set; }
        #endregion

        #region 近期内容父级日期节点
        public static List<RichTreeViewItems> RecentContentDateItemList = new()
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

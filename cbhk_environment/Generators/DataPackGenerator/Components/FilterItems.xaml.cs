using cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WK.Libraries.BetterFolderBrowserNS;

namespace cbhk_environment.Generators.DataPackGenerator.Components
{
    /// <summary>
    /// FilterItems.xaml 的交互逻辑
    /// </summary>
    public partial class FilterItems : UserControl
    {
        public FilterItems()
        {
            InitializeComponent();
        }

        //最终结果
        public string FilterBlock
        {
            get
            {
                string result;
                result = "{\"namespace\":\"" + NameSpaceBox.Text + "\",\"path\":\"" + PathBox.Text + "\"},";
                return result;
            }
        }

        /// <summary>
        /// 设置要过滤的命名空间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetNameSpaceClick(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowser folderBrowser = new BetterFolderBrowser()
            {
                Multiselect = false,
                Title = "请选择当前数据包过滤器的命名空间",
                RootFolder = Environment.SpecialFolder.MyComputer.ToString(),
            };

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folderBrowser.SelectedFolder))
                {
                    NameSpaceBox.Text = folderBrowser.SelectedFolder.Replace("\\","/");
                }
            }
        }

        /// <summary>
        /// 设置要过滤的路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetPathClick(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowser folderBrowser = new BetterFolderBrowser()
            {
                Multiselect = false,
                Title = "请选择当前数据包过滤器的路径",
                RootFolder = Environment.SpecialFolder.MyComputer.ToString(),
            };

            //若已选择命名空间则直接指定初始化目录
            if (NameSpaceBox.Text.Trim() != "")
                folderBrowser.RootFolder = NameSpaceBox.Text + "\\";

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folderBrowser.SelectedFolder))
                {
                    PathBox.Text = folderBrowser.SelectedFolder.Replace("\\", "/");
                }
            }
        }
        
        /// <summary>
        /// 从序列中删除自身
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DatapackGenerateSetupDataContext.DatapackFilterSource.Remove(this);
        }
    }
}

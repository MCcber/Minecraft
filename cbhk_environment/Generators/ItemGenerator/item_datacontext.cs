using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.Generators.ItemGenerator.Components;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator
{
    public class item_datacontext:ObservableObject
    {
        #region 返回、运行和保存等指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        public RelayCommand SaveAll { get; set; }
        public RelayCommand AddItem { get; set; }
        public RelayCommand ClearItem { get; set; }
        public RelayCommand ImportItemFromClipboard { get; set; }
        public RelayCommand ImportItemFromFile { get; set; }
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

        //物品页数据源
        public ObservableCollection<RichTabItems> ItemPageList { get; set; } = new() { new RichTabItems() { Style = Application.Current.Resources["RichTabItemStyle"] as Style, Header = "物品", IsContentSaved = true } };
        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconItems.png";

        public item_datacontext()
        {
            #region 链接指令
            RunCommand = new RelayCommand(run_command);
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            SaveAll = new RelayCommand(SaveAllCommand);
            AddItem = new RelayCommand(AddItemCommand);
            ClearItem = new RelayCommand(ClearItemCommand);
            ImportItemFromClipboard = new RelayCommand(ImportItemFromClipboardCommand);
            ImportItemFromFile = new RelayCommand(ImportItemFromFileCommand);
            #endregion

            #region 初始化成员
            ItemPages itemPage = new() { FontWeight = FontWeights.Normal };
            ItemPageList[0].Content = itemPage;
            #endregion
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        private void AddItemCommand()
        {
            RichTabItems richTabItems = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Header = "物品",
                IsContentSaved = true
            };
            ItemPages itemPage = new() { FontWeight = FontWeights.Normal };
            richTabItems.Content = itemPage;
            ItemPageList.Add(richTabItems);
            if (ItemPageList.Count == 1)
            {
                TabControl tabControl = richTabItems.FindParent<TabControl>();
                tabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        private void ClearItemCommand()
        {
            ItemPageList.Clear();
        }

        /// <summary>
        /// 从剪切板导入
        /// </summary>
        private void ImportItemFromClipboardCommand()
        {
            
        }

        /// <summary>
        /// 从文件导入
        /// </summary>
        private void ImportItemFromFileCommand()
        {

        }

        /// <summary>
        /// 保存所有物品
        /// </summary>
        private void SaveAllCommand()
        {

        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            win.Close();
            Item.cbhk.Topmost = true;
            Item.cbhk.WindowState = WindowState.Normal;
            Item.cbhk.Show();
            Item.cbhk.Topmost = false;
            Item.cbhk.ShowInTaskbar = true;
        }

        /// <summary>
        /// 全部生成
        /// </summary>
        private void run_command()
        {

        }
    }
}

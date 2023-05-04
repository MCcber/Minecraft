using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WK.Libraries.BetterFolderBrowserNS.Helpers;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// Function.xaml 的交互逻辑
    /// </summary>
    public partial class Function : UserControl
    {
        #region 可破坏可放置方块等数据源
        public ObservableCollection<CanDestroyItems> CanDestroyItemsSource { get; set; } = new();
        public ObservableCollection<CanPlaceOnItems> CanPlaceOnItemsSource { get; set; } = new();
        public ObservableCollection<EnchantmentItems> EnchantmentItemsSource { get; set; } = new();
        #endregion

        #region 可破坏方块列表
        private string CanDestroyBlockResult
        {
            get
            {
                string result = CanDestroyItemsSource.Count > 0 ? "CanDestroy:[" + string.Join(',', CanDestroyItemsSource.Select(item => item.Result)) + "]," : "";
                return result;
            }
        }
        #endregion

        #region 可破坏方块列表
        private string CanPlaceOnBlockResult
        {
            get
            {
                string result = CanPlaceOnItemsSource.Count > 0 ? "CanPlaceOn:[" + string.Join(",", CanPlaceOnItemsSource.Select(item => item.Result)) + "]," : "";
                return result;
            }
        }
        #endregion

        #region 附魔列表
        private string EnchantmentResult
        {
            get
            {
                string result = EnchantmentItemsSource.Count > 0 ? "Enchantments:[" + string.Join(",", EnchantmentItemsSource.Select(item => item.Result)) + "]," : "";
                return result;
            }
        }
        #endregion

        #region 材质引用
        private string Material
        {
            get
            {
                string result = material.Text.Trim().Length > 0 ? "material:\"" + material.Text + "\"," : "";
                return result;
            }
        }
        #endregion

        #region 纹饰引用
        private string Pattern
        {
            get
            {
                string result = pattern.Text.Trim().Length > 0 ? "pattern:\"" + pattern.Text + "\"" : "";
                return result;
            }
        }
        #endregion

        #region 盔甲纹饰
        private string TrimResult
        {
            get
            {
                string result = "Trim:{" + (Material.Length > 0 ? Material : "") + (Pattern.Length > 0 ? "," + Pattern : "") + "},";
                return result;
            }
        }
        #endregion

        #region 合并结果
        public string Result
        {
            get
            {
                string result = CanDestroyBlockResult + CanPlaceOnBlockResult + EnchantmentResult + TrimResult;
                return result;
            }
        }
        #endregion

        public Function()
        {
            InitializeComponent();
            DataContext = this;
            #region 连接指令
            //添加
            CanDestroyBlock.Modify = new RelayCommand<FrameworkElement>(AddCanDestroyBlockClick);
            CanPlaceOnBlock.Modify = new RelayCommand<FrameworkElement>(AddCanPlaceOnBlockClick);
            Enchantment.Modify = new RelayCommand<FrameworkElement>(AddEnchantmentClick);
            //清空
            CanDestroyBlock.Fresh = new RelayCommand<FrameworkElement>(ClearCanDestroyBlockClick);
            CanPlaceOnBlock.Fresh = new RelayCommand<FrameworkElement>(ClearCanPlaceOnBlockClick);
            Enchantment.Fresh = new RelayCommand<FrameworkElement>(ClearEnchantmentClick);
            #endregion
            #region 初始化数据
            CanDestroyBlockPanel.ItemsSource = CanDestroyItemsSource;
            CanPlaceOnBlockPanel.ItemsSource = CanPlaceOnItemsSource;
            EnchantmentPanel.ItemsSource = EnchantmentItemsSource;
            #endregion
        }

        #region 附魔、可破坏可放置、属性等数据的编辑和清空

        /// <summary>
        /// 清空附魔数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearEnchantmentClick(FrameworkElement obj)
        {
            EnchantmentItemsSource.Clear();
        }

        /// <summary>
        /// 清空可放置方块数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearCanPlaceOnBlockClick(FrameworkElement obj)
        {
            CanPlaceOnItemsSource.Clear();
        }

        /// <summary>
        /// 清空可破坏方块数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearCanDestroyBlockClick(FrameworkElement obj)
        {
            CanDestroyItemsSource.Clear();
        }

        /// <summary>
        /// 增加附魔列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEnchantmentClick(FrameworkElement obj)
        {
            EnchantmentItemsSource.Add(new EnchantmentItems());
        }

        /// <summary>
        /// 增加可放置方块列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCanPlaceOnBlockClick(FrameworkElement obj)
        {
            CanPlaceOnItemsSource.Add(new CanPlaceOnItems());
        }

        /// <summary>
        /// 增加可破坏方块列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCanDestroyBlockClick(FrameworkElement obj)
        {
            CanDestroyItemsSource.Add(new CanDestroyItems());
        }
        #endregion

        /// <summary>
        /// 为盔甲纹饰引用路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrimData_Click(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowserDialog folderBrowserDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Title = "为盔甲纹饰引用一个命名空间",
                AllowMultiselect = false
            };
            if(folderBrowserDialog.ShowDialog())
            {
                Button button = sender as Button;
                int currentRowIndex = Grid.GetRow(button);
                Grid parent = button.Parent as Grid;
                foreach (FrameworkElement item in parent.Children)
                {
                    int row = Grid.GetRow(item);
                    int column = Grid.GetColumn(item);
                    if(row == currentRowIndex && column == 1)
                    {
                        TextBox textBox = item as TextBox;
                        textBox.Text = folderBrowserDialog.FileName;
                    }
                }
            }
        }
    }
}

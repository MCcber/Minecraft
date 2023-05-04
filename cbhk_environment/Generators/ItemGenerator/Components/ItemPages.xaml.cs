using cbhk_environment.ControlsDataContexts;
using cbhk_environment.GenerateResultDisplayer;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
        private IconComboBoxItem select_item_id_source;
        public IconComboBoxItem SelectItemIdSource
        {
            get { return select_item_id_source; }
            set
            {
                select_item_id_source = value;
                UpdateUILayOut();
            }
        }

        private string ItemId
        {
            get
            {
                string key = MainWindow.ItemDataBase.Where(item => Regex.Match(item.Key, @"[\u4e00-\u9fa5]+").ToString() == SelectItemIdSource.ComboBoxItemText).First().Key;
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
        }

        /// <summary>
        /// 保存指令
        /// </summary>
        private void SaveCommand()
        {

        }

        /// <summary>
        /// 执行生成
        /// </summary>
        private void run_command()
        {
            string nbt;
            nbt = "{" + (Common.Result + Function.Result + Data.Result).TrimEnd(',') + "}";
            if (SelectedVersion == "1.12-")
                Result = "give @p " + ItemId + " " + Data.ItemCount.Value + " " + Data.ItemDamage.Value + " " + nbt;
            else
                Result = "give @p " + ItemId + nbt + " " + Data.ItemCount.Value;
            Displayer displayer = Displayer.GetContentDisplayer();
            displayer.GeneratorResult(Result, "物品", icon_path);
            displayer.Show();
        }

        /// <summary>
        /// 清除不需要的特指数据
        /// </summary>
        private void ClearUnnecessaryDataCommand()
        {

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
        /// 根据物品ID实时显示隐藏特指数据
        /// </summary>
        private void UpdateUILayOut()
        {

        }
    }
}

﻿using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// CanDestroyItems.xaml 的交互逻辑
    /// </summary>
    public partial class CanDestroyItems : UserControl
    {
        private IconComboBoxItem block;
        public IconComboBoxItem Block
        {
            get
            {
                return block;
            }
            set
            {
                block = value;
            }
        }

        public string Result
        {
            get
            {
                string result = "\"" + MainWindow.ItemDataBase.Where(item => item.Key.Split(':')[1] == Block.ComboBoxItemText).Select(item=>item.Key).First().Split(':')[0]+"\"";
                return result;
            }
        }
        public CanDestroyItems()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 删除该控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconTextButtons_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl parent = this.FindParent<ItemsControl>();
            ObservableCollection<CanDestroyItems> canDestroyItems = parent.ItemsSource as ObservableCollection<CanDestroyItems>;
            //删除自己
            canDestroyItems.Remove(this);
            parent.FindParent<Accordion>().FindChild<IconButtons>().Focus();
        }

        /// <summary>
        /// 加载所有子级成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanDestroyItemLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            comboBoxs.ItemsSource = MainWindow.BlockIDSource;
        }
    }
}

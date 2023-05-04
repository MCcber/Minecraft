using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// EnchantmentItems.xaml 的交互逻辑
    /// </summary>
    public partial class EnchantmentItems : UserControl
    {
        private string enchantmentID;
        public string EnchantmentId
        {
            get
            {
                return enchantmentID;
            }
            set
            {
                enchantmentID = value;
            }
        }

        private string enchantmentLevel = "1";
        public string EnchantmentLevel
        {
            get
            {
                return enchantmentLevel;
            }
            set
            {
                enchantmentLevel = value;
            }
        }

        public string Result
        {
            get
            {
                string result = "";
                string id = MainWindow.EnchantmentDataBase.Where(item=>item.Value.Contains(EnchantmentId)).Select(item=>item.Key).First();
                result = "{id:\"minecraft:"+id+"\",lvl:"+EnchantmentLevel+"s}";
                return result;
            }
        }

        public EnchantmentItems()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 载入附魔列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnchantmentIdLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            comboBoxs.ItemsSource = MainWindow.EnchantmentIdSource;
        }

        /// <summary>
        /// 删除该控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconTextButtons_Click(object sender, RoutedEventArgs e)
        {
            StackPanel parent = Parent as StackPanel;
            //删除自己
            parent.Children.Remove(this);
        }
    }
}

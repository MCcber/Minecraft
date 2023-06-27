using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// EnchantmentItems.xaml 的交互逻辑
    /// </summary>
    public partial class EnchantmentItems : UserControl
    {
        public string Result
        {
            get
            {
                string result = "";
                string id = MainWindow.EnchantmentDataBase.Where(item=>Regex.Replace(item.Value,@"\d+","") == ID.SelectedValue.ToString()).Select(item=>item.Key).First();
                result = "{id:\"minecraft:"+id+"\",lvl:"+Level.Value+"s}";
                return result;
            }
        }

        public EnchantmentItems()
        {
            InitializeComponent();
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
            StackPanel parent = this.FindParent<StackPanel>();
            //删除自己
            parent.Children.Remove(this);
            parent.FindParent<Accordion>().FindChild<IconButtons>().Focus();
        }
    }
}

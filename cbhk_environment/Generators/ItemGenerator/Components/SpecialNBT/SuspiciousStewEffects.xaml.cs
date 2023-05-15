using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components.SpecialNBT
{
    /// <summary>
    /// SuspiciousStewEffects.xaml 的交互逻辑
    /// </summary>
    public partial class SuspiciousStewEffects : UserControl
    {
        #region 合并结果
        public string Result
        {
            get
            {
                string currentName = (EffectID.SelectedItem as IconComboBoxItem).ComboBoxItemText;
                string EffectIdResult = MainWindow.MobEffectDataBase.Where(item=>Regex.Replace(item.Value,@"\d+","") == currentName).First().Value;
                EffectIdResult = Regex.Match(EffectIdResult,@"\d+").ToString();
                string result = "{EffectDuration:"+EffectDuration.Value+ ",EffectId:"+ EffectIdResult + "}";
                return result;
            }
        }
        #endregion

        public SuspiciousStewEffects()
        {
            InitializeComponent();
            EffectID.ItemsSource = MainWindow.MobEffectIdSource;
        }

        /// <summary>
        /// 删除该控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconTextButtons_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StackPanel stackPanel = this.FindParent<StackPanel>();
            stackPanel.Children.Remove(this);
            stackPanel.FindParent<Accordion>().FindChild<IconButtons>().Focus();
        }
    }
}

using System.Windows;

namespace cbhk_environment.SettingForm
{
    /// <summary>
    /// IndividualizationForm.xaml 的交互逻辑
    /// </summary>
    public partial class IndividualizationForm
    {
        public IndividualizationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开轮播图设置窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetLinks(object sender, RoutedEventArgs e)
        {
            SetRoatationChart setRoatationChart = new SetRoatationChart();
            if(setRoatationChart.ShowDialog() == true)
            {

            }
        }

        private void SettingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}

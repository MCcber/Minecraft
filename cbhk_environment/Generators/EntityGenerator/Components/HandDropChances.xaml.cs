using System.Windows.Controls;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    /// <summary>
    /// HandDropChances.xaml 的交互逻辑
    /// </summary>
    public partial class HandDropChances : UserControl
    {
        #region 合并数据
        public string Result
        {
            get
            {
                string result = "";
                if (EnableButton.IsChecked.Value)
                {
                    string data = mainhand.Value + "f," + offhand.Value + "f";
                    result = "HandDropChances:[" + data.Trim(',') + "]";
                }
                return result;
            }
        }
        #endregion

        public HandDropChances()
        {
            InitializeComponent();
        }
    }
}

namespace cbhk_environment.GeneralTools.Information
{
    /// <summary>
    /// MessageDisplayer.xaml 的交互逻辑
    /// </summary>
    public partial class MessageDisplayer
    {
        public MessageDisplayer()
        {
            InitializeComponent();
        }

        private void CommonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;Hide();
        }
    }
}

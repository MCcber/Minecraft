using System.Windows;
namespace cbhk_environment.Generators.RecipeGenerator.Components
{
    /// <summary>
    /// ItemsDisplayer.xaml 的交互逻辑
    /// </summary>
    public partial class ItemsDisplayer
    {
        public ItemsDisplayer()
        {
            InitializeComponent();
        }

        private void CommonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}

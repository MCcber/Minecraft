using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    /// <summary>
    /// EntityBag.xaml 的交互逻辑
    /// </summary>
    public partial class EntityBag : UserControl
    {
        public EntityBag()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 从剪切板导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportFromClipboard_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 从文件导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportFromFile_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 删除本实例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconTextButtons_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}

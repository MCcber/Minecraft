using System.Windows.Controls;

namespace cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        /// <summary>
        /// 保存页面容器引用
        /// </summary>
        public Frame PageFrameReference = null;
        /// <summary>
        /// 保存模板选择页面引用
        /// </summary>
        public TemplateSelectPage TemplateSelectPageReference = null;

        public HomePage()
        {
            InitializeComponent();
        }
    }
}

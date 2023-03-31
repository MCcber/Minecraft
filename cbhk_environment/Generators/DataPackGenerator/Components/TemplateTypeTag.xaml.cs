using System.Windows.Controls;

namespace cbhk_environment.Generators.DataPackGenerator.Components
{
    /// <summary>
    /// TemplateTypeTag.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateTypeTag : UserControl
    {
        public TemplateTypeTag(string text)
        {
            InitializeComponent();
            Text.Text = text;
        }
    }
}

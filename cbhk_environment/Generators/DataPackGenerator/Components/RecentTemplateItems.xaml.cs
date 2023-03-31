using cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.DataPackGenerator.Components
{
    /// <summary>
    /// RecentTemplateItems.xaml 的交互逻辑
    /// </summary>
    public partial class RecentTemplateItems : UserControl
    {
        //所属命名空间
        public string FileNameSpace = "";
        //文件路径
        public string FilePath = "";

        //选中后的背景色
        SolidColorBrush SelectedColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3D3D3D"));
        //未选中的背景色
        SolidColorBrush UnSelectColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
        //存储数据上下文的引用
        public TemplateSelectDataContext initializationDatacontext = null;

        /// <summary>
        /// 实例化一个历史模板
        /// </summary>
        /// <param name="filePath">模板路径</param>
        /// <param name="typeName">类型名</param>
        /// <param name="fileImage">模板图标</param>
        /// <param name="templateType">模板类型</param>
        public RecentTemplateItems(string filePath,string fileType,string typeName,string fileImage,string nameSpace)
        {
            InitializeComponent();

            FilePath = filePath;
            FileNameSpace = nameSpace;
            TemplateName.Text = typeName;
            TemplateImage.Source = new BitmapImage(new Uri(fileImage, UriKind.Absolute));
            TemplateType.Children.Add(new TemplateTypeTag(fileType));
        }

        /// <summary>
        /// 选中后同步右侧模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateSelectorChecked(object sender, RoutedEventArgs e)
        {
            Background = SelectedColor;

            if (!TemplateSelectDataContext.TemplateCheckLock)
            {
                //表示开始更新
                TemplateSelectDataContext.TemplateCheckLock = true;

                foreach (TemplateItems templateItems in datapack_datacontext.TemplateList)
                {
                    if (templateItems.TemplateName.Text == TemplateName.Text)
                    {
                        templateItems.TemplateSelector.IsChecked = true;
                        //表示加入已选择模板列表
                        TemplateSelectDataContext.SelectedTemplateItemList.Add(templateItems);
                    }
                }

                //更新完毕
                TemplateSelectDataContext.TemplateCheckLock = false;
            }
        }

        /// <summary>
        /// 取消后取消右侧模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateSelectorUnchecked(object sender, RoutedEventArgs e)
        {
            Background = UnSelectColor;

            if (!TemplateSelectDataContext.TemplateCheckLock)
            {
                //表示开始更新
                TemplateSelectDataContext.TemplateCheckLock = true;

                foreach (TemplateItems templateItems in datapack_datacontext.TemplateList)
                {
                    if (templateItems.TemplateName.Text == TemplateName.Text)
                        templateItems.TemplateSelector.IsChecked = false;
                }

                //更新完毕
                TemplateSelectDataContext.TemplateCheckLock = false;
            }
        }

        /// <summary>
        /// 切换选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControlMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TemplateSelector.IsChecked = !TemplateSelector.IsChecked;
        }
    }
}

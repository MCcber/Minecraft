using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// Data.xaml 的交互逻辑
    /// </summary>
    public partial class Data : UserControl
    {
        //属性数据源
        public ObservableCollection<AttributeItems> AttributeSource = new();

        #region 合并结果
        public string Result
        {
            get
            {
                string result = AttributeSource.Count > 0 ? "AttributeModifiers:[" + string.Join(',', AttributeSource.Select(item =>
                {
                    return item.Result;
                })) + "]" : "";
                return result;
            }
        }
        #endregion

        public Data()
        {
            InitializeComponent();
            Attributes.Modify = new RelayCommand<FrameworkElement>(AddAttributeCommand);
            Attributes.Fresh = new RelayCommand<FrameworkElement>(ClearAttributesComand);
            AttributesPanel.ItemsSource = AttributeSource;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="obj"></param>
        private void AddAttributeCommand(FrameworkElement obj)
        {
            AttributeSource.Add(new AttributeItems() { Margin = new(0,2,0,0) });
        }

        /// <summary>
        /// 清空属性
        /// </summary>
        /// <param name="obj"></param>
        private void ClearAttributesComand(FrameworkElement obj)
        {
            AttributeSource.Clear();
        }
    }
}

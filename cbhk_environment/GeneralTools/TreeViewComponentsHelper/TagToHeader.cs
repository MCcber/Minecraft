using cbhk_environment.CustomControls;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace cbhk_environment.GeneralTools.TreeViewComponentsHelper
{
    /// <summary>
    /// 处理当前节点的Tag属性向控件的转换
    /// </summary>
    public class TagToHeader : IValueConverter
    {
        /// <summary>
        /// 白色刷子
        /// </summary>
        SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        /// <summary>
        /// 黑色刷子
        /// </summary>
        SolidColorBrush blackBrush = new((Color)ColorConverter.ConvertFromString("#000000"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //转换为字符串
            string currentValue = value.ToString();
            //所附着的节点
            RichTreeViewItems parentItem = parameter as RichTreeViewItems;

            DockPanel dockPanel = new()
            {
                LastChildFill = false
            };
            return dockPanel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

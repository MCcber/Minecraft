using cbhk_environment.CustomControls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.GeneralTools.TreeViewComponentsHelper
{
    /// <summary>
    /// 处理每一层控件由Tag转向控件的实际逻辑
    /// </summary>
    public class ReturnTargetComponents
    {
        /// <summary>
        /// 普通文本块的灰刷
        /// </summary>
        private static SolidColorBrush grayBrush = new((Color)ColorConverter.ConvertFromString("#838383"));
        /// <summary>
        /// 白色刷子
        /// </summary>
        private static SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        /// <summary>
        /// 黑色刷子
        /// </summary>
        private static SolidColorBrush blackBrush = new((Color)ColorConverter.ConvertFromString("#000000"));

        /// <summary>
        /// 判断是否需要切换子结构
        /// </summary>
        /// <returns></returns>
        public static bool IsNeedSwitchStrcuture(JObject currentStructure)
        {
            bool result = false;
            JToken HaveEnum = currentStructure["enum"];
            if (HaveEnum != null)
                result = bool.Parse(HaveEnum.ToString());
            return result;
        }

        public static List<FrameworkElement> SetHeader(ref DockPanel container,ref RichTreeViewItems parentItem,List<string> requestList,List<string> enumList = null)
        {
            List<FrameworkElement> result = new();

            #region 封装用于切换子结构的ComboBox
            ComboBox subStructureSwitchBox = new()
            {
                Uid = "enumBox",
                Style = Application.Current.Resources["TextComboBoxStyle"] as Style,
                SelectedIndex = 0,
                FontSize = 12,
            };
            if (enumList != null)
                subStructureSwitchBox.ItemsSource = enumList;
            subStructureSwitchBox.SelectionChanged += SubStructureSwitchBox_SelectionChanged;
            #endregion

            #region 一般的数据控件
            TextBlock displayBlock = new()
            {
                Foreground = whiteBrush,
                FontSize = 12
            };
            TextBlock key = new()
            {
                Foreground = whiteBrush,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            double minValue = 0;
            double maxValue = 0;
            //if (tags[i] == "TAG_Short")
            //{
            //    minValue = short.MinValue;
            //    maxValue = short.MaxValue;
            //}
            //else
            //if (tags[i] == "TAG_Float")
            //{
            //    minValue = float.MinValue;
            //    maxValue = float.MaxValue;
            //}
            //else
            //if (tags[i] == "TAG_Double")
            //{
            //    minValue = double.MinValue;
            //    maxValue = double.MaxValue;
            //}
            //else
            //if (tags[i] == "TAG_Byte")
            //{
            //    minValue = byte.MinValue;
            //    maxValue = byte.MaxValue;
            //}
            //else
            //{
            //    minValue = int.MinValue;
            //    maxValue = int.MaxValue;
            //}
            Slider numberBox = new()
            {
                Margin = new Thickness(5, 0, 0, 0),
                Height = 25,
                Width = 100,
                Value = 0,
                Minimum = minValue,
                Maximum = maxValue,
                Style = Application.Current.Resources["NumberBoxStyle"] as Style,
                FontSize = 12
            };
            TextBox textBox = new()
            {
                Foreground = whiteBrush,
                FontSize = 12
            };
            IconTextButtons generatorButton = new()
            {
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(5),
                Style = Application.Current.Resources["IconTextButton"] as Style,
                Foreground = blackBrush,
                Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonNormal.png", UriKind.RelativeOrAbsolute))),
                PressedBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonPressed.png", UriKind.RelativeOrAbsolute))),
                FontSize = 12
            };
            #endregion

            #region 子节点、用于切换结构
            RichTreeViewItems subItem = new()
            {
                ConnectingLineFill = grayBrush,
                Style = Application.Current.Resources["RichTreeViewItems"] as Style
            };
            subItem.Expanded += CompoundItemExpanded;
            #endregion

            return result;
        }

        /// <summary>
        /// 下拉框切换子结构的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SubStructureSwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 展开复合节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CompoundItemExpanded(object sender, RoutedEventArgs e)
        {

        }
    }
}

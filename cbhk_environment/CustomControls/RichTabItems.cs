using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;
using System.Windows.Data;
using System;
using System.Globalization;

namespace cbhk_environment.CustomControls
{
    public class RichTabItems:TabItem
    {
        #region DependencyProperties
        public BitmapImage HeaderImage
        {
            get { return (BitmapImage)GetValue(HeaderImageProperty); }
            set { SetValue(HeaderImageProperty, value); }
        }

        public static readonly DependencyProperty HeaderImageProperty =
            DependencyProperty.Register("HeaderImage", typeof(BitmapImage), typeof(RichTabItems), new PropertyMetadata(default(BitmapImage)));

        public Brush CloseButtonBackground
        {
            get { return (Brush)GetValue(CloseButtonBackgroundProperty); }
            set { SetValue(CloseButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonBackgroundProperty =
            DependencyProperty.Register("CloseButtonBackground", typeof(Brush), typeof(RichTabItems), new PropertyMetadata(default(Brush)));

        public Brush CloseButtonBorderBrush
        {
            get { return (Brush)GetValue(CloseButtonBorderBrushProperty); }
            set { SetValue(CloseButtonBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonBorderBrushProperty =
            DependencyProperty.Register("CloseButtonBorderBrush", typeof(Brush), typeof(RichTabItems), new PropertyMetadata(default(Brush)));

        public Thickness CloseButtonBorderThickness
        {
            get { return (Thickness)GetValue(CloseButtonBorderThicknessProperty); }
            set { SetValue(CloseButtonBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonBorderThicknessProperty =
            DependencyProperty.Register("CloseButtonBorderThickness", typeof(Thickness), typeof(RichTabItems), new PropertyMetadata(default(Thickness)));

        public Brush CloseButtonForeground
        {
            get { return (Brush)GetValue(CloseButtonForegroundProperty); }
            set { SetValue(CloseButtonForegroundProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonForegroundProperty =
            DependencyProperty.Register("CloseButtonForeground", typeof(Brush), typeof(RichTabItems), new PropertyMetadata(default(Brush)));

        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonVisibilityProperty =
            DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(RichTabItems), new PropertyMetadata(default(Visibility)));

        public bool IsContentSaved
        {
            get { return (bool)GetValue(IsContentSavedProperty); }
            set { SetValue(IsContentSavedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSaved.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsContentSavedProperty =
            DependencyProperty.Register("IsContentSaved", typeof(bool), typeof(RichTabItems), new PropertyMetadata(default(bool)));


        #endregion

        static int current_index = -1;
        static int select_index = -1;
        static RichTabItems current_item = null;
        static RichTabItems select_item = null;
        static bool Draging = false;
        /// <summary>
        /// 对应某个树视图的节点，用于快速得知当前标签页对应的项目文件是否被包括
        /// </summary>
        public RichTreeViewItems mappingItem = null;
        /// <summary>
        /// 对应某个树视图的节点的父级，用于快速得知当前标签页对应的项目文件是否被包括
        /// </summary>
        public RichTreeViewItems mappingParentItem = null;

        public RichTabItems()
        {
            PreviewMouseLeftButtonDown += TabItem_PreviewMouseLeftButtonDown;
            MouseLeftButtonUp += TabItem_MouseLeftButtonUp;
            MouseEnter += TabItem_MouseEnter;
        }

        public void CloseRichTabItemsClick(object sender, RoutedEventArgs e)
        {
            RichTabItems item = (sender as Button).TemplatedParent as RichTabItems;
            TabControl parent = item.Parent as TabControl;
            parent.Items.Remove(item);
        }

        #region 处理拖拽互换位置
        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Draging = true;
            current_item = sender as RichTabItems;
            TabControl current_parent = current_item.Parent as TabControl;
            current_index = current_parent.Items.IndexOf(current_item);
        }

        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(select_item != null && current_item != null)
            {
                SolidColorBrush selectedItemBackground = select_item.Background as SolidColorBrush;
                SolidColorBrush currentItemBackground = current_item.Background as SolidColorBrush;
                SolidColorBrush selectedItemForeground = select_item.Foreground as SolidColorBrush;
                SolidColorBrush currentItemForeground = current_item.Foreground as SolidColorBrush;
                Style currentItemStyle = current_item.Style;
                Style selectedItemStyle = select_item.Style;
                string selectedItemHeaderText = select_item.Header.ToString();
                string currentItemHeaderText = current_item.Header.ToString();
                if (select_index != current_index && select_index != -1 && current_index != -1)
                {
                    TabControl current_parent = (sender as RichTabItems).Parent as TabControl;
                    if (select_item != null && current_item != null && current_index != -1 && select_index != -1)
                    {
                        RichTabItems new_select_item = new RichTabItems()
                        {
                            Header = selectedItemHeaderText,
                            IsContentSaved = true,
                            Content = select_item.Content,
                            Foreground = selectedItemForeground,
                            Background = selectedItemBackground,
                            Style = selectedItemStyle
                        };
                        RichTabItems new_current_item = new RichTabItems()
                        {
                            Header = currentItemHeaderText,
                            IsContentSaved = true,
                            Content = current_item.Content,
                            Foreground = currentItemForeground,
                            Background = currentItemBackground,
                            Style = currentItemStyle
                        };
                        new_select_item.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;
                        new_current_item.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;
                        new_select_item.PreviewMouseLeftButtonDown += TabItem_PreviewMouseLeftButtonDown;
                        new_current_item.PreviewMouseLeftButtonDown += TabItem_PreviewMouseLeftButtonDown;
                        new_select_item.MouseEnter += TabItem_MouseEnter;
                        new_current_item.MouseEnter += TabItem_MouseEnter;

                        current_parent.Items.RemoveAt(select_index);
                        current_parent.Items.Insert(select_index, new_current_item);
                        current_parent.Items.RemoveAt(current_index);
                        current_parent.Items.Insert(current_index, new_select_item);
                        current_parent.SelectedIndex = select_index;
                    }
                }
                Draging = false;
                current_index = -1;
                select_index = -1;
                current_item = null;
                select_item = null;
            }
        }

        private void TabItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Draging)
            {
                RichTabItems current_item = sender as RichTabItems;
                select_item = current_item;
                TabControl current_parent = current_item.Parent as TabControl;
                select_index = current_parent.Items.IndexOf(current_item);
            }

            Grid grid = Template.FindName("templateRoot", this) as Grid;
            if(grid.ToolTip == null && (File.Exists(Uid) || Directory.Exists(Uid)))
            {
                grid.ToolTip = Uid;
                ToolTipService.SetInitialShowDelay(grid, 0);
                ToolTipService.SetShowDuration(grid, 1500);
            }
        }
        #endregion
    }

    public class RichTabItemsSaveMarkVisibilityController : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            bool state = bool.Parse(value.ToString());
            if (state)
                return Visibility.Hidden;
            else 
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

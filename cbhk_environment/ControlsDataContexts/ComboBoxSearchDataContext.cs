using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace cbhk_environment.ControlsDataContexts
{
    public class ComboBoxSearchDataContext
    {
        public Popup pop = new Popup();

        ComboBox current_box;
        public void ItemSearcher(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox box) return;
            if (box.Text.Trim().Length == 0)
            {
                pop.IsOpen = false;
                return;
            }

            if(box.TemplatedParent != null)
            {
                current_box = box.TemplatedParent as ComboBox;
                current_box.IsDropDownOpen = false;

                #region 打开下拉框
                ObservableCollection<IconComboBoxItem> dataGroup = current_box.ItemsSource as ObservableCollection<IconComboBoxItem>;
                var target_data_groups = dataGroup.Where(item => item.ComboBoxItemText.StartsWith(box.Text.Trim()));
                if (target_data_groups.Count() > 1 && box.Text.Trim().Length > 0)
                {
                    pop = CreatePop(pop, target_data_groups, current_box, current_box.ItemTemplate);
                    pop.IsOpen = true;
                }
                #endregion

                #region 搜索目标成员
                IEnumerable<IconComboBoxItem> item_source = current_box.ItemsSource as IEnumerable<IconComboBoxItem>;
                IEnumerable<IconComboBoxItem> select_item = item_source.Where(item => item.ComboBoxItemText == box.Text);
                if (select_item.Count() == 1)
                    current_box.SelectedItem = select_item.First();
                #endregion
            }
        }

        /// <summary>
        /// 展开搜索视图
        /// </summary>
        /// <param name="pop"></param>
        /// <param name="listSource"></param>
        /// <param name="element"></param>
        /// <param name="display_template"></param>
        /// <returns></returns>
        public Popup CreatePop(Popup pop, IEnumerable<IconComboBoxItem> listSource, FrameworkElement element, DataTemplate display_template)
        {
            Border border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(0)
            };

            ScrollViewer viewer = new ScrollViewer()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            ListBox listbox = new ListBox
            {
                Background = null,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                MinWidth = 200,
                MaxHeight = 250,
                ItemsSource = listSource,
                ItemTemplate = display_template
            };

            VirtualizingPanel.SetIsVirtualizing(listbox, true);
            VirtualizingPanel.SetVirtualizationMode(listbox, VirtualizationMode.Recycling);
            ScrollViewer.SetVerticalScrollBarVisibility(listbox, ScrollBarVisibility.Disabled);
            ScrollViewer.SetHorizontalScrollBarVisibility(listbox, ScrollBarVisibility.Disabled);

            viewer.Content = listbox;

            listbox.MouseDoubleClick += Listbox_MouseDoubleClick;

            border.Child = viewer;

            pop.Child = border;

            pop.Placement = PlacementMode.Bottom;

            pop.PlacementTarget = element;

            return pop;
        }

        /// <summary>
        /// 更新已选中成员并更新显示文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox box = sender as ListBox;
            if (box.SelectedItem == null) return;
            IconComboBoxItem selected_item = box.SelectedItem as IconComboBoxItem;
            current_box.SelectedItem = selected_item;
            pop.IsOpen = false;
        }
    }

    public class IconComboBoxItem
    {
        public ImageSource ComboBoxItemIcon { get; set; } = new BitmapImage();
        public string ComboBoxItemText { get; set; } = "";
        public string ComboBoxItemId { get; set; } = "";
    }
}

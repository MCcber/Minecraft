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
            TextBox box = sender as TextBox;
            if (box == null) return;
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

        public Popup CreatePop(Popup pop, IEnumerable<IconComboBoxItem> listSource, FrameworkElement element, DataTemplate display_template)
        {
            Border border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(0)
            };

            //StackPanel panel1 = new StackPanel();
            //panel1.Children.Clear();
            //panel1.Background = new SolidColorBrush(Colors.Black);

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

            //panel1.Children.Add(viewer);

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
    }
    //public class IconComboBoxItem:ObservableObject
    //{
    //    public string item_text;
    //    public string ItemText
    //    {
    //        get { return item_text; }
    //        set
    //        {
    //            item_text = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public string item_image_path;
    //    public string ItemImagePath
    //    {
    //        get { return item_image_path; }
    //        set
    //        {
    //            item_image_path = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public BitmapImage item_image;
    //    public BitmapImage ItemImage
    //    {
    //        get { return item_image; }
    //        set
    //        {
    //            item_image = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public int item_index;
    //    public int ItemIndex
    //    {
    //        get { return item_index; }
    //        set
    //        {
    //            item_index = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}
}

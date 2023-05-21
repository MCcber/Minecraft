using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.GeneralTools.Displayer;
using cbhk_environment.Generators.RecipeGenerator.Components;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.RecipeGenerator
{
    public class recipe_datacontext : ObservableObject
    {
        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        #endregion

        #region 保存物品ID
        private IconComboBoxItem select_item_id_source;
        public IconComboBoxItem SelectItemIdSource
        {
            get { return select_item_id_source; }
            set
            {
                select_item_id_source = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 所有类型的配方
        static CraftingTable craftingTable = null;
        static Furnace furnace = null;
        static BlastFurnace blastFurnace = null;
        static SmithingTable smithingTable = null;
        static Smoker smoker = null;
        static Stonecutter stonecutter = null;
        static CampFire campFire = null;
        static List<UserControl> RecipeTypes = null;
        #endregion

        //配方窗体可视化区域引用
        Grid RecipeZone = null;
        //被抓取的物品
        public static Image GrabedImage = new Image();

        //是否选择物品
        public static bool IsGrabingItem = false;

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconRecipes.png";
        //右侧缓存区引用
        public ListView originalItemViewer = null;
        //滚动视图
        public ScrollViewer scrollViewer = null;
        //拖拽源
        public static Image drag_source = null;

        public ObservableCollection<ItemStructure> ItemsSource { get; set; } = new ObservableCollection<ItemStructure> { };

        #region 已选中的成员
        private ItemStructure selectedItem = null;
        public ItemStructure SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
            }
        }
        #endregion

        #region 搜索值
        private string searchText = "";
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                if(originalItemViewer != null)
                {
                    ReciptItemSource.View.Refresh();
                }
            }
        }
        #endregion

        //数据源对象
        private CollectionViewSource ReciptItemSource = new();

        /// <summary>
        /// 表示是否已订阅搜索事件
        /// </summary>
        bool GotFocused = false;

        //异步锁
        object itemLoadLock = new();

        /// <summary>
        /// 配方操作类型
        /// </summary>
        public enum RecipeModifyTypes
        {
            Add,
            Delete
        }

        public recipe_datacontext()
        {
            #region 链接命令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            RunCommand = new RelayCommand(run_command);
            #endregion

            #region 异步载入物品序列
            BindingOperations.EnableCollectionSynchronization(ItemsSource, itemLoadLock);
            //加载物品集合
            string uriDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
            string urlPath = "";
            Task.Run(() =>
            {
                lock (itemLoadLock)
                {
                    foreach (var item in MainWindow.ItemDataBase)
                    {
                        urlPath = uriDirectoryPath + item.Key[..item.Key.IndexOf(":")] + ".png";
                        if (File.Exists(urlPath))
                            ItemsSource.Add(new ItemStructure(new Uri(urlPath, UriKind.Absolute),item.Key));
                    }
                }
            });
            #endregion
        }

        private void return_command(CommonWindow win)
        {
            Recipe.cbhk.Topmost = true;
            Recipe.cbhk.WindowState = WindowState.Normal;
            Recipe.cbhk.Show();
            Recipe.cbhk.Topmost = false;
            Recipe.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 搜索框获取焦点后执行过滤事件绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(!GotFocused)
            {
                TextBox textBox = sender as TextBox;
                Window window = Window.GetWindow(textBox);
                ReciptItemSource = window.FindResource("RecipeItemSource") as CollectionViewSource;
                ReciptItemSource.Filter += CollectionViewSource_Filter;
                GotFocused = true;
            }
        }

        /// <summary>
        /// 物品过滤事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (SearchText.Trim().Length > 0)
            {
                e.Accepted = false;
                ItemStructure itemStructure = e.Item as ItemStructure;
                string currentItemID = Path.GetFileNameWithoutExtension(itemStructure.ImagePath.ToString());
                string IDAndName = itemStructure.IDAndName;

                if ((currentItemID.Contains(SearchText) && IDAndName.Contains(SearchText)) || (IDAndName.Contains(SearchText) && IDAndName.StartsWith(currentItemID)))
                    e.Accepted = true;
            }
            else
                e.Accepted = true;
        }

        private void run_command()
        {
            string result = "";
            string file_name = "";

            #region 工作台
            result = craftingTable.Visibility == Visibility.Visible ?craftingTable.RecipeData: result;
            file_name = craftingTable.Visibility == Visibility.Visible ? craftingTable.RecipeFileName.Text.Trim() : file_name;
            #endregion

            #region 高炉
            result = blastFurnace.Visibility == Visibility.Visible ?blastFurnace.RecipeData: result;
            file_name = blastFurnace.Visibility == Visibility.Visible ? blastFurnace.RecipeFileName.Text.Trim() : file_name;
            #endregion

            #region 营火
            result = campFire.Visibility == Visibility.Visible ? campFire.RecipeData : result;
            file_name = campFire.Visibility == Visibility.Visible ? campFire.RecipeFileName.Text.Trim() : file_name;
            #endregion

            #region 熔炉
            result = furnace.Visibility == Visibility.Visible ?furnace.RecipeData: result;
            file_name = furnace.Visibility == Visibility.Visible ? furnace.RecipeFileName.Text.Trim() : file_name;
            #endregion

            #region 烟熏炉
            result = smoker.Visibility == Visibility.Visible ?smoker.RecipeData: result;
            file_name = smoker.Visibility == Visibility.Visible ? smoker.RecipeFileName.Text.Trim() : file_name;
            #endregion

            #region 切石机
            result = stonecutter.Visibility == Visibility.Visible ?stonecutter.RecipeData: result;
            file_name = stonecutter.Visibility == Visibility.Visible ? stonecutter.RecipeFileName.Text.Trim() : file_name;
            #endregion

            #region 锻造台
            result = smithingTable.Visibility == Visibility.Visible ?smithingTable.RecipeData:result;
            file_name = smithingTable.Visibility == Visibility.Visible ? smithingTable.RecipeFileName.Text.Trim() : file_name;
            #endregion

            SaveFileDialog folderBrowser = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Title = "请选择配方文件生成路径",
                DefaultExt = "json",
                AddExtension = true,
                Filter = "(*.json)|*.json;",
                RestoreDirectory = true,
                CheckPathExists = true
            };
            if (folderBrowser.ShowDialog().Value == true)
            {
                File.WriteAllText(folderBrowser.FileName, result);
                OpenFolderThenSelectFiles.ExplorerFile(folderBrowser.FileName);
            }

            //Displayer displayer = Displayer.GetContentDisplayer();
            //displayer.GeneratorResult(result ,"配方",icon_path);
            //displayer.Show();
        }

        /// <summary>
        /// 载入物品库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ItemsLoaded(object sender, RoutedEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            originalItemViewer = (tabControl.Items[0] as TextTabItems).Content as ListView;
            originalItemViewer.DataContext = this;
            originalItemViewer.MouseMove += Bag_MouseMove;
            originalItemViewer.PreviewMouseLeftButtonDown += SelectItemClickDown;
            originalItemViewer.MouseLeave += ListBox_MouseLeave;
        }

        /// <summary>
        /// 载入左侧切换栏图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IconViewerLoaded(object sender, RoutedEventArgs e)
        {
            StackPanel parent = sender as StackPanel;
            Style btn_style = (parent.Children[0] as IconTextButtons).Style;
            //获取所有配方图标文件
            string[] icon_files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Recipe\\images");
            for (int i = 0; i < icon_files.Length; i++)
            {
                string current_file_name = Path.GetFileName(icon_files[i]);
                if (current_file_name.Contains("icon.png") && current_file_name.Length > 8)
                {
                    IconTextButtons iconTextButtons = new IconTextButtons
                    {
                        Background = new ImageBrush(new BitmapImage(new Uri(icon_files[i], UriKind.Absolute))),
                        PressedBackground = new ImageBrush(new BitmapImage(new Uri(icon_files[i], UriKind.Absolute))),
                        Tag = icon_files[i],
                        Height = 50,
                        Width = 50,
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        BorderThickness = new Thickness(1),
                        Style = btn_style,
                        ClickMode = ClickMode.Release
                    };
                    iconTextButtons.Click += RecipeTyleSwitcher;
                    parent.Children.Add(iconTextButtons);
                }
            }
        }

        /// <summary>
        /// 切换配方类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecipeTyleSwitcher(object sender, RoutedEventArgs e)
        {
            IconTextButtons textButtons = sender as IconTextButtons;
            foreach (var item in RecipeTypes)
            {
                if (Path.GetFileNameWithoutExtension(textButtons.Tag.ToString()).Replace("_icon","") == item.Tag.ToString())
                {
                    Panel.SetZIndex(item,1);
                    item.Opacity = 1;
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    Panel.SetZIndex(item, 0);
                    item.Opacity = 0;
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 载入编辑区控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ModifyGridLoaded(object sender, RoutedEventArgs e)
        {
            Grid zone = sender as Grid;

            #region 初始化所有配方类型
            craftingTable = new CraftingTable();
            furnace = new Furnace();
            blastFurnace = new BlastFurnace();
            smithingTable = new SmithingTable();
            smoker = new Smoker();
            stonecutter = new Stonecutter();
            campFire = new CampFire();
            #endregion

            #region 连接所有类型的配方引用
            RecipeTypes = new List<UserControl> { };
            RecipeTypes.Add(craftingTable);
            RecipeTypes.Add(furnace);
            RecipeTypes.Add(blastFurnace);
            RecipeTypes.Add(smithingTable);
            RecipeTypes.Add(smoker);
            RecipeTypes.Add(stonecutter);
            RecipeTypes.Add(campFire);
            #endregion

            #region 添加所有类型的配方
            zone.Children.Add(furnace);
            zone.Children.Add(blastFurnace);
            zone.Children.Add(smithingTable);
            zone.Children.Add(smoker);
            zone.Children.Add(stonecutter);
            zone.Children.Add(craftingTable);
            zone.Children.Add(campFire);
            furnace.Opacity = blastFurnace.Opacity = smithingTable.Opacity = smoker.Opacity = stonecutter.Opacity = campFire.Opacity = 0;
            furnace.Visibility = blastFurnace.Visibility = smithingTable.Visibility = smoker.Visibility = stonecutter.Visibility = campFire.Visibility = Visibility.Collapsed;
            #endregion
        }

        /// <summary>
        /// 处理开始拖拽物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelectItemClickDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Image)
                SelectedItem = null;
            if (SelectedItem != null)
            {
                IsGrabingItem = true;

                DependencyObject obj = (DependencyObject)e.OriginalSource;
                while (obj != null && obj is not ListViewItem)
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }
                ListViewItem item = obj as ListViewItem;
                ItemStructure itemStructure = item.Content as ItemStructure;

                drag_source = new Image
                {
                    Source = new BitmapImage(SelectedItem.ImagePath),
                    Tag = itemStructure.IDAndName[..itemStructure.IDAndName.IndexOf(':')]
                };
                GrabedImage = drag_source;

                if (IsGrabingItem && drag_source != null)
                {
                    DataObject dataObject = new(typeof(Image), GrabedImage);
                    if (dataObject != null)
                        DragDrop.DoDragDrop(drag_source, dataObject, DragDropEffects.Move);
                    IsGrabingItem = false;
                }
            }
        }

        /// <summary>
        /// 获取窗体可视化区域引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RecipeZoneLoaded(object sender, RoutedEventArgs e)
        {
            RecipeZone = sender as Grid;
            RecipeZone.MouseMove += SelectItemMove;
            RecipeZone.MouseLeftButtonUp += RecipeZoneMouseLeftButtonUp;
        }

        /// <summary>
        /// 右击删除该物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteItemClick(object sender, MouseButtonEventArgs e)
        {
            UniformGrid parent = (sender as Image).Parent as UniformGrid;
            parent.Children.Remove(sender as Image);
        }

        /// <summary>
        /// 松开后停止拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RecipeZoneMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsGrabingItem = false;
        }

        /// <summary>
        /// 处理拖拽物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectItemMove(object sender, MouseEventArgs e)
        {
            if (IsGrabingItem && drag_source != null && GrabedImage != null)
            {
                DataObject dataObject = new(typeof(Image), GrabedImage);
                if(dataObject != null)
                DragDrop.DoDragDrop(drag_source, dataObject, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// 检测预览视图是否有内容重叠
        /// </summary>
        private static int CheckOverLap(Image image)
        {
            int index = -1;
            int count = craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children.Count;
            Image current_image;
            for (int i = 0; i < count; i++)
            {
                current_image = craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children[i] as Image;
                if(current_image.Tag == image.Tag)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// 更新右侧多选预览视图
        /// </summary>
        /// <param name="image">图像控件</param>
        /// <param name="delete">是否删除</param>
        /// <param name="cover">是否覆盖</param>
        public static void UpdateMultipleItemView(Image image,RecipeModifyTypes modifyTypes,List<Image> images)
        {
            int index = CheckOverLap(image);
            switch (modifyTypes)
            {
                case RecipeModifyTypes.Add:
                    if (index == -1)
                        craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children.Add(image);
                    else
                        craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children[index].Visibility = Visibility.Visible;
                    break;
                case RecipeModifyTypes.Delete:
                    images.Remove(image);
                    int result = 0;
                    result += CraftingTable.FirstItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.SecondItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.ThirdItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.FourthItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.FifthItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.SixthItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.SeventhItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.EighthItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    result += CraftingTable.NinthItem.Find(item => item.Tag == image.Tag) != null ? 1 : 0;
                    if (result >= 2)
                        craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children[index].Visibility = Visibility.Collapsed;
                    else
                        craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children.RemoveAt(index);
                    if (craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children.Count == 1)
                        craftingTable.ItemInfomationWindow.CurrentItemInfomation.Children[0].Visibility = Visibility.Visible;
                    break;
            }
        }

        public void ListBox_MouseLeave(object sender, MouseEventArgs e)
        {
            SelectedItem = null;
        }

        /// <summary>
        /// 鼠标移入背包后选中对应的成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Bag_MouseMove(object sender, MouseEventArgs e)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(originalItemViewer, Mouse.GetPosition(originalItemViewer));
            if(hitTestResult != null)
            {
                var item = hitTestResult.VisualHit;
                while (item != null && item is not ListViewItem)
                    item = VisualTreeHelper.GetParent(item);

                if (item != null)
                {
                    int i = originalItemViewer.Items.IndexOf(((ListViewItem)item).DataContext);
                    if (i >= 0 && i < originalItemViewer.Items.Count)
                        originalItemViewer.SelectedIndex = i;
                }
            }
        }
    }
}

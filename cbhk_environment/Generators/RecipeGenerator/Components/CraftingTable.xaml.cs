using cbhk_environment.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.RecipeGenerator.Components
{
    /// <summary>
    /// CraftingTable.xaml 的交互逻辑
    /// </summary>
    public partial class CraftingTable : UserControl
    {
        #region 1到9号物品数据
        public static List<Image> FirstItem = new List<Image> { };
        public static List<Image> SecondItem = new List<Image> { };
        public static List<Image> ThirdItem = new List<Image> { };
        public static List<Image> FourthItem = new List<Image> { };
        public static List<Image> FifthItem = new List<Image> { };
        public static List<Image> SixthItem = new List<Image> { };
        public static List<Image> SeventhItem = new List<Image> { };
        public static List<Image> EighthItem = new List<Image> { };
        public static List<Image> NinthItem = new List<Image> { };
        #endregion

        #region 知识之书图标
        System.Drawing.Bitmap KnowLedgeBook = null;
        BitmapImage KnowLedgeBookImage = null;
        #endregion

        #region 默认键值
        List<char> DefaultKeyList = new List<char> { 'm','j','s','b','c','d','e','f','g' };
        int defaultKeyListIndex = 0;
        #endregion

        //存储九宫格容器引用
        public static Grid CraftingTableGrid = null;

        //存储合成模式的引用
        RadiusToggleButtons IsShaped = null;

        //空白图像
        System.Windows.Media.ImageSource empty_image = null;

        #region 1到9号物品对应的键
        public List<string> Keys
        {
            get { return (List<string>)GetValue(FirstKeyProperty); }
            set { SetValue(FirstKeyProperty, value); }
        }

        public static readonly DependencyProperty FirstKeyProperty =
            DependencyProperty.Register("Keys", typeof(List<string>), typeof(CraftingTable), new PropertyMetadata(default(List<string>)));
        #endregion

        //是否显示当前槽位物品
        public static bool DisplayCurrentItem = false;

        //显示物品信息的窗体
        public ItemsDisplayer ItemInfomationWindow = new ItemsDisplayer();

        //获取生成数量的引用
        Slider RecipeCount = null;
        
        //获取文件名引用
        public TextBox RecipeFileName = null;

        #region 当前槽位被点击子级物品,用于绑定单个物品的标签
        private Image currentSubItem = null;
        public Image CurrentSubItem
        {
            get { return currentSubItem; }
            set { currentSubItem = value; }
        }
        #endregion

        #region 当前选中的物品
        private Image currentItemImage = null;
        public Image CurrentItemImage
        {
            get { return currentItemImage; }
            set { currentItemImage = value; }
        }
        #endregion

        #region 生成结果
        private string recipe_result = "";
        public string RecipeResult
        {
            get
            {
                string result = "";
                RecipeCount.Value = int.Parse(RecipeCount.Value.ToString().Contains(".") ? RecipeCount.Value.ToString().Split('.')[0].Replace("-", "").Trim() : RecipeCount.Value.ToString().Replace("-", "").Trim());
                result = "\"result\":{\"item\":\"minecraft:" + recipe_result + "\",\"count\":"+ RecipeCount.Value +"}";
                return result;
            }
            set { recipe_result = value; }
        }
        #endregion

        #region 最终数据
        public string RecipeData
        {
            get
            {
                Dictionary<string, List<string>> Content = new Dictionary<string, List<string>> { };
                List<List<Image>> ItemIdList = new List<List<Image>> { FirstItem,SecondItem,ThirdItem,FourthItem,FifthItem,SixthItem,SeventhItem,EighthItem,NinthItem };
                string result = "";
                for (int i = 0; i < Keys.Count; i++)
                {
                    foreach (Image a_item in ItemIdList[i])
                    {
                        if (!Content.ContainsKey(Keys[i]))
                            Content.Add(Keys[i], ItemIdList[i].Select(item => item.Tag.ToString()).ToList());
                        else
                            Content[Keys[i]].AddRange(ItemIdList[i].Select(item => item.Tag.ToString()).ToList());
                    }
                }
                #region 处理有序和无序
                if(!IsShaped.IsChecked.Value)
                {
                    string PatternData = "\"pattern\":[";
                    string KeyData = "\"key\":{";
                    for (int i = 0; i < Keys.Count; i+=3)
                    {
                        PatternData += "\"" + Keys[i] + " " + Keys[i+1] + " " + Keys[i+2] +"\",";
                    }
                    for (int i = 0; i < ItemIdList.Count; i++)
                    {
                        if (ItemIdList[i].Count > 1)
                            KeyData += "\"" + Keys[i] + "\":[" + string.Join(",", ItemIdList[i].Select(item => "{\"item\":\"minecraft:" + item.Tag.ToString() + "\""+ (item.Name.Trim() != "" ? ",\"tag\":\"" + item.Name + "\"" : "") + "}").ToArray()).TrimEnd(',') + "],";
                        else
                        if (ItemIdList[i].Count == 1)
                            KeyData += "\"" + Keys[i] + "\":{" + "\"item\":\"minecraft:" + ItemIdList[i].First().Tag.ToString() + "\""+ (ItemIdList[i].First().Name.Trim() != "" ? ",\"tag\":\"" + ItemIdList[i].First().Name + "\"" : "") + "},";
                    }
                    PatternData = PatternData.TrimEnd(',') + "],";
                    KeyData = KeyData.TrimEnd(',') + "},";
                    result = "{\"type\": \"minecraft:crafting_shaped\"," + (GroupId.Text.Trim() != "" ? "\"group\":\"" + GroupId.Text + "\"," : "") + PatternData + KeyData + RecipeResult + "}";
                }
                else
                {
                    string IngredientData = "\"ingredients\": [";
                    for (int i = 0; i < ItemIdList.Count; i++)
                    {
                        if (ItemIdList[i].Count == 1)
                            IngredientData += "{\"item\": \"minecraft:" + ItemIdList[i].First().Tag.ToString() + "\""+ (ItemIdList[i].First().Name.Trim() != "" ? ",\"tag\":\"" + ItemIdList[i].First().Name + "\"" : "") + "},";
                        else
                            if (ItemIdList[i].Count > 1)
                            IngredientData += "[" + string.Join(",", ItemIdList[i].Select(item => "{\"item\":\"minecraft:" + item.Tag.ToString() + "\""+ (item.Name.Trim() != "" ? ",\"tag\":\"" + item.Name + "\"" : "") + "}").ToArray()).TrimEnd(',') + "],";
                    }
                    IngredientData = IngredientData.TrimEnd(',') + "],";
                    result = "{  \"type\": \"minecraft:crafting_shapeless\"," + (GroupId.Text.Trim() != "" ? "\"group\":\"" + GroupId.Text + "\"," : "") + IngredientData + RecipeResult + "}";
                }
                #endregion
                return result;
            }
        }
        #endregion

        #region 是否为有序合成
        private bool shaped = true;
        public bool Shaped
        {
            get { return shaped; }
            set { shaped = value; }
        }
        #endregion

        #region 是否开启多选模式
        private bool multiple_mode = false;
        public bool MultipleMode
        {
            get { return multiple_mode; }
            set { multiple_mode = value; }
        }
        #endregion

        recipe_datacontext.RecipeModifyTypes modifyTypes = new recipe_datacontext.RecipeModifyTypes();

        public CraftingTable()
        {
            InitializeComponent();

            #region 初始化知识之书
            KnowLedgeBook = new System.Drawing.Bitmap(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Recipe\\images\\knowledge_book.png");
            KnowLedgeBook = GeneralTools.ChangeBitmapSize.Magnifier(KnowLedgeBook, 10);
            KnowLedgeBookImage = GeneralTools.BitmapImageConverter.ToBitmapImage(KnowLedgeBook);
            #endregion

            ItemInfomationWindow.DataContext = this;

            Keys = new List<string> { "","","","","","","","","" };
        }

        /// <summary>
        /// 载入背景图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundImageLoaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory+ "resources\\configs\\Recipe\\images\\crafting_table.png", UriKind.Absolute));
        }

        /// <summary>
        /// 获取多选模式引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultipleModeLoaded(object sender, RoutedEventArgs e)
        {
            TextToggleButtons textToggleButtons = sender as TextToggleButtons;
            textToggleButtons.DataContext = this;
            Binding binding = new Binding()
            {
                Path = new PropertyPath("MultipleMode"),
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(textToggleButtons, TextToggleButtons.IsCheckedProperty, binding);
        }

        /// <summary>
        /// 获取九宫格容器引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CraftingTableGridLoaded(object sender, RoutedEventArgs e)
        {
            CraftingTableGrid = sender as Grid;
        }

        /// <summary>
        /// 获取合成模式的引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsShapeLoaded(object sender, RoutedEventArgs e)
        {
            IsShaped = sender as RadiusToggleButtons;
        }

        /// <summary>
        /// 移除Tag中的非法字符
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveIllegalCharacter(object sender, KeyEventArgs e)
        {
            TextBox current_box = sender as TextBox;
            current_box.Text = Regex.Replace(current_box.Text, @"[\\/:*?" + "\"" + "<>|]", "").ToString();
            //current_box.Text = Regex.Replace(current_box.Text, @"^\d+", "").ToString();
        }

        /// <summary>
        /// 绑定单个物品的tag数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemTagBinder(object sender, MouseButtonEventArgs e)
        {
            #region 动态绑定每个槽位的标签信息
            CurrentSubItem = sender as Image;
            Binding tag_binder = new Binding()
            {
                Path = new PropertyPath("CurrentSubItem.Name"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            BindingOperations.SetBinding(ItemInfomationWindow.TagBox, TextBox.TextProperty, tag_binder);
            #endregion
        }

        /// <summary>
        /// 删除对应槽位的物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCacheImage(object sender, MouseButtonEventArgs e)
        {
            Image current_image = sender as Image;
            modifyTypes = recipe_datacontext.RecipeModifyTypes.Delete;
            switch (current_image.Uid)
            {
                case "0":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, FirstItem);
                    Border image_border0 = CraftingTableGrid.Children[0] as Border;
                    Image image0 = image_border0.Child as Image;
                    if (FirstItem.Count == 0)
                        image0.Source = empty_image;
                    else
                        if (FirstItem.Count == 1)
                        image0.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "1":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, SecondItem);
                    Border image_border1 = CraftingTableGrid.Children[1] as Border;
                    Image image1 = image_border1.Child as Image;
                    if (SecondItem.Count == 0)
                        image1.Source = empty_image;
                    else
                        if (SecondItem.Count == 1)
                        image1.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "2":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, ThirdItem);
                    Border image_border2 = CraftingTableGrid.Children[2] as Border;
                    Image image2 = image_border2.Child as Image;
                    if (ThirdItem.Count == 0)
                        image2.Source = empty_image;
                    else
                        if (ThirdItem.Count == 1)
                        image2.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "3":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, FourthItem);
                    Border image_border3 = CraftingTableGrid.Children[3] as Border;
                    Image image3 = image_border3.Child as Image;
                    if (FourthItem.Count == 0)
                        image3.Source = empty_image;
                    else
                        if (FourthItem.Count == 1)
                        image3.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "4":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, FifthItem);
                    Border image_border4 = CraftingTableGrid.Children[4] as Border;
                    Image image4 = image_border4.Child as Image;
                    if (FifthItem.Count == 0)
                        image4.Source = empty_image;
                    else
                        if (FifthItem.Count == 1)
                        image4.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "5":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, SixthItem);
                    Border image_border5 = CraftingTableGrid.Children[5] as Border;
                    Image image5 = image_border5.Child as Image;
                    if (SixthItem.Count == 0)
                        image5.Source = empty_image;
                    else
                        if (SixthItem.Count == 1)
                        image5.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "6":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, SeventhItem);
                    Border image_border6 = CraftingTableGrid.Children[6] as Border;
                    Image image6 = image_border6.Child as Image;
                    if (SeventhItem.Count == 0)
                        image6.Source = empty_image;
                    else
                        if (SeventhItem.Count == 1)
                        image6.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "7":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, EighthItem);
                    Border image_border7 = CraftingTableGrid.Children[7] as Border;
                    Image image7 = image_border7.Child as Image;
                    if (EighthItem.Count == 0)
                        image7.Source = empty_image;
                    else
                        if (EighthItem.Count == 1)
                        image7.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
                case "8":
                    recipe_datacontext.UpdateMultipleItemView(current_image, modifyTypes, NinthItem);
                    Border image_border8 = CraftingTableGrid.Children[8] as Border;
                    Image image8 = image_border8.Child as Image;
                    if (NinthItem.Count == 0)
                        image8.Source = empty_image;
                    else
                        if (NinthItem.Count == 1)
                        image8.Source = (ItemInfomationWindow.CurrentItemInfomation.Children[0] as Image).Source;
                    break;
            }
        }

        /// <summary>
        /// 更新物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemUpdate(object sender, DragEventArgs e)
        {
            recipe_datacontext.IsGrabingItem = !recipe_datacontext.IsGrabingItem;

            #region 放大当前图像
            Image current_item = sender as Image;
            string item_id = recipe_datacontext.GrabedImage.Tag.ToString();
            string image_path = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + item_id + ".png";
            BitmapImage bitmapImage = new BitmapImage(new Uri(image_path,UriKind.Absolute));
            #endregion

            #region 获取空图像引用
            if (empty_image == null)
                empty_image = current_item.Source;
            #endregion

            #region 右侧预览新增图像
            Image cache_image = new Image()
            {
                Height = current_item.Height,
                Cursor = Cursors.Hand,
                Width = current_item.Width,
                Tag = item_id,
                ToolTip = item_id + " 右击删除",
                Uid = current_item.Uid,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            RenderOptions.SetBitmapScalingMode(cache_image,BitmapScalingMode.HighQuality);
            RenderOptions.SetClearTypeHint(cache_image,ClearTypeHint.Enabled);
            ToolTipService.SetInitialShowDelay(cache_image, 0);
            ToolTipService.SetShowDuration(cache_image, 1000);
            cache_image.MouseRightButtonDown += DeleteCacheImage;
            cache_image.MouseLeftButtonDown += ItemTagBinder;
            #endregion

            current_item.Tag = item_id;
            modifyTypes = recipe_datacontext.RecipeModifyTypes.Add;
            switch (current_item.Uid)
            {
                case "0":
                    if (MultipleMode || FirstItem.Count == 0)
                    {
                        if(FirstItem.Where(item=>item.Tag == cache_image.Tag).Count() == 0)
                        FirstItem.Add(cache_image);
                        current_item.Source = FirstItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, FirstItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        FirstItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, FirstItem);
                    }
                    break;
                case "1":
                    if (MultipleMode || SecondItem.Count == 0)
                    {
                        if (SecondItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            SecondItem.Add(cache_image);
                        current_item.Source = SecondItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, SecondItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        SecondItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, SecondItem);
                    }
                    break;
                case "2":
                    if (MultipleMode || ThirdItem.Count == 0)
                    {
                        if (ThirdItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            ThirdItem.Add(cache_image);
                        current_item.Source = ThirdItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, ThirdItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        ThirdItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, ThirdItem);
                    }
                    break;
                case "3":
                    if (MultipleMode || FourthItem.Count == 0)
                    {
                        if (FirstItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            FourthItem.Add(cache_image);
                        current_item.Source = FourthItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, FourthItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        FourthItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, FourthItem);
                    }
                    break;
                case "4":
                    if (MultipleMode || FifthItem.Count == 0)
                    {
                        if (FifthItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            FifthItem.Add(cache_image);
                        current_item.Source = FifthItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, FifthItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        FifthItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, FifthItem);
                    }
                    break;
                case "5":
                    if (MultipleMode || SixthItem.Count == 0)
                    {
                        if (SixthItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            SixthItem.Add(cache_image);
                        current_item.Source = SixthItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, SixthItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        SixthItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, SixthItem);
                    }
                    break;
                case "6":
                    if (MultipleMode || SeventhItem.Count == 0)
                    {
                        if (SeventhItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            SeventhItem.Add(cache_image);
                        current_item.Source = SeventhItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, SeventhItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        SeventhItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, SeventhItem);
                    }
                    break;
                case "7":
                    if (MultipleMode || EighthItem.Count == 0)
                    {
                        if (EighthItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            EighthItem.Add(cache_image);
                        current_item.Source = EighthItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, EighthItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        EighthItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, EighthItem);
                    }
                    break;
                case "8":
                    if (MultipleMode || NinthItem.Count == 0)
                    {
                        if (NinthItem.Where(item => item.Tag == cache_image.Tag).Count() == 0)
                            NinthItem.Add(cache_image);
                        current_item.Source = NinthItem.Count < 2 ? bitmapImage : KnowLedgeBookImage;
                        cache_image.Source = bitmapImage;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, NinthItem);
                    }
                    else
                    {
                        current_item.Source = bitmapImage;
                        NinthItem[0] = cache_image;
                        cache_image.Source = current_item.Source;
                        recipe_datacontext.UpdateMultipleItemView(cache_image, modifyTypes, NinthItem);
                    }
                    break;
                case "9":
                    current_item.Source = bitmapImage;
                    RecipeResult = current_item.Tag.ToString();
                    break;
            }
        }

        /// <summary>
        /// 分配默认键值 
        /// </summary>
        private char SetDefaultKey()
        {
            if (defaultKeyListIndex >= DefaultKeyList.Count)
                defaultKeyListIndex = 0;
            char result = DefaultKeyList[defaultKeyListIndex];
            defaultKeyListIndex++;
            return result;
        }

        /// <summary>
        /// 选中物品,更新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectItemClick(object sender, MouseButtonEventArgs e)
        {
            DisplayCurrentItem = !DisplayCurrentItem;
            Image image = sender as Image;
            Border border = image.Parent as Border;
            if (image != CurrentItemImage && CurrentItemImage != null)
            {
                Border last_parent = CurrentItemImage.Parent as Border;
                last_parent.BorderThickness = new Thickness(0);
                DisplayCurrentItem = true;
            }
            border.BorderThickness = DisplayCurrentItem? new Thickness(5):new Thickness(0);
            CurrentItemImage = image;

            #region 动态绑定每个槽位的键信息
            Binding key_binder = new Binding()
            {
                Path = new PropertyPath("Keys["+int.Parse(image.Uid)+"]"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            BindingOperations.SetBinding(ItemInfomationWindow.KeyBox, TextBox.TextProperty, key_binder);
            #endregion

            #region 动态"绑定"每个槽位的物品信息
            switch (image.Uid)
            {
                case "0":
                    if (FirstItem.Count > 0)
                    {
                        if(FirstItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = FirstItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "1":
                    if (SecondItem.Count > 0)
                    {
                        if (SecondItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = SecondItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "2":
                    if (ThirdItem.Count > 0)
                    {
                        if (ThirdItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = ThirdItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "3":
                    if (FourthItem.Count > 0)
                    {
                        if (FourthItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = FourthItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "4":
                    if (FifthItem.Count > 0)
                    {
                        if (FifthItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = FifthItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "5":
                    if (SixthItem.Count > 0)
                    {
                        if (SixthItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = SixthItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "6":
                    if (SeventhItem.Count > 0)
                    {
                        if (SeventhItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = SeventhItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "7":
                    if (EighthItem.Count > 0)
                    {
                        if (EighthItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = EighthItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
                case "8":
                    if (NinthItem.Count > 0)
                    {
                        if (NinthItem.Count == 1 && ItemInfomationWindow.KeyBox.Text.Trim() == "")
                            ItemInfomationWindow.KeyBox.Text = SetDefaultKey().ToString();
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                        {
                            int count = NinthItem.Where(a_image => a_image.Tag == item.Tag).Count();
                            if (count > 0)
                                item.Visibility = Visibility.Visible;
                            else
                                item.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                        foreach (Image item in ItemInfomationWindow.CurrentItemInfomation.Children)
                            item.Visibility = Visibility.Collapsed;
                    break;
            }
            #endregion

            if (DisplayCurrentItem)
            {
                Window window = Window.GetWindow(image);
                Point point = image.TransformToAncestor(window).Transform(new Point(0, 0));
                point = PointToScreen(point);
                ItemInfomationWindow.Left = point.X-180;
                ItemInfomationWindow.Top = point.Y-400;
                ItemInfomationWindow.Show();
            }
            else
                ItemInfomationWindow.Hide();
        }

        /// <summary>
        /// 结果数量点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResultCountLoaded(object sender, RoutedEventArgs e)
        {
            RecipeCount = sender as Slider;
        }

        /// <summary>
        /// 获取文件名引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecipeFileNameLoaded(object sender, RoutedEventArgs e)
        {
            RecipeFileName = sender as TextBox;
        }

        /// <summary>
        /// 抬起右键后删除合成结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRecipeResultClick(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            image.Source = empty_image;
            RecipeResult = "";
        }
    }
}

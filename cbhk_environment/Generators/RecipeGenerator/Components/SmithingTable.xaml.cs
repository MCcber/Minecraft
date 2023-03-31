using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static cbhk_environment.Generators.RecipeGenerator.recipe_datacontext;

namespace cbhk_environment.Generators.RecipeGenerator.Components
{
    /// <summary>
    /// SmithingTable.xaml 的交互逻辑
    /// </summary>
    public partial class SmithingTable : UserControl
    {
        //获取基础物品的引用
        Image BasedItem = null;
        //获取材料的引用
        Image AdditionItem = null;
        //空内容图像
        ImageSource empty_image = null;
        //背景图文件路径
        string background_path = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Recipe\\images\\smithing_table.png";

        //是否显示当前槽位物品
        public static bool DisplayCurrentItem = false;

        //获取经验的引用
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

        #region 基础物品标签
        private string basedItemTag = "";
        public string BasedItemTag
        {
            get
            {
                return basedItemTag;
            }
            set
            {
                basedItemTag = value;
            }
        }
        #endregion

        #region 材料标签
        private string additionTag = "";
        public string AdditionTag
        {
            get
            {
                return additionTag;
            }
            set
            {
                additionTag = value;
            }
        }
        #endregion

        #region 生成结果
        private string recipe_result = "";
        public string RecipeResult
        {
            get
            {
                string result;
                string CountData = ",\"count\":";

                RecipeCount.Value = int.Parse(RecipeCount.Value.ToString().Contains(".") ? RecipeCount.Value.ToString().Split('.')[0].Replace("-", "") : RecipeCount.Value.ToString());
                CountData += RecipeCount.Value;
                result = "\"result\":\"minecraft:" + recipe_result + "\"" + CountData;
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
                string result = "{\"type\":\"minecraft:smithing\"," + (GroupId.Text.Trim() != "" ? "\"group\":\"" + GroupId.Text + "\"," : "");
                string BaseData = "";
                string AdditionData = "";
                if(BasedItem.Tag != null)
                {
                    BaseData = string.Join("", "\"item\":\"minecraft:" + BasedItem.Tag.ToString() + "\""+ (BasedItemTag.Trim() != "" ? ",\"tag\":\"" + BasedItemTag + "\"" : "") + "");
                    BaseData = "\"base\":{" + BaseData + "},";
                }
                if(AdditionItem.Tag != null)
                {
                    AdditionData = string.Join("", "\"item\":\"minecraft:" + AdditionItem.Tag.ToString() + "\""+ (AdditionTag.Trim() != "" ? ",\"tag\":\"" + AdditionTag.Trim() + "\"" : "") + "");
                    AdditionData = "\"addition\":{" + AdditionData + "},";
                }
                result += BaseData + AdditionData + RecipeResult + "}";
                return result;
            }
        }
        #endregion

        public SmithingTable()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 更新物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemUpdate(object sender, DragEventArgs e)
        {
            IsGrabingItem = !IsGrabingItem;

            #region 放大当前图像
            Image current_item = sender as Image;
            string item_id = GrabedImage.Tag.ToString();
            string image_path = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + item_id + ".png";
            BitmapImage bitmapImage = new BitmapImage(new Uri(image_path, UriKind.Absolute));
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
            };
            ToolTipService.SetInitialShowDelay(cache_image, 0);
            ToolTipService.SetShowDuration(cache_image, 1000);
            cache_image.MouseRightButtonUp += DeleteClick;
            #endregion

            ToolTipService.SetInitialShowDelay(current_item, 0);
            ToolTipService.SetShowDuration(current_item, 1000);
            current_item.Tag = item_id;
            switch (current_item.Uid)
            {
                case "0":
                    BasedItem.Source = bitmapImage;
                    BasedItem.Tag = item_id;
                    cache_image.Source = BasedItem.Source;
                    break;
                case "1":
                    AdditionItem.Source = bitmapImage;
                    AdditionItem.Tag = item_id;
                    cache_image.Source = AdditionItem.Source;
                    break;
                case "2":
                    current_item.Source = bitmapImage;
                    RecipeResult = current_item.Tag.ToString();
                    break;
            }
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
        /// 载入背景图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundLoaded(object sender, RoutedEventArgs e)
        {
            Image current_image = sender as Image;
            current_image.Source = new BitmapImage(new Uri(background_path, UriKind.Absolute));
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
        /// 获取经验引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecipeCountLoaded(object sender, RoutedEventArgs e)
        {
            RecipeCount = sender as Slider;
        }

        /// <summary>
        /// 获取基础物品引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasedItemLoaded(object sender, RoutedEventArgs e)
        {
            BasedItem = sender as Image;
        }

        /// <summary>
        /// 获取材料物品引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdditionItemLoaded(object sender, RoutedEventArgs e)
        {
            AdditionItem = sender as Image;
        }

        /// <summary>
        /// 右击删除合成材料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClick(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image.Uid == "0")
                BasedItem.Source = empty_image;
            else
                AdditionItem.Source = empty_image;
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

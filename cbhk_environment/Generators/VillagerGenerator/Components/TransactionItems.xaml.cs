using cbhk_environment.GeneralTools;
using cbhk_environment.GeneralTools.Displayer;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cbhk_environment.Generators.VillagerGenerator.Components
{
    /// <summary>
    /// TransactionItems.xaml 的交互逻辑
    /// </summary>
    public partial class TransactionItems : UserControl
    {
        #region 物品数据
        /// <summary>
        /// 1 或 0 (true/false) - true代表交易会提供经验球。Java版中所有自然生成的村民的交易都会给予经验球。
        /// </summary>
        private bool rewardExp = true;
        public bool RewardExp
        {
            get { return rewardExp; }
            set
            {
                rewardExp = value;
            }
        }

        /// <summary>
        /// 代表在交易选项失效前能进行的最大交易次数。当交易被刷新时，以2到12的随机数增加。
        /// </summary>
        private string maxUses = "0";
        public string MaxUses
        {
            get { return maxUses; }
            set
            {
                maxUses = value;
            }
        }

        /// <summary>
        /// 已经交易的次数，如果大于或等于maxUses，该交易将失效。
        /// </summary>
        private string uses = "0";
        public string Uses
        {
            get { return uses; }
            set
            {
                uses = value;
            }
        }

        /// <summary>
        /// 村民从此交易选项中能获得的经验值。
        /// </summary>
        private string xp = "0";
        public string Xp
        {
            get { return xp; }
            set
            {
                xp = value;
            }
        }

        /// <summary>
        /// 根据需求确定的第一个收购物品的价格调节。当村民重新供应后更新此字段。
        /// </summary>
        private string demand = "0";
        public string Demand
        {
            get { return demand; }
            set
            {
                demand = value;
            }
        }

        /// <summary>
        /// 添加到第一个收购物品的价格调节。
        /// </summary>
        private string specialPrice = "0";
        public string SpecialPrice
        {
            get { return specialPrice; }
            set
            {
                specialPrice = value;
            }
        }

        /// <summary>
        /// 当前应用到此交易选项价格的乘数。
        /// </summary>
        private string priceMultiplier = "0";
        public string PriceMultiplier
        {
            get { return priceMultiplier; }
            set
            {
                priceMultiplier = value;
            }
        }
        #endregion

        #region 当前交易项数据
        public string TransactionItemData
        {
            get
            {
                if (Buy.Tag != null)
                {
                    string buyItemCount = ModifyBuyItemCount.Value.ToString();
                    string buyBItemCount = ModifyBuyBItemCount.Value.ToString();
                    string sellItemCount = ModifySellItemCount.Value.ToString();
                    if (buyItemCount.Contains('.'))
                        buyItemCount = buyItemCount[..buyItemCount.IndexOf('.')];
                    if (buyBItemCount.Contains('.'))
                        buyBItemCount = buyBItemCount[..buyBItemCount.IndexOf('.')];
                    if (sellItemCount.Contains('.'))
                        sellItemCount = sellItemCount[..sellItemCount.IndexOf('.')];

                    string result;
                    string rewardExp = "rewardExp:" + (RewardExp ? 1 : 0) + "b,";
                    string maxUses = MaxUses.Trim() != "" ? "maxUses:" + MaxUses + "," : "";
                    string uses = Uses.Trim() != "" ? "uses:" + Uses + "," : "";

                    #region 购入物品AB与卖出物品数据
                    ItemStructure buyItemData = Buy.Tag as ItemStructure;
                    ItemStructure buyBItemData = BuyB.Tag as ItemStructure;
                    ItemStructure sellItemData = Sell.Tag as ItemStructure;

                    //补齐双引号对
                    string buyData = buyItemData != null ? Regex.Replace(buyItemData.NBT, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":") : "{id:\"minecraft:air\",Count:1}";
                    string buyBData = buyBItemData != null ? Regex.Replace(buyBItemData.NBT, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":") : "{}";
                    string sellData = sellItemData != null ? Regex.Replace(sellItemData.NBT, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":") : "{id:\"minecraft:air\",Count:1}";
                    //清除数值型数据的单位
                    buyData = Regex.Replace(buyData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
                    buyBData = Regex.Replace(buyBData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
                    sellData = Regex.Replace(sellData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");

                    JObject buyObj = JObject.Parse(buyData);
                    JObject buybObj = JObject.Parse(buyBData);
                    JObject sellObj = JObject.Parse(sellData);

                    buyObj["Count"] = int.Parse(buyItemCount);
                    buybObj["Count"] = int.Parse(buyBItemCount);
                    sellObj["Count"] = int.Parse(sellItemCount);

                    //去除双引号对
                    string buy = "buy:" + Regex.Replace(buyObj.ToString(), @"([\{\[,])([\s+]?\w+[\s+]?):", "$1$2:").Replace("\r","").Replace("\n","") + ",";
                    buy = Regex.Replace(buy, @"\s+", "");
                    string buyB = "buyB:" + Regex.Replace(buybObj.ToString(), @"([\{\[,])([\s+]?\w+[\s+]?):", "$1$2:").Replace("\r", "").Replace("\n", "") + ",";
                    buyB = Regex.Replace(buyB, @"\s+", "");
                    string sell = "sell:" + Regex.Replace(sellObj.ToString(), @"([\{\[,])([\s+]?\w+[\s+]?):", "$1$2:").Replace("\r", "").Replace("\n", "") + ",";
                    sell = Regex.Replace(sell, @"\s+", "");
                    #endregion

                    string xp = Xp.Trim() != "" ? "xp:" + Xp + "," : "";
                    string demand = Demand.Trim() != "" ? "demand:" + Demand + "," : "";
                    string specialPrice = SpecialPrice.Trim() != "" ? "specialPrice:" + SpecialPrice + "," : "";
                    string priceMultiplier = PriceMultiplier.Trim() != "" ? "priceMultiplier:" + PriceMultiplier + "," : "";
                    result = rewardExp + maxUses + uses + buy + buyB + sell + xp + demand + specialPrice + priceMultiplier;
                    result = "{" + result.TrimEnd(',') + "}";
                    return result;
                }
                else
                    return "";
            }
        }
        #endregion

        public TransactionItems()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 更新物品显示图像以及文本提示
        /// </summary>
        /// <param name="old_image"></param>
        /// <param name="new_image"></param>
        private void UpdateItem(Image old_image,Image new_image)
        {
            int startIndex = new_image.Source.ToString().LastIndexOf('/') + 1;
            int endIndex = new_image.Source.ToString().LastIndexOf('.');
            string itemID = new_image.Source.ToString()[startIndex..endIndex];
            string toolTip = MainWindow.ItemIdSource.Where(item=>item.ComboBoxItemId == itemID).Select(item=>item.ComboBoxItemText).First();
            old_image.Source = new_image.Source;
            ToolTip tooltipObj = new()
            {
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#484848")),
                Content = toolTip
            };
            old_image.ToolTip = tooltipObj;
            ToolTipService.SetBetweenShowDelay(old_image, 0);
            ToolTipService.SetInitialShowDelay(old_image, 0);
        }

        /// <summary>
        /// 更新第一个收购物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBuyItem(object sender, DragEventArgs e)
        {
            Image image = e.Data.GetData(typeof(Image).ToString()) as Image;
            Image current_image = sender as Image;
            current_image.Tag = image.Tag;
            UpdateItem(current_image,image);
        }

        /// <summary>
        /// 更新第二个收购物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBuybItem(object sender, DragEventArgs e)
        {
            Image image = e.Data.GetData(typeof(Image).ToString()) as Image;
            Image current_image = sender as Image;
            current_image.Tag = image.Tag;
            UpdateItem(current_image, image);
        }

        /// <summary>
        /// 更新出售物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateSellItem(object sender, DragEventArgs e)
        {
            Image image = e.Data.GetData(typeof(Image).ToString()) as Image;
            Image current_image = sender as Image;
            current_image.Tag = image.Tag;
            UpdateItem(current_image, image);
        }

        /// <summary>
        /// 更新价格数据
        /// </summary>
        /// <param name="demand">根据需求确定的第一个收购物品的价格调节。当村民重新供应后更新此字段。</param>
        /// <param name="priceMultiplier">当前应用到此交易选项价格的乘数。</param>
        /// <param name="minor_negative">言论类型</param>
        /// <param name="trading">言论类型</param>
        /// <param name="specialPrice">添加到第一个收购物品的价格调节。</param>
        public void UpdateDiscountData(int minor_negative = 0,int trading = 0)
        {
            //获取原价
            int original_price = int.Parse(ModifyBuyItemCount.Value.ToString());
            int price = (int.Parse(Demand) * int.Parse(PriceMultiplier) * original_price) + (int.Parse(PriceMultiplier) * minor_negative) - (int.Parse(PriceMultiplier) * trading * 10) + int.Parse(SpecialPrice) + original_price;

            //如果最终价格不同于原价则开启打折效果
            if (price != original_price)
            {
                DisCount.Text = price.ToString();
                DisCount.Visibility = Visibility.Visible;
            }
            else
            {
                DisCount.Text = "";
            }
        }

        /// <summary>
        /// 恢复价格数据
        /// </summary>
        public void HideDiscountData(bool Hide = true)
        {
            if(Hide)
            {
                DisCount.Visibility = Visibility.Collapsed;
            }
            else
            {
                DisCount.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 删除该控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            CustomControls.IconTextButtons iconTextButtons = sender as CustomControls.IconTextButtons;
            TransactionItems template_parent = iconTextButtons.FindParent<TransactionItems>();
            villager_datacontext context = (Window.GetWindow(iconTextButtons) as Villager).DataContext as villager_datacontext;
            context.transactionItems.Remove(template_parent);
        }
    }
}

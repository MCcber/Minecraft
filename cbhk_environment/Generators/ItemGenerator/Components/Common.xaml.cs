using System.Linq;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// Common.xaml 的交互逻辑
    /// </summary>
    public partial class Common : UserControl
    {
        #region 保存物品信息隐藏选项
        private string ItemHideFlags
        {
            get
            {
                string selectedItem = HideFlagsBox.SelectedValue.ToString();
                string key = MainWindow.HideInfomationDataBase.Where(item => item.Value == selectedItem).First().Key;
                string result = key != "0" ? "HideFlags:" + key + "b," : "";
                return result;
            }
        }
        #endregion
        #region 保存名称与描述
        private string ItemDisplay
        {
            get
            {
                string DisplayNameString = ItemName.Text.Trim() != "" ? "Name:'{\"text\":\"" + ItemName.Text + "\"}'," : "";
                string ItemLoreString = ItemLore.Text.Trim() != "" ? "Lore:[\"[\\\"" + ItemLore.Text + "\\\"]\"]" : "";
                string result = DisplayNameString != "" || ItemLoreString != "" ? "display:{" + (DisplayNameString + ItemLoreString).TrimEnd(',') + "}," : "";
                return result;
            }
        }
        #endregion
        #region 无法破坏
        private string UnbreakableString
        {
            get
            {
                return Unbreakable.IsChecked.Value ? "Unbreakable:1b," : "";
            }
        }
        #endregion
        #region 交互锁定
        private string CustomCreativeLockResult
        {
            get
            {
                string result = CustomCreativeLock.IsChecked.Value ? "CustomCreativeLock:{}," :"";
                return result;
            }
        }
        #endregion
        #region 物品模型
        private string CustomModelDataResult
        {
            get
            {
                string result = CustomModelData.Value >= 0 ? "CustomModelData:" +CustomModelData.Value + ",":"";
                return result;
            }
        }
        #endregion
        #region 修理损耗
        private string RepairCostResult
        {
            get
            {
                string result = RepairCost.Value > 0 ? "RepairCost:" + RepairCost.Value + ",":"";
                return result;
            }
        }
        #endregion

        #region 合并结果
        public string Result
        {
            get
            {
                string result = ItemDisplay + ItemHideFlags + UnbreakableString + CustomCreativeLockResult + CustomModelDataResult + RepairCostResult;
                return result;
            }
        }
        #endregion

        public Common()
        {
            InitializeComponent();
            HideFlagsBox.ItemsSource = MainWindow.HideFlagsSource;
        }
    }
}

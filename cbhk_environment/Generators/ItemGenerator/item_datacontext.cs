using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using cbhk_environment.CustomControls;
using System.Windows.Controls;
using cbhk_environment.Generators.ItemGenerator.Components;
using cbhk_environment.ControlsDataContexts;
using System.Linq;
using System.Text.RegularExpressions;
using cbhk_environment.GenerateResultDisplayer;
using PotionTypeItems = cbhk_environment.Generators.ItemGenerator.Components.PotionTypeItems;
using System.Collections.ObjectModel;

namespace cbhk_environment.Generators.ItemGenerator
{
    public class item_datacontext:ObservableObject
    {
        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        #endregion

        #region 编辑可破坏方块、附魔、属性等交互指令
        public RelayCommand<FrameworkElement> AddCanDestroyBlock { get; set; }
        public RelayCommand<FrameworkElement> AddCanPlaceOnBlock { get; set; }
        public RelayCommand<FrameworkElement> AddEnchantment { get; set; }
        public RelayCommand<FrameworkElement> AddAttribute { get; set; }
        public RelayCommand<FrameworkElement> AddSpecial { get; set; }
        public RelayCommand<FrameworkElement> ClearCanDestroyBlocks { get; set; }
        public RelayCommand<FrameworkElement> ClearCanPlaceOnBlocks { get; set; }
        public RelayCommand<FrameworkElement> ClearEnchantments { get; set; }
        public RelayCommand<FrameworkElement> ClearAttributes { get; set; }
        public RelayCommand<FrameworkElement> ClearSpecials { get; set; }
        #endregion

        #region 附魔、属性、可破坏可放置等面板引用
        StackPanel EnchantmentPanel = null;
        StackPanel AttributePanel = null;
        StackPanel CanDestroyBlocksPanel = null;
        StackPanel CanPlaceOnBlocksPanel = null;
        StackPanel SpecialPanel = null;
        #endregion

        #region 版本
        private string selectedVersion = "";
        public string SelectedVersion
        {
            get
            {
                return selectedVersion;
            }
            set
            {
                selectedVersion = value;
            }
        }
        private ObservableCollection<string> VersionSource = new ObservableCollection<string> { "1.12-","1.13+" };
        #endregion

        public item_datacontext()
        {
            #region 连接指令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            RunCommand = new RelayCommand(run_command);

            AddCanDestroyBlock = new RelayCommand<FrameworkElement>(AddCanDestroyBlockClick);
            AddCanPlaceOnBlock = new RelayCommand<FrameworkElement>(AddCanPlaceOnBlockClick);
            AddEnchantment = new RelayCommand<FrameworkElement>(AddEnchantmentClick);
            AddAttribute = new RelayCommand<FrameworkElement>(AddAttributeClick);
            AddSpecial = new RelayCommand<FrameworkElement>(AddSpecialClick);

            ClearCanDestroyBlocks = new RelayCommand<FrameworkElement>(ClearCanDestroyBlockClick);
            ClearCanPlaceOnBlocks = new RelayCommand<FrameworkElement>(ClearCanPlaceOnBlockClick);
            ClearEnchantments = new RelayCommand<FrameworkElement>(ClearEnchantmentClick);
            ClearAttributes = new RelayCommand<FrameworkElement>(ClearAttributeClick);
            ClearSpecials = new RelayCommand<FrameworkElement>(ClearSpecialClick);
            #endregion

            #region 清除所有数据
            #endregion
        }

        /// <summary>
        /// 清空数据成员
        /// </summary>
        /// <param name="obj"></param>
        private void ClearItemClick(FrameworkElement obj)
        {
            Accordion Accordion = obj as Accordion;
            StackPanel stackPanel = Accordion.Content as StackPanel;
            stackPanel.Children.Clear();
        }

        /// <summary>
        /// 清空特殊数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearSpecialClick(FrameworkElement obj)
        {
            ClearItemClick(obj);
        }

        /// <summary>
        /// 清空属性数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearAttributeClick(FrameworkElement obj)
        {
            ClearItemClick(obj);
        }

        /// <summary>
        /// 清空附魔数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearEnchantmentClick(FrameworkElement obj)
        {
            ClearItemClick(obj);
        }

        /// <summary>
        /// 清空可放置方块数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearCanPlaceOnBlockClick(FrameworkElement obj)
        {
            ClearItemClick(obj);
        }

        /// <summary>
        /// 清空可破坏方块数据
        /// </summary>
        /// <param name="obj"></param>
        private void ClearCanDestroyBlockClick(FrameworkElement obj)
        {
            ClearItemClick(obj);
        }

        /// <summary>
        /// 增加特殊数据成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSpecialClick(FrameworkElement obj)
        {
            PotionTypeItems potionTypeItems = new PotionTypeItems();
            Accordion Accordion = obj as Accordion;
            StackPanel stackPanel = Accordion.Content as StackPanel;
            stackPanel.Children.Add(potionTypeItems);
        }

        /// <summary>
        /// 增加属性列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAttributeClick(FrameworkElement obj)
        {
            AttributeItems attributeItems = new AttributeItems();
            Accordion Accordion = obj as Accordion;
            StackPanel stackPanel = Accordion.Content as StackPanel;
            stackPanel.Children.Add(attributeItems);
        }

        /// <summary>
        /// 增加附魔列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEnchantmentClick(FrameworkElement obj)
        {
            Accordion Accordion = obj as Accordion;
            EnchantmentItems enchantmentItems = new EnchantmentItems();
            StackPanel stackPanel = Accordion.Content as StackPanel;
            stackPanel.Children.Add(enchantmentItems);
        }

        /// <summary>
        /// 增加可放置方块列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCanPlaceOnBlockClick(FrameworkElement obj)
        {
            Accordion Accordion = obj as Accordion;
            CanPlaceOnItems canPlaceOnItems = new CanPlaceOnItems();
            StackPanel stackPanel = Accordion.Content as StackPanel;
            stackPanel.Children.Add(canPlaceOnItems);
        }

        /// <summary>
        /// 增加可破坏方块列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCanDestroyBlockClick(FrameworkElement obj)
        {
            Accordion Accordion = obj as Accordion;
            CanDestroyItems canDestroyItems = new CanDestroyItems();
            StackPanel stackPanel = Accordion.Content as StackPanel;
            stackPanel.Children.Add(canDestroyItems);
        }

        #region 版本
        private bool behavior_lock = true;
        private bool version1_12 = false;
        public bool Version1_12
        {
            get { return version1_12; }
            set
            {
                version1_12 = value;
                if (behavior_lock)
                {
                    behavior_lock = false;
                    Version1_13 = !version1_12;
                    behavior_lock = true;
                }
                OnPropertyChanged();
            }
        }
        private bool version1_13 = true;
        public bool Version1_13
        {
            get { return version1_13; }
            set
            {
                version1_13 = value;
                if (behavior_lock)
                {
                    behavior_lock = false;
                    Version1_12 = !version1_13;
                    behavior_lock = true;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconItems.png";

        private void return_command(CommonWindow win)
        {
            win.Close();
            Item.cbhk.Topmost = true;
            Item.cbhk.WindowState = WindowState.Normal;
            Item.cbhk.Show();
            Item.cbhk.Topmost = false;
            Item.cbhk.ShowInTaskbar = true;
        }
        private void run_command()
        {
            string result = "";
            result += UnbreakableString+ ItemDamageString + ItemDisplay + ItemHideFlags + CustomTag + CanDestroyBlockList + CanPlaceOnBlockList + EnchantmentList + AttributeModifiers + MobEffectList;
            result = result.Trim() != "" ? "give @p minecraft:" + ItemId + "{" + result.TrimEnd(',') + "} " + ItemCount : (ItemId.Trim() != "" ? "give @p minecraft:" + ItemId + " "+ (ItemCount.Trim() == "1" || ItemCount.Trim() == "0" ? "" : ItemCount) : "");
            Displayer displayer = Displayer.GetContentDisplayer();
            displayer.GeneratorResult(result, "物品", icon_path);
            displayer.Show();
        }

        #region 运行与使用
        #region 无法破坏
        private bool unbreakable;
        public bool Unbreakable
        {
            get { return unbreakable; }
            set
            {
                unbreakable = value;
                OnPropertyChanged();
            }
        }
        private string UnbreakableString
        {
            get
            {
                return Unbreakable?"Unbreakable:1b,":"";
            }
        }
        #endregion

        #region 保存名称与描述
        private string ItemDisplay
        {
            get
            {
                string DisplayNameString = DisplayName.Trim() != "" ? "Name:'{\"text\":\"" + DisplayName + "\"}'" : "";
                string ItemLoreString = ItemLore.Trim() != "" ? ",Lore:[\"[\\\"" + ItemLore + "\\\"]\"]" : "";
                string result = DisplayNameString != "" || ItemLoreString != ""?"display:{"+DisplayNameString+ItemLoreString+"},":"";
                return result;
            }
        }
        #endregion

        #region 保存物品附加值/损耗值
        public string ItemDamageString
        {
            get
            {
                string result = ItemDamage.Trim()!="" && ItemDamage.Trim() != "0" ?"Damage:"+ItemDamage+",":"";
                return result;
            }
        }
        #endregion

        #region 保存物品信息隐藏选项
        private string hide_infomation_option;
        public string HideInfomationOption
        {
            get { return hide_infomation_option; }
            set
            {
                hide_infomation_option = value;
                OnPropertyChanged();
            }
        }
        private string ItemHideFlags
        {
            get
            {
                string key = MainWindow.HideInfomationDataBase.Where(item => item.Value == HideInfomationOption).First().Key;
                string result = key!="0"? "HideFlags:"+key+"b," : "";
                return result;
            }
        }
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
        private string ItemId
        {
            get
            {
                string key = MainWindow.ItemDataBase.Where(item => Regex.Match(item.Key, @"[\u4e00-\u9fa5]+").ToString() == SelectItemIdSource.ComboBoxItemText).First().Key;
                string result = key != "" ? Regex.Match(key,@"[a-zA-Z_]+").ToString() : "";
                return result;
            }
        }
        #endregion

        #region 保存物品数量
        private string item_count = "";
        public string ItemCount
        {
            get { return item_count; }
            set
            {
                item_count = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 保存物品自定义标签
        private string custom_tag = "";

        public string CustomTag
        {
            get 
            {
                string result = custom_tag.Trim() != "" ? custom_tag + "," : "";
                return result;
            }
            set
            {
                custom_tag = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 保存可破坏方块列表
        private string CanDestroyBlockList
        {
            get
            {
                if (CanDestroyBlocksPanel.Children.Count > 0)
                {
                    string result = "CanDestroy:[";
                    foreach (CanDestroyItems item in CanDestroyBlocksPanel.Children)
                    {
                        result += item.Result;
                    }
                    result = result.Trim(',')+"],";
                    return result;
                }
                else
                    return "";
            }
        }
        #endregion

        #region 保存可放置方块列表
        private string CanPlaceOnBlockList
        {
            get
            {
                if (CanPlaceOnBlocksPanel.Children.Count > 0)
                {
                    string result = "CanPlaceOn:[";
                    foreach (CanPlaceOnItems item in CanPlaceOnBlocksPanel.Children)
                    {
                        result += item.Result;
                    }
                    result = result.Trim(',') + "],";
                    return result;
                }
                else
                    return "";
            }
        }
        #endregion

        #region 保存附魔信息列表
        private string EnchantmentList
        {
            get
            {
                if (EnchantmentPanel.Children.Count > 0)
                {
                    string result = "Enchantments:[";
                    foreach (EnchantmentItems item in EnchantmentPanel.Children)
                    {
                        result += item.Result;
                    }
                    result = result.Trim(',') + "],";
                    return result;
                }
                else
                    return "";
            }
        }
        #endregion

        #region 保存属性列表
        private string AttributeModifiers
        {
            get
            {
                if (AttributePanel.Children.Count > 0)
                {
                    string result = "AttributeModifiers:[";
                    foreach (AttributeItems item in AttributePanel.Children)
                    {
                        result += item.Result;
                    }
                    result = result.Trim(',') + "],";
                    return result;
                }
                else
                    return "";
            }
        }
        #endregion

        #region 保存药水效果列表
        private string MobEffectList
        {
            get
            {
                if (SpecialPanel.Children.Count > 0)
                {
                    string result = "CustomPotionEffects:[";
                    foreach (PotionTypeItems item in SpecialPanel.Children)
                    {
                        result += item.Result;
                    }
                    result = result.Trim(',') + "],";
                    return result;
                }
                else
                    return "";
            }
        }
        #endregion

        #endregion

        #region 加载与显示

        #region 保存物品附加值/损耗值
        private string item_damage = "";
        public string ItemDamage
        {
            get { return item_damage; }
            set
            {
                item_damage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 保存物品名称
        private string display_name = "";
        public string DisplayName
        {
            get { return display_name; }
            set
            {
                display_name = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 保存物品描述
        private string item_lore = "";
        public string ItemLore
        {
            get { return item_lore; }
            set
            {
                item_lore = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        public void VersionLoaded(object sender,RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.ItemsSource = VersionSource;
        }

        /// <summary>
        /// 载入物品id列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ItemIdsLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            comboBoxs.ItemsSource = MainWindow.ItemIdSource;
        }

        /// <summary>
        /// 载入物品隐藏信息选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HideFlagsLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox textComboBox = sender as ComboBox;
            textComboBox.ItemsSource = MainWindow.HideFlagsSource;
        }

        /// <summary>
        /// 载入可破坏方块面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CanDestroyBlockPanelLoaded(object sender, RoutedEventArgs e)
        {
            CanDestroyBlocksPanel = (sender as Accordion).Content as StackPanel;
        }

        /// <summary>
        /// 载入可放置方块面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CanPlaceBlockPanelLoaded(object sender, RoutedEventArgs e)
        {
            CanPlaceOnBlocksPanel = (sender as Accordion).Content as StackPanel;
        }

        /// <summary>
        /// 载入属性面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AttributePanelLoaded(object sender, RoutedEventArgs e)
        {
            AttributePanel = (sender as Accordion).Content as StackPanel;
        }

        /// <summary>
        /// 载入附魔面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EnchantmentPanelLoaded(object sender, RoutedEventArgs e)
        {
            EnchantmentPanel = (sender as Accordion).Content as StackPanel;
        }

        /// <summary>
        /// 载入特殊面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SpecialPanelLoaded(object sender, RoutedEventArgs e)
        {
            SpecialPanel = (sender as Accordion).Content as StackPanel;
        }
    }
}

using cbhk_environment.ControlsDataContexts;
using cbhk_environment.Generators.EntityGenerator.Components;
using cbhk_environment.GenerateResultDisplayer;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using cbhk_environment.CustomControls;
using Windows.ApplicationModel.Chat;
using System.Collections.ObjectModel;

namespace cbhk_environment.Generators.EntityGenerator
{
    public class entity_datacontext: ObservableObject
    {
        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        #endregion

        #region 生成方式
        //切换逻辑锁
        bool behavior_lock = true;
        #region 召唤
        private bool summon = true;
        public bool Summon
        {
            get { return summon; }
            set
            {
                summon = value;
                if(behavior_lock)
                {
                    behavior_lock = false;
                    Give = !Summon;
                    behavior_lock = true;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region 怪物蛋
        private bool give;
        public bool Give
        {
            get { return give; }
            set
            {
                give = value;
                if(behavior_lock)
                {
                    behavior_lock = false;
                    Summon = !Give;
                    behavior_lock = true;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region 通用NBT

        #region 名称
        private string custom_name = "";
        public string CustomName
        {
            get{ return custom_name; }
            set
            {
                custom_name = value;
                OnPropertyChanged();
            }
        }
        public string CustomNameString
        {
            get
            {
                string result;
                result = CustomName.Trim() != "" ? "CustomName:'{\"text\":\"" + CustomName + "\"}'," : "";
                return result;
            }
            set
            {
                custom_name = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 名称可见性
        private bool custom_name_visible;
        public bool CustomNameVisible
        {
            get { return custom_name_visible; }
            set
            {
                custom_name_visible = value;
                OnPropertyChanged();
            }
        }
        private string CustomNameVisibleString
        {
            get
            {
                string result;
                result = CustomNameVisible ? "CustomNameVisible:1b," :"";
                return result;
            }
        }
        #endregion

        #region 血量
        private string health = "0";
        public string Health
        {
            get
            {
                string result;
                result = health.Trim() != "0" && health.Trim() != "" ?"Health:"+health+"f,":"";
                return result;
            }
            set
            {
                health = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Tags
        private string tags = "";
        public string Tags
        {
            get
            {
                string result = "";
                if(tags.Trim()!="")
                {
                    string[] tag_array = tags.Split(' ');
                    foreach (string a_tag in tag_array)
                    {
                        result += "\""+ a_tag + "\",";
                    }
                    result = result.Trim()!=""? "Tags:["+result.TrimEnd(',')+"],":"";
                }
                return result;
            }
            set
            {
                tags = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Motion

        private string Motion
        {
            get
            {
                string result;
                result = MotionX.Trim() != "0" || MotionY.Trim() != "0" || MotionZ.Trim() != "0" ?"Motion:["+MotionX+"d,"+MotionY+"d,"+MotionZ+"d],":"";
                return result;
            }
        }

        #region X
        private string motion_x = "0";
        public string MotionX
        {
            get { return motion_x; }
            set
            {
                motion_x = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Y
        private string motion_y = "0";
        public string MotionY
        {
            get { return motion_y; }
            set
            {
                motion_y = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Z
        private string motion_z = "0";
        public string MotionZ
        {
            get { return motion_z; }
            set
            {
                motion_z = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Team
        private string team = "";
        public string Team
        {
            get
            {
                string result;
                result = team.Trim() != "" ?"Team:\""+team+"\",":"";
                return result;
            }
            set
            {
                team = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Fire
        private string fire = "0";
        public string Fire
        {
            get
            {
                string result;
                result = fire.Trim() != "0" ?"Fire:"+fire.Trim()+"s,":"";
                return result;
            }
            set
            {
                fire = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 无敌
        private bool invulnerable;
        public bool Invulnerable
        {
            get { return invulnerable; }
            set
            {
                invulnerable = value;
                OnPropertyChanged();
            }
        }
        private string InvulnerableString
        {
            get
            {
                string result;
                result = Invulnerable ? "Invulnerable:1b," : "";
                return result;
            }
        }
        #endregion

        #region NoAI
        private bool noai;
        public bool NoAI
        {
            get { return noai; }
            set
            {
                noai = value;
                OnPropertyChanged();
            }
        }
        private string NoAIString
        {
            get
            {
                string result;
                result = NoAI ? "NoAI:1b," : "";
                return result;
            }
        }
        #endregion

        #region 发光
        private bool glowing;
        public bool Glowing
        {
            get { return glowing; }
            set
            {
                glowing = value;
                OnPropertyChanged();
            }
        }
        private string GlowingString
        {
            get
            {
                string result;
                result = Glowing ? "Glowing:1b," : "";
                return result;
            }
        }
        #endregion

        #region 无重力
        private bool nogravity;
        public bool NoGravity
        {
            get { return nogravity; }
            set
            {
                nogravity = value;
                OnPropertyChanged();
            }
        }
        private string NoGravityString
        {
            get
            {
                string result;
                result = NoGravity ? "NoGravity:1b," : "";
                return result;
            }
        }
        #endregion

        #region 可拾取物品
        private bool canPickUpLoot;
        public bool CanPickUpLoot
        {
            get { return canPickUpLoot; }
            set
            {
                canPickUpLoot = value;
                OnPropertyChanged();
            }
        }
        private string CanPickUpLootString
        {
            get
            {
                string result;
                result = CanPickUpLoot ? "CanPickUpLoot:1b," : "";
                return result;
            }
        }
        #endregion

        #region 无声
        private bool silent;
        public bool Silent
        {
            get { return silent; }
            set
            {
                silent = value;
                OnPropertyChanged();
            }
        }
        private string SilentString
        {
            get
            {
                string result;
                result = Silent ? "Silent:1b," : "";
                return result;
            }
        }
        #endregion

        #region 不被自然刷新
        private bool persistenceRequired;
        public bool PersistenceRequired
        {
            get { return persistenceRequired; }
            set
            {
                persistenceRequired = value;
                OnPropertyChanged();
            }
        }
        private string PersistenceRequiredString
        {
            get
            {
                string result;
                result = PersistenceRequired ? "PersistenceRequired:1b," : "";
                return result;
            }
        }
        #endregion

        #endregion

        #region 数据

        #region 实体ID
        private IconComboBoxItem entity_id = null;
        public IconComboBoxItem EntityId
        {
            get { return entity_id; }
            set
            {
                entity_id = value;
                OnPropertyChanged();
            }
        }
        private string EntityIdString
        {
            get
            {
                string result = "";
                result = MainWindow.EntityDataBase.Where(item=>item.Key.Contains(EntityId.ComboBoxItemText)).First().Key;
                result = Regex.Match(result, @"[a-zA-Z_]+").ToString();
                return result.Trim() != ""? result:"";
            }
        }
        #endregion

        #region 属性
        private string Attributes
        {
            get
            {
                string result = "Attributes:[";
                result += MaxHealth + KnockbackResistance + MovementSpeed + FollowRange + AttackDamage + AttackSpeed + Armor + ArmorToughness;
                result = result.Trim() != "Attributes:[" ? result.TrimEnd(',')+"],":"";
                return result;
            }
        }
        #endregion

        #region 最大生命值
        private string max_health = "0";
        public string MaxHealth
        {
            get
            {
                string result;
                result = max_health.Trim() != "0" && max_health.Trim() != "" ? "{Base:"+max_health+"d,Name:\"generic.max_health\"}," : "";
                return result;
            }
            set
            {
                max_health = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 抗击退
        private string knockback_resistance = "0";
        public string KnockbackResistance
        {
            get
            {
                string result;
                result = knockback_resistance.Trim() != "0" && knockback_resistance.Trim() != "" ? "{Base:" + knockback_resistance + "d,Name:\"generic.knockback_resistance\"}," : "";
                return result;
            }
            set
            {
                knockback_resistance = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 移动速度
        private string movement_speed = "0";
        public string MovementSpeed
        {
            get
            {
                string result;
                result = movement_speed.Trim() != "0" && movement_speed.Trim() != "" ? "{Base:" + movement_speed + "d,Name:\"generic.movement_speed\"}," : "";
                return result;
            }
            set
            {
                movement_speed = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 跟踪距离
        private string follow_range = "0";
        public string FollowRange
        {
            get
            {
                string result;
                result = follow_range.Trim() != "0" && follow_range.Trim() != "" ? "{Base:" + follow_range + "d,Name:\"generic.follow_range\"}," : "";
                return result;
            }
            set
            {
                follow_range = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 攻击伤害
        private string attack_damage = "0";
        public string AttackDamage
        {
            get
            {
                string result;
                result = attack_damage.Trim() != "0" && attack_damage.Trim() != "" ? "{Base:" + attack_damage + "d,Name:\"generic.attack_damage\"}," : "";
                return result;
            }
            set
            {
                attack_damage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 攻击速度
        private string attack_speed = "0";
        public string AttackSpeed
        {
            get
            {
                string result;
                result = attack_speed.Trim() != "0" && attack_speed.Trim() != "" ? "{Base:" + attack_speed + "d,Name:\"generic.attack_speed\"}," : "";
                return result;
            }
            set
            {
                attack_speed = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 护甲
        private string armor = "0";
        public string Armor
        {
            get
            {
                string result;
                result = armor.Trim() != "0" && armor.Trim() != "" ? "{Base:" + armor + "d,Name:\"generic.armor\"}," : "";
                return result;
            }
            set
            {
                armor = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 护甲韧性
        private string armor_toughness = "0";
        public string ArmorToughness
        {
            get
            {
                string result;
                result = armor_toughness.Trim() != "0" && armor_toughness.Trim() != "" ? "{Base:" + armor_toughness + "d,Name:\"generic.armor_toughness\"}," : "";
                return result;
            }
            set
            {
                armor_toughness = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 药水效果
        private string ActiveEffects
        {
            get
            {
                if (MobEffectStackPanel.Children.Count > 0)
                {
                    string result = "ActiveEffects:[";
                    foreach (PotionTypeItems item in MobEffectStackPanel.Children)
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

        #region 外观

        #region 手部
        private string HandItems
        {
            get
            {
                string result;
                result = MainHand.Trim() != "" || OffHand.Trim() != "" ?"HandItems:["+MainHand+","+OffHand+"],":"";
                return result;
            }
        }
        //掉率
        private string HandDropChances
        {
            get
            {
                string result;
                result = MainHandDropChance != 0 || OffHandDropChance != 0 ? "HandDropChances:["+MainHandDropChance+"f,"+OffHandDropChance+"f]," : "";
                return result;
            }
        }
        #endregion

        #region 主手
        private string main_hand = "";
        public string MainHand
        {
            get { return main_hand; }
            set
            {
                main_hand = value;
                OnPropertyChanged();
            }
        }
        //掉率
        private double mainhand_dropchance = 0f;
        public double MainHandDropChance
        {
            get { return mainhand_dropchance; }
            set
            {
                mainhand_dropchance = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 副手
        private string off_hand = "";
        public string OffHand
        {
            get { return off_hand; }
            set
            {
                off_hand = value;
                OnPropertyChanged();
            }
        }
        //掉率
        private double offhand_dropchance = 0;
        public double OffHandDropChance
        {
            get { return offhand_dropchance; }
            set
            {
                offhand_dropchance = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 装备
        private string ArmorItems
        {
            get
            {
                string result;
                result = HeadItem.Trim() != "" || ChestItem.Trim()!="" || LegItem.Trim()!=""||BootItem.Trim()!="" ? "ArmorItems:[" + HeadItem + "," + ChestItem + "," + LegItem + "," + BootItem + "]," : "";
                return result;
            }
        }
        //掉率
        private string ArmorDropChances
        {
            get
            {
                string result;
                result = HeadItemDropChance != 0 || ChestItemDropChance != 0 || LegItemDropChance != 0 || BootItemDropChance != 0 ? "ArmorDropChances:["+ HeadItemDropChance + "f,"+ChestItemDropChance+"f,"+LegItemDropChance+"f,"+BootItemDropChance+"f]," : "";
                return result;
            }
        }
        #endregion

        #region 头部
        private string head_item = "";
        public string HeadItem
        {
            get { return head_item; }
            set
            {
                head_item = value;
                OnPropertyChanged();
            }
        }
        //掉率
        private double headitem_dropchance = 0f;
        public double HeadItemDropChance
        {
            get { return headitem_dropchance; }
            set
            {
                headitem_dropchance = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 胸部
        private string chest_item = "";
        public string ChestItem
        {
            get { return chest_item; }
            set
            {
                chest_item = value;
                OnPropertyChanged();
            }
        }
        //掉率
        private double chest_item_dropchance = 0f;
        public double ChestItemDropChance
        {
            get { return chest_item_dropchance; }
            set
            {
                chest_item_dropchance = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 腿部
        private string leg_item = "";
        public string LegItem
        {
            get { return leg_item; }
            set
            {
                leg_item = value;
                OnPropertyChanged();
            }
        }
        //掉率
        private double leg_item_dropchance = 0f;
        public double LegItemDropChance
        {
            get { return leg_item_dropchance; }
            set
            {
                leg_item_dropchance = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 脚部
        private string boot_item = "";
        public string BootItem
        {
            get { return boot_item; }
            set
            {
                boot_item = value;
                OnPropertyChanged();
            }
        }
        //掉率
        private double boot_item_dropchance = 0f;
        public double BootItemDropChance
        {
            get { return boot_item_dropchance; }
            set
            {
                boot_item_dropchance = value;
                OnPropertyChanged();
            }
        }
        #endregion

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

        //实体药水效果栈面板
        StackPanel MobEffectStackPanel = null;
        //实体骑乘成员栈面板
        StackPanel MobPassengerStackPanel = null;

        /// <summary>
        /// 添加药水
        /// </summary>
        public RelayCommand<FrameworkElement> AddPotion { get; set; }
        /// <summary>
        /// 清空药水
        /// </summary>
        public RelayCommand<FrameworkElement> ClearPotions { get; set; }
        /// <summary>
        /// 添加乘客
        /// </summary>
        public RelayCommand<FrameworkElement> AddPassenger { get; set; }
        /// <summary>
        /// 清空乘客
        /// </summary>
        public RelayCommand<FrameworkElement> ClearPassenger { get; set; }

        public entity_datacontext()
        {
            #region 连接指令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            RunCommand = new RelayCommand(run_command);
            AddPotion = new RelayCommand<FrameworkElement>(AddPotionClick);
            ClearPotions = new RelayCommand<FrameworkElement>(ClearPotionClick);
            AddPassenger = new RelayCommand<FrameworkElement>(AddPassengerClick);
            ClearPassenger = new RelayCommand<FrameworkElement>(ClearPassengerClick);
            #endregion
        }

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconEntities.png";

        private void return_command(CommonWindow win)
        {
            Entity.cbhk.Topmost = true;
            Entity.cbhk.WindowState = WindowState.Normal;
            Entity.cbhk.Show();
            Entity.cbhk.Topmost = false;
            Entity.cbhk.ShowInTaskbar = true;
            win.Close();
        }
        private void run_command()
        {
            if (EntityIdString.Length > 0)
            {
                string result;
                result = CustomNameString + CustomNameVisibleString + Health + Tags + Motion + Team + Fire + InvulnerableString + NoAIString + GlowingString + NoGravityString + CanPickUpLootString + SilentString + PersistenceRequiredString + Attributes + HandItems + HandDropChances + ArmorItems + ArmorDropChances + ActiveEffects;
                result = result.Trim() != "" ? "summon minecraft:" + EntityIdString + " ~ ~1 ~ {" + result.TrimEnd(',') + "}" : "summon minecraft:" + EntityIdString + " ~ ~1 ~";
                Displayer displayer = Displayer.GetContentDisplayer();
                displayer.GeneratorResult(result, "实体", icon_path);
                displayer.Show();
            }
            else
                MessageBox.Show("缺少必要参数","生成失败",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        public void VersionLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.ItemsSource = VersionSource;
        }

        /// <summary>
        /// 实体药水效果栈载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MobEffectStackPanelLoaded(object sender, RoutedEventArgs e)
        {
            MobEffectStackPanel = (sender as Accordion).Content as StackPanel;
        }

        /// <summary>
        /// 实体骑乘成员栈载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MobPassengerStackPanelLoaded(object sender, RoutedEventArgs e)
        {
            MobPassengerStackPanel = (sender as Accordion).Content as StackPanel;
        }

        /// <summary>
        /// 添加药水效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPotionClick(FrameworkElement sender)
        {
            Accordion potionAccordion = sender as Accordion;
            PotionTypeItems potionTypeItems = new PotionTypeItems();
            StackPanel stackPanel = potionAccordion.Content as StackPanel;
            stackPanel.Children.Add(potionTypeItems);
        }

        /// <summary>
        /// 清空药水效果
        /// </summary>
        /// <param name="sender"></param>
        private void ClearPotionClick(FrameworkElement sender)
        {
            Accordion potionAccordion = sender as Accordion;
            StackPanel stackPanel = potionAccordion.Content as StackPanel;
            stackPanel.Children.Clear();
        }

        /// <summary>
        /// 添加乘客
        /// </summary>
        /// <param name="sender"></param>
        private void AddPassengerClick(FrameworkElement sender)
        {

        }

        /// <summary>
        /// 清空乘客
        /// </summary>
        /// <param name="sender"></param>
        private void ClearPassengerClick(FrameworkElement sender)
        {
        }

        /// <summary>
        /// 载入实体ID列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EntityIdsLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            comboBoxs.ItemsSource = MainWindow.EntityIdSource;
        }
    }
}

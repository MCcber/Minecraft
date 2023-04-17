using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.GenerateResultDisplayer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Devices.Lights;
using Windows.Media.Protection.PlayReady;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    public partial class entityPagesDataContext : ObservableObject
    {
        #region 运行指令
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
                if (behavior_lock)
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
                if (behavior_lock)
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

        #region 全选和反选bool型NBT
        public RelayCommand<FrameworkElement> SelectAllBoolNBTs { get; set; }
        public RelayCommand<FrameworkElement> ReverseAllBoolNBTs { get; set; }
        #endregion

        //清除不需要的特指数据
        public RelayCommand ClearUnnecessaryData { get; set; }

        #region 共通标签显示隐藏

        #region 实体共通标签可见性
        private Visibility entityCommonTagsVisibility = Visibility.Visible;
        public Visibility EntityCommonTagsVisibility
        {
            get
            {
                return entityCommonTagsVisibility;
            }
            set
            {
                entityCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 活体共通标签可见性
        private Visibility livingBodyCommonTagsVisibility = Visibility.Visible;
        public Visibility LivingBodyCommonTagsVisibility
        {
            get
            {
                return livingBodyCommonTagsVisibility;
            }
            set
            {
                livingBodyCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 生物共通标签可见性
        private Visibility mobCommonTagsVisibility = Visibility.Visible;
        public Visibility MobCommonTagsVisibility
        {
            get
            {
                return mobCommonTagsVisibility;
            }
            set
            {
                mobCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region 特殊标签显示隐藏

        #region 可愤怒生物额外字段可见性
        private Visibility angryCreatureExtraFieldVisibility = Visibility.Collapsed;
        public Visibility AngryCreatureExtraFieldVisibility
        {
            get
            {
                return angryCreatureExtraFieldVisibility;
            }
            set
            {
                angryCreatureExtraFieldVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 可繁殖生物额外字段可见性
        private Visibility breedableMobExtraFieldsVisibility = Visibility.Collapsed;
        public Visibility BreedableMobExtraFieldsVisibility
        {
            get
            {
                return breedableMobExtraFieldsVisibility;
            }
            set
            {
                breedableMobExtraFieldsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 可在袭击中生成的生物共通标签可见性
        private Visibility commonTagsForMobsSpawnedInRaidsVisibility = Visibility.Collapsed;
        public Visibility CommonTagsForMobsSpawnedInRaidsVisibility
        {
            get
            {
                return commonTagsForMobsSpawnedInRaidsVisibility;
            }
            set
            {
                commonTagsForMobsSpawnedInRaidsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 可骑乘生物共通标签可见性
        private Visibility commonTagsForRideableEntitiesVisibility = Visibility.Collapsed;
        public Visibility CommonTagsForRideableEntitiesVisibility
        {
            get
            {
                return commonTagsForRideableEntitiesVisibility;
            }
            set
            {
                commonTagsForRideableEntitiesVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 僵尸类生物共通标签可见性
        private Visibility commonTagsForZombiesVisibility = Visibility.Collapsed;
        public Visibility CommonTagsForZombiesVisibility
        {
            get
            {
                return commonTagsForZombiesVisibility;
            }
            set
            {
                commonTagsForZombiesVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 可驯服生物共通标签可见性
        private Visibility tameableMobExtraFieldsVisibility = Visibility.Collapsed;
        public Visibility TameableMobExtraFieldsVisibility
        {
            get
            {
                return tameableMobExtraFieldsVisibility;
            }
            set
            {
                tameableMobExtraFieldsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #endregion

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

        #region 数据

        #region 实体ID
        private IconComboBoxItem entity_id = null;
        public IconComboBoxItem SelectedEntityId
        {
            get { return entity_id; }
            set
            {
                entity_id = value;
                OnPropertyChanged();
                //更新特指和共通标签、额外字段
                if (SpecialViewer != null)
                    UpdateUILayOut();
            }
        }
        private string SelectedEntityIdString
        {
            get
            {
                string result = "";
                result = MainWindow.EntityDataBase.Where(item => item.Key.Contains(SelectedEntityId.ComboBoxItemText)).First().Key;
                result = GetEntityID().Match(result).ToString();
                return result.Trim() != "" ? result : "";
            }
        }
        #endregion

        #region 版本、实体数据源
        public ObservableCollection<IconComboBoxItem> EntityIds { get; set; } = MainWindow.EntityIdSource;
        public ObservableCollection<string> VersionSource { get; set; } = new() { "1.12-", "1.13+" };
        #endregion

        #region 属性
        private string Attributes
        {
            get
            {
                string result = "Attributes:[";
                result += MaxHealthString + KnockbackResistanceString + MovementSpeedString + FollowRangeString + AttackDamageString + AttackSpeedString + ArmorString + ArmorToughnessString;
                result = result.Trim() != "Attributes:[" ? result.TrimEnd(',') + "]," : "";
                return result;
            }
        }

        #region 最大生命值
        private double max_health = 0;
        public double MaxHealth
        {
            get
            {
                return max_health;
            }
            set
            {
                max_health = value;
                OnPropertyChanged();
                MaxHealthString = max_health != 0 ? "{Base:" + max_health + "d,Name:\"generic.max_health\"}," : "";
            }
        }
        private string MaxHealthString = "";
        #endregion

        #region 抗击退
        private double knockback_resistance = 0;
        public double KnockbackResistance
        {
            get
            {
                return knockback_resistance;
            }
            set
            {
                knockback_resistance = value;
                OnPropertyChanged();
                KnockbackResistanceString = knockback_resistance != 0 ? "{Base:" + knockback_resistance + "d,Name:\"generic.knockback_resistance\"}," : "";
            }
        }
        private string KnockbackResistanceString = "";
        #endregion

        #region 移动速度
        private double movement_speed = 0;
        public double MovementSpeed
        {
            get
            {
                return movement_speed;
            }
            set
            {
                movement_speed = value;
                OnPropertyChanged();
                MovementSpeedString = movement_speed != 0 ? "{Base:" + movement_speed + "d,Name:\"generic.movement_speed\"}," : "";
            }
        }
        private string MovementSpeedString = "";
        #endregion

        #region 跟踪距离
        private double follow_range = 0;
        public double FollowRange
        {
            get
            {
                return follow_range;
            }
            set
            {
                follow_range = value;
                OnPropertyChanged();
                FollowRangeString = follow_range != 0 ? "{Base:" + follow_range + "d,Name:\"generic.follow_range\"}," : "";
            }
        }
        private string FollowRangeString = "";
        #endregion

        #region 攻击伤害
        private double attack_damage = 0;
        public double AttackDamage
        {
            get
            {
                return attack_damage;
            }
            set
            {
                attack_damage = value;
                OnPropertyChanged();
                AttackDamageString = attack_damage != 0 ? "{Base:" + attack_damage + "d,Name:\"generic.attack_damage\"}," : "";
            }
        }
        private string AttackDamageString = "";
        #endregion

        #region 攻击速度
        private double attack_speed = 0;
        public double AttackSpeed
        {
            get
            {
                return attack_speed;
            }
            set
            {
                attack_speed = value;
                OnPropertyChanged();
                AttackSpeedString = attack_speed != 0 ? "{Base:" + attack_speed + "d,Name:\"generic.attack_speed\"}," : "";
            }
        }
        private string AttackSpeedString = "";
        #endregion

        #region 护甲
        private double armor = 0;
        public double Armor
        {
            get
            {
                return armor;
            }
            set
            {
                armor = value;
                OnPropertyChanged();
                ArmorString = armor != 0 ? "{Base:" + armor + "d,Name:\"generic.armor\"}," : "";
            }
        }
        private string ArmorString = "";
        #endregion

        #region 护甲韧性
        private double armor_toughness = 0;
        public double ArmorToughness
        {
            get
            {
                return armor_toughness;
            }
            set
            {
                armor_toughness = value;
                OnPropertyChanged();
                ArmorToughnessString = armor_toughness != 0 ? "{Base:" + armor_toughness + "d,Name:\"generic.armor_toughness\"}," : "";
            }
        }
        private string ArmorToughnessString = "";
        #endregion
        #endregion

        #region 药水效果
        private string ActiveEffects
        {
            get
            {
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
                result = MainHand.Trim() != "" || OffHand.Trim() != "" ? "HandItems:[" + MainHand + "," + OffHand + "]," : "";
                return result;
            }
        }
        //掉率
        private string HandDropChances
        {
            get
            {
                string result;
                result = MainHandDropChance != 0 || OffHandDropChance != 0 ? "HandDropChances:[" + MainHandDropChance + "f," + OffHandDropChance + "f]," : "";
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
                result = HeadItem.Trim() != "" || ChestItem.Trim() != "" || LegItem.Trim() != "" || BootItem.Trim() != "" ? "ArmorItems:[" + HeadItem + "," + ChestItem + "," + LegItem + "," + BootItem + "]," : "";
                return result;
            }
        }
        //掉率
        private string ArmorDropChances
        {
            get
            {
                string result;
                result = HeadItemDropChance != 0 || ChestItemDropChance != 0 || LegItemDropChance != 0 || BootItemDropChance != 0 ? "ArmorDropChances:[" + HeadItemDropChance + "f," + ChestItemDropChance + "f," + LegItemDropChance + "f," + BootItemDropChance + "f]," : "";
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
        #endregion

        #region 路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconEntities.png";
        string buttonNormalImage = "pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonNormal.png";
        string buttonPressedImage = "pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonPressed.png";
        ImageBrush buttonNormalBrush = null;
        ImageBrush buttonPressedBrush = null;
        string MobEffectsFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\MobAttributes.json";
        string NBTStructureFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Entity\\data";
        string SpecialNBTStructureFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Entity\\data\\SpecialTags.json";
        #endregion

        #region 字段与引用
        //特指结果集合
        public ObservableCollection<NBTDataStructure> SpecialTagsResult { get; set; } = new();
        //实体、活体、生物结果集合
        public ObservableCollection<NBTDataStructure> CommonResult { get; set; } = new();
        //白色画刷
        SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        //黑色画刷
        SolidColorBrush blackBrush = new((Color)ColorConverter.ConvertFromString("#000000"));
        //橙色画刷
        SolidColorBrush orangeBrush = new((Color)ColorConverter.ConvertFromString("#FFE5B663"));
        //获取实体英文id
        [GeneratedRegex("[a-zA-Z_]+")]
        private static partial Regex GetEntityID();
        //特殊实体的共通标签链表
        List<string> specialEntityCommonTagList = new();
        //特殊实体特指标签字典,用于动态切换内容
        Dictionary<string,Grid> specialDataDictionary = new();
        //特殊标签面板
        ScrollViewer SpecialViewer = null;
        //存储作为多开和外部工具时最终的结果
        private string Result { get; set; }
        #endregion

        public entityPagesDataContext()
        {
            #region 连接指令
            RunCommand = new RelayCommand(run_command);
            AddPotion = new RelayCommand<FrameworkElement>(AddPotionClick);
            ClearPotions = new RelayCommand<FrameworkElement>(ClearPotionClick);
            AddPassenger = new RelayCommand<FrameworkElement>(AddPassengerClick);
            ClearPassenger = new RelayCommand<FrameworkElement>(ClearPassengerClick);
            SelectAllBoolNBTs = new RelayCommand<FrameworkElement>(SelectAllBoolNBTsCommand);
            ReverseAllBoolNBTs = new RelayCommand<FrameworkElement>(ReverseAllBoolNBTsCommand);
            ClearUnnecessaryData = new RelayCommand(ClearUnnecessaryDataCommand);
            #endregion

            #region 初始化字段
            buttonNormalBrush = new ImageBrush(new BitmapImage(new Uri(buttonNormalImage, UriKind.RelativeOrAbsolute)));
            buttonPressedBrush = new ImageBrush(new BitmapImage(new Uri(buttonPressedImage, UriKind.RelativeOrAbsolute)));
            #endregion
        }

        /// <summary>
        /// 最终结算
        /// </summary>
        /// <param name="MultipleMode"></param>
        private void FinalSettlement(object MultipleOrExtern)
        {
            Result = string.Join(",", SpecialTagsResult.Select(item =>
            {
                if (item.HaveCurrentType == Visibility.Visible)
                    return item.Result;
                else
                    return "";
            })) + "," + string.Join(",", CommonResult.Select(item =>
            {
                if (item.HaveCurrentType == Visibility.Visible)
                    return item.Result;
                else
                    return "";
            }));
            if (Summon)
            Result = Result.Trim() != "" ? "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~ {" + Result.TrimEnd(',') + "}" : "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~";
            else
            {
                if (SelectedVersion == "1.12-")
                    Result = "give @p minecraft:spawner_egg 1 0 {EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\" " + (Result.Length > 0 ? "," + Result.TrimEnd(',') : "") + "}}";
                else
                    Result = "give @p minecraft:pig_spawner_egg{EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\"" + (Result.Length > 0 ? "," + Result.TrimEnd(',') : "") + "}} 1";
            }

            if(bool.Parse(MultipleOrExtern.ToString()))
            {
                Displayer displayer = Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "实体", icon_path);
                displayer.Topmost = true;
                displayer.Show();
                displayer.Topmost = false;
            }
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="MultipleMode"></param>
        public void GetResult(bool MultipleOrExtern = false)
        {
            Thread settlement = new(new ParameterizedThreadStart(FinalSettlement));
            settlement.Start(MultipleOrExtern);
        }

        /// <summary>
        /// 运行生成
        /// </summary>
        private void run_command()
        {
            string result;
            result = string.Join(",", SpecialTagsResult.Select(item =>
            {
                if (item.HaveCurrentType == Visibility.Visible && item.Result.Length > 0)
                    return item.Result;
                else
                    return "";
            })) + "," + string.Join(",", CommonResult.Select(item =>
            {
                if (item.HaveCurrentType == Visibility.Visible && item.Result.Length > 0)
                    return item.Result;
                else
                    return "";
            }));
            result = result.Trim(',');
            if (Summon)
                result = result.Trim() != "" ? "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~ {" + result + "}" : "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~";
            else
            {
                if (SelectedVersion == "1.12-")
                    result = "give @p minecraft:spawner_egg 1 0 {EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\" " + (result.Length > 0 ? "," + result : "") + "}}";
                else
                    result = "give @p minecraft:pig_spawner_egg{EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\"" + (result.Length > 0 ? "," + result : "") + "}} 1";
            }

            Displayer displayer = Displayer.GetContentDisplayer();
            displayer.GeneratorResult(result, "实体", icon_path);
            displayer.Topmost = true;
            displayer.Show();
            displayer.Topmost = false;
        }

        /// <summary>
        /// 清除不需要的数据
        /// </summary>
        private void ClearUnnecessaryDataCommand()
        {
            Grid currentGrid = specialDataDictionary[SelectedEntityIdString];
            specialDataDictionary.Clear();
            specialDataDictionary.Add(SelectedEntityIdString, currentGrid);
        }

        /// <summary>
        /// 根据需求构造控件
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        private List<FrameworkElement> ComponentsGenerator(ComponentData Request)
        {
            List<FrameworkElement> result = new();
            TextBlock displayText = new()
            {
                Text = Request.description,
                Foreground = whiteBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (Request.toolTip.Length > 0)
            {
                displayText.ToolTip = Request.toolTip;
                ToolTipService.SetInitialShowDelay(displayText, 0);
                ToolTipService.SetBetweenShowDelay(displayText, 0);
            }

            result.Add(displayText);
            ComponentEvents componentEvents = new();
            switch (Request.dataType)
            {
                case "TAG_BlockState":
                    {
                        Grid blockStateGrid = new();
                        blockStateGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                        blockStateGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                        blockStateGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        blockStateGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        TextBlock name = new()
                        {
                            Text = "名称",
                            ToolTip = "要使用的方块的命名空间ID。",
                            Foreground = whiteBrush
                        };
                        ToolTipService.SetBetweenShowDelay(name,0);
                        ToolTipService.SetInitialShowDelay(name, 0);
                        ComboBox blockBox = new()
                        {
                            Style = Application.Current.Resources["IconComboBoxStyle"] as Style,
                            ItemsSource = MainWindow.BlockIDSource,
                            Width = 610,
                            Height = 25,
                            SelectedIndex = 0
                        };
                        Accordion attributeAccordion = new()
                        {
                            Margin = new Thickness(10, 2, 10, 0),
                            Background = orangeBrush,
                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                            Title = "属性",
                            TitleForeground = blackBrush,
                            ModifyName = "添加",
                            FreshName = "清除",
                            ModifyForeground = blackBrush,
                            FreshForeground = blackBrush
                        };
                        blockStateGrid.Children.Add(name);
                        blockStateGrid.Children.Add(blockBox);
                        blockStateGrid.Children.Add(attributeAccordion);
                        Grid.SetRow(name, 0);
                        Grid.SetColumn(name, 0);
                        Grid.SetRow(blockBox, 0);
                        Grid.SetColumn(blockBox, 1);
                        Grid.SetRow(attributeAccordion, 1);
                        Grid.SetColumn(attributeAccordion, 0);
                        Grid.SetColumnSpan(attributeAccordion, 2);
                        result.Remove(displayText);
                        result.Add(blockStateGrid);
                    }
                    break;
                case "TAG_List":
                    {
                        if (Request.dependency.Length > 0)
                        {
                            switch (Request.dependency)
                            {
                                case "ItemGenerator":
                                    Accordion itemAccordion = new()
                                    {
                                        Style = Application.Current.Resources["AccordionStyle"] as Style,
                                        Background = orangeBrush,
                                        Title = Request.description,
                                        Margin = new Thickness(10, 2, 10, 0),
                                        TitleForeground = blackBrush,
                                        ModifyName = "添加",
                                        FreshName = "清空",
                                        ModifyForeground = blackBrush,
                                        FreshForeground = blackBrush,
                                        Tag = new NBTDataStructure() { Result = "",HaveCurrentType = Visibility.Collapsed,DataType = Request.dataType, NBTGroup = Request.nbtType }
                                    };
                                    StackPanel itemPanel = new();
                                    itemAccordion.Content = itemPanel;
                                    result.Add(itemAccordion);
                                    result.Remove(displayText);
                                    break;
                            }
                        }
                    }
                    break;
                case "TAG_Float_Array":
                    {
                        JArray children = JArray.Parse(Request.children);
                        Grid floatGrid = new() { Uid = Request.dataType, Name = Request.key, Tag = new NBTDataStructure() { Result = "",HaveCurrentType = Visibility.Collapsed,DataType = Request.dataType,NBTGroup = Request.nbtType } };
                        for (int i = 0; i < children.Count; i++)
                        {
                            floatGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            floatGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            JObject child = JObject.Parse(children[i].ToString());
                            JArray range = JArray.Parse(child["range"].ToString());
                            string description = child["description"].ToString();
                            string toolTip = child["toolTip"].ToString();
                            float minValue = float.MinValue;
                            float maxValue = float.MaxValue;
                            if (range != null)
                            {
                                minValue = float.Parse(range[0].ToString());
                                maxValue = float.Parse(range[1].ToString());
                            }
                            TextBlock descriptionText = new()
                            {
                                Text = description,
                                ToolTip = toolTip,
                                Foreground = whiteBrush,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            if(toolTip.Length > 0)
                            {
                                descriptionText.ToolTip = toolTip;
                                ToolTipService.SetInitialShowDelay(descriptionText, 0);
                                ToolTipService.SetBetweenShowDelay(descriptionText, 0);
                            }
                            Slider numberBox = new()
                            {
                                Minimum = minValue,
                                Maximum = maxValue,
                                Value = 0,
                                Style = Application.Current.Resources["NumberBoxStyle"] as Style,
                            };

                            if (floatGrid.Children.Count == 0)
                                numberBox.Uid = "0";
                            else
                                numberBox.Uid = "1";
                            floatGrid.Children.Add(descriptionText);
                            floatGrid.Children.Add(numberBox);
                            Grid.SetColumn(descriptionText,floatGrid.ColumnDefinitions.Count - 2);
                            Grid.SetColumn(numberBox,floatGrid.ColumnDefinitions.Count - 1);
                        }
                        floatGrid.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(floatGrid);
                    }
                    break;
                case "TAG_Byte":
                case "TAG_Int":
                case "TAG_Float":
                case "TAG_Double":
                case "TAG_Short":
                case "TAG_UUID":
                case "TAG_Pos":
                    {
                        double minValue = 0;
                        double maxValue = 0;
                        if (Request.dataType == "TAG_Byte")
                        {
                            minValue = byte.MinValue;
                            maxValue = byte.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Int" || Request.dataType == "TAG_UUID")
                        {
                            minValue = int.MinValue;
                            maxValue = int.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Float")
                        {
                            minValue = float.MinValue;
                            maxValue = float.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Double" || Request.dataType == "TAG_Pos")
                        {
                            minValue = double.MinValue;
                            maxValue = double.MaxValue;
                        }
                        else
                            if (Request.dataType == "TAG_Short")
                        {
                            minValue = short.MinValue;
                            maxValue = short.MaxValue;
                        }

                        Slider numberBox1 = new()
                        {
                            Name = Request.key,
                            Uid = Request.dataType,
                            Minimum = minValue,
                            Maximum = maxValue,
                            Value = 0,
                            Style = Application.Current.Resources["NumberBoxStyle"] as Style,
                            Tag = new NBTDataStructure() { Result = "", HaveCurrentType = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                        };
                        numberBox1.GotFocus += componentEvents.ValueChangedHandler;
                        if (Request.dataType == "TAG_Pos" || Request.dataType == "TAG_UUID")
                        {
                            Slider numberBox2 = new()
                            {
                                Name = Request.key,
                                Uid = Request.dataType,
                                Minimum = minValue,
                                Maximum = maxValue,
                                Value = 0,
                                Style = Application.Current.Resources["NumberBoxStyle"] as Style
                            };
                            Slider numberBox3 = new()
                            {
                                Name = Request.key,
                                Uid = Request.dataType,
                                Minimum = minValue,
                                Maximum = maxValue,
                                Value = 0,
                                Style = Application.Current.Resources["NumberBoxStyle"] as Style
                            };
                            Slider numberBox4 = new()
                            {
                                Name = Request.key,
                                Uid = Request.dataType,
                                Minimum = minValue,
                                Maximum = maxValue,
                                Value = 0,
                                Style = Application.Current.Resources["NumberBoxStyle"] as Style,
                            };
                            numberBox2.GotFocus += componentEvents.ValueChangedHandler;
                            numberBox3.GotFocus += componentEvents.ValueChangedHandler;
                            numberBox4.GotFocus += componentEvents.ValueChangedHandler;
                            TextToggleButtons enableButton = new()
                            {
                                Style = Application.Current.Resources["TextToggleButtonsStyle"] as Style,
                                Padding = new Thickness(5),
                                Content = "启用"
                            };
                            Grid numberGrid = new() { Tag = new NBTDataStructure() { Result = "",HaveCurrentType = Visibility.Collapsed,DataType = Request.dataType,NBTGroup = Request.nbtType } };
                            numberGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            numberGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            numberGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            numberGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            numberGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            numberGrid.Children.Add(numberBox1);
                            numberGrid.Children.Add(numberBox2);
                            numberGrid.Children.Add(numberBox3);
                            Grid.SetColumn(numberBox1, 0);
                            Grid.SetColumn(numberBox2, 1);
                            Grid.SetColumn(numberBox3, 2);
                            if (Request.dataType == "TAG_UUID")
                            {
                                IconTextButtons randomButton = new()
                                {
                                    Style = Application.Current.Resources["IconTextButton"] as Style,
                                    Background = buttonNormalBrush,
                                    Padding = new Thickness(5),
                                    PressedBackground = buttonPressedBrush,
                                    Content = "生成"
                                };
                                numberGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                                numberGrid.Children.Add(numberBox4);
                                numberGrid.Children.Add(randomButton);
                                numberGrid.Children.Add(enableButton);
                                Grid.SetColumn(randomButton, numberGrid.ColumnDefinitions.Count - 2);
                                Grid.SetColumn(enableButton, numberGrid.ColumnDefinitions.Count - 1);
                                Grid.SetColumn(numberBox4, 3);
                            }
                            result.Add(numberGrid);
                        }
                        else
                            result.Add(numberBox1);
                    }
                    break;
                case "TAG_String_List":
                case "TAG_String":
                case "TAG_Long":
                    {
                        TextBox stringBox = new() { Foreground = whiteBrush, Uid = Request.dataType, Name = Request.key,Tag = new NBTDataStructure() { Result = "",HaveCurrentType = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                        stringBox.GotFocus += componentEvents.ValueChangedHandler;
                        if(Request.dataType == "TAG_String_List")
                        {
                            string NewToolTip = Request.toolTip + "(以;分割成员)";
                            displayText.ToolTip = NewToolTip;
                        }
                        result.Add(stringBox);
                    }
                    break;
                case "TAG_Boolean":
                    {
                        TextCheckBoxs textCheckBoxs = new()
                        {
                            Uid = Request.dataType,
                            Name = Request.key,
                            Foreground = whiteBrush,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            HeaderWidth = 20,
                            HeaderHeight = 20,
                            Style = Application.Current.Resources["TextCheckBox"] as Style,
                            Content = Request.description,
                            Tag = new NBTDataStructure() { Result = "",HaveCurrentType = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                        };
                        if (Request.toolTip.Length > 0)
                        {
                            textCheckBoxs.ToolTip = Request.toolTip;
                            ToolTipService.SetBetweenShowDelay(textCheckBoxs,0);
                            ToolTipService.SetInitialShowDelay(textCheckBoxs, 0);
                        }
                        result.Remove(displayText);
                        textCheckBoxs.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(textCheckBoxs);
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 根据菜单UID分配可见性绑定器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AccordionVisibilitylLoaded(object sender, RoutedEventArgs e)
        {
            Accordion accordion = sender as Accordion;
            Binding visibilityBinder = new()
            {
                Path = new PropertyPath(accordion.Uid + "Visibility"),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(accordion, UIElement.VisibilityProperty, visibilityBinder);
        }

        /// <summary>
        /// 加入特指数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SpecialTagsPanelLoaded(object sender, RoutedEventArgs e)
        {
            SpecialViewer = sender as ScrollViewer;
            SelectedEntityId ??= EntityIds.First();
            UpdateUILayOut();
        }

        /// <summary>
        /// 根据当前切换的实体ID来动态切换UI元素的显隐
        /// </summary>
        private void UpdateUILayOut()
        {
            string data = File.ReadAllText(SpecialNBTStructureFilePath);
            JArray array = JArray.Parse(data);
            //清空特指数据
            SpecialTagsResult.Clear();

            for (int i = 0; i < array.Count; i++)
            {
                JToken currentToken = array[i];
                string type = currentToken["type"].ToString();
                if (type == SelectedEntityIdString)
                {
                    JArray commonTags = JArray.Parse(currentToken["common"].ToString());
                    List<string> commonTagList = commonTags.ToList().ConvertAll(item => item.ToString());
                    //计算本次与上次共通标签的差集,关闭指定菜单，而不是全部关闭再依次判断打开
                    List<string> closedCommonTagList = specialEntityCommonTagList.Except(commonTagList).ToList();
                    #region 处理特指NBT
                    if (!specialDataDictionary.ContainsKey(SelectedEntityIdString))
                    {
                        JArray children = JArray.Parse(currentToken["children"].ToString());
                        List<FrameworkElement> components = new();
                        Grid newGrid = new();
                        newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        #region 更新控件集合
                        foreach (JObject nbtStructure in children.Cast<JObject>())
                        {
                            List<FrameworkElement> result = JsonToComponentConverter(nbtStructure);
                            components.AddRange(result);
                        }
                        #endregion
                        #region 应用更新后的集合
                        bool LeftIndex = true;
                        for (int j = 0; j < components.Count; j++)
                        {
                            if(LeftIndex || newGrid.RowDefinitions.Count == 0)
                            newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                            newGrid.Children.Add(components[j]);
                            if (components[j] is not Accordion)
                            {
                                Grid.SetRow(components[j], newGrid.RowDefinitions.Count - 1);
                                Grid.SetColumn(components[j], LeftIndex?0:1);
                                LeftIndex = !LeftIndex;
                            }
                            else
                            {
                                Grid.SetRow(components[j], newGrid.RowDefinitions.Count - 1);
                                Grid.SetColumn(components[j], 0);
                                Grid.SetColumnSpan(components[j], 2);
                            }
                            if (components[j] is TextCheckBoxs)
                            {
                                newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                                LeftIndex = true;
                                Grid.SetColumnSpan(components[j], 2);
                            }
                        }
                        #endregion
                        specialDataDictionary.Add(SelectedEntityIdString, newGrid);
                        SpecialViewer.Content = newGrid;
                    }
                    else
                        SpecialViewer.Content = specialDataDictionary[SelectedEntityIdString];
                    #endregion
                    #region 处理额外字段与共通标签的显示隐藏
                    Type currentClassType = GetType();
                    PropertyInfo[] propertyInfos = currentClassType.GetProperties();
                    foreach (var item in closedCommonTagList)
                    {
                        object v = Convert.ChangeType(Visibility.Collapsed, currentClassType.GetProperty(item + "Visibility").PropertyType);
                        currentClassType.GetProperty(item + "Visibility")?.SetValue(this, v, null);
                    }
                    foreach (var item in commonTagList)
                    {
                        object v = Convert.ChangeType(Visibility.Visible, currentClassType.GetProperty(item + "Visibility").PropertyType);
                        currentClassType.GetProperty(item + "Visibility")?.SetValue(this, v, null);
                    }
                    //同步本次计算后的特殊实体共通标签链表
                    if (specialEntityCommonTagList.Count > 0)
                        specialEntityCommonTagList.Clear();
                    specialEntityCommonTagList.AddRange(commonTagList);
                    #endregion
                    break;
                }
            }
        }

        /// <summary>
        /// 展开数据类型,判断是否应该添加子级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ContentAccordionExpanded(object sender, RoutedEventArgs e)
        {
            Accordion accordion = sender as Accordion;
            string currentUID = accordion.Uid;
            ScrollViewer accordionContent = accordion.Content as ScrollViewer;
            Grid subGrid = accordion.Content as Grid;
            subGrid ??= accordionContent.Content as Grid;
            if (subGrid.Children.Count > 0) return;
            Accordion parentAccordion = accordion.FindParent<Accordion>();
            bool IsSubAccordion = false;
            //分辨当前是实体、生物还是活体,都不是则为其它共通标签
            IsSubAccordion = parentAccordion != null;

            #region 搜索当前实体ID对应的JSON对象
            string data = File.ReadAllText(SpecialNBTStructureFilePath);
            JArray array = JArray.Parse(data);
            JObject targetObj = array.Where(item =>
            {
                JObject currentObj = item as JObject;
                if (currentObj["type"].ToString() == SelectedEntityIdString)
                    return true;
                return false;
            }).First() as JObject;
            #endregion

            string type = targetObj["type"].ToString();
            string commonTagData = targetObj["common"].ToString();
            JArray commonTags = JArray.Parse(commonTagData);
            List<string> commonTagList = commonTags.ToList().ConvertAll(item => item.ToString());

            #region 处理共通标签
            List<FrameworkElement> components = new();
            foreach (string commonString in commonTagList)
            {
                if (IsSubAccordion && commonString != parentAccordion.Uid) continue;
                string commonFilePath = NBTStructureFolderPath + "\\" + commonString + ".json";
                string commonContent = File.ReadAllText(commonFilePath);
                JArray commonArray = JArray.Parse(commonContent);
                foreach (JObject commonItem in commonArray.Cast<JObject>())
                {
                    //判断数据类型,筛选对应的网格容器
                    string dataType = JArray.Parse(commonItem["tag"].ToString())[0].ToString();
                    string numberType = dataType.ToLower().Replace("tag_", "");
                    bool IsNumber = numberType == "pos" || numberType == "float_array" || numberType == "uuid" || numberType == "float" || numberType == "short" || numberType == "byte" || numberType == "int" || numberType == "long" || numberType == "double";
                    IsNumber = IsNumber && currentUID == "number";
                    if (dataType.ToLower().Contains(currentUID) || IsNumber)
                    {
                        List<FrameworkElement> result = JsonToComponentConverter(commonItem, commonString);
                        components.AddRange(result);
                    }
                }
            }
            #endregion
            #region 应用控件集合
            bool LeftIndex = true;
            foreach (FrameworkElement item in components)
            {
                if(LeftIndex || subGrid.RowDefinitions.Count == 0)
                subGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                subGrid.Children.Add(item);
                if (item is not Accordion)
                {
                    Grid.SetRow(item, subGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(item, LeftIndex ? 0 : 1);
                    LeftIndex = !LeftIndex;
                }
                else
                {
                    Grid.SetRow(item, subGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(item, 0);
                    Grid.SetColumnSpan(item, 2);
                }
                if (item is TextCheckBoxs)
                {
                    subGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    LeftIndex = true;
                    Grid.SetColumnSpan(item, 2);
                }
            }
            #endregion
        }

        /// <summary>
        /// JSON转控件
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="nbtStructure"></param>
        private List<FrameworkElement> JsonToComponentConverter(JObject nbtStructure,string NBTType = "")
        {
            string tag = JArray.Parse(nbtStructure["tag"].ToString())[0].ToString();
            string key = nbtStructure["key"].ToString();
            JToken children = nbtStructure["children"];
            string description = nbtStructure["description"].ToString();
            JToken toolTipObj = nbtStructure["toolTip"];
            string toolTip = toolTipObj != null ? toolTipObj.ToString() : "";
            JToken dependencyObj = nbtStructure["dependency"];
            string dependency = dependencyObj != null ? dependencyObj.ToString() : "";

            ComponentData componentData = new()
            {
                dataType = tag,
                key = key,
                toolTip = toolTip,
                description = description,
                dependency = dependency,
                nbtType = NBTType
            };
            if (children != null)
                componentData.children = children.ToString();
            List<FrameworkElement> componentGroup = ComponentsGenerator(componentData);
            return componentGroup;
        }

        /// <summary>
        /// 载入状态效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MobEffectsLoaded(object sender, RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            string mobEffectsData = File.ReadAllText(MobEffectsFilePath);
            JArray mobEffects = JArray.Parse(mobEffectsData);
            int rowIndex = 0;

            foreach (JObject effectData in mobEffects.Cast<JObject>())
            {
                string dataType = JArray.Parse(effectData["tag"].ToString())[0].ToString();
                string key = effectData["key"].ToString();
                string description = effectData["description"].ToString();
                JArray range = JArray.Parse(effectData["range"].ToString());
                double minValue = double.Parse(range[0].ToString());
                double maxValue = double.Parse(range[1].ToString());
                double value = minValue < 0 ? 0 : minValue;

                TextBlock displayText = new()
                {
                    Text = description,
                    Foreground = whiteBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Slider slider = new()
                {
                    Minimum = minValue,
                    Maximum = maxValue,
                    Value = value,
                    Style = Application.Current.Resources["NumberBoxStyle"] as Style
                };

                grid.Children.Add(displayText);
                grid.Children.Add(slider);
                Grid.SetRow(displayText, rowIndex);
                Grid.SetColumn(displayText, 0);
                Grid.SetRow(slider, rowIndex);
                Grid.SetColumn(slider, 1);

                RowDefinition row = new() { Height = new GridLength(1, GridUnitType.Star) };
                grid.RowDefinitions.Add(row);
                rowIndex++;
            }
        }

        /// <summary>
        /// 生成随机整数
        /// </summary>
        private void GeneratorNumberCommand()
        {
        }

        /// <summary>
        /// 切换UUID的开启和关闭
        /// </summary>
        /// <param name="obj"></param>
        private void SwitchUUIDCommand(FrameworkElement ele)
        {
            TextToggleButtons textToggleButtons = ele as TextToggleButtons;
            textToggleButtons.Content = textToggleButtons.IsChecked.Value ? "禁用" : "启用";
        }

        /// <summary>
        /// 反选所有bool型NBT
        /// </summary>
        /// <param name="ele"></param>
        private void ReverseAllBoolNBTsCommand(FrameworkElement ele)
        {
            Accordion accordion = ele as Accordion;
            ScrollViewer scrollViewer = accordion.Content as ScrollViewer;
            Grid grid = scrollViewer.Content as Grid;
            //grid.Children.Cast<TextCheckBoxs>().ToList().ForEach(item => item.IsChecked = !item.IsChecked.Value);
        }

        /// <summary>
        /// 全选所有bool型NBT
        /// </summary>
        private void SelectAllBoolNBTsCommand(FrameworkElement ele)
        {
            Accordion accordion = ele as Accordion;
            ScrollViewer scrollViewer = accordion.Content as ScrollViewer;
            Grid grid = scrollViewer.Content as Grid;
            //grid.Children.Cast<TextCheckBoxs>().ToList().ForEach(item => item.IsChecked = true);
        }

        /// <summary>
        /// 点击顶级菜单后父视图滚动到此
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Accordion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Accordion accordion = sender as Accordion;
            ScrollViewer scrollViewer = accordion.FindParent<ScrollViewer>();
            ScrollToSomeWhere.Scroll(accordion,scrollViewer);
        }

        /// <summary>
        /// 滚动到底时滚动其父级ScrollViewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if(scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight || scrollViewer.VerticalOffset == 0)
            {
                ScrollViewer parent = scrollViewer.FindParent<ScrollViewer>();
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };
                parent.RaiseEvent(eventArg);
            }
        }

        /// <summary>
        /// 添加药水效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPotionClick(FrameworkElement sender)
        {
            Accordion potionAccordion = sender as Accordion;
            PotionTypeItems potionTypeItems = new();
            ScrollViewer scrollViewer = potionAccordion.Content as ScrollViewer;
            StackPanel stackPanel = scrollViewer.Content as StackPanel;
            stackPanel.Children.Add(potionTypeItems);
        }

        /// <summary>
        /// 清空药水效果
        /// </summary>
        /// <param name="sender"></param>
        private void ClearPotionClick(FrameworkElement sender)
        {
            Accordion potionAccordion = sender as Accordion;
            ScrollViewer scrollViewer = potionAccordion.Content as ScrollViewer;
            StackPanel stackPanel = scrollViewer.Content as StackPanel;
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
    }

    public class ComponentData
    {
        public string children { get; set; }
        public string dataType { get; set; }
        public string nbtType { get; set; }
        public string key { get; set; }
        public string description { get; set; }
        public string toolTip { get; set; }
        public string dependency { get; set; }
    }
}

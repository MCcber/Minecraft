using cbhk_environment.Distributor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static cbhk_environment.FilePathComparator;
using cbhk_environment.SettingForm;
using Point = System.Windows.Point;
using Hardcodet.Wpf.TaskbarNotification;
using cbhk_environment.ControlsDataContexts;
using System.Collections.ObjectModel;
using MSScriptControl;
using Image = System.Windows.Controls.Image;
using System.Text.RegularExpressions;
using cbhk_environment.CustomControls;

namespace cbhk_environment
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow:Window
    {
        /// <summary>
        /// 主页可见性
        /// </summary>
        public static MainWindowProperties.Visibility cbhk_visibility = MainWindowProperties.Visibility.MinState;

        /// <summary>
        /// 生成器背景图路径
        /// </summary>
        private string spawner_image_path =  AppDomain.CurrentDomain.BaseDirectory+ "resources\\spawner_button_images";

        private string spawner_highlight_image_path = AppDomain.CurrentDomain.BaseDirectory + "resources\\spawner_button_images\\highlight";

        /// <summary>
        /// 生成器视图列数
        /// </summary>
        private int spawner_button_column = 3;

        /// <summary>
        /// 生成器视图行列数
        /// </summary>
        private int spawner_button_row= 5;

        /// <summary>
        /// 生成器背景图索引
        /// </summary>
        private int spawner_backround_index = 0;

        //骨架屏计时器
        System.Windows.Forms.Timer SkeletonTimer = new System.Windows.Forms.Timer()
        {
            Interval = 1000,
            Enabled = false
        };

        public TaskbarIcon taskbar_icon;

        #region js脚本执行者
        private static string language = "javascript";
        public static ScriptControlClass json_parser = new()
        {
            Language = language
        };
        public static object JsonScript(string JScript)
        {
            object Result = null;
            try
            {
                Result = json_parser.Eval(JScript);
            }
            catch (Exception ex)
            {
                return ex.Source + "\n" + ex.Message;
            }
            return Result;
        }
        #endregion

        #region 所有数据源对象
        //配方生成器图片源
        public static ObservableCollection<Image> RecipeImageList = new ObservableCollection<Image> { };
        //属性数据源
        public static ObservableCollection<string> AttributeSource = new ObservableCollection<string> { };
        //属性生效槽位数据源
        public static ObservableCollection<string> AttributeSlotSource = new ObservableCollection<string> { };
        //属性值类型数据源
        public static ObservableCollection<string> AttributeValueTypeSource = new ObservableCollection<string> { };
        //物品id数据源
        public static ObservableCollection<IconComboBoxItem> ItemIdSource = new ObservableCollection<IconComboBoxItem> { };
        //方块id数据源
        public static ObservableCollection<IconComboBoxItem> BlockIDSource = new ObservableCollection<IconComboBoxItem> { };
        //附魔id数据源
        public static ObservableCollection<string> EnchantmentIdSource = new ObservableCollection<string> { };
        //保存id与name
        public static Dictionary<string, BitmapImage> EntityDataBase = new Dictionary<string, BitmapImage> { };
        //物品id数据源
        public static ObservableCollection<IconComboBoxItem> EntityIdSource = new ObservableCollection<IconComboBoxItem> { };
        //保存药水id与name
        public static Dictionary<string, string> MobEffectDataBase = new Dictionary<string, string> { };
        //药水id数据源
        public static ObservableCollection<IconComboBoxItem> MobEffectIdSource = new ObservableCollection<IconComboBoxItem> { };
        //保存物品id与name
        public static Dictionary<string, BitmapImage> ItemDataBase = new Dictionary<string, BitmapImage> { };
        //保存方块id与name
        public static Dictionary<string, BitmapImage> BlockDataBase = new Dictionary<string, BitmapImage> { };
        //保存附魔id与name
        public static Dictionary<string, string> EnchantmentDataBase = new Dictionary<string, string> { };
        //保存属性id与name
        public static Dictionary<string, string> AttribuiteDataBase = new Dictionary<string, string> { };
        //保存属性的生效槽位
        public static Dictionary<string, string> AttributeSlotDataBase = new Dictionary<string, string> { };
        //保存属性值类型
        public static Dictionary<string, string> AttributeValueTypeDatabase = new Dictionary<string, string> { };
        //保存隐藏信息id与name
        public static Dictionary<string, string> HideInfomationDataBase = new Dictionary<string, string> { };
        //信息隐藏标记
        public static ObservableCollection<string> HideFlagsSource = new ObservableCollection<string> { };

        //标签生成器的过滤类型数据源
        public static ObservableCollection<string> TypeItemSource = new ObservableCollection<string> { };

        //标签生成器的复选框列表
        public static ObservableCollection<RichCheckBoxs> TagSpawnerItemCheckBoxList = new ObservableCollection<RichCheckBoxs> { };
        public static ObservableCollection<RichCheckBoxs> BlockCheckBoxList = new ObservableCollection<RichCheckBoxs> { };
        public static ObservableCollection<RichCheckBoxs> TagSpawnerBiomeCheckBoxList = new ObservableCollection<RichCheckBoxs> { };
        public static ObservableCollection<RichCheckBoxs> EntityCheckBoxList = new ObservableCollection<RichCheckBoxs> { };

        //粒子列表数据源
        public static ObservableCollection<string> ParticleDataBase = new ObservableCollection<string> { };

        //音效列表数据源
        public static ObservableCollection<string> SoundDataBase = new ObservableCollection<string> { };

        //音效列表id名称字典
        public static Dictionary<string,string> SoundIdNameSource = new Dictionary<string,string> { };

        //记分板判据类型数据源
        public static ObservableCollection<string> ScoreboardTypeDataBase = new ObservableCollection<string> { };

        //队伍颜色列表
        public static ObservableCollection<string> TeamColorDataBase = new ObservableCollection<string> { };
        #endregion

        //用户数据
        Dictionary<string, string> UserData = new Dictionary<string, string> { };

        public MainWindow(Dictionary<string,string> user_info)
        {
            InitializeComponent();
            UserData = user_info;
            ReadDataSource();
            InitUIData();
            SkeletonTimer.Tick += SkeletonScreenShowDuration;
        }

        /// <summary>
        /// 骨架屏持续时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkeletonScreenShowDuration(object sender, EventArgs e)
        {
            SkeletonGrid.Visibility = Visibility.Collapsed;
            GeneratorTable.Visibility = Visibility.Visible;
            SkeletonTimer.Enabled = false;
        }

        /// <summary>
        /// 读取所有数据源,提供给所有生成器使用
        /// </summary>
        private void ReadDataSource()
        {
            #region 获取所有物品的id和对应的中文
            SolidColorBrush white_brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\items.json") &&
               File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && ItemDataBase.Count == 0)
            {
                string items_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\items.json");
                string js_file = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js");

                JsonScript(js_file);
                string item_id;
                string item_name;

                JsonScript("parseJSON(" + items_json + ");");
                int item_count = int.Parse(JsonScript("data.length").ToString());
                for (int i = 0; i < item_count; i++)
                {
                    item_id = JsonScript("getJSON('[" + i + "].id');").ToString();
                    item_name = JsonScript("getJSON('[" + i + "].name');").ToString();
                    BitmapImage image = null;
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + item_id + ".png"))
                    {
                        image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + item_id + ".png", UriKind.Relative));
                        ItemIdSource.Add(new IconComboBoxItem() { ComboBoxItemText = item_name, ComboBoxItemIcon = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + item_id + ".png", UriKind.Absolute)) });
                        RenderOptions.SetBitmapScalingMode(image,BitmapScalingMode.NearestNeighbor);
                        RenderOptions.SetClearTypeHint(image,ClearTypeHint.Enabled);

                        #region 给配方生成器提供数据
                        Image recipeImage = new Image()
                        {
                            Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + item_id + ".png", UriKind.Absolute)),
                            ToolTip = item_id + " " + item_name,
                            Width = 100,
                            Height = 100,
                            Tag = Regex.Match(item_id, @"[a-zA-Z_]+").ToString()
                        };
                        RenderOptions.SetBitmapScalingMode(recipeImage, BitmapScalingMode.HighQuality);
                        RenderOptions.SetClearTypeHint(recipeImage, ClearTypeHint.Enabled);
                        ToolTipService.SetInitialShowDelay(recipeImage, 0);
                        ToolTipService.SetShowDuration(recipeImage, 1000);
                        RecipeImageList.Add(recipeImage);
                        #endregion

                    }
                    if (!ItemDataBase.ContainsKey(item_id + ":" + item_name))
                        ItemDataBase.Add(item_id + ":" + item_name, image);

                    TagSpawnerItemCheckBoxList.Add(new RichCheckBoxs()
                    {
                        Uid = "Item",
                        Height = 50,
                        Margin = new Thickness(10, 0, 0, 0),
                        FontSize = 15,
                        Foreground = white_brush,
                        Tag = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + item_id + ".png",
                        HeaderHeight = 20,
                        HeaderWidth = 20,
                        ContentImage = null,
                        ImageWidth = 50,
                        ImageHeight = 50,
                        HeaderText = item_id + " " + item_name,
                        TextMargin = new Thickness(40, 0, 0, 0)
                    });
                }
            }
            #endregion

            #region 获取所有方块的id和对应中文
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\blocks.json") &&
                File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && BlockDataBase.Count == 0)
            {
                string blocks_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\blocks.json");
                string block_id;
                string block_name;

                JsonScript("parseJSON(" + blocks_json + ");");
                int block_count = int.Parse(JsonScript("getLength();").ToString());
                for (int i = 0; i < block_count; i++)
                {
                    block_id = JsonScript("getJSON('[" + i + "].id');").ToString();
                    block_name = JsonScript("getJSON('[" + i + "].name');").ToString();
                    BitmapImage image = null;
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\block_and_block_images\\" + block_id + ".png"))
                    {
                        image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + block_id + ".png", UriKind.Relative));
                        RenderOptions.SetBitmapScalingMode(image,BitmapScalingMode.HighQuality);
                        RenderOptions.SetClearTypeHint(image,ClearTypeHint.Enabled);
                        BlockIDSource.Add(new IconComboBoxItem() { ComboBoxItemText = block_name, ComboBoxItemIcon = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\block_and_block_images\\" + block_id + ".png", UriKind.Absolute)) });
                        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                        RenderOptions.SetClearTypeHint(image, ClearTypeHint.Enabled);
                    }
                    if (!BlockDataBase.ContainsKey(block_id + ":" + block_name))
                        BlockDataBase.Add(block_id + ":" + block_name, image);

                    BlockCheckBoxList.Add(new RichCheckBoxs()
                    {
                        Uid = "Block",
                        Height = 50,
                        Margin = new Thickness(10, 0, 0, 0),
                        FontSize = 15,
                        Foreground = white_brush,
                        Tag = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\block_and_block_images\\" + block_id + ".png",
                        HeaderHeight = 20,
                        HeaderWidth = 20,
                        ContentImage = null,
                        ImageWidth = 50,
                        ImageHeight = 50,
                        HeaderText = block_id + " " + block_name,
                        TextMargin = new Thickness(40, 0, 0, 0)
                    });
                }
            }
            #endregion

            #region 获取所有附魔id和描述
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\enchantments.json") &&
                File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && EnchantmentDataBase.Count == 0)
            {
                string enchantments_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\enchantments.json");

                ObservableCollection<string> itemDataGroups = new ObservableCollection<string>();
                string enchantment_id = "";
                string enchantment_name = "";
                string enchantment_num = "";

                JsonScript("parseJSON(" + enchantments_json + ");");

                int enchantment_count = int.Parse(JsonScript("getLength();").ToString());
                for (int i = 0; i < enchantment_count; i++)
                {
                    enchantment_id = JsonScript("getJSON('[" + i + "].id');").ToString();
                    enchantment_name = JsonScript("getJSON('[" + i + "].name');").ToString();
                    enchantment_num = JsonScript("getJSON('[" + i + "].num');").ToString();
                    EnchantmentDataBase.Add(enchantment_id, enchantment_name + enchantment_num);
                    itemDataGroups.Add(enchantment_name);
                }
                EnchantmentIdSource = itemDataGroups;
            }
            #endregion

            #region 获取所有药水效果和描述
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\mob_effects.json") &&
                File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && MobEffectDataBase.Count == 0)
            {
                string potion_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\mob_effects.json");

                string potion_id = "";
                string potion_name = "";
                string potion_num = "";

                JsonScript("parseJSON(" + potion_json + ");");

                int effect_count = int.Parse(JsonScript("getLength();").ToString());
                for (int i = 0; i < effect_count; i++)
                {
                    potion_id = JsonScript("getJSON('[" + i + "].id');").ToString();
                    potion_name = JsonScript("getJSON('[" + i + "].name');").ToString();
                    potion_num = JsonScript("getJSON('[" + i + "].num');").ToString();

                    MobEffectDataBase.Add(potion_id, potion_name + potion_num);
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\mob_effects_images\\" + potion_id + ".png"))
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\mob_effects_images\\" + potion_id + ".png", UriKind.Absolute));
                        RenderOptions.SetBitmapScalingMode(bitmapImage,BitmapScalingMode.NearestNeighbor);
                        RenderOptions.SetClearTypeHint(bitmapImage,ClearTypeHint.Enabled);
                        MobEffectIdSource.Add(new IconComboBoxItem() { ComboBoxItemText = potion_name, ComboBoxItemIcon = bitmapImage });
                    }
                }
            }
            #endregion

            #region 获取属性列表
            if (AttributeSource.Count == 0)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\AttributeIds.ini"))
                {
                    string[] attribute = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\AttributeIds.ini");
                    for (int i = 0; i < attribute.Length; i++)
                    {
                        string[] attribute_info = attribute[i].Split(':');
                        string attribue_id = attribute_info[0];
                        string attribute_name = attribute_info[1];
                        if (!AttribuiteDataBase.ContainsKey(attribue_id))
                            AttribuiteDataBase.Add(attribue_id, attribute_name);
                        AttributeSource.Add(attribute_name);
                    }
                }
            }
            #endregion

            #region 获取属性生效槽位列表
            if (AttributeSlotSource.Count == 0)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\Slots.ini"))
                {
                    string[] attribute = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\Slots.ini");
                    for (int i = 0; i < attribute.Length; i++)
                    {
                        string[] attributes = attribute[i].Split(':');
                        AttributeSlotDataBase.Add(attributes[0], attributes[1]);
                        AttributeSlotSource.Add(attributes[1]);
                    }
                }
            }
            #endregion

            #region 获取属性值类型列表
            if (AttributeValueTypeSource.Count == 0)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\ValueTypes.ini"))
                {
                    string[] attribute = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\ValueTypes.ini");
                    for (int i = 0; i < attribute.Length; i++)
                    {
                        string[] attributes = attribute[i].Split(':');
                        AttributeValueTypeDatabase.Add(attributes[0], attributes[1]);
                        AttributeValueTypeSource.Add(attributes[1]);
                    }
                }
            }
            #endregion

            #region 获取信息隐藏标记
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\HideInfomationOptions.ini"))
            {
                string[] hide_flag = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Item\\data\\HideInfomationOptions.ini");
                foreach (string item in hide_flag)
                {
                    string[] hide_info = item.Split(':');
                    if (!HideInfomationDataBase.ContainsKey(hide_info[0]))
                    HideInfomationDataBase.Add(hide_info[0], hide_info[1]);
                    HideFlagsSource.Add(hide_info[1]);
                }
            }
            #endregion

            #region 获取所有实体的id和对应的中文
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entities.json") &&
               File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && EntityDataBase.Count == 0)
            {
                string entities_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entities.json");
                string entity_id = "";
                string entity_name = "";
                BitmapImage image = null;

                JsonScript("parseJSON(" + entities_json + ");");
                int item_count = int.Parse(JsonScript("getLength();").ToString());
                for (int i = 0; i < item_count; i++)
                {
                    entity_id = JsonScript("getJSON('[" + i + "].id');").ToString();
                    entity_name = JsonScript("getJSON('[" + i + "].name');").ToString();
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + entity_id + "_spawn_egg.png"))
                    {
                        image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + entity_id + "_spawn_egg.png", UriKind.Absolute));

                        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                        RenderOptions.SetClearTypeHint(image, ClearTypeHint.Enabled);

                        EntityIdSource.Add(new IconComboBoxItem() { ComboBoxItemText = entity_name, ComboBoxItemIcon = image });
                    }
                    else
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + entity_id + ".png"))
                    {
                        image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + entity_id + ".png",UriKind.Absolute));

                        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                        RenderOptions.SetClearTypeHint(image, ClearTypeHint.Enabled);

                        EntityIdSource.Add(new IconComboBoxItem() { ComboBoxItemText = entity_name, ComboBoxItemIcon = image });
                    }
                    if (!EntityDataBase.ContainsKey(entity_id + ":" + entity_name))
                        EntityDataBase.Add(entity_id + ":" + entity_name, image);

                    EntityCheckBoxList.Add(new RichCheckBoxs()
                    {
                        Uid = "Entity",
                        Height = 50,
                        Margin = new Thickness(10, 0, 0, 0),
                        FontSize = 15,
                        Foreground = white_brush,
                        Tag = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\" + entity_id + "_spawn_egg.png",
                        HeaderHeight = 20,
                        HeaderWidth = 20,
                        ContentImage = null,
                        ImageWidth = 50,
                        ImageHeight = 50,
                        HeaderText = entity_id + " " + entity_name,
                        TextMargin = new Thickness(40, 0, 0, 0)
                    });
                }
            }
            #endregion

            #region 获取所有粒子id
            if(File.Exists(AppDomain.CurrentDomain.BaseDirectory+ "resources\\data_sources\\particles.json") && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && ParticleDataBase.Count == 0)
            {
                string particle_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\particles.json");
                string particle_id = "";
                JsonScript("parseJSON(" + particle_json + ");");
                int item_count = int.Parse(JsonScript("getLength();").ToString());
                for (int i = 0; i < item_count; i++)
                {
                    particle_id = JsonScript("getJSON('[" + i + "]');").ToString();
                    ParticleDataBase.Add(particle_id);
                }
            }
            #endregion

            #region 获取所有音效id
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\sounds.json") && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && SoundDataBase.Count == 0)
            {
                string sound_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\sounds.json");
                string sound_id = "";
                string sound_name = "";
                JsonScript("parseJSON(" + sound_json + ");");
                int item_count = int.Parse(JsonScript("getLength();").ToString());
                for (int i = 0; i < item_count; i++)
                {
                    sound_id = JsonScript("getJSON('[" + i + "].id');").ToString();
                    sound_name = JsonScript("getJSON('[" + i + "].name');").ToString();

                    SoundIdNameSource.Add(sound_id, sound_name);
                    SoundDataBase.Add(sound_id);
                }
            }
            #endregion

            #region 获取所有队伍颜色
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\teamColor.ini"))
            {
                string[] team_colors = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\teamColor.ini");
                for (int i = 0; i < team_colors.Length; i++)
                {
                    TeamColorDataBase.Add(team_colors[i]);
                }
            }
            #endregion

            #region 获取所有记分板判据类型
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\scoreboardType.json") && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\json_reader.js") && ScoreboardTypeDataBase.Count == 0)
            {
                string scoreboardType_json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\scoreboardType.json");
                string scoreboard_type;
                JsonScript("parseJSON(" + scoreboardType_json + ");");
                int item_count = int.Parse(JsonScript("getLength();").ToString());
                //按正则提取记分板的类型分支成员
                Regex GetTypeItems = new Regex(@"(?<=\{)[^}]*(?=\})");
                for (int i = 0; i < item_count; i++)
                {
                    scoreboard_type = JsonScript("getJSON('[" + i + "]');").ToString();
                    if(scoreboard_type.Contains("{"))
                    {
                        string item = GetTypeItems.Match(scoreboard_type).ToString();
                        string type_head = GetTypeItems.Replace(scoreboard_type, "").Replace("{", "").Replace("}", "");
                        switch (item)
                        {
                            case "teamColor":
                                foreach (var color in TeamColorDataBase)
                                {
                                    ScoreboardTypeDataBase.Add(type_head + color);
                                }
                                break;
                            case "itemName":
                                foreach (var an_item in ItemDataBase)
                                {
                                    string item_key = an_item.Key.Split('.')[0];
                                    ScoreboardTypeDataBase.Add(type_head + item_key);
                                }
                                break;
                            case "entityName":
                                foreach (var an_entity in EntityDataBase)
                                {
                                    string item_key = an_entity.Key.Split('.')[0];
                                    ScoreboardTypeDataBase.Add(type_head + item_key);
                                }
                                break;
                        }
                    }
                    else
                    ScoreboardTypeDataBase.Add(scoreboard_type);
                }
            }
            #endregion

            #region 加载过滤类型
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Tag\\TypeFilter.ini"))
            {
                string[] Types = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Tag\\TypeFilter.ini");
                for (int i = 0; i < Types.Length; i++)
                {
                    TypeItemSource.Add(Types[i]);
                }
            }
            #endregion
        }

        /// <summary>
        /// 初始化界面数据
        /// </summary>
        private void InitUIData()
        {
            #region 初始化托盘
            taskbar_icon = FindResource("cbhk_taskbar") as TaskbarIcon;
            //显示
            taskbar_icon.Visibility = Visibility.Visible;
            taskbar_icon.DataContext = new resources.MainFormDataContext.NotifyIconViewModel(this);
            #endregion

            #region 加载启动器配置
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\cbhk.ini"))
            {
                string[] configs = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\cbhk.ini");
                for (int i = 0; i < configs.Length; i++)
                {
                    string[] data = configs[i].Split(':');
                    switch (data[0])
                    {
                        case "CBHKVisibility":
                            {
                                switch (data[1])
                                {
                                    case "KeepState":
                                        {
                                            cbhk_visibility = MainWindowProperties.Visibility.KeepState;
                                            break;
                                        }
                                    case "MinState":
                                        {
                                            cbhk_visibility = MainWindowProperties.Visibility.MinState;
                                            break;
                                        }
                                    case "Close":
                                        {
                                            cbhk_visibility = MainWindowProperties.Visibility.Close;
                                            break;
                                        }
                                }
                                break;
                            }
                        case "CloseToTray":
                            {
                                MainWindowProperties.CloseToTray = bool.Parse(data[1]);
                                break;
                            }
                        //case "AutoStart":
                        //    {
                        //        MainWindowProperties.AutoStart = bool.Parse(data[1]);
                        //        if (MainWindowProperties.AutoStart)
                        //            GeneralTools.AutoStart.CreateShortcut("C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup", "命令管家", AppDomain.CurrentDomain.BaseDirectory + "cbhk.exe", "命令管家1.19", AppDomain.CurrentDomain.BaseDirectory + "cb.ico");
                        //        else
                        //            File.Delete("C:\\Users\\"+Environment.UserName+"\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\命令管家");
                        //        break;
                        //    }
                        case "LinkAnimationDelay":
                            {
                                MainWindowProperties.LinkAnimationDelay = int.Parse(data[1]);
                                break;
                            }
                    }
                }
            }
            #endregion

            #region 加载用户数据
            //没有头像就加载默认头像
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\user_head.png"))
            {
                user_frame.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\user_head.png", UriKind.Absolute));
            }
            else
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\command_block.png"))
            {
                user_frame.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\command_block.png", UriKind.RelativeOrAbsolute));
            }
            user_frame_border.MouseLeftButtonUp += (a,b) => { if(UserData.TryGetValue("UserID", out string value)) System.Diagnostics.Process.Start("explorer.exe", "https://mc.metamo.cn/u/" + value); };
            #endregion

            #region 加载轮播图数据
            //读取本地现有轮播图数据
            List<string> TargetUrlList = new List<string> { };
            if(Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\link_data"))
            {
                foreach (string data in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "resources\\link_data","*.png"))
                {
                    if(!Path.GetFileNameWithoutExtension(data).EndsWith("Icon"))
                    TargetUrlList.Add(data);
                }
                rotationChartBody.SetAll(TargetUrlList);
            }
            #endregion

            #region 初始化生成器按钮

            #region 生成器背景图列表
            List<BitmapImage> spawner_background = new List<BitmapImage> { };
            //获取生成器图片列表
            string[] spawn_background_list = null;

            if (Directory.Exists(spawner_image_path))
                spawn_background_list = Directory.GetFiles(spawner_image_path);
            List<FileNameString> spawnerBgPathSorter = new List<FileNameString> { };

            //分配值给比较器
            if(spawn_background_list != null && spawn_background_list.Length > 0)
                for (int i = 0; i < spawn_background_list.Length; i++)
                {
                    string current_path = Path.GetFileNameWithoutExtension(spawn_background_list[i]);
                    spawnerBgPathSorter.Add(new FileNameString() { FileName = current_path, FilePath = spawn_background_list[i], FileIndex = i });
                }
            FileNameComparer fileNameComparer = new FileNameComparer { };
            //比较器开始排序
            spawnerBgPathSorter.Sort(fileNameComparer);
            //遍历本地生成器图像目录
            foreach (FileNameString item in spawnerBgPathSorter)
            {
                //添加进背景图链表
                if (File.Exists(item.FilePath))
                    spawner_background.Add(new BitmapImage(new Uri(item.FilePath, UriKind.Absolute)));
                else
                    spawner_background.Add(null);
            }
            #endregion

            #region 生成布局,分配方法
            //实例化方法分配器,链接数据上下文
            GeneratorFunction sf = new GeneratorFunction(this);
            //遍历方法分配器中的启动器属性
            int current_column = 0;
            int current_row = 0;
            int spawner_background_length = spawner_background.Count;

            for (int i = 0; i < spawner_background_length; i++)
            {
                //检查是否需要重置行和列
                current_row = current_column > spawner_button_column ? ++current_row : current_row;
                current_column = current_column > spawner_button_column ? 0 : current_column;

                #region 实例化生成器按钮
                IconTextButtons spawner_btn = new IconTextButtons
                {
                    DataContext = sf,
                    Width = 188,
                    Height = 70,
                    BorderBrush = null,
                    NeedMouseOverStyle = true,
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                };
                spawner_btn.SetValue(StyleProperty, Application.Current.Resources["IconTextButton"]);

                if (spawner_background[i] != null)
                {
                    int function_index = int.Parse(Path.GetFileNameWithoutExtension(spawner_background[i].UriSource.ToString()));
                    spawner_btn.Background = new ImageBrush(spawner_background[i]);
                    spawner_btn.PressedBackground = new ImageBrush(new BitmapImage(new Uri(spawner_highlight_image_path+"\\"+function_index+".png",UriKind.Absolute)));
                    spawner_btn.Command = sf.spawner_functions[function_index];
                    spawner_btn.CommandParameter = this;
                }
                //在页面上生成对应的行或列
                ColumnDefinition cd = new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Auto)
                };
                RowDefinition rd = new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Auto)
                };
                #endregion
                GeneratorTable.ColumnDefinitions.Add(cd);
                GeneratorTable.RowDefinitions.Add(rd);
                GeneratorTable.Children.Add(spawner_btn);
                //设置按钮的行列的同时迭代1个单位
                Grid.SetRow(spawner_btn, current_row);
                Grid.SetColumn(spawner_btn, current_column++);
            }
            #endregion

            #endregion
        }

        /// <summary>
        /// 个性化设置
        /// </summary>
        private void IndividualizationForm(object sender, EventArgs e)
        {
            IndividualizationForm indivi_form = new IndividualizationForm();
            if(indivi_form.ShowDialog() == true)
            {

            }
        }

        /// <summary>
        /// 启动项设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatupItemClick(object sender, RoutedEventArgs e)
        {
            StartupItemForm sif = new StartupItemForm();

            #region 把当前的启动数据传递给启动项设置窗体
            //sif.AutoStartup.IsChecked = MainWindowProperties.AutoStart;

            sif.CloseToTray.IsChecked = MainWindowProperties.CloseToTray;
            sif.StateComboBox.SelectedIndex = 0;
            if (cbhk_visibility == MainWindowProperties.Visibility.MinState)
                sif.StateComboBox.SelectedIndex = 1;
            else
                if (cbhk_visibility == MainWindowProperties.Visibility.Close)
                sif.StateComboBox.SelectedIndex = 2;
            #endregion

            if (sif.ShowDialog() == true)
            {
                //主页可见性
                cbhk_visibility = sif.StateComboBox.SelectedIndex == 0? MainWindowProperties.Visibility.KeepState:(sif.StateComboBox.SelectedIndex == 1?MainWindowProperties.Visibility.MinState:MainWindowProperties.Visibility.Close);
                //是否开机自启
                //MainWindowProperties.AutoStart = sif.AutoStartup.IsChecked.Value;
                //是否关闭后缩小到托盘
                MainWindowProperties.CloseToTray = sif.CloseToTray.IsChecked.Value;
                //轮播图播放速度
            }
        }

        #region 窗体行为
        /// <summary>
        /// 由于不是主窗体，所以退出应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            SaveConfigs();
            taskbar_icon.Visibility = MainWindowProperties.CloseToTray ? Visibility.Visible : Visibility.Collapsed;
            ShowInTaskbar = cbhk_visibility != MainWindowProperties.Visibility.MinState;
            WindowState = WindowState.Minimized;

            if (!MainWindowProperties.CloseToTray)
            {
                taskbar_icon.Visibility = Visibility.Collapsed;
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 保存启动器配置
        /// </summary>
        private void SaveConfigs()
        {
            //保存的配置
            //if (MainWindowProperties.AutoStart)
            //    GeneralTools.AutoStart.CreateShortcut("C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup", "命令管家", AppDomain.CurrentDomain.BaseDirectory + "cbhk.exe", "命令管家1.19", AppDomain.CurrentDomain.BaseDirectory + "cb.ico");
            //else
            //    File.Delete("C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\命令管家.lnk");

            if(Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs"))
            {
                List<string> data = new List<string> { };
                data.Add("CBHKVisibility:" + cbhk_visibility.ToString());
                //data.Add("AutoStart:"+MainWindowProperties.AutoStart);
                data.Add("CloseToTray:" + MainWindowProperties.CloseToTray);
                data.Add("LinkAnimationDelay:" + MainWindowProperties.LinkAnimationDelay);
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\cbhk.ini", data.ToArray());
            }
        }

        /// <summary>
        /// 最小化窗体
        /// </summary>
        private void MinFormSize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 鼠标拖拽窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point title_range = e.GetPosition(TitleStack);
            if (title_range.X >= 0 && title_range.X < TitleStack.ActualWidth && title_range.Y >= 0 && title_range.Y < TitleStack.ActualHeight && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// 最大化窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    MaxWidth = SystemParameters.WorkArea.Width+16;
                    MaxHeight = SystemParameters.WorkArea.Height+16;
                    //BorderThickness = new Thickness(5); //最大化后需要调整
                    //Margin = new Thickness(0);
                    break;
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    TitleStack.Margin = new Thickness(100, 0, 0, 0);
                    //BorderThickness = new Thickness(5);
                    //Margin = new Thickness(10);
                    break;
            }

            //switch (WindowState)
            //{
            //    case WindowState.Maximized:
            //        //MaxWidth = SystemParameters.WorkArea.Width + 16;
            //        //MaxHeight = SystemParameters.WorkArea.Height + 16;
            //        //BorderThickness = new Thickness(5); //最大化后需要调整
            //        Left = Top = 0;
            //        MaxHeight = SystemParameters.WorkArea.Height;
            //        MaxWidth = SystemParameters.WorkArea.Width;
            //        break;
            //    case WindowState.Normal:
            //        BorderThickness = new Thickness(0);
            //        break;
            //}
        }
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel this_Panel = null;
            if (Equals(typeof(StackPanel), e.Source.GetType()))
                this_Panel = e.Source as StackPanel;
            else
                return;
            if(e.ClickCount == 2 && this_Panel.Name == "TitleStack")
            {
                WindowState = WindowState == WindowState.Maximized ?WindowState.Normal:WindowState.Maximized;
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SkeletonTimer.Enabled = true;
        }

        /// <summary>
        /// 切换标签页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            rotationChartBody.SwitchTimer.IsEnabled = tabControl.SelectedIndex == 1;
        }
    }

    /// <summary>
    /// 管家的启动属性
    /// </summary>
    public static class MainWindowProperties
    {
        /// <summary>
        /// 开机自启
        /// </summary>
        //public static bool AutoStart { get; set; } = false;

        /// <summary>
        /// 关闭后缩小到托盘
        /// </summary>
        public static bool CloseToTray { get; set; } = true;

        /// <summary>
        /// 轮播图播放延迟
        /// </summary>
        public static int LinkAnimationDelay { get; set; } = 3;

        /// <summary>
        /// 主页可见性
        /// </summary>
        public enum Visibility
        {
            KeepState,
            MinState,
            Close
        }
    }
}

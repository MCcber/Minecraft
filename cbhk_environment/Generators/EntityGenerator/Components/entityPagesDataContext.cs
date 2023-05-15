﻿using cbhk_environment.ControlsDataContexts;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    public partial class entityPagesDataContext : ObservableObject
    {
        #region 运行指令、保存等指令
        public RelayCommand RunCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ClearUnnecessaryData { get; set; }
        #endregion

        #region 指示是否需要展示生成结果
        private bool showResult = false;
        public bool ShowResult
        {
            get
            {
                return showResult;
            }
            set
            {
                showResult = value;
            }
        }
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

        #region 作为工具
        public bool UseForTool { get; set; } = false;
        #endregion

        #region 当前实体页引用
        EntityPages currentEntityPage = null;
        #endregion

        #region 通用NBT

        #region 全选和反选bool型NBT
        public RelayCommand<FrameworkElement> SelectAllBoolNBTs { get; set; }
        public RelayCommand<FrameworkElement> ReverseAllBoolNBTs { get; set; }
        #endregion

        #region 共通标签是否开放编辑

        #region 允许编辑实体共通标签
        private bool entityCommonTagsEnabled = true;
        public bool EntityCommonTagsEnabled
        {
            get
            {
                return entityCommonTagsEnabled;
            }
            set
            {
                entityCommonTagsEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 允许编辑活体共通标签
        private bool livingBodyCommonTagsEnabled = true;
        public bool LivingBodyCommonTagsEnabled
        {
            get
            {
                return livingBodyCommonTagsEnabled;
            }
            set
            {
                livingBodyCommonTagsEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 允许编辑活体共通标签
        private bool mobCommonTagsEnabled = true;
        public bool MobCommonTagsEnabled
        {
            get
            {
                return mobCommonTagsEnabled;
            }
            set
            {
                mobCommonTagsEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

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

        #region 其余共通标签显示隐藏

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

        #region 箭类投掷物共通标签可见性
        private Visibility arrowProjectileCommonTagsVisibility = Visibility.Collapsed;
        public Visibility ArrowProjectileCommonTagsVisibility
        {
            get
            {
                return arrowProjectileCommonTagsVisibility;
            }
            set
            {
                arrowProjectileCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 容器实体共通标签可见性
        private Visibility containerEntityCommonTagsVisibility = Visibility.Collapsed;
        public Visibility ContainerEntityCommonTagsVisibility
        {
            get
            {
                return containerEntityCommonTagsVisibility;
            }
            set
            {
                containerEntityCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 火球类投掷物共通标签可见性
        private Visibility fireballProjectileCommonTagsVisibility = Visibility.Collapsed;
        public Visibility FireballProjectileCommonTagsVisibility
        {
            get
            {
                return fireballProjectileCommonTagsVisibility;
            }
            set
            {
                fireballProjectileCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 物品类投掷物共通标签可见性
        private Visibility itemProjectileCommonTagsVisibility = Visibility.Collapsed;
        public Visibility ItemProjectileCommonTagsVisibility
        {
            get
            {
                return itemProjectileCommonTagsVisibility;
            }
            set
            {
                itemProjectileCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 矿车共通标签可见性
        private Visibility mineCartCommonTagsVisibility = Visibility.Collapsed;
        public Visibility MineCartCommonTagsVisibility
        {
            get
            {
                return mineCartCommonTagsVisibility;
            }
            set
            {
                mineCartCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 药水效果共通标签可见性
        private Visibility potionEffectCommonTagsVisibility = Visibility.Collapsed;
        public Visibility PotionEffectCommonTagsVisibility
        {
            get
            {
                return potionEffectCommonTagsVisibility;
            }
            set
            {
                potionEffectCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 投掷物共通标签可见性
        private Visibility projectileCommonTagsVisibility = Visibility.Collapsed;
        public Visibility ProjectileCommonTagsVisibility
        {
            get
            {
                return projectileCommonTagsVisibility;
            }
            set
            {
                projectileCommonTagsVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #endregion

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
                {
                    UpdateUILayOut();
                }
            }
        }
        public string SelectedEntityIdString
        {
            get
            {
                string result = "";
                result = MainWindow.EntityDataBase.Where(item => item.Key[(item.Key.IndexOf(':') + 1)..] == SelectedEntityId.ComboBoxItemText).First().Key;
                result = GetEntityID().Match(result).ToString();
                return result.Trim() != "" ? result : "";
            }
        }
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
        ImageBrush buttonNormalBrush;
        ImageBrush buttonPressedBrush;
        string NBTStructureFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Entity\\data\\";
        string SpecialNBTStructureFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Entity\\data\\SpecialTags.json";
        string entityImageFolderPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\";
        string itemImageFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
        #endregion

        #region 字段与引用
        //属性数据
        public ObservableCollection<NBTDataStructure> AttributeResult { get; set; } = new();
        //存储当前实体的乘客
        public ObservableCollection<NBTDataStructure> PassengerResult { get; set; } = new();
        //实体数据源
        public ObservableCollection<IconComboBoxItem> EntityIds { get; set; } = new();
        //版本
        public ObservableCollection<string> VersionSource { get; set; } = new() { "1.12-", "1.13+" };
        //特指结果集合
        public Dictionary<string,ObservableCollection<NBTDataStructure>> SpecialTagsResult { get; set; } = new();
        //实体、活体、生物结果集合
        public ObservableCollection<NBTDataStructure> CommonResult { get; set; } = new();
        //在生成时标记当前实体拥有哪些共通标签
        private List<string> CurrentCommonTags = new();
        //白色画刷
        SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        //黑色画刷
        SolidColorBrush blackBrush = new((Color)ColorConverter.ConvertFromString("#000000"));
        //橙色画刷
        SolidColorBrush orangeBrush = new((Color)ColorConverter.ConvertFromString("#FFE5B663"));
        //存储外部读取进来的实体数据
        public JObject ExternallyReadEntityData { get; set; } = null;

        #region 标记当前是否为导入外部数据的模式
        private bool importMode = false;
        public bool ImportMode
        {
            get
            {
                return importMode;
            }
            set
            {
                importMode = value;

            }
        }
        #endregion

        #region 是否同步到文件
        public bool SyncToFile { get; set; }
        public string ExternFilePath { get; set; }
        #endregion

        //获取实体英文id
        [GeneratedRegex("[a-zA-Z_]+")]
        private partial Regex GetEntityID();
        //特殊实体的共通标签链表
        List<string> specialEntityCommonTagList = new();
        //特殊实体特指标签字典,用于动态切换内容
        Dictionary<string,Grid> specialDataDictionary = new();
        //特殊标签面板
        ScrollViewer SpecialViewer = null;
        //存储最终的结果
        private string Result { get; set; }
        #endregion

        public entityPagesDataContext()
        {
            #region 连接指令
            RunCommand = new RelayCommand(run_command);
            SaveCommand = new RelayCommand(save_command);
            SelectAllBoolNBTs = new RelayCommand<FrameworkElement>(SelectAllBoolNBTsCommand);
            ReverseAllBoolNBTs = new RelayCommand<FrameworkElement>(ReverseAllBoolNBTsCommand);
            ClearUnnecessaryData = new RelayCommand(ClearUnnecessaryDataCommand);
            #endregion

            #region 初始化字段
            buttonNormalBrush = new ImageBrush(new BitmapImage(new Uri(buttonNormalImage, UriKind.RelativeOrAbsolute)));
            buttonPressedBrush = new ImageBrush(new BitmapImage(new Uri(buttonPressedImage, UriKind.RelativeOrAbsolute)));

            string SpecialData = File.ReadAllText(SpecialNBTStructureFilePath);
            JArray specialArray = JArray.Parse(SpecialData);
            string entityID;
            string entityName = "";
            for (int i = 0; i < specialArray.Count; i++)
            {
                IconComboBoxItem item = new();
                entityID = specialArray[i]["type"].ToString();
                string iconPath = File.Exists(entityImageFolderPath + entityID + "_spawn_egg.png") ? entityImageFolderPath + entityID + "_spawn_egg.png" : entityImageFolderPath + entityID + ".png";
                List<string> entityNameObj = MainWindow.EntityDataBase.Where(item => item.Key[..item.Key.IndexOf(':')] == entityID).Select(item=>item.Key).ToList();
                if (entityNameObj.Count > 0)
                    entityName = entityNameObj[0][(entityNameObj[0].IndexOf(':') + 1)..];
                else
                    entityName = "";
                if (entityName.Length > 0)
                {
                    item.ComboBoxItemText = entityName;
                    EntityIds.Add(item);
                }
                if (File.Exists(iconPath))
                    item.ComboBoxItemIcon = new BitmapImage(new Uri(iconPath, UriKind.RelativeOrAbsolute));
            }
            #endregion
        }
        
        /// <summary>
        /// 载入实体页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EntityPageLoaded(object sender,RoutedEventArgs e)
        {
            currentEntityPage = sender as EntityPages;
        }

        /// <summary>
        /// 搜索当前实体下拥有哪些共通标签,用于过滤当前实体不存在的数据
        /// </summary>
        private void CollectionCommonTagsMark()
        {
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
            CurrentCommonTags.Clear();
            CurrentCommonTags = JArray.Parse(targetObj["common"].ToString()).ToList().ConvertAll(item=>item.ToString());
        }

        /// <summary>
        /// 最终结算
        /// </summary>
        /// <param name="MultipleMode"></param>
        private void FinalSettlement(object MultipleOrExtern)
        {
            CollectionCommonTagsMark();
            Result = string.Join(",",SpecialTagsResult[SelectedEntityIdString].Select(item =>
            {
                if (item != null && item.Result.Length > 0)
                    return item.Result;
                return "";
            })) + "," + string.Join(",", CommonResult.Select(item =>
            {
                if (CurrentCommonTags.Contains(item.NBTGroup) && item.Result.Length > 0)
                    return item.Result;
                else
                    return "";
            }));
            Result = Result.Trim(',');
            if (Summon)
            Result = Result.Trim() != "" ? "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~ {" + Result + "}" : "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~";
            else
            {
                if (SelectedVersion == "1.12-")
                    Result = "give @p minecraft:spawner_egg 1 0 {EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\" " + (Result.Length > 0 ? "," + Result : "") + "}}";
                else
                    Result = "give @p minecraft:pig_spawner_egg{EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\"" + (Result.Length > 0 ? "," + Result : "") + "}} 1";
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
        /// 运行保存
        /// </summary>
        private void save_command()
        {
            //执行生成
            FinalSettlement(false);
            Microsoft.Win32.SaveFileDialog saveFileDialog = new()
            {
                AddExtension = true,
                RestoreDirectory = true,
                CheckPathExists = true,
                DefaultExt = "command",
                Filter = "Command files (*.command;)|*.command;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Title = "保存为命令文件"
            };
            if (saveFileDialog.ShowDialog().Value && Directory.Exists(Path.GetDirectoryName(saveFileDialog.FileName)))
                File.WriteAllText(saveFileDialog.FileName, Result);
        }

        /// <summary>
        /// 运行生成
        /// </summary>
        private void run_command()
        {
            CollectionCommonTagsMark();
            Result = string.Join(",",SpecialTagsResult[SelectedEntityIdString].Select(item =>
            {
                if (item != null && item.Result.Length > 0)
                    return item.Result;
                return "";
            })) + "," + string.Join(",", CommonResult.Select(item =>
            {
                if (CurrentCommonTags.Contains(item.NBTGroup) && item.Result.Length > 0)
                    return item.Result;
                else
                    return "";
            }));
            Result = Regex.Replace(Result.Trim(','), @",{2,}", ",");
            if (UseForTool)
            {
                Result = "{" + Result + "}";
                Entity entity = Window.GetWindow(currentEntityPage) as Entity;
                entity.DialogResult = true;
                return;
            }
            if (Summon)
                Result = Result.Trim() != "" ? "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~ {" + Result + "}" : "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~";
            else
            {
                if (SelectedVersion == "1.12-")
                    Result = "give @p minecraft:spawner_egg 1 0 {EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\" " + (Result.Length > 0 ? "," + Result : "") + "}}";
                else
                    Result = "give @p minecraft:pig_spawner_egg{EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\"" + (Result.Length > 0 ? "," + Result : "") + "}} 1";
            }

            if(SyncToFile && ExternFilePath.Length > 0 && File.Exists(ExternFilePath))
                File.WriteAllText(ExternFilePath, Result);

            if (ShowResult)
            {
                Displayer displayer = Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "实体", icon_path);
                displayer.Topmost = true;
                displayer.Show();
                displayer.Focus();
                displayer.Topmost = false;
            }
            else
                Clipboard.SetText(Result);
        }

        public string run_command(bool showResult)
        {
            CollectionCommonTagsMark();
            string AttributesData = AttributeResult.Count > 0 ? "Attributes:[" + string.Join(",", AttributeResult.Select(item => item.Result)).Trim(',') + "]" : "";
            AttributesData = AttributesData == "Attributes:[]" ? "" : AttributesData;
            string PassengersData = PassengerResult.Count > 0 ? "Passengers:[" + string.Join(",", PassengerResult.Select(item => item.Result)).Trim(',') + "]" : "";
            PassengersData = PassengersData == "Passengers:[]" ? "" : PassengersData;

            Result = string.Join(",", SpecialTagsResult[SelectedEntityIdString].Select(item =>
            {
                if (item != null && item.Result.Length > 0)
                    return item.Result;
                return "";
            })) + "," + string.Join(",", CommonResult.Select(item =>
            {
                if (CurrentCommonTags.Contains(item.NBTGroup) && item.Result.Length > 0)
                    return item.Result;
                else
                    return "";
            })) + "," + AttributesData + "," + PassengersData;
            Result = Regex.Replace(Result.Trim(','), @",{2,}", ",");
            if (Summon)
                Result = Result.Trim() != "" ? "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~ {" + Result + "}" : "summon minecraft:" + SelectedEntityIdString + " ~ ~ ~";
            else
            {
                if (SelectedVersion == "1.12-")
                    Result = "give @p minecraft:spawner_egg 1 0 {EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\" " + (Result.Length > 0 ? "," + Result : "") + "}}";
                else
                    Result = "give @p minecraft:pig_spawner_egg{EntityTag:{id:\"minecraft:" + SelectedEntityIdString + "\"" + (Result.Length > 0 ? "," + Result : "") + "}} 1";
            }

            if (showResult)
            {
                Displayer displayer = Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result, "实体", icon_path);
                displayer.Topmost = true;
                displayer.Show();
                displayer.Focus();
                displayer.Topmost = false;
            }
            else
                Clipboard.SetText(Result);
            return Result;
        }

        /// <summary>
        /// 清除不需要的数据
        /// </summary>
        private void ClearUnnecessaryDataCommand()
        {
            Grid currentGrid = specialDataDictionary[SelectedEntityIdString];
            specialDataDictionary.Clear();
            specialDataDictionary.Add(SelectedEntityIdString, currentGrid);
            foreach (var item in SpecialTagsResult)
            {
                if (item.Key != SelectedEntityIdString)
                    item.Value.Clear();
            }
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
                Uid = Request.nbtType,
                Text = Request.description.TrimEnd('。').TrimEnd('.'),
                Foreground = whiteBrush,
                HorizontalAlignment = HorizontalAlignment.Right,
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
                        BlockState blockState = new()
                        {
                            Uid = Request.nbtType,
                            Tag = new NBTDataStructure() { DataType = Request.dataType,NBTGroup = Request.nbtType },
                            Name = Request.key
                        };
                        blockState.BlockID.ToolTip = Request.description;
                        ToolTipService.SetBetweenShowDelay(blockState.BlockID,0);
                        ToolTipService.SetInitialShowDelay(blockState.BlockID, 0);
                        BlockStateDataContext blockStateDataContext = blockState.AttributeAccordion.DataContext as BlockStateDataContext;
                        Grid.SetColumnSpan(blockState,2);
                        blockState.GotFocus += componentEvents.ValueChangedHandler;
                        result.Remove(displayText);
                        result.Add(blockState);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Give)
                                key = "EntityTag." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if (currentObj != null)
                            {
                                JToken BlockID = currentObj["Name"];
                                if(BlockID != null)
                                {
                                    string BlockIDString = BlockID.ToString();
                                    JToken BlockProperties = currentObj["Properties"];
                                    string blockName = MainWindow.BlockDataBase.Where(item => item.Key[..item.Key.IndexOf(':')] == BlockIDString).First().Key;
                                    blockName = blockName[(blockName.IndexOf(':') + 1)..];
                                    blockStateDataContext.SelectedBlockID = blockStateDataContext.BlockList.Where(item=>item.ComboBoxItemText == blockName).First();
                                    if (BlockProperties != null)
                                    {
                                        List<JProperty> properties = (BlockProperties as JObject).Properties().ToList();
                                        int PropertyCount = properties.Count;
                                        for (int i = 0; i < PropertyCount; i++)
                                        {
                                            blockStateDataContext.AddAttributeCommand(blockState.AttributeAccordion);
                                            blockStateDataContext.AttributeKeys.Add(properties[i].Name);
                                            blockStateDataContext.SelectedAttributeKeys[^1] = properties[i].Name;
                                            blockStateDataContext.SelectedAttributeValues[^1] = properties[i].Value.ToString();
                                        }
                                        blockState.Focus();
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    break;
                case "TAG_List":
                    {
                        if (Request.dependency != null && Request.dependency.Length > 0)
                        {
                            switch (Request.dependency)
                            {
                                case "ItemGenerator":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            JToken data = ExternallyReadEntityData.SelectToken(key);
                                            if (data != null)
                                            {
                                                JArray Items = JArray.Parse(data.ToString());
                                                string imagePath = "";
                                                for (int i = 0; i < Items.Count; i++)
                                                {
                                                    string itemID = JObject.Parse(Items[i].ToString())["id"].ToString();
                                                    Image image = new() { Tag = new NBTDataStructure() { Result = Items[i].ToString(), Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                                                    imagePath = itemImageFilePath + itemID + ".png";
                                                    if (File.Exists(imagePath))
                                                        image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                                                    EntityBag entityBag = new();
                                                    (entityBag.ItemIcon.Child as Image).Source = image.Source;
                                                    itemPanel.Children.Add(entityBag);
                                                }
                                                itemAccordion.Focus();
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "AreaEffectCloudEffects":
                                    {
                                        Accordion areaEffectCloudEffectsAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        areaEffectCloudEffectsAccordion.Content = scrollViewer;
                                        areaEffectCloudEffectsAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(areaEffectCloudEffectsAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray Effects)
                                            {
                                                for (int i = 0; i < Effects.Count; i++)
                                                {
                                                    componentEvents.AddAreaEffectCloudCommand(areaEffectCloudEffectsAccordion);
                                                    StackPanel contentPanel = (itemPanel.Children[i] as AreaEffectCloudEffects).EffectListPanel;
                                                    #region 提取数据
                                                    JToken Ambient = Effects[i]["Ambient"];
                                                    JToken Amplifier = Effects[i]["Amplifier"];
                                                    JToken Duration = Effects[i]["Duration"];
                                                    JToken Id = Effects[i]["Id"];
                                                    JToken ShowIcon = Effects[i]["ShowIcon"];
                                                    JToken ShowParticles = Effects[i]["ShowParticles"];
                                                    JToken effect_changed_timestamp = Effects[i].SelectToken("FactorCalculationData.effect_changed_timestamp");
                                                    JToken factor_current = Effects[i].SelectToken("FactorCalculationData.factor_current");
                                                    JToken factor_previous_frame = Effects[i].SelectToken("FactorCalculationData.factor_previous_frame");
                                                    JToken factor_start = Effects[i].SelectToken("FactorCalculationData.factor_start");
                                                    JToken factor_target = Effects[i].SelectToken("FactorCalculationData.factor_target");
                                                    JToken had_effect_last_tick = Effects[i].SelectToken("FactorCalculationData.had_effect_last_tick");
                                                    JToken padding_duration = Effects[i].SelectToken("FactorCalculationData.padding_duration");
                                                    #endregion
                                                    #region 应用数据
                                                    if (Ambient != null)
                                                    contentPanel.FindChild<TextCheckBoxs>("Ambient").IsChecked = Ambient.ToString() == "1";
                                                    if(Amplifier != null)
                                                    contentPanel.FindChild<Slider>("Amplifier").Value = byte.Parse(Amplifier.ToString());
                                                    if(Duration != null)
                                                    contentPanel.FindChild<Slider>("Duration").Value = int.Parse(Duration.ToString());
                                                    if(Id != null)
                                                    {
                                                        string id = Id.ToString().Replace("minecraft:","");
                                                        string currentID = MainWindow.MobEffectDataBase.Keys.Where(item => item == id).First();
                                                        contentPanel.FindChild<ComboBox>("Id").SelectedIndex = MainWindow.MobEffectDataBase.Keys.ToList().IndexOf(currentID);
                                                    }
                                                    if (ShowIcon != null)
                                                    contentPanel.FindChild<TextCheckBoxs>("ShowIcon").IsChecked = ShowIcon.ToString() == "1";
                                                    if(ShowParticles != null)
                                                    contentPanel.FindChild<TextCheckBoxs>("ShowParticles").IsChecked = ShowParticles.ToString() == "1";
                                                    Grid grid = (contentPanel.FindChild<Accordion>().Content as ScrollViewer).Content as Grid;
                                                    if(effect_changed_timestamp != null)
                                                    grid.FindChild<Slider>("effect_changed_timestamp").Value = int.Parse(effect_changed_timestamp.ToString());
                                                    if(factor_current != null)
                                                    grid.FindChild<Slider>("factor_current").Value = int.Parse(factor_current.ToString());
                                                    if(factor_previous_frame != null)
                                                    grid.FindChild<Slider>("factor_previous_frame").Value = int.Parse(factor_previous_frame.ToString());
                                                    if(factor_start != null)
                                                    grid.FindChild<Slider>("factor_start").Value = int.Parse(factor_start.ToString());
                                                    if(factor_target != null)
                                                    grid.FindChild<Slider>("factor_target").Value = int.Parse(factor_target.ToString());
                                                    if(had_effect_last_tick != null)
                                                    grid.FindChild<TextCheckBoxs>("had_effect_last_tick").IsChecked = had_effect_last_tick.ToString() == "1";
                                                    if(padding_duration != null)
                                                    grid.FindChild<Slider>("padding_duration").Value = int.Parse(padding_duration.ToString());
                                                    #endregion
                                                }
                                                areaEffectCloudEffectsAccordion.Focus();
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "AttributeModifiers":
                                    {
                                        Accordion AttributesAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")),
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        AttributesAccordion.Content = scrollViewer;
                                        AttributesAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Remove(displayText);
                                        result.Add(AttributesAccordion);
                                        #region 分析是否需要代入导入的数据
                                        if(ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray AttributeArray)
                                            {
                                                foreach (var attributeObj in AttributeArray)
                                                {
                                                    componentEvents.AddAttributeCommand(AttributesAccordion);
                                                    Attributes attributes = itemPanel.Children[^1] as Attributes;
                                                    //基础值
                                                    attributes.Base.Value = double.Parse(attributeObj["Base"].ToString());
                                                    //属性名
                                                    JToken attributeName = attributeObj["Name"].ToString();
                                                    //确定应该选中哪个成员
                                                    ObservableCollection<string> attributeNameList = attributes.AttributeName.ItemsSource as ObservableCollection<string>;
                                                    attributes.AttributeName.SelectedValue = attributeName.ToString();
                                                    if (attributeObj.SelectToken("Modifiers") is JArray Modifiers)
                                                    {
                                                        foreach (JToken item in Modifiers)
                                                        {
                                                            attributes.AddModifierCommand(null);
                                                            JToken amountObj = item.SelectToken("Amount");
                                                            JToken nameObj = item.SelectToken("Name");
                                                            JToken operationObj = item.SelectToken("Operation");
                                                            JArray uuidArray = item.SelectToken("UUID") as JArray;
                                                            if (amountObj != null)
                                                                attributes.AttributeModifiersSource[^1].Amount.Value = double.Parse(amountObj.ToString());
                                                            if (nameObj != null)
                                                                attributes.AttributeModifiersSource[^1].ModifierName.Text = nameObj.ToString();
                                                            if (operationObj != null)
                                                                attributes.AttributeModifiersSource[^1].Operation.SelectedIndex = int.Parse(operationObj.ToString());
                                                            if (uuidArray != null)
                                                            {
                                                                int uid0 = int.Parse(uuidArray[0].ToString());
                                                                int uid1 = int.Parse(uuidArray[1].ToString());
                                                                int uid2 = int.Parse(uuidArray[2].ToString());
                                                                int uid3 = int.Parse(uuidArray[3].ToString());
                                                                attributes.AttributeModifiersSource[^1].UUID.number0.Value = uid0;
                                                                attributes.AttributeModifiersSource[^1].UUID.number1.Value = uid1;
                                                                attributes.AttributeModifiersSource[^1].UUID.number2.Value = uid2;
                                                                attributes.AttributeModifiersSource[^1].UUID.number3.Value = uid3;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "Passengers":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            JToken data = ExternallyReadEntityData.SelectToken(key);
                                            if (data != null)
                                            {
                                                JArray Entities = JArray.Parse(data.ToString());
                                                string imagePath = "";
                                                for (int i = 0; i < Entities.Count; i++)
                                                {
                                                    string itemID = JObject.Parse(Entities[i].ToString())["id"].ToString();
                                                    Image image = new() { Tag = new NBTDataStructure() { Result = Entities[i].ToString(), Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                                                    imagePath = itemImageFilePath + itemID + ".png";
                                                    if (File.Exists(imagePath))
                                                        image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                                                    PassengerItems passengerItems = new();
                                                    (passengerItems.DisplayEntity.Child as Image).Source = image.Source;
                                                    itemPanel.Children.Add(passengerItems);
                                                }
                                                itemAccordion.Focus();
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "ArmorDropChances":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ArmorDropChances armorDropChances = new();
                                        itemPanel.Children.Add(armorDropChances);
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray data)
                                            {
                                                armorDropChances.boots.Value = double.Parse(data[0].ToString());
                                                armorDropChances.legs.Value = double.Parse(data[1].ToString());
                                                armorDropChances.chest.Value = double.Parse(data[2].ToString());
                                                armorDropChances.helmet.Value = double.Parse(data[3].ToString());
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "ArmorItems":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ArmorItems armorItems = new();
                                        itemPanel.Children.Add(armorItems);
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray data)
                                            {
                                                armorItems.boots.Tag = data[0].ToString();
                                                armorItems.legs.Tag = data[1].ToString();
                                                armorItems.chest.Tag = data[2].ToString();
                                                armorItems.helmet.Tag = data[3].ToString();
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "HandDropChances":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ArmorDropChances armorDropChances = new();
                                        itemPanel.Children.Add(armorDropChances);
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray data)
                                            {
                                                armorDropChances.boots.Value = double.Parse(data[0].ToString());
                                                armorDropChances.legs.Value = double.Parse(data[1].ToString());
                                                armorDropChances.chest.Value = double.Parse(data[2].ToString());
                                                armorDropChances.helmet.Value = double.Parse(data[3].ToString());
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "HandItems":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(2, 2, 2, 0),
                                            TitleForeground = blackBrush,
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        HandItems handItems = new();
                                        itemPanel.Children.Add(handItems);
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            if (ExternallyReadEntityData.SelectToken(key) is JArray data)
                                            {
                                                handItems.mainhand.Tag = data[0].ToString();
                                                handItems.offhand.Tag = data[1].ToString();
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case "TAG_Compound":
                    {
                        if (Request.dependency != null && Request.dependency.Length > 0)
                        {
                            switch (Request.dependency)
                            {
                                case "SuspectsEntity":
                                    {
                                        SuspectsEntities suspectsEntities = new()
                                        {
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Tag = new NBTDataStructure() { DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        suspectsEntities.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Remove(displayText);
                                        result.Add(suspectsEntities);
                                        #region 分析是否需要代入导入的数据
                                        if(ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag.";
                                            else
                                                key = "";
                                            JArray suspectArray = ExternallyReadEntityData.SelectToken(key+"anger.suspects") as JArray;
                                            Accordion accordion = suspectsEntities.FindChild<Accordion>();
                                            StackPanel stackPanel = (accordion.Content as ScrollViewer).Content as StackPanel;
                                            for (int i = 0; i < suspectArray.Count; i++)
                                            {
                                                suspectsEntities.AddSuspectsEntitiesCommand(accordion);
                                                StackPanel contentPanel = stackPanel.Children[^1] as StackPanel;
                                                JToken anger = ExternallyReadEntityData.SelectToken(key+"anger.suspects[" + i + "].anger");
                                                JToken uuid = ExternallyReadEntityData.SelectToken(key + "anger.suspects[" + i + "].uuid").ToString();
                                                if (anger != null)
                                                (contentPanel.Children[0] as Slider).Value = int.Parse(anger.ToString());
                                                if(uuid != null)
                                                {
                                                    JArray uuids = JArray.Parse(uuid.ToString());
                                                    UUIDOrPosGroup uUIDOrPosGroup = contentPanel.Children[0] as UUIDOrPosGroup;
                                                    uUIDOrPosGroup.number0.Value = int.Parse(uuids[0].ToString());
                                                    uUIDOrPosGroup.number1.Value = int.Parse(uuids[1].ToString());
                                                    uUIDOrPosGroup.number2.Value = int.Parse(uuids[2].ToString());
                                                    uUIDOrPosGroup.number3.Value = int.Parse(uuids[3].ToString());
                                                    uUIDOrPosGroup.IsUUID = true;
                                                    uUIDOrPosGroup.EnableButton.IsChecked = true;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "VibrationMonitor":
                                    {
                                        VibrationMonitors vibrationMonitors = new()
                                        {
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Tag = new NBTDataStructure() { DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        vibrationMonitors.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Remove(displayText);
                                        result.Add(vibrationMonitors);
                                        #region 分析是否需要代入导入的数据
                                        if(ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            else
                                                key = "";
                                            vibrationMonitors.VibrationMonitorsEnableButton.IsChecked = ExternallyReadEntityData.SelectToken(key + "listener") != null;
                                            #region 游戏事件
                                            JToken gameEvent = ExternallyReadEntityData.SelectToken(key+"listener.event.game_event");
                                            JToken distance = ExternallyReadEntityData.SelectToken(key + "listener.event.distance");
                                            JArray event_pos = ExternallyReadEntityData.SelectToken(key + "listener.event.pos") as JArray;
                                            JArray event_source = ExternallyReadEntityData.SelectToken(key + "listener.event.source") as JArray;
                                            JArray projectile_owner = ExternallyReadEntityData.SelectToken(key + "listener.event.projectile_owner") as JArray;
                                            #endregion
                                            #region 候选的游戏事件
                                            JToken gameEventC = ExternallyReadEntityData.SelectToken(key + "listener.selector.event.game_event");
                                            JToken distanceC = ExternallyReadEntityData.SelectToken(key + "listener.selector.event.distance");
                                            JArray event_posC = ExternallyReadEntityData.SelectToken(key + "listener.selector.event.pos") as JArray;
                                            JArray event_sourceC = ExternallyReadEntityData.SelectToken(key + "listener.selector.event.source") as JArray;
                                            JArray projectile_ownerC = ExternallyReadEntityData.SelectToken(key + "listener.selector.event.projectile_owner") as JArray;
                                            JToken event_delayC = ExternallyReadEntityData.SelectToken(key + "listener.event_delay");
                                            JToken selector_tick = ExternallyReadEntityData.SelectToken(key + "listener.selector.tick");
                                            #endregion
                                            JToken range = ExternallyReadEntityData.SelectToken(key + "listener.range");
                                            JToken event_delay = ExternallyReadEntityData.SelectToken(key + "listener.event_delay");
                                            JToken source_type = ExternallyReadEntityData.SelectToken(key + "listener.source.type");
                                            //振动监听器正在监听的游戏事件
                                            if (gameEvent != null)
                                                vibrationMonitors.game_event.Text = gameEvent.ToString();
                                            if (distance != null)
                                                vibrationMonitors.distance.Value = float.Parse(distance.ToString());
                                            if (event_pos != null)
                                            {
                                                vibrationMonitors.VibrationSourcePos.EnableButton.IsChecked = true;
                                                vibrationMonitors.VibrationSourcePos.IsUUID = false;
                                                vibrationMonitors.VibrationSourcePos.number0.Value = double.Parse(event_pos[0].ToString());
                                                vibrationMonitors.VibrationSourcePos.number1.Value = double.Parse(event_pos[1].ToString());
                                                vibrationMonitors.VibrationSourcePos.number2.Value = double.Parse(event_pos[2].ToString());
                                            }
                                            if(event_source != null)
                                            {
                                                vibrationMonitors.TargetUUID.EnableButton.IsChecked = true;
                                                vibrationMonitors.TargetUUID.number0.Value = int.Parse(event_source[0].ToString());
                                                vibrationMonitors.TargetUUID.number1.Value = int.Parse(event_source[1].ToString());
                                                vibrationMonitors.TargetUUID.number2.Value = int.Parse(event_source[2].ToString());
                                                vibrationMonitors.TargetUUID.number3.Value = int.Parse(event_source[3].ToString());
                                            }
                                            if(projectile_owner != null)
                                            {
                                                vibrationMonitors.ProjectileUUID.EnableButton.IsChecked = true;
                                                vibrationMonitors.ProjectileUUID.number0.Value = int.Parse(projectile_owner[0].ToString());
                                                vibrationMonitors.ProjectileUUID.number1.Value = int.Parse(projectile_owner[1].ToString());
                                                vibrationMonitors.ProjectileUUID.number2.Value = int.Parse(projectile_owner[2].ToString());
                                                vibrationMonitors.ProjectileUUID.number3.Value = int.Parse(projectile_owner[3].ToString());
                                            }
                                            if (event_delay != null)
                                                vibrationMonitors.EventDelay.Value = int.Parse(event_delay.ToString());
                                            if (range != null)
                                                vibrationMonitors.Range.Value = int.Parse(range.ToString());
                                            //振动选择器
                                            if(selector_tick != null)
                                                vibrationMonitors.tick.Value = int.Parse(selector_tick.ToString());
                                            if (gameEventC != null)
                                                vibrationMonitors.game_eventC.Text = gameEventC.ToString();
                                            if (distanceC != null)
                                                vibrationMonitors.distanceC.Value = float.Parse(distanceC.ToString());
                                            if(event_posC != null)
                                            {
                                                vibrationMonitors.VibrationSourcePosC.EnableButton.IsChecked = true;
                                                vibrationMonitors.VibrationSourcePosC.IsUUID = false;
                                                vibrationMonitors.VibrationSourcePosC.number0.Value = double.Parse(event_posC[0].ToString());
                                                vibrationMonitors.VibrationSourcePosC.number1.Value = double.Parse(event_posC[1].ToString());
                                                vibrationMonitors.VibrationSourcePosC.number2.Value = double.Parse(event_posC[2].ToString());
                                            }
                                            if(event_sourceC != null)
                                            {
                                                vibrationMonitors.TargetUUIDC.EnableButton.IsChecked = true;
                                                vibrationMonitors.TargetUUIDC.number0.Value = int.Parse(event_sourceC[0].ToString());
                                                vibrationMonitors.TargetUUIDC.number1.Value = int.Parse(event_sourceC[1].ToString());
                                                vibrationMonitors.TargetUUIDC.number2.Value = int.Parse(event_sourceC[2].ToString());
                                                vibrationMonitors.TargetUUIDC.number3.Value = int.Parse(event_sourceC[3].ToString());
                                            }
                                            if(projectile_ownerC != null)
                                            {
                                                vibrationMonitors.ProjectileUUIDC.EnableButton.IsChecked = true;
                                                vibrationMonitors.ProjectileUUIDC.number0.Value = int.Parse(projectile_ownerC[0].ToString());
                                                vibrationMonitors.ProjectileUUIDC.number1.Value = int.Parse(projectile_ownerC[1].ToString());
                                                vibrationMonitors.ProjectileUUIDC.number2.Value = int.Parse(projectile_ownerC[2].ToString());
                                                vibrationMonitors.ProjectileUUIDC.number3.Value = int.Parse(projectile_ownerC[3].ToString());
                                            }
                                            //振动监听器的位置数据
                                            if (source_type != null)
                                            {
                                                if (source_type.ToString() == "block")
                                                    vibrationMonitors.VibrationMonitorTypeBox.SelectedIndex = 0;
                                                else
                                                    vibrationMonitors.VibrationMonitorTypeBox.SelectedIndex = 1;
                                                JArray listenerSourcePos = ExternallyReadEntityData.SelectToken(key + "listener.source.pos") as JArray;
                                                JToken listenerSourceSourceEntity = ExternallyReadEntityData.SelectToken(key + "listener.source.source_entity");
                                                JToken yOffset = ExternallyReadEntityData.SelectToken(key + "listener.source.y_offset");
                                                if (listenerSourcePos != null)
                                                {
                                                    vibrationMonitors.BlockGroup.Visibility = Visibility.Visible;
                                                    vibrationMonitors.EntityGroup0.Visibility = vibrationMonitors.EntityGroup1.Visibility = Visibility.Collapsed;
                                                    vibrationMonitors.pos.IsUUID = false;
                                                    vibrationMonitors.pos.EnableButton.IsChecked = true;
                                                    vibrationMonitors.pos.number0.Value = int.Parse(listenerSourcePos[0].ToString());
                                                    vibrationMonitors.pos.number1.Value = int.Parse(listenerSourcePos[1].ToString());
                                                    vibrationMonitors.pos.number2.Value = int.Parse(listenerSourcePos[2].ToString());
                                                }
                                                else
                                                {
                                                    if (listenerSourceSourceEntity != null)
                                                    {
                                                        if (listenerSourceSourceEntity is JArray)
                                                        {
                                                            JArray entityUUID = listenerSourceSourceEntity as JArray;
                                                            vibrationMonitors.source_entityUUID.EnableButton.IsChecked = true;
                                                            vibrationMonitors.source_entityUUID.number0.Value = int.Parse(entityUUID[0].ToString());
                                                            vibrationMonitors.source_entityUUID.number1.Value = int.Parse(entityUUID[1].ToString());
                                                            vibrationMonitors.source_entityUUID.number2.Value = int.Parse(entityUUID[2].ToString());
                                                            vibrationMonitors.source_entityUUID.number3.Value = int.Parse(entityUUID[3].ToString());
                                                        }
                                                        else
                                                            if (listenerSourceSourceEntity is JObject)
                                                        {
                                                            JToken entityID = ExternallyReadEntityData.SelectToken(key + "listener.source.source_entity.id");
                                                            if (entityID != null)
                                                            {
                                                                vibrationMonitors.sourceEntityDisplayer.Tag = entityID.ToString();
                                                                (vibrationMonitors.sourceEntityDisplayer.Child as Image).Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + entityID.ToString() + ".png"));
                                                            }
                                                            else
                                                                vibrationMonitors.source_entityValue.Value = int.Parse(listenerSourceSourceEntity.ToString());
                                                        }
                                                    }
                                                    if (yOffset != null)
                                                        vibrationMonitors.y_offset.Value = double.Parse(yOffset.ToString());
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "ItemGenerator":
                                    {
                                        Accordion itemAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(0, 0, 0, 2),
                                            TitleForeground = blackBrush,
                                            ModifyName = "添加",
                                            FreshName = "清空",
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        StackPanel itemPanel = new() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2F2F")) };
                                        ScrollViewer scrollViewer = new()
                                        {
                                            MaxHeight = 200,
                                            Content = itemPanel,
                                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                        };
                                        itemAccordion.Content = scrollViewer;
                                        itemAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(itemAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要代入导入的数据
                                        if (ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            JToken data = ExternallyReadEntityData.SelectToken(key);
                                            if (data != null)
                                            {
                                                JArray Items = JArray.Parse(data.ToString());
                                                string imagePath = "";
                                                for (int i = 0; i < Items.Count; i++)
                                                {
                                                    string itemID = JObject.Parse(Items[i].ToString())["id"].ToString();
                                                    Image image = new() { Tag = new NBTDataStructure() { Result = Items[i].ToString(), Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                                                    imagePath = itemImageFilePath + itemID + ".png";
                                                    if (File.Exists(imagePath))
                                                        image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                                                    EntityBag entityBag = new();
                                                    (entityBag.ItemIcon.Child as Image).Source = image.Source;
                                                    itemPanel.Children.Add(entityBag);
                                                }
                                                itemAccordion.Focus();
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "LeashData":
                                    {
                                        Accordion leashAccordion = new()
                                        {
                                            MaxHeight = 200,
                                            Uid = Request.nbtType,
                                            Name = Request.key,
                                            Style = Application.Current.Resources["AccordionStyle"] as Style,
                                            Background = orangeBrush,
                                            Title = Request.description,
                                            BorderThickness = new Thickness(0),
                                            Margin = new Thickness(10, 2, 10, 0),
                                            TitleForeground = blackBrush,
                                            ModifyVisibility = Visibility.Collapsed,
                                            FreshVisibility = Visibility.Collapsed,
                                            ModifyForeground = blackBrush,
                                            FreshForeground = blackBrush,
                                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                                        };
                                        LeashData leashData = new();
                                        leashAccordion.Content = leashData;
                                        leashAccordion.GotFocus += componentEvents.ValueChangedHandler;
                                        result.Add(leashAccordion);
                                        result.Remove(displayText);
                                        #region 分析是否需要导入数据
                                        if(ImportMode)
                                        {
                                            string key = Request.key;
                                            if (Give)
                                                key = "EntityTag." + key;
                                            JToken uuid = ExternallyReadEntityData.SelectToken(key + ".UUID");
                                            JToken x = ExternallyReadEntityData.SelectToken(key + ".X");
                                            JToken y = ExternallyReadEntityData.SelectToken(key + ".Y");
                                            JToken z = ExternallyReadEntityData.SelectToken(key + ".Z");
                                            leashData.Tied.IsChecked = uuid != null || x != null || y != null || z != null;
                                            if(uuid != null)
                                            {
                                                JArray uuidArray = JArray.Parse(uuid.ToString());
                                                leashData.TiedByEntity.IsChecked = true;
                                                leashData.tractor.EnableButton.IsChecked = true;
                                                leashData.BeingLed_Click(leashData.TiedByEntity,null);
                                                leashData.tractor.number0.Value = int.Parse(uuidArray[0].ToString());
                                                leashData.tractor.number1.Value = int.Parse(uuidArray[1].ToString());
                                                leashData.tractor.number2.Value = int.Parse(uuidArray[2].ToString());
                                                leashData.tractor.number3.Value = int.Parse(uuidArray[3].ToString());
                                            }
                                            else
                                            if (x != null && y != null && z != null)
                                            {
                                                leashData.TiedByFence.IsChecked = true;
                                                leashData.fence.EnableButton.IsChecked = true;
                                                leashData.TiedToAFence_Click(leashData.TiedByFence, null);
                                                leashData.fence.number0.Value = int.Parse(x.ToString());
                                                leashData.fence.number1.Value = int.Parse(y.ToString());
                                                leashData.fence.number2.Value = int.Parse(z.ToString());
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "Brain":
                                    break;
                            }
                        }
                    }
                    break;
                case "TAG_UUID_List":
                    {
                        UUIDListGenerator uUIDListGenerator = new()
                        {
                            Uid = Request.nbtType,
                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                        };
                        uUIDListGenerator.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(uUIDListGenerator);
                        result.Remove(displayText);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Give)
                                key = "EntityTag." + key;
                            JToken data = ExternallyReadEntityData.SelectToken(key);
                            if (data != null)
                            {
                                JArray uuidArray = JArray.Parse(data.ToString().Replace("I;",""));
                                for (int i = 0; i < uuidArray.Count; i++)
                                {
                                    JArray idToken = JArray.Parse(uuidArray[i].ToString());
                                    if(idToken != null)
                                    {
                                        string itemID = idToken[i].ToString();
                                    }
                                }
                                uUIDListGenerator.Focus();
                            }
                        }
                        #endregion
                    }
                    break;
                case "TAG_Float_Array":
                    {
                        JArray children = JArray.Parse(Request.children);
                        Grid floatGrid = new() { Uid = Request.nbtType, Name = Request.key, Tag = new NBTDataStructure() { Result = "",Visibility = Visibility.Collapsed,DataType = Request.dataType,NBTGroup = Request.nbtType } };

                        Binding visibilityBinder = new()
                        {
                            Mode = BindingMode.OneWay,
                            Path = new PropertyPath(Request.nbtType + "Visibility")
                        };
                        displayText.SetBinding(UIElement.VisibilityProperty,visibilityBinder);
                        floatGrid.SetBinding(UIElement.VisibilityProperty,visibilityBinder);
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
                        #region 分析是否需要代入导入的数据
                        if(ImportMode)
                        {
                            string key = Request.key;
                            if (Give)
                                key = "EntityTag." + key;
                            JToken currentToken = ExternallyReadEntityData.SelectToken(key);
                            if(currentToken != null)
                            {
                                JArray floatArray = JArray.Parse(currentToken.ToString());
                                float number0 = float.Parse(floatArray[0].ToString());
                                float number1 = float.Parse(floatArray[1].ToString());
                                bool Finish0 = false;
                                bool Finish1 = false;
                                for (int i = 0; i < floatGrid.Children.Count; i++)
                                {
                                    if (floatGrid.Children[i] is Slider)
                                    {
                                        Slider currentObj = floatGrid.Children[i] as Slider;
                                        if (floatGrid.Children[i].Uid == "0")
                                        {
                                            currentObj.Value = number0;
                                            Finish0 = true;
                                        }
                                        if (floatGrid.Children[i].Uid == "1")
                                        {
                                            currentObj.Value = number1;
                                            Finish1 = true;
                                        }
                                    }

                                    if (Finish0 && Finish1)
                                        break;
                                }
                            }
                            floatGrid.Focus();
                        }
                        #endregion
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
                        Binding visibilityBinder = new()
                        {
                            Mode = BindingMode.OneWay,
                            Path = new PropertyPath(Request.nbtType + "Visibility")
                        };
                        displayText.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
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

                        if (Request.dataType == "TAG_Pos" || Request.dataType == "TAG_UUID")
                        {
                            UUIDOrPosGroup uUIDOrPosGroup = new()
                            {
                                Uid = Request.nbtType,
                                Name = Request.key,
                                IsUUID = Request.dataType == "TAG_UUID",
                                Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                            };
                            result.Add(uUIDOrPosGroup);
                            uUIDOrPosGroup.SetBinding(UIElement.VisibilityProperty,visibilityBinder);
                            uUIDOrPosGroup.GotFocus += componentEvents.ValueChangedHandler;
                            #region 分析是否需要代入导入的数据
                            if (ImportMode)
                            {
                                string key = Request.key;
                                if (Give)
                                    key = "EntityTag." + key;
                                JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                                if(currentObj != null)
                                {
                                    JArray dataArray = JArray.Parse(currentObj.ToString());
                                    uUIDOrPosGroup.number0.Value = int.Parse(dataArray[0].ToString());
                                    uUIDOrPosGroup.number1.Value = int.Parse(dataArray[1].ToString());
                                    uUIDOrPosGroup.number2.Value = int.Parse(dataArray[2].ToString());
                                    uUIDOrPosGroup.number3.Value = int.Parse(dataArray[3].ToString());
                                }
                                uUIDOrPosGroup.Focus();
                            }
                            #endregion
                        }
                        else
                        {
                            Slider numberBox1 = new()
                            {
                                Name = Request.key,
                                Uid = Request.nbtType,
                                Minimum = minValue,
                                Maximum = maxValue,
                                Value = 0,
                                Style = Application.Current.Resources["NumberBoxStyle"] as Style,
                                Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                            };
                            numberBox1.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                            numberBox1.GotFocus += componentEvents.ValueChangedHandler;
                            result.Add(numberBox1);
                            numberBox1.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                            #region 分析是否需要代入导入的数据
                            if (ImportMode)
                            {
                                string key = Request.key;
                                if (Give)
                                    key = "EntityTag." + key;
                                JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                                if(currentObj != null)
                                {
                                    numberBox1.Value = int.Parse(currentObj.ToString());
                                }
                            }
                            #endregion
                        }
                    }
                    break;
                case "TAG_String_List":
                case "TAG_String":
                case "TAG_Long":
                    {
                        Binding visibilityBinder = new()
                        {
                            Mode = BindingMode.OneWay,
                            Path = new PropertyPath(Request.nbtType + "Visibility")
                        };
                        TextBox stringBox = new() { BorderBrush = blackBrush, Foreground = whiteBrush, Uid = Request.nbtType, Name = Request.key,Tag = new NBTDataStructure() { Result = "",Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                        displayText.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                        stringBox.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                        stringBox.GotFocus += componentEvents.ValueChangedHandler;
                        if(Request.dataType == "TAG_String_List")
                        {
                            string NewToolTip = Request.toolTip + "(以,分割成员,请遵守NBT语法)";
                            displayText.ToolTip = NewToolTip;
                        }
                        result.Add(stringBox);
                        #region 分析是否需要代入导入的数据
                        if(ImportMode)
                        {
                            string key = Request.key;
                            if (Give)
                                key = "EntityTag." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if(currentObj != null)
                            {
                                stringBox.Text = currentObj.ToString().Replace("\"","");
                            }
                            stringBox.Focus();
                        }
                        #endregion
                    }
                    break;
                case "TAG_StringReference":
                    {
                        Binding visibilityBinder = new()
                        {
                            Mode = BindingMode.OneWay,
                            Path = new PropertyPath(Request.nbtType + "Visibility")
                        };
                        Grid grid = new() { Uid = Request.nbtType, Name = Request.key, Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType } };
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                        TextBox stringBox = new() { IsReadOnly = true,CaretBrush = whiteBrush, BorderBrush = blackBrush, Foreground = whiteBrush };
                        IconTextButtons textButtons = new()
                        {
                            Style = Application.Current.Resources["IconTextButton"] as Style,
                            Background = buttonNormalBrush,
                            PressedBackground = buttonPressedBrush,
                            Content = "设置引用",
                            Padding = new Thickness(5,2,5,2)
                        };
                        grid.Children.Add(stringBox);
                        grid.Children.Add(textButtons);
                        Grid.SetColumn(stringBox, 0);
                        Grid.SetColumn(textButtons, 1);
                        displayText.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                        grid.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                        grid.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(grid);
                    }
                    break;
                case "TAG_Boolean":
                    {
                        Binding visibilityBinder = new()
                        {
                            Mode = BindingMode.OneWay,
                            Path = new PropertyPath(Request.nbtType + "Visibility")
                        };
                        TextCheckBoxs textCheckBoxs = new()
                        {
                            Uid = Request.nbtType,
                            Name = Request.key,
                            Foreground = whiteBrush,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            HeaderWidth = 20,
                            HeaderHeight = 20,
                            Style = Application.Current.Resources["TextCheckBox"] as Style,
                            Content = Request.description,
                            Tag = new NBTDataStructure() { Result = "",Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                        };
                        displayText.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                        textCheckBoxs.SetBinding(UIElement.VisibilityProperty, visibilityBinder);
                        Grid.SetColumnSpan(textCheckBoxs,2);
                        if (Request.toolTip.Length > 0)
                        {
                            textCheckBoxs.ToolTip = Request.toolTip;
                            ToolTipService.SetBetweenShowDelay(textCheckBoxs,0);
                            ToolTipService.SetInitialShowDelay(textCheckBoxs, 0);
                        }
                        result.Remove(displayText);
                        textCheckBoxs.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(textCheckBoxs);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Give)
                                key = "EntityTag." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if(currentObj != null)
                                textCheckBoxs.IsChecked = currentObj.ToString() == "1" || currentObj.ToString() == "true";
                            textCheckBoxs.Focus();
                        }
                        #endregion
                    }
                    break;
                case "TAG_Enum":
                    {
                        MatchCollection matchCollection = Regex.Matches(Request.toolTip,@"[a-zA-Z_]+");
                        List<string> enumValueList = matchCollection.ToList().ConvertAll(item=>item.ToString());
                        ComboBox comboBox = new()
                        {
                            ItemsSource = enumValueList,
                            Uid = Request.nbtType,
                            Foreground = whiteBrush,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Style = Application.Current.Resources["TextComboBoxStyle"] as Style,
                            Name = Request.key,
                            SelectedIndex = 0,
                            Tag = new NBTDataStructure() { Result = "", Visibility = Visibility.Collapsed, DataType = Request.dataType, NBTGroup = Request.nbtType }
                        };
                        comboBox.GotFocus += componentEvents.ValueChangedHandler;
                        result.Add(comboBox);
                        #region 分析是否需要代入导入的数据
                        if (ImportMode)
                        {
                            string key = Request.key;
                            if (Give)
                                key = "EntityTag." + key;
                            JToken currentObj = ExternallyReadEntityData.SelectToken(key);
                            if (currentObj != null)
                            {
                                comboBox.SelectedIndex = enumValueList.IndexOf(currentObj.ToString());
                            }
                            comboBox.Focus();
                        }
                        #endregion
                    }
                    break;
            }

            #region 删除已读取的键
            if (ImportMode)
                ExternallyReadEntityData.Remove(Request.key);
            if(ExternallyReadEntityData != null)
            {
                string RemainData = ExternallyReadEntityData.ToString();
                if (RemainData == "[]" || RemainData == "{}")
                {
                    ExternallyReadEntityData = null;
                    ImportMode = false;
                }
            }
            #endregion

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
            TabControl tabControl = sender as TabControl;
            SpecialViewer = (tabControl.Items[0] as TextTabItems).Content as ScrollViewer;
            if(EntityIds.Count > 0)
            SelectedEntityId ??= EntityIds.First();
            UpdateUILayOut();
        }

        /// <summary>
        /// 根据当前切换的实体ID来动态切换UI元素的显隐
        /// </summary>
        public void UpdateUILayOut()
        {
            string data = File.ReadAllText(SpecialNBTStructureFilePath);
            JArray array = JArray.Parse(data);

            #region 搜索当前实体ID对应的JSON对象
            List<JToken> targetList = array.Where(item =>
            {
                JObject currentObj = item as JObject;
                if (currentObj["type"].ToString() == SelectedEntityIdString)
                    return true;
                return false;
            }).ToList();
            #endregion

            if (targetList.Count > 0)
            {
                JObject targetObj = targetList.First() as JObject;
                JArray commonTags = JArray.Parse(targetObj["common"].ToString());
                List<string> commonTagList = commonTags.ToList().ConvertAll(item => item.ToString());
                //计算本次与上次共通标签的差集,关闭指定菜单，而不是全部关闭再依次判断打开
                List<string> closedCommonTagList = specialEntityCommonTagList.Except(commonTagList).ToList();
                #region 处理特指NBT
                if (!specialDataDictionary.ContainsKey(SelectedEntityIdString))
                {
                    JArray children = JArray.Parse(targetObj["children"].ToString());
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
                        if (LeftIndex || newGrid.RowDefinitions.Count == 0)
                            newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                        newGrid.Children.Add(components[j]);
                        if (components[j] is Accordion || components[j] is TextCheckBoxs || components[j] is SuspectsEntities || components[j] is VibrationMonitors)
                        {
                            Grid.SetRow(components[j], newGrid.RowDefinitions.Count - 1);
                            Grid.SetColumn(components[j], 0);
                            Grid.SetColumnSpan(components[j], 2);
                            LeftIndex = true;
                        }
                        else
                        {
                            Grid.SetRow(components[j], newGrid.RowDefinitions.Count - 1);
                            Grid.SetColumn(components[j], LeftIndex ? 0 : 1);
                            LeftIndex = !LeftIndex;
                        }
                    }
                    #endregion

                    if (!specialDataDictionary.ContainsKey(SelectedEntityIdString))
                        specialDataDictionary.Add(SelectedEntityIdString, newGrid);
                    SpecialViewer.Content = newGrid;
                }
                else
                {
                    Grid cacheGrid = specialDataDictionary[SelectedEntityIdString];
                    SpecialViewer.Content = cacheGrid;
                }
                #endregion
                #region 处理额外字段与共通标签的显示隐藏
                Type currentClassType = GetType();
                PropertyInfo[] propertyInfos = currentClassType.GetProperties();
                #region 需要隐藏的共通标签
                TabControl tabControl = (SpecialViewer.Parent as TextTabItems).Parent as TabControl;
                foreach (var item in closedCommonTagList)
                {
                    TextTabItems textTabItem = tabControl.FindChild<TextTabItems>(item);
                    if (textTabItem != null)
                        textTabItem.Visibility = Visibility.Collapsed;
                    //PropertyInfo visibilityPropertyInfo = currentClassType.GetProperty(item + "Visibility");
                    //if(visibilityPropertyInfo != null)
                    //{
                    //    object visibility = Convert.ChangeType(Visibility.Collapsed, visibilityPropertyInfo.PropertyType);
                    //    currentClassType.GetProperty(item + "Visibility")?.SetValue(this, visibility, null);
                    //}
                    //PropertyInfo enabledPropertyInfo = currentClassType.GetProperty(item + "Enabled");
                    //if(enabledPropertyInfo != null)
                    //{
                    //    object enable = Convert.ChangeType(false, enabledPropertyInfo.PropertyType);
                    //    currentClassType.GetProperty(item + "Enabled")?.SetValue(this, enable, null);
                    //}
                }
                if(closedCommonTagList.Count == 0)
                {
                    for (int i = 6; i < tabControl.Items.Count; i++)
                        (tabControl.Items[i] as FrameworkElement).Visibility = Visibility.Collapsed;
                }
                #endregion
                #region 需要显示的共通标签
                foreach (var item in commonTagList)
                {
                    TextTabItems textTabItem = tabControl.FindChild<TextTabItems>(item);
                    if(textTabItem != null)
                        textTabItem.Visibility = Visibility.Visible;
                    //PropertyInfo visibilityPropertyInfo = currentClassType.GetProperty(item + "Visibility");
                    //if (visibilityPropertyInfo != null)
                    //{
                    //    object visibility = Convert.ChangeType(Visibility.Visible, visibilityPropertyInfo.PropertyType);
                    //    currentClassType.GetProperty(item + "Visibility")?.SetValue(this, visibility, null);
                    //}
                    //PropertyInfo enabledPropertyInfo = currentClassType.GetProperty(item + "Enabled");
                    //if (enabledPropertyInfo != null)
                    //{
                    //    object enable = Convert.ChangeType(true, enabledPropertyInfo.PropertyType);
                    //    currentClassType.GetProperty(item + "Enabled")?.SetValue(this, enable, null);
                    //}
                }
                #endregion
                #region 同步本次计算后的特殊实体共通标签链表
                if (specialEntityCommonTagList.Count > 0)
                    specialEntityCommonTagList.Clear();
                specialEntityCommonTagList.AddRange(commonTagList);
                #endregion
                #endregion
            }
        }

        /// <summary>
        /// 切换特指或共通标签顶级页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void TagsTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await GeneratorSpecifyTags(sender as TabControl);
        }

        private async Task GeneratorSpecifyTags(TabControl tabControl)
        {
            if (tabControl.SelectedValue is not TextTabItems textTabItem) return;
            string currentUID = textTabItem.Uid;
            ScrollViewer tabContent = textTabItem.Content as ScrollViewer;
            Grid subGrid = tabContent.Content as Grid;
            if (subGrid.Children.Count > 0) return;
            Grid newGrid = new();
            newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //分辨当前是实体、生物还是活体,都不是则为其它共通标签
            bool IsSubContainer = tabControl.SelectedIndex > 5;
            //检查实体、活体、生物三大共通标签是否都被添加
            List<string> AddedCommonTags = new();
            for (int i = 0; i < subGrid.Children.Count; i++)
            {
                FrameworkElement frameworkElement = subGrid.Children[i] as FrameworkElement;
                NBTDataStructure dataStructure = frameworkElement.Tag as NBTDataStructure;
                if (dataStructure != null && !AddedCommonTags.Contains(dataStructure.NBTGroup))
                    AddedCommonTags.Add(dataStructure.NBTGroup);
            }
            if ((currentUID == "string" || currentUID == "number" || currentUID == "boolean" || currentUID == "list" || currentUID == "compound") && AddedCommonTags.Count >= 3) return;

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
            var sortOrder = new List<string> { "EntityCommonTags", "LivingBodyCommonTags", "MobCommonTags" };
            foreach (string commonString in commonTagList)
            {
                if (IsSubContainer && commonString != currentUID) continue;
                string commonFilePath = NBTStructureFolderPath + commonString + ".json";
                string commonContent = File.ReadAllText(commonFilePath);
                JArray commonArray = JArray.Parse(commonContent);
                foreach (JObject commonItem in commonArray.Cast<JObject>())
                {
                    //判断数据类型,筛选对应的网格容器
                    string dataType = JArray.Parse(commonItem["tag"].ToString())[0].ToString();
                    string numberType = dataType.ToLower().Replace("tag_", "");
                    bool IsNumber = numberType == "pos" || numberType == "float_array" || numberType == "uuid" || numberType == "float" || numberType == "short" || numberType == "byte" || numberType == "int" || numberType == "long" || numberType == "double";
                    IsNumber = IsNumber && currentUID == "number";
                    string currentComponentType = dataType.ToLower().Replace("tag_", "");
                    if (((currentComponentType == currentUID || currentComponentType == (currentUID + "_list") || IsNumber) && tabControl.SelectedIndex <= 5) || tabControl.SelectedIndex > 5)
                    {
                        List<FrameworkElement> result = JsonToComponentConverter(commonItem, commonString);
                        result.Sort((x, y) => sortOrder.IndexOf(x.Uid).CompareTo(sortOrder.IndexOf(y.Uid)));
                        components.AddRange(result);
                    }
                }
            }
            #endregion
            #region 应用控件集合
            bool LeftIndex = true;
            await textTabItem.Dispatcher.InvokeAsync(() =>
            {
                foreach (FrameworkElement item in components)
                {
                    if (LeftIndex || subGrid.RowDefinitions.Count == 0)
                        subGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    subGrid.Children.Add(item);
                    if (item is Accordion || item is TextCheckBoxs || item is SuspectsEntities || item is VibrationMonitors)
                    {
                        Grid.SetRow(item, subGrid.RowDefinitions.Count - 1);
                        Grid.SetColumn(item, 0);
                        Grid.SetColumnSpan(item, 2);
                        LeftIndex = true;
                    }
                    else
                    {
                        Grid.SetRow(item, subGrid.RowDefinitions.Count - 1);
                        Grid.SetColumn(item, LeftIndex ? 0 : 1);
                        LeftIndex = !LeftIndex;
                    }
                }
                tabContent.Content = subGrid;
            });
            #endregion
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
            ScrollViewer tabContent = accordion.Content as ScrollViewer;
            Grid subGrid = tabContent.Content as Grid;
            Accordion parentAccordion = accordion.FindParent<Accordion>();
            bool IsSubContainer = false;
            //分辨当前是实体、生物还是活体,都不是则为其它共通标签
            IsSubContainer = parentAccordion != null;
            //检查实体、活体、生物三大共通标签是否都被添加
            List<string> AddedCommonTags = new();
            for (int i = 0; i < subGrid.Children.Count; i++)
            {
                FrameworkElement frameworkElement = subGrid.Children[i] as FrameworkElement;
                NBTDataStructure dataStructure = frameworkElement.Tag as NBTDataStructure;
                if(dataStructure != null && !AddedCommonTags.Contains(dataStructure.NBTGroup))
                AddedCommonTags.Add(dataStructure.NBTGroup);
            }
            if ((parentAccordion == null && AddedCommonTags.Count >= 3) || (parentAccordion != null && AddedCommonTags.Count > 0)) return;

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
            var sortOrder = new List<string> { "EntityCommonTags", "LivingBodyCommonTags", "MobCommonTags" };
            foreach (string commonString in commonTagList)
            {
                if ((IsSubContainer && commonString != parentAccordion.Uid) || AddedCommonTags.Contains(commonString)) continue;
                string commonFilePath = NBTStructureFolderPath + commonString + ".json";
                string commonContent = File.ReadAllText(commonFilePath);
                JArray commonArray = JArray.Parse(commonContent);
                foreach (JObject commonItem in commonArray.Cast<JObject>())
                {
                    //判断数据类型,筛选对应的网格容器
                    string dataType = JArray.Parse(commonItem["tag"].ToString())[0].ToString();
                    string numberType = dataType.ToLower().Replace("tag_", "");
                    bool IsNumber = numberType == "pos" || numberType == "float_array" || numberType == "uuid" || numberType == "float" || numberType == "short" || numberType == "byte" || numberType == "int" || numberType == "long" || numberType == "double";
                    IsNumber = IsNumber && currentUID == "number";
                    string currentComponentType = dataType.ToLower().Replace("tag_", "");
                    if (currentComponentType == currentUID || currentComponentType == (currentUID + "_list") || IsNumber)
                    {
                        List<FrameworkElement> result = JsonToComponentConverter(commonItem, commonString);
                        result.Sort((x, y) => sortOrder.IndexOf(x.Uid).CompareTo(sortOrder.IndexOf(y.Uid)));
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
                if (item is Accordion || item is TextCheckBoxs || item is SuspectsEntities || item is VibrationMonitors)
                {
                    Grid.SetRow(item, subGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(item, 0);
                    Grid.SetColumnSpan(item, 2);
                    LeftIndex = true;
                }
                else
                {
                    Grid.SetRow(item, subGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(item, LeftIndex ? 0 : 1);
                    LeftIndex = !LeftIndex;
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
            JToken descriptionObj = nbtStructure["description"];
            string description = descriptionObj != null ?descriptionObj.ToString():"";
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
        /// 反选所有bool型NBT
        /// </summary>
        /// <param name="ele"></param>
        private void ReverseAllBoolNBTsCommand(FrameworkElement ele)
        {
            Accordion accordion = ele as Accordion;
            ScrollViewer scrollViewer = accordion.Content as ScrollViewer;
            Grid grid = scrollViewer.Content as Grid;
            foreach (var item in grid.Children)
            {
                if (item is TextCheckBoxs)
                {
                    TextCheckBoxs textCheckBoxs = item as TextCheckBoxs;
                    textCheckBoxs.Focus();
                    textCheckBoxs.IsChecked = !textCheckBoxs.IsChecked.Value;
                }
            }
        }

        /// <summary>
        /// 全选所有bool型NBT
        /// </summary>
        private void SelectAllBoolNBTsCommand(FrameworkElement ele)
        {
            Accordion accordion = ele as Accordion;
            ScrollViewer scrollViewer = accordion.Content as ScrollViewer;
            Grid grid = scrollViewer.Content as Grid;
            foreach (var item in grid.Children)
            {
                if(item is TextCheckBoxs)
                {
                    TextCheckBoxs textCheckBoxs = item as TextCheckBoxs;
                    textCheckBoxs.Focus();
                    textCheckBoxs.IsChecked = true;
                }
            }
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
    }

    /// <summary>
    /// 动态控件数据结构
    /// </summary>
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

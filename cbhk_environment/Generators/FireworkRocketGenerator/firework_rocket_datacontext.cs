using cbhk_environment.CustomControls;
using cbhk_environment.CustomControls.ColorPickers;
using cbhk_environment.GeneralTools.ScrollViewerHelper;
using cbhk_environment.GenerateResultDisplayer;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace cbhk_environment.Generators.FireworkRocketGenerator
{
    public class firework_rocket_datacontext: ObservableObject
    {

        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
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
                OnPropertyChanged();
            }
        }

        //数据源
        private ObservableCollection<string> VersionSource = new ObservableCollection<string> { "1.12-","1.13+" };
        #endregion

        #region 生成行为
        bool behavior_lock = true;
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
        private bool give = false;
        public bool Give
        {
            get { return give; }
            set
            {
                give = value;
                if (behavior_lock)
                {
                    behavior_lock = false;
                    Summon = !give;
                    behavior_lock = true;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region 轨迹
        private string FireworkTrajectoryString
        {
            get
            {
                string result = (Flicker? "Flicker:1b," : "") + (Trail? "Trail:1b," : "");
                return result;
            }
        }
        #endregion

        #region 闪烁
        public bool Flicker { get; set; } = false;
        #endregion

        #region 拖尾
        public bool Trail { get; set; } = false;
        #endregion

        #region 时长
        private double duration = 1;
        public double Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged();
            }
        }

        private string FireworkDurationString
        {
            get
            {
                string result;
                result = "Flight:"+ Duration + ",";
                return result;
            }
        }
        #endregion

        #region 已飞行刻数
        private double life = 0;
        public double Life
        {
            get
            {
                return life;
            }
            set
            {
                life = value;
                OnPropertyChanged();
            }
        }
        private string LifeString
        {
            get
            {
                string result = Life > 0? "Life:" +Life+",":"";
                return result;
            }
        }
        #endregion

        #region 发射时长
        private double lifeTime = 20;
        public double LifeTime
        {
            get
            {
                return lifeTime;
            }
            set
            {
                lifeTime = value;
                OnPropertyChanged();
            }
        }
        private string LifeTimeString
        {
            get
            {
                string result = LifeTime > 0 ? "LifeTime:" + LifeTime + "," : "";
                return result;
            }
        }
        #endregion

        #region 主颜色
        private string MainColorsString
        {
            get
            {
                string result = "Colors:[I;";
                foreach (Border item in MainColors)
                {
                    result += Convert.ToUInt64(item.Background.ToString().Substring(2),16) + ",";
                }
                result = result.Trim() != "Colors:[I;" ? result.TrimEnd(',') + "]," : "";
                return result;
            }
        }
        #endregion

        #region 备选颜色
        private string FadeColorsString
        {
            get
            {
                string result = "FadeColors:[I;";
                foreach (Border item in FadeColors)
                {
                    result += Convert.ToUInt64(item.Background.ToString().Substring(2),16) + ",";
                }
                result = result.Trim() != "FadeColors:[I;" ? result.TrimEnd(',') + "]," : "";
                return result;
            }
        }
        #endregion

        #region 按角度飞出
        //按角度飞出
        public bool FlyAngle { get; set; }
        private string FlyAngleString
        {
            get
            {
                string result;
                result = FlyAngle ? "ShotAtAngle:true," :"";
                return result;
            }
        }
        #endregion

        #region 清除淡入/淡出
        public RelayCommand<FrameworkElement> ClearMainColor { get; set; }
        public RelayCommand<FrameworkElement> ClearFadeColor { get; set; }
        #endregion

        #region 已选择的形状
        private int selectedShape = 0;
        public int SelectedShape
        {
            get
            {
                return selectedShape;
            }
            set
            {
                selectedShape = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 点/连续(模式切换)
        private bool selectedModeLock = true;
        private bool pointMode = true;
        public bool PointMode
        {
            get
            {
                return pointMode;
            }
            set
            {
                pointMode = value;
                if(selectedModeLock)
                {
                    selectedModeLock = false;
                    ContinuousMode = !pointMode;
                    selectedModeLock = true;
                }
                OnPropertyChanged();
            }
        }

        private bool continuousMode = false;
        public bool ContinuousMode
        {
            get
            {
                return continuousMode;
            }
            set
            {
                continuousMode = value;
                if (selectedModeLock)
                {
                    selectedModeLock = false;
                    PointMode = !continuousMode;
                    selectedModeLock = true;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region 已选择颜色
        private SolidColorBrush selectedColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
        public SolidColorBrush SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
                if(ContinuousMode)
                {
                    Border border = new Border()
                    {
                        Width = 25,
                        Background = selectedColor
                    };
                    border.MouseRightButtonUp += DeleteColorMouseRightButtonUp;
                    if (AddInMain)
                    {
                        border.Uid = "Main";
                        MainColors.Add(border);
                    }
                    else
                    {
                        border.Uid = "Fade";
                        FadeColors.Add(border);
                    }
                }
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// 形状数据源
        /// </summary>
        ObservableCollection<string> shapeList { get; set; } = new ObservableCollection<string> { };

        #region 最终形状数据
        private string FireworkShapeString
        {
            get
            {
                string result = "Type:"+SelectedShape + ",";
                return result;
            }
        }
        #endregion

        #region 主颜色库
        private ObservableCollection<Border> mainColors = new ObservableCollection<Border> { };
        public ObservableCollection<Border> MainColors
        {
            get
            {
                return mainColors;
            }
            set
            {
                mainColors = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 备选颜色库
        private ObservableCollection<Border> fadeColors = new ObservableCollection<Border> { };
        public ObservableCollection<Border> FadeColors
        {
            get
            {
                return fadeColors;
            }
            set
            {
                fadeColors = value;
                OnPropertyChanged();
            }
        }
        #endregion

        //原版颜色映射库
        private Dictionary<string, string> OriginColorDictionary = new Dictionary<string, string> { };

        /// <summary>
        /// 原版颜色库面板
        /// </summary>
        UniformGrid structureColorGrid = null;
        //拾色器
        ColorPickers colorpicker = null;

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconFireworks.png";
        //原版颜色库路径
        string colorStoragePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\FireworkRocket\\images";
        //形状路径
        string shapePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\FireworkRocket\\data\\shapes.ini";
        //颜色映射表路径
        string colorTablePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\FireworkRocket\\data\\structureColorDictionary.ini";

        #region 加入淡入或淡出
        private bool addInMain = true;
        public bool AddInMain
        {
            get
            {
                return addInMain;
            }
            set
            {
                addInMain = value;
                OnPropertyChanged();
            }
        }
        private bool addInFade = false;
        public bool AddInFade
        {
            get
            {
                return addInFade;
            }
            set
            {
                addInFade = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 淡入淡出滚动视图引用
        ScrollViewer mainScrollViewer = null;
        ScrollViewer fadeScrollViewer = null;
        #endregion

        #region 全选和反选原版颜色库
        public RelayCommand<FrameworkElement> SelectedAllStructureColor { get; set; }
        public RelayCommand<FrameworkElement> ReverseAllStructureColor { get; set; }
        #endregion

        #region 滚轮缩放倍率
        double viewScale = 0;
        double ViewScale
        {
            get
            {
                return viewScale;
            }
            set
            {
                viewScale = value;
                if (viewScale < 0.1)
                    viewScale = 0.1;
                if(viewScale > 2)
                    viewScale = 2;
            }
        }
        #endregion

        public firework_rocket_datacontext()
        {
            #region 连接指令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            RunCommand = new RelayCommand(run_command);
            SelectedAllStructureColor = new RelayCommand<FrameworkElement>(SelectedAllStructureColorCommand);
            ReverseAllStructureColor = new RelayCommand<FrameworkElement>(ReverseAllStructureColorCommand);
            ClearMainColor = new RelayCommand<FrameworkElement>(ClearMainColorCommand);
            ClearFadeColor = new RelayCommand<FrameworkElement>(ClearFadeColorCommand);
            #endregion
        }

        private void return_command(CommonWindow win)
        {
            FireworkRocket.cbhk.Topmost = true;
            FireworkRocket.cbhk.WindowState = WindowState.Normal;
            FireworkRocket.cbhk.Show();
            FireworkRocket.cbhk.Topmost = false;
            FireworkRocket.cbhk.ShowInTaskbar = true;
            win.Close();
        }
        private void run_command()
        {
            string result = "";
            if (Give)
            {
                result = "{Explosions:[{";
                result += FireworkShapeString + FireworkTrajectoryString + MainColorsString + FadeColorsString;
                result = result.TrimEnd(',') + "}],";
                result += FireworkDurationString.TrimEnd(',') + "}";
                result = "give @p firework_rocket{Fireworks:" + result + "}";
            }
            else
            if (Summon)
            {
                result = FireworkShapeString + FireworkTrajectoryString + MainColorsString + FadeColorsString;
                result = "FireworksItem:{id:firework_rocket,Count:1b,tag:{Fireworks:{Explosions:[{" + result.TrimEnd(',') + "}],";
                result = "summon firework_rocket ~ ~ ~ {" + LifeTimeString + LifeString + FlyAngleString + result + FireworkDurationString.TrimEnd(',') + "}}}}";
            }
            Displayer displayer = Displayer.GetContentDisplayer();
            displayer.GeneratorResult(result , "烟花火箭", icon_path);
            displayer.Show();
        }

        /// <summary>
        /// 滚轮缩放色谱视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Canvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta < 0)
                    ViewScale -= 0.1;
                else
                    ViewScale += 0.1;
                ScaleTransform scaleTransform = new ScaleTransform
                {
                    ScaleX = ViewScale
                };
                Canvas canvas = sender as Canvas;
                ScrollViewer scrollViewer = canvas.Children[0] as ScrollViewer;
                scrollViewer.RenderTransform = scaleTransform;
            }
        }

        /// <summary>
        /// 左击并抬起拾色器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ColorPickers_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(colorpicker.pop.IsOpen && PointMode)
            {
                if (AddInMain)
                {
                    Border border = new()
                    {
                        Width = 25,
                        Background = colorpicker.SelectColor
                    };
                    border.MouseRightButtonUp += DeleteColorMouseRightButtonUp;
                    border.Uid = "Main";
                    MainColors.Add(border);
                }
                if(AddInFade)
                {
                    Border border = new()
                    {
                        Width = 25,
                        Background = colorpicker.SelectColor
                    };
                    border.MouseRightButtonUp += DeleteColorMouseRightButtonUp;
                    border.Uid = "Fade";
                    FadeColors.Add(border);
                }
            }
        }

        /// <summary>
        /// 清空淡出颜色
        /// </summary>
        private void ClearFadeColorCommand(FrameworkElement obj)
        {
            FadeColors.Clear();
        }

        /// <summary>
        /// 清空淡入颜色
        /// </summary>
        private void ClearMainColorCommand(FrameworkElement obj)
        {
            MainColors.Clear();
        }

        /// <summary>
        /// 反选所有结构色
        /// </summary>
        private void ReverseAllStructureColorCommand(FrameworkElement obj)
        {
            foreach (var item in structureColorGrid.Children)
            {
                IconCheckBoxs iconCheckBoxs = item as IconCheckBoxs;
                iconCheckBoxs.IsChecked = !iconCheckBoxs.IsChecked.Value;
            }
        }

        /// <summary>
        /// 全选所有结构色
        /// </summary>
        private void SelectedAllStructureColorCommand(FrameworkElement obj)
        {
            foreach (var item in structureColorGrid.Children)
            {
                IconCheckBoxs iconCheckBoxs = item as IconCheckBoxs;
                iconCheckBoxs.IsChecked = true;
            }
        }

        /// <summary>
        /// 载入原版颜色库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StructureColorList_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            structureColorGrid = scrollViewer.Content as UniformGrid;

            string[] colorArray = Directory.GetFiles(colorStoragePath);
            string[] colorTable = File.ReadAllLines(colorTablePath);
            foreach (var item in colorArray)
            {
                if(item.Contains("dye"))
                {
                    string colorName = Path.GetFileNameWithoutExtension(item);
                    colorName = colorName.Substring(0, colorName.LastIndexOf('_'));
                    System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(item,UriKind.Absolute));
                    IconCheckBoxs iconCheckBoxs = new IconCheckBoxs
                    {
                        ContentImage = bitmapImage,
                        HeaderHeight = 25,
                        HeaderWidth = 25,
                        SnapsToDevicePixels = true,
                        UseLayoutRounding = true,
                        ToolTip = colorName,
                        Tag = colorName,
                        Style = Application.Current.Resources["IconCheckBox"] as Style
                    };
                    iconCheckBoxs.Checked += StructureColorChecked;
                    RenderOptions.SetBitmapScalingMode(iconCheckBoxs,BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetClearTypeHint(iconCheckBoxs,ClearTypeHint.Enabled);
                    ToolTipService.SetShowDuration(iconCheckBoxs,1000);
                    ToolTipService.SetInitialShowDelay(iconCheckBoxs,0);
                    structureColorGrid.Children.Add(iconCheckBoxs);
                }
            }

            string colorID = "";
            string colorString = "";
            foreach (var item in colorTable)
            {
                colorID = item.Split(':')[0];
                colorString = item.Split(':')[1];
                OriginColorDictionary.Add(colorID,colorString);
            }
        }

        /// <summary>
        /// 已选择结构色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StructureColorChecked(object sender, RoutedEventArgs e)
        {
            IconCheckBoxs iconCheckBoxs = sender as IconCheckBoxs;
            string searchTarget = iconCheckBoxs.Tag.ToString();
            string colorValue = OriginColorDictionary.Where(item => item.Value == searchTarget).Select(item => item.Key).First();
            Border border = new Border()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorValue)),
                Width = 25,
            };
            border.MouseRightButtonUp += DeleteColorMouseRightButtonUp;
            if (AddInMain)
            {
                border.Uid = "Main";
                MainColors.Add(border);
            }
            else
            {
                border.Uid = "Fade";
                FadeColors.Add(border);
            }

        }

        /// <summary>
        /// 淡入颜色视图滚动到最右侧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainColorItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainScrollViewer.ScrollToHorizontalOffset(mainScrollViewer.ExtentWidth);
        }

        /// <summary>
        /// 淡出颜色视图滚动到最右侧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FadeColorItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            fadeScrollViewer.ScrollToHorizontalOffset(fadeScrollViewer.ExtentWidth);
        }

        /// <summary>
        /// 右击删除指定颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteColorMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border.Uid == "Main")
                MainColors.Remove(border);
            else
                FadeColors.Remove(border);
        }

        /// <summary>
        /// 载入淡入颜色滚动视图引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainColorGridScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            mainScrollViewer = sender as ScrollViewer;
        }

        /// <summary>
        /// 载入淡出颜色滚动视图引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FadeColorGridScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            fadeScrollViewer = sender as ScrollViewer;
        }

        /// <summary>
        /// 为拾色器的矩形选择面板订阅左击抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ColorPickerLoaded(object sender,RoutedEventArgs e)
        {
            colorpicker = sender as ColorPickers;
            colorpicker.rectColorGrid.PreviewMouseLeftButtonUp += ColorPickers_PreviewMouseLeftButtonUp;
        }

        /// <summary>
        /// 载入版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VersionLoaded(object sender,RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.ItemsSource = VersionSource;
        }

        /// <summary>
        /// 载入形状
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShapeLoaded(object sender,RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if(File.Exists(shapePath))
            {
                string[] shapes = File.ReadAllLines(shapePath);
                foreach (string shape in shapes)
                {
                    shapeList.Add(shape.Substring(shape.LastIndexOf(':') + 1));
                }
                comboBox.ItemsSource = shapeList;
            }
        }

        /// <summary>
        /// 处理图片
        /// </summary>
        //private void HandleImage(int effectWidth)
        //{
        //    if (map == null) return;
        //    this.mainCanvas.Children.Clear();
        //    List<Particle> particleList = MosaicHelper.AdjustTobMosaic(map, effectWidth);
        //    foreach (var pd in particleList)
        //    {
        //        var ep = new Ellipse
        //        {
        //            Width = pd.Size,
        //            Height = pd.Size,
        //            Fill = new SolidColorBrush(Colors.Black),
        //        };
        //        Canvas.SetTop(ep, pd.Position.Y);
        //        Canvas.SetLeft(ep, pd.Position.X);
        //        this.mainCanvas.Children.Add(ep);
        //    }
        //}
    }
}

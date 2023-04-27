using cbhk_environment.ControlsDataContexts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.IO;
using cbhk_environment.CustomControls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    public class BlockStateDataContext:ObservableObject
    {
        #region 添加与清空属性
        public RelayCommand<FrameworkElement> AddAttribute { get; set; }
        public RelayCommand<FrameworkElement> ClearAttribute { get; set; }
        #endregion

        #region 字段
        private static string blockStateFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\blockStates.json";
        private ImageBrush buttonNormal = new(new BitmapImage(new Uri("pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonNormal.png",UriKind.RelativeOrAbsolute)));
        private ImageBrush buttonPressed = new(new BitmapImage(new Uri("pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonPressed.png", UriKind.RelativeOrAbsolute)));
        private static string blockStateContent = "";
        private static JArray blockStateArray = null;
        #endregion

        #region 数据源
        /// <summary>
        /// 存储方块的所有属性
        /// </summary>
        public ObservableCollection<string> BlockAttributes { get; set; } = new();
        /// <summary>
        /// 存储方块属性对应的键与值集合
        /// </summary>
        public ObservableCollection<Dictionary<string, List<string>>> AttributeKeyValuePairs { get; set; } = new();
        /// <summary>
        /// 存储方块的所有属性键
        /// </summary>
        public ObservableCollection<string> AttributeKeys { get; set; } = new();
        /// <summary>
        /// 方块列表
        /// </summary>
        public ObservableCollection<IconComboBoxItem> BlockList { get; set; } = MainWindow.BlockIDSource;
        /// <summary>
        /// 已选中的方块键集合
        /// </summary>
        public ObservableCollection<string> SelectedAttributeKeys { get; set; } = new();
        /// <summary>
        /// 已选中的方块值集合
        /// </summary>
        public ObservableCollection<string> SelectedAttributeValues { get; set; } = new();

        #region 选中的方块ID
        private IconComboBoxItem selectedBlockID = null;
        public IconComboBoxItem SelectedBlockID
        {
            get
            {
                return selectedBlockID;
            }
            set
            {
                selectedBlockID = value;
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// 方块状态缓存,用于存储编辑过的方块数据
        /// </summary>
        //private Dictionary<string,Grid> BlockStateCache { get; set; } = new();

        /// <summary>
        /// 属性视图
        /// </summary>
        //private ScrollViewer scrollViewer = null;

        public string SelectedBlockIDString
        {
            get
            {
                string currentValue = MainWindow.BlockDataBase.Where(item => item.Key[(item.Key.IndexOf(':') + 1)..] == SelectedBlockID.ComboBoxItemText).Select(item => item.Key).First();
                currentValue = currentValue[..currentValue.IndexOf(':')];
                return currentValue;
            }
        }
        /// <summary>
        /// 选中的方块属性
        /// </summary>
        public string SelectedBlockAttribute { get; set; }
        /// <summary>
        /// 选中的方块属性值
        /// </summary>
        public string SelectedAttributeValue { get; set; }
        #endregion

        public BlockStateDataContext()
        {
            AddAttribute = new RelayCommand<FrameworkElement>(AddAttributeCommand);
            ClearAttribute = new RelayCommand<FrameworkElement>(ClearAttributeCommand);
            blockStateContent = File.ReadAllText(blockStateFilePath);
            blockStateArray = JArray.Parse(blockStateContent);
            SelectedBlockID ??= BlockList[0];
            UpdateBlockAttributes();
        }

        /// <summary>
        /// 属性视图载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            //scrollViewer = sender as ScrollViewer;
        }

        /// <summary>
        /// 更新方块属性数据源
        /// </summary>
        private void UpdateBlockAttributes()
        {
            if (SelectedBlockID != null)
            {
                AttributeKeyValuePairs.Clear();
                AttributeKeys.Clear();
                string currentBlockID = SelectedBlockIDString;

                #region 解析方块状态JSON文件,找到对应的属性集合
                AttributeKeyValuePairs.Add(new Dictionary<string, List<string>>());
                for (int i = 0; i < blockStateArray.Count; i++)
                {
                    List<string> blocks = JArray.Parse(blockStateArray[i]["blocks"].ToString()).ToList().ConvertAll(item => item.ToString());
                    if (blocks.Contains(currentBlockID))
                    {
                        string key = blockStateArray[i]["key"].ToString();
                        AttributeKeys.Add(key);
                        AttributeKeyValuePairs[^1].Add(key,new List<string>());
                        List<string> values = JArray.Parse(blockStateArray[i]["allow"].ToString()).ToList().ConvertAll(item => item.ToString());
                        _ = values.All(item =>
                        {
                            AttributeKeyValuePairs[^1][key].Add(item);
                            return true;
                        });
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 更新方块属性值数据源
        /// </summary>
        private void UpdateBlockAttributeValues(ComboBox comboBox)
        {
            Grid grid = comboBox.Parent as Grid;
            if (grid == null) return;
            int rowIndex = Grid.GetRow(comboBox);
            ComboBox valueBox = grid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == rowIndex && Grid.GetColumn(e) == 1) as ComboBox;
            if (AttributeKeys.Count == 0)
            {
                grid.Children.Clear();
                valueBox.ItemsSource = null;
                valueBox.Items.Clear();
                return;
            }
            List<string> currentValueList = AttributeKeyValuePairs[^1][SelectedAttributeKeys[rowIndex].ToString()];
            valueBox.ItemsSource = currentValueList;
            valueBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 清空属性
        /// </summary>
        private void ClearAttributeCommand(FrameworkElement ele)
        {
            Accordion accordion = ele as Accordion;
            Grid grid = (accordion.Content as ScrollViewer).Content as Grid;
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            SelectedAttributeKeys.Clear();
            SelectedAttributeValues.Clear();
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        public void AddAttributeCommand(FrameworkElement ele)
        {
            if (AttributeKeyValuePairs.Count == 0) return;
            Accordion accordion = ele as Accordion;
            ScrollViewer scrollViewer = accordion.Content as ScrollViewer;
            Grid grid = scrollViewer.Content as Grid;
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            int currentIndex = grid.RowDefinitions.Count - 1;
            Binding keyBinder = new()
            {
                Path = new PropertyPath("SelectedAttributeKeys[" + currentIndex + "]"),
                Mode = BindingMode.TwoWay
            };
            Binding valueSelectedBinder = new()
            {
                Path = new PropertyPath("SelectedAttributeValues[" + currentIndex + "]"),
                Mode = BindingMode.TwoWay
            };
            ComboBox AttributeKey = new()
            {
                Style = Application.Current.Resources["TextComboBoxStyle"] as Style,
                Uid = currentIndex.ToString(),
                Name = "key",
                DataContext = this,
                ItemsSource = AttributeKeys,
                Height = 25
            };
            SelectedAttributeKeys.Add("");
            AttributeKey.SetBinding(Selector.SelectedItemProperty, keyBinder);
            AttributeKey.SelectionChanged += AttributeKey_SelectionChanged;
            AttributeKey.SelectedIndex = 0;
            ComboBox AttributeValue = new()
            {
                Style = Application.Current.Resources["TextComboBoxStyle"] as Style,
                Uid = currentIndex.ToString(),
                Name = "value",
                DataContext = this,
                ItemsSource = AttributeKeyValuePairs[^1][AttributeKeys[0]],
                Height = 25
            };
            IconTextButtons deleteButton = new()
            {
                Style = Application.Current.Resources["IconTextButton"] as Style,
                Background = buttonNormal,
                Width = 25,
                Content = "X",
                PressedBackground = buttonPressed
            };
            SelectedAttributeValues.Add("");
            AttributeValue.SetBinding(Selector.SelectedItemProperty, valueSelectedBinder);
            deleteButton.Click += (a, b) =>
            {
                grid.Children.Remove(AttributeKey);
                grid.Children.Remove(AttributeValue);
                grid.Children.Remove(a as UIElement);
            };
            grid.Children.Add(AttributeKey);
            grid.Children.Add(AttributeValue);
            grid.Children.Add(deleteButton);
            Grid.SetRow(AttributeKey,grid.RowDefinitions.Count - 1);
            Grid.SetRow(AttributeValue, grid.RowDefinitions.Count - 1);
            Grid.SetRow(deleteButton, grid.RowDefinitions.Count - 1);
            Grid.SetColumn(AttributeKey,0);
            Grid.SetColumn(AttributeValue,1);
            Grid.SetColumn(deleteButton, 2);
            AttributeValue.SelectedIndex = 0;
        }

        /// <summary>
        /// 属性键切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void AttributeKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBlockAttributeValues(sender as ComboBox);
        }

        /// <summary>
        /// 方块ID切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BlockID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBlockAttributes();
        }
    }
}

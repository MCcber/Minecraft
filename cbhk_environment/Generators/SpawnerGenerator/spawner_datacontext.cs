using cbhk_environment.CustomControls;
using cbhk_environment.GenerateResultDisplayer;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.SpawnerGenerator
{
    public class spawner_datacontext: ObservableObject
    {
        /// <summary>
        /// 刷怪笼配置文件路径
        /// </summary>
        string treeViewStructureFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Spawner\\data\\structure.json";
        /// <summary>
        /// 刷怪笼树结构数据源
        /// </summary>
        public ObservableCollection<RichTreeViewItems> SpawnerStructureItems { get; set; } = new ObservableCollection<RichTreeViewItems>();

        /// <summary>
        /// 普通文本块的灰刷
        /// </summary>
        SolidColorBrush grayBrush = new((Color)ColorConverter.ConvertFromString("#838383"));

        #region 复合结构颜色表
        string[] CompoundColorArray = new string[] { "#1B1B1B", "#386330" };
        int CompoundColorIndex = 0;
        SolidColorBrush CompoundColorBrush = new();
        #endregion

        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        #endregion

        #region 用于Tag转换到控件的绑定
        Binding controllerBinder = new()
        {
            Path = new PropertyPath("Tag"),
            Mode = BindingMode.OneTime,
            RelativeSource = new RelativeSource(RelativeSourceMode.Self),
            Converter = new TagToHeader()
        };
        #endregion

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconSpawner.png";

        public spawner_datacontext()
        {
            #region 绑定指令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            RunCommand = new RelayCommand(run_command);
            #endregion

            #region 初始化刷怪笼树结构
            string spawnerStructure = File.ReadAllText(treeViewStructureFilePath);
            JArray keyValuePairs = JArray.Parse(spawnerStructure);
            foreach (JObject item in keyValuePairs)
            {
                RichTreeViewItems currentItem = new()
                {
                    ConnectingLineFill = grayBrush,
                    Tag = item.ToString(),
                    Style = Application.Current.Resources["RichTreeViewItems"] as Style
                };

                #region 如果类型为Compound
                string currentChildren = item["children"].ToString();
                if(currentChildren.Length > 2)
                {
                    JArray children = JArray.Parse(currentChildren);
                    foreach (JObject obj in children)
                    {
                        MessageBox.Show(obj.ToString());
                        RichTreeViewItems SubItem = new()
                        {
                            ConnectingLineFill = grayBrush,
                            Tag = obj.ToString(),
                            Style = Application.Current.Resources["RichTreeViewItems"] as Style
                        };
                        currentItem.Items.Add(SubItem);
                    }
                    currentItem.Expanded += CompoundItemExpanded;
                    #endregion
                }
                BindingOperations.SetBinding(currentItem, HeaderedItemsControl.HeaderProperty, controllerBinder);
                SpawnerStructureItems.Add(currentItem);
            }
            #endregion
        }

        /// <summary>
        /// 展开复合节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompoundItemExpanded(object sender, RoutedEventArgs e)
        {
            RichTreeViewItems currentItem = sender as RichTreeViewItems;
            object headObj = (currentItem.Items[0] as RichTreeViewItems).Header;
            if(currentItem.Tag != null && headObj == null)
            {
                string rootJSON;
                #region 绑定子节点的Tag
                foreach (RichTreeViewItems subItem in currentItem.Items)
                {
                    rootJSON = subItem.Tag.ToString();
                    JObject rootObj = JObject.Parse(rootJSON);
                    JArray children = JArray.Parse(rootObj["children"].ToString());

                    #region 处理子节点的子节点
                    foreach (JObject obj in children)
                    {
                        RichTreeViewItems subsubItem = new()
                        {
                            ConnectingLineFill = grayBrush,
                            Tag = obj.ToString(),
                            Style = Application.Current.Resources["RichTreeViewItems"] as Style
                        };
                        subItem.Items.Add(subsubItem);
                    }
                    subItem.Expanded += CompoundItemExpanded;
                    #endregion

                    BindingOperations.SetBinding(subItem, HeaderedItemsControl.HeaderProperty, controllerBinder);
                }
                #endregion
            }
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            Spawner.cbhk.Topmost = true;
            Spawner.cbhk.WindowState = WindowState.Normal;
            Spawner.cbhk.Show();
            Spawner.cbhk.Topmost = false;
            Spawner.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 生成结果
        /// </summary>
        private void run_command()
        {
            string result = "/setblock ~ ~ ~ spawner {}";
            Displayer displayer = Displayer.GetContentDisplayer();
            displayer.GeneratorResult(result,"刷怪笼",icon_path);
            displayer.Show();
        }
    }

    public class TagToHeader : IValueConverter
    {
        /// <summary>
        /// 白色刷子
        /// </summary>
        SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        /// <summary>
        /// 黑色刷子
        /// </summary>
        SolidColorBrush blackBrush = new((Color)ColorConverter.ConvertFromString("#000000"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DockPanel dockPanel = new()
            {
                LastChildFill = false
            };

            string currentValue = value.ToString();
            JObject keyValuePairs = JObject.Parse(currentValue);

            #region 字符串转为控件

            #region 判断是否需要生成器按钮
            string childrenData = keyValuePairs["children"].ToString();
            JArray children = JArray.Parse(childrenData);
            IconTextButtons generatorButton = new()
            {
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(5),
                Style = Application.Current.Resources["IconTextButton"] as Style,
                Foreground = blackBrush,
                Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonNormal.png", UriKind.RelativeOrAbsolute))),
                PressedBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonPressed.png", UriKind.RelativeOrAbsolute))),
                FontSize = 12
            };
            #endregion

            string tagList = Regex.Match(keyValuePairs["tag"].ToString().Replace("\r","").Replace("\n",""),@"(?<=\[).*(?=\])").ToString();
            List<string> tags = tagList.Split(',').ToList();

            TextBlock key = new()
            {
               Text = keyValuePairs["key"].ToString(),
               Foreground = whiteBrush,
               FontSize = 12,
               HorizontalAlignment = HorizontalAlignment.Center,
               VerticalAlignment = VerticalAlignment.Center,
               TextAlignment = TextAlignment.Center
            };

            object descriptionObj = keyValuePairs["description"];
            IconTextButtons description = new()
            {
                Margin = new Thickness(5, 0, 0, 0),
                Style = Application.Current.Resources["IconTextButton"] as Style,
                Foreground = whiteBrush,
                Width = 18,
                Height = 18,
                Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/cbhk_environment;component/resources/common/images/Issue.png",UriKind.RelativeOrAbsolute))),
                FontSize = 12
            };
            if (descriptionObj != null)
                description.ToolTip = descriptionObj.ToString();
            ToolTipService.SetInitialShowDelay(description,0);
            ToolTipService.SetShowDuration(description,1000);

            dockPanel.Children.Add(key);

            #region 在key后添加启动生成器的按钮
            if (children.Count == 0)
            {
                object dependencyObj = keyValuePairs["dependency"];
                if (dependencyObj != null && dependencyObj.ToString() == "EntityGenerator")
                {
                    EntityGenerator.Entity entity = new();
                    generatorButton.Content = "实体生成器";
                    generatorButton.ClickMode = ClickMode.Release;
                    generatorButton.Click += (a, b) =>
                    {
                        if(entity.ShowDialog().Value)
                        {

                        }
                    };
                    dockPanel.Children.Add(generatorButton);
                }
            }
            #endregion

            for (int i = 0; i < tags.Count; i++)
            {
                string currentTag = Regex.Replace(tags[i].Replace("\"", ""),@"\s","");
                switch (currentTag)
                {
                    case "TAG_Boolean":
                        ComboBox comboBox = new()
                        {
                            Margin = new Thickness(5, 0, 0, 0),
                            Width = 60,
                            IsReadOnly = true,
                            Foreground = whiteBrush,
                            Style = Application.Current.Resources["TextComboBoxStyle"] as Style,
                            FontSize = 12,
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            SelectedIndex = 0
                        };
                        comboBox.Items.Add("True");
                        comboBox.Items.Add("False");
                        dockPanel.Children.Add(comboBox);
                        break;
                    case "TAG_Short":
                    case "TAG_Int":
                        double minValue;
                        double maxValue;
                        if (tags[i] == "TAG_Short")
                        {
                            minValue = short.MinValue; 
                            maxValue = short.MaxValue;
                        }
                        else
                        {
                            minValue = int.MinValue;
                            maxValue = int.MaxValue;
                        }
                        Slider slider = new()
                        {
                            Margin = new Thickness(5,0,0,0),
                            Height = 25,
                            Width = 100,
                            Value = 0,
                            Minimum = minValue,
                            Maximum = maxValue,
                            Style = Application.Current.Resources["NumberBoxStyle"] as Style,
                            FontSize = 12
                        };
                        dockPanel.Children.Add(slider);
                        break;
                }
            }

            if(description.ToolTip != null && description.ToolTip.ToString().Length > 0)
            dockPanel.Children.Add(description);
            #endregion
            return dockPanel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

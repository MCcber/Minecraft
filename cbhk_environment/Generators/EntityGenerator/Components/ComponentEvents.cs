using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    public partial class ComponentEvents
    {
        [GeneratedRegex("\\d+")]
        private static partial Regex IsHaveNumber();

        /// <summary>
        /// 首次获得焦点时执行绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValueChangedHandler(object sender, RoutedEventArgs e)
        {
            #region 共通变量
            Accordion parent = (sender as FrameworkElement).FindParent<Accordion>();
            Accordion grandParent = null;
            if (parent != null)
                grandParent = parent.FindParent<Accordion>();
            int currentIndex = 0;
            EntityPages entityPages = (sender as FrameworkElement).FindParent<EntityPages>();
            if (entityPages == null) return;
            entityPagesDataContext entityPage = entityPages.DataContext as entityPagesDataContext;
            PropertyPath propertyPath = null;
            Binding valueBinder = new()
            {
                Mode = BindingMode.OneWayToSource,
                Converter = new TagToString()
            };
            #endregion

            #region 是否为可疑实体列表
            if (sender is SuspectsEntities)
            {
                SuspectsEntities suspectsEntities = sender as SuspectsEntities;
                NBTDataStructure dataStructure = suspectsEntities.Tag as NBTDataStructure;
                if (!IsHaveNumber().IsMatch(suspectsEntities.Uid))
                {
                    suspectsEntities.LostFocus += SuspectsEntities_LostFocus;
                    entityPage.SpecialTagsResult.Add(dataStructure);
                    currentIndex = entityPage.SpecialTagsResult.Count - 1;
                    suspectsEntities.Uid = currentIndex.ToString();
                    propertyPath = new("SpecialTagsResult[" + currentIndex + "]");

                    valueBinder.Path = propertyPath;
                    var currentTag = suspectsEntities.Tag;
                    BindingOperations.SetBinding(suspectsEntities, FrameworkElement.TagProperty, valueBinder);
                    suspectsEntities.Tag = currentTag;
                }
            }
            #endregion

            #region 是否为振动监听器
            if (sender is VibrationMonitors)
            {
                VibrationMonitors vibrationMonitors = sender as VibrationMonitors;
                NBTDataStructure dataStructure = vibrationMonitors.Tag as NBTDataStructure;
                if (!IsHaveNumber().IsMatch(vibrationMonitors.Uid))
                {
                    vibrationMonitors.LostFocus += VibrationMonitors_LostFocus;
                    entityPage.SpecialTagsResult.Add(dataStructure);
                    currentIndex = entityPage.SpecialTagsResult.Count - 1;
                    vibrationMonitors.Uid = currentIndex.ToString();
                    propertyPath = new("SpecialTagsResult[" + currentIndex + "]");

                    valueBinder.Path = propertyPath;
                    var currentTag = vibrationMonitors.Tag;
                    BindingOperations.SetBinding(vibrationMonitors, FrameworkElement.TagProperty, valueBinder);
                    vibrationMonitors.Tag = currentTag;
                }
            }
            #endregion

            #region 是否为背包
            if (sender is Accordion && ((sender as FrameworkElement).Uid == "Item" || (sender as FrameworkElement).Uid == "Items"))
            {
                Accordion accordion = sender as Accordion;
                accordion.Modify = new RelayCommand<FrameworkElement>(AddItemCommand);
                accordion.Fresh = new RelayCommand<FrameworkElement>(ClearItemCommand);
            }
            #endregion

            #region 是否为坐标或UID
            if (sender is UUIDOrPosGroup)
            {
                UUIDOrPosGroup uUIDOrPosGroup = sender as UUIDOrPosGroup;
                NBTDataStructure dataStructure = uUIDOrPosGroup.Tag as NBTDataStructure;
                if (!IsHaveNumber().IsMatch(uUIDOrPosGroup.Uid))
                {
                    uUIDOrPosGroup.LostFocus += UUIDOrPosGroup_LostFocus;
                    if (parent.Uid != "SpecialTags")
                    {
                        entityPage.CommonResult.Add(dataStructure);
                        currentIndex = entityPage.CommonResult.Count - 1;
                        uUIDOrPosGroup.Uid = currentIndex.ToString();
                        propertyPath = new("CommonResult[" + currentIndex + "]");
                    }
                    else
                    {
                        entityPage.SpecialTagsResult.Add(dataStructure);
                        currentIndex = entityPage.SpecialTagsResult.Count - 1;
                        uUIDOrPosGroup.Uid = currentIndex.ToString();
                        propertyPath = new("SpecialTagsResult[" + currentIndex + "]");
                    }
                    valueBinder.Path = propertyPath;
                    var currentTag = uUIDOrPosGroup.Tag;
                    BindingOperations.SetBinding(uUIDOrPosGroup, FrameworkElement.TagProperty, valueBinder);
                    uUIDOrPosGroup.Tag = currentTag;
                }
            }
            #endregion

            #region 是否为方块状态
            if (sender is BlockState)
            {
                BlockState blockState = sender as BlockState;
                NBTDataStructure dataStructure = blockState.Tag as NBTDataStructure;
                if(!IsHaveNumber().IsMatch(blockState.Uid))
                {
                    blockState.MouseLeave += BlockState_MouseLeave;
                    if (parent.Uid != "SpecialTags")
                    {
                        entityPage.CommonResult.Add(dataStructure);
                        currentIndex = entityPage.CommonResult.Count - 1;
                        blockState.Uid = currentIndex.ToString();
                        propertyPath = new("CommonResult[" + currentIndex + "]");
                    }
                    else
                    {
                        entityPage.SpecialTagsResult.Add(dataStructure);
                        currentIndex = entityPage.SpecialTagsResult.Count - 1;
                        blockState.Uid = currentIndex.ToString();
                        propertyPath = new("SpecialTagsResult[" + currentIndex + "]");
                    }
                    valueBinder.Path = propertyPath;
                    var currentTag = blockState.Tag;
                    BindingOperations.SetBinding(blockState, FrameworkElement.TagProperty, valueBinder);
                    blockState.Tag = currentTag;
                }
            }
            #endregion

            #region 是否为网格
            if (sender is Grid)
            {
                Grid grid = sender as Grid;
                NBTDataStructure dataStructure = grid.Tag as NBTDataStructure;
                if(!IsHaveNumber().IsMatch(grid.Uid))
                {
                    string unit = "";

                    unit = grid.Uid[(grid.Uid.IndexOf('_') + 1)..grid.Uid.LastIndexOf('_')].ToLower()[0..1].Replace("i", "");
                    Slider subSlider0 = grid.FindChild<Slider>("0");
                    Slider subSlider1 = grid.FindChild<Slider>("1");
                    dataStructure.Result = "\"" + grid.Name + "\":[" + subSlider0.Value + unit + "," + subSlider1 + unit + "]";
                    grid.LostFocus += Grid_LostFocus;

                    if (parent.Uid != "SpecialTags")
                    {
                        entityPage.CommonResult.Add(dataStructure);
                        currentIndex = entityPage.CommonResult.Count - 1;
                        grid.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("CommonResult[" + currentIndex + "]");
                    }
                    else
                    {
                        entityPage.SpecialTagsResult.Add(dataStructure);
                        currentIndex = entityPage.SpecialTagsResult.Count - 1;
                        grid.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("SpecialTagsResult[" + currentIndex + "]");
                    }
                    valueBinder.Path = propertyPath;
                    var currentTag = grid.Tag;
                    BindingOperations.SetBinding(grid, FrameworkElement.TagProperty, valueBinder);
                    grid.Tag = currentTag;
                }
            }
            #endregion

            #region 当前为数字框
            if (sender is Slider)
            {
                Slider slider = sender as Slider;
                NBTDataStructure dataStructure = slider.Tag as NBTDataStructure;
                if (!IsHaveNumber().IsMatch(slider.Uid))
                {
                    slider.ValueChanged += NumberBoxValueChanged;
                    if (parent.Uid != "SpecialTags")
                    {
                        entityPage.CommonResult.Add(dataStructure);
                        currentIndex = entityPage.CommonResult.Count - 1;
                        slider.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("CommonResult[" + currentIndex + "]");
                    }
                    else
                    {
                        entityPage.SpecialTagsResult.Add(dataStructure);
                        currentIndex = entityPage.SpecialTagsResult.Count - 1;
                        slider.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("SpecialTagsResult[" + currentIndex + "]");
                    }
                    valueBinder.Path = propertyPath;
                    var currentTag = slider.Tag;
                    BindingOperations.SetBinding(slider, FrameworkElement.TagProperty, valueBinder);
                    slider.Tag = currentTag;
                }
            }
            #endregion

            #region 当前为Long型或文本框
            if (sender is TextBox)
            {
                TextBox textBox = sender as TextBox;
                NBTDataStructure dataStructure = textBox.Tag as NBTDataStructure;
                if (!IsHaveNumber().IsMatch(textBox.Uid))
                {

                    if (textBox.Uid == "TAG_String_List")
                        textBox.LostFocus += StringListBox_LostFocus;
                    else
                    if (textBox.Uid == "TAG_Long")
                        textBox.LostFocus += LongNumberBox_LostFocus;
                    else
                        textBox.LostFocus += StringBox_LostFocus;

                    if (parent.Uid != "SpecialTags")
                    {
                        entityPage.CommonResult.Add(textBox.Tag as NBTDataStructure);
                        currentIndex = entityPage.CommonResult.Count - 1;
                        textBox.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("CommonResult[" + currentIndex + "]");
                    }
                    else
                    {
                        entityPage.SpecialTagsResult.Add(textBox.Tag as NBTDataStructure);
                        currentIndex = entityPage.SpecialTagsResult.Count - 1;
                        textBox.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("SpecialTagsResult[" + currentIndex + "]");
                    }

                    valueBinder.Path = propertyPath;
                    var currengTag = dataStructure;
                    BindingOperations.SetBinding(textBox, FrameworkElement.TagProperty, valueBinder);
                    textBox.Tag = currengTag;
                }
            }
            #endregion

            #region 当前为是否框
            if (sender is TextCheckBoxs)
            {
                TextCheckBoxs textCheckBoxs = sender as TextCheckBoxs;
                NBTDataStructure dataStructure = textCheckBoxs.Tag as NBTDataStructure;
                if(!IsHaveNumber().IsMatch(textCheckBoxs.Uid))
                {
                    textCheckBoxs.Checked += CheckBox_Checked;
                    textCheckBoxs.Unchecked += CheckBox_Unchecked;

                    if (parent.Uid != "SpecialTags")
                    {
                        entityPage.CommonResult.Add(dataStructure);
                        currentIndex = entityPage.CommonResult.Count - 1;
                        textCheckBoxs.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("CommonResult[" + currentIndex + "]");
                    }
                    else
                    {
                        entityPage.SpecialTagsResult.Add(dataStructure);
                        currentIndex = entityPage.SpecialTagsResult.Count - 1;
                        textCheckBoxs.Uid = currentIndex.ToString();
                        propertyPath = new PropertyPath("SpecialTagsResult[" + currentIndex + "]");
                    }

                    valueBinder.Path = propertyPath;
                    var currentTag = textCheckBoxs.Tag;
                    BindingOperations.SetBinding(textCheckBoxs, FrameworkElement.TagProperty, valueBinder);
                    textCheckBoxs.Tag = currentTag;
                }
            }
            #endregion
        }

        /// <summary>
        /// 清空背包中的物品
        /// </summary>
        /// <param name="obj"></param>
        private void ClearItemCommand(FrameworkElement obj)
        {
            Accordion accordion = obj as Accordion;
            StackPanel stackPanel = (accordion.Content as ScrollViewer).Content as StackPanel;
            stackPanel.Children.Clear();
        }

        /// <summary>
        /// 为背包添加物品
        /// </summary>
        /// <param name="obj"></param>
        private void AddItemCommand(FrameworkElement obj)
        {
            Accordion accordion = obj as Accordion;
            StackPanel stackPanel = (accordion.Content as ScrollViewer).Content as StackPanel;
            EntityBag entityBag = new();
            if(accordion.Uid == "Items" || (stackPanel.Children.Count == 0 && accordion.Uid == "Item"))
            stackPanel.Children.Add(entityBag);
        }

        /// <summary>
        /// 可疑实体列表数据更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SuspectsEntities_LostFocus(object sender, RoutedEventArgs e)
        {
            SuspectsEntities suspectsEntities = sender as SuspectsEntities;
            NBTDataStructure dataStructure = suspectsEntities.Tag as NBTDataStructure;
            Accordion accordion = suspectsEntities.FindChild<Accordion>();
            StackPanel stackPanel = (accordion.Content as ScrollViewer).Content as StackPanel;
            if (stackPanel.Children.Count > 0)
            {
                string result = "";
                foreach (StackPanel item in stackPanel.Children)
                {
                    if(item.Tag != null)
                    result += item.Tag.ToString() + ",";
                }
                result = result.TrimEnd(',');
                dataStructure.Result = "anger:{suspects:[" + result + "]}";
            }
            else
                dataStructure.Result = "";
        }

        /// <summary>
        /// 振动传感器数据更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VibrationMonitors_LostFocus(object sender, RoutedEventArgs e)
        {
            VibrationMonitors vibrationMonitors = sender as VibrationMonitors;
            NBTDataStructure dataStructure = vibrationMonitors.Tag as NBTDataStructure;
            if (vibrationMonitors.VibrationMonitorsEnableButton.IsChecked.Value)
            {
                TabControl tabControl = vibrationMonitors.FindChild<TabControl>();
                StringBuilder result = new();
                for (int i = 0; i < tabControl.Items.Count; i++)
                {
                    FrameworkElement control = tabControl.Items[i] as FrameworkElement;
                    if (control.Tag != null)
                        result.Append(control.Tag.ToString() + ",");
                }
                string eventResult = "event_delay:" + vibrationMonitors.EventDelay.Value + ",";
                string rangeResult = "range:" + vibrationMonitors.Range.Value + ",";
                string content = result.ToString().TrimEnd(',').Length > 0 ? result.ToString().TrimEnd(',') : "";
                content = eventResult + rangeResult + content;
                content = content.TrimEnd(',');
                dataStructure.Result = vibrationMonitors.Name + ":{" + content + "}";
            }
            else
                dataStructure.Result = "";
        }

        /// <summary>
        /// UID或坐标组数据更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void UUIDOrPosGroup_LostFocus(object sender, RoutedEventArgs e)
        {
            UUIDOrPosGroup uUIDOrPosGroup = sender as UUIDOrPosGroup;
            NBTDataStructure dataStructure = uUIDOrPosGroup.Tag as NBTDataStructure;
            if (uUIDOrPosGroup.EnableButton.IsChecked.Value)
            {
                string unit = dataStructure.DataType.Replace("TAG_", "").ToLower()[0..1].Replace("i", "").Replace("u","");
                dataStructure.Result = uUIDOrPosGroup.Name + ":[" + uUIDOrPosGroup.number0.Value + unit + "," + uUIDOrPosGroup.number1.Value + unit + "," + uUIDOrPosGroup.number2.Value + unit + (dataStructure.DataType == "TAG_UUID" ? "," + uUIDOrPosGroup.number3.Value + unit : "") + "]";
            }
            else
                dataStructure.Result = "";
        }

        /// <summary>
        /// 方块状态数据更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlockState_MouseLeave(object sender, MouseEventArgs e)
        {
            BlockState blockState = sender as BlockState;
            BlockStateDataContext blockStateDataContext = blockState.AttributeAccordion.DataContext as BlockStateDataContext;
            NBTDataStructure dataStructure = blockState.Tag as NBTDataStructure;
            StringBuilder AttributeContent = new();
            int value = 0;
            bool IsInt;
            string blockID = blockStateDataContext.SelectedBlockIDString;
            int index = 0;
            string currentValue = "";
            _ = blockStateDataContext.SelectedAttributeKeys.All(item =>
            {
                if(item != null && item.Length > 0)
                {
                    currentValue = blockStateDataContext.SelectedAttributeValues[index];
                    _ = bool.TryParse(currentValue, out bool IsBool);
                    IsInt = int.TryParse(currentValue, out value);
                    if (!IsBool && !IsInt)
                        currentValue = "\"" + currentValue + "\"";
                    AttributeContent.Append(item + ":" + currentValue + ",");
                    index++;
                    return true;
                }
                return false;
            });
            dataStructure.Result = blockState.Name + ":{Name:\"" + blockID + "\",Properties:{" + AttributeContent.ToString().TrimEnd(',') + "}";
        }

        /// <summary>
        /// 数值型控件值更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberBoxValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            NBTDataStructure dataStructure = slider.Tag as NBTDataStructure;
            if (slider.Value == 0)
            {
                dataStructure.Result = "";
            }
            else
                dataStructure.Result = slider.Name + ":" + slider.Value + dataStructure.DataType.Replace("TAG_","").ToLower()[0..1].Replace("i", "");
        }

        /// <summary>
        /// 文本框文本值更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StringBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            NBTDataStructure dataStructure = textBox.Tag as NBTDataStructure;
            if (textBox.Text.Length == 0)
                dataStructure.Result = "";
            else
                dataStructure.Result = textBox.Name + ":\"" + textBox.Text + "\"";
        }

        /// <summary>
        /// 更新网格内所携带所有对象的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_LostFocus(object sender,RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            NBTDataStructure dataStructure = grid.Tag as NBTDataStructure;

            #region 两个浮点数成员
            Slider subSlider0 = grid.FindChild<Slider>("0");
            Slider subSlider1 = grid.FindChild<Slider>("1");
            string unit;
            unit = grid.Uid[(grid.Uid.IndexOf('_') + 1)..grid.Uid.LastIndexOf('_')].ToLower()[0..1];
            if (subSlider0 != null && subSlider1 != null)
            {
                if (subSlider0.Value != 0 || subSlider1.Value != 0)
                    dataStructure.Result = grid.Name + ":[" + subSlider0.Value + unit + "," + subSlider1 + unit + "]";
                else
                    dataStructure.Result = "";
            }
            #endregion
        }

        /// <summary>
        /// 字符串数组文本更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StringListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            NBTDataStructure dataStructure = textBox.Tag as NBTDataStructure;
            if(textBox.Text.Length == 0)
                dataStructure.Result = "";
            else
            {
                string[] valueList = textBox.Text.Split(';');
                string value = string.Join(",", "\""+ valueList + "\"");
                dataStructure.Result = textBox.Name + ":[" + value + "]";
            }
        }

        /// <summary>
        /// long型数值文本更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongNumberBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            NBTDataStructure dataStructure = textBox.Tag as NBTDataStructure;
            if (textBox.Text.Trim().Length > 0)
                dataStructure.Result = textBox.Name + ":" + textBox.Text + "l";
            else
                dataStructure.Result = "";
        }

        /// <summary>
        /// 切换按钮取消选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TextCheckBoxs textCheckBoxs = sender as TextCheckBoxs;
            NBTDataStructure dataStructure = textCheckBoxs.Tag as NBTDataStructure;
            dataStructure.Result = "";
        }

        /// <summary>
        /// 切换按钮选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TextCheckBoxs textCheckBoxs = sender as TextCheckBoxs;
            NBTDataStructure dataStructure = textCheckBoxs.Tag as NBTDataStructure;
            dataStructure.Result = textCheckBoxs.Name + ":true";
        }
    }

    /// <summary>
    /// Tag转字符串
    /// </summary>
    public class TagToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (NBTDataStructure)value;
        }
    }

    public class NBTDataStructure:FrameworkElement
    {
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }

        public string DataType { get; set; }

        /// <summary>
        /// 标记当前实例属于哪个共通标签
        /// </summary>
        public string NBTGroup { get; set; }
    }
}

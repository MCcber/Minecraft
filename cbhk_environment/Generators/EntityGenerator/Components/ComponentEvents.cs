using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    public class ComponentEvents
    {
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
            entityPagesDataContext entityPage = (sender as FrameworkElement).FindParent<EntityPages>().DataContext as entityPagesDataContext;
            PropertyPath propertyPath = null;
            Binding visibilityBinder = new()
            {
                Mode = BindingMode.OneWay
            };
            Binding valueBinder = new()
            {
                Mode = BindingMode.OneWayToSource,
                Converter = new TagToString()
            };
            #endregion

            #region 是否为网格
            if(sender is Grid)
            {
                Grid grid = sender as Grid;
                NBTDataStructure dataStructure = grid.Tag as NBTDataStructure;
                if(grid.Tag == null && grid.Uid.Length > 1)
                {
                    string unit = "";

                    #region 如果是浮点数网格
                    unit = grid.Uid[(grid.Uid.IndexOf('_') + 1)..grid.Uid.LastIndexOf('_')].ToLower()[0..1].Replace("i", "");
                    Slider subSlider0 = grid.FindChild<Slider>("0");
                    Slider subSlider1 = grid.FindChild<Slider>("1");
                    if (grandParent != null)
                        visibilityBinder.Path = new PropertyPath(grandParent.Uid + "Visibility");
                    else
                        visibilityBinder.Path = new PropertyPath(dataStructure.NBTGroup + "Visibility");
                    BindingOperations.SetBinding(dataStructure, NBTDataStructure.HaveCurrentTypeProperty, visibilityBinder);

                    dataStructure.Result = "\"" + grid.Name + "\":[" + subSlider0.Value + unit + "," + subSlider1 + unit + "]";
                    grid.LostFocus += Grid_LostFocus;
                    #endregion

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
                    BindingOperations.SetBinding(grid, FrameworkElement.TagProperty, valueBinder);
                }
            }
            #endregion

            #region 当前为数字框
            if (sender is Slider)
            {
                Slider slider = sender as Slider;
                NBTDataStructure dataStructure = slider.Tag as NBTDataStructure;
                if (slider.Uid.Length > 1)
                {
                    if (grandParent != null)
                        visibilityBinder.Path = new PropertyPath(grandParent.Uid + "Visibility");
                    else
                        visibilityBinder.Path = new PropertyPath(dataStructure.NBTGroup + "Visibility");
                    BindingOperations.SetBinding(dataStructure, NBTDataStructure.HaveCurrentTypeProperty, visibilityBinder);
                    dataStructure.Result = "\"" + slider.Name + "\":" + slider.Value + slider.Uid;
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
                    BindingOperations.SetBinding(slider, FrameworkElement.TagProperty, valueBinder);
                }
            }
            #endregion

            #region 当前为Long型或文本框
            if (sender is TextBox)
            {
                TextBox textBox = sender as TextBox;
                NBTDataStructure dataStructure = textBox.Tag as NBTDataStructure;
                if (textBox.Uid.Length > 1)
                {
                    if (grandParent != null)
                        visibilityBinder.Path = new PropertyPath(grandParent.Uid + "Visibility");
                    else
                        visibilityBinder.Path = new PropertyPath(dataStructure.NBTGroup + "Visibility");
                    BindingOperations.SetBinding(dataStructure, NBTDataStructure.HaveCurrentTypeProperty, visibilityBinder);

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
                if(textCheckBoxs.Uid.Length > 1)
                {
                    textCheckBoxs.Checked += CheckBox_Checked;
                    textCheckBoxs.Unchecked += CheckBox_Unchecked;

                    if (grandParent != null)
                        visibilityBinder.Path = new PropertyPath(grandParent.Uid + "Visibility");
                    else
                        visibilityBinder.Path = new PropertyPath(dataStructure.NBTGroup + "Visibility");
                    BindingOperations.SetBinding(dataStructure, NBTDataStructure.HaveCurrentTypeProperty, visibilityBinder);

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
                    BindingOperations.SetBinding(textCheckBoxs, FrameworkElement.TagProperty, valueBinder);
                }
            }
            #endregion
        }

        /// <summary>
        /// 数值型控件值更新事件
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
                dataStructure.Result = slider.Name + ":" + slider.Value + slider.Uid;
        }

        /// <summary>
        /// 文本框文本值更新事件
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
                    dataStructure.Result = "\"" + grid.Name + "\":[" + subSlider0.Value + unit + "," + subSlider1 + unit + "]";
                else
                    dataStructure.Result = "";
            }
            #endregion
        }

        /// <summary>
        /// 字符串数组文本更新事件
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
                string value = string.Join(",", valueList);
                dataStructure.Result = textBox.Name + ":[\"" + value + "\"]";
            }
        }

        /// <summary>
        /// long型数值文本更新事件
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
        /// 切换按钮取消选中事件
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
        /// 切换按钮选中事件
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

    public class NBTDataStructure:TextBlock
    {
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 当前实体ID下是否拥有该值，拥有则可见否则不可见
        /// </summary>
        public Visibility HaveCurrentType
        {
            get { return (Visibility)GetValue(HaveCurrentTypeProperty); }
            set { SetValue(HaveCurrentTypeProperty, value); }
        }

        public static readonly DependencyProperty HaveCurrentTypeProperty =
            DependencyProperty.Register("HaveCurrentType", typeof(Visibility), typeof(NBTDataStructure), new PropertyMetadata(default(Visibility)));

        public string DataType { get; set; }
        public string NBTGroup { get; set; }
    }
}

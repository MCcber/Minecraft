using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    /// <summary>
    /// Attributes.xaml 的交互逻辑
    /// </summary>
    public partial class Attributes : UserControl
    {
        string attributeNameFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\MobAttributes.json";
        JArray attributeArray = null;
        ObservableCollection<string> AttributeNames = new();
        public ObservableCollection<AttributeModifiers> AttributeModifiersSource = new();
        //public string SelectedAttributeName = "";

        public string Result
        {
            get
            {
                string result = "";
                string SelectedName = AttributeName.SelectedValue.ToString();
                if(SelectedName.Length > 0)
                result = "{Base: " + Base.Value + "d" + (AttributeModifiersSource.Count > 0 ? ",Modifiers:[" + string.Join(',', AttributeModifiersSource.Select(item => item.Result)) : "") + "],Name:\"" + SelectedName[(SelectedName.IndexOf(':') + 1)..SelectedName.LastIndexOf(':')] + "\"}";
                return result;
            }
        }

        public Attributes()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 载入属性名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttributeName_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string attributeNameContent = File.ReadAllText(attributeNameFilePath);
            attributeArray = JArray.Parse(attributeNameContent);
            foreach (JToken attributeObj in attributeArray)
                AttributeNames.Add(attributeObj["key"].ToString() + ":" + attributeObj["description"].ToString());
            comboBox.ItemsSource = AttributeNames;
            //if(SelectedAttributeName.Length > 0)
            //{
            //    string currentName = AttributeNames.Where(item=> item[(item.IndexOf(":") + 1)..item.LastIndexOf(':')] == SelectedAttributeName).First();
            //    comboBox.SelectedIndex = AttributeNames.IndexOf(currentName);
            //}
        }

        /// <summary>
        /// 切换选中的属性名后更新对应的值范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttributeName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            JArray range = JArray.Parse(attributeArray[AttributeName.SelectedIndex]["range"].ToString());
            Base.Minimum = double.Parse(range[0].ToString());
            Base.Maximum = double.Parse(range[1].ToString());
        }

        /// <summary>
        /// 载入修饰符添加与清空行为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modifiers_Loaded(object sender, RoutedEventArgs e)
        {
            Modifiers.Modify = new RelayCommand<FrameworkElement>(AddModifierCommand);
            Modifiers.Fresh = new RelayCommand<FrameworkElement>(ClearModifierCommand);
            ItemsControl itemsControl = (Modifiers.Content as ScrollViewer).Content as ItemsControl;
            itemsControl.ItemsSource = AttributeModifiersSource;
        }

        /// <summary>
        /// 添加修饰成员
        /// </summary>
        /// <param name="obj"></param>
        public void AddModifierCommand(FrameworkElement obj)
        {
            AttributeModifiersSource.Add(new AttributeModifiers());
        }

        /// <summary>
        /// 清空修饰成员
        /// </summary>
        /// <param name="obj"></param>
        private void ClearModifierCommand(FrameworkElement obj)
        {
            AttributeModifiersSource.Clear();
        }
    }
}

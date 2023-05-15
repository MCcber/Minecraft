using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components.SpecialNBT
{
    /// <summary>
    /// DebugProperties.xaml 的交互逻辑
    /// </summary>
    public partial class DebugProperties : UserControl
    {
        #region 方块ID集合与方块键集合
        private ObservableCollection<IconComboBoxItem> BlockIDList = MainWindow.BlockIDSource;
        private ObservableCollection<string> PropertyList = new();
        #endregion

        #region 字段
        private static string BlockPropertiesFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\BlockStates.json";
        private static JObject BlocksPropertyObject = null;
        #endregion

        #region 合并数据
        public string Result
        {
            get
            {
                string result = "";
                string selectedBlock = (BlockId.SelectedValue as IconComboBoxItem).ComboBoxItemText;
                string currentID = MainWindow.BlockDataBase.Keys.Where(item => item[(item.IndexOf(':') + 1)..] == selectedBlock).First();
                currentID = currentID[..currentID.IndexOf(':')];
                bool IsNumber = int.TryParse(BlockProperty.SelectedValue.ToString(),out int number);
                _ = bool.TryParse(BlockProperty.SelectedValue.ToString(),out bool boolean);
                string value = "";
                if (IsNumber)
                    value = number.ToString();
                else
                if (boolean)
                    value = boolean.ToString().ToLower();
                else
                    value = BlockProperty.SelectedValue.ToString();
                result = "\"" + currentID + "\":" + ((IsNumber || boolean) ? value : "\"" + value + "\"");
                return result;
            }
        }
        #endregion

        public DebugProperties()
        {
            InitializeComponent();

            #region 初始化数据
            BlockId.ItemsSource = BlockIDList;
            BlockProperty.ItemsSource = PropertyList;
            string content = File.ReadAllText(BlockPropertiesFilePath);
            BlocksPropertyObject = JObject.Parse(content);
            BlockId.SelectedIndex = 0;
            BlockProperty.SelectedIndex = 0;
            #endregion
        }

        /// <summary>
        /// 更新方块ID，更新右侧方块对应的key集合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlockId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(BlockId.SelectedValue is IconComboBoxItem)
            {
                string selectedBlock = (BlockId.SelectedValue as IconComboBoxItem).ComboBoxItemText;
                string targetObj = MainWindow.BlockDataBase.Keys.Where(item => item[(item.IndexOf(':') + 1)..] == selectedBlock).First();
                targetObj = targetObj[..targetObj.IndexOf(':')];
                JObject blockPropertiesObj = BlocksPropertyObject.SelectToken("minecraft:" + targetObj + ".properties") as JObject;
                PropertyList.Clear();
                if (blockPropertiesObj != null)
                {
                    List<JProperty> properties = blockPropertiesObj.Properties().ToList();
                    for (int i = 0; i < properties.Count; i++)
                        PropertyList.Add(properties[i].Name);
                    BlockProperty.SelectedIndex = 0;
                }
            }
        }

        private void IconTextButtons_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StackPanel stackPanel = Parent as StackPanel;
            stackPanel.Children.Remove(this);
            stackPanel.FindParent<Accordion>().FindChild<IconButtons>().Focus();
        }
    }
}

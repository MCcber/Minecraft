using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    /// <summary>
    /// PassengerItems.xaml 的交互逻辑
    /// </summary>
    public partial class PassengerItems : UserControl
    {
        public PassengerItems()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 引用存档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReferenceSaveClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                DefaultExt = ".command",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Multiselect = false,
                RestoreDirectory = true,
                Title = "请选择一个实体文件"
            };
            if(openFileDialog.ShowDialog().Value)
            {
                if(File.Exists(openFileDialog.FileName))
                {
                    string entityData = File.ReadAllText(openFileDialog.FileName);
                    entityData = entityData[entityData.IndexOf('{')..entityData.LastIndexOf('}')];
                    //补齐缺失双引号对的key
                    entityData = Regex.Replace(entityData, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
                    //清除数值型数据的单位
                    entityData = Regex.Replace(entityData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
                    if (entityData.Trim().Length == 0) return;
                    JToken entityTag = JObject.Parse(entityData)["EntityTag"];
                    if (entityTag != null)
                        entityData = entityTag.ToString();
                    else
                        entityTag = JObject.Parse(entityData);
                    string entityID = entityTag["id"].ToString();
                    DisplayEntity.Tag = entityData;
                    (DisplayEntity.Child as Image).Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + entityID + ".png", UriKind.RelativeOrAbsolute));
                }
            }
        }

        /// <summary>
        /// 删除当前乘客
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtons_Click(object sender, RoutedEventArgs e)
        {
            IconTextButtons iconTextButtons = sender as IconTextButtons;
            StackPanel stackPanel = iconTextButtons.FindParent<StackPanel>();
            stackPanel.Children.Remove(this);
        }
    }
}

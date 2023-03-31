using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace cbhk_environment.Generators.VillagerGenerator.Components
{
    /// <summary>
    /// GossipsItems.xaml 的交互逻辑
    /// </summary>
    public partial class GossipsItems : UserControl
    {

        #region 返回该言论的数据
        public string GossipData
        {
            get
            {
                string result;
                string item_data = Type.SelectedItem.ToString();
                string TypeData = item_data.Trim() != ""?"Type:"+ item_data+",":"";
                string ValueData = Value.ToString().Trim() != "" ? "Value:" + (Value.ToString().Contains(".") ? Value.ToString().Split('.')[0] :Value.ToString()) +",":"";
                string TargetData = Target.Text.Trim() != "" ?"Target:\""+Target.Text+"\",":"";
                result = TypeData != "" || ValueData != "" || TargetData != "" ?TypeData + ValueData + TargetData:"";
                result = "{" + result.TrimEnd(',') + "},";
                return result;
            }
        }
        #endregion

        //言论类型
        ObservableCollection<string> TypeList = new ObservableCollection<string> { };

        //言论配置文件路径
        string TypeFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Villager\\data\\GossipTypes.ini";

        public GossipsItems()
        {
            InitializeComponent();
        }

        private void TypeLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CustomControls.TextComboBoxs textComboBoxs = sender as CustomControls.TextComboBoxs;
            //读取言论类型
            if (File.Exists(TypeFilePath))
            {
                string[] types = File.ReadAllLines(TypeFilePath);
                for (int i = 0; i < types.Length; i++)
                {
                    TypeList.Add(types[i]);
                }
                textComboBoxs.ItemsSource = TypeList;
            }
        }

        private void IconTextButtons_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            villager_datacontext.gossipItems.Remove(this);
        }
    }
}

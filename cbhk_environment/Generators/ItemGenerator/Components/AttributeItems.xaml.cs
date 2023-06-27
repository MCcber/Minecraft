using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// AttributeItems.xaml 的交互逻辑
    /// </summary>
    public partial class AttributeItems : UserControl
    {
        #region 属性分量
        private string attributeID;
        private string attributeIDString = "";
        public string AttributeID
        {
            get
            {
                return attributeID;
            }
            set
            {
                attributeID = value;
                attributeIDString = MainWindow.AttribuiteDataBase.Where(item => item.Value == value).Select(item => item.Key).First();
            }
        }

        private string attributeSlot;
        private string attributeSlotString = "";
        public string AttributeSlot
        {
            get
            {
                return attributeSlot;
            }
            set
            {
                attributeSlot = value;
                attributeSlotString = MainWindow.AttributeSlotDataBase.Where(item => item.Value == value).Select(item=>item.Key).First();
            }
        }

        private string attributeValue = "0";
        public string AttributeValue
        {
            get
            {
                return attributeValue;
            }
            set
            {
                attributeValue = value;
            }
        }

        private string attributeValueType;
        private string attributeValueTypeString = "";
        public string AttributeValueType
        {
            get
            {
                return attributeValueType;
            }
            set
            {
                attributeValueType = value;
                attributeValueTypeString = MainWindow.AttributeValueTypeDatabase.Where(item => item.Value == value).Select(item=>item.Key).First();
            }
        }

        private string attributeName = "";
        public string AttributeName
        {
            get
            {
                return attributeName;
            }
            set
            {
                attributeName = value;
            }
        }
        #endregion

        /// <summary>
        /// 合并数据
        /// </summary>
        public string Result
        {
            get
            {
                Random random = new Random();
                string uid0 = random.Next(1000,10000).ToString();
                string uid1 = random.Next(1000, 10000).ToString();
                string uid2 = random.Next(1000, 10000).ToString();
                string uid3 = random.Next(1000, 10000).ToString();
                string result = "{AttributeName:\""+attributeIDString+"\",Name:\""+AttributeName+"\",Amount:"+AttributeValue+"d,Operation:"+attributeValueTypeString+",UUID:[I;"+uid0+","+uid1+","+uid2+","+uid3+"],Slot:\""+attributeSlotString+"\"}";
                return result;
            }
        }

        public AttributeItems()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 载入属性ID列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttributeIdsLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox ComboBox = sender as ComboBox;
            ComboBox.ItemsSource = MainWindow.AttributeSource;
        }

        /// <summary>
        /// 载入属性生效槽位ID列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttributeSlotsLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox ComboBox = sender as ComboBox;
            ComboBox.ItemsSource = MainWindow.AttributeSlotSource;
        }

        /// <summary>
        /// 载入属性值类型ID列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttributeValueTypesLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox ComboBox = sender as ComboBox;
            ComboBox.ItemsSource = MainWindow.AttributeValueTypeSource;
        }

        /// <summary>
        /// 删除当前属性成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<AttributeItems> attributeItems = this.FindParent<Data>().AttributeSource;
            attributeItems.Remove(this);
        }
    }
}

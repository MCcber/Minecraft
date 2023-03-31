using cbhk_environment.ControlsDataContexts;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components
{
    /// <summary>
    /// PotionTypeItems.xaml 的交互逻辑
    /// </summary>
    public partial class PotionTypeItems : UserControl
    {
        private IconComboBoxItem potionType;
        public IconComboBoxItem PotionType
        {
            get
            {
                return potionType;
            }
            set
            {
                potionType = value;
            }
        }

        private double potionDuration = 0;
        public double PotionDuration
        {
            get
            {
                return potionDuration;
            }
            set
            {
                potionDuration = value;
            }
        }

        private double potionLevel = 0;
        public double PotionLevel
        {
            get
            {
                return potionLevel;
            }
            set
            {
                potionLevel = value;
            }
        }

        public string Result
        {
            get
            {
                string result = "";
                string id = "";
                id = MainWindow.MobEffectDataBase.Where(item=>item.Value.Contains(PotionType.ComboBoxItemText)).Select(item=>item.Value).First();
                id = System.Text.RegularExpressions.Regex.Match(id, @"\d+").ToString();
                string Duration = PotionDuration.ToString();
                string Level = PotionLevel.ToString();
                result = "{Id:"+id+"b,Duration:"+ (Duration.Contains('.')? Duration.Substring(0, Duration.IndexOf('.')) :Duration) + ",Amplifier:"+ (Level.Contains('.') ? Level.Substring(0, Level.IndexOf('.')) : Level) + "b,Ambient:0b,ShowParticles:0b},";
                return result;
            }
        }

        public PotionTypeItems()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 删除当前药水效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconTextButtons_Click(object sender, RoutedEventArgs e)
        {
            StackPanel parent = Parent as StackPanel;
            //删除自己
            parent.Children.Remove(this);
        }

        /// <summary>
        /// 载入药水类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MobEffectIdLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            comboBoxs.ItemsSource = MainWindow.MobEffectIdSource;
        }
    }
}

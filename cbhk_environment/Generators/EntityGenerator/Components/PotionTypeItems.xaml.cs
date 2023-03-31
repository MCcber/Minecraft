using cbhk_environment.ControlsDataContexts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.EntityGenerator.Components
{
    /// <summary>
    /// PotionTypeItems.xaml 的交互逻辑
    /// </summary>
    public partial class PotionTypeItems : UserControl
    {
        string EffectIdString = "";

        public string Result
        {
            get
            {
                if (EffectIdString.Length > 0)
                {
                    string Duration = EffectDuration.Value.ToString();
                    Duration = Duration.Contains('.') ? Duration.Substring(0, Duration.IndexOf('.')) : Duration;
                    string Level = EffectLevel.Value.ToString();
                    Level = Level.Contains('.') ? Level.Substring(0, Level.IndexOf('.')) : Level;
                    string result = "{Id:" + EffectIdString + "b,Duration:" + Duration + ",Amplifier:" + Level + "b,Ambient:0b,ShowParticles:0b},";
                    return result;
                }
                else
                    return "";
            }
        }

        public PotionTypeItems()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 删除当前控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconTextButtons_Click(object sender, RoutedEventArgs e)
        {
            //删除自己
            StackPanel parent = Parent as StackPanel;
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

        /// <summary>
        /// 更新药水效果id列表成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MobEffectIdSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMobEffectData();
        }

        /// <summary>
        /// 药水效果时长更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MobEffectDurationSelectionChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateMobEffectData();
        }

        /// <summary>
        /// 药水效果等级更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MobEffectLevelSelectionChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateMobEffectData();
        }

        /// <summary>
        /// 更新实体药水效果数据
        /// </summary>
        private void UpdateMobEffectData()
        {
            if (EffectID.SelectedItem != null && int.TryParse(EffectDuration.Value.ToString(), out int effectDuration) && int.TryParse(EffectDuration.Value.ToString(), out int effectLevel))
            {
                IconComboBoxItem comboBoxItem = EffectID.SelectedItem as IconComboBoxItem;
                EffectIdString = MainWindow.MobEffectDataBase.Where(item => Regex.Match(item.Value, @"[\u4E00-\u9FFF]+").ToString() == comboBoxItem.ComboBoxItemText).First().Value;
                EffectIdString = Regex.Match(EffectIdString, @"\d+").ToString();
            }
        }
    }
}

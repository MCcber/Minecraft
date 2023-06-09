﻿using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator.Components.SpecialNBT
{
    /// <summary>
    /// CustomPotionEffects.xaml 的交互逻辑
    /// </summary>
    public partial class CustomPotionEffects : UserControl
    {
        #region 浮点数取值范围
        public float FloatMinValue
        {
            get
            {
                return float.MinValue;
            }
        }
        public float FloatMaxValue
        {
            get
            {
                return float.MaxValue;
            }
        }
        #endregion

        #region 合并数据
        private bool HaveResult = true;
        public string Result
        {
            get
            {
                if (HaveResult)
                {
                    string result = "";
                    List<string> Data = new();
                    List<string> FactorCalculationList = new();
                    foreach (FrameworkElement component in EffectListPanel.Children)
                    {
                        if (component is DockPanel)
                        {
                            DockPanel dockPanel = component as DockPanel;
                            foreach (FrameworkElement subChild in dockPanel.Children)
                            {
                                if (subChild is Slider)
                                {
                                    Slider slider = subChild as Slider;
                                    Data.Add(slider.Uid + ":" + slider.Value);
                                }
                                if (subChild is TextCheckBoxs)
                                {
                                    TextCheckBoxs textCheckBoxs = subChild as TextCheckBoxs;
                                    if (textCheckBoxs.IsChecked.Value)
                                        Data.Add(textCheckBoxs.Uid + ":1b");
                                }
                                if (subChild is ComboBox)
                                {
                                    ComboBox comboBox = subChild as ComboBox;
                                    string currentID = MainWindow.MobEffectDataBase.ElementAt(comboBox.SelectedIndex).Key;
                                    Data.Add(comboBox.Uid + ":\"minecraft:" + currentID + "\"");
                                }
                            }
                        }
                        else
                        if (component is TextCheckBoxs)
                        {
                            TextCheckBoxs textCheckBoxs = component as TextCheckBoxs;
                            if (textCheckBoxs.IsChecked.Value)
                                Data.Add(textCheckBoxs.Uid + ":1b");
                        }
                        else
                        if (component is Slider)
                        {
                            Slider slider = component as Slider;
                            if (slider.Value > 0)
                                Data.Add(slider.Uid + ":" + slider.Value + (Equals(slider.Maximum, float.MaxValue) ? "f" : ""));
                        }
                    }
                    foreach (FrameworkElement component in FactorCalculationDataGrid.Children)
                    {
                        if (component is TextCheckBoxs)
                        {
                            TextCheckBoxs textCheckBoxs = component as TextCheckBoxs;
                            if (textCheckBoxs.IsChecked.Value)
                                FactorCalculationList.Add(textCheckBoxs.Uid + ":1b");
                        }
                        else
                        if (component is Slider)
                        {
                            Slider slider = component as Slider;
                            if (slider.Value > 0)
                                FactorCalculationList.Add(slider.Uid + ":" + slider.Value + (Equals(slider.Maximum, float.MaxValue) ? "f" : ""));
                        }
                    }
                    result = "{" + string.Join(",", Data) + (FactorCalculationList.Count > 0 ? ",FactorCalculationData:{" + string.Join(",", FactorCalculationList) + "}" : "") + "}";
                    if (result == "{}")
                        result = "";
                    return result;
                }
                return "";
            }
        }
        #endregion

        public CustomPotionEffects()
        {
            InitializeComponent();
            DataContext = this;
            EffectAccordion.Fresh = new CommunityToolkit.Mvvm.Input.RelayCommand<FrameworkElement>(CloseEffectCommand);
        }

        /// <summary>
        /// 删除此状态效果
        /// </summary>
        /// <param name="obj"></param>
        private void CloseEffectCommand(FrameworkElement obj)
        {
            StackPanel stackPanel = Parent as StackPanel;
            stackPanel.Children.Remove(this);
            stackPanel.FindParent<Accordion>().FindChild<IconButtons>().Focus();
            HaveResult = false;
        }

        /// <summary>
        /// 载入状态效果数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectID_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.ItemsSource == null)
            {
                comboBox.ItemsSource = MainWindow.MobEffectIdSource;
                comboBox.SelectedIndex = 0;
            }
        }
    }
}

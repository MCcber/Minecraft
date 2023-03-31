using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace cbhk_environment.CustomControls
{
    public class RotationChartSetItem:Button
    {
        public BitmapImage ItemIcon
        {
            get { return (BitmapImage)GetValue(ItemIconProperty); }
            set { SetValue(ItemIconProperty, value); }
        }

        public static readonly DependencyProperty ItemIconProperty =
            DependencyProperty.Register("ItemIcon", typeof(BitmapImage), typeof(RotationChartSetItem), new PropertyMetadata(null));

        public string ItemUrl
        {
            get { return (string)GetValue(ItemUrlProperty); }
            set { SetValue(ItemUrlProperty, value); }
        }
        public static readonly DependencyProperty ItemUrlProperty =
            DependencyProperty.Register("ItemUrl", typeof(string), typeof(RotationChartSetItem), new PropertyMetadata(default(string)));

        public RelayCommand<FrameworkElement> SetUrl
        {
            get { return (RelayCommand<FrameworkElement>)GetValue(SetUrlProperty); }
            set { SetValue(SetUrlProperty, value); }
        }

        public static readonly DependencyProperty SetUrlProperty =
            DependencyProperty.Register("SetUrl", typeof(RelayCommand<FrameworkElement>), typeof(RotationChartSetItem), new PropertyMetadata(null));

        public RelayCommand<FrameworkElement> DeleteUrl
        {
            get { return (RelayCommand<FrameworkElement>)GetValue(DeleteUrlProperty); }
            set { SetValue(DeleteUrlProperty, value); }
        }

        public static readonly DependencyProperty DeleteUrlProperty =
            DependencyProperty.Register("DeleteUrl", typeof(RelayCommand<FrameworkElement>), typeof(RotationChartSetItem), new PropertyMetadata(null));
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.CustomControls
{
    public class TextButtons:Button
    {
        public BitmapImage ThicknessBackground
        {
            get { return (BitmapImage)GetValue(ThicknessBackgroundProperty); }
            set { SetValue(ThicknessBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ThicknessBackgroundProperty =
            DependencyProperty.Register("ThicknessBackground", typeof(BitmapImage), typeof(TextButtons), new PropertyMetadata(default(BitmapImage)));

        public SolidColorBrush MouseOverBackground
        {
            get { return (SolidColorBrush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(SolidColorBrush), typeof(TextButtons), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush MouseLeftDownBackground
        {
            get { return (SolidColorBrush)GetValue(MouseLeftDownBackgroundProperty); }
            set { SetValue(MouseLeftDownBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MouseLeftDownBackgroundProperty =
            DependencyProperty.Register("MouseLeftDownBackground", typeof(SolidColorBrush), typeof(TextButtons), new PropertyMetadata(default(SolidColorBrush)));
    }
}

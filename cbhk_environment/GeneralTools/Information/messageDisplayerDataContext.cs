using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.GeneralTools.Information
{
    public class messageDisplayerDataContext:ObservableObject
    {
        public enum MessageBoxType
        {
            Error,
            Infomation
        }

        public string DisplayInfomation { get; set; }

        public ImageSource MessageIcon { get; set; }

        public string MessageTitle { get; set; }

        #region 确定和取消
        public RelayCommand<Window> Yes { get; set; }
        public RelayCommand<Window> No { get; set; }
        #endregion

        private string errorIconPath = "pack://application:,,,/cbhk_environment;component/resources/cbhk_form/images/Error.png";
        private string infoIconPath = "pack://application:,,,/cbhk_environment;component/resources/cbhk_form/images/Info.png";

        public static ImageSource errorIcon;
        public static ImageSource infoIcon;
        public messageDisplayerDataContext()
        {
            Yes = new RelayCommand<Window>(YesCommand);
            No = new RelayCommand<Window>(NoCommand);
            errorIcon = new BitmapImage(new Uri(errorIconPath, UriKind.RelativeOrAbsolute));
            infoIcon = new BitmapImage(new Uri(infoIconPath, UriKind.RelativeOrAbsolute));
            //MessageTitle = type == MessageBoxType.Error ? "错误" : "警告";
            //MessageIcon = type == MessageBoxType.Error ? errorIcon : infoIcon;
            //DisplayInfomation = content;
        }

        private void NoCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void YesCommand(Window window)
        {
            window.DialogResult = true;
        }

        private void CommonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NoCommand(sender as Window);
        }
    }
}

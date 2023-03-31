using cbhk_environment.CustomControls;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace cbhk_environment.SettingForm
{
    /// <summary>
    /// SetRoatationChart.xaml 的交互逻辑
    /// </summary>
    public partial class SetRoatationChart
    {
        public RelayCommand AddItem { get; set; }
        public RelayCommand ClearItems { get; set; }
        public SetRoatationChart()
        {
            InitializeComponent();
            DataContext = this;
            AddItem = new RelayCommand(AddRotationChartItem);
            ClearItems = new RelayCommand(ClearRotationChartItem);
            InitUI();        
        }

        private void InitUI()
        {
            string[] LinkList = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "resources\\link_data","*.txt");
            foreach (string link in LinkList)
            {
                RotationChartSetItem rotationChartSetItem = new RotationChartSetItem();
                LinkStackPanel.Children.Add(rotationChartSetItem);

                string imagePath = Path.GetDirectoryName(link) + "\\" + Path.GetFileNameWithoutExtension(link) + "Icon.png";
                if (File.Exists(imagePath))
                {
                    rotationChartSetItem.ItemIcon = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }

                rotationChartSetItem.SetUrl = new RelayCommand<FrameworkElement>(SetUrlCommand);
                rotationChartSetItem.DeleteUrl = new RelayCommand<FrameworkElement>(DeleteUrlCommand);
                if (File.Exists(link))
                {
                    rotationChartSetItem.ItemUrl = link;
                }
            }
        }

        private void ClearRotationChartItem()
        {
            LinkStackPanel.Children.Clear();
        }

        private void AddRotationChartItem()
        {
            RotationChartSetItem rotationChartSetItem = new RotationChartSetItem();
            rotationChartSetItem.SetUrl = new RelayCommand<FrameworkElement>(SetUrlCommand);
            rotationChartSetItem.DeleteUrl = new RelayCommand<FrameworkElement>(DeleteUrlCommand);
            LinkStackPanel.Children.Add(rotationChartSetItem);
        }

        private void DeleteUrlCommand(FrameworkElement obj)
        {
            LinkStackPanel.Children.Remove(obj);
        }

        private void SetUrlCommand(FrameworkElement obj)
        {
            RotationChartSetItem rotationChartSetItem = (obj as TextBox).TemplatedParent as RotationChartSetItem;
            System.Windows.Forms.OpenFileDialog betterFolderBrowser = new System.Windows.Forms.OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Filter = "文本文件(*.txt)|*.txt",
                Title = "请选择一个包含链接的文本文件"
            };
            if(betterFolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if(File.Exists(betterFolderBrowser.FileName))
                {
                    if(betterFolderBrowser.FileName != AppDomain.CurrentDomain.BaseDirectory + "resources\\link_data\\" + Path.GetFileName(betterFolderBrowser.FileName))
                    File.Copy(betterFolderBrowser.FileName, AppDomain.CurrentDomain.BaseDirectory + "resources\\link_data\\" + Path.GetFileName(betterFolderBrowser.FileName));
                    string urlString = File.ReadAllText(betterFolderBrowser.FileName);
                    rotationChartSetItem.ItemUrl = urlString;
                    WebClient client = new WebClient();
                    client.DownloadFile(urlString + "/favicon.ico", AppDomain.CurrentDomain.BaseDirectory+ "resources\\link_data\\"+Path.GetFileNameWithoutExtension(betterFolderBrowser.FileName) + "Icon.png");

                    if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "resources\\link_data\\" + Path.GetFileNameWithoutExtension(betterFolderBrowser.FileName) + "Icon.png"))
                    {
                        rotationChartSetItem.ItemIcon = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\link_data\\" + Path.GetFileNameWithoutExtension(betterFolderBrowser.FileName) + "Icon.png", UriKind.Absolute));
                    }
                }
            }
        }
    }
}

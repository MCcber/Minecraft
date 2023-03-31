using cbhk_environment.CustomControls;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace cbhk_environment.Generators.OnlyOneCommandGenerator
{
    public class only_one_command_datacontext:ObservableObject
    {
        /// <summary>
        /// 生成盔甲架数据
        /// </summary>
        public RelayCommand RunCommand { get; set; }

        /// <summary>
        /// 返回主页
        /// </summary>
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }

        /// <summary>
        /// 添加一条ooc
        /// </summary>
        public RelayCommand AddOneCommandPage { get; set; }

        /// <summary>
        /// 清空ooc
        /// </summary>
        public RelayCommand ClearCommandPage { get; set; }

        public TabControl OneCommandTabControl = null;

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconCommandBlock.png";

        private RichTabItems currentItem = null;
        public RichTabItems CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                currentItem = value;
                OnPropertyChanged();
            }
        }

        public only_one_command_datacontext()
        {
            #region 绑定指令
            RunCommand = new RelayCommand(run_command);
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            AddOneCommandPage = new RelayCommand(AddOneCommandPageCommand);
            ClearCommandPage = new RelayCommand(ClearCommandPageCommand);
            #endregion
        }

        /// <summary>
        /// 清空ooc
        /// </summary>
        private void ClearCommandPageCommand()
        {
            OneCommandTabControl.Items.Clear();
        }

        /// <summary>
        /// 添加ooc
        /// </summary>
        private void AddOneCommandPageCommand()
        {
            //ScrollViewer scrollViewer = new ScrollViewer
            //{
            //    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            //    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            //    Style = Application.Current.Resources["DefaultScrollViewer"] as Style
            //};
            RichTextBox richTextBox = new RichTextBox()
            {
                MinHeight = 318,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A")),
                Foreground = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(0),
                FontSize = 15,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
            EnabledFlowDocument enabledFlowDocument = new EnabledFlowDocument()
            {
                LineHeight = 10
            };
            richTextBox.Document = enabledFlowDocument;
            //scrollViewer.Content = richTextBox;
            RichTabItems tabItem = new RichTabItems
            {
                Padding = new Thickness(10, 2, 0, 0),
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.White),
                Header = "OOC",
                IsContentSaved = true,
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Content = richTextBox
            };
            CurrentItem = tabItem;
            OneCommandTabControl.Items.Add(tabItem);
            OneCommandTabControl.SelectedItem = tabItem;
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="obj"></param>
        private void return_command(CommonWindow win)
        {
            OnlyOneCommand.cbhk.Topmost = true;
            OnlyOneCommand.cbhk.WindowState = WindowState.Normal;
            OnlyOneCommand.cbhk.Show();
            OnlyOneCommand.cbhk.Topmost = false;
            OnlyOneCommand.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 执行生成
        /// </summary>
        private void run_command()
        {
            string resultStartpart = "summon falling_block ~ ~1.5 ~ {Time:1,Block:\"minecraft:redstone_block\",Motion:[0d,-1d,0d],Passengers:[{id:falling_block,Time:1,Block:\"minecraft:activator_rail\",Passengers:[{id:commandblock_minecart,Command:\"blockdata ~ ~-2 ~ {auto:0b,Command:\\\"\\\"}\"},{id:commandblock_minecart,Command:\"setblock ~1 ~-2 ~ repeating_command_block 5 replace {Command:\\\"\\\",auto:1b}\"},";
            string resultEndPart = "{id:commandblock_minecart,Command:\"setblock ~ ~1 ~ command_block 0 replace {auto:1b,Command:\\\"fill ~ ~ ~ ~ ~-2 ~ air\\\"}\"},{id:commandblock_minecart,Command:\"kill @e[type=commandblock_minecart,r=1]\"}]}]}";
            string resultContent = "";

            int Offset = 2;

            foreach (RichTabItems tab in OneCommandTabControl.Items)
            {
                if(tab.Uid == "")
                {
                    RichTextBox richTextBox = tab.Content as RichTextBox;
                    foreach (Paragraph para in richTextBox.Document.Blocks)
                    {
                        TextRange content = new TextRange(para.ContentStart, para.ContentEnd);
                        resultContent += "{id:commandblock_minecart,Command:\"setblock ~" + Offset + " ~-2 ~ chain_command_block 5 replace {Command:\\\"" + content.Text + "\\\",auto:1b}\"},";
                        Offset++;
                    }
                }
            }

            string result = resultStartpart + resultContent + resultEndPart;
            GenerateResultDisplayer.Displayer displayer = GenerateResultDisplayer.Displayer.GetContentDisplayer();
            displayer.GeneratorResult(result, "OOC",icon_path);
            displayer.Show();
        }

        public void TabControlLoaded(object sender,RoutedEventArgs e)
        {
            OneCommandTabControl = sender as TabControl;
            OneCommandTabControl.SelectionChanged += OneCommandTabControl_SelectionChanged;
        }

        private void OneCommandTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentItem = OneCommandTabControl.SelectedItem as RichTabItems;
        }
    }
}

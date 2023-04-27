﻿using cbhk_environment.CustomControls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cbhk_environment.GenerateResultDisplayer
{
    /// <summary>
    /// Displayer.xaml 的交互逻辑
    /// </summary>
    public partial class Displayer
    {
        SolidColorBrush tranparentBrush = new((Color)ColorConverter.ConvertFromString("Transparent"));
        SolidColorBrush blackBrush = new((Color)ColorConverter.ConvertFromString("#000000"));
        string buttonNormalImage = "pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonNormal.png";
        string buttonPressedImage = "pack://application:,,,/cbhk_environment;component/resources/common/images/ButtonPressed.png";
        ImageBrush buttonNormalBrush = null;
        ImageBrush buttonPressedBrush = null;
        /// <summary>
        /// 进程锁
        /// </summary>
        private static object obj = new();

        /// <summary>
        /// 单例模式,用于显示生成结果
        /// </summary>
        private Displayer()
        {
            InitializeComponent();

            #region 初始化字段
            buttonNormalBrush = new ImageBrush(new BitmapImage(new Uri(buttonNormalImage, UriKind.RelativeOrAbsolute)));
            buttonPressedBrush = new ImageBrush(new BitmapImage(new Uri(buttonPressedImage, UriKind.RelativeOrAbsolute)));
            #endregion
        }

        /// <summary>
        /// 单例,避免指令重排
        /// </summary>
        private volatile static Displayer content_displayer;

        public static Displayer GetContentDisplayer()
        {
            if (content_displayer == null)
            {
                lock (obj)
                {
                    if (content_displayer == null)
                    {
                        content_displayer = new Displayer();
                    }
                }
            }
            return content_displayer;
        }

        /// <summary>
        /// 生成结果
        /// </summary>
        /// <param name="Overlying">是否覆盖</param>
        /// <param name="spawn_result">数据集</param>
        /// <param name="header_text">数据头</param>
        /// <param name="head_image_pathes">所表示的生成器图标</param>
        public void GeneratorResult(string spawn_result, string header_text, string head_image_path)
        {
            #region 遍历当前标签页对象，查找是否有相同类型的
            bool ExistSamePage = false;
            RichTabItems currentTabItem = null;
            foreach (RichTabItems tab in ResultTabControl.Items)
            {
                ExistSamePage = tab.Header.ToString() == header_text;
                if (ExistSamePage)
                {
                    currentTabItem = tab;
                    break;
                }
            }
            #endregion

            #region 创建复制生成结果的按钮
            IconTextButtons copyResultButton = new()
            {
                Style = Application.Current.Resources["IconTextButton"] as Style,
                Content = "复制结果",
                Margin = new Thickness(0,0,5,0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(5),
                Foreground = blackBrush,
                Background = buttonNormalBrush,
                PressedBackground = buttonPressedBrush
            };
            #endregion

            #region 处理新加入的数据
            //如果不存在相同标签页
            if (!ExistSamePage)
            {
                EnabledFlowDocument flowDocument = new() { LineHeight = 1 };

                #region 显示生成器图标
                Paragraph firstParagraph = new() { FontSize = 25, TextAlignment = TextAlignment.Center };
                firstParagraph.Inlines.Add(new Run("------------ "));
                firstParagraph.Inlines.Add(new Image()
                {
                    Source = new BitmapImage(new Uri(head_image_path, UriKind.Absolute)),
                    Width = 20,
                    Height = 20
                });
                firstParagraph.Inlines.Add(new Run(" ------------"));
                #endregion

                copyResultButton.Click += (a,b) => { Clipboard.SetText(spawn_result); };
                Paragraph paragraph = new();
                paragraph.Inlines.Add(copyResultButton);
                paragraph.Inlines.Add(new Run(spawn_result));
                flowDocument.Blocks.Add(firstParagraph);
                flowDocument.Blocks.Add(paragraph);
                RichTextBox result_box = new()
                {
                    IsReadOnly = true,
                    Document = flowDocument,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Background = tranparentBrush,
                    FontSize = 15,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    CaretBrush = new SolidColorBrush(Colors.White)
                };
                ScrollViewer.SetVerticalScrollBarVisibility(result_box,ScrollBarVisibility.Auto);
                ScrollViewer.SetHorizontalScrollBarVisibility(result_box, ScrollBarVisibility.Disabled);

                RichTabItems itt = new()
                {
                    IsContentSaved = true,
                    Content = result_box,
                    Foreground = new SolidColorBrush(Colors.White),
                    Header = header_text,
                    Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                    HeaderImage = new BitmapImage(new Uri(head_image_path, UriKind.RelativeOrAbsolute)),
                };
                content_displayer.ResultTabControl.Items.Add(itt);
                content_displayer.ResultTabControl.SelectedItem = itt;
            }
            else//如果存在相同标签页
            {
                RichTextBox richTextBox = currentTabItem.Content as RichTextBox;
                Paragraph firstParagraph = richTextBox.Document.Blocks.ElementAt(1) as Paragraph;

                #region 用于分割的段落
                Paragraph splitParagraph = new() { FontSize = 25, TextAlignment = TextAlignment.Center };
                Run splitRun = new("--------------------");
                splitParagraph.Inlines.Add(splitRun);
                #endregion

                Paragraph newParagraph = new();
                copyResultButton.Click += (a,b) => { Clipboard.SetText(spawn_result); };
                newParagraph.Inlines.Add(copyResultButton);
                newParagraph.Inlines.Add(new Run(spawn_result));
                richTextBox.Document.Blocks.InsertBefore(firstParagraph,splitParagraph);
                richTextBox.Document.Blocks.InsertBefore(splitParagraph, newParagraph);
            }
            #endregion
        }

        /// <summary>
        /// 隐藏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}

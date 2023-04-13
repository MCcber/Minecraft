using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cbhk_environment.Generators.TagGenerator
{
    public class tag_datacontext: ObservableObject
    {
        #region 生成与返回
        public RelayCommand RunCommand { get; set; }

        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        #endregion

        #region 替换
        private bool replace;
        public bool Replace
        {
            get { return replace; }
            set
            {
                replace = value;
                OnPropertyChanged();
            }
        }
        #endregion

        //存储最终生成的列表
        List<string> BlocksAndItems = new();
        List<string> Entities = new();

        #region 搜索内容
        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                TagViewSource?.View.Refresh();
            }
        }
        #endregion

        #region 所有标签成员
        private ObservableCollection<TagItemTemplate> tagItems = new ObservableCollection<TagItemTemplate> { };
        public ObservableCollection<TagItemTemplate> TagItems
        {
            get
            {
                return tagItems;
            }
            set
            {
                tagItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        //载入进程锁
        object tagItemsLock = new();

        //标签容器
        ListView TagZone = null;

        #region 当前选中成员
        private TagItemTemplate selectedItem = null;
        public TagItemTemplate SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
            }
        }
        #endregion

        #region 全选或反选
        private bool selectedAll = false;
        public bool SelectedAll
        {
            get
            {
                return selectedAll;
            }
            set
            {
                selectedAll = value;
                if (TagViewSource != null)
                {
                    foreach (TagItemTemplate tagItemTemplate in TagViewSource.View)
                    {
                        if (tagItemTemplate.DisplayText.Trim().Length > 0)
                        {
                            string itemString = tagItemTemplate.DisplayText.Trim()[..tagItemTemplate.DisplayText.Trim().IndexOf(' ')];
                            tagItemTemplate.BeChecked = selectedAll;

                            if (tagItemTemplate.BeChecked.Value)
                            {
                                if (tagItemTemplate.DataType == "Item")
                                    BlocksAndItems.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Entity")
                                    Entities.Add("\"minecraft:" + itemString + "\",");
                            }
                            else
                            {
                                if (tagItemTemplate.DataType == "Item")
                                    BlocksAndItems.Remove("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Entity")
                                    Entities.Remove("\"minecraft:" + itemString + "\",");
                            }
                        }
                    }
                }
            }
        }
        private bool reverseAll = false;
        public bool ReverseAll
        {
            get
            {
                return reverseAll;
            }
            set
            {
                reverseAll = value;
                if (TagViewSource != null)
                {
                    foreach (TagItemTemplate tagItemTemplate in TagViewSource.View)
                    {
                        if (tagItemTemplate.DisplayText.Trim().Length > 0)
                        {
                            string itemString = tagItemTemplate.DisplayText.Trim()[..tagItemTemplate.DisplayText.Trim().IndexOf(' ')];
                            tagItemTemplate.BeChecked = !tagItemTemplate.BeChecked.Value;
                            if (tagItemTemplate.BeChecked.Value)
                            {
                                if (tagItemTemplate.DataType == "Item")
                                    BlocksAndItems.Add("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Entity")
                                    Entities.Add("\"minecraft:" + itemString + "\",");
                            }
                            else
                            {
                                if (tagItemTemplate.DataType == "Item")
                                    BlocksAndItems.Remove("\"minecraft:" + itemString + "\",");
                                else
                                if (tagItemTemplate.DataType == "Entity")
                                    Entities.Remove("\"minecraft:" + itemString + "\",");
                            }
                        }
                    }
                }
            }
        }
        #endregion

        //对象数据源
        CollectionViewSource TagViewSource = null;

        public tag_datacontext()
        {
            #region 链接指令
            RunCommand = new RelayCommand(run_command);
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            #endregion

            #region 异步载入标签成员
            BindingOperations.EnableCollectionSynchronization(TagItems, tagItemsLock);
            Task.Run(() =>
            {
                lock(tagItemsLock)
                {
                    string uriDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
                    string urlPath = "";
                    string id = "";
                    foreach (var item in MainWindow.ItemDataBase)
                    {
                        id = item.Key[..item.Key.IndexOf(":")];
                        urlPath = uriDirectoryPath + id + ".png";
                        int matchCount = MainWindow.EntityDataBase.Where(block => block.Key[..block.Key.IndexOf(':')] == id).Count();
                        string uid = matchCount > 0 ? "Entity" : "Item";
                        TagItems.Add(new TagItemTemplate()
                        {
                            Icon = new Uri(urlPath, UriKind.RelativeOrAbsolute),
                            DisplayText = item.Key.Replace(":", " "),
                            DataType = uid,
                            BeChecked = false
                        });
                    }
                }
            });
            #endregion
        }

        /// <summary>
        /// 过滤与搜索内容不相关的成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (SearchText.Trim().Length > 0)
            {
                e.Accepted = false;
                TagItemTemplate tagItemTemplate = e.Item as TagItemTemplate;
                string currentItemIDAndName = tagItemTemplate.DisplayText;
                if (currentItemIDAndName.Contains(SearchText))
                    e.Accepted = true;
            }
            else
                e.Accepted = true;
        }

        /// <summary>
        /// 载入类型过滤列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TypeFilterLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBoxs = sender as ComboBox;
            comboBoxs.ItemsSource = MainWindow.TypeItemSource;
        }

        /// <summary>
        /// 更新过滤类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            Tag.cbhk.Topmost = true;
            Tag.cbhk.WindowState = WindowState.Normal;
            Tag.cbhk.Show();
            Tag.cbhk.Topmost = false;
            Tag.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 执行生成
        /// </summary>
        private void run_command()
        {
            string result = string.Join("\r\n",BlocksAndItems) + "\r\n" + string.Join("\r\n",Entities).TrimEnd(',');
            result = "{\r\n  \"replace\": " + Replace.ToString().ToLower() + ",\r\n\"values\": [\r\n" + result.TrimEnd('\n').TrimEnd('\r').TrimEnd(',') + "\r\n]\r\n}";
            SaveFileDialog saveFileDialog = new()
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "json",
                Filter = "JSON files (*.json;)|*.json;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Title = "保存为JSON文件"
            };
            if (saveFileDialog.ShowDialog().Value)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(saveFileDialog.FileName));
                File.WriteAllText(saveFileDialog.FileName, result);
                OpenFolderThenSelectFiles.ExplorerFile(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// 左击选中成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ListBoxClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem != null)
            {
                ReverseValue(SelectedItem);
            }
        }

        public void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            TagZone = sender as ListView;

            #region 获取数据源引用，订阅过滤事件
            Window parent = Window.GetWindow(TagZone);
            TagViewSource = parent.FindResource("TagItemSource") as CollectionViewSource;
            TagViewSource.Filter += CollectionViewSource_Filter;
            #endregion
        }

        /// <summary>
        /// 反转目标成员的值
        /// </summary>
        /// <param name="CurrentItem"></param>
        private void ReverseValue(TagItemTemplate CurrentItem)
        {
            if (TagZone.ItemContainerGenerator.ContainerFromItem(CurrentItem) is ListViewItem listViewItem)
            {
                ContentPresenter contentPresenter = ChildrenHelper.FindVisualChild<ContentPresenter>(listViewItem);
                RichCheckBoxs iconCheckBoxs = contentPresenter.ContentTemplate.FindName("checkbox", contentPresenter) as RichCheckBoxs;
                string displayText = CurrentItem.DisplayText;
                string itemString;
                if (displayText.Trim().Length > 0)
                {
                    itemString = displayText.Trim()[..displayText.Trim().IndexOf(' ')];
                    if (Regex.IsMatch(displayText.Trim(), "[a-zA-z_]+"))
                    {
                        iconCheckBoxs.IsChecked = !iconCheckBoxs.IsChecked.Value;
                        if (iconCheckBoxs.IsChecked.Value)
                        {
                            if (CurrentItem.DataType == "Item")
                                BlocksAndItems.Add("\"minecraft:" + itemString + "\",");
                            else
                            if (CurrentItem.DataType == "Entity")
                                Entities.Add("\"minecraft:" + itemString + "\",");
                        }
                        else
                        {
                            if (CurrentItem.DataType == "Item")
                                BlocksAndItems.Remove("\"minecraft:" + itemString + "\",");
                            else
                            if (CurrentItem.DataType == "Entity")
                                Entities.Remove("\"minecraft:" + itemString + "\",");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 载入成员的属性模板
    /// </summary
    public class TagItemTemplate:ObservableObject
    {
        public Uri Icon { get; set; }
        public string DataType { get; set; }
        public string DisplayText { get; set; }

        private bool? beChecked = false;
        public bool? BeChecked
        {
            get
            {
                return beChecked;
            }
            set
            {
                beChecked = value;
                OnPropertyChanged();
            }
        }
    }
}
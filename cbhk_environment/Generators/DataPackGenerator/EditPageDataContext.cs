﻿using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.Generators.DataPackGenerator.Components.EditPage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace cbhk_environment.Generators.DataPackGenerator
{
    public class EditPageDataContext : ObservableObject
    {
        #region 数据包搜索文本框内容
        private string datapackSeacherValue = "";
        public string DatapackSeacherValue
        {
            get => datapackSeacherValue;
            set
            {
                SetProperty(ref datapackSeacherValue, value);
                #region 搜索拥有指定数据的节点

                #endregion
            }
        }
        #endregion

        #region 文本编辑器标签页数据源、数据包管理器树视图数据源
        public ObservableCollection<RichTabItems> FunctionModifyTabItems { get; set; } = new();
        public ObservableCollection<TreeViewItem> DatapackTreeViewItems { get; set; } = new();
        #endregion

        #region 文本编辑器标签页容器可见性
        private Visibility functionModifyTabControlVisibility = Visibility.Visible;
        public Visibility FunctionModifyTabControlVisibility
        {
            get => functionModifyTabControlVisibility;
            set => SetProperty(ref functionModifyTabControlVisibility,value);
        }
        #endregion

        #region 当前选中的文本编辑器
        private RichTabItems selectedFileItem = null;
        public RichTabItems SelectedFileItem
        {
            get => selectedFileItem;
            set => SetProperty(ref selectedFileItem, value);
        }
        #endregion

        #region 数据包管理器右键菜单指令集
        public RelayCommand AddNewItem { get; set; }
        public RelayCommand AddExistingItems { get; set; }
        public RelayCommand AddNewFolder { get; set; }
        public RelayCommand Cut { get; set; }
        public RelayCommand Copy { get; set; }
        public RelayCommand Paste { get; set; }
        public RelayCommand CopyFullPath { get; set; }
        public RelayCommand OpenWithResourceManagement { get; set; }
        public RelayCommand ExcludeFromProject { get; set; }
        public RelayCommand OpenWithTerminal { get; set; }
        public RelayCommand Delete { get; set; }
        public RelayCommand Attribute { get; set; }
        #endregion

        /// <summary>
        /// 记录剪切状态
        /// </summary>
        bool IsCuted = false;
        /// <summary>
        /// 被剪切的节点
        /// </summary>
        TreeViewItem BeCopyOrCutNode = null;
        /// <summary>
        /// 解决方案视图被选中的成员
        /// </summary>
        TreeViewItem SolutionViewSelectedItem { get; set; } = null;

        #region 画刷
        private SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        private SolidColorBrush transparentBrush = new((Color)ColorConverter.ConvertFromString("Transparent"));
        #endregion

        public EditPageDataContext()
        {
            #region 链接指令
            AddNewItem = new RelayCommand(AddItemCommand);
            AddExistingItems = new RelayCommand(AddExistingItemsCommand);
            AddNewFolder = new RelayCommand(AddFolderCommand);
            Cut = new RelayCommand(CutCommand);
            Copy = new RelayCommand(CopyCommand);
            Paste = new RelayCommand(PasteCommand);
            CopyFullPath = new RelayCommand(CopyFullPathCommand);
            OpenWithResourceManagement = new RelayCommand(OpenWithResourceManagementCommand);
            ExcludeFromProject = new RelayCommand(ExcludeFromProjectCommand);
            OpenWithTerminal = new RelayCommand(OpenWithTerminalCommand);
            Delete = new RelayCommand(DeleteCommand);
            Attribute = new RelayCommand(AttributeCommand);
            #endregion
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InitDataLoaded(object sender, RoutedEventArgs e)
        {
            #region 文本编辑区
            RichTabItems richTabItem = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Uid = "DemonstrationPage",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                Header = "欢迎使用",
                IsContentSaved = true
            };
            FunctionModifyTabItems.Add(richTabItem);
            FlowDocument document = richTabItem.FindParent<Page>().FindResource("WelcomeDocument") as FlowDocument;
            RichTextBox richTextBox = new()
            {
                Document = document,
                IsReadOnly = true,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"))
            };
            richTabItem.Content = richTextBox;
            #endregion
            #region 数据包管理器树
            TreeViewItem item = new();
            DatapackTreeViewItems.Add(item);
            DatapackTreeViewItems.Remove(item);
            #endregion
        }

        /// <summary>
        /// 左侧编辑区选中标签页更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FunctionModifyTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FunctionModifyTabControlVisibility = FunctionModifyTabItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 解决方案视图选中成员更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SolutionViewer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SolutionViewSelectedItem = (sender as TreeView).SelectedItem as TreeViewItem;
        }

        /// <summary>
        /// 新建项
        /// </summary>
        private void AddItemCommand()
        {
            if(Directory.Exists(SolutionViewSelectedItem.Uid))
            {
                AddFileForm addFileForm = new();
                AddFileFormDataContext context = addFileForm.DataContext as AddFileFormDataContext;
                DatapackTreeItems header = SolutionViewSelectedItem.Header as DatapackTreeItems;
                TreeViewItem root = addFileForm.FileTypeViewer.Items[0] as TreeViewItem;
                foreach (TreeViewItem item in root.Items)
                {
                    if ((item.Uid.Contains('\\') && item.Uid.Contains(header.HeadText.Text)) || (!item.Uid.Contains('\\') && item.Uid == header.HeadText.Text))
                    {
                        context.DefaultSelectedNewFile = "new " + (header.HeadText.Text.EndsWith('s')? header.HeadText.Text[0..(header.HeadText.Text.Length - 1)] : header.HeadText.Text);
                        break;
                    }
                }
                if (addFileForm.ShowDialog().Value)
                {
                    if(File.Exists(context.SelectedNewFile.Path) && Directory.Exists(SolutionViewSelectedItem.Uid))
                    {
                        File.Copy(context.SelectedNewFile.Path,SolutionViewSelectedItem.Uid + "\\" + context.NewFileName);
                        DatapackTreeItems datapackTreeItems = new();
                        datapackTreeItems.HeadText.Text = context.NewFileName;
                        datapackTreeItems.DatapackMarker.Visibility = Visibility.Collapsed;
                        datapackTreeItems.Icon.Visibility = Visibility.Visible;
                        TreeViewItem newViewItem = new()
                        {
                            Header = datapackTreeItems,
                            Uid = SolutionViewSelectedItem.Uid + "\\" + context.NewFileName
                        };
                        newViewItem.MouseDoubleClick += DoubleClickAnalysisAndOpen;
                        SolutionViewSelectedItem.Items.Add(newViewItem);
                    }
                }
            }
        }

        /// <summary>
        /// 现有项
        /// </summary>
        private void AddExistingItemsCommand()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Filter = "Json文件|*.json;|Mcfunction文件|*.mcfunction;|Mcmeta文件|*.mcmeta;",
                DefaultExt = ".json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Multiselect = true,
                RestoreDirectory = true,
                Title = "添加现有项"
            };
            if(openFileDialog.ShowDialog().Value && Directory.Exists(SolutionViewSelectedItem.Uid))
            {
                string extension;
                foreach (string item in openFileDialog.FileNames)
                {
                    if(File.Exists(item))
                    {
                        DatapackTreeItems datapackTreeItems = new();
                        datapackTreeItems.DatapackMarker.Visibility = Visibility.Collapsed;
                        datapackTreeItems.Icon.Visibility = Visibility.Visible;
                        extension = Path.GetFileNameWithoutExtension(item);
                        if (Application.Current.Resources[extension] is DrawingImage drawingImage)
                            datapackTreeItems.Icon.Source = drawingImage;
                        TreeViewItem newItem = new()
                        {
                            Uid = SolutionViewSelectedItem.Uid + "\\" + Path.GetFileName(item),
                            Header = datapackTreeItems
                        };
                        SolutionViewSelectedItem.Items.Add(newItem);
                    };
                }
            }
        }

        /// <summary>
        /// 添加文件夹
        /// </summary>
        private void AddFolderCommand()
        {
            if(Directory.Exists(SolutionViewSelectedItem.Uid))
            {
                string newUid = SolutionViewSelectedItem.Uid + (SolutionViewSelectedItem.Uid.EndsWith('\\') ? "" : "\\") + "新文件夹";
                DatapackTreeItems datapackTreeItems = new();
                datapackTreeItems.DatapackMarker.Visibility = Visibility.Collapsed;
                datapackTreeItems.Icon.Visibility = Visibility.Visible;
                datapackTreeItems.Icon.Source = Application.Current.Resources["FolderClosed"] as ImageSource;
                datapackTreeItems.HeadText.Visibility = Visibility.Collapsed;
                datapackTreeItems.FileNameEditor.Visibility = Visibility.Visible;
                datapackTreeItems.FileNameEditor.Focus();
                datapackTreeItems.FileNameEditor.Text = datapackTreeItems.HeadText.Text = "新文件夹";
                TreeViewItem folderItem = new()
                {
                    Uid = newUid,
                    Header = datapackTreeItems
                };
                folderItem.MouseDoubleClick += DoubleClickAnalysisAndOpen;
                SolutionViewSelectedItem.Items.Add(folderItem);
            }
        }

        /// <summary>
        /// 剪切
        /// </summary>
        private void CutCommand()
        {
            Clipboard.SetText(SolutionViewSelectedItem.Uid);
            BeCopyOrCutNode = SolutionViewSelectedItem;
            IsCuted = true;
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        private void CopyCommand()
        {
            Clipboard.SetText(SolutionViewSelectedItem.Uid);
            BeCopyOrCutNode = SolutionViewSelectedItem;
        }

        /// <summary>
        /// 粘贴对象
        /// </summary>
        private void PasteCommand()
        {
            string path = Clipboard.GetText();
            TreeViewItem currentParent;
            if (!Directory.Exists(SolutionViewSelectedItem.Uid) && File.Exists(SolutionViewSelectedItem.Uid) && SolutionViewSelectedItem.Parent is TreeViewItem)
                currentParent = SolutionViewSelectedItem.Parent as TreeViewItem;
            else
                currentParent = SolutionViewSelectedItem;
            if ((Directory.Exists(path) || File.Exists(path)) && path != currentParent.Uid)
            {
                if (IsCuted)
                {
                    //移动被剪切的节点到当前节点的子级
                    if(BeCopyOrCutNode != null && BeCopyOrCutNode.Parent != null)
                    {
                        #region 操作被剪切的节点
                        if (BeCopyOrCutNode.Parent is TreeViewItem beCutParent)
                            beCutParent.Items.Remove(BeCopyOrCutNode);
                        else
                        {
                            TreeView parent = BeCopyOrCutNode.Parent as TreeView;
                            parent.Items.Remove(BeCopyOrCutNode);
                        }
                        currentParent.Items.Add(BeCopyOrCutNode);
                        #endregion

                        //更新本地磁盘的文件
                        File.Move(path,currentParent.Uid + Path.GetFileName(path));

                        #region 更新编辑区文件的UID路径数据
                        foreach (RichTabItems tab in FunctionModifyTabItems)
                        {
                            if (tab.Uid == path)
                            {
                                if (!Directory.Exists(SolutionViewSelectedItem.Uid))
                                    Directory.CreateDirectory(SolutionViewSelectedItem.Uid);
                                tab.Uid = SolutionViewSelectedItem.Uid + "\\" + tab.Header.ToString();
                                break;
                            }
                        }
                        #endregion
                    }
                    IsCuted = false;
                }
                else
                {
                    TreeViewItem treeViewItem = new()
                    {
                        Uid = path,
                        Header = BeCopyOrCutNode.Header
                    };
                    SolutionViewSelectedItem.Items.Add(treeViewItem);
                    //复制本地磁盘的文件
                    File.Copy(path, currentParent.Uid + Path.GetFileName(path));
                }
            }
        }

        /// <summary>
        /// 复制完整路径
        /// </summary>
        private void CopyFullPathCommand()
        {
            Clipboard.SetText(SolutionViewSelectedItem.Uid);
        }

        /// <summary>
        /// 从资源管理器打开
        /// </summary>
        private void OpenWithResourceManagementCommand()
        {
            if (File.Exists(SolutionViewSelectedItem.Uid))
                OpenFolderThenSelectFiles.ExplorerFile(SolutionViewSelectedItem.Uid);
            else
                Process.Start("explorer.exe",SolutionViewSelectedItem.Uid);
        }

        /// <summary>
        /// 从项目中排除
        /// </summary>
        private void ExcludeFromProjectCommand()
        {
            #region 编辑区对应标签页改为未保存
            foreach (RichTabItems tab in FunctionModifyTabItems)
            {
                if (tab.Uid == SolutionViewSelectedItem.Uid)
                {
                    tab.IsContentSaved = false;
                    break;
                }
            }
            #endregion
            if (SolutionViewSelectedItem.Parent != null)
            {
                TreeViewItem parent = SolutionViewSelectedItem.Parent as TreeViewItem;
                parent.Items.Remove(SolutionViewSelectedItem);
            }
            else
            {
                TreeView parent = SolutionViewSelectedItem.Parent as TreeView;
                parent.Items.Remove(SolutionViewSelectedItem);
            }
        }

        /// <summary>
        /// 在终端打开
        /// </summary>
        private void OpenWithTerminalCommand()
        {
            //TemplateItems templateItems = TreeViewItem.Header as TemplateItems;
            //if(Directory.Exists(templateItems.Uid))
            //Process.Start(@"explorer.exe", "cd " + templateItems.Uid);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        private void DeleteCommand()
        {
            #region 删除文件或文件夹
            if (Directory.Exists(SolutionViewSelectedItem.Uid))
                Directory.Delete(SolutionViewSelectedItem.Uid, true);
            else
                if(File.Exists(SolutionViewSelectedItem.Uid))
                File.Delete(SolutionViewSelectedItem.Uid);
            #endregion
            //删除右侧树视图中对应的节点
            ExcludeFromProjectCommand();
        }

        /// <summary>
        /// 查看属性
        /// </summary>
        private void AttributeCommand()
        {

        }

        /// <summary>
        /// 双击分析文件并打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void DoubleClickAnalysisAndOpen(object sender, MouseButtonEventArgs e)
        {
            await DoubleClickAnalysisAndOpenAsync(sender as TreeViewItem);
        }

        /// <summary>
        /// 异步执行文件分析与打开任务
        /// </summary>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        private async Task DoubleClickAnalysisAndOpenAsync(TreeViewItem currentItem)
        {
            Datapack datapack = Window.GetWindow(currentItem) as Datapack;
            await datapack.Dispatcher.InvokeAsync(() =>
            {
                string fileContent = File.ReadAllText(currentItem.Uid);
                RichTabItems item = new()
                {
                    Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                    Uid = currentItem.Uid,
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                    Header = Path.GetFileName(currentItem.Uid),
                    IsContentSaved = true
                };
                TextEditor textEditor = new()
                {
                    ShowLineNumbers = true,
                    Background = transparentBrush,
                    Foreground = whiteBrush,
                    BorderThickness = new Thickness(0),
                    WordWrap = true,
                    Text = fileContent,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                textEditor.TextChanged += TextEditor_TextChanged;
                textEditor.KeyDown += TextEditor_KeyDown;
                item.Content = textEditor;
                FunctionModifyTabItems.Add(item);
                SelectedFileItem = item;
            });
        }

        /// <summary>
        /// 检测快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            TextEditor textEditor = sender as TextEditor;
            RichTabItems parent = textEditor.Parent as RichTabItems;
            Datapack datapack = Window.GetWindow(parent) as Datapack;
            #region 保存
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                await datapack.Dispatcher.InvokeAsync(() =>
                {
                    string folder = Path.GetDirectoryName(parent.Uid);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    _ = File.WriteAllTextAsync(parent.Uid, textEditor.Text);
                    parent.IsContentSaved = true;
                });
            }
            #endregion
        }

        /// <summary>
        /// 文件编辑事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_TextChanged(object sender, System.EventArgs e)
        {
            TextEditor textEditor = sender as TextEditor;
            RichTabItems parent = textEditor.Parent as RichTabItems;
            parent.IsContentSaved = false;
        }
    }
}

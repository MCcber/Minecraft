using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.Generators.DataPackGenerator.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.AvalonEdit;
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

        #region 当前选中的文本编辑器
        private RichTabItems selectedFileItem = null;
        public RichTabItems SelectedFileItem
        {
            get => selectedFileItem;
            set => SetProperty(ref selectedFileItem, value);
        }
        #endregion

        #region 数据包管理器树引用
        TreeView ContentView = null;
        #endregion

        #region 数据包管理器右键菜单
        public RelayCommand AddFile { get; set; }
        public RelayCommand AddFolder { get; set; }
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
        RichTreeViewItems BeCutNode = null;

        #region 画刷
        private SolidColorBrush whiteBrush = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        private SolidColorBrush transparentBrush = new((Color)ColorConverter.ConvertFromString("Transparent"));
        #endregion

        public EditPageDataContext()
        {
            #region 链接指令
            AddFile = new RelayCommand(AddFileCommand);
            AddFolder = new RelayCommand(AddFolderCommand);
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
            RichTreeViewItems item = new();
            DatapackTreeViewItems.Add(item);
            ContentView = item.FindParent<TreeView>();
            DatapackTreeViewItems.Remove(item);
            #endregion
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        private void AddFileCommand()
        {
        }

        /// <summary>
        /// 添加文件夹
        /// </summary>
        private void AddFolderCommand()
        {

        }

        /// <summary>
        /// 剪切
        /// </summary>
        private void CutCommand()
        {
            RichTreeViewItems richTreeViewItems = ContentView.SelectedItem as RichTreeViewItems;
            Clipboard.SetText(richTreeViewItems.Uid);
            IsCuted = true;
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        private void CopyCommand()
        {
            RichTreeViewItems richTreeViewItems = ContentView.SelectedItem as RichTreeViewItems;
            Clipboard.SetText(richTreeViewItems.Uid);
        }

        /// <summary>
        /// 粘贴对象
        /// </summary>
        private void PasteCommand()
        {
            string path = Clipboard.GetText();
            RichTreeViewItems selectedItem = ContentView.SelectedItem as RichTreeViewItems;
            if (!Directory.Exists(selectedItem.Uid) && File.Exists(selectedItem.Uid) && selectedItem.Parent != null)
                selectedItem = selectedItem.Parent as RichTreeViewItems;
            if (Directory.Exists(path) || File.Exists(path))
            {
                if (IsCuted)
                {
                    //移动被剪切的节点到当前节点的子级
                    if(BeCutNode != null && BeCutNode.Parent != null)
                    {
                        if (BeCutNode.Parent is RichTreeViewItems beCutParent)
                            beCutParent.Items.Remove(BeCutNode);
                        else
                            ContentView.Items.Remove(BeCutNode);
                        selectedItem.Items.Add(BeCutNode);

                        foreach (RichTabItems tab in FunctionModifyTabItems)
                        {
                            if (tab.Uid == path)
                            {
                                if (!Directory.Exists(selectedItem.Uid))
                                    Directory.CreateDirectory(selectedItem.Uid);
                                tab.Uid = selectedItem.Uid + "\\" + tab.Header.ToString();
                                break;
                            }
                        }
                    }
                    IsCuted = false;
                }
                else
                {
                    RichTreeViewItems richTreeViewItems = new RichTreeViewItems()
                    {
                        Uid = path,
                        Tag = ContentReader.ContentType.UnKnown
                    };
                    ContentReader.ContentType contentType = ContentReader.ContentType.UnKnown;
                }
            }
        }

        /// <summary>
        /// 复制完整路径
        /// </summary>
        private void CopyFullPathCommand()
        {
            RichTreeViewItems richTreeViewItems = ContentView.SelectedItem as RichTreeViewItems;
        }

        /// <summary>
        /// 从资源管理器打开
        /// </summary>
        private void OpenWithResourceManagementCommand()
        {
            RichTreeViewItems richTreeViewItems = ContentView.SelectedItem as RichTreeViewItems;
        }

        /// <summary>
        /// 从项目中排除
        /// </summary>
        private void ExcludeFromProjectCommand()
        {
            RichTreeViewItems richTreeViewItems = ContentView.SelectedItem as RichTreeViewItems;
            #region 编辑区对应标签页改为未保存
            foreach (RichTabItems tab in FunctionModifyTabItems)
            {
                if (tab.Uid == richTreeViewItems.Uid)
                {
                    tab.IsContentSaved = false;
                    break;
                }
            }
            #endregion
            if (richTreeViewItems.Parent != null)
            {
                RichTreeViewItems parent = richTreeViewItems.Parent as RichTreeViewItems;
                parent.Items.Remove(richTreeViewItems);
            }
            else
            {
                ContentView.Items.Remove(ContentView.SelectedItem);
            }
        }

        /// <summary>
        /// 在终端打开
        /// </summary>
        private void OpenWithTerminalCommand()
        {
            RichTreeViewItems richTreeViewItems = ContentView.SelectedItem as RichTreeViewItems;
            //TemplateItems templateItems = richTreeViewItems.Header as TemplateItems;
            //if(Directory.Exists(templateItems.Uid))
            //Process.Start(@"explorer.exe", "cd " + templateItems.Uid);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        private void DeleteCommand()
        {
            RichTreeViewItems richTreeViewItems = ContentView.SelectedItem as RichTreeViewItems;
            //TemplateItems templateItems = richTreeViewItems.Header as TemplateItems;
            #region 删除文件
            if (Directory.Exists(richTreeViewItems.Uid))
                Directory.Delete(richTreeViewItems.Uid, true);
            else
                if(File.Exists(richTreeViewItems.Uid))
                File.Delete(richTreeViewItems.Uid);
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

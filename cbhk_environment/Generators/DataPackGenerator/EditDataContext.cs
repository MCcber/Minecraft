using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.Generators.DataPackGenerator.Components;
using cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace cbhk_environment.Generators.DataPackGenerator
{
    public class EditDataContext : ObservableObject
    {
        //获取文本编辑区的引用
        public static TabControl FileModifyZone = null;

        //获取内容视图引用
        public static TreeView ContentView = null;

        #region 数据包搜索文本框内容
        private string datapackSeacherValue = "";
        public string DatapackSeacherValue
        {
            get
            {
                return datapackSeacherValue;
            }
            set
            {
                datapackSeacherValue = value;
            }
        }
        #endregion

        #region 当前数据包版本
        public ObservableCollection<string> AddFileSearchFileTypeSource { get; set; } = new ObservableCollection<string> { };
        #endregion

        /// <summary>
        /// 文件模板成员列表
        /// </summary>
        //public ObservableCollection<TemplateItems> TemplateItemList { get; set; } = new ObservableCollection<TemplateItems> { };

        /// <summary>
        /// 左侧树视图模板类型成员
        /// </summary>
        public static ObservableCollection<RichTreeViewItems> TemplateTypeItemList { get; set; } = new ObservableCollection<RichTreeViewItems> { };

        #region 生成与返回
        public RelayCommand Run { get; set; }

        public RelayCommand<FrameworkElement> Return { get; set; }
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

        #region 文件添加窗体.文件名称
        private static string newFileName = "File1.json";
        public string NewFileName
        {
            get
            {
                return newFileName;
            }
            set
            {
                newFileName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 确定/取消添加文件
        private bool addFileFormResult = false;
        public bool AddFileFormResult
        {
            get
            {
                return addFileFormResult;
            }
            set
            {
                addFileFormResult = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<Window> SureToAddFile { get; set; }
        public RelayCommand<Window> CancelToAddFile { get; set; }
        #endregion

        #region 新建文件窗体右侧文件功能类型和描述
        private string rightSideFileType = "";
        public string RightSideFileType
        {
            get
            {
                return rightSideFileType.TrimStart('.');
            }
            set
            {
                rightSideFileType = value;
                OnPropertyChanged();
            }
        }

        private string rightSideFileDescription = "";
        public string RightSideFileDescription
        {
            get
            {
                return rightSideFileDescription.TrimStart('.');
            }
            set
            {
                rightSideFileDescription = value;
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// 记录剪切状态
        /// </summary>
        bool IsCuted = false;
        /// <summary>
        /// 被剪切的节点
        /// </summary>
        RichTreeViewItems BeCutNode = null;

        public EditDataContext()
        {
            #region 链接指令
            //RunCommand = new RelayCommand(run_command);
            //ReturnCommand = new RelayCommand<FrameworkElement>(return_command);

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

            SureToAddFile = new RelayCommand<Window>(SureToAddFileCommand);
            CancelToAddFile = new RelayCommand<Window>(CancelToAddFileCommand);
            #endregion
        }

        #region 添加文件窗体逻辑

        /// <summary>
        /// 添加文件窗体版本载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddFileFormFileTypeLoaded(object sender,RoutedEventArgs e)
        {
            AddFileSearchFileTypeSource.Add(".json");
            AddFileSearchFileTypeSource.Add(".mcfunction");
        }

        /// <summary>
        /// 确定添加文件
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void SureToAddFileCommand(Window win)
        {
            win.DialogResult = true;
        }

        /// <summary>
        /// 取消添加文件
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CancelToAddFileCommand(Window win)
        {
            win.DialogResult = false;
        }

        /// <summary>
        /// 关闭添加文件窗体
        /// </summary>
        /// <param name="win"></param>
        public void ClosingAddFileForm(Window win)
        {
            win.DialogResult = false;
        }
        #endregion

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

                        foreach (RichTabItems tab in FileModifyZone.Items)
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
            foreach (RichTabItems tab in FileModifyZone.Items)
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
        /// 获取文本编辑区的引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FileModifyZoneLoaded(object sender, RoutedEventArgs e)
        {
            FileModifyZone = sender as TabControl;
        }

        /// <summary>
        /// 获取内容树视图引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ContentViewLoaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(FrameworkElement obj)
        {
            Window win = Window.GetWindow(obj);
            DataPack.cbhk.Topmost = true;
            DataPack.cbhk.WindowState = WindowState.Normal;
            DataPack.cbhk.Show();
            DataPack.cbhk.Topmost = false;
            DataPack.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 执行生成
        /// </summary>
        private void run_command()
        {
            RichTabItems CurrentItem = FileModifyZone.SelectedItem as RichTabItems;
            RichTextBox CurrentTextBox = CurrentItem.Content as RichTextBox;
            TextRange CurrentContent = new(CurrentTextBox.Document.ContentStart, CurrentTextBox.Document.ContentEnd);
            _ = File.WriteAllTextAsync(CurrentItem.Uid, CurrentContent.Text);
        }
    }
}

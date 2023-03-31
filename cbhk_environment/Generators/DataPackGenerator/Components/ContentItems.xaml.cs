using cbhk_environment.CustomControls;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace cbhk_environment.Generators.DataPackGenerator.Components
{
    /// <summary>
    /// ContentItems.xaml 的交互逻辑
    /// </summary>
    public partial class ContentItems : UserControl
    {
        /// <summary>
        /// 保存文件原来的名字
        /// </summary>
        private string originalName = "";

        /// <summary>
        /// 文件类型
        /// </summary>
        public ContentReader.ContentType FileType { get; set; } = ContentReader.ContentType.UnKnown;

        /// <summary>
        /// 是否为数据包
        /// </summary>
        public bool IsDataPack = false;

        /// <summary>
        /// 存储该节点的数据包父节点
        /// </summary>
        public RichTreeViewItems DataPackItemReference = null;
        public ContentReader.DataPackMetaStruct DataPackMetaInfo = new ContentReader.DataPackMetaStruct();

        public ContentItems(string FilePath, ContentReader.ContentType type)
        {
            InitializeComponent();

            FileType = type;
            Uid = FilePath;
            DisplayFileName.Text = Path.GetFileName(FilePath);

            if (FilePath != null && File.Exists(FilePath))
            {
                string extension = Path.GetExtension(FilePath);
                var keys = datapack_datacontext.IconDictionary.Keys;
                foreach (var key in keys)
                {
                    if (key != null && key.ToString() == extension)
                    {
                        if (Application.Current.TryFindResource(key) is DrawingImage icon)
                            FileTypeIcon.Source = icon;
                        break;
                    }
                }
            }
            else
            if (FilePath != null && Directory.Exists(FilePath))
            {
                string iconKey = "folder_closed";
                if (type == ContentReader.ContentType.DataPack)
                    iconKey = "datapack";

                var keys = datapack_datacontext.IconDictionary.Keys;
                foreach (var key in keys)
                {
                    if (key != null && key.ToString() == iconKey && iconKey != "")
                    {
                        if (Application.Current.TryFindResource(key) is DrawingImage icon)
                            FileTypeIcon.Source = icon;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 开始编辑文件名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyFileNameClick(object sender, RoutedEventArgs e)
        {
            originalName = FileName.Text = DisplayFileName.Text;
            FileName.Focus();
            FileName.SelectAll();

            FileName.Visibility = Visibility.Visible;
            DisplayFileName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 文件名修改完毕,根据UID中的路径修改对应的实体文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyCompletedKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && originalName != FileName.Text)
            {
                if (File.Exists(Uid))
                {
                    //string extensionName = Path.GetExtension(Uid);
                    string pathName = Path.GetDirectoryName(Uid);
                    File.Move(pathName + "\\" + originalName /*+ extensionName*/, pathName + "\\" + FileName.Text /*+ extensionName*/);
                    Uid = pathName + "\\" + FileName.Text;
                }
                else
                    if (Directory.Exists(Uid))
                {
                    DirectoryInfo folderNameInfo = new DirectoryInfo(Uid);
                    string folderName = folderNameInfo.Parent.FullName;
                    Directory.Move(Uid, folderName + "\\" + FileName.Text);
                    Uid = folderName + "\\" + FileName.Text;
                }
                else
                    FileName.Text = originalName;

                DisplayFileName.Text = FileName.Text;
                FileName.Visibility = Visibility.Collapsed;
                DisplayFileName.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 删除该文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            RichTreeViewItems parent = Parent as RichTreeViewItems;
            if (parent.Parent is RichTreeViewItems)
            {
                RichTreeViewItems grand_parent = parent.Parent as RichTreeViewItems;
                grand_parent.Items.Remove(parent);
            }
            else
                if (parent.Parent is TreeView)
            {
                TreeView grand_parent = parent.Parent as TreeView;
                grand_parent.Items.Remove(parent);
            }
        }
    }
}
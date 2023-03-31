using cbhk_environment.CustomControls;
using cbhk_environment.Generators.DataPackGenerator.Components;
using cbhk_environment.Generators.WrittenBookGenerator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WK.Libraries.BetterFolderBrowserNS;

namespace cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms
{
    /// <summary>
    /// 属性设置窗体逻辑处理
    /// </summary>
    public class DatapackGenerateSetupDataContext: ObservableObject
    {
        #region 属性设置窗体:上一步、下一步和设置路径等指令
        public RelayCommand<Page> AttributeLastStep { get; set; }
        public RelayCommand<Page> AttributeNextStep { get; set; }
        public RelayCommand SetDatapackPath { get; set; }
        public RelayCommand SetDatapackDescription { get; set; }
        public RelayCommand AddDatapackFilter { get; set; }
        public RelayCommand ClearDatapackFilter { get; set; }
        public RelayCommand CopyDatapackName { get; set; }
        public RelayCommand CopyDatapackPath { get; set; }
        public RelayCommand CopyDatapackDescription { get; set; }
        public RelayCommand CopyDatapackMainNameSpace { get; set; }
        public RelayCommand<TextCheckBoxs> SynchronizePackAndNameSpaceName { get; set; }
        #endregion

        #region 存储数据包的名称
        private string datapackName = "Datapack";
        public string DatapackName
        {
            get { return datapackName; }
            set
            {
                datapackName = value;

                if (datapackName.Trim() == "")
                    DatapackNameIsNull = Visibility.Visible;
                else
                    DatapackNameIsNull = Visibility.Hidden;

                OnPropertyChanged();
            }
        }
        #endregion

        #region 存储数据包的主命名空间名称
        private string datapackMainNameSpace = "Datapack";
        public string DatapackMainNameSpace
        {
            get { return datapackMainNameSpace; }
            set
            {
                datapackMainNameSpace = value;

                if (datapackMainNameSpace.Trim() == "")
                    DatapackNameIsNull = Visibility.Visible;
                else
                    DatapackNameIsNull = Visibility.Hidden;

                OnPropertyChanged();
            }
        }
        #endregion

        #region 存储数据包的保存路径
        private string selectedDatapackPath = "";
        public string SelectedDatapackPath
        {
            get { return selectedDatapackPath; }
            set
            {
                selectedDatapackPath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 存储数据包对应的游戏版本
        private string selectedDatapackVersion = "";
        public string SelectedDatapackVersion
        {
            get { return selectedDatapackVersion; }
            set
            {
                selectedDatapackVersion = value;
                OnPropertyChanged();
                if (selectedDatapackVersion != null)
                    SelectedDatapackVersionString = selectedDatapackVersion;
            }
        }
        private string SelectedDatapackVersionString = "";
        #endregion

        #region 数据包名称为空时的提示可见性
        private Visibility datapackNameIsNull = Visibility.Hidden;
        public Visibility DatapackNameIsNull
        {
            get { return datapackNameIsNull; }
            set
            {
                datapackNameIsNull = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 数据包命名空间为空时的提示可见性
        private Visibility datapackMainNameSpaceIsNull = Visibility.Hidden;
        public Visibility DatapackMainNameSpaceIsNull
        {
            get { return datapackMainNameSpaceIsNull; }
            set
            {
                datapackMainNameSpaceIsNull = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 命名空间可操作性
        private bool canModifyNameSpace = true;
        public bool CanModifyNameSpace
        {
            get { return canModifyNameSpace; }
            set
            {
                canModifyNameSpace = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 存储数据包的描述类型
        private string selectedDatapackDescriptionType = "";
        public string SelectedDatapackDescriptionType
        {
            get { return selectedDatapackDescriptionType; }
            set
            {
                selectedDatapackDescriptionType = value;
                OnPropertyChanged();

                #region 同步设置不同数据类型的控件可见性
                foreach (FrameworkElement item in DescriptionContainer.Children)
                {
                    if (item.Uid == selectedDatapackDescriptionType || item.Uid.Contains(selectedDatapackDescriptionType))
                        item.Visibility = Visibility.Visible;
                    else
                        if (item.Uid != "")
                        item.Visibility = Visibility.Collapsed;
                }
                #endregion
            }
        }
        #endregion

        #region 存储数据包的描述
        private string selectedDatapackDescription = "This is a Datapack";
        public string SelectedDatapackDescription
        {
            get { return selectedDatapackDescription; }
            set
            {
                selectedDatapackDescription = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 存储数据包JSON组件类型的描述数据
        private string jsonObjectDescription = "";
        public string JsonObjectDescription
        {
            get { return jsonObjectDescription; }
            set
            {
                jsonObjectDescription = value;
            }
        }
        private string jsonArrayDescription = "";
        private string JsonArrayDescription
        {
            get { return jsonArrayDescription; }
            set
            {
                jsonArrayDescription = value;
            }
        }
        #endregion

        #region 存储数据包的过滤器
        public static ObservableCollection<FilterItems> DatapackFilterSource { get; set; } = new ObservableCollection<FilterItems> { };
        private string datapackFilter = "";
        public string DatapackFilter
        {
            get { return datapackFilter; }
            set
            {
                datapackFilter = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 存储数据包简介类型键入控件父级容器、布尔类型、切换控件的引用
        ComboBox DescriptionBoolBox = null;
        DockPanel DescriptionContainer = null;
        ComboBox DescriptionTypeSwitcher = null;
        #endregion

        #region 版本、生成路径、描述等数据
        public ObservableCollection<string> VersionList { get; set; } = new ObservableCollection<string> { };
        public ObservableCollection<string> GeneratorPathList { get; set; } = new ObservableCollection<string> { };

        string DatapackGeneratorFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\generatorPathes.ini";

        //数据包简介数据类型配置文件路径
        string DatapackDescriptionTypeFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\descriptionTypeList.ini";

        //用于显示数据包简介的文档对象
        List<EnabledFlowDocument> DescriptionDisplayDocument = null;
        #endregion

        #region 实例化
        public DatapackGenerateSetupDataContext()
        {
            AttributeLastStep = new RelayCommand<Page>(AttributeLastStepCommand);
            AttributeNextStep = new RelayCommand<Page>(AttributeNextStepCommand);
            SetDatapackPath = new RelayCommand(SetDatapackPathCommand);
            SetDatapackDescription = new RelayCommand(SetDatapackDescriptionCommand);
            AddDatapackFilter = new RelayCommand(AddDatapackFilterCommand);
            ClearDatapackFilter = new RelayCommand(ClearDatapackFilterCommand);
            CopyDatapackName = new RelayCommand(CopyDatapackNameCommand);
            CopyDatapackPath = new RelayCommand(CopyDatapackPathCommand);
            CopyDatapackDescription = new RelayCommand(CopyDatapackDescriptionCommand);
            CopyDatapackMainNameSpace = new RelayCommand(CopyDatapackMainNameSpaceCommand);
            SynchronizePackAndNameSpaceName = new RelayCommand<TextCheckBoxs>(SynchronizePackAndNameSpaceNameCommand);
        }
        #endregion

        /// <summary>
        /// 数据包生成路径载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DatapackGeneratorPathLoaded(object sender, RoutedEventArgs e)
        {
            string[] files = File.ReadAllLines(DatapackGeneratorFilePath);
            foreach (string item in files)
            {
                GeneratorPathList.Add(item);
            }
        }

        /// <summary>
        /// 初始化数据包简介数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DatapackDescriptionTypeLoaded(object sender, RoutedEventArgs e)
        {
            DescriptionTypeSwitcher = sender as ComboBox;

            if (File.Exists(DatapackDescriptionTypeFilePath) && DescriptionTypeSwitcher.ItemsSource == null)
            {
                ObservableCollection<string> descriptionList = new ObservableCollection<string> { };
                DescriptionTypeSwitcher.ItemsSource = descriptionList;

                #region 解析简介类型配置文件
                string[] DescriptionList = File.ReadAllLines(DatapackDescriptionTypeFilePath);

                foreach (string descriptionItem in DescriptionList)
                {
                    descriptionList.Add(descriptionItem);
                }
                #endregion
            }
        }

        /// <summary>
        /// 初始化数据包所对应游戏版本数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DatapackVersionLoaded(object sender, RoutedEventArgs e)
        {
            if (VersionList.Count == 0)
            {
                #region 设置版本配置文件
                foreach (var versionItem in datapack_datacontext.DatapackVersionDatabase)
                {
                    VersionList.Add(versionItem.Key);
                }
                if (VersionList.Count > 0)
                    SelectedDatapackVersion = VersionList[0];
                #endregion
            }
        }

        /// <summary>
        /// 初始化数据包简介父级容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DatapackDescriptionContainerLoaded(object sender, RoutedEventArgs e)
        {
            DescriptionContainer = sender as DockPanel;
        }

        /// <summary>
        /// 初始化数据包简介布尔类数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DatapackDescriptionBoolTypeLoaded(object sender, RoutedEventArgs e)
        {
            //获取引用
            DescriptionBoolBox = sender as ComboBox;

            if(DescriptionBoolBox.Items.Count == 0)
            {
                DescriptionBoolBox.Items.Add("true");
                DescriptionBoolBox.Items.Add("false");
            }
        }

        /// <summary>
        /// 初始化已选中的所有模板的标签类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TypeTagListLoaded(object sender, RoutedEventArgs e)
        {
            DockPanel container = sender as DockPanel;
            bool IsFirst = true;

            if(container.Children.Count == 0)
            foreach (string SelectedTemplateTypeTag in TemplateSelectDataContext.SelectedTemplateTypeTagList)
            {
                TemplateTypeTag templateTypeTag = new TemplateTypeTag(SelectedTemplateTypeTag);
                if (IsFirst)
                    templateTypeTag.Margin = new Thickness(0);
                IsFirst = false;
                container.Children.Add(templateTypeTag);
            }
        }

        /// <summary>
        /// 添加一个数据包过滤器成员
        /// </summary>
        private void AddDatapackFilterCommand()
        {
            FilterItems filterItems = new FilterItems();
            DatapackFilterSource.Add(filterItems);
        }

        /// <summary>
        /// 清空数据包的过滤器
        /// </summary>
        private void ClearDatapackFilterCommand()
        {
            DatapackFilterSource.Clear();
        }

        /// <summary>
        /// 设置数据包的描述
        /// </summary>
        private void SetDatapackDescriptionCommand()
        {
            //实例化一个成书生成器作为JSON文本编辑工具
            WrittenBook writtenBook = new WrittenBook(DataPack.cbhk, true);
            written_book_datacontext datacontext = writtenBook.DataContext as written_book_datacontext;

            if (DescriptionDisplayDocument != null)
            {
                datacontext.HistroyFlowDocumentList = DescriptionDisplayDocument;
            }

            if (writtenBook.ShowDialog() == true)
            {
                DescriptionDisplayDocument = datacontext.HistroyFlowDocumentList;
                JsonArrayDescription = datacontext.result;
                JsonObjectDescription = datacontext.object_result;
            }
        }

        /// <summary>
        /// 设置数据包的路径
        /// </summary>
        private void SetDatapackPathCommand()
        {
            BetterFolderBrowser folderBrowser = new BetterFolderBrowser()
            {
                Multiselect = false,
                Title = "请选择当前数据包生成路径",
                RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folderBrowser.SelectedPath))
                {
                    string selectedPath = folderBrowser.SelectedPath;
                    if (GeneratorPathList.Count > 0)
                    {
                        if (GeneratorPathList.Where(item => item == selectedPath).Count() == 0)
                        {
                            GeneratorPathList.Insert(0,selectedPath);
                            if (GeneratorPathList.Count > 100)
                                GeneratorPathList.Remove(GeneratorPathList.Last());
                        }
                    }
                    else
                    {
                        GeneratorPathList.Insert(0,selectedPath);
                    }
                    SelectedDatapackPath = selectedPath;
                }
            }
        }

        /// <summary>
        /// 属性设置窗体进入下一步
        /// </summary>
        private void AttributeNextStepCommand(Page page)
        {
            #region 生成路径链表保存至文件
            string generatorPathContent = string.Join("\r\n", GeneratorPathList);
            File.WriteAllText(DatapackGeneratorFilePath, generatorPathContent);
            #endregion

            #region 数据包名、主命名空间、生成路径任意一项为空则不提供生成服务
            string SelectedDatapackPathString = SelectedDatapackPath.Trim();
            if (DatapackName.Trim() == "" ||
                DatapackMainNameSpace.Trim() == "" ||
                SelectedDatapackPathString == "" ||
                !Directory.Exists(SelectedDatapackPathString))
                return;
            #endregion

            #region 合并选择的路径和数据包名
            string RootPath = SelectedDatapackPath + "\\" + DatapackName + "\\";
            //创建数据包文件夹
            Directory.CreateDirectory(RootPath);
            #endregion

            #region 作为数据包写入到最近使用的内容中
            string recentDatapackPath = RootPath.TrimEnd('\\');
            File.WriteAllText(datapack_datacontext.recentContentsFolderPath + "\\" + DatapackName + ".content", recentDatapackPath);
            #endregion

            #region 整理数据包的各种属性
            //整合过滤器数据
            if (DatapackFilterSource.Count > 0)
                DatapackFilter = ",\"filter\":{\"block\":[" + string.Join("", DatapackFilterSource.Select(item => item.FilterBlock)).TrimEnd(',') + "]}";
            //搜索对应的数据包版本号
            foreach (var item in datapack_datacontext.DatapackVersionDatabase)
            {
                if (item.Key == SelectedDatapackVersion)
                {
                    SelectedDatapackVersionString = item.Value;
                    break;
                }
            }

            //合并简介信息
            if (SelectedDatapackDescriptionType == "Object")
                SelectedDatapackDescription = JsonObjectDescription;
            else
                if (SelectedDatapackDescriptionType == "Array")
                SelectedDatapackDescription = JsonArrayDescription;
            if (SelectedDatapackDescriptionType == "Bool")
                SelectedDatapackDescription = DescriptionBoolBox.SelectedItem.ToString();

            //合并pack信息
            string pack = "\"pack\":{\"pack_format\":" + SelectedDatapackVersionString + ",\"description\":\"" + SelectedDatapackDescription + "\"}";

            //合并最终配置文件数据
            string pack_mcmeta = "{" + pack + DatapackFilter + "}";
            #endregion

            #region 根据填写的生成路径，数据包名和主命名空间来生成一个初始包
            Directory.CreateDirectory(SelectedDatapackPathString + "\\" + DatapackName + "\\data\\" + DatapackMainNameSpace);
            File.WriteAllText(SelectedDatapackPathString + "\\" + DatapackName + "\\pack.mcmeta", pack_mcmeta, System.Text.Encoding.UTF8);
            #endregion

            #region 记住并复制模板
            foreach (var selectedTemplateItem in TemplateSelectDataContext.SelectedTemplateItemList)
            {
                string fileType = selectedTemplateItem.FileType.ToLower();
                SelectedDatapackVersionString = SelectedDatapackVersion;

                //自动选取较高的版本
                if (SelectedDatapackVersionString.Contains("~"))
                {
                    string[] twoVersion = SelectedDatapackVersionString.Split('~');
                    string leftVersion = twoVersion[0].Replace(".", "");
                    string rightVersion = twoVersion[1].Replace(".", "");
                    int leftVersionValue = int.Parse(leftVersion);
                    int rightVersionValue = int.Parse(rightVersion);

                    SelectedDatapackVersionString = leftVersionValue > rightVersionValue ? twoVersion[0] : twoVersion[1];
                }

                if (File.Exists(TemplateSelectDataContext.TemplateDataFilePath + "\\" + SelectedDatapackVersionString + "\\" + selectedTemplateItem.TemplateID + fileType))
                {
                    #region 整合路径与模板数据
                    //写入当前的历史模板
                    string WriteFilePath = TemplateSelectDataContext.RecentTemplateDataFilePath + "\\" + SelectedDatapackVersionString + "\\" + selectedTemplateItem.TemplateID + ".json";

                    //整合模板文件路径
                    string FilePath = TemplateSelectDataContext.TemplateDataFilePath + "\\" + SelectedDatapackVersionString + "\\" + selectedTemplateItem.TemplateID + fileType;

                    //整合目标路径
                    string targetPath = SelectedDatapackPathString + DatapackName + "\\data\\" + DatapackMainNameSpace + "\\" + selectedTemplateItem.FileNameSpace + "\\" + selectedTemplateItem.TemplateID + fileType;

                    string targetFolderPath = Path.GetDirectoryName(targetPath);
                    Directory.CreateDirectory(targetFolderPath);

                    //整合历史模板的json数据
                    string templateJSON = "{\"TemplateName\":\"" + selectedTemplateItem.TemplateName.Text + "\",\"FileType\":\"" + fileType + "\",\"FileNameSpace\":\"" + selectedTemplateItem.FileNameSpace + "\",\"FilePath\":\"" + FilePath.Replace("\\", "\\\\") + "\",\"IconPath\":\"" + datapack_datacontext.TemplateIconFilePath.Replace("\\", "\\\\") + "\\\\" + selectedTemplateItem.TemplateID + ".png" + "\"}";
                    #endregion

                    //写入历史模板目录中
                    File.WriteAllText(WriteFilePath, templateJSON);
                    //复制到数据包生成路径中(生成路径+数据包名+数据包主命名空间+模板所属命名空间+模板ID+已选择的文件类型)
                    File.Copy(FilePath, targetPath,true);
                }
            }
            //处理完毕后清空已选择模板列表成员
            TemplateSelectDataContext.SelectedTemplateItemList.Clear();
            #endregion

            #region 属性设置窗体任务完成，跳转到编辑页
            ContentReader.DataPackMetaStruct metaStruct = new ContentReader.DataPackMetaStruct()
            {
                Description = SelectedDatapackDescription,
                //Filter = DatapackFilter,
                Version = SelectedDatapackVersion
            };
            datapack_datacontext datapackDatacontext = Window.GetWindow(page).DataContext as datapack_datacontext;
            EditPage editPage = datapackDatacontext.GetEditPage();
            RichTreeViewItems contentNodes = ContentReader.ReadTargetContent(SelectedDatapackPathString + "\\" + DatapackName, ContentReader.ContentType.DataPack, metaStruct);
            datapack_datacontext.newTreeViewItems.Add(contentNodes);
            editPage = new EditPage
            {
                DataContext = new EditDataContext()
            };
            NavigationService.GetNavigationService(page).Navigate(editPage);
            #endregion
        }

        /// <summary>
        /// 属性设置窗体进入上一步
        /// </summary>
        private void AttributeLastStepCommand(Page page)
        {
            //返回模板选择页
            NavigationService.GetNavigationService(page).GoBack();
        }

        /// <summary>
        /// 复制数据包的简介
        /// </summary>
        private void CopyDatapackDescriptionCommand()
        {
            Clipboard.SetText(SelectedDatapackDescription);
        }

        /// <summary>
        /// 复制数据包的路径
        /// </summary>
        private void CopyDatapackPathCommand()
        {
            Clipboard.SetText(SelectedDatapackPath);
        }

        /// <summary>
        /// 复制数据包的名称
        /// </summary>
        private void CopyDatapackNameCommand()
        {
            Clipboard.SetText(DatapackName);
        }

        /// <summary>
        /// 复制数据包的主命名空间名称
        /// </summary>
        private void CopyDatapackMainNameSpaceCommand()
        {
            Clipboard.SetText(DatapackMainNameSpace);
        }

        /// <summary>
        /// 同步主命名空间和数据包的名称
        /// </summary>
        private void SynchronizePackAndNameSpaceNameCommand(TextCheckBoxs box)
        {
            CanModifyNameSpace = !box.IsChecked.Value;
            DatapackMainNameSpace = DatapackName;
        }
    }
}

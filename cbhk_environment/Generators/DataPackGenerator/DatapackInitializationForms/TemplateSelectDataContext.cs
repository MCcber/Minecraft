using cbhk_environment.CustomControls;
using cbhk_environment.Generators.DataPackGenerator.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms
{
    public class TemplateSelectDataContext: ObservableObject
    {
        /// <summary>
        /// 初始化模板选择和属性设置窗体数据上下文
        /// </summary>
        public TemplateSelectDataContext()
        {
            #region 链接命令
            TemplateLastStep = new RelayCommand<Page>(TemplateLastStepCommand);
            TemplateNextStep = new RelayCommand<Page>(TemplateNextStepCommand);
            ClearAllSelectParameters = new RelayCommand(ClearAllSelectParametersCommand);
            SelectAllTemplates = new RelayCommand<TextCheckBoxs>(SelectAllTemplatesCommand);
            ReverseAllTemplates = new RelayCommand<TextCheckBoxs>(ReverseAllTemplatesCommand);
            #endregion

            #region 载入版本
            if (Directory.Exists(TemplateDataFilePath))
            {
                string[] versionList = Directory.GetDirectories(TemplateDataFilePath);

                #region 添加版本的默认选中项
                if (VersionList.Count == 0)
                    VersionList.Add("所有版本");
                #endregion

                foreach (string version in versionList)
                {
                    string versionString = Path.GetFileName(version);
                    VersionList.Add(versionString);
                }
            }
            DefaultVersion = VersionList.Last();
            #endregion

            //初始化完毕
            ParametersReseting = false;
        }

        #region 存储已选中的模板
        public static List<TemplateItems> SelectedTemplateItemList = new List<TemplateItems>() { };
        #endregion

        #region 模板存放路径
        public static string TemplateDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\templates\\presets";
        #endregion

        #region 近期使用的模板存放路径
        public static string RecentTemplateDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\recent_templates";
        #endregion

        /// <summary>
        /// 近期使用的模板成员
        /// </summary>
        public static ObservableCollection<RecentTemplateItems> RecentTemplateList { get; set; } = new ObservableCollection<RecentTemplateItems>();

        /// <summary>
        /// 模板状态锁，防止更新死循环
        /// </summary>
        public static bool TemplateCheckLock = false;

        /// <summary>
        /// 已选择的版本
        /// </summary>
        public string SelectedVersionString = "";
        /// <summary>
        /// 已选择的文件类型
        /// </summary>
        public string SelectedFileTypeString = "";

        /// <summary>
        /// 存放版本列表
        /// </summary>
        public ObservableCollection<string> VersionList { get; set; } = new ObservableCollection<string> { };

        #region 引用主页的文件类型和功能类型数据源
        public static ObservableCollection<string> FileTypeList { get; set; } = datapack_datacontext.FileTypeList;
        public ObservableCollection<string> FunctionTypeList { get; set; } = datapack_datacontext.FunctionTypeList;
        #endregion

        #region 表示正在执行参数重置
        bool ParametersReseting = true;
        #endregion

        #region 存储已选择的版本
        public int SelectedVersionIndex { get; set; } = 0;
        private string selectedVersion = "";
        public string SelectedVersion
        {
            get { return selectedVersion; }
            set
            {
                selectedVersion = value;
                OnPropertyChanged();

                if (SelectedVersionIndex != 0)
                    ClearAllParametersVisibility = Visibility.Visible;
                if (selectedVersion != null && selectedVersion.Trim() != "" && !ParametersReseting)
                {
                    UpdateTemplateListVisibility(SelectedVersionIndex != 0 || SelectedFileTypeIndex != 0 || SelectedFunctionTypeIndex != 0 || SearchText.Trim().Length > 0);
                    IsResetSelectParameter();
                }
            }
        }
        #endregion

        #region 存储已选择的文件类型
        public int SelectedFileTypeIndex { get; set; } = 0;
        private string selectedFileType = "";
        public string SelectedFileType
        {
            get { return selectedFileType; }
            set
            {
                selectedFileType = value;
                OnPropertyChanged();

                if (SelectedFileTypeIndex != 0 && !ParametersReseting)
                    ClearAllParametersVisibility = Visibility.Visible;
                if (selectedFileType != null && selectedFileType.Trim() != "" && !ParametersReseting)
                {
                    UpdateTemplateListVisibility(SelectedVersionIndex != 0 || SelectedFileTypeIndex != 0 || SelectedFunctionTypeIndex != 0 || SearchText.Trim().Length > 0);
                    IsResetSelectParameter();
                }
            }
        }
        #endregion

        #region 存储已选择的功能类型
        public int SelectedFunctionTypeIndex { get; set; } = 0;
        private string selectedFunctionType = "";
        public string SelectedFunctionType
        {
            get { return selectedFunctionType; }
            set
            {
                selectedFunctionType = value;
                OnPropertyChanged();

                if (SelectedFunctionTypeIndex != 0 && !ParametersReseting)
                    ClearAllParametersVisibility = Visibility.Visible;
                if (selectedFunctionType != null && selectedFunctionType.Trim() != "" && !ParametersReseting)
                {
                    UpdateTemplateListVisibility(SelectedVersionIndex != 0 || SelectedFileTypeIndex != 0 || SelectedFunctionTypeIndex != 0 || SearchText.Trim().Length > 0);
                    IsResetSelectParameter();
                }
            }
        }
        #endregion

        #region 存储默认版本
        public static string DefaultVersion { get; set; }
        #endregion

        #region 存储默认文件类型
        public string DefaultFileType { get; set; } = ".json";
        #endregion

        #region 存储搜索模板搜索文本
        private string searchText = "";
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;

                if (searchText.Trim() == "" && SelectedVersion == "所有版本" && SelectedFileType == "所有文件类型" && SelectedFunctionType == "所有功能类型")
                {
                    datapack_datacontext.TemplateList.All(item => { item.Visibility = Visibility.Visible; return true; });
                    return;
                }

                //设置清除筛选按钮可见
                ClearAllParametersVisibility = Visibility.Visible;

                foreach (TemplateItems Template in datapack_datacontext.TemplateList)
                {
                    //默认为关闭
                    Template.Visibility = Visibility.Collapsed;
                    string TemplateName = Template.TemplateName.Text;
                    string TemplateDescription = Template.TemplateDescription.Text;

                    #region 对比一系列数据
                    if (searchText == TemplateName || searchText.Contains(TemplateName) || TemplateName.Contains(searchText))
                    {
                        Template.Visibility = Visibility.Visible;
                        continue;
                    }
                    if (searchText == TemplateDescription || searchText.Contains(TemplateDescription) || TemplateDescription.Contains(searchText))
                    {
                        Template.Visibility = Visibility.Visible;
                        continue;
                    }

                    DockPanel TypeTagPanel = Template.TemplateTypeTagPanel;
                    searchText = searchText.ToUpper();
                    foreach (TemplateTypeTag tag in TypeTagPanel.Children)
                    {
                        if (tag.Text.Text == searchText || tag.Text.Text.Contains(searchText) || searchText.Contains(tag.Text.Text))
                        {
                            Template.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                    #endregion
                }

                //搜索完毕后更新版本，文件类型和功能类型筛选
                if (SelectedVersion != "所有版本" || SelectedFileType != "所有文件类型" || SelectedFunctionType != "所有功能类型")
                    UpdateTemplateListVisibility(true);
            }
        }
        #endregion


        #region 模板窗体:上一步、下一步命令和清除所有筛选
        public RelayCommand<Page> TemplateLastStep { get; set; }
        public RelayCommand<Page> TemplateNextStep { get; set; }
        public RelayCommand ClearAllSelectParameters { get; set; }
        #endregion

        #region 全选模板、反选模板
        public RelayCommand<TextCheckBoxs> SelectAllTemplates { get; set; }
        public RelayCommand<TextCheckBoxs> ReverseAllTemplates { get; set; }
        #endregion

        #region 存储所有类型的模板标签
        public static List<string> SelectedTemplateTypeTagList = new List<string> { };
        #endregion

        #region 清空筛选参数按钮可见性
        private Visibility clearAllParametersVisibility = Visibility.Hidden;
        public Visibility ClearAllParametersVisibility
        {
            get { return clearAllParametersVisibility; }
            set
            {
                clearAllParametersVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 模板选择窗体逻辑处理

        /// <summary>
        /// 清除所有筛选参数
        /// </summary>
        private void ClearAllSelectParametersCommand()
        {
            ClearAllParametersVisibility = Visibility.Hidden;
            ParametersReseting = true;
            SelectedVersionIndex = SelectedFileTypeIndex = SelectedFunctionTypeIndex = 0;
            SelectedVersion = VersionList.First();
            SelectedFileType = FileTypeList.First();
            SelectedFunctionType = FunctionTypeList.First();
            SearchText = "";
            datapack_datacontext.TemplateList.All(item =>
            {
                item.Visibility = Visibility.Visible;
                return true;
            });
            ParametersReseting = false;
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        private void TemplateLastStepCommand(Page page)
        {
            NavigationService.GetNavigationService(page).GoBack();
        }

        /// <summary>
        /// 进入下一步
        /// </summary>
        private void TemplateNextStepCommand(Page page)
        {
            //获取已选择的版本和文件类型
            SelectedVersionString = SelectedVersion;
            SelectedFileTypeString = SelectedFileType.ToLower();

            if (SelectedVersionString == "所有版本")
                SelectedVersionString = DefaultVersion;
            if (SelectedFileTypeString == "所有文件类型")
                SelectedFileTypeString = DefaultFileType.ToLower();

            foreach (var selectedTemplateItemList in SelectedTemplateItemList)
            {
                string FileType = selectedTemplateItemList.FileType;
                string FunctionType = selectedTemplateItemList.FunctionType;
                SelectedTemplateTypeTagList.Clear();
                SelectedTemplateTypeTagList.Add(FileType);
                SelectedTemplateTypeTagList.Clear();
                SelectedTemplateTypeTagList.Add(FunctionType);
            }

            //载入数据包属性设置页
            datapack_datacontext datapackDatacontext = Window.GetWindow(page).DataContext as datapack_datacontext;
            DatapackGenerateSetupPage datapackGenerateSetupPage = datapackDatacontext.GetDatapackGenerateSetupPage();
            if (datapackGenerateSetupPage == null)
                datapackGenerateSetupPage = new DatapackGenerateSetupPage();
            if(datapackGenerateSetupPage.DataContext == null)
            datapackGenerateSetupPage.DataContext = new DatapackGenerateSetupDataContext();
            NavigationService.GetNavigationService(page).Navigate(datapackGenerateSetupPage);
        }

        /// <summary>
        /// 全选模板
        /// </summary>
        /// <param name="obj"></param>
        private void SelectAllTemplatesCommand(TextCheckBoxs obj)
        {
            datapack_datacontext.TemplateList.All(item => { if (item.Visibility == Visibility.Visible) item.TemplateSelector.IsChecked = obj.IsChecked; return true; });
        }

        /// <summary>
        /// 反选模板
        /// </summary>
        /// <param name="obj"></param>
        private void ReverseAllTemplatesCommand(TextCheckBoxs obj)
        {
            datapack_datacontext.TemplateList.All(item => { if (item.Visibility == Visibility.Visible) item.TemplateSelector.IsChecked = !item.TemplateSelector.IsChecked; return true; });
        }

        /// <summary>
        /// 全选历史模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelectAllRecentTemplatesClick(object sender, RoutedEventArgs e)
        {
            TextCheckBoxs box = sender as TextCheckBoxs;
            RecentTemplateList.All(item => { if (item.Visibility == Visibility.Visible) item.TemplateSelector.IsChecked = box.IsChecked; return true; });
        }

        /// <summary>
        /// 反选历史模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReverseSelectAllRecentTemplatesClick(object sender, RoutedEventArgs e)
        {
            TextCheckBoxs box = sender as TextCheckBoxs;
            RecentTemplateList.All(item => { if (item.Visibility == Visibility.Visible) item.TemplateSelector.IsChecked = !item.TemplateSelector.IsChecked; return true; });
        }

        /// <summary>
        /// 根据模板成员属性来更新它们的可见性
        /// </summary>
        private void UpdateTemplateListVisibility(bool Searched = false)
        {
            //忽略版本
            bool IgnoreVersion = false;
            //忽略文件类型
            bool IgnoreFileType = false;
            //忽略功能类型
            bool IgnoreFunctionType = false;
            string CurrentVersion = SelectedVersion;
            string CurrentFileType = SelectedFileType;

            //当前选中的版本
            if (SelectedVersionIndex == 0)
            {
                IgnoreVersion = true;
                CurrentVersion = DefaultVersion;
            }
            //当前选中文件类型
            if (SelectedFileTypeIndex == 0)
            {
                IgnoreFileType = true;
                CurrentFileType = DefaultFileType;
            }

            //当前选中功能类型
            if (SelectedFunctionTypeIndex == 0)
                IgnoreFunctionType = true;

            //若全部忽略则显示逻辑由搜索框负责
            if(IgnoreVersion && IgnoreFileType && IgnoreFunctionType)
            {
                datapack_datacontext.TemplateList.All(item => { item.Visibility = Visibility.Visible; return true; });
                return;
            }

            //获取版本集合
            string[] versionList = Directory.GetDirectories(TemplateDataFilePath);

            #region 比较版本、文件类型和功能类型是否相同
            bool SameVersion = false;
            bool SameFileType = false;
            bool SameFunctionType = false;
            #endregion

            foreach (TemplateItems templateItem in datapack_datacontext.TemplateList)
            {
                DockPanel container = templateItem.TemplateTypeTagPanel;

                SameVersion = false;
                SameFileType = false;
                SameFunctionType = false;

                #region 筛选版本、文件类型和功能类型
                if (IgnoreVersion)
                {
                    string CurrentFileNameWithOutExtension = TemplateDataFilePath + "\\" + CurrentVersion + "\\" + templateItem.TemplateID;

                    foreach (string extension in FileTypeList)
                    {
                        string extensionString = extension.ToLower();
                        if(File.Exists(CurrentFileNameWithOutExtension + extensionString))
                        {
                            SameVersion = true;
                            break;
                        }
                    }
                }
                else
                if (File.Exists(TemplateDataFilePath + "\\" + CurrentVersion + "\\" + templateItem.TemplateID + CurrentFileType.ToLower()))
                    SameVersion = true;

                foreach (TemplateTypeTag item in container.Children)
                {
                    if (item.Text.Text == CurrentFileType.Replace(".","").ToUpper() || IgnoreFileType)
                        SameFileType = true;
                    if (item.Text.Text == SelectedFunctionType || IgnoreFunctionType)
                        SameFunctionType = true;

                    if (SameFileType && SameFunctionType)
                        break;
                }
                #endregion

                if (Searched)
                {
                    if ((!SameVersion || !SameFileType || !SameFunctionType) && templateItem.Visibility == Visibility.Visible)
                        templateItem.Visibility = Visibility.Collapsed;
                }
                else
                if(SameVersion && SameFileType && SameFunctionType)
                    templateItem.Visibility = Visibility.Visible;
                else
                templateItem.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 判断是否隐藏清除筛选按钮
        /// </summary>
        private void IsResetSelectParameter()
        {
            if (SelectedVersionIndex == 0 && SelectedFileTypeIndex == 0 && SelectedFunctionTypeIndex == 0 && SearchText.Trim() == "")
                ClearAllParametersVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// 载入近期使用的模板列表元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RecentTemplateListLoaded(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(RecentTemplateDataFilePath) && RecentTemplateList.Count == 0)
            {
                string[] recentTemplateDataList = Directory.GetFiles(RecentTemplateDataFilePath,"*.*",SearchOption.AllDirectories);
                foreach (string recentTemplateData in recentTemplateDataList)
                {
                    string recentTemplateString = File.ReadAllText(recentTemplateData);
                    JObject recentTemplateObj = JObject.Parse(recentTemplateString);

                    string templateName = recentTemplateObj["TemplateName"].ToString();
                    string fileType = recentTemplateObj["FileType"].ToString();
                    string nameSpace = recentTemplateObj["FileNameSpace"].ToString();
                    string filePath = recentTemplateObj["FilePath"].ToString();
                    string iconPath = AppDomain.CurrentDomain.BaseDirectory + recentTemplateObj["IconPath"].ToString();

                    RecentTemplateItems recentTemplateItems = new(filePath, fileType, templateName, iconPath, nameSpace)
                    {
                        initializationDatacontext = this
                    };
                    RecentTemplateList.Add(recentTemplateItems);
                }
            }
        }
        #endregion
    }
}

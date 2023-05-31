using cbhk_environment.CustomControls;
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
            //TemplateLastStep = new RelayCommand<Page>(TemplateLastStepCommand);
            //TemplateNextStep = new RelayCommand<Page>(TemplateNextStepCommand);
            //ClearAllSelectParameters = new RelayCommand(ClearAllSelectParametersCommand);
            //SelectAllTemplates = new RelayCommand<TextCheckBoxs>(SelectAllTemplatesCommand);
            //ReverseAllTemplates = new RelayCommand<TextCheckBoxs>(ReverseAllTemplatesCommand);
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
        //public static List<TemplateItems> SelectedTemplateItemList = new List<TemplateItems>() { };
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
        //public static ObservableCollection<RecentTemplateItems> RecentTemplateList { get; set; } = new ObservableCollection<RecentTemplateItems>();

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
    }
}

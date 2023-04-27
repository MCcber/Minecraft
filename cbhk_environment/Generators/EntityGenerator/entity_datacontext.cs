using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.GeneralTools.Information;
using cbhk_environment.Generators.EntityGenerator.Components;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.EntityGenerator
{
    public class entity_datacontext : ObservableObject
    {
        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        #endregion

        #region 添加实体、清空实体、导入实体
        public RelayCommand AddEntity { get; set; }
        public RelayCommand ClearEntity { get; set; }
        public RelayCommand ImportEntityFromClipboard { get; set; }
        public RelayCommand ImportEntityFromFile { get; set; }
        #endregion

        #region 字段与引用
        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconEntities.png";
        //实体标签页的数据源
        public ObservableCollection<RichTabItems> EntityPageList { get; set; } = new() { new RichTabItems() { Style = Application.Current.Resources["RichTabItemStyle"] as Style,Header = "实体",IsContentSaved = true } };
        //实体标签页数量,用于为共通标签提供数据
        private int passengerMaxIndex = 0;
        public int PassengerMaxIndex
        {
            get
            {
                return passengerMaxIndex;
            }
            set
            {
                passengerMaxIndex = EntityPageList.Count - 1;
                OnPropertyChanged();
            }
        }
        #endregion


        public entity_datacontext()
        {
            #region 连接指令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            RunCommand = new RelayCommand(run_command);
            AddEntity = new RelayCommand(AddEntityCommand);
            ClearEntity = new RelayCommand(ClearEntityCommand);
            ImportEntityFromClipboard = new RelayCommand(ImportEntityFromClipboardCommand);
            ImportEntityFromFile = new RelayCommand(ImportEntityFromFileCommand);
            #endregion

            #region 初始化成员
            EntityPages entityPages = new() { FontWeight = FontWeights.Normal };
            EntityPageList[0].Content = entityPages;
            #endregion
        }

        /// <summary>
        /// 处理导入的数据
        /// </summary>
        /// <param name="filePath"></param>
        private void ImportDataHandler(string filePathOrData,bool IsPath = true)
        {
            string GeneratorMode = "";
            bool version1_12 = false;
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath? File.ReadAllText(filePathOrData):filePathOrData;

            #region 提取可用NBT数据和实体ID
            string nbtData = "", entityID = "";

            nbtData = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];
            //补齐缺失双引号对的key
            nbtData = Regex.Replace(nbtData, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            nbtData = Regex.Replace(nbtData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");

            //召唤实体
            if (Regex.IsMatch(data, @"^/?summon"))
            {
                GeneratorMode = "Summon";
                entityID = Regex.Match(data, @"(?<=/?summon\s)([\w:]+)").ToString();
            }
            else//给予怪物蛋
                if (Regex.IsMatch(data, @"^/?give (@[apesr])|(\w+) ([\w]+_)?spawn_egg"))
            {
                GeneratorMode = "Give";
                bool v1_13 = Regex.IsMatch(data, @"(?<=/?give\s@[apesr]\s)(\w+)(?=_spawn_egg)");
                entityID = Regex.Match(data, @"(?<=/?give\s@[apesr]\s)(\w+)(?=_spawn_egg)").ToString();
                if (!v1_13)
                    version1_12 = true;
            }
            else
            {
                displayContext.DisplayInfomation = "该指令内容与实体生成无关";
                displayContext.MessageTitle = "导入失败";
                displayContext.MessageIcon = displayContext.errorIcon;
                messageDisplayer.ShowDialog();
                EntityPageList.RemoveAt(EntityPageList.Count - 1);
                return;
            }

            JToken entityTagID = JObject.Parse(nbtData).SelectToken("EntityTag.id");
            if (entityTagID != null && entityID.Length == 0)
                entityID = entityTagID.ToString();

            //过滤掉命名空间
            entityID = Regex.Replace(entityID, @"[\w\\/\.]+:", "").Trim();
            #endregion

            try
            {
                JObject nbtObj = JObject.Parse(nbtData);

                //启用外部导入模式
                List<string> result = MainWindow.EntityDataBase.Where(item => item.Key[..item.Key.IndexOf(':')] == entityID).Select(item => item.Key).ToList();
                if (result.Count > 0)
                {
                    entityID = result[0];
                    entityID = entityID[(entityID.IndexOf(':') + 1)..];
                    AddEntityCommand(nbtObj, entityID, version1_12,GeneratorMode);
                }
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                displayContext.MessageIcon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                messageDisplayer.ShowDialog();
                EntityPageList.RemoveAt(EntityPageList.Count - 1);
            }
        }

        /// <summary>
        /// 从文件导入实体
        /// </summary>
        private void ImportEntityFromFileCommand()
        {
            Microsoft.Win32.OpenFileDialog dialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                RestoreDirectory = true,
                DefaultExt = ".command",
                Multiselect = false,
                Title = "请选择一个Minecraft实体数据文件"
            };
            if (dialog.ShowDialog().Value)
                if (File.Exists(dialog.FileName))
                    ImportDataHandler(dialog.FileName);
        }

        /// <summary>
        /// 从剪切板导入实体
        /// </summary>
        private void ImportEntityFromClipboardCommand()
        {
            ImportDataHandler(Clipboard.GetText(), false);
        }

        /// <summary>
        /// 清空实体
        /// </summary>
        private void ClearEntityCommand()
        {
            EntityPageList.Clear();
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        private void AddEntityCommand()
        {
            RichTabItems richTabItems = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Header = "实体",
                IsContentSaved = true
            };
            EntityPages entityPages = new() { FontWeight = FontWeights.Normal };
            richTabItems.Content = entityPages;
            EntityPageList.Add(richTabItems);
            if (EntityPageList.Count == 1)
            {
                TabControl tabControl = richTabItems.FindParent<TabControl>();
                tabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        private void AddEntityCommand(JObject externData, string selectedEntityID,bool version1_12,string mode)
        {
            RichTabItems richTabItems = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Header = "实体",
                IsContentSaved = true
            };
            EntityPages entityPages = new() { FontWeight = FontWeights.Normal };
            if(externData != null)
            {
                entityPagesDataContext context = entityPages.DataContext as entityPagesDataContext;
                if (mode == "Summon")
                    context.Summon = true;
                else
                    context.Give = true;
                context.ImportMode = true;
                context.ExternallyReadEntityData = externData;
                if (version1_12)
                    context.SelectedVersion = "1.12-";
                context.SelectedEntityId = context.EntityIds.Where(item => item.ComboBoxItemText == selectedEntityID).First();
            }
            richTabItems.Content = entityPages;
            EntityPageList.Add(richTabItems);
            if(EntityPageList.Count == 1)
            {
                TabControl tabControl = richTabItems.FindParent<TabControl>();
                tabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            Entity.cbhk.Topmost = true;
            Entity.cbhk.WindowState = WindowState.Normal;
            Entity.cbhk.Show();
            Entity.cbhk.Topmost = false;
            Entity.cbhk.ShowInTaskbar = true;
            win.Close();
        }

        /// <summary>
        /// 全部生成
        /// </summary>
        private void run_command()
        {
        }
    }
}

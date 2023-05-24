using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using cbhk_environment.Generators.ItemGenerator.Components;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cbhk_environment.Generators.ItemGenerator
{
    public class item_datacontext:ObservableObject
    {
        #region 返回、运行和保存等指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        public RelayCommand SaveAll { get; set; }
        public RelayCommand AddItem { get; set; }
        public RelayCommand ClearItem { get; set; }
        public RelayCommand ImportItemFromClipboard { get; set; }
        public RelayCommand ImportItemFromFile { get; set; }
        #endregion

        #region 显示结果
        private bool showGeneratorResult = false;
        public bool ShowGeneratorResult
        {
            get
            {
                return showGeneratorResult;
            }
            set
            {
                showGeneratorResult = value;
            }
        }
        #endregion

        //物品页数据源
        public ObservableCollection<RichTabItems> ItemPageList { get; set; } = new() { new RichTabItems() { Style = Application.Current.Resources["RichTabItemStyle"] as Style, Header = "物品", IsContentSaved = true } };
        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconItems.png";

        public item_datacontext()
        {
            #region 链接指令
            RunCommand = new RelayCommand(run_command);
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            SaveAll = new RelayCommand(SaveAllCommand);
            AddItem = new RelayCommand(AddItemCommand);
            ClearItem = new RelayCommand(ClearItemCommand);
            ImportItemFromClipboard = new RelayCommand(ImportItemFromClipboardCommand);
            ImportItemFromFile = new RelayCommand(ImportItemFromFileCommand);
            #endregion

            #region 初始化成员
            ItemPages itemPage = new() { FontWeight = FontWeights.Normal };
            ItemPageList[0].Content = itemPage;
            #endregion
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        private void AddItemCommand()
        {
            RichTabItems richTabItems = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Header = "物品",
                IsContentSaved = true
            };
            ItemPages itemPage = new() { FontWeight = FontWeights.Normal };
            richTabItems.Content = itemPage;
            ItemPageList.Add(richTabItems);
            if (ItemPageList.Count == 1)
            {
                TabControl tabControl = richTabItems.FindParent<TabControl>();
                tabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        private void ClearItemCommand()
        {
            ItemPageList.Clear();
        }

        /// <summary>
        /// 从文件导入
        /// </summary>
        private void ImportItemFromFileCommand()
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
                {
                    ObservableCollection<RichTabItems> result = ItemPageList;
                    ExternalDataImportManager.ImportItemDataHandler(dialog.FileName, ref result);
                }
        }

        /// <summary>
        /// 从剪切板导入
        /// </summary>
        private void ImportItemFromClipboardCommand()
        {
            ObservableCollection<RichTabItems> result = ItemPageList;
            ExternalDataImportManager.ImportItemDataHandler(Clipboard.GetText(), ref result, false);
        }

        /// <summary>
        /// 保存所有物品
        /// </summary>
        private async void SaveAllCommand()
        {
            await GeneratorAndSaveAllItems();
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            win.Close();
            Item.cbhk.Topmost = true;
            Item.cbhk.WindowState = WindowState.Normal;
            Item.cbhk.Show();
            Item.cbhk.Topmost = false;
            Item.cbhk.ShowInTaskbar = true;
        }

        /// <summary>
        /// 全部生成
        /// </summary>
        private async void run_command()
        {
            await GeneratorAllItems();
        }

        private async Task GeneratorAllItems()
        {
            StringBuilder Result = new();
            foreach (var itemPage in ItemPageList)
            {
                await itemPage.Dispatcher.InvokeAsync(() =>
                {
                    ItemPages itemPages = itemPage.Content as ItemPages;
                    string result = itemPages.run_command(false) + "\r\n";
                    Result.Append(result);
                });
            }
            if (ShowGeneratorResult)
            {
                GenerateResultDisplayer.Displayer displayer = GenerateResultDisplayer.Displayer.GetContentDisplayer();
                displayer.GeneratorResult(Result.ToString(), "物品", icon_path);
                displayer.Show();
            }
            else
                Clipboard.SetText(Result.ToString());
        }

        private async Task GeneratorAndSaveAllItems()
        {
            List<string> Result = new();
            List<string> FileNameList = new();

            foreach (var itemPage in ItemPageList)
            {
                await itemPage.Dispatcher.InvokeAsync(() =>
                {
                    ItemPages itemPages = itemPage.Content as ItemPages;
                    string result = itemPages.run_command(false);
                    string nbt = "";
                    if (result.Contains('{'))
                    {
                        nbt = result[result.IndexOf('{')..(result.IndexOf('}') + 1)];
                        //补齐缺失双引号对的key
                        nbt = Regex.Replace(nbt, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
                        //清除数值型数据的单位
                        nbt = Regex.Replace(nbt, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
                    }
                    JObject resultJSON = JObject.Parse(nbt);
                    string entityIDPath = "";
                    if (result.StartsWith("give"))
                        entityIDPath = "EntityTag.CustomName";
                    else
                        if (nbt.Length > 0)
                        entityIDPath = "CustomName";
                    JToken name = resultJSON.SelectToken(entityIDPath);
                    FileNameList.Add(itemPages.SelectedItemIdString + (name != null ? "-" + name.ToString() : ""));
                    Result.Add(result);
                });
            }
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new()
            {
                Description = "请选择要保存的目录",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog.SelectedPath))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "resources\\saves\\Item\\");
                    for (int i = 0; i < Result.Count; i++)
                    {
                        _ = File.WriteAllTextAsync(folderBrowserDialog.SelectedPath + FileNameList[i] + ".command", Result[i]);
                        _ = File.WriteAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + "resources\\saves\\Item\\" + FileNameList[i] + ".command", Result[i]);
                    }
                }
            }
        }
    }
}

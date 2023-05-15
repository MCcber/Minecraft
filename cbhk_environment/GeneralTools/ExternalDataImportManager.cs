using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools.Information;
using cbhk_environment.Generators.FireworkRocketGenerator.Components;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace cbhk_environment.GeneralTools
{
    public class ExternalDataImportManager
    {
        #region 处理导入外部实体数据
        public static string GetEntityDataHandler(string filePathOrData, bool IsPath = true)
        {
            string result = "";
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;
            #region 提取可用NBT数据和实体ID
            if (data.Contains("{") && data.Contains("}"))
                result = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];
            //补齐缺失双引号对的key
            result = Regex.Replace(result, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            result = Regex.Replace(result, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
            return result;
            #endregion
        }
        public static void ImportEntityDataHandler(string filePathOrData, ref ObservableCollection<RichTabItems> itemPageList, bool IsPath = true)
        {
            string GeneratorMode = "";
            bool version1_12 = false;
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;

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
                _ = messageDisplayer.ShowDialog();
                itemPageList.RemoveAt(itemPageList.Count - 1);
                return;
            }

            try
            {
                JToken entityTagID = JObject.Parse(nbtData).SelectToken("EntityTag.id");
                if (entityTagID != null && entityID.Length == 0)
                    entityID = entityTagID.ToString();
                //过滤掉命名空间
                entityID = Regex.Replace(entityID, @"[\w\\/\.]+:", "").Trim();
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                displayContext.MessageIcon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
                return;
            }
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
                    //添加实体命令
                    AddEntityCommand(nbtObj, entityID, version1_12, GeneratorMode, IsPath ? filePathOrData : "",ref itemPageList);
                }
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                displayContext.MessageIcon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
            }
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        private static void AddEntityCommand(JObject externData, string selectedEntityID, bool version1_12, string mode, string filePath,ref ObservableCollection<RichTabItems> itemPageList)
        {
            RichTabItems richTabItems = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Header = "实体",
                IsContentSaved = true
            };
            Generators.EntityGenerator.Components.EntityPages entityPages = new() { FontWeight = FontWeights.Normal };
            if (externData != null)
            {
                Generators.EntityGenerator.Components.entityPagesDataContext context = entityPages.DataContext as Generators.EntityGenerator.Components.entityPagesDataContext;
                if (mode == "Summon")
                    context.Summon = true;
                else
                    context.Give = true;
                context.ImportMode = true;
                if (filePath.Length > 0 && File.Exists(filePath))
                    context.ExternFilePath = filePath;
                context.ExternallyReadEntityData = externData;
                if (version1_12)
                    context.SelectedVersion = "1.12-";
                context.SelectedEntityId = context.EntityIds.Where(item => item.ComboBoxItemText == selectedEntityID).First();
            }
            richTabItems.Content = entityPages;
            itemPageList.Add(richTabItems);
            if (itemPageList.Count == 1)
            {
                System.Windows.Controls.TabControl tabControl = richTabItems.FindParent<System.Windows.Controls.TabControl>();
                tabControl.SelectedIndex = 0;
            }
        }
        #endregion

        #region 处理导入外部物品数据
        public static string GetItemDataHandler(string filePathOrData, bool IsPath = true)
        {
            string result = "";
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;
            #region 提取可用NBT数据和物品ID
            if (data.Contains("{") && data.Contains("}"))
                result = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];
            //补齐缺失双引号对的key
            result = Regex.Replace(result, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            result = Regex.Replace(result, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
            return result;
            #endregion
        }
        public static void ImportItemDataHandler(string filePathOrData, ref ObservableCollection<RichTabItems> itemPageList, bool IsPath = true, bool ReferenceMode = false)
        {
            string GeneratorMode = "";
            bool version1_12 = false;
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;

            #region 提取可用NBT数据和实体ID
            string nbtData = "", itemID = "";
            if(data.Contains("{") && data.Contains("}"))
            nbtData = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];
            //补齐缺失双引号对的key
            nbtData = Regex.Replace(nbtData, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            nbtData = Regex.Replace(nbtData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");

            //召唤实体
            if (Regex.IsMatch(data, @"^/?summon"))
            {
                GeneratorMode = "Summon";
                itemID = Regex.Match(data, @"(?<=/?summon\s)([\w:]+)").ToString();
            }
            else//给予怪物蛋
                if (Regex.IsMatch(data, @"^/?give (@[apesr])|(\w+) ([\w]+_)?"))
            {
                GeneratorMode = "Give";
                bool v1_13 = Regex.IsMatch(data, @"(?<=/?give\s@[apesr]\s)(\w+)");
                itemID = Regex.Match(data, @"(?<=/?give\s@[apesr]\s)(\w+)").ToString();
                if (!v1_13)
                    version1_12 = true;
            }
            else
            {
                displayContext.DisplayInfomation = "该指令内容与物品生成无关";
                displayContext.MessageTitle = "导入失败";
                displayContext.MessageIcon = displayContext.errorIcon;
                _ = messageDisplayer.ShowDialog();
                itemPageList.RemoveAt(itemPageList.Count - 1);
                return;
            }

            try
            {
                JToken itemTagID = JObject.Parse(nbtData).SelectToken("Item.id");
                if (itemTagID != null && itemID.Length == 0)
                    itemID = itemTagID.ToString();
                //过滤掉命名空间
                itemID = Regex.Replace(itemID, @"[\w\\/\.]+:", "").Trim();
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                displayContext.MessageIcon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
                return;
            }
            #endregion

            try
            {
                JObject nbtObj = JObject.Parse(nbtData);
                //启用外部导入模式
                List<string> result = MainWindow.ItemDataBase.Where(item => item.Key[..item.Key.IndexOf(':')] == itemID).Select(item => item.Key).ToList();
                if (result.Count > 0)
                {
                    itemID = result[0];
                    itemID = itemID[(itemID.IndexOf(':') + 1)..];
                    //添加实体命令
                    if(!ReferenceMode)
                    AddItemCommand(nbtObj, itemID, version1_12, GeneratorMode, IsPath ? filePathOrData : "", ref itemPageList);
                }
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                displayContext.MessageIcon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
            }
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        private static void AddItemCommand(JObject externData, string selectedItemID, bool version1_12, string mode, string filePath, ref ObservableCollection<RichTabItems> itemPageList)
        {
            RichTabItems richTabItems = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Header = "物品",
                IsContentSaved = true
            };
            Generators.ItemGenerator.Components.ItemPages itemPages = new() { FontWeight = FontWeights.Normal };
            if (externData != null)
            {
                if (mode == "Summon")
                    itemPages.Summon = true;
                else
                    itemPages.Give = true;
                itemPages.ImportMode = true;
                if (filePath.Length > 0 && File.Exists(filePath))
                    itemPages.ExternFilePath = filePath;
                itemPages.ExternallyReadEntityData = externData;
                if (version1_12)
                    itemPages.SelectedVersion = "1.12-";
                IconComboBoxItem itemId = MainWindow.ItemIdSource.Where(item => item.ComboBoxItemText == selectedItemID).First();
                itemPages.SelectedItemId = itemId;
            }
            richTabItems.Content = itemPages;
            itemPageList.Add(richTabItems);
            if (itemPageList.Count == 1)
            {
                System.Windows.Controls.TabControl tabControl = richTabItems.FindParent<System.Windows.Controls.TabControl>();
                tabControl.SelectedIndex = 0;
            }
        }
        #endregion

        #region 处理导入外部烟花数据
        public static void ImportFireworkDataHandler(string filePathOrData, ref ObservableCollection<RichTabItems> itemPageList, bool IsPath = true)
        {
            string GeneratorMode;
            bool version1_12 = false;
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;

            #region 提取可用NBT数据和烟花ID
            string nbtData = "";
            if (data.Contains('{') && data.Contains('}'))
                nbtData = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];
            //补齐缺失双引号对的key
            nbtData = Regex.Replace(nbtData, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            nbtData = Regex.Replace(nbtData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");

            //召唤实体
            if (Regex.IsMatch(data, @"^/?summon"))
                GeneratorMode = "Summon";
            else//给予怪物蛋
                if (Regex.IsMatch(data, @"^/?give (@[apesr])|(\w+) ((minecraft:)?(firework_rocket))"))
            {
                GeneratorMode = "Give";
                bool v1_13 = Regex.IsMatch(data, @"(?<=/?give\s@[apesr]\s)(\w+)");
                if (!v1_13)
                    version1_12 = true;
            }
            else
            {
                displayContext.DisplayInfomation = "该指令内容与烟花生成无关";
                displayContext.MessageTitle = "导入失败";
                displayContext.MessageIcon = displayContext.errorIcon;
                _ = messageDisplayer.ShowDialog();
                itemPageList.RemoveAt(itemPageList.Count - 1);
                return;
            }

            try
            {
                JToken itemTagID = JObject.Parse(nbtData).SelectToken("Item.id");
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                displayContext.MessageIcon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
                return;
            }
            #endregion

            try
            {
                JObject nbtObj = JObject.Parse(nbtData);
                //添加实体命令
                AddFireworkCommand(nbtObj, version1_12, GeneratorMode, IsPath ? filePathOrData : "", ref itemPageList);
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                displayContext.MessageIcon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
            }
        }

        /// <summary>
        /// 添加烟花
        /// </summary>
        private static void AddFireworkCommand(JObject externData, bool version1_12, string mode, string filePath, ref ObservableCollection<RichTabItems> itemPageList)
        {
            RichTabItems richTabItems = new()
            {
                Style = Application.Current.Resources["RichTabItemStyle"] as Style,
                Header = "烟花",
                IsContentSaved = true
            };
            FireworkRocketPages itemPages = new() { FontWeight = FontWeights.Normal };
            FireworkRocketPagesDataContext context = itemPages.DataContext as FireworkRocketPagesDataContext;
            if (externData != null)
            {
                if (mode == "Summon")
                    context.Summon = true;
                else
                    context.Give = true;
                context.ImportMode = true;
                if (filePath.Length > 0 && File.Exists(filePath))
                    context.ExternFilePath = filePath;
                context.ExternallyReadEntityData = externData;
                if (version1_12)
                    context.SelectedVersion = "1.12-";
            }
            itemPageList.Add(richTabItems);
            if (itemPageList.Count == 1)
            {
                System.Windows.Controls.TabControl tabControl = richTabItems.FindParent<System.Windows.Controls.TabControl>();
                tabControl.SelectedIndex = 0;
            }
            richTabItems.Content = itemPages;
        }
        #endregion
    }
}

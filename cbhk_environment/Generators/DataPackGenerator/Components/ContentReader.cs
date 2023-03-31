using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools.ScrollViewerHelper;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace cbhk_environment.Generators.DataPackGenerator.Components
{
    public class ContentReader
    {
        #region 数据包元数据的数据结构
        public struct DataPackMetaStruct
        {
            /// <summary>
            /// 版本
            /// </summary>
            public string Version;
            /// <summary>
            /// 字符描述
            /// </summary>
            public string Description;
            /// <summary>
            /// 对象或数组描述
            /// </summary>
            public RichParagraph DescriptionObjectOrArray;
            /// <summary>
            /// 描述的类型
            /// </summary>
            public string DescriptionType;
            /// <summary>
            /// 过滤器
            /// </summary>
            public RichParagraph Filter;
        };
        #endregion

        //目标类型目录配置文件路径
        static string TargetFolderNameListFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\targetFolderNameList.ini";
        //能够读取的文件类型配置文件路径
        static string ReadableFileExtensionListFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\ReadableFileExtensionList.ini";
        //原版命名空间配置文件路径
        static string OriginalTargetFolderNameListFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\DataPack\\data\\minecraftNameSpaceList.ini";
        //存储目标目录类型列表
        static List<string> TargetFolderNameList = new List<string> { };
        //存储能够读取的文件类型列表
        static List<string> ReadableFileExtensionList = new List<string> { };
        //存储原版命名空间类型列表
        static List<string> OriginalTargetFolderNameList = new List<string> { };

        /// <summary>
        /// 传入目标文件夹路径，返回数据包节点对象，该对象订阅展开事件
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static RichTreeViewItems ReadTargetContent(string folderPath,ContentType type,DataPackMetaStruct metaStruct)
        {
            RichTreeViewItems result = null;

            #region 初始化配置文件
            if (File.Exists(TargetFolderNameListFilePath) && TargetFolderNameList.Count == 0)
                TargetFolderNameList = File.ReadAllLines(TargetFolderNameListFilePath).ToList();

            if (File.Exists(ReadableFileExtensionListFilePath) && ReadableFileExtensionList.Count == 0)
                ReadableFileExtensionList = File.ReadAllLines(ReadableFileExtensionListFilePath).ToList();

            if (File.Exists(OriginalTargetFolderNameListFilePath) && OriginalTargetFolderNameList.Count == 0)
                OriginalTargetFolderNameList = File.ReadAllLines(OriginalTargetFolderNameListFilePath).ToList();
            #endregion

                //如果路径不为空且目标命名空间和读取配置列表均不为空则继续执行
            if (folderPath != null && TargetFolderNameList.Count > 0 && ReadableFileExtensionList.Count > 0)
            {
                switch (type)
                {
                    case ContentType.DataPack:
                        if (Directory.Exists(folderPath))
                        {
                            //拥有pack.mcmeta文件和data文件夹,证实确实是数据包文件夹
                            if (Directory.Exists(folderPath + "\\data") && File.Exists(folderPath + "\\pack.mcmeta"))
                            {
                                ContentItems contentItems = new ContentItems(folderPath, type)
                                {
                                    IsDataPack = true,
                                    DataPackMetaInfo = metaStruct
                                };

                                RichTreeViewItems CurrentNode = new RichTreeViewItems()
                                {
                                    Uid = folderPath,
                                    Tag = type,
                                    Header = contentItems
                                };

                                result = CurrentNode;

                                //如果数据包头的子级有目录则创建一个空子级
                                int SubDirectoryCount = Directory.GetDirectories(folderPath + "\\data").Count();
                                if (SubDirectoryCount > 0)
                                {
                                    CurrentNode.Expanded += GetCurrentSubContent;
                                    CurrentNode.Items.Add("");
                                }
                            }
                        }
                        break;
                    case ContentType.Folder:
                        if (Directory.Exists(folderPath))
                        {
                            string[] subContent = Directory.GetDirectories(folderPath);

                            //读取所有目录
                            foreach (string item in subContent)
                            {
                                //实例化一个文件夹节点
                                ContentItems contentItems = new ContentItems(item, type);
                                ContentType folderOrFile = ContentType.FolderOrFile;
                                RichTreeViewItems CurrentNode = new RichTreeViewItems
                                {
                                    Uid = item,
                                    Tag = folderOrFile,
                                    Header = contentItems
                                };

                                result = CurrentNode;

                                int SubDirectoryCount = Directory.GetDirectories(item).Count();
                                if (SubDirectoryCount > 0)
                                {
                                    CurrentNode.Expanded += GetCurrentSubContent;
                                    CurrentNode.Items.Add("");
                                }
                                break;
                            }
                        }
                        break;
                    case ContentType.File:
                        if (Directory.Exists(folderPath))
                        {
                            string[] subContent = Directory.GetFiles(folderPath);
                            foreach (string item in subContent)
                            {
                                //实例化一个文件节点
                                ContentItems contentItems = new ContentItems(item, type);
                                RichTreeViewItems CurrentNode = new RichTreeViewItems
                                {
                                    Uid = item,
                                    Tag = type,
                                    Header = contentItems,
                                };
                                result = CurrentNode;
                                break;
                            }
                        }

                        if(File.Exists(folderPath))
                        {
                            //实例化一个文件节点
                            ContentItems contentItems = new ContentItems(folderPath, type);
                            RichTreeViewItems CurrentNode = new RichTreeViewItems
                            {
                                Uid = folderPath,
                                Tag = type,
                                Header = contentItems,
                            };
                            result = CurrentNode;
                        }
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 分析指定内容属于哪个类型
        /// </summary>
        /// <param name="contentPath"></param>
        /// <returns></returns>
        public static ContentType GetTargetContentType(string contentPath)
        {
            ContentType resultType = ContentType.UnKnown;

            //当前内容为数据包
            if(Directory.Exists(contentPath+"\\data") && File.Exists(contentPath+"\\pack.mcmeta"))
                return ContentType.DataPack;
            //当前内容为文件夹
            if (Directory.Exists(contentPath))
                return ContentType.Folder;
            //当前内容为文件
            if (File.Exists(contentPath+".json") || File.Exists(contentPath+".mcfunction"))
                return ContentType.File;

            return resultType;
        }

        /// <summary>
        /// 解析pack.mcmeta文件
        /// </summary>
        /// <returns>返回meta文件数据</returns>
        public static DataPackMetaStruct McmetaParser(string path)
        {
            string content;
            string jsonData = "";
            using (StreamReader streamReader = new StreamReader(path))
            {
                content = streamReader.ReadToEnd();
                ContentType contentType = GetTargetContentType(content);
                if(contentType == ContentType.DataPack)
                using (StreamReader streamReader1 = new StreamReader(content + "\\pack.mcmeta"))
                {
                    jsonData = streamReader1.ReadToEnd();
                }
            }
            DataPackMetaStruct result = new DataPackMetaStruct();
            //描述的段落对象
            RichParagraph descriptionParagraph = new RichParagraph();
            //判断描述是否为数组
            bool descriptionIsArray = false;
            //描述数组长度
            int descriptionCount = 0;
            //过滤器长度
            int blockCount = 0;
            //过滤器对象
            RichParagraph filterParagraph = new RichParagraph();
            //载入目标路径的元数据文件
            //string jsonData = File.ReadAllText(content,System.Text.Encoding.UTF8);
            datapack_datacontext.JsonScript("var data =" + jsonData);

            #region 提取版本数据
            object VersionObj = datapack_datacontext.JsonScript("data.pack.pack_format");
            #endregion

            #region 判断该包是否有简介和过滤器
            object DescriptionObj = datapack_datacontext.JsonScript("data.pack.description");
            object FilterObj = datapack_datacontext.JsonScript("data.filter");
            object BlockObj = datapack_datacontext.JsonScript("data.filter.block");
            #endregion

            //把数字版本号转为游戏版本号(该参数为必要参数)
            if (VersionObj != null)
            {
                VersionObj = datapack_datacontext.DatapackVersionDatabase.Where(item => item.Value == VersionObj.ToString()).Select(item => item.Key).First();

                //检查简介内容
                if (DescriptionObj != null)
                {
                    bool.TryParse(datapack_datacontext.JsonScript("CurrentIsArray('.pack.description');").ToString(),out descriptionIsArray);
                    if (descriptionIsArray)
                        descriptionCount = int.Parse(datapack_datacontext.JsonScript("data.pack.description.length").ToString());
                }

                //检查过滤器是否有成员
                if (FilterObj != null && BlockObj != null)
                    int.TryParse(datapack_datacontext.JsonScript("data.filter.block.length").ToString(), out blockCount);

                string action = "";
                string value = "";
                object actionObj = null;
                object valueObj = null;
                string component = "";

                #region 处理描述
                if (descriptionIsArray)
                {
                    result.DescriptionType = "Array";
                    for (int i = 0; i < descriptionCount; i++)
                    {
                        #region 处理文本组件的各种属性
                        object textObj = datapack_datacontext.JsonScript("data.pack.description[" + i + "].text");
                        string text = textObj != null ? "\"text\":\"" + textObj.ToString() +"\"," :"";
                        object colorObj = datapack_datacontext.JsonScript("data.pack.description[" + i + "].color");
                        string color = colorObj != null ? "\"color\":\"" + colorObj.ToString() + "\"," : "";

                        object HaveBold = datapack_datacontext.JsonScript("data.pack.description[" + i + "].bold");
                        string Bold = HaveBold != null ? "\"bold\":"+HaveBold.ToString()+"," : "";
                        object HaveItalic = datapack_datacontext.JsonScript("data.pack.description[" + i + "].italic");
                        string Italic = HaveItalic != null ? "\"italic\":"+HaveItalic.ToString()+"," : "";
                        object HaveUnderlined = datapack_datacontext.JsonScript("data.pack.description[" + i + "].underlined");
                        string Underlined = HaveUnderlined != null ? "\"underlined\":" + HaveUnderlined.ToString() + "," : "";
                        object HaveStrikethrough = datapack_datacontext.JsonScript("data.pack.description[" + i + "].strikethrough");
                        string Strikethrough = HaveStrikethrough != null ? "\"strikethrough\":" + HaveUnderlined.ToString() + "," : "";
                        object HaveObfuscated = datapack_datacontext.JsonScript("data.pack.description[" + i + "].obfuscated");
                        string Obfuscated = HaveObfuscated != null ? "\"obfuscated\":" + HaveObfuscated.ToString() + "," : "";

                        string ClickEvent = "";
                        object HaveClickEvent = datapack_datacontext.JsonScript("data.pack.description[" + i + "].clickEvent");
                        if(HaveClickEvent != null)
                        {
                            actionObj = datapack_datacontext.JsonScript("data.pack.description[" + i + "].clickEvent.action");
                            valueObj = datapack_datacontext.JsonScript("data.pack.description[" + i + "].clickEvent.value");
                            if(actionObj != null)
                                action = "\"action\":\""+ actionObj.ToString()+"\",";
                            if(valueObj != null)
                                value = "\"value\":\""+valueObj.ToString()+"\"";
                            ClickEvent = "\"clickEvent\":{" + action + value + "},";
                        }
                        string HoverEvent = "";
                        object HaveHoverEvent = datapack_datacontext.JsonScript("data.pack.description[" + i + "].hoverEvent");
                        if(HaveHoverEvent != null)
                        {
                            actionObj = datapack_datacontext.JsonScript("data.pack.description[" + i + "].hoverEvent.action");
                            valueObj = datapack_datacontext.JsonScript("data.pack.description[" + i + "].hoverEvent.value");
                            if (actionObj != null)
                                action = "\"action\":\"" + actionObj.ToString() + "\",";
                            if (valueObj != null)
                                value = "\"value\":\"" + valueObj.ToString() + "\"";
                            HoverEvent = "\"hoverEvent\":" + HaveHoverEvent.ToString() + ",";
                        }

                        object HaveInsertion = datapack_datacontext.JsonScript("data.pack.description[" + i + "].insertion");
                        string Insertion = HaveInsertion != null ? "\"insertion\":" + HaveInsertion.ToString() + "," : "";
                        #endregion

                        component = text + color + Bold + Italic + Underlined + Strikethrough + Obfuscated + ClickEvent + HoverEvent + Insertion;
                        descriptionParagraph.Inlines.Add(new RichRun()
                        {
                            Text = "{" + component.Trim() + "}"
                        });
                    }
                }
                else
                if (DescriptionObj != null)//描述类型为对象
                {
                    result.DescriptionType = "Object";
                    #region 处理文本组件的各种属性
                    object textObj = datapack_datacontext.JsonScript("data.pack.description.text");
                    string text = textObj != null ? "\"text\":\"" + textObj.ToString() + "\"," : "";
                    object colorObj = datapack_datacontext.JsonScript("data.pack.description.color");
                    string color = colorObj != null ? "\"color\":\"" + colorObj.ToString() + "\"," : "";

                    object HaveBold = datapack_datacontext.JsonScript("data.pack.description.bold");
                    string Bold = HaveBold != null ? "\"bold\":" + HaveBold.ToString() + "," : "";
                    object HaveItalic = datapack_datacontext.JsonScript("data.pack.description.italic");
                    string Italic = HaveItalic != null ? "\"italic\":" + HaveItalic.ToString() + "," : "";
                    object HaveUnderlined = datapack_datacontext.JsonScript("data.pack.description.underlined");
                    string Underlined = HaveUnderlined != null ? "\"underlined\":" + HaveUnderlined.ToString() + "," : "";
                    object HaveStrikethrough = datapack_datacontext.JsonScript("data.pack.description.strikethrough");
                    string Strikethrough = HaveStrikethrough != null ? "\"strikethrough\":" + HaveUnderlined.ToString() + "," : "";
                    object HaveObfuscated = datapack_datacontext.JsonScript("data.pack.description.obfuscated");
                    string Obfuscated = HaveObfuscated != null ? "\"obfuscated\":" + HaveObfuscated.ToString() + "," : "";

                    string ClickEvent = "";
                    object HaveClickEvent = datapack_datacontext.JsonScript("data.pack.description.clickEvent");
                    if (HaveClickEvent != null)
                    {
                        actionObj = datapack_datacontext.JsonScript("data.pack.description.clickEvent.action");
                        valueObj = datapack_datacontext.JsonScript("data.pack.description.clickEvent.value");
                        if (actionObj != null)
                            action = "\"action\":\"" + actionObj.ToString() + "\",";
                        if (valueObj != null)
                            value = "\"value\":\"" + valueObj.ToString() + "\"";
                        ClickEvent = "\"clickEvent\":{" + action + value + "},";
                    }
                    string HoverEvent = "";
                    object HaveHoverEvent = datapack_datacontext.JsonScript("data.pack.description.hoverEvent");
                    if (HaveHoverEvent != null)
                    {
                        actionObj = datapack_datacontext.JsonScript("data.pack.description.hoverEvent.action");
                        valueObj = datapack_datacontext.JsonScript("data.pack.description.hoverEvent.value");
                        if (actionObj != null)
                            action = "\"action\":\"" + actionObj.ToString() + "\",";
                        if (valueObj != null)
                            value = "\"value\":\"" + valueObj.ToString() + "\"";
                        HoverEvent = "\"hoverEvent\":" + HaveHoverEvent.ToString() + ",";
                    }

                    object HaveInsertion = datapack_datacontext.JsonScript("data.pack.description.insertion");
                    string Insertion = HaveInsertion != null ? "\"insertion\":" + HaveInsertion.ToString() + "," : "";
                    #endregion

                    component = text + color + Bold + Italic + Underlined + Strikethrough + Obfuscated + ClickEvent + HoverEvent + Insertion;
                    DescriptionObj = new RichRun()
                    {
                        Text = "{" + component.Trim() + "}"
                    };
                }
                else//描述类型为字符串，数值或布尔
                {
                    result.DescriptionType = "String";
                    bool.TryParse(datapack_datacontext.JsonScript("data.pack.description").ToString(),out bool IsBool);
                    int.TryParse(datapack_datacontext.JsonScript("data.pack.description").ToString(), out int IsInt);
                    if (IsBool)
                        result.DescriptionType = "Bool";
                    else
                        if (IsInt > 0)
                        result.DescriptionType = "Int";
                    DescriptionObj = datapack_datacontext.JsonScript("data.pack.description").ToString();
                }
                #endregion

                #region 处理过滤器
                for (int i = 0; i < blockCount; i++)
                {
                    object filterNameSpace = datapack_datacontext.JsonScript("data.pack.filter.block[" + i + "].namespace");
                    object filterPath = datapack_datacontext.JsonScript("data.pack.filter.block[" + i + "].path");
                    string filterItem = filterNameSpace != null?"{\"namespace\":\""+filterNameSpace.ToString()+(filterPath != null? "\",\"path\":\"" + filterPath + "\"":"")+"},":"";
                    filterParagraph.Inlines.Add(new RichRun()
                    {
                        Text = filterItem.TrimEnd(',')
                    });
                }
                #endregion

                #region 处理完毕后设置返回对象的属性
                result.Version = VersionObj.ToString();
                result.Description = DescriptionObj.ToString();
                result.DescriptionObjectOrArray = descriptionParagraph;
                result.Filter = filterParagraph;
                #endregion

                return result;
            }
            else
                return new DataPackMetaStruct();
        }

        /// <summary>
        /// 获取当前节点的子级内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GetCurrentSubContent(object sender, RoutedEventArgs e)
        {
            RichTreeViewItems CurrentNode = sender as RichTreeViewItems;
            ContentType type = (ContentType)CurrentNode.Tag;

            //若已处理则返回
            if (CurrentNode.Items.Count >= 1 && !(CurrentNode.Items[0] is string)) return;

            //清空初始化的子级
            CurrentNode.Items.Clear();
            string currentPath = CurrentNode.Uid;
            //AddSecurity(currentPath);
            switch (type)
            {
                //当前内容节点类型为数据包
                case ContentType.DataPack:
                    //获取命名空间上级目录
                    string NameSpaceParentPath = CurrentNode.Uid + "\\data";

                    if(Directory.Exists(NameSpaceParentPath))
                    {
                        //遍历能够执行逻辑的命名空间
                        string[] suspectedTargetNameSpaceList = Directory.GetDirectories(NameSpaceParentPath);
                        foreach (string suspectedTargetNameSpace in suspectedTargetNameSpaceList)
                        {
                            string folderName = Path.GetFileNameWithoutExtension(suspectedTargetNameSpace);
                            List<string> matchDirectries = TargetFolderNameList.Where(TargetFolderName => Directory.Exists(suspectedTargetNameSpace + "\\" + TargetFolderName)).ToList();

                            //检测到当前文件夹为命名空间
                            if (matchDirectries.Count > 0)
                            {
                                #region 新建内容节点
                                ContentType newType = ContentType.FolderOrFile;
                                ContentItems contentItems = new ContentItems(suspectedTargetNameSpace, newType)
                                {
                                    DataPackItemReference = CurrentNode
                                };
                                RichTreeViewItems SubNode = new RichTreeViewItems()
                                {
                                    Header = contentItems,
                                    Tag = newType,
                                    Uid = suspectedTargetNameSpace
                                };
                                CurrentNode.Items.Add(SubNode);

                                int SubDirectoriesCount = Directory.GetDirectories(suspectedTargetNameSpace).Count();
                                int SubFilesCount = Directory.GetFiles(suspectedTargetNameSpace).Count();

                                //如果拥有子级内容则添加一个空子级并订阅展开事件
                                if (SubDirectoriesCount + SubFilesCount > 0)
                                {
                                    SubNode.Expanded += GetCurrentSubContent;
                                    SubNode.Items.Add("");
                                }
                                #endregion
                            }
                        }
                    }
                    break;
                case ContentType.FolderOrFile:
                    #region 确认父节点数据
                    RichTreeViewItems parentItem = (sender as RichTreeViewItems).Parent as RichTreeViewItems;
                    ContentItems parentContentItem = parentItem.Header as ContentItems;
                    #endregion

                    #region 获取所有子级内容并新建内容节点
                    string[] SubDirectories = Directory.GetDirectories(currentPath);
                    string[] SubFiles = Directory.GetFiles(currentPath);

                    foreach (string SubDirectory in SubDirectories)
                    {
                        ContentItems contentItems = new ContentItems(SubDirectory, type);
                        if (parentContentItem.IsDataPack)
                            contentItems.DataPackItemReference = parentItem;
                        else
                            contentItems.DataPackItemReference = parentContentItem.DataPackItemReference;

                        RichTreeViewItems SubNode = new RichTreeViewItems()
                        {
                            Header = contentItems,
                            Tag = type,
                            Uid = SubDirectory
                        };

                        CurrentNode.Items.Add(SubNode);

                        int SubDirectoriesCount = Directory.GetDirectories(SubDirectory).Count();
                        int SubFilesCount = Directory.GetFiles(SubDirectory).Count();

                        //如果拥有子级内容则添加一个空子级并订阅展开事件
                        if (SubDirectoriesCount + SubFilesCount > 0)
                        {
                            SubNode.Expanded += GetCurrentSubContent;
                            SubNode.Items.Add("");
                        }
                    }

                    foreach (string SubFile in SubFiles)
                    {
                        ContentItems contentItems = new ContentItems(SubFile, ContentType.File);
                        RichTreeViewItems subItem = new RichTreeViewItems()
                        {
                            Header = contentItems,
                            Tag = type,
                            Uid = SubFile
                        };
                        subItem.MouseDoubleClick += ClickToModifyTheFile;
                        CurrentNode.Items.Add(subItem);
                    }
                    #endregion
                    break;
            }
        }

        /// <summary>
        /// 双击分析文件后进行编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ClickToModifyTheFile(object sender, MouseButtonEventArgs e)
        {
            RichTreeViewItems currentItem = sender as RichTreeViewItems;

            string FilePath = currentItem.Uid;
            string FileName = Path.GetFileName(FilePath);
            string extension = Path.GetExtension(FilePath);

            #region 优化FileName的内容长度
            string CurrentFileName = FileName.Substring(0, FileName.LastIndexOf("."));
            if (CurrentFileName.Length > 20)
            {
                FileName = CurrentFileName;
                string StartPart = FileName.Substring(0, 10);
                string EndPart = FileName.Substring(FileName.Length - 10);
                FileName = StartPart + "..." + EndPart + extension;
            }
            #endregion

            if (File.Exists(FilePath))
            {
                if(extension == ".mcfunction")
                {
                    TabControl fileZone = EditDataContext.FileModifyZone;
                    
                    #region 实例化文本编辑框
                    TextEditor textBox = new()
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A8A8A8")),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A")),
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3D3D3D")),
                        BorderThickness = new Thickness(0),
                        FontFamily = new FontFamily("Minecraft"),
                        FontWeight = FontWeights.Normal,
                        FontSize = 20,
                        WordWrap = true,
                        ShowLineNumbers = true
                    };
                    VirtualizingPanel.SetIsVirtualizing(textBox, true);
                    VirtualizingPanel.SetVirtualizationMode(textBox, VirtualizationMode.Recycling);
                    ICSharpCode.AvalonEdit.Search.SearchPanel.Install(textBox);
                    TextEditorOptions editorOptions = new TextEditorOptions
                    {
                        InheritWordWrapIndentation = true,
                        ColumnRulerPosition = 10
                    };
                    #endregion

                    ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Auto);
                    ScrollViewer.SetHorizontalScrollBarVisibility(textBox, ScrollBarVisibility.Disabled);
                    //文本内容更新时通知标签页显示未保存标记
                    textBox.TextChanged += RichTextBoxTextChanged;
                    //执行保存操作时通知标签页隐藏未保存标记
                    textBox.KeyDown += RichTextBoxKeyDown;

                    #region 更新当前选中的.mcfunction文件的内容
                    string FunctionContent = File.ReadAllText(FilePath);
                    textBox.Text = FunctionContent;
                    #endregion

                    RichTabItems richTabItems = new RichTabItems
                    {
                        Uid = FilePath,
                        Header = FileName,
                        Padding = new Thickness(10,2,0,0),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323232")),
                        Content = textBox,
                        IsContentSaved = true
                    };
                    if(currentItem != null)
                        richTabItems.mappingItem = currentItem;
                    if (currentItem.Parent != null)
                        richTabItems.mappingParentItem = currentItem.Parent as RichTreeViewItems;
                    richTabItems.SetValue(FrameworkElement.StyleProperty, Application.Current.Resources["RichTabItemStyle"]);
                    fileZone.Items.Add(richTabItems);
                }
                else
                if(extension == ".json")
                {
                    //识别该json文件所属的功能类型并打开对应的生成器

                }
            }
        }

        /// <summary>
        /// Ctrl+S保存当前文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RichTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                RichTextBox richTextBox = sender as RichTextBox;
                RichTabItems CurrentItem = richTextBox.FindParent<RichTabItems>();
                if(CurrentItem != null)
                CurrentItem.IsContentSaved = true;
                TextRange textRange = new TextRange(richTextBox.Document.ContentStart,richTextBox.Document.ContentEnd);
                //覆盖数据
                File.WriteAllText(CurrentItem.Uid, textRange.Text);
                if(!CurrentItem.mappingParentItem.Items.Contains(CurrentItem.mappingItem))
                {
                    string path = CurrentItem.mappingParentItem.Uid + "\\" + CurrentItem.Header.ToString();
                    ContentType contentType = GetTargetContentType(path);
                    ContentItems contentItems = new ContentItems(path,contentType);
                    RichTreeViewItems richTreeViewItems = new RichTreeViewItems()
                    {
                        Uid = path,
                        Tag = contentType,
                        Header = contentItems
                    };
                    CurrentItem.mappingParentItem.Items.Add(richTreeViewItems);
                }
            }
        }

        /// <summary>
        /// 文档更新后显示更新标记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RichTextBoxTextChanged(object sender, EventArgs e)
        {
            RichTabItems CurrentItem = (sender as TextEditor).FindParent<RichTabItems>();
            if (CurrentItem != null)
                CurrentItem.IsContentSaved = false;
        }

        /// <summary>
        ///为文件夹添加users，everyone用户组的完全控制权限
        /// </summary>
        /// <param name="dirPath"></param>
        private static void AddSecurity(string dirPath)
        {
            //获取文件夹信息
            var dir = new DirectoryInfo(dirPath);
            //获得该文件夹的所有访问权限
            var dirSecurity = dir.GetAccessControl(AccessControlSections.All);
            //设定文件ACL继承
            var inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            //添加ereryone用户组的访问权限规则 完全控制权限
            var everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            //添加Users用户组的访问权限规则 完全控制权限
            var usersFileSystemAccessRule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out var isModified);
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, usersFileSystemAccessRule, out isModified);
            //设置访问权限
            dir.SetAccessControl(dirSecurity);
        }

        /// <summary>
        /// 用于区分需要新建内容的类型
        /// </summary>
        public enum ContentType
        {
            DataPack,
            FolderOrFile,
            Folder,
            File,
            UnKnown
        }
    }
}

using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools;
using ICSharpCode.AvalonEdit;
using Newtonsoft.Json.Linq;
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

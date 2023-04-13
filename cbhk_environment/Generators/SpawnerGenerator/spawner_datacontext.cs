using cbhk_environment.CustomControls;
using cbhk_environment.GenerateResultDisplayer;
using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace cbhk_environment.Generators.SpawnerGenerator
{
    public class spawner_datacontext: ObservableObject
    {
        /// <summary>
        /// 刷怪笼配置文件路径
        /// </summary>
        private readonly string treeViewStructureFilePath = AppDomain.CurrentDomain.BaseDirectory + "resources\\configs\\Spawner\\data\\structure.json";
        /// <summary>
        /// 刷怪笼树结构数据源
        /// </summary>
        public ObservableCollection<RichTreeViewItems> SpawnerStructureItems { get; set; } = new ObservableCollection<RichTreeViewItems>();

        /// <summary>
        /// 普通文本块的灰刷
        /// </summary>
        SolidColorBrush grayBrush = new((Color)ColorConverter.ConvertFromString("#838383"));

        #region 复合结构颜色表
        string[] CompoundColorArray = new string[] { "#1B1B1B", "#386330" };
        int CompoundColorIndex = 0;
        SolidColorBrush CompoundColorBrush = new();
        #endregion

        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        #endregion

        //复制结果
        RelayCommand CopyResult { get; set; }

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconSpawner.png";

        public spawner_datacontext()
        {
            #region 绑定指令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            CopyResult = new RelayCommand(CopyResultCommand);
            #endregion

            #region 初始化刷怪笼树结构
            string spawnerStructure = File.ReadAllText(treeViewStructureFilePath);
            SpawnerStructureItems = GeneralTools.TreeViewComponentsHelper.TreeViewConveter.Handler(spawnerStructure);
            #endregion
        }

        /// <summary>
        /// 复制结果
        /// </summary>
        private void CopyResultCommand()
        {
            //TextRange textRange = new(resultDocument.ContentStart, resultDocument.ContentEnd);
            //string result = "/setblock ~ ~ ~ spawner " + textRange.Text;
            //Clipboard.SetText(result);
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="win"></param>
        private void return_command(CommonWindow win)
        {
            Spawner.cbhk.Topmost = true;
            Spawner.cbhk.WindowState = WindowState.Normal;
            Spawner.cbhk.Show();
            Spawner.cbhk.Topmost = false;
            Spawner.cbhk.ShowInTaskbar = true;
            win.Close();
        }
    }
}

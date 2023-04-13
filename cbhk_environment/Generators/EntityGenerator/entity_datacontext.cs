using cbhk_environment.WindowDictionaries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace cbhk_environment.Generators.EntityGenerator
{
    public class entity_datacontext : ObservableObject
    {
        #region 返回和运行指令
        public RelayCommand<CommonWindow> ReturnCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        #endregion

        //本生成器的图标路径
        string icon_path = "pack://application:,,,/cbhk_environment;component/resources/common/images/spawnerIcons/IconEntities.png";

        public entity_datacontext()
        {
            #region 连接指令
            ReturnCommand = new RelayCommand<CommonWindow>(return_command);
            RunCommand = new RelayCommand(run_command);
            #endregion
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

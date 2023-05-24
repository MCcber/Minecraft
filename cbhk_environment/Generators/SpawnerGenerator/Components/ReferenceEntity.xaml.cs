using cbhk_environment.GeneralTools;
using cbhk_environment.Generators.EntityGenerator;
using cbhk_environment.Generators.EntityGenerator.Components;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace cbhk_environment.Generators.SpawnerGenerator.Components
{
    /// <summary>
    /// ReferenceEntity.xaml 的交互逻辑
    /// </summary>
    public partial class ReferenceEntity : UserControl
    {
        public ReferenceEntity()
        {
            InitializeComponent();
            Tag = "{id:\"minecraft:pig\"}";
            EntityIcon.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\pig_spawn_egg.png"));
        }

        /// <summary>
        /// 设置为空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetEmpty_Click(object sender, RoutedEventArgs e)
        {
            Tag = "";
            EntityIcon.Source = null;
        }

        /// <summary>
        /// 从剪切板导入实体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            string data = ExternalDataImportManager.GetEntityDataHandler(Clipboard.GetText(),false);
            Tag = data;
            string id = JObject.Parse(data)["id"].ToString().Replace("minecraft:","");
            string iconPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + id + ".png";
            if (!File.Exists(iconPath))
                iconPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + id + "_spawn_egg.png";
            if (File.Exists(iconPath))
                EntityIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
            else
                EntityIcon.Source = null;
        }

        /// <summary>
        /// 从文件导入实体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportFromFile_Click(object sender,RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Multiselect = false,
                RestoreDirectory = true,
                Title = "请选择一个实体文件"
            };
            if (openFileDialog.ShowDialog().Value)
            {
                string data = ExternalDataImportManager.GetEntityDataHandler(openFileDialog.FileName);
                Tag = data;
                if (File.Exists(openFileDialog.FileName))
                {
                    string id = JObject.Parse(data)["id"].ToString().Replace("minecraft:", "");
                    string iconPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + id + ".png";
                    if (!File.Exists(iconPath))
                        iconPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + id + "_spawn_egg.png";
                    if (File.Exists(iconPath))
                        EntityIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
                    else
                        EntityIcon.Source = null;
                }
            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Spawn_Click(object sender, RoutedEventArgs e)
        {
            EntityGenerator.Entity entity = new();
            EntityGenerator.entity_datacontext context = entity.DataContext as EntityGenerator.entity_datacontext;
            entityPagesDataContext pageContext = (context.EntityPageList[0].Content as EntityPages).DataContext as entityPagesDataContext;
            pageContext.UseForTool = true;
            if (entity.ShowDialog().Value)
            {
                string data = ExternalDataImportManager.GetEntityDataHandler(pageContext.Result,false);
                Tag = pageContext.Result;
                string id = JObject.Parse(data)["id"].ToString().Replace("minecraft:", "");
                string iconPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + id + ".png";
                if(!File.Exists(iconPath))
                    iconPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\" + id + "_spawn_egg.png";
                if (File.Exists(iconPath))
                    EntityIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
                else
                    EntityIcon.Source = null;
            }
        }
    }
}

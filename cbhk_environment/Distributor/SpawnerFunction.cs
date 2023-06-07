using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows;

namespace cbhk_environment.Distributor
{
    public class GeneratorFunction: ObservableObject
    {
        //启动OOC生成器
        public RelayCommand StartOnlyOneCommandGenerator { get; set; }
        //启动数据包生成器
        public RelayCommand StartDatapacksGenerator { get; set; }
        //启动盔甲架生成器
        public RelayCommand StartArmorStandsGenerator { get; set; }
        //启动成书生成器
        public RelayCommand StartWrittenBooksGenerator { get; set; }
        //启动战利品表生成器
        public RelayCommand StartLootTablesGenerator { get; set; }
        //启动刷怪笼生成器
        public RelayCommand StartSpawnersGenerator { get; set; }
        //启动配方生成器
        public RelayCommand StartRecipesGenerator { get; set; }
        //启动村民生成器
        public RelayCommand StartVillagersGenerator { get; set; }
        //启动成就生成器
        public RelayCommand StartAdvancementsGenerator { get; set; }
        //启动标签文件生成器
        public RelayCommand StartTagsGenerator { get; set; }
        //启动物品生成器
        public RelayCommand StartItemsGenerator { get; set; }
        //启动烟花生成器
        public RelayCommand StartFireworksGenerator { get; set; }
        //启动实体生成器
        public RelayCommand StartEntitiesGenerator { get; set; }
        //启动维度生成器
        public RelayCommand StartDimensionsGenerator { get; set; }
        //启动生物群系生成器
        public RelayCommand StartBiomesGenerator { get; set; }

        public List<RelayCommand> spawner_functions;

        public MainWindow cbhk;
        public GeneratorFunction(MainWindow win)
        {
            cbhk = win;
            #region 绑定命令
            //OOC
            StartOnlyOneCommandGenerator = new RelayCommand(StartOnlyOneCommandGeneratorCommand);
            //刷怪笼生成器
            StartSpawnersGenerator = new RelayCommand(StartSpawnerGeneratorCommand);
            //盔甲架
            StartArmorStandsGenerator = new RelayCommand(StartArmorStandsGeneratorCommand);
            //标签
            StartTagsGenerator = new RelayCommand(StartTagsGeneratorCommand);
            //物品
            StartItemsGenerator = new RelayCommand(StartItemsGeneratorCommand);
            //实体
            StartEntitiesGenerator = new RelayCommand(StartEntityGeneratorCommand);
            //烟花
            StartFireworksGenerator = new RelayCommand(StartFireworksGeneratorCommand);
            //配方
            StartRecipesGenerator = new RelayCommand(StartRecipesGeneratorCommand);
            //村民
            StartVillagersGenerator = new RelayCommand(StartVillagersGeneratorCommand);
            //成书
            StartWrittenBooksGenerator = new RelayCommand(StartWrittenBooksGeneratorCommand);
            //数据包
            StartDatapacksGenerator = new RelayCommand(StartDatapacksGeneratorCommand);
            #endregion

            //为外界提供索引形式的访问渠道
            spawner_functions = new List<RelayCommand>() {StartOnlyOneCommandGenerator,StartDatapacksGenerator,StartArmorStandsGenerator,
                                                          StartWrittenBooksGenerator,StartLootTablesGenerator,StartSpawnersGenerator,
                                                          StartRecipesGenerator, StartVillagersGenerator,StartAdvancementsGenerator,
                                                          StartTagsGenerator,StartItemsGenerator,StartFireworksGenerator,
                                                          StartEntitiesGenerator,StartDimensionsGenerator,StartBiomesGenerator };
        }

        /// <summary>
        /// 启动ooc生成器
        /// </summary>
        private void StartOnlyOneCommandGeneratorCommand()
        {
            Generators.OnlyOneCommandGenerator.OnlyOneCommand ooc_window = new Generators.OnlyOneCommandGenerator.OnlyOneCommand(cbhk);
            SetCBHKState();
            ooc_window.Topmost = true;
            ooc_window.Show();
            ooc_window.Topmost = false;
        }

        /// <summary>
        /// 启动刷怪笼生成器
        /// </summary>
        private void StartSpawnerGeneratorCommand()
        {
            Generators.SpawnerGenerator.Spawner s_window = new Generators.SpawnerGenerator.Spawner(cbhk);
            SetCBHKState();
            s_window.Topmost = true;
            s_window.Show();
            s_window.Topmost = false;
        }

        /// <summary>
        /// 启动as生成器
        /// </summary>
        private void StartArmorStandsGeneratorCommand()
        {
            Generators.ArmorStandGenerator.ArmorStand as_window = new Generators.ArmorStandGenerator.ArmorStand(cbhk);
            SetCBHKState();
            as_window.Topmost = true;
            as_window.Show();
            as_window.Topmost = false;
        }

        /// <summary>
        /// 启动tag生成器
        /// </summary>
        private void StartTagsGeneratorCommand()
        {
            Generators.TagGenerator.Tag tag_window = new Generators.TagGenerator.Tag(cbhk);
            SetCBHKState();
            tag_window.Topmost = true;
            tag_window.Show();
            tag_window.Topmost = false;
        }

        /// <summary>
        /// 启动物品生成器
        /// </summary>
        private void StartItemsGeneratorCommand()
        {
            Generators.ItemGenerator.Item item_window = new Generators.ItemGenerator.Item(cbhk);
            SetCBHKState();
            item_window.Topmost = true;
            item_window.Show();
            item_window.Topmost = false;
        }

        /// <summary>
        /// 启动实体生成器
        /// </summary>
        private void StartEntityGeneratorCommand()
        {
            Generators.EntityGenerator.Entity entity_window = new Generators.EntityGenerator.Entity(cbhk);
            SetCBHKState();
            entity_window.Topmost = true;
            entity_window.Show();
            entity_window.Topmost = false;
        }

        /// <summary>
        /// 启动烟花生成器
        /// </summary>
        private void StartFireworksGeneratorCommand()
        {
            Generators.FireworkRocketGenerator.FireworkRocket fireworkRocket = new Generators.FireworkRocketGenerator.FireworkRocket(cbhk);
            SetCBHKState();
            fireworkRocket.Topmost = true;
            fireworkRocket.Show();
            fireworkRocket.Topmost = false;
        }

        /// <summary>
        /// 启动配方生成器
        /// </summary>
        private void StartRecipesGeneratorCommand()
        {
            Generators.RecipeGenerator.Recipe recipe = new Generators.RecipeGenerator.Recipe(cbhk);
            SetCBHKState();
            recipe.Topmost = true;
            recipe.Show();
            recipe.Topmost = false;
        }

        /// <summary>
        /// 启动村民生成器
        /// </summary>
        private void StartVillagersGeneratorCommand()
        {
            Generators.VillagerGenerator.Villager villager = new Generators.VillagerGenerator.Villager(cbhk);
            SetCBHKState();
            villager.Topmost = true;
            villager.Show();
            villager.Topmost = false;
        }

        /// <summary>
        /// 启动成书生成器
        /// </summary>
        private void StartWrittenBooksGeneratorCommand()
        {
            Generators.WrittenBookGenerator.WrittenBook writtenBook = new Generators.WrittenBookGenerator.WrittenBook(cbhk);
            SetCBHKState();
            writtenBook.Topmost = true;
            writtenBook.Show();
            writtenBook.Topmost = false;
        }

        /// <summary>
        /// 启动数据包生成器
        /// </summary>
        private void StartDatapacksGeneratorCommand()
        {
            Generators.DataPackGenerator.Datapack dataPack = new Generators.DataPackGenerator.Datapack(cbhk);
            SetCBHKState();
            dataPack.Topmost = true;
            dataPack.Show();
            dataPack.Topmost = false;
        }

        /// <summary>
        /// 设置主窗体状态
        /// </summary>
        private void SetCBHKState()
        {
            switch (MainWindow.cbhk_visibility)
            {
                case MainWindowProperties.Visibility.MinState:
                    cbhk.WindowState = WindowState.Minimized;
                    break;
                case MainWindowProperties.Visibility.Close:
                    cbhk.WindowState = WindowState.Minimized;
                    cbhk.ShowInTaskbar = false;
                    break;
            }
        }
    }
}

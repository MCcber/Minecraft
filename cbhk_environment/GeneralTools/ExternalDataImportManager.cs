﻿using cbhk_environment.ControlsDataContexts;
using cbhk_environment.CustomControls;
using cbhk_environment.GeneralTools.Displayer;
using cbhk_environment.GeneralTools.Information;
using cbhk_environment.Generators.FireworkRocketGenerator.Components;
using cbhk_environment.Generators.RecipeGenerator;
using cbhk_environment.Generators.RecipeGenerator.Components;
using cbhk_environment.Generators.SpawnerGenerator.Components;
using cbhk_environment.Generators.TagGenerator;
using cbhk_environment.Generators.VillagerGenerator;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace cbhk_environment.GeneralTools
{
    public class ExternalDataImportManager
    {
        #region 处理导入外部标签
        public static void ImportTagDataHandler(string filePathOrData, ref ObservableCollection<TagItemTemplate> itemList,ref tag_datacontext context, bool IsPath = true)
        {
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;

            try
            {
                JObject content = JObject.Parse(data);
                if (content.SelectToken("replace") is JToken replace)
                    context.Replace = replace.ToString().ToString().ToLower() == "true";
                if(content.SelectToken("values") is JArray valuesArray)
                {
                    List<string> values = valuesArray.ToList().ConvertAll(item=>item.ToString().Replace("minecraft:",""));
                    StringBuilder id = new();
                    foreach (var item in itemList)
                    {
                        id.Clear();
                        if (item.DisplayText.Contains(' '))
                            id.Append(item.DisplayText[..item.DisplayText.IndexOf(' ')]);
                        else
                            id.Append(item.DisplayText);
                        item.BeChecked = values.Where(value => value == id.ToString()).Any();
                        if(item.BeChecked.Value)
                        {
                            switch (item.DataType)
                            {
                                case "Item":
                                    context.Items.Add(id.ToString());
                                    break;
                                case "Block&Item":
                                    context.Blocks.Add(id.ToString());
                                    break;
                                case "Entity":
                                    context.Entities.Add(id.ToString());
                                    break;
                                case "GameEvent":
                                    context.GameEvent.Add(id.ToString());
                                    break;
                                case "Biome":
                                    context.Biomes.Add(id.ToString());
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                messageDisplayer.Icon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
            }
        }
        #endregion

        #region 处理导入外部配方
        public static void ImportRecipeDataHandler(string filePathOrData, ref recipe_datacontext recipeContext, bool IsPath = true)
        {
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;
            try
            {
                JObject json = JObject.Parse(data);
                if (json["type"] is JToken recipeType)
                {
                    switch (recipeType.ToString().Replace("minecraft:",""))
                    {
                        case "crafting_shaped":
                        case "crafting_shapeness":
                            {
                                recipe_datacontext.RecipeType type = recipe_datacontext.RecipeType.CraftingTable;
                                CraftingTable craftingTable = recipeContext.AddRecipeCommand(type) as CraftingTable;
                                craftingTableDataContext context = craftingTable.DataContext as craftingTableDataContext;
                                context.ImportMode = true;
                                context.ExternalData = json;
                            }
                            break;
                        case "smithing_transform":
                        case "smithing_trim":
                            {
                                recipe_datacontext.RecipeType type = recipe_datacontext.RecipeType.SmithingTable;
                                SmithingTable smithingTable = recipeContext.AddRecipeCommand(type) as SmithingTable;
                                smithingTableDataContext context = smithingTable.DataContext as smithingTableDataContext;
                                context.ImportMode = true;
                                context.ExternalData = json;
                            }
                            break;
                        case "blasting":
                            {
                                recipe_datacontext.RecipeType type = recipe_datacontext.RecipeType.BlastFurnace;
                                BlastFurnace blastFurnace = recipeContext.AddRecipeCommand(type) as BlastFurnace;
                                blastFurnaceDataContext context = blastFurnace.DataContext as blastFurnaceDataContext;
                                context.ImportMode = true;
                                context.ExternalData = json;
                            }
                            break;
                        case "campfire_cooking":
                            {
                                recipe_datacontext.RecipeType type = recipe_datacontext.RecipeType.Campfire;
                                Campfire campfire = recipeContext.AddRecipeCommand(type) as Campfire;
                                campfireDataContext context = campfire.DataContext as campfireDataContext;
                                context.ImportMode = true;
                                context.ExternalData = json;
                            }
                            break;
                        case "smelting":
                            {
                                recipe_datacontext.RecipeType type = recipe_datacontext.RecipeType.Furnace;
                                Furnace furnace = recipeContext.AddRecipeCommand(type) as Furnace;
                                furnaceDataContext context = furnace.DataContext as furnaceDataContext;
                                context.ImportMode = true;
                                context.ExternalData = json;
                            }
                            break;
                        case "smoker":
                            {
                                recipe_datacontext.RecipeType type = recipe_datacontext.RecipeType.Smoker;
                                Smoker smoker = recipeContext.AddRecipeCommand(type) as Smoker;
                                smokerDataContext context = smoker.DataContext as smokerDataContext;
                                context.ImportMode = true;
                                context.ExternalData = json;
                            }
                            break;
                        case "stonecutting":
                            {
                                recipe_datacontext.RecipeType type = recipe_datacontext.RecipeType.Stonecutter;
                                Stonecutter stonecutter = recipeContext.AddRecipeCommand(type) as Stonecutter;
                                stonecutterDataContext context = stonecutter.DataContext as stonecutterDataContext;
                                context.ImportMode = true;
                                context.ExternalData = json;
                            }
                            break;
                    }
                }
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                messageDisplayer.Icon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
            }
        }
        #endregion

        #region 处理导入外部刷怪笼
        public static void ImportSpawnerDataHandler(string filePathOrData, ref ObservableCollection<RichTabItems> itemPageList, bool IsPath = true)
        {
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;

            #region 提取可用NBT数据
            string nbtData = "";
            if (data.Contains('{') && data.Contains('}'))
                nbtData = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];
            //补齐缺失双引号对的key
            nbtData = Regex.Replace(nbtData, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            nbtData = Regex.Replace(nbtData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
            #endregion

            try
            {
                JObject nbtObj = JObject.Parse(nbtData);
                //确定版本
                bool version1_12 = Regex.IsMatch(data, @"^/setblock (minecraft:)?mob_spawner");
                AddSpawnerData(nbtObj,version1_12?"1.12-":"1.13+",itemPageList);
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                messageDisplayer.Icon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
            }
        }
        public static string GetSpawnerDataHandler(string filePathOrData, bool IsPath = true)
        {
            string result = "";
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;

            if (data.Length == 0) return result;

            #region 提取可用NBT数据和实体ID
            if (data.Contains('{') && data.Contains('}'))
                result = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];

            if (result.Length > 0)
            {
                //补齐缺失双引号对的key
                result = Regex.Replace(result, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
                //清除数值型数据的单位
                result = Regex.Replace(result, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
            }

            return result;
            #endregion
        }

        /// <summary>
        /// 添加刷怪笼
        /// </summary>
        /// <param name="nbtObj"></param>
        /// <param name="itemPageList"></param>
        private static void AddSpawnerData(JObject nbtObj,string version, ObservableCollection<RichTabItems> itemPageList)
        {
            SpawnerPage spawnerPage = new() { FontWeight = FontWeights.Normal };
            RichTabItems richTabItems = new()
            {
                Header = "刷怪笼",
                IsContentSaved = true,
                Content = spawnerPage,
                Style = Application.Current.Resources["RichTabItemStyle"] as Style
            };
            spawnerPageDataContext context = spawnerPage.DataContext as spawnerPageDataContext;
            itemPageList.Add(richTabItems);
            context.SelectedVersion = version;

            #region 开启导入模式，为外部数据赋值
            context.ExternalSpawnerData = nbtObj;
            context.ImportMode = true;
            #endregion

            #region 处理潜在实体数据
            if (nbtObj.SelectToken("SpawnPotentials") is JArray spawnPotentials)
            {
                foreach (JObject spawnPotential in spawnPotentials.Cast<JObject>())
                {
                    context.AddSpawnPotentialCommand(null);
                    SpawnPotential spawnPotentialInstance = context.SpawnPotentials[^1];
                    if (spawnPotential.SelectToken("weight") is JToken weight)
                        spawnPotentialInstance.weight.Value = short.Parse(weight.ToString());

                    #region 方块光限制
                    if (spawnPotential.SelectToken("data.custom_spawn_rules.block_light_limit") is JArray BlockLightArray)
                    {
                        spawnPotentialInstance.BlockLightValueType.IsChecked = false;
                        spawnPotentialInstance.UseDefaultBlockLightValue.IsChecked = false;
                        spawnPotentialInstance.BlockLightRange.Visibility = Visibility.Visible;
                        spawnPotentialInstance.BlockLightValue.Visibility = Visibility.Collapsed;
                        spawnPotentialInstance.BlockLightMinValue.Value = int.Parse(BlockLightArray[0].ToString());
                        spawnPotentialInstance.BlockLightMaxValue.Value = int.Parse(BlockLightArray[1].ToString());
                    }
                    else
                        if (spawnPotential.SelectToken("data.custom_spawn_rules.block_light_limit") is JToken BlockLight)
                    {
                        spawnPotentialInstance.BlockLightValueType.IsChecked = true;
                        spawnPotentialInstance.UseDefaultBlockLightValue.IsChecked = false;
                        spawnPotentialInstance.BlockLightRange.Visibility = Visibility.Collapsed;
                        spawnPotentialInstance.BlockLightValue.Visibility = Visibility.Visible;
                        spawnPotentialInstance.BlockLightValue.Value = int.Parse(BlockLight.ToString());
                    }
                    #endregion

                    #region 天空光限制
                    if (spawnPotential.SelectToken("data.custom_spawn_rules.sky_light_limit") is JArray SkyLightArray)
                    {
                        spawnPotentialInstance.SkyLightValueType.IsChecked = false;
                        spawnPotentialInstance.UseDefaultSkyLightValue.IsChecked = false;
                        spawnPotentialInstance.SkyLightRange.Visibility = Visibility.Visible;
                        spawnPotentialInstance.SkyLightValue.Visibility = Visibility.Collapsed;
                        spawnPotentialInstance.SkyLightMinValue.Value = int.Parse(SkyLightArray[0].ToString());
                        spawnPotentialInstance.SkyLightMaxValue.Value = int.Parse(SkyLightArray[1].ToString());
                    }
                    else
                        if (spawnPotential.SelectToken("data.custom_spawn_rules.sky_light_limit") is JToken SkyLight)
                    {
                        spawnPotentialInstance.SkyLightValueType.IsChecked = true;
                        spawnPotentialInstance.UseDefaultSkyLightValue.IsChecked = false;
                        spawnPotentialInstance.SkyLightRange.Visibility = Visibility.Collapsed;
                        spawnPotentialInstance.SkyLightValue.Visibility = Visibility.Visible;
                        spawnPotentialInstance.SkyLightValue.Value = int.Parse(SkyLight.ToString());
                    }
                    #endregion

                    #region 实体数据
                    if (spawnPotential.SelectToken("data.entity") is JObject entity)
                    {
                        string data = entity.ToString();
                        spawnPotentialInstance.entity.Tag = data;
                        string entityID = JObject.Parse(data)["id"].ToString().Replace("minecraft:","");
                        string rootPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\entityImages\\";
                        string iconPath = rootPath + entityID + ".png";
                        if (!File.Exists(iconPath))
                            iconPath = rootPath + entityID + "_spawn_egg.png";
                        spawnPotentialInstance.entity.EntityIcon.Source = new BitmapImage(new Uri(iconPath));
                    }
                    #endregion
                }
            }
            #endregion

            TabControl tabControl = richTabItems.FindParent<TabControl>();
            tabControl.SelectedIndex = itemPageList.Count - 1;
        }
        #endregion

        #region 处理导入外部实体数据
        public static string GetEntityDataHandler(string filePathOrData, bool IsPath = true)
        {
            string result = "";
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;
            string entityID = "";

            //召唤实体物品
            if (Regex.IsMatch(data, @"^/?summon"))
                entityID = Regex.Match(data, @"(?<=/?summon\s)([\w:]+)").ToString();

            if (data.Length == 0) return result;

            #region 提取可用NBT数据和实体ID
            if (data.Contains('{') && data.Contains('}'))
                result = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];

            if(result.Length > 0)
            {
                //补齐缺失双引号对的key
                result = Regex.Replace(result, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
                //清除数值型数据的单位
                result = Regex.Replace(result, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");

                #region 插入实体ID
                JObject resultObj = JObject.Parse(result);
                JToken idObj = resultObj.SelectToken("id");
                idObj ??= resultObj.SelectToken("EntityTag.id");
                if (idObj == null)
                    resultObj.Add("id", "\"minecraft:" + entityID + "\"");
                    result = resultObj.ToString();
                #endregion
            }

            return result;
            #endregion
        }
        
        /// <summary>
        /// 导入村民数据
        /// </summary>
        public static void ImportVillagerDataHandler(string filePathOrData,villager_datacontext context, bool IsPath = true)
        {
            MessageDisplayer messageDisplayer = new();
            messageDisplayerDataContext displayContext = messageDisplayer.DataContext as messageDisplayerDataContext;
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;

            #region 提取可用NBT数据和村民ID
            string nbtData = "";
            if (data.Contains('{') && data.Contains('}'))
                nbtData = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];
            //补齐缺失双引号对的key
            nbtData = Regex.Replace(nbtData, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            nbtData = Regex.Replace(nbtData, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");
            #endregion

            try
            {
                JObject nbtObj = JObject.Parse(nbtData);
                if (nbtObj.SelectToken("EntitTag") is JObject entityTag)
                    nbtObj = JObject.Parse(entityTag.ToString());
                AddVillagerData(nbtObj,context);
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                messageDisplayer.Icon = displayContext.errorIcon;
                displayContext.MessageTitle = "导入失败";
                _ = messageDisplayer.ShowDialog();
            }
        }

        /// <summary>
        /// 设置村民数据
        /// </summary>
        /// <param name="nbtObj"></param>
        /// <param name="context"></param>
        private static void AddVillagerData(JObject nbtObj,villager_datacontext context)
        {
            #region 处理交易数据
            if(nbtObj.SelectToken("Offers.Recipes") is JArray Recipes)
            {
                string rootPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\data_sources\\item_and_block_images\\";
                foreach (JObject recipe in Recipes.Cast<JObject>())
                {
                    #region 读取数据
                    #region buy,buyB,sell
                    string buyID = "";
                    JToken buyCountObj = recipe.SelectToken("buy.Count");

                    string buyBID = "";
                    JToken buyBCountObj = recipe.SelectToken("buyB.Count");

                    string sellID = "";
                    JToken sellCountObj = recipe.SelectToken("sell.Count");
                    #endregion
                    #region other
                    JToken demand = recipe.SelectToken("demand");
                    JToken maxUses = recipe.SelectToken("maxUses");
                    JToken priceMultiplier = recipe.SelectToken("priceMultiplier");
                    JToken rewardExp = recipe.SelectToken("rewardExp");
                    JToken specialPrice = recipe.SelectToken("specialPrice");
                    JToken uses = recipe.SelectToken("uses");
                    JToken xp = recipe.SelectToken("xp");
                    #endregion
                    #endregion

                    #region 应用数据
                    #region buy
                    if (recipe.SelectToken("buy.id") is JToken buyIDObj)
                        buyID = buyIDObj.ToString().Replace("minecraft:", "");
                    bool ExistItem = false;
                    if (buyID.Length > 0)
                    {
                        ExistItem = true;
                        context.AddTransactionItemCommand();
                        string iconPath = rootPath + buyID + ".png";
                        if (File.Exists(iconPath))
                            context.transactionItems[^1].Buy.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));

                        if (buyCountObj != null)
                            context.transactionItems[^1].ModifyBuyItemCount.Value = int.Parse(buyCountObj.ToString());
                        Uri iconUri = new(iconPath, UriKind.Absolute);
                        ItemStructure imageTag = new(iconUri, buyID, recipe.SelectToken("buy.tag") is JObject buyTagObj ? buyTagObj.ToString() : "");
                        context.transactionItems[^1].Buy.Source = new BitmapImage(iconUri);
                        context.transactionItems[^1].Buy.Tag = imageTag;
                    }
                    #endregion
                    #region buyB
                    if(ExistItem)
                    {
                        if (recipe.SelectToken("buyB.id") is JToken buyBIDObj)
                            buyBID = buyBIDObj.ToString().Replace("minecraft:", "");
                        if (buyBID.Length > 0)
                        {
                            string iconPath = rootPath + buyBID + ".png";
                            if (File.Exists(iconPath))
                                context.transactionItems[^1].BuyB.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));

                            if (buyBCountObj != null)
                                context.transactionItems[^1].ModifyBuyBItemCount.Value = int.Parse(buyCountObj.ToString());
                            Uri iconUri = new(iconPath, UriKind.Absolute);
                            ItemStructure imageTag = new(iconUri, buyBID, recipe.SelectToken("buyB.tag") is JObject buyTagObj ? buyTagObj.ToString() : "");
                            context.transactionItems[^1].BuyB.Source = new BitmapImage(iconUri);
                            context.transactionItems[^1].BuyB.Tag = imageTag;
                        }
                    }
                    #endregion
                    #region sell
                    if (ExistItem)
                    {
                        if (recipe.SelectToken("sell.id") is JToken sellIDObj)
                            sellID = sellIDObj.ToString().Replace("minecraft:", "");
                        if (sellID.Length > 0)
                        {
                            string iconPath = rootPath + sellID + ".png";
                            if (File.Exists(iconPath))
                                context.transactionItems[^1].Sell.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));

                            if (sellCountObj != null)
                                context.transactionItems[^1].ModifySellItemCount.Value = int.Parse(sellCountObj.ToString());
                            Uri iconUri = new(iconPath, UriKind.Absolute);
                            ItemStructure imageTag = new(iconUri, sellID, recipe.SelectToken("sell.tag") is JObject sellTagObj ? sellTagObj.ToString() : "");
                            context.transactionItems[^1].Sell.Source = new BitmapImage(iconUri);
                            context.transactionItems[^1].Sell.Tag = imageTag;
                        }
                    }
                    #endregion
                    #region other
                    if (ExistItem)
                    {
                        context.transactionItems[^1].Demand = demand.ToString();
                        context.transactionItems[^1].MaxUses = maxUses.ToString();
                        context.transactionItems[^1].PriceMultiplier = priceMultiplier.ToString();
                        context.transactionItems[^1].RewardExp = rewardExp.ToString() == "1" || rewardExp.ToString() == "true";
                        context.transactionItems[^1].SpecialPrice = specialPrice.ToString();
                        context.transactionItems[^1].Uses = uses.ToString();
                        context.transactionItems[^1].Xp = xp.ToString();
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion

            #region 处理言论数据
            if (nbtObj.SelectToken("Gossips") is JArray Gossips)
            {
                foreach (JObject gossip in Gossips.Cast<JObject>())
                {
                    context.AddGossipItemCommand();
                    JArray targetUID = gossip.SelectToken("Target") as JArray;
                    JObject type = gossip.SelectToken("Type") as JObject;
                    JToken value = gossip.SelectToken("Value");
                    if (targetUID != null)
                        context.gossipItems[^1].Target.Text = targetUID[0].ToString() + "," + targetUID[1].ToString() + "," + targetUID[2].ToString() + "," + targetUID[3].ToString();
                    if (type != null)
                        context.gossipItems[^1].Type.SelectedValue = type.ToString();
                    if (value != null)
                        context.gossipItems[^1].Value.Value = int.Parse(value.ToString());
                }
            }
            #endregion

            #region 处理种类、职业、等级等数据
            if (nbtObj.SelectToken("VillagerData") is JObject VillagerData)
            {
                JToken level = VillagerData.SelectToken("level");
                JToken profession = VillagerData.SelectToken("profession");
                JToken type = VillagerData.SelectToken("type");
                if(level != null)
                context.VillagerLevel = level.ToString();
                if(profession != null)
                    context.VillagerProfessionType = context.VillagerProfessionTypeDataBase.Where(item => item.Key == profession.ToString().Replace("minecraft:","")).First().Value;
                if(type != null)
                    context.VillagerType = context.VillagerTypeDataBase.Where(item => item.Key == type.ToString().Replace("minecraft:", "")).First().Value;
            }
            #endregion

            #region 交配意愿
            if (nbtObj.SelectToken("Willing") is JToken Willing)
                context.Willing = Willing.ToString() == "1" || Willing.ToString() == "true";
            #endregion

            #region 补货间隔
            if (nbtObj.SelectToken("LastRestock") is JToken LastRestock)
                context.LastRestock = double.Parse(LastRestock.ToString());
            #endregion

            #region 当前经验
            if (nbtObj.SelectToken("Xp") is JToken Xp)
                context.Xp = int.Parse(Xp.ToString());
            #endregion
        }

        /// <summary>
        /// 导入实体数据
        /// </summary>
        /// <param name="filePathOrData"></param>
        /// <param name="itemPageList"></param>
        /// <param name="IsPath"></param>
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
                messageDisplayer.Icon = displayContext.errorIcon;
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
                messageDisplayer.Icon = displayContext.errorIcon;
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
                messageDisplayer.Icon = displayContext.errorIcon;
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
                TabControl tabControl = richTabItems.FindParent<TabControl>();
                tabControl.SelectedIndex = 0;
            }
        }
        #endregion

        #region 处理导入外部物品数据
        public static string GetItemDataHandler(string filePathOrData, bool IsPath = true)
        {
            string result = "";
            string data = IsPath ? File.ReadAllText(filePathOrData) : filePathOrData;
            string itemID = "";

            //给予物品
            if (Regex.IsMatch(data, @"^/?give (@[apesr])|(\w+) ([\w]+_)?"))
                itemID = Regex.Match(data, @"(?<=/?give\s@[apesr]\s)(\w+)").ToString();

            if (data == null) return result;

            #region 提取可用NBT数据和物品ID
            if (data.Contains('{') && data.Contains('}'))
                result = data[data.IndexOf('{')..(data.LastIndexOf('}') + 1)];

            //补齐缺失双引号对的key
            result = Regex.Replace(result, @"([\{\[,])([\s+]?\w+[\s+]?):", "$1\"$2\":");
            //清除数值型数据的单位
            result = Regex.Replace(result, @"(\d+[\,\]\}]?)([a-zA-Z])", "$1").Replace("I;", "");

            if (result.Length > 0)
            {
                #region 插入物品ID
                JObject resultObj = JObject.Parse(result);
                if (resultObj.SelectToken("Item") is not JObject)
                {
                    if(!data.StartsWith('{') && !data.EndsWith('}'))
                    {
                        int itemCount = int.Parse(Regex.Match(data, @"\d+$").ToString());
                        resultObj = JObject.Parse("{Count:" + itemCount + ",tag:" + result.Replace("\n", "").Replace("\r", "") + "}");
                        resultObj.Add("id", "minecraft:" + itemID);
                    }
                }
                else
                    if(resultObj.SelectToken("Item") is JObject itemObj)
                    resultObj = JObject.Parse(itemObj.ToString());
                    result = Regex.Replace(resultObj.ToString(),@"\s+","");
                #endregion
            }

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
            if (nbtData.Length == 0)
            {
                displayContext.DisplayInfomation = "该指令内容与物品生成无关";
                displayContext.MessageTitle = "导入失败";
                messageDisplayer.Icon = displayContext.errorIcon;
                _ = messageDisplayer.ShowDialog();
                itemPageList.RemoveAt(itemPageList.Count - 1);
                return;
            }

            try
            {
                JToken itemTagID = JObject.Parse(nbtData).SelectToken("Item.id");
                itemTagID ??= JObject.Parse(nbtData).SelectToken("id");
                if (itemTagID != null && itemID.Length == 0)
                    itemID = itemTagID.ToString();
                //过滤掉命名空间
                itemID = Regex.Replace(itemID, @"[\w\\/\.]+:", "").Trim();
            }
            catch
            {
                displayContext.DisplayInfomation = "文件内容格式不合法";
                messageDisplayer.Icon = displayContext.errorIcon;
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
                messageDisplayer.Icon = displayContext.errorIcon;
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

            if (mode == "Summon")
                itemPages.Summon = true;
            else
                itemPages.Give = true;
            if (filePath.Length > 0 && File.Exists(filePath))
                itemPages.ExternFilePath = filePath;
            if (externData != null)
            {
                itemPages.ImportMode = true;
                itemPages.ExternallyReadEntityData = externData;
            }
            if (version1_12)
                itemPages.SelectedVersion = "1.12-";
            IconComboBoxItem itemId = MainWindow.ItemIdSource.Where(item => item.ComboBoxItemText == selectedItemID).First();
            itemPages.SelectedItemId = itemId;

            richTabItems.Content = itemPages;
            itemPageList.Add(richTabItems);
            if (itemPageList.Count == 1)
            {
                TabControl tabControl = richTabItems.FindParent<TabControl>();
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
                messageDisplayer.Icon = displayContext.errorIcon;
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
                messageDisplayer.Icon = displayContext.errorIcon;
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
                messageDisplayer.Icon = displayContext.errorIcon;
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
                TabControl tabControl = richTabItems.FindParent<TabControl>();
                tabControl.SelectedIndex = 0;
            }
            richTabItems.Content = itemPages;
        }
        #endregion
    }
}

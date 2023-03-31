using System;

namespace cbhk_environment.GeneralTools.Displayer
{
    /// <summary>
    /// 物品数据结构(用于快速查找中英文以提升搜索效率)
    /// </summary>
    public class ItemStructure
    {
        public Uri ImagePath { get; set; }
        public string IDAndName { get; set; }

        public ItemStructure(Uri ImagePath, string IDAndName)
        {
            this.ImagePath = ImagePath;
            this.IDAndName = IDAndName;
        }
    }
}

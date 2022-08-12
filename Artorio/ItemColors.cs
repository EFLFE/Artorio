using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Artorio
{
    internal static class ItemColors
    {
        private static readonly ItemData[] items;

        static ItemColors()
        {
            // вики врёт
            items = new ItemData[]
            {
                // floors
                new ItemData("stone-path", Color.FromRgb(80,82,72), ItemTypeEnum.Floor),
                new ItemData("concrete", Color.FromRgb(56, 56, 56), ItemTypeEnum.Floor),
                new ItemData("refined-concrete-left", Color.FromRgb(48, 48, 40), ItemTypeEnum.Floor),
                new ItemData("hazard-concrete-left", Color.FromRgb(176, 138, 32), ItemTypeEnum.Floor),
                new ItemData("refined-hazard-concrete-left", Color.FromRgb(112, 90, 24) , ItemTypeEnum.Floor),

                // objects
                new ItemData("wooden-chest", 0, 96, 144, ItemTypeEnum.Entity),
                new ItemData("iron-chest", 0, 96, 144, ItemTypeEnum.Entity),
                new ItemData("steel-chest", 0, 96, 144, ItemTypeEnum.Entity),
                new ItemData("transport-belt", 200, 160, 64, ItemTypeEnum.Entity),
                new ItemData("fast-transport-belt", 200, 160, 64, ItemTypeEnum.Entity),
                new ItemData("express-transport-belt", 200, 160, 64, ItemTypeEnum.Entity),
                new ItemData("underground-belt", 112, 88, 0, ItemTypeEnum.Entity),
                new ItemData("fast-underground-belt", 112, 88, 0, ItemTypeEnum.Entity),
                new ItemData("express-underground-belt", 112, 88, 0, ItemTypeEnum.Entity),
                new ItemData("pipe", 64, 130, 160, ItemTypeEnum.Entity),
                new ItemData("heat-pipe", 56, 130, 168, ItemTypeEnum.Entity),
                new ItemData("stone-wall", 200, 216, 200, ItemTypeEnum.Entity),
                new ItemData("gate", 128, 128, 128, ItemTypeEnum.Entity),

                // objects 2x2
                new ItemData("accumulator", 120, 122, 120, ItemTypeEnum.Entity_2x2),
                new ItemData("gun-turret", 200, 162, 24, ItemTypeEnum.Entity_2x2),
                new ItemData("laser-turret", 216, 42, 40, ItemTypeEnum.Entity_2x2),

                // objects 3x3
                new ItemData("storage-tank", 128, 162, 184, ItemTypeEnum.Entity_3x3),
                new ItemData("solar-panel", 24, 32, 32, ItemTypeEnum.Entity_3x3),

                //   new ItemData("", , ItemTypeEnum.Object),
            };
        }

        public static IEnumerable<ItemData> ForEachItemData()
        {
            for (int i = 0; i < items.Length; i++)
            {
                yield return items[i];
            }
            yield break;
        }

        public static bool FindItem(string internalName, out ItemData data)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].InternalName.Equals(internalName, StringComparison.Ordinal))
                {
                    data = items[i];
                    return true;
                }
            }
            data = null;
            return false;
        }

    }

    public sealed class ItemData
    {
        public readonly string InternalName;
        public readonly Color MapColor;
        public readonly ItemTypeEnum ItemType;

        public ItemData(string internalName, Color mapColor, ItemTypeEnum itemType)
        {
            InternalName = internalName;
            MapColor = mapColor;
            ItemType = itemType;
        }

        public ItemData(string internalName, byte r, byte g, byte b, ItemTypeEnum itemType)
        {
            InternalName = internalName;
            MapColor = Color.FromRgb(r, g, b);
            ItemType = itemType;
        }
    }

    public enum ItemTypeEnum
    {
        Floor,
        Entity,
        Entity_2x2,
        Entity_3x3,
    }
}

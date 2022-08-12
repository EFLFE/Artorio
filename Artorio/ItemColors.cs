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
            items = new ItemData[]
            {
                // floors
                new ItemData("stone-path", Color.FromRgb(80,82,72), ItemTypeEnum.Floor),
                new ItemData("concrete", Color.FromRgb(56, 56, 56), ItemTypeEnum.Floor),
                new ItemData("refined-concrete", Color.FromRgb(48, 48, 40), ItemTypeEnum.Floor),
                new ItemData("hazard-concrete", Color.FromRgb(176, 138, 32), ItemTypeEnum.Floor),
                new ItemData("refined-hazard-concrete", Color.FromRgb(112, 90, 24) , ItemTypeEnum.Floor),

                // objects
                new ItemData("wooden-chest", 0, 96, 144, ItemTypeEnum.Object),
                new ItemData("iron-chest", 0, 96, 144, ItemTypeEnum.Object),
                new ItemData("steel-chest", 0, 96, 144, ItemTypeEnum.Object),
                new ItemData("transport-belt", 200, 160, 64, ItemTypeEnum.Object),
                new ItemData("fast-transport-belt", 200, 160, 64, ItemTypeEnum.Object),
                new ItemData("express-transport-belt", 200, 160, 64, ItemTypeEnum.Object),
                new ItemData("underground-belt", 112, 88, 0, ItemTypeEnum.Object),
                new ItemData("fast-underground-belt", 112, 88, 0, ItemTypeEnum.Object),
                new ItemData("express-underground-belt", 112, 88, 0, ItemTypeEnum.Object),
                new ItemData("pipe", 64, 130, 160, ItemTypeEnum.Object),
                new ItemData("heat-pipe", 56, 130, 168, ItemTypeEnum.Object),
                new ItemData("stone-wall", 200, 216, 200, ItemTypeEnum.Object),
                new ItemData("gate", 128, 128, 128, ItemTypeEnum.Object),

                // objects 2x2
                new ItemData("accumulator", 120, 122, 120, ItemTypeEnum.Object_2x2),
                new ItemData("gun-turret", 200, 162, 24, ItemTypeEnum.Object_2x2),
                new ItemData("laser-turret", 216, 42, 40, ItemTypeEnum.Object_2x2),

                // objects 3x3
                new ItemData("storage-tank", 128, 162, 184, ItemTypeEnum.Object_3x3),
                new ItemData("solar-panel", 24, 32, 32, ItemTypeEnum.Object_3x3),

                //   new ItemData("", , ItemTypeEnum.Object),
            };
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

    internal sealed class ItemData
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

    internal enum ItemTypeEnum
    {
        Floor,
        Object,
        Object_2x2,
        Object_3x3,
    }
}

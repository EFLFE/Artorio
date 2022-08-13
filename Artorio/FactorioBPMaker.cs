using Artorio.EFPng;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using zlib;
using static System.Net.WebRequestMethods;

namespace Artorio
{
    /// <summary>
    /// Factorio blueprint maker.
    /// </summary>
    public sealed class FactorioBPMaker
    {
        private PngData png;
        private StringBuilder sb;
        private StringBuilder sbTiles;
        private StringBuilder sbEntities;
        private List<IReadOnlyFilterConfig> filterConfigs;
        private int items;
        private int entityNumber;
        private bool[,] mask;

        private CultureInfo occultism;
        private ItemData[] generatedDataPreview;

        public ItemData[] GeneratedDataPreview => generatedDataPreview;

        public PngData LoadedPng => png;

        public bool HaveFilterConfig => filterConfigs.Count > 0;

        public int CurrentProgress { get; private set; }

        public int TargetProgress { get; private set; }

        public FactorioBPMaker()
        {
            sb = new StringBuilder();
            sbTiles = new StringBuilder();
            sbEntities = new StringBuilder();
            filterConfigs = new List<IReadOnlyFilterConfig>();

            occultism = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            occultism.NumberFormat.NumberDecimalSeparator = ".";
        }

        public void AddFilterConfig(IReadOnlyFilterConfig filter) => filterConfigs.Add(filter);

        public void ClearFilterConfig() => filterConfigs.Clear();

        public void LoadImage(string path, out bool loadSuccess, out string errorOrWarning)
        {
            loadSuccess = true;
            errorOrWarning = null;

            try
            {
                png = new PngData(path);

                // warning for big size
                int size = png.GetPixelWidth * png.GetPixelHeight;
                if (size >= 1048576) // 1024*1024
                {
                    errorOrWarning = "The size of the image is extreme! Next steps will be at your own risk.";
                }
            }
            catch (Exception ex)
            {
                loadSuccess = false;
                errorOrWarning = ex.Message;
            }

        }

        public string CreateBlueprint(out int insertItems)
        {
            if (filterConfigs.Count == 0)
                throw new Exception("Missing filter configs.");

            if (png == null)
                throw new Exception("Image not loaded.");

            string output = null;
            items = 0;
            entityNumber = 0;
            insertItems = 0;

            sb.Clear();
            sbTiles.Clear();
            sbEntities.Clear();

            sb.AppendLine("{");
            sb.AppendLine("  'blueprint': {");
            sb.AppendLine("    'icons': [");
            sb.AppendLine("      {");
            sb.AppendLine("        'signal': {");
            sb.AppendLine("          'type': 'item',");
            sb.AppendLine("          'name': 'stone-brick'");
            sb.AppendLine("        },");
            sb.AppendLine("        'index': 1");
            sb.AppendLine("      }");
            sb.AppendLine("    ],");

            sbTiles.AppendLine("    'tiles': [ ");
            sbEntities.AppendLine("    'entities': [ ");

            int lastCount = sb.Length + sbTiles.Length + sbEntities.Length;
            generatedDataPreview = new ItemData[png.GetDataWidth * png.GetDataHeight];

            int hw = png.GetPixelWidth / 2;
            int hh = png.GetPixelHeight / 2;
            CurrentProgress = 0;
            TargetProgress = png.GetDataHeight * (png.GetDataWidth / 4) + 1;
            mask = new bool[(png.GetPixelWidth + 4), (png.GetPixelHeight + 4)]; // +4 to avoid checks

            for (int y = 0; y < png.GetDataHeight; y++)
            {
                for (int x = 0, xiter = 0; x < png.GetDataWidth; x += 4, xiter++)
                {
                    var pixel = png.GetPixelData(x, y);

                    // parse
                    foreach (var filter in filterConfigs)
                    {
                        if (filter.Item.ItemType != ItemTypeEnum.Floor)
                        {
                            switch (filter.Item.ItemType)
                            {
                                case ItemTypeEnum.Entity:
                                    if (mask[xiter, y])
                                        continue;
                                    break;

                                case ItemTypeEnum.Entity_2x2:
                                    if (mask[xiter, y] ||
                                        mask[xiter + 1, y] ||
                                        mask[xiter, y + 1] ||
                                        mask[xiter + 1, y + 1])
                                        continue;
                                    break;

                                case ItemTypeEnum.Entity_3x3:
                                    for (int ox = 0; ox < 3; ox++)
                                        for (int oy = 0; oy < 3; oy++)
                                            if (mask[xiter + ox, y + oy])
                                                continue;
                                    break;
                            }
                        }

                        if (filter.UseRangeColor)
                        {
                            if (pixel.R >= filter.FromColor.R && pixel.R <= filter.ToColor.R &&
                                pixel.G >= filter.FromColor.G && pixel.G <= filter.ToColor.G &&
                                pixel.B >= filter.FromColor.B && pixel.B <= filter.ToColor.B)
                            { }
                            else continue;
                        }
                        else if (!CompareRGBColors(pixel, filter.FromColor))
                        {
                            continue;
                        }

                        InsertJSONItem(xiter, hw, y, hh, filter.Item);

                        switch (filter.Item.ItemType)
                        {
                            case ItemTypeEnum.Entity_2x2:
                                mask[xiter, y] = true;
                                mask[xiter + 1, y] = true;
                                mask[xiter, y + 1] = true;
                                mask[xiter + 1, y + 1] = true;
                                break;

                            case ItemTypeEnum.Entity_3x3:
                                for (int ox = 0; ox < 3; ox++)
                                    for (int oy = 0; oy < 3; oy++)
                                        mask[xiter + ox, y + oy] = true;
                                break;

                            case ItemTypeEnum.Entity:
                                mask[xiter, y] = true;
                                break;
                        }
                        break;
                    }

                    CurrentProgress++;
                }


            }

            if (lastCount == sb.Length)
            {
                // "No pixel found.";
            }
            else
            {
                int len = Environment.NewLine.Length;

                // последняя запятая
                sbTiles.Remove(sbTiles.Length - len - 1, 1);
                sbEntities.Remove(sbEntities.Length - len - 1, 1);

                sbTiles.AppendLine("    ],");
                sbEntities.AppendLine("    ],");

                sb.AppendLine(sbTiles.ToString());
                sb.AppendLine(sbEntities.ToString());

                sb.AppendLine("    'item': 'blueprint',");
                sb.AppendLine("    'version': 73015558146");
                sb.AppendLine("  } }");

                sb.Replace('\'', '"');

                output = CreateBlueprintFormat();
            }

            insertItems = items;
            CurrentProgress++;
            return output;
        }

        private bool CompareRGBAColors(Color pngColor, System.Windows.Media.Color wpfColor)
        {
            return
                pngColor.R == wpfColor.R &&
                pngColor.G == wpfColor.G &&
                pngColor.B == wpfColor.B &&
                pngColor.A == wpfColor.A;
        }

        private bool CompareRGBColors(Color pngColor, System.Windows.Media.Color wpfColor)
        {
            return
                pngColor.R == wpfColor.R &&
                pngColor.G == wpfColor.G &&
                pngColor.B == wpfColor.B;
        }

        private void InsertJSONItem(int xiter, int hw, int y, int hh, ItemData itemData)
        {
            items++;
            generatedDataPreview[xiter + y * png.GetPixelWidth] = itemData;

            if (itemData.ItemType == ItemTypeEnum.Floor)
            {
                sbTiles.AppendLine("      {");
                sbTiles.AppendLine("        'position': {");
                sbTiles.AppendLine($"          'x': {xiter - hw},");
                sbTiles.AppendLine($"          'y': {y - hh}");
                sbTiles.AppendLine("        },");
                sbTiles.AppendLine($"        'name': '{itemData.InternalName}'");
                sbTiles.AppendLine("      },");
            }
            else
            {
                // Entity
                entityNumber++;

                sbEntities.AppendLine(" {");
                sbEntities.AppendLine($" 'entity_number': {entityNumber},");
                sbEntities.AppendLine($" 'name': '{itemData.InternalName}',");
                sbEntities.AppendLine(" 'position': {");
                sbEntities.AppendLine($"  'x': {(xiter - (hw - 0.5)).ToString(occultism)},"); // "x": -0.5,
                sbEntities.AppendLine($"  'y': {(y - (hh - 0.5)).ToString(occultism)}");
                sbEntities.AppendLine(" }},");
            }
        }

        private string CreateBlueprintFormat()
        {
            // https://wiki.factorio.com/Blueprint_string_format
            /*
             * A blueprint string is a JSON representation of the blueprint, compressed with zlib deflate
             * and then encoded using base64 with a version byte in front. The version byte is currently 0
             * for vanilla 0.15 and 0.16. So to get the JSON representation of a blueprint from a blueprint
             * string, skip the first byte, base64 decode the string, and finally decompress using zlib inflate.
            */

            byte[] inData = Encoding.UTF8.GetBytes(sb.ToString());
            CompressData(inData, out byte[] outData);
            string result = Convert.ToBase64String(outData, Base64FormattingOptions.None);
            return "0" + result;
        }

        private void CompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
        }

        public static string Decompress(string blueprint)
        {
            string text = blueprint;
            string output = null;

            if (string.IsNullOrWhiteSpace(text) || text[0] != '0')
                return null;

            byte[] inData = Convert.FromBase64String(text.Substring(1));
            DecompressData(inData, out byte[] outData);
            output = Encoding.UTF8.GetString(outData);

            return output;
        }

        private static void DecompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

    }
}

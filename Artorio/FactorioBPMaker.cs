﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Artorio.EFPng;
using zlib;

namespace Artorio
{
    /// <summary>
    /// Factorio blueprint maker.
    /// </summary>
    sealed class FactorioBPMaker
    {
        private PngData png;
        private StringBuilder sb;
        private List<IReadOnlyFilterConfig> filterConfigs;
        private int items;

        public bool HaveFilterConfig => filterConfigs.Count > 0;

        public int CurrentProgress { get; private set; }

        public int TargetProgress { get; private set; }

        public FactorioBPMaker()
        {
            sb = new StringBuilder();
            filterConfigs = new List<IReadOnlyFilterConfig>();
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
            insertItems = 0;

            sb.Clear();

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
            sb.AppendLine("    'tiles': [");

            int lastCount = sb.Length;

            int hw = png.GetPixelWidth / 2;
            int hh = png.GetPixelHeight / 2;
            CurrentProgress = 0;
            TargetProgress = png.GetDataHeight * (png.GetDataWidth / 4) + 1;

            for (int y = 0; y < png.GetDataHeight; y++)
            {
                for (int x = 0, xx = 0; x < png.GetDataWidth; x += 4, xx++)
                {
                    var pixel = png.GetPixelData(x, y);

                    // parse
                    foreach (var filter in filterConfigs)
                    {
                        if (filter.UseRangeColor)
                        {
                            if (pixel.R >= filter.FromColor.R && pixel.R <= filter.ToColor.R &&
                                pixel.G >= filter.FromColor.G && pixel.G <= filter.ToColor.G &&
                                pixel.B >= filter.FromColor.B && pixel.B <= filter.ToColor.B)
                            {
                                InsertJSONItem(xx - hw, y - hh, filter.ItemName);
                                break;
                            }
                        }
                        else if (CompareRGBColors(pixel, filter.ToColor))
                        {
                            InsertJSONItem(xx - hw, y - hh, filter.ItemName);
                            break;
                        }
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

                sb.Remove(sb.Length - len - 1, 1); // последняя запятая
                sb.AppendLine("    ],");
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

        private void InsertJSONItem(int x, int y, string itemName)
        {
            items++;
            sb.AppendLine("      {");
            sb.AppendLine("        'position': {");
            sb.AppendLine($"          'x': {x},");
            sb.AppendLine($"          'y': {y}");
            sb.AppendLine("        },");
            sb.AppendLine($"        'name': '{itemName}'");
            sb.AppendLine("      },"); //!!
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

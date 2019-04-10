using System;
using System.IO;
using Hjg.Pngcs;

namespace Artorio.EFPng
{
    internal sealed class PngData
    {
        private byte[,] imgData;
        private int pixelWidth;
        private int pixelHeight;
        private int dataWidth;
        private int dataHeight; // == PixelHeight

        // image name
        public string LoadedImageName { get; private set; }

        public int GetPixelWidth => pixelWidth;

        public int GetPixelHeight => pixelHeight;

        public int GetDataWidth => dataWidth;
        public int GetDataHeight => pixelHeight;

        // only alpha
        public bool IsAlpha => true;

        /// <summary>
        /// Get ImageInfo.
        /// </summary>
        public ImageInfo Info { get; private set; }

        /// <summary>
        /// Create empty.
        /// </summary>
        public PngData(int pixelWidth, int pixelHeight, string imageName)
        {
            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;
            dataWidth = this.pixelWidth * (IsAlpha ? 4 : 3);
            dataHeight = this.pixelHeight;
            imgData = new byte[dataHeight, dataWidth];
            Info = new ImageInfo(this.pixelWidth, this.pixelHeight, 8, true);
            LoadedImageName = imageName ?? string.Empty;
        }

        /// <summary>
        /// Load from file.
        /// </summary>
        /// <param name="pngFilePath">Full file path.</param>
        public PngData(string pngFilePath)
        {
            if (!File.Exists(pngFilePath))
                throw new Exception($"Png file '{pngFilePath}' not exists.");

            LoadedImageName = Path.GetFileNameWithoutExtension(pngFilePath);
            PngReader source = FileHelper.CreatePngReader(pngFilePath);

            pixelWidth = source.ImgInfo.Cols;
            pixelHeight = source.ImgInfo.Rows;
            dataWidth = pixelWidth * 4; // auto convert ot argb
            dataHeight = pixelHeight;
            Info = new ImageInfo(source.ImgInfo.Cols, source.ImgInfo.Rows, 8, true);

            imgData = new byte[dataHeight, dataWidth];
            //ConsoleEx.WriteDebug($"Load image '{LoadedImageName}'");

            if (source.ImgInfo.Channels == 1)
                LoadGrayscaleImage(source);
            else if (source.ImgInfo.Channels == 3)
                LoadNoAlphaImage(source);
            else if (source.ImgInfo.Channels == 4)
                LoadRGBAImage(source);
            else
                throw new Exception($"Not support image format (Channels: {source.ImgInfo.Channels.ToString()}).");

            source.End();
        }

        private void LoadRGBAImage(PngReader source)
        {
            byte[] buffer = null;

            for (int dataY = 0; dataY < pixelHeight; dataY++)
            {
                buffer = source.ReadRowByte(buffer, dataY);

                for (int dataX = 0, bufX = 0; bufX < buffer.Length;)
                {
                    imgData[dataY, dataX++] = buffer[bufX++];
                    imgData[dataY, dataX++] = buffer[bufX++];
                    imgData[dataY, dataX++] = buffer[bufX++];
                    imgData[dataY, dataX++] = buffer[bufX++];
                }
            }
        }

        private void LoadGrayscaleImage(PngReader source)
        {
            byte[] buffer = null;

            for (int dataY = 0; dataY < pixelHeight; dataY++)
            {
                buffer = source.ReadRowByte(buffer, dataY);

                for (int dataX = 0, bufX = 0; bufX < buffer.Length;)
                {
                    imgData[dataY, dataX++] = buffer[bufX];
                    imgData[dataY, dataX++] = buffer[bufX];
                    imgData[dataY, dataX++] = buffer[bufX++];
                    imgData[dataY, dataX++] = byte.MaxValue;
                }
            }
        }

        private void LoadNoAlphaImage(PngReader source)
        {
            byte[] buffer = null;

            for (int dataY = 0; dataY < pixelHeight; dataY++)
            {
                buffer = source.ReadRowByte(buffer, dataY);

                for (int dataX = 0, bufX = 0; bufX < buffer.Length;)
                {
                    imgData[dataY, dataX++] = buffer[bufX++];
                    imgData[dataY, dataX++] = buffer[bufX++];
                    imgData[dataY, dataX++] = buffer[bufX++];
                    imgData[dataY, dataX++] = byte.MaxValue;
                }
            }
        }

        public Color GetPixelData(int x, int y)
        {
            return new Color(imgData[y, x], imgData[y, x + 1], imgData[y, x + 2], imgData[y, x + 3]);
        }

    }
}

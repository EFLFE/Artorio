using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Artorio
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private string result;

        public PreviewWindow()
        {
            InitializeComponent();
        }

        public void ShowWindow(FactorioBPMaker factorioBPMaker, string result, int insertItems)
        {
            this.result = result;
            var genData = factorioBPMaker.GeneratedDataPreview;
            var png = factorioBPMaker.LoadedPng;

            using (Bitmap bitmap = new Bitmap(png.GetPixelWidth, png.GetPixelHeight))
            {
                int i = -1;
                // draw
                for (int y = 0; y < png.GetPixelHeight; y++)
                {
                    for (int x = 0; x < png.GetPixelWidth; x++)
                    {
                        i++;
                        if (genData[i] == null)
                        {
                            bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                            continue;
                        }

                        var clr = genData[i].MapColor;
                        bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(clr.R, clr.G, clr.B));
                    }
                }

                // apply
                image.Source = BitmapToImageSource(bitmap);
            }

            info.Text = $"Size: {png.GetPixelWidth} x {png.GetPixelHeight} :: Items: {insertItems}";
            ShowDialog();
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(result, TextDataFormat.Text);
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

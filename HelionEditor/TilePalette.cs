using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HelionEditor
{
    class TilePalette
    {
        Canvas canvas;
        System.Windows.Controls.Image selectedImage;

        public List<BitmapImage> tiles = new List<BitmapImage>();

        public int selectedID = -1;

        System.Windows.Media.SolidColorBrush frameColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 200, 200));
        System.Windows.Media.SolidColorBrush selectedFrameColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 220, 20));

        public TilePalette(Canvas canvas, System.Windows.Controls.Image selectedImage)
        {
            this.canvas = canvas;
            this.selectedImage = selectedImage;
            canvas.MouseDown += Canvas_MouseDown;
        }

        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(canvas);
            int tileX = (int)(mousePosition.X - 3) / 36;
            int tileY = (int)(mousePosition.Y - 3) / 36;
            int tileID = tileY * 5 + tileX;
            if (selectedID != -1)
            {
                ((Canvas)canvas.Children[selectedID]).Background = frameColor;
            }
            ((Canvas)canvas.Children[tileID]).Background = selectedFrameColor;
            selectedID = tileID;
            selectedImage.Source = tiles[tileID];
        }

        public TilePalette Initialize()
        {
            var pathToTiles = AppDomain.CurrentDomain.BaseDirectory + "Tiles";
            if (Directory.Exists(pathToTiles))
            {
                int currentID = 0;
                string[] tilesets = Directory.GetFiles(pathToTiles);
                for (int i = 0; i < tilesets.Length; i++)
                {
                    Bitmap tileset = (Bitmap)Bitmap.FromFile(tilesets[i]);
                    for (int x = 0; x < tileset.Width / 32; x++)
                    {
                        for (int y = 0; y < tileset.Height / 32; y++)
                        {
                            bool valid = false;
                            Bitmap bmp = new Bitmap(32, 32);
                            for (int xx = 0; xx < 32; xx++)
                            {
                                for (int yy = 0; yy < 32; yy++)
                                {
                                    Color color = tileset.GetPixel(x * 32 + xx, y * 32 + yy);
                                    if (!valid && color.A > 0)
                                        valid = true;
                                    bmp.SetPixel(xx, yy, color);
                                }
                            }
                            if (valid)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                                    var bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.StreamSource = ms;
                                    bi.EndInit();
                                    tiles.Add(bi);

                                    Canvas frameImage = new Canvas();
                                    frameImage.Width = frameImage.Height = 34;
                                    frameImage.Background = frameColor;
                                    frameImage.Margin = new Thickness(3 + (currentID % 5) * 36, 3 + currentID / 5 * 36, 3, 3);
                                    canvas.Children.Add(frameImage);

                                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                                    image.Width = image.Height = 32;
                                    image.Source = bi;
                                    image.Margin = new Thickness(1);
                                    frameImage.Children.Add(image);
                                    currentID++;
                                }
                            }
                        }
                    }
                }
                canvas.Height = currentID / 5 * 36 + 4;
            }
            return this;
        }
    }
}

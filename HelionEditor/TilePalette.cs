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

        List<BitmapImage> tiles = new List<BitmapImage>();

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
            Console.WriteLine(tileID + ") " + tileX + ":" + tileY);
            //((System.Windows.Controls.Image)canvas.Children[tileID]).Visibility = Visibility.Hidden;
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
                            Bitmap bmp = new Bitmap(34, 34);
                            for (int xx = 0; xx < 34; xx++)
                            {
                                for (int yy = 0; yy < 34; yy++)
                                {
                                    if (xx == 0 || yy == 0 || xx == 33 || yy == 33)
                                    {
                                        bmp.SetPixel(xx, yy, System.Drawing.Color.WhiteSmoke);
                                    }
                                    else
                                    {
                                        System.Drawing.Color color = tileset.GetPixel(x * 32 + xx - 1, y * 32 + yy - 1);
                                        if (!valid && color.A > 0)
                                            valid = true;
                                        bmp.SetPixel(xx, yy, color);
                                    }
                                }
                            }
                            if (valid)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                                    image.Width = image.Height = 34;
                                    var bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.StreamSource = ms;
                                    bi.EndInit();
                                    tiles.Add(bi);
                                    image.Source = bi;
                                    image.Margin = new Thickness(3 + (currentID % 5) * 36, 3 + currentID / 5 * 36, 3, 3);
                                    canvas.Children.Add(image);
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

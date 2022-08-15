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
    class Editor
    {
        Canvas canvas;
        public GameLevel level;
        BitmapImage emptyCell;
        TilePalette palette;

        public Editor(Canvas canvas, TilePalette palette)
        {
            this.palette = palette;
            this.canvas = canvas;
            emptyCell = DrawEmptyCell();
        }

        BitmapImage DrawEmptyCell()
        {
            Bitmap bmp = new Bitmap(32, 32);
            for (int xx = 0; xx < 32; xx++)
            {
                for (int yy = 0; yy < 32; yy++)
                {
                    if (xx == 0 || yy == 0 || xx == 31 || yy == 31)
                    {
                        bmp.SetPixel(xx, yy, Color.WhiteSmoke);
                    }
                    else
                        bmp.SetPixel(xx, yy, (xx / 2 + yy / 2) % 2 == 0 ? Color.DarkGray : Color.LightGray);
                }
            }
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }

        public void Init(GameLevel level)
        {
            this.level = level;
            UpdateCanvas(level.width, level.height, level.levelLayers);
        }

        public void UpdateTile(int X, int Y, int layer)
        {
            ((System.Windows.Controls.Image)canvas.Children[Y * level.width + X]).Source = palette.tiles[level.levelLayers[layer].cells[X, Y]];
        }

        void UpdateCanvas(int width, int height, LevelLayer[] data)
        {
            canvas.Children.Clear();
            int currentID = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    {
                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Width = image.Height = 32;
                    int id = data[0].cells[x, y];
                    image.Source = id== -1 ? emptyCell : palette.tiles[id];
                    image.Margin = new Thickness((currentID % width) * 32, currentID / width * 32, 0, 0);
                    canvas.Children.Add(image);
                    currentID++;
                }
            }
            canvas.Width = width * 32;
            canvas.Height = height * 32;
        }
    }
}

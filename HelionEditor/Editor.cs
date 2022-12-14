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
    enum Tool
    {
        Brush,
        Erase,
        Bucket
    }

    class Editor
    {
        public GameLevel Level;
        Canvas canvas;
        BitmapImage emptyCell;
        TilePalette palette;
        int layer;
        Tool tool = Tool.Brush;
        Label layersCounter;
        public Editor(Canvas canvas, TilePalette palette, Slider layerSelector, Label layerCounter)
        {
            layersCounter = layerCounter;
            layersCounter.Content = "Layer : 1";
            this.palette = palette;
            this.canvas = canvas;
            emptyCell = DrawEmptyCell();
            layerSelector.ValueChanged += LayerSelector_ValueChanged;
        }

        private void LayerSelector_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            layer = (int)e.NewValue;
            layersCounter.Content = "Layer : "+(int)(layer+1);
        }

        BitmapImage DrawEmptyCell()
        {
            Bitmap bmp = new Bitmap(32, 32);
            for (int xx = 0; xx < 32; xx++)
            {
                for (int yy = 0; yy < 32; yy++)
                {
                    if (xx == 31 || yy == 31)
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
            this.Level = level;
            UpdateCanvas(level.Width, level.Height, level.LevelLayers);
        }

        public void SetSize(int width, int height)
        {
            if (Level == null || (Level.Width == width && Level.Height == height))
                return;
            Level.SetSize(width, height);
            Init(Level);
        }

        public void ChangeTool(Tool tool)
        {
            this.tool = tool;
        }

        public void UpdateTile(int X, int Y)
        {
            if (X >= 0 && Y >= 0 && X < Level.Width && Y < Level.Height && Level != null)
                switch (tool)
                {
                    case Tool.Brush:
                        UseBrush(X, Y);
                        break;
                    case Tool.Erase:
                        UseErase(X, Y);
                        break;
                    case Tool.Bucket:
                        UseBucket(X, Y);
                        break;
                }
        }

        void UseBrush(int X, int Y)
        {
            if (palette.SelectedID >= 0 && Level.SetTile(layer, X, Y, palette.SelectedID))
                ((System.Windows.Controls.Image)canvas.Children[layer * Level.Width * Level.Height + Y * Level.Width + X]).Source = palette.Tiles[Level.LevelLayers[layer].cells[X, Y]];
        }

        void UseErase(int X, int Y)
        {
            Level.SetTile(layer, X, Y, -1);
            if (layer == 0)
                ((System.Windows.Controls.Image)canvas.Children[layer * Level.Width * Level.Height + Y * Level.Width + X]).Source = emptyCell;
            else
                ((System.Windows.Controls.Image)canvas.Children[layer * Level.Width * Level.Height + Y * Level.Width + X]).Source = null;
        }

        void UseBucket(int X, int Y)
        {
            int touchedID = Level.LevelLayers[layer].cells[X, Y];
            Queue<System.Drawing.Point> checkList = new Queue<System.Drawing.Point>();
            bool[,] checkedList = new bool[Level.Width, Level.Height];
            UseBrush(X, Y);

            foreach (var neighbor in GetNeighbors(X, Y))
            {
                checkList.Enqueue(neighbor);
            }
            while (checkList.Count > 0)
            {
                var neighbor = checkList.Dequeue();
                checkedList[neighbor.X, neighbor.Y] = true;
                if (Level.LevelLayers[layer].cells[neighbor.X,neighbor.Y] == touchedID)
                {
                    UseBrush(neighbor.X, neighbor.Y);
                    foreach (var nbr in GetNeighbors(neighbor.X, neighbor.Y))
                    {
                        if (!checkedList[nbr.X, nbr.Y])
                        {
                            checkList.Enqueue(nbr);
                        }
                    }
                }
            }
        }

        public void ClearLayer()
        {
            for (int x = 0; x < Level.Width; x++)
            {
                for (int y = 0; y < Level.Height; y++)
                {
                    UseErase(x, y);
                }
            }
        }

        List<System.Drawing.Point> GetNeighbors(int X, int Y)
        {
            var neighbors = new List<System.Drawing.Point>();
            if (X > 0)
                neighbors.Add(new System.Drawing.Point(X - 1, Y));
            if (Y > 0)
                neighbors.Add(new System.Drawing.Point(X, Y - 1));
            if (X < Level.Width-1)
                neighbors.Add(new System.Drawing.Point(X + 1, Y));
            if (Y < Level.Height-1)
                neighbors.Add(new System.Drawing.Point(X, Y + 1));
            return neighbors;
        } 

        void UpdateCanvas(int width, int height, LevelLayer[] data)
        {
            canvas.Children.Clear();
            
            for (int l = 0; l < 5; l++)
            {
                int currentID = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                        image.Width = image.Height = 32;
                        int id = data[l].cells[x, y];
                        if (l == 0)
                        {
                            image.Source = id == -1 ? emptyCell : palette.Tiles[id];
                        } 
                        else if (id != -1)
                            image.Source = palette.Tiles[id];
                        image.Margin = new Thickness(currentID % width * 32, currentID / width * 32, 0, 0);
                        canvas.Children.Add(image);
                        currentID++;
                    }
                }
            }

            canvas.Width = width * 32;
            canvas.Height = height * 32;
        }

        public int GetTile(int X, int Y)
        {
            if (X >= 0 && Y >= 0 && X < Level.Width && Y < Level.Height && Level != null)
                return Level.LevelLayers[layer].cells[X, Y];
            else
                return -1;
        }
    }
}

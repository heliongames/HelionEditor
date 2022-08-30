using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HelionEditor
{
    [Serializable]
    class GameLevel
    {
        public LevelLayer[] LevelLayers;
        public int Width;
        public int Height;

        public GameLevel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            LevelLayers = new LevelLayer[5];
            for (int i = 0; i < LevelLayers.Length; i++)
            {
                LevelLayers[i] = new LevelLayer(width, height);
            }
        }

        public void SetSize(int width, int height)
        {
            LevelLayer[] newLevelLayers = new LevelLayer[5];
            for (int i = 0; i < 5; i++)
            {
                newLevelLayers[i] = new LevelLayer(width, height);
                for (int x = 0; x < Math.Min(width,this.Width); x++)
                {
                    for (int y = 0; y < Math.Min(height, this.Height); y++)
                    {
                        newLevelLayers[i].cells[x, y] = LevelLayers[i].cells[x, y];
                    }
                }
            }
            LevelLayers = newLevelLayers;
            this.Width = width;
            this.Height = height;
        }

        public bool SetTile(int layer, int X, int Y, int ID)
        {
            if (LevelLayers[layer].cells[X, Y] != ID)
            {
                LevelLayers[layer].cells[X, Y] = ID;
                return true;
            }
            return false;
        }

        public byte[] ToByteArray()
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public static GameLevel FromByteArray(byte[] source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(source))
            {
                return (GameLevel)formatter.Deserialize(stream);
            }
        }
    }
    [Serializable]
    class LevelLayer
    {
        public int[,] cells;

        public LevelLayer(int width, int height)
        {
            cells = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = -1;
                }
            }
        }
    }
}

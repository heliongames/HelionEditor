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
        public LevelLayer[] levelLayers;
        public int width;
        public int height;

        public GameLevel(int width, int height)
        {
            this.width = width;
            this.height = height;
            levelLayers = new LevelLayer[6];
            for (int i = 0; i < levelLayers.Length; i++)
            {
                levelLayers[i] = new LevelLayer(width, height);
            }
        }

        public bool SetTile(int layer, int X, int Y, int ID)
        {
            if (levelLayers[layer].cells[X, Y] != ID)
            {
                levelLayers[layer].cells[X, Y] = ID;
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

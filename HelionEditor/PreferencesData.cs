using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelionEditor
{
    [Serializable]
    class PreferencesData
    {
        public double Width;
        public double Height;
        public double Top;
        public double Left;
        public string PathToTiles;
        public List<string> RecentFiles;

        public PreferencesData(double width, double height, double top, double left, List<string> recentFiles, string pathToTiles)
        {
            this.Width = width;
            this.Height = height;
            this.Top = top;
            this.Left = left;
            this.RecentFiles = recentFiles;
            this.PathToTiles = pathToTiles;
        }
    }
}

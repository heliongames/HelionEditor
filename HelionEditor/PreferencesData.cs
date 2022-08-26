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
        public double width;
        public double height;
        public double top;
        public double left;
        public string pathToTiles;
        public List<string> recentFiles;

        public PreferencesData(double width, double height, double top, double left, List<string> recentFiles, string pathToTiles)
        {
            this.width = width;
            this.height = height;
            this.top = top;
            this.left = left;
            this.recentFiles = recentFiles;
            this.pathToTiles = pathToTiles;
        }
    }
}

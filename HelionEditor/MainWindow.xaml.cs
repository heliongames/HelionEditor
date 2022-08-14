using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelionEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TilePalette palette;
        string fileName;
        string filePath;
        public MainWindow()
        {
            InitializeComponent();
            palette = new TilePalette(CanvasPalette, ImageSelectedTile).Initialize();
        }

        private void NewItem(object sender, RoutedEventArgs e)
        {

        }

        private void OpenItem(object sender, RoutedEventArgs e)
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "csl files (*.csl)|*.csl|All files (*.*)|*.*";
            if (filedialog.ShowDialog() == true)
            {
                filePath = filedialog.FileName;
                fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
        }

        private void SaveItem(object sender, RoutedEventArgs e)
        {
            string jsonText = "";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csl files (*.csl)|*.csl";
            fileDialog.DefaultExt = ".csl";
            fileDialog.FileName = fileName;
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllText(fileDialog.FileName, jsonText);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            string jsonText = "";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csl files (*.csl)|*.csl";
            fileDialog.DefaultExt = ".csl";
            fileDialog.FileName = fileName+"_copy";
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllText(fileDialog.FileName, jsonText);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

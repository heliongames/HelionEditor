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
        static public string fileName;
        static public string filePath;

        public static bool saveStatus;

        public MainWindow()
        {
            InitializeComponent();
            palette = new TilePalette(CanvasPalette, ImageSelectedTile).Initialize();
            DataContext = new MyDataContext();
        }

        private void NewItem(object sender, RoutedEventArgs e)
        {

        }

        private void OpenItem(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }


        private void SaveItem(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }
        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileAs();
        }
        private void OpenFile()
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "csl files (*.csl)|*.csl|All files (*.*)|*.*";
            if (filedialog.ShowDialog() == true)
            {
                filePath = filedialog.FileName;
                fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
        }

        private void SaveFile()
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

        private void SaveFileAs()
        {
            string jsonText = "";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csl files (*.csl)|*.csl";
            fileDialog.DefaultExt = ".csl";
            fileDialog.FileName = fileName + "_copy";
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllText(fileDialog.FileName, jsonText);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }



    public class Close : ICommand
    {
        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class New : ICommand
    {
        public void Execute(object parameter)
        {
            //new
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class Open : ICommand
    {
        public void Execute(object parameter)
        {
            OpenFile();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        private void OpenFile()
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "csl files (*.csl)|*.csl|All files (*.*)|*.*";
            if (filedialog.ShowDialog() == true)
            {
                MainWindow.filePath = filedialog.FileName;
                MainWindow.fileName = System.IO.Path.GetFileNameWithoutExtension(MainWindow.filePath);
            }
        }

        public event EventHandler CanExecuteChanged;
    }

    public class SaveAs : ICommand
    {
        public void Execute(object parameter)
        {
            SaveFileAs();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private void SaveFileAs()
        {
            string jsonText = "";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csl files (*.csl)|*.csl";
            fileDialog.DefaultExt = ".csl";
            fileDialog.FileName = MainWindow.fileName + "_copy";
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllText(fileDialog.FileName, jsonText);
                MainWindow.fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

        public event EventHandler CanExecuteChanged;
    }

    public class Save : ICommand
    {
        public void Execute(object parameter)
        {
            SaveFile();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private void SaveFile()
        {
            string jsonText = "";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csl files (*.csl)|*.csl";
            fileDialog.DefaultExt = ".csl";
            fileDialog.FileName = MainWindow.fileName;
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllText(fileDialog.FileName, jsonText);
                MainWindow.fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

        public event EventHandler CanExecuteChanged;
    }


    public class MyDataContext
    {
        ICommand _closeCommand = new Close();
        ICommand _saveCommand = new Save();
        ICommand _saveAsCommand = new SaveAs();
        ICommand _newCommand = new New();
        ICommand _openCommand = new Open();

        public ICommand Close
        {
            get { return _closeCommand; }
        }

        public ICommand Save
        {
            get { return _saveCommand; }
        }

        public ICommand SaveAs
        {
            get { return _saveAsCommand; }
        }

        public ICommand New
        {
            get { return _newCommand; }
        }
        public ICommand Open
        {
            get { return _openCommand; }
        }
    }
}

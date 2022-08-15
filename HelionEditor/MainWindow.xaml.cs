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
    public partial class MainWindow : Window
    {
        TilePalette palette;
        static public string fileName;
        static public string filePath;

        public static bool saveStatus;
        static Editor editor;

        public MainWindow()
        {
            InitializeComponent();
            palette = new TilePalette(CanvasPalette, ImageSelectedTile).Initialize();
            DataContext = new MyDataContext();
            editor = new Editor(CanvasLevel, palette);
        }

        private void NewItem(object sender, RoutedEventArgs e)
        {
            NewFile();
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

        public static void NewFile()
        {
            editor.Init(new GameLevel(16, 16));
        }

        public static void OpenFile()
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "csl files (*.csl)|*.csl|All files (*.*)|*.*";
            if (filedialog.ShowDialog() == true)
            {
                filePath = filedialog.FileName;
                byte[] data = File.ReadAllBytes(filePath);
                editor.Init(GameLevel.FromByteArray(data));
            }
        }

        public static void SaveFile()
        {
            byte[] data = editor.level.ToByteArray();
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csl files (*.csl)|*.csl";
            fileDialog.DefaultExt = ".csl";
            fileDialog.FileName = fileName;
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(fileDialog.FileName, data);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

        public static void SaveFileAs()
        {
            byte[] data = editor.level.ToByteArray();
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csl files (*.csl)|*.csl";
            fileDialog.DefaultExt = ".csl";
            fileDialog.FileName = fileName + "_copy";
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(fileDialog.FileName, data);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CanvasLevel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePosition = e.GetPosition(CanvasLevel);
                int tileX = (int)mousePosition.X / 32;
                int tileY = (int)mousePosition.Y / 32;
                if (editor.level.SetTile(0, tileX, tileY, palette.selectedID))
                    editor.UpdateTile(tileX, tileY, 0);
            }
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
            MainWindow.NewFile();
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
            MainWindow.OpenFile();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class SaveAs : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.SaveFileAs();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class Save : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.SaveFile();
        }

        public bool CanExecute(object parameter)
        {
            return true;
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

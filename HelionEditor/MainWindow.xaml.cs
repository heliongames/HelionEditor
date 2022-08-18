﻿using Microsoft.Win32;
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

        static public MainWindow Instance;
        bool initialized;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ClearSelectedTool();
            palette = new TilePalette(CanvasPalette, ImageSelectedTile).Initialize();
            DataContext = new MyDataContext();
            editor = new Editor(CanvasLevel, palette, SliderLayerSelector, LayersCounter);
            LabelTileInfo.Content = $"[{0},{0}]";
        }

        private void InitializeWindow()
        {
            string pathToPreferencesData = System.AppDomain.CurrentDomain.BaseDirectory + "pref.json";
            if (File.Exists(pathToPreferencesData))
            {
                var preferencesData = Newtonsoft.Json.JsonConvert.DeserializeObject<PreferencesData>(File.ReadAllText(pathToPreferencesData));
                this.Top = preferencesData.top;
                this.Left = preferencesData.left;
                this.Width = preferencesData.width;
                this.Height = preferencesData.height;
            }
            initialized = true;
        }

        private void SavePreferences()
        {
            string pathToPreferencesData = System.AppDomain.CurrentDomain.BaseDirectory + "pref.json";
            var preferencesData = new PreferencesData(Width, Height, Top, Left);
            File.WriteAllText(pathToPreferencesData, Newtonsoft.Json.JsonConvert.SerializeObject(preferencesData));
        }

        private void NewItem(object sender, RoutedEventArgs e)
        {
            NewFile();
            fileName = "unnamed";
            this.Title = $"HGL Editor [{fileName}.csl]*";
        }

        private void OpenItem(object sender, RoutedEventArgs e)
        {
            OpenFile();
            this.Title = $"HGL Editor [{fileName}.csl]";
        }

        private void SaveItem(object sender, RoutedEventArgs e)
        {
            SaveFile();
            if (fileName != "" && fileName != null)
                this.Title = $"HGL Editor [{fileName}.csl]";
            else
                this.Title = $"HGL Editor";
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileAs();
            if (fileName != "" && fileName != null)
                this.Title = $"HGL Editor [{fileName}.csl]";
            else
                this.Title = $"HGL Editor";
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
                fileName = System.IO.Path.GetFileNameWithoutExtension(filedialog.FileName);
                byte[] data = File.ReadAllBytes(filePath);
                editor.Init(GameLevel.FromByteArray(data));
            }
        }

        public static void SaveFile()
        {   
            if(filePath != null)
            {
                byte[] data = editor.level.ToByteArray();
                File.WriteAllBytes(filePath, data);
                return;
            }
            SaveFileAs();
        }

        public static void SaveFileAs()
        {
            if (fileName != "" && fileName != null) { 
                byte[] data = editor.level.ToByteArray();
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "csl files (*.csl)|*.csl";
                fileDialog.DefaultExt = ".csl";
                fileDialog.FileName = fileName;
                if (fileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(fileDialog.FileName, data);
                    filePath = fileDialog.FileName;
                    fileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
                }
            }
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CanvasLevel_MouseMove(object sender, MouseEventArgs e)
        {
            int tileX = (int)e.GetPosition(CanvasLevel).X / 32;
            int tileY = (int)e.GetPosition(CanvasLevel).Y / 32;

            if(editor.GetTile(tileX, tileY) == -1)
                LabelTileInfo.Content = $"[{tileX},{tileY}]";
            else
                LabelTileInfo.Content = $"[{tileX},{tileY}] | ID:{editor.GetTile(tileX, tileY)}";
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Draw(e.GetPosition(CanvasLevel));
            }
        }

        private void CanvasLevel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Draw(e.GetPosition(CanvasLevel));
        }

        void Draw(System.Windows.Point mousePosition)
        {
            int tileX = (int)mousePosition.X / 32;
            int tileY = (int)mousePosition.Y / 32;
            editor.UpdateTile(tileX, tileY);
            this.Title = $"HGL Editor [{fileName}.csl]*";
        }

        static SolidColorBrush button = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        SolidColorBrush selectedButtonColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 250, 150));

        public void ButtonToolBrush_Click(object sender, RoutedEventArgs e)
        {
            ClearSelectedTool();
            ButtonToolBrush.Background = selectedButtonColor;
            editor.ChangeTool(Tool.Brush);
        }

        public void ButtonToolErase_Click(object sender, RoutedEventArgs e)
        {
            ClearSelectedTool();
            ButtonToolErase.Background = selectedButtonColor;
            editor.ChangeTool(Tool.Erase);
        }

        public void ButtonToolBucket_Click(object sender, RoutedEventArgs e)
        {
            ClearSelectedTool();
            ButtonToolBucket.Background = selectedButtonColor;
            editor.ChangeTool(Tool.Bucket);
        }

        public void ClearSelectedTool()
        {
            ButtonToolBrush.Background = button;
            ButtonToolBucket.Background = button;
            ButtonToolErase.Background = button;
        }

        public static void ClearLayer()
        {
            editor.ClearLayer();
        }

        private void ClearLayer(object sender, RoutedEventArgs e)
        {
            editor.ClearLayer();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            InitializeWindow();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SavePreferences();
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

    public class ClearLayer : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.ClearLayer();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class Preferences : ICommand
    {
        public void Execute(object parameter)
        {
            //Pref
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class BrushTool : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.Instance.ButtonToolBrush_Click(this,  new RoutedEventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class EraseTool : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.Instance.ButtonToolErase_Click(this, new RoutedEventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class BucketTool : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.Instance.ButtonToolBucket_Click(this, new RoutedEventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class Help : ICommand
    {
        public void Execute(object parameter)
        {
            //help
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
        ICommand _clearLayerCommand = new ClearLayer();
        ICommand _prefCommand = new Preferences();
        ICommand _brushToolCommand = new BrushTool();
        ICommand _eraseToolCommand = new EraseTool();
        ICommand _bucketToolCommand = new BucketTool();
        ICommand _helpCommand = new Help();


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

        public ICommand ClearLayer
        {
            get { return _clearLayerCommand; }
        }

        public ICommand Preferences
        {
            get { return _prefCommand; }
        }

        public ICommand BrushTool
        {
            get { return _brushToolCommand; }
        }
        public ICommand EraseTool
        {
            get { return _eraseToolCommand; }
        }
        public ICommand BucketTool
        {
            get { return _bucketToolCommand; }
        }
        public ICommand Help
        {
            get { return _helpCommand; }
        }
    }
}

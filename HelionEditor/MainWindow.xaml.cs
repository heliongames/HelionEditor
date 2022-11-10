using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace HelionEditor
{
    public partial class MainWindow : Window
    {
        TilePalette palette;
        static public string FileName;
        static public string FilePath;

        public static bool SaveStatus;
        static Editor editor;
        static List<string> recentFiles;

        static public MainWindow Instance;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ClearSelectedTool();
            DataContext = new MyDataContext();
            LabelTileInfo.Content = $"[{0},{0}]";
        }

        private void InitializeWindow()
        {
            double top = 0;
            double left = 0;
            double width = 0;
            double height = 0;

            string[] args = Environment.GetCommandLineArgs();

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "-t")
                    top = double.Parse(args[i + 1]);
                if (args[i] == "-l")
                    left = double.Parse(args[i + 1]);
                if (args[i] == "-w")
                    width = double.Parse(args[i + 1]);
                if (args[i] == "-h")
                    height = double.Parse(args[i + 1]);
            }

            Console.WriteLine($"Top: [{top}]|Left: [{left}]|Width: [{width}]|Height: [{height}]");

            GridPreferences.Visibility = Visibility.Collapsed;
            string pathToPreferencesData = AppDomain.CurrentDomain.BaseDirectory + "pref.json";

            var preferencesData = new PreferencesData(0, 0, 0, 0, null, null);
            
            if (File.Exists(pathToPreferencesData))
            {
                preferencesData = Newtonsoft.Json.JsonConvert.DeserializeObject<PreferencesData>(File.ReadAllText(pathToPreferencesData));
            }
            recentFiles = preferencesData.RecentFiles != null ? preferencesData.RecentFiles : new List<string>();
            if (preferencesData.PathToTiles == null)
                preferencesData.PathToTiles = AppDomain.CurrentDomain.BaseDirectory + "Tiles";
            palette = new TilePalette(CanvasPalette, ImageSelectedTile, preferencesData.PathToTiles).Initialize();
            editor = new Editor(CanvasLevel, palette, SliderLayerSelector, LayersCounter);
            LabelPathToTiles.Content = preferencesData.PathToTiles;
            InitializeRecentFiles();
            Top = top != 0 ? top : preferencesData.Top;
            Left = left != 0 ? left : preferencesData.Left;
            Width = width != 0 ? width : preferencesData.Width;
            Height = height != 0 ? height : preferencesData.Height;
        }

        private void InitializeRecentFiles()
        {
            while (recentFiles.Count > 10)
                recentFiles.RemoveAt(0);
            MenuItemRecentFiles.Items.Clear();
            for (int i = recentFiles.Count - 1; i >= 0; i--)
            {
                var file = recentFiles[i];
                MenuItem item = new MenuItem();
                item.Header = file;
                item.Click += (object sender, RoutedEventArgs e) =>
                {
                    FilePath = ((MenuItem)sender).Header.ToString();
                    FileName = System.IO.Path.GetFileNameWithoutExtension(FilePath);
                    if(File.Exists(FilePath))
                    {
                        byte[] data = File.ReadAllBytes(FilePath);
                        editor.Init(GameLevel.FromByteArray(data));
                        recentFiles.Add(FilePath);
                        Instance.InitializeRecentFiles();
                        Instance.SavePreferences();
                    } 
                    else
                    {
                        foreach (var fileItem in recentFiles)
                        {

                            if(fileItem == FilePath)
                            {
                                recentFiles.Remove(fileItem);
                                MenuItemRecentFiles.Items.Remove(sender);
                                break;
                            }
                        }
                        MessageBox.Show("File not found", "ERROR");
                    }
                };
                MenuItemRecentFiles.Items.Add(item);
            }
        }

        private void SavePreferences()
        {
            string pathToPreferencesData = AppDomain.CurrentDomain.BaseDirectory + "pref.json";
            var preferencesData = new PreferencesData(Width, Height, Top, Left, recentFiles, palette.PathToTiles);
            File.WriteAllText(pathToPreferencesData, Newtonsoft.Json.JsonConvert.SerializeObject(preferencesData));
        }

        private void NewItem(object sender, RoutedEventArgs e)
        {
            NewFile();
            FileName = "unnamed";
            SaveStatus = false;
            this.Title = $"HGL Editor [{FileName}.csl]*";
        }

        private void OpenItem(object sender, RoutedEventArgs e)
        {
            OpenFile();
            SaveStatus = false;
            this.Title = $"HGL Editor [{FileName}.csl]";
        }

        public void SaveItem(object sender, RoutedEventArgs e)
        {
            SaveFile();
            SaveStatus = true;
            if (FileName != "" && FileName != null)
                this.Title = $"HGL Editor [{FileName}.csl]";
        }

        public void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileAs();
            SaveStatus = true;
            if (FileName != "" && FileName != null)
                this.Title = $"HGL Editor [{FileName}.csl]";
        }

        public static void NewFile()
        {
            editor.Init(new GameLevel(int.Parse(Instance.TextboxWidth.Text), int.Parse(Instance.TextboxHeight.Text)));
        }

        public static void OpenFile()
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "csl files (*.csl)|*.csl|All files (*.*)|*.*";
            if (filedialog.ShowDialog() == true)
            {
                FilePath = filedialog.FileName;
                FileName = System.IO.Path.GetFileNameWithoutExtension(filedialog.FileName);
                byte[] data = File.ReadAllBytes(FilePath);
                editor.Init(GameLevel.FromByteArray(data));
                recentFiles.Add(FilePath);
                Instance.InitializeRecentFiles();
                Instance.SavePreferences();
            }
        }

        public static void SaveFile()
        {
            if(FilePath != null)
            {
                byte[] data = editor.Level.ToByteArray();
                File.WriteAllBytes(FilePath, data);
                return;
            }
            SaveFileAs();
        }

        public static void SaveFileAs()
        {
            if (FileName != "" && FileName != null) { 
                byte[] data = editor.Level.ToByteArray();
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "csl files (*.csl)|*.csl";
                fileDialog.DefaultExt = ".csl";
                fileDialog.FileName = FileName;
                if (fileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(fileDialog.FileName, data);
                    FilePath = fileDialog.FileName;
                    FileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
                    recentFiles.Add(FilePath);
                    Instance.InitializeRecentFiles();
                    Instance.SavePreferences();
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
            SaveStatus = false;
        }

        void Draw(System.Windows.Point mousePosition)
        {
            int tileX = (int)mousePosition.X / 32;
            int tileY = (int)mousePosition.Y / 32;
            editor.UpdateTile(tileX, tileY);
            if(!SaveStatus)
                this.Title = $"HGL Editor [{FileName}.csl]*";
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

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            if (e.Text == "\r")
            {
                int w = Clamp(int.Parse(TextboxWidth.Text), 1, 128);
                int h = Clamp(int.Parse(TextboxHeight.Text), 1, 128);
                TextboxWidth.Text = w.ToString();
                TextboxHeight.Text = h.ToString();
                e.Handled = false;
                editor.SetSize(int.Parse(TextboxWidth.Text), int.Parse(TextboxHeight.Text));
                CanvasBackground.Focus();
            }
            else
                e.Handled = regex.IsMatch(e.Text);
        }

        int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        private void ChoosePathToTiles(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                palette.PathToTiles = folder;
                palette.Initialize();
                SavePreferences();
                LabelPathToTiles.Content = folder;
                Console.WriteLine(folder);
            }
        }

        private void ClosePreferences(object sender, MouseButtonEventArgs e)
        {
            GridPreferences.Visibility = Visibility.Collapsed;
        }

        public void OpenPreferences(object sender, RoutedEventArgs e)
        {
            GridPreferences.Visibility = Visibility.Visible;
        }

        public void Help(object sender, RoutedEventArgs e)
        {
            var destinationurl = "http://helion.games/hgl/";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
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
            MainWindow.Instance.SaveAs(null, null);
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
            MainWindow.Instance.SaveItem(null, null);
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
            MainWindow.Instance.OpenPreferences(null, null);
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
            MainWindow.Instance.Help(null, null);
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

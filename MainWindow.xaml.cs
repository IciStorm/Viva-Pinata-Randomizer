using Randomizer.Core;
using Randomizer.PKG;
using ReqBlock;
using System.ComponentModel;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using WinForms = System.Windows.Forms;

namespace Randomizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainViewModel ViewModel { get; set; }
        private string filepath = "";

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
        }

        private void btnFilePath_Click(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();
            dialog.Description = "Please select your 'Viva Pinata' folder.";
            dialog.UseDescriptionForTitle = true;
            WinForms.DialogResult result = dialog.ShowDialog();

            if (result == WinForms.DialogResult.OK)
            {
                if (dialog.SelectedPath.EndsWith(@"\Viva Pinata") || dialog.SelectedPath.EndsWith(@"\Viva Pinata\bundles_packages"))
                {
                    btnRandomize.IsEnabled = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Invalid folder. Make sure to select the folder that has 'Viva Pinata.exe'");
                    btnRandomize.IsEnabled = false;
                    return;
                }
            }
        }



        private void LoadPkg_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "PKG files (*.pkg)|*.pkg" };
            if (dialog.ShowDialog() != true) return;

            string filepath = dialog.FileName;
            var pkg = PkgHandler.ReadPKG(filepath);

            string exportDir = System.IO.Path.GetDirectoryName(filepath) ?? AppDomain.CurrentDomain.BaseDirectory;
            string exportPath = System.IO.Path.Combine(exportDir, "Patched_" + System.IO.Path.GetFileName(filepath));
            string logPath = System.IO.Path.Combine(exportDir, "RandomizationLog.txt");

            RandomizerLogic.RunFullRandomization(pkg, exportPath, logPath);

            System.Windows.MessageBox.Show($"Full randomization complete.\nLog: {logPath}");
        }
    }
}

public class MainViewModel : INotifyPropertyChanged
{
    private string _isFilePathGood;
    public string isFilePathGood
    {
        get => _isFilePathGood;
        set
        {
            _isFilePathGood = value;
            OnPropertyChanged(nameof(isFilePathGood));
        }
    }

    public MainViewModel()
    {
        _isFilePathGood = "NO"; 
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
using System.IO;
using System.Windows;
using Microsoft.Win32;
using VoxReader.Interfaces;
using VoxToStructure.format.schematic;

namespace VoxToStructure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var formatItem = ComboBox.SelectedItem;
            if (formatItem == null)
            {
                MessageBox.Show("Select an output format first!", "Output format", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var loader = new OpenFileDialog();
            loader.Filter = "MagicaVoxel File (*.vox)|*.vox";
            if (loader.ShowDialog() == true)
                loadAndConvert(loader.FileName, ComboBox.Text);
        }

        private void loadAndConvert(string file, string targetFormat)
        {
            if (Path.GetExtension(file).Equals(".vox"))
            {
                IVoxFile vox = VoxReader.VoxReader.Read(file);
                var outputPath = Path.ChangeExtension(file, targetFormat);
                switch (targetFormat)
                {
                    case ".schematic":
                    {
                        var output = new SchematicFile();
                        var schematicFile = output.FromVox(vox);
                        output.Save(schematicFile, outputPath);
                        MessageBox.Show("Conversion successful, saved to " + outputPath);
                        break;
                    }
                    default:
                    {
                        MessageBox.Show("Format currently not supported", "Unsupported format", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Unrecognized File");
            }
        }
    }
}
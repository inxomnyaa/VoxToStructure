using System;
using System.IO;
using System.Linq;
using System.Text;
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
            loader.Multiselect = true;
            loader.Filter = "MagicaVoxel File (*.vox)|*.vox";
            if (loader.ShowDialog() == true) loadAndConvert(loader.FileNames, ComboBox.Text);
        }

        private void loadAndConvert(string[] files, string targetFormat)
        {
            var successful = new StringBuilder(files.Length);
            foreach (var file in files)
            {
                try
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
                                successful.Append("\n" + outputPath);
                                break;
                            }
                            default:
                            {
                                throw new FileFormatException("Format " + targetFormat + " currently not supported");
                            }
                        }
                    }
                    else
                    {
                        throw new FileFormatException("Unrecognized File " + file);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    MessageBox.Show("Conversion of " + file + " failed.\n" + e, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            if (successful.Length > 0)
                MessageBox.Show("Conversion of " + successful.ToString().Count(c => c == '\n') + "/" + files.Length + " files successful, saved to:" + successful);
        }
    }
}
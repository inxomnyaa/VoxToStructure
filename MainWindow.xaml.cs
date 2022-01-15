using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
                                successful.Append(Environment.NewLine + outputPath);
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
                    Debug.WriteLine("Conversion of " + file + " failed.");
                    Debug.WriteLine(e);
                    MessageBox.Show("Conversion of " + file + " failed." + Environment.NewLine + e, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            if (successful.Length > 0)
            {
                var filesSuccessful = "Conversion of " + Regex.Matches(successful.ToString(), Environment.NewLine).Count + "/" + files.Length + " files successful";
                var savedFilesTo = "Saved files to:" + successful;
                Debug.WriteLine(filesSuccessful);
                Debug.WriteLine(savedFilesTo);
                MessageBox.Show(savedFilesTo, filesSuccessful, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
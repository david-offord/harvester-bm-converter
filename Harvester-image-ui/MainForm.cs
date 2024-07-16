using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;

namespace Harvester_image_ui
{
    public partial class MainForm : Form
    {
        //if theyre converting 1 by 1, open the right folder each time
        string lastOpenedBmFolder;
        string lastOpenedAbmFolder;
        string lastOpenedPaletteFolder;

        bool saveAsGif = true;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ConvertSingleFileButton_Click(object sender, EventArgs e)
        {
            string bmFilePath = "";
            string palettePath = "";
            string savePath = "";
            string fileName = "";

            //open the image file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (string.IsNullOrWhiteSpace(lastOpenedBmFolder) == false)
                    openFileDialog.InitialDirectory = lastOpenedBmFolder;

                openFileDialog.Filter = "Harvester image file (*.BM)|*.BM";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Open .BM File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    bmFilePath = openFileDialog.FileName;
                    fileName = openFileDialog.SafeFileName;
                    lastOpenedBmFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                }
                else
                {
                    return;
                }
            }

            //open the palette file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (string.IsNullOrWhiteSpace(lastOpenedPaletteFolder) == false)
                    openFileDialog.InitialDirectory = lastOpenedPaletteFolder;

                openFileDialog.Filter = "Harvester palette file (*.PAL)|*.PAL";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Open .PAL File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    palettePath = openFileDialog.FileName;
                    lastOpenedPaletteFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                }
                else
                {
                    return;
                }
            }


            //open the palette file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (string.IsNullOrWhiteSpace(lastOpenedBmFolder) == false)
                    saveFileDialog.InitialDirectory = lastOpenedBmFolder;

                saveFileDialog.Filter = "Bitmap Image (*.BMP)|*.BMP";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Save .BMP File";
                saveFileDialog.DefaultExt = ".BMP";
                saveFileDialog.FileName = fileName.Split('.')[0] + ".BMP";//cause the file name will always be .BM, split and add the .bmp


                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    savePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                HarvesterImageConverter.ConvertImage(bmFilePath, palettePath, savePath);
                //MessageBox.Show($"Success!", "Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred. Exception details: {ex.Message}", "Error!");
            }

        }

        private void ConvertFolderButton_Click(object sender, EventArgs e)
        {
            string bmFolderPath = "";
            string paletteFolderPath = "";
            string saveFolder = "";
            List<string> unmatchedPals = new List<string>();

            //open the image file
            using (VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog())
            {
                openFolderDialog.Description = "Open Folder With .BM Files";

                if (openFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    bmFolderPath = openFolderDialog.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            //open the palette file
            using (VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog())
            {
                openFolderDialog.Description = "Open Folder With .PAL Files";

                if (openFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    paletteFolderPath = openFolderDialog.SelectedPath;
                }
                else
                {
                    return;
                }
            }


            using (VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog())
            {
                openFolderDialog.Description = "Save Images to folder (will overwrite any existing files)";

                if (openFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    saveFolder = openFolderDialog.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            try
            {
                FileInfo[] bmFiles = new DirectoryInfo(bmFolderPath).GetFiles().Where(x => x.Name.ToUpper().EndsWith(".BM")).ToArray(); ;
                FileInfo[] palFiles = new DirectoryInfo(paletteFolderPath).GetFiles().Where(x => x.Name.ToUpper().EndsWith(".PAL")).ToArray();

                if (palFiles.Length == 0)
                    throw new Exception("No PAL files found");
                if (bmFiles.Length == 0)
                    throw new Exception("No BM files found");

                //go through each bm file. Try to find a .PAL file to match it
                foreach (var bmFile in bmFiles)
                {
                    string bmFileNameNoExtension = bmFile.Name.Split('.')[0];
                    FileInfo palFileName = palFiles.FirstOrDefault(x => x.Name.StartsWith(bmFileNameNoExtension));
                    //if there are no PAL files that match, match with default
                    if (palFileName == null)
                    {
                        palFileName = palFiles[0];
                        unmatchedPals.Add(bmFile.Name);
                    }

                    HarvesterImageConverter.ConvertImage(bmFile.FullName, palFileName.FullName, Path.Combine(saveFolder, bmFileNameNoExtension + ".BMP"));
                }

                if (unmatchedPals.Count > 0)
                {
                    string warningMessage = $"I was unable to auto-match the following .BM files to .PAL files, and a default palette was used:";

                    unmatchedPals.ForEach(x => warningMessage += x + "\n");
                    warningMessage += "\nThese images may look distorted. Please convert these individually.";

                    MessageBox.Show(warningMessage, "Warning!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred. Exception details: {ex.Message}", "Error!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HelpInfo hi = new HelpInfo();
            hi.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string abmFilePath = "";
            string palettePath = "";
            string savePath = "";
            string fileName = "";

            //open the image file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (string.IsNullOrWhiteSpace(lastOpenedAbmFolder) == false)
                    openFileDialog.InitialDirectory = lastOpenedAbmFolder;

                openFileDialog.Filter = "Harvester image file (*.ABM)|*.ABM";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Open .ABM File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    abmFilePath = openFileDialog.FileName;
                    fileName = openFileDialog.SafeFileName;
                    lastOpenedAbmFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                }
                else
                {
                    return;
                }
            }

            //open the palette file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (string.IsNullOrWhiteSpace(lastOpenedPaletteFolder) == false)
                    openFileDialog.InitialDirectory = lastOpenedPaletteFolder;

                openFileDialog.Filter = "INVITE.PAL (*.PAL)|*.PAL";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Open INVITE.PAL File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    palettePath = openFileDialog.FileName;
                    lastOpenedPaletteFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                }
                else
                {
                    return;
                }
            }

            string saveFileName = "";
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (string.IsNullOrWhiteSpace(lastOpenedBmFolder) == false)
                    saveFileDialog.InitialDirectory = lastOpenedBmFolder;

                if (saveAsGif)
                {
                    saveFileDialog.Filter = "Bitmap Image (*.GIF)|*.GIF";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.Title = "Save .GIF File";
                    saveFileDialog.DefaultExt = ".GIF";
                    saveFileDialog.FileName = fileName.Split('.')[0] + ".GIF";//cause the file name will always be .BM, split and add the .GIF
                }
                else
                {
                    saveFileDialog.Filter = "Bitmap Image (*.PNG)|*.PNG";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.Title = "Save PNG File (a file will be generated for each frame. A number will be appended to the filename)";
                    saveFileDialog.DefaultExt = ".PNG";
                    saveFileDialog.FileName = fileName.Split('.')[0] + ".PNG";//cause the file name will always be .BM, split and add the .GIF
                }



                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    savePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                HarvesterImageConverter.ConvertAnimatedImage(abmFilePath, palettePath, savePath, saveAsGif);
                //MessageBox.Show($"Success!", "Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred. Exception details: {ex.Message}", "Error!");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string abmFolderPath = "";
            string palettePath = "";
            string saveFolder = "";

            //open the image file
            using (VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog())
            {
                openFolderDialog.Description = "Open Folder With .ABM Files";
                openFolderDialog.UseDescriptionForTitle = true;

                if (openFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    abmFolderPath = openFolderDialog.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            //open the palette file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (string.IsNullOrWhiteSpace(lastOpenedPaletteFolder) == false)
                    openFileDialog.InitialDirectory = lastOpenedPaletteFolder;

                openFileDialog.Filter = "INVITE.PAL (*.PAL)|*.PAL";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Open INVITE.PAL File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    palettePath = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }


            using (VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog())
            {
                openFolderDialog.Description = "Save Images to folder (will overwrite any existing files)";
                openFolderDialog.UseDescriptionForTitle = true;

                if (openFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    saveFolder = openFolderDialog.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            try
            {
                FileInfo[] abmFiles = new DirectoryInfo(abmFolderPath).GetFiles().Where(x => x.Name.ToUpper().EndsWith(".ABM")).ToArray(); ;
                
                if (abmFiles.Length == 0)
                    throw new Exception("No ABM files found");

                //go through each bm file. Try to find a .PAL file to match it
                foreach (var abmFile in abmFiles)
                {
                    string abmFileNameNoExtension = abmFile.Name.Split('.')[0];
                    HarvesterImageConverter.ConvertAnimatedImage(abmFile.FullName, palettePath, Path.Combine(saveFolder, abmFileNameNoExtension + ".GIF"));
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred. Exception details: {ex.Message}", "Error!");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void exportAsGif_CheckedChanged(object sender, EventArgs e)
        {
            saveAsGif = exportAsGif.Checked;
        }
    }
}

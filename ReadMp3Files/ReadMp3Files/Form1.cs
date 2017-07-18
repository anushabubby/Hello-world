using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
namespace ReadMp3Files
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openDialog = new CommonOpenFileDialog();
            openDialog.InitialDirectory = "C:\\Users";
            openDialog.IsFolderPicker = true;
            if (openDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = openDialog.FileName;
                UpdateID3Tags(path);
            }
        }

        public void UpdateID3Tags(string path)
        {
            string[] AllDirectories = Directory.GetDirectories(path);
            if (AllDirectories.Length > 0)
            {
                foreach (string dirPath in AllDirectories)
                {
                    if (Directory.GetDirectories(dirPath).Length > 0)
                    {
                        UpdateID3Tags(dirPath);
                    }
                        Details directoryDetails = CheckDirectoryDetails(dirPath);
                    if (directoryDetails.FilesArray.Length > 0)
                    {
                        RefactorFileDetails(directoryDetails.Path, directoryDetails.Name, directoryDetails.Year, directoryDetails.FilesArray);
                    }
                    else
                    {
                        //check if there are further directories.
                    }
                }
            }
            else if (AllDirectories.Length == 1)
            {
                Details directoryDetails = CheckDirectoryDetails(path);
                RefactorFileDetails(directoryDetails.Path, directoryDetails.Name, directoryDetails.Year, directoryDetails.FilesArray);
            }
        }

        public Details CheckDirectoryDetails(string path)
        {
            string dirName = null; uint dirYear = Convert.ToUInt32(DateTime.Now.Year.ToString());
            Details newDirectoryDetails = new Details();

            string directoryPath = path.ToString();
            string directoryName = Path.GetFileName(directoryPath);
            string[] FilesInDir = Directory.GetFiles(directoryPath, "*.mp3");

            if (FilesInDir.Length > 0)
            {
                if (directoryName.Contains("("))
                {
                    dirName = directoryName.Split('(', ')')[0];
                    dirYear = Convert.ToUInt32(directoryName.Split('(', ')')[1]);
                }
                newDirectoryDetails.Path = directoryPath;
                newDirectoryDetails.Name = dirName != null ? dirName : directoryName;
                newDirectoryDetails.Year = dirYear;
                newDirectoryDetails.FilesArray = FilesInDir;
            }
            else
            {
                newDirectoryDetails.Path = directoryPath;
                newDirectoryDetails.Name = directoryName;
                newDirectoryDetails.Year = dirYear;
                newDirectoryDetails.FilesArray = FilesInDir;
            }
            return newDirectoryDetails;
    }

        public void RefactorFileDetails(string dirPath, string dirName, uint dirYear, string[] FilesInDir)
        {
            foreach(string filePath in FilesInDir)
            {
                if (Path.GetExtension(filePath).ToLower() == ".mp3")
                {
                    TagLib.File file = TagLib.File.Create(filePath);
                    file.Tag.Title = dirName;
                    file.Tag.Album = dirName;
                    string Genres = file.Tag.Genres.First();
                    file.Tag.Year = dirYear;
                    file.Save();
                }
            }

        }
    }
}

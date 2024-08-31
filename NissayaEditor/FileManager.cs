using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Word;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {
        class FileManager
        {
            enum FileType { TXT, RTF, ODT, DOCX };
            public string fName;
            FileType fType = FileType.TXT;
            public string fileContent = string.Empty;
            RichTextBox richTextBox;

            public FileManager(string fname, RichTextBox rtb)
            {
                fName = fname;
                richTextBox = rtb;
                string ext = Path.GetExtension(fname).ToLower();
                if (ext == ".txt") fType = FileType.TXT;
                if (ext == ".rtf") fType = FileType.RTF;
                if (ext == ".odt") fType = FileType.ODT;
                if (ext == ".docx" || ext == ".doc") fType = FileType.DOCX;
            }

            public string ReadFile()
            {
                switch (fType)
                {
                    case FileType.TXT:
                        fileContent = ReadFileTxt();
                        break;
                    case FileType.RTF:
                        richTextBox.LoadFile(fName);
                        fileContent = richTextBox.Text;
                        break;
                    case FileType.DOCX:
                        fileContent = ReadFileDoc();
                        break;
                }
                string s = MyanmarCharsCleanup(fileContent);
                if (s != fileContent)
                {
                    fileContent = s;
                    SaveFile(fileContent);
                }
                return fileContent;
            }

            private string ReadFileTxt()
            {
                using (StreamReader reader = new StreamReader(fName))
                {
                    fileContent = reader.ReadToEnd();
                }
                return fileContent;
            }

            private string ReadFileDoc()
            {
                string s = string.Empty;
                // Open a doc file.
                try
                {
                    Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
                    Document document = application.Documents.Open(fName);

                    s = document.Content.Text;
                    //application.Quit();
                }
                catch (FileLoadException e)
                {
                    MessageBox.Show(e.Message);
                }
                return s;
            }

            public string SaveFile(string s, string outFile = "")
            {
                if (outFile.Length == 0) outFile = fName;
                //string[] lines = new string[] { fileContent };
                //File.WriteAllLines(fName, fileContent, Encoding.UTF8);
                s = MyanmarCharsCleanup(s);
                switch (fType)
                {
                    case FileType.TXT:
                        using (StreamWriter writer = new StreamWriter(outFile))
                        {
                            writer.WriteLine(s);
                        }
                        break;
                    case FileType.RTF:
                        richTextBox.SaveFile(outFile);
                        break;
                }
                return s;
            }
        }
    }
}

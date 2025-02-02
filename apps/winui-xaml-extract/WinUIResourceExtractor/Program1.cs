using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUIResourceExtractor
{
    internal class Program1
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WinUI Resource Extractor 2.0 \n");

            var directories = Directory.GetDirectories(s_ControlsPath);

            Console.WriteLine($"Total Number of Files : {WinUIXamlFiles.Count}");
            XamlFileObjectTable xamlFileTable = new();

            foreach(string file in WinUIXamlFiles)
            {
                XamlFileObject xamlFileObject = XamlFileObject.Create(file);
                xamlFileTable.Insert(file, xamlFileObject);
            }

            xamlFileTable.FlagUsedResources();

            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "outs");
            Directory.CreateDirectory(outputDirectory);
            foreach(string file in WinUIXamlFiles)
            {
                XamlFileObject? xamlFileObject = xamlFileTable.GetFileObject(file);
                
                if (xamlFileObject is null)
                    continue;

                xamlFileObject.Save(outputDirectory);
            }
        }

        #region Private Methods

        private static List<string> WinUIXamlFiles
        {
            get
            {
                if(s_winuiXamlFiles is null)
                {
                    var directories = Directory.GetDirectories(s_ControlsPath);
                    List<string> xamlFiles = new();

                    foreach (string directory in directories)
                    {
                        var files = Directory.GetFiles(directory, "*.xaml", SearchOption.TopDirectoryOnly);
                        xamlFiles.AddRange(files);
                    }

                    s_winuiXamlFiles = xamlFiles;
                }

                return s_winuiXamlFiles;
            }
        }

        #endregion

        #region Private Members

        static List<string>? s_winuiXamlFiles;
        static string s_ControlsPath = "C:\\work\\repos\\microsoft-ui-xaml-lift\\controls\\dev\\";

        #endregion
    }
}

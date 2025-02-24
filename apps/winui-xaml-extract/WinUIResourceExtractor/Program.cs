using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUIResourceExtractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WinUI Resource Extractor 2.0 \n");

            Console.WriteLine($"Total Number of Files : {WinUIXamlFiles.Count}");

            XamlFileObjectTable xamlFileTable = new();

            foreach (string file in WinUIXamlFiles)
            {
                XamlFileObject xamlFileObject = XamlFileObject.Create(file);
                xamlFileTable.Insert(file, xamlFileObject);
            }

            xamlFileTable.FlagUsedResources();

            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "outs");
            Directory.CreateDirectory(outputDirectory);
            foreach (string file in WinUIXamlFiles)
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
                if (s_winuiXamlFiles is null)
                {
                    string controlStyleDirectoryPath = Path.Combine(s_RepoRoot, s_ControlsPath);
                    var directories = Directory.GetDirectories(controlStyleDirectoryPath);
                
                    List<string> xamlFiles = new();

                    foreach (string directory in directories)
                    {
                        var files = Directory.GetFiles(directory, "*.xaml", SearchOption.TopDirectoryOnly);
                        xamlFiles.AddRange(files);
                    }

                    s_winuiXamlFiles = xamlFiles;

                    if(s_IncludedFiles is not null)
                    {
                        foreach(string file in s_IncludedFiles)
                        {
                            s_winuiXamlFiles.Add(Path.Combine(s_RepoRoot, file));
                        }
                    }

                    if(s_ExcludedFiles is not null)
                    {
                        int cnt = 0;
                        foreach(string file in s_ExcludedFiles)
                        {
                            //Console.WriteLine(file);
                            bool removed = s_winuiXamlFiles.Remove(Path.Combine(s_RepoRoot,file));
                            //if (removed) cnt++;
                        }
                        //Console.WriteLine($"Removed {cnt} files");
                    }
                }

                return s_winuiXamlFiles;
            }
        }

        #endregion

        #region Private Members

        static List<string>? s_winuiXamlFiles;
        static string s_ControlsPath = @".\controls\dev\";

        static List<string> s_IncludedFiles = new() { @".\controls\dev\Materials\Acrylic\AcrylicBrush_themeresources.xaml" };
        static List<string> s_ExcludedFiles = new() { @".\controls\dev\CommonStyles\Deprecated_themeresources_any.xaml",
                                                          @".\controls\dev\CommonStyles\Deprecated_themeresources.xaml" };


        static string s_RepoRoot = @"C:\work\repos\microsoft-ui-xaml-lift\";
        #endregion
    }
}

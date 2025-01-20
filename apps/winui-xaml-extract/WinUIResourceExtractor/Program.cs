using System.Diagnostics;
using System.Xml;

namespace WinUIResourceExtractor
{
    // Why do I need this ?
    // What are the questions I need to find the answers for ?
    //      1. Which resources are currently being used in WinUI's styles 
    //      2. Collate a list of all the brushes / colors and other resources that are being used in WinUI
    //      2. Find a list of all the Fluent colors and brushes defined in WinUI

    internal class Program
    {
        static void Main(string[] args)
        {
            string output_directory = Directory.GetCurrentDirectory();

            Console.WriteLine("WinUI Resource Extractor !! Hope this works.");
            var directories = Directory.GetDirectories(s_ControlsPath);

            foreach(string file in GetXamlStyleFiles())
            {
                ProcessFile(file);
            }
        }

        private static void DumpAnalysis(string file)
        {
            throw new NotImplementedException();
        }

        static void ProcessFile(string xamlFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xamlFile);

            XmlNamespaceManager mgr = new(doc.NameTable);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            mgr.AddNamespace("default", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");

            XmlElement? root = doc.DocumentElement;

            // Extract Styles from the files
            //Console.WriteLine($"{Path.GetFileName(file)} : {ExtractStyles(file).Count}");
            XmlNodeList styles = doc.GetElementsByTagName("Style");

            foreach(XmlNode style in styles)
            {
                ExtractUsedResourcesFromStyles(style);
            }

            // SelectNodes is not working due to namespace issue, we need to add an XmlNamespaceManager,
            // add the necessary namespaces and only then we can select nodes

            // Extract ThemeResources from the file
            List<XmlNode> definedThemeResourcesDark = new();
            List<XmlNode> definedThemeResourcesLight = new();
            List<XmlNode> definedThemeResourcesHighContrast = new();
            List<XmlNode> definedStaticResources = new();

            foreach (XmlNode node in root.ChildNodes)
            {
                if(node.Name.Contains("ResourceDictionary.ThemeDictionaries"))
                {
                    ParseThemeDictionary(node, xamlFile);
                }
                else if(node.Name.Contains("Style"))
                {
                    continue;
                }
                else
                {
                    definedStaticResources.Add(node);
                }
            }

            XmlNodeList themeDictionaries = doc.GetElementsByTagName("ResourceDictionary.ThemeDictionaries");
            if(themeDictionaries.Count != 0)
            {
                if (themeDictionaries.Count > 1)
                    throw new InvalidDataException("Assumed that there should be only one main ThemeDictionaries per file");

               

                // All the nodes have the same number of resources defined so we will not worry about every variation here
                XmlNode? defaultThemeDictionary = themeDictionaries[0];
                if(defaultThemeDictionary is not null)
                {
                    XmlNodeList? keyNodes = defaultThemeDictionary.SelectNodes("//@x:Key", mgr);
                    if(keyNodes is not null)
                    {
                        foreach(XmlAttribute key in keyNodes)
                        {
                            definedThemeResources.Add(key.Value);
                        }
                    }
                }
            }

            // Extract StaticResources from the files

            foreach(XmlNode child in root.ChildNodes)
            {
                if (child.Name.Contains("Style") || child.Name.Contains("ResourceDictionary.ThemeDictionaries"))
                    continue;

                XmlNodeList keyAttributes = child.SelectNodes("//@x:Key", mgr);
                if(keyAttributes is not null)
                {
                    foreach (XmlAttribute attr in keyAttributes)
                    {
                        definedStaticResources.Add(attr.Value);
                    }
                }
            }

            stylesDict[xamlFile] = styles;
        }

        private static void ParseThemeDictionary(XmlNode themeDictionary, string xamlFile)
        {
            List<XmlNodeList> themeResources = new(3);

            foreach(XmlNode child in themeDictionary)
            {
                ThemeMode mode = GetThemeMode(child);
                themeResources[mode] = child.ChildNodes;

            }
        }

        private static ThemeMode GetThemeMode(XmlNode node)
        {
            throw new NotImplementedException();
        }

        private static void ExtractUsedResourcesFromStyles(XmlNode style)
        {
            List<string> usedThemeResources = new();
            List<string> usedStaticResources = new();
            List<string> templateBindings = new();

            XmlNodeList? attributes = style.SelectNodes("//@*");

            if (attributes is null)
                return;

            foreach (XmlAttribute attr in attributes)
            {
                int valueType = -1;

                if (attr.Value.StartsWith("{"))
                {
                    if (attr.Value.StartsWith("{ThemeResource"))
                    {
                        valueType = 0;
                    }
                    else if (attr.Value.StartsWith("{StaticResource"))
                    {
                        valueType = 1;
                    }
                    else if (attr.Value.StartsWith("{TemplateBinding"))
                    {
                        valueType = 2;
                    }
                }

                string val = attr.Value.Substring(attr.Value.IndexOf(' ') + 1);
                val = val.Substring(0, val.Length - 1);

                switch (valueType)
                {
                    case 0:
                        usedThemeResources.Add(val);
                        break;
                    case 1:
                        usedStaticResources.Add(val);
                        break;
                    case 2:
                        templateBindings.Add(val);
                        break;
                    default:
                        break;
                }
            }

            usedThemeResourcesDict[style] = usedThemeResources;
            usedStaticResourcesDict[style] = usedStaticResources;
            templateBindingsDict[style] = templateBindings;
        }

        static IEnumerable<string> GetXamlStyleFiles()
        {
            var directories = Directory.GetDirectories(s_ControlsPath);

            foreach(string directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.xaml", SearchOption.TopDirectoryOnly);
                foreach(string file in files)
                {
                    yield return file;
                }
            }
        }

        static XmlNodeList ExtractStyles(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            return doc.GetElementsByTagName("Style");
        }

        static Dictionary<string, XmlNodeList> stylesDict = new();
        static Dictionary<XmlNode, List<string>> usedThemeResourcesDict = new();
        static Dictionary<XmlNode, List<string>> usedStaticResourcesDict= new();
        static Dictionary<XmlNode, List<string>> templateBindingsDict = new();

        static string s_ControlsPath = "C:\\work\\repos\\microsoft-ui-xaml-lift\\controls\\dev\\";


        enum ThemeMode
        {
            Light = 0,

            Dark = 1,

            HighContrast = 2
        }
    }
}

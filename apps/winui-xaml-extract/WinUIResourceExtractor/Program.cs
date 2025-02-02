using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using ExtType = WinUIResourceExtractor.Enums.ExtensionType;
using Theme = WinUIResourceExtractor.Enums.ThemeMode;

namespace WinUIResourceExtractor
{
    // Why do I need this ?
    // What are the questions I need to find the answers for ?
    //      1. Which resources are currently being used in WinUI's styles 
    //      2. Collate a list of all the brushes / colors and other resources that are being used in WinUI
    //      2. Find a list of all the Fluent colors and brushes defined in WinUI

    internal class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("WinUI Resource Extractor !! Hope this works. \n");
        //    var directories = Directory.GetDirectories(s_ControlsPath);

        //    int fc = 0;
        //    foreach(string file in GetXamlStyleFiles())
        //    {
        //        fc++;
        //        ProcessFile(file);
        //        CombineDefinedResources(file);
        //    }

        //    MarkUsedResources();
        //    SaveDefinedResources();
        //    SaveUsedResources();

        //    Console.WriteLine($"Total Number of Files : {fc}");
        //    Console.WriteLine($"Total Number of Styles : {styleResourcesDict.Count}");
        //}

        private static void SaveUsedResources()
        {
            string directory = Directory.GetCurrentDirectory();

            string usedStaticResourceFile = Path.Combine(directory, "WinUI.SR.Used.xaml");
            string notUsedStaticResourceFile = Path.Combine(directory, "WinUI.SR.Not.xaml");
            XmlDocument usedDoc = new();
            XmlDocument otherDoc = new();
            usedDoc.LoadXml(s_rootElement);
            otherDoc.LoadXml(s_rootElement);
            
            for(int i=0;i<allDefinedStaticResourceKeys.Count;i++)
            {
                if (allUsedStaticResources[i])
                {
                    XmlNode node = usedDoc.CreateNode(XmlNodeType.Element, "Key", null);
                    node.AppendChild(usedDoc.CreateTextNode(allDefinedStaticResourceKeys[i]));
                    usedDoc.DocumentElement?.AppendChild(node);
                }
                else
                {
                    XmlNode node = otherDoc.CreateNode(XmlNodeType.Element, "Key", null);
                    node.AppendChild(otherDoc.CreateTextNode(allDefinedStaticResourceKeys[i]));
                    otherDoc.DocumentElement?.AppendChild(node);
                }
            }

            usedDoc.Save(usedStaticResourceFile);
            otherDoc.Save(notUsedStaticResourceFile);
        }

        private static void SaveDefinedResources()
        {
            string directory = Directory.GetCurrentDirectory();

            string stylesDumpFile = Path.Combine(directory, "WinUI.Styles.xaml");
            XmlDocument styleDocument = new XmlDocument();
            styleDocument.LoadXml(s_rootElement);

            foreach (XmlNodeList nodes in stylesDict.Values)
            {
                foreach(XmlNode node in nodes)
                {
                    XmlNode inode = styleDocument.ImportNode(node, true);
                    styleDocument.DocumentElement?.AppendChild(inode);
                }
            }

            styleDocument.Save(stylesDumpFile);


            string staticResourceDumpFile = Path.Combine(directory, "WinUI.StaticResource.xaml");
            XmlDocument staticResourceDocument = new XmlDocument();
            staticResourceDocument.LoadXml(s_rootElement);

            foreach(XmlNode node in allDefinedStaticResources)
            {
                XmlNode inode = staticResourceDocument.ImportNode(node, true);
                staticResourceDocument.DocumentElement?.AppendChild(inode);
            }

            staticResourceDocument.Save(staticResourceDumpFile);
        }

        private static void MarkUsedResources()
        {
            List<bool> usedStaticResources = MarkUsedStaticResources();
            List<List<bool>> usedThemeResources = MarkUsedThemeResources();

            RecursiveMarkUsedStaticResources(ref usedStaticResources);
            RecursiveMarkUsedThemeResources(ref usedThemeResources);

            int totalUsedStaticResources = 0;
            for(int i=0;i<usedStaticResources.Count;i++) { if(usedStaticResources[i]) totalUsedStaticResources++; }

            List<int> totalUsedThemeResources = new List<int>();
            totalUsedThemeResources.Add(0);
            totalUsedThemeResources.Add(0);
            totalUsedThemeResources.Add(0);

            foreach (Theme mode in Enum.GetValues(typeof(Theme)))
            {
                if (mode == Theme.Null) continue;

                for (int i = 0; i < usedThemeResources[(int)mode].Count; i++)
                {
                    if (usedThemeResources[(int)mode][i])
                        totalUsedThemeResources[(int)mode]++;
                }
            }

            allUsedStaticResources = usedStaticResources;
            allUsedThemeResources = usedThemeResources;

            Console.WriteLine($"Total Used Static Resources : {allDefinedStaticResourceKeys.Count} {totalUsedStaticResources}");
            Console.WriteLine($"Total Used Theme Resources :");
            Console.WriteLine($"\t Light : {allDefinedThemeResourceKeys[(int)Theme.Light].Count} {totalUsedThemeResources[(int)Theme.Light]}");
            Console.WriteLine($"\t Dark : {allDefinedThemeResourceKeys[(int)Theme.Dark].Count} {totalUsedThemeResources[(int)Theme.Dark]}");
            Console.WriteLine($"\t HighContrast : {allDefinedThemeResourceKeys[(int)Theme.HighContrast].Count} {totalUsedThemeResources[(int)Theme.HighContrast]}");
        }

        private static void RecursiveMarkUsedThemeResources(ref List<List<bool>> usedThemeResources)
        {
            for(int mode=0;mode<3;mode++)
            {
                Queue<int> queue = new Queue<int>();
                for (int i = 0; i < usedThemeResources[mode].Count; i++)
                {
                    if (usedThemeResources[mode][i])
                    {
                        queue.Enqueue(i);
                    }
                }
            
                while(queue.Count > 0)
                {
                    int i = queue.Dequeue();
                    XmlNode node = allDefinedThemeResources[mode][i];
                    if (XamlUtils.TryGetUsedResource(node, out List<string> usedResources))
                    {
                        foreach (string usedResource in usedResources)
                        {
                            for (int j = 0; j < allDefinedStaticResources.Count; j++)
                            {
                                if (!usedThemeResources[mode][j] && string.Equals(XamlUtils.GetKeyName(allDefinedStaticResources[j]), usedResource, StringComparison.OrdinalIgnoreCase))
                                {
                                    usedThemeResources[mode][j] = true;
                                    queue.Enqueue(j);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void RecursiveMarkUsedStaticResources(ref List<bool> usedStaticResources)
        {
            Queue<int> queue = new Queue<int>();
            for(int i=0;i<usedStaticResources.Count;i++)
            {
                if(usedStaticResources[i])
                {
                    queue.Enqueue(i);
                }
            }

            while(queue.Count > 0)
            {
                int i = queue.Dequeue();
                XmlNode node = allDefinedStaticResources[i];
                if (XamlUtils.TryGetUsedResource(node, out List<string> usedResources))
                {
                    foreach(string usedResource in usedResources)
                    {
                        for (int j=0; j < allDefinedStaticResources.Count;j++)
                        {
                            if (!usedStaticResources[j] && string.Equals(XamlUtils.GetKeyName(allDefinedStaticResources[j]), usedResource, StringComparison.OrdinalIgnoreCase))
                            {
                                usedStaticResources[j] = true;
                                queue.Enqueue(j);
                            }
                        }
                    }
                }
            }
        }

        private static List<bool> MarkUsedStaticResources()
        {
            List<bool> usedStaticResources = new List<bool>(new bool[allDefinedStaticResources.Count]);
            for (int i = 0; i < allDefinedStaticResources.Count; i++) { usedStaticResources[i] = false; }

            // Marking all static resources used
            foreach (XmlNode style in styleResourcesDict.Keys)
            {
                List<string> styleStaticResources = styleResourcesDict[style][(int)ExtType.StaticResource];
                List<string> styleThemeResources = styleResourcesDict[style][(int)ExtType.ThemeResource];

                if (styleStaticResources is not null)
                {
                    foreach (string staticResource in styleStaticResources)
                    {
                        for (int i = 0; i < allDefinedStaticResources.Count; i++)
                        {
                            if (string.Equals(allDefinedStaticResourceKeys[i], staticResource, StringComparison.OrdinalIgnoreCase))
                            {
                                if (usedStaticResources[i] == false)
                                {
                                    usedStaticResources[i] = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (styleThemeResources is not null)
                {
                    foreach (string staticResource in styleThemeResources)
                    {
                        for (int i = 0; i < allDefinedStaticResources.Count; i++)
                        {
                            if (string.Equals(allDefinedStaticResourceKeys[i], staticResource, StringComparison.OrdinalIgnoreCase))
                            {
                                if (usedStaticResources[i] == false)
                                {
                                    usedStaticResources[i] = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return usedStaticResources;
        }

        private static List<List<bool>> MarkUsedThemeResources()
        {
            List<List<bool>> usedThemeResources = new List<List<bool>>();
            for (int i = 0; i < 3; i++)
            {
                List<bool> listOfBools = new List<bool>();
                for (int j = 0; j < allDefinedThemeResources[i].Count; j++) listOfBools.Add(false);
                usedThemeResources.Add(listOfBools);
            }


            // Marking first level of theme resources used
            foreach (XmlNode style in styleResourcesDict.Keys)
            {
                foreach (Theme mode in Enum.GetValues(typeof(Theme)))
                {
                    if (mode == Theme.Null)
                        continue;

                    List<string> styleThemeResources = styleResourcesDict[style][(int)ExtType.ThemeResource];
                    if (styleThemeResources is not null)
                    {
                        foreach (string themeResource in styleThemeResources)
                        {
                            for (int i = 0; i < allDefinedThemeResources[(int)mode].Count; i++)
                            {
                                if (string.Equals(XamlUtils.GetKeyName(allDefinedThemeResources[(int)mode][i]), themeResource, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (usedThemeResources[(int)mode][i] == false)
                                    {
                                        usedThemeResources[(int)mode][i] = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return usedThemeResources;
        }


        private static void CombineDefinedResources(string file)
        {
            // Combining StaticResource's
            foreach(XmlNode node in definedStaticResourcesDict[file])
            {
                allDefinedStaticResourceKeys.Add(XamlUtils.GetKeyName(node));
                allDefinedStaticResources.Add(node);
            }

            if(allDefinedThemeResources.Count < 3)
            {
                allDefinedThemeResources.Add(new List<XmlNode>());
                allDefinedThemeResources.Add(new List<XmlNode>());
                allDefinedThemeResources.Add(new List<XmlNode>());
                allDefinedThemeResourceKeys.Add(new List<string>());
                allDefinedThemeResourceKeys.Add(new List<string>());
                allDefinedThemeResourceKeys.Add(new List<string>());
            }
        
            // Combining ThemeResource's
            
            foreach(Theme mode in Enum.GetValues(typeof(Theme)))
            {
                if (mode == Theme.Null)
                    continue;

                if (definedThemeResourcesDict[file] is not null)
                {
                    foreach(XmlNode node in definedThemeResourcesDict[file][(int)mode])
                    {
                        allDefinedThemeResourceKeys[(int)mode].Add(XamlUtils.GetKeyName(node));
                        allDefinedThemeResources[(int)mode].Add(node);
                    }
                }
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

            // SelectNodes was not working due to namespace issue, we need to add an XmlNamespaceManager,
            // add the necessary namespaces and only then we can select nodes
            XmlNamespaceManager mgr = new(doc.NameTable);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            mgr.AddNamespace("default", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");

            XmlElement? root = doc.DocumentElement;

            // Extract Styles from the files
            XmlNodeList styles = doc.GetElementsByTagName("Style");

            foreach(XmlNode style in styles)
            {
                var styleResources = ExtractUsedResourcesFromStyles(style);
                if(styleResources is not null)
                {
                    styleResourcesDict[style] = styleResources;
                }
            }


            // Extract ThemeResources from the file
            List<XmlNodeList>? definedThemeResources = null;
            List<XmlNode> definedStaticResources = new();

            foreach (XmlNode node in root.ChildNodes)
            {
                if(node.Name.Contains("ResourceDictionary.ThemeDictionaries"))
                {
                    definedThemeResources = ParseThemeDictionary(node, xamlFile);
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

            stylesDict[xamlFile] = styles;
            definedStaticResourcesDict[xamlFile] = definedStaticResources;
            definedThemeResourcesDict[xamlFile] = definedThemeResources ?? new();
        }

        private static List<XmlNodeList> ParseThemeDictionary(XmlNode themeDictionary, string xamlFile)
        {
            List<XmlNodeList> themeResources = new(3);
            themeResources.Add(null);
            themeResources.Add(null);
            themeResources.Add(null);

            foreach (XmlNode child in themeDictionary)
            {
                Theme mode = XamlUtils.GetTheme(child);
                if (mode == Theme.Null) 
                    continue;
                themeResources[(int)mode] = child.ChildNodes;
            }

            return themeResources;
        }

        private static List<List<string>>? ExtractUsedResourcesFromStyles(XmlNode style)
        {

            List<List<string>> styleResources = new(3);
            styleResources.Add(new List<string>());
            styleResources.Add(new List<string>());
            styleResources.Add(new List<string>());

            XmlNodeList? attributes = style.SelectNodes("//@*");

            if (attributes is null)
            {
                return null;
            }

            foreach (XmlAttribute attr in attributes)
            {
                ExtType ext = XamlUtils.GetMarkupExtensionType(attr.Value);
                if(ext is ExtType.Null)
                {
                    continue;
                }

                string val = attr.Value.Substring(attr.Value.IndexOf(' ') + 1);
                val = val.Substring(0, val.Length - 1);

                styleResources[(int)ext].Add(val);
            }

            return styleResources;
        }

        private static IEnumerable<string> GetXamlStyleFiles()
        {var directories = Directory.GetDirectories(s_ControlsPath);

            foreach(string directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.xaml", SearchOption.TopDirectoryOnly);
                foreach(string file in files)
                {
                    yield return file;
                }
            }
            
        }

        #region Private Fields

        static Dictionary<string, XmlNodeList> stylesDict = new();
        static Dictionary<XmlNode, List<List<string>>> styleResourcesDict = new();
        static Dictionary<string, List<XmlNodeList>> definedThemeResourcesDict = new();
        static Dictionary<string, List<XmlNode>> definedStaticResourcesDict = new();

        static List<List<XmlNode>> allDefinedThemeResources = new(3);
        static List<XmlNode> allDefinedStaticResources = new();

        static List<List<string>> allDefinedThemeResourceKeys = new(3);
        static List<string> allDefinedStaticResourceKeys = new();
        static List<List<bool>> allUsedThemeResources = new(3);
        static List<bool> allUsedStaticResources = new();

        #endregion

        static string s_ControlsPath = "C:\\work\\repos\\microsoft-ui-xaml-lift\\controls\\dev\\";

        const string s_rootElement = "<ResourceDictionary xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></ResourceDictionary>";

    }
}

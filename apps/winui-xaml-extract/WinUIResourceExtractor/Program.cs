using System.Diagnostics;
using System.Globalization;
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

            int fc = 0;
            foreach(string file in GetXamlStyleFiles())
            {
                fc++;
                ProcessFile(file);
                CombineDefinedResources(file);
            }

            MarkUsedResources();

            Console.WriteLine($"Total Defined Theme Resources : {allDefinedThemeResources[(int)ThemeMode.Light].Count}");
            Console.WriteLine($"Total Defined Static Resources : {allDefinedStaticResources.Count}");
            Console.WriteLine($"Total Number of Styles : {styleResourcesDict.Count}");
            Console.WriteLine($"Total Number of Files : {fc}");

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

            foreach(ThemeMode mode in Enum.GetValues(typeof(ThemeMode)))
            {
                if(mode == ThemeMode.Null) continue;

                for (int i = 0; i < usedThemeResources[(int)mode].Count; i++)
                {
                    if (usedThemeResources[(int)mode][i])
                        totalUsedThemeResources[(int)mode]++;
                }
            }

            Console.WriteLine($"Total Used Static Resources : {totalUsedStaticResources}");
            Console.WriteLine($"Total Used Theme Resources :");
            Console.WriteLine($"\n Light : {totalUsedThemeResources[(int)ThemeMode.Light]}");
            Console.WriteLine($"\n Dark : {totalUsedThemeResources[(int)ThemeMode.Dark]}");
            Console.WriteLine($"\n HighContrast : {totalUsedThemeResources[(int)ThemeMode.HighContrast]}");
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
                    if (TryGetUsedResource(node, out List<string> usedResources))
                    {
                        foreach (string usedResource in usedResources)
                        {
                            for (int j = 0; j < allDefinedStaticResources.Count; j++)
                            {
                                if (!usedThemeResources[mode][j] && string.Equals(GetKeyName(allDefinedStaticResources[j]), usedResource, StringComparison.OrdinalIgnoreCase))
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
                if (TryGetUsedResource(node, out List<string> usedResources))
                {
                    foreach(string usedResource in usedResources)
                    {
                        for (int j=0; j < allDefinedStaticResources.Count;j++)
                        {
                            if (!usedStaticResources[j] && string.Equals(GetKeyName(allDefinedStaticResources[j]), usedResource, StringComparison.OrdinalIgnoreCase))
                            {
                                usedStaticResources[j] = true;
                                queue.Enqueue(j);
                            }
                        }
                    }
                }
            }
        }

        private static bool TryGetUsedResource(XmlNode node, out List<string> usedResources)
        {
            usedResources = new List<string>();

            if(node.Name.Contains("StaticResource"))
            {
                foreach(XmlAttribute attr in node.SelectNodes("//@*"))
                {
                    if(attr.Name.Contains("ResourceKey"))
                    {
                        usedResources.Add(attr.Value);
                    }
                }
            }
            else
            {
                foreach(XmlAttribute attr in node.SelectNodes("//@*"))
                {
                    ExtensionType ext = GetValueExtensionType(attr.Value);
                    if (ext is ExtensionType.Null)
                        continue;

                    if(ext is not ExtensionType.TemplateBinding)
                    {
                        string val = attr.Value.Substring(attr.Value.IndexOf(' ') + 1);
                        val = val.Substring(0, val.Length - 1);
                        usedResources.Add(val);
                    }
                }
            }

            return usedResources.Count != 0;
        }

        private static List<bool> MarkUsedStaticResources()
        {
            List<bool> usedStaticResources = new List<bool>(new bool[allDefinedStaticResources.Count]);
            for (int i = 0; i < allDefinedStaticResources.Count; i++) { usedStaticResources[i] = false; }

            // Marking all static resources used
            foreach (XmlNode style in styleResourcesDict.Keys)
            {
                List<string> styleStaticResources = styleResourcesDict[style][(int)ExtensionType.StaticResource];
                if (styleStaticResources is not null)
                {
                    foreach (string staticResource in styleStaticResources)
                    {
                        for (int i = 0; i < allDefinedStaticResources.Count; i++)
                        {
                            if (string.Equals(GetKeyName(allDefinedStaticResources[i]), staticResource, StringComparison.OrdinalIgnoreCase))
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
                foreach (ThemeMode mode in Enum.GetValues(typeof(ThemeMode)))
                {
                    if (mode == ThemeMode.Null)
                        continue;

                    List<string> styleThemeResources = styleResourcesDict[style][(int)ExtensionType.ThemeResource];
                    if (styleThemeResources is not null)
                    {
                        foreach (string themeResource in styleThemeResources)
                        {
                            for (int i = 0; i < allDefinedThemeResources[(int)mode].Count; i++)
                            {
                                if (string.Equals(GetKeyName(allDefinedThemeResources[(int)mode][i]), themeResource, StringComparison.OrdinalIgnoreCase))
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


        private static string GetKeyName(XmlNode node)
        {
            foreach(XmlAttribute attr in node.SelectNodes("//@*"))
            {
                if(attr.Name.Contains("Key"))
                {
                    return attr.Value;
                }
            }

            return "";
        }

        private static void CombineDefinedResources(string file)
        {
            // Combining StaticResource's
            allDefinedStaticResources.AddRange(definedStaticResourcesDict[file]);

            if(allDefinedThemeResources.Count < 3)
            {
                allDefinedThemeResources.Add(new List<XmlNode>());
                allDefinedThemeResources.Add(new List<XmlNode>());
                allDefinedThemeResources.Add(new List<XmlNode>());
            }
        
            // Combining ThemeResource's
            foreach(ThemeMode mode in Enum.GetValues(typeof(ThemeMode)))
            {
                if (mode == ThemeMode.Null)
                    continue;

                if (definedThemeResourcesDict[file] is not null)
                {
                    foreach(XmlNode node in definedThemeResourcesDict[file][(int)mode])
                    {
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
            List<XmlNodeList> definedThemeResources = null;
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

            #region Not Needed Code

            //XmlNodeList themeDictionaries = doc.GetElementsByTagName("ResourceDictionary.ThemeDictionaries");
            //if(themeDictionaries.Count != 0)
            //{
            //    if (themeDictionaries.Count > 1)
            //        throw new InvalidDataException("Assumed that there should be only one main ThemeDictionaries per file");



            //    // All the nodes have the same number of resources defined so we will not worry about every variation here
            //    XmlNode? defaultThemeDictionary = themeDictionaries[0];
            //    if(defaultThemeDictionary is not null)
            //    {
            //        XmlNodeList? keyNodes = defaultThemeDictionary.SelectNodes("//@x:Key", mgr);
            //        if(keyNodes is not null)
            //        {
            //            foreach(XmlAttribute key in keyNodes)
            //            {
            //                definedThemeResources.Add(key.Value);
            //            }
            //        }
            //    }
            //}

            // Extract StaticResources from the files

            //foreach(XmlNode child in root.ChildNodes)
            //{
            //    if (child.Name.Contains("Style") || child.Name.Contains("ResourceDictionary.ThemeDictionaries"))
            //        continue;

            //    XmlNodeList keyAttributes = child.SelectNodes("//@x:Key", mgr);
            //    if(keyAttributes is not null)
            //    {
            //        foreach (XmlAttribute attr in keyAttributes)
            //        {
            //            definedStaticResources.Add(attr.Value);
            //        }
            //    }
            //}

            #endregion

            stylesDict[xamlFile] = styles;
            definedStaticResourcesDict[xamlFile] = definedStaticResources;
            definedThemeResourcesDict[xamlFile] = definedThemeResources;
        }

        private static List<XmlNodeList> ParseThemeDictionary(XmlNode themeDictionary, string xamlFile)
        {
            List<XmlNodeList> themeResources = new(3);
            themeResources.Add(null);
            themeResources.Add(null);
            themeResources.Add(null);

            foreach (XmlNode child in themeDictionary)
            {
                ThemeMode mode = GetThemeMode(child);
                if (mode == ThemeMode.Null) 
                    continue;
                themeResources[(int)mode] = child.ChildNodes;
            }

            return themeResources;
        }

        private static ThemeMode GetThemeMode(XmlNode node)
        {
            foreach(XmlAttribute attr in node.Attributes)
            {
                if(attr.Name.Contains("Key"))
                {
                    switch(attr.Value)
                    {
                        case "Default":
                            return ThemeMode.Dark;
                        case "Light":
                            return ThemeMode.Light;
                        case "HighContrast":
                            return ThemeMode.HighContrast;
                    }
                }
            }

            return ThemeMode.Null;
        }

        private static ExtensionType GetValueExtensionType(ReadOnlySpan<char> value)
        {
            if (value.Length == 0)
                return ExtensionType.Null;

            if (value.StartsWith("{"))
            {

                if(value.StartsWith("{ThemeResource", StringComparison.OrdinalIgnoreCase))
                {
                    return ExtensionType.ThemeResource;
                }

                if(value.StartsWith("{StaticResource", StringComparison.OrdinalIgnoreCase))
                {
                    return ExtensionType.StaticResource;
                }

                if(value.Contains("TemplateBinding", StringComparison.OrdinalIgnoreCase))
                {
                    return ExtensionType.TemplateBinding;
                }

            }

            return ExtensionType.Null;
        }

        private static List<List<string>>? ExtractUsedResourcesFromStyles(XmlNode style)
        {

            List<List<string>> styleResources = new(3);
            styleResources.Add(new List<string>());
            styleResources.Add(new List<string>());
            styleResources.Add(new List<string>());

            XmlNodeList? attributes = style.SelectNodes("//@*");

            if (attributes is null)
                return null;

            foreach (XmlAttribute attr in attributes)
            {
                ExtensionType ext = GetValueExtensionType(attr.Value);
                if(ext is ExtensionType.Null) 
                    continue;

                string val = attr.Value.Substring(attr.Value.IndexOf(' ') + 1);
                val = val.Substring(0, val.Length - 1);

                styleResources[(int)ext].Add(val);
            }

            return styleResources;
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

        #region Private Fields

        static Dictionary<string, XmlNodeList> stylesDict = new();
        static Dictionary<XmlNode, List<List<string>>> styleResourcesDict = new();
        static Dictionary<string, List<XmlNodeList>> definedThemeResourcesDict = new();
        static Dictionary<string, List<XmlNode>> definedStaticResourcesDict = new();

        static List<List<XmlNode>> allDefinedThemeResources = new(3);
        static List<XmlNode> allDefinedStaticResources = new();

        static XmlNamespaceManager s_manager;

        #endregion

        static string s_ControlsPath = "E:\\repos\\microsoft-ui-xaml-lift\\controls\\dev\\";

        #region Private Structs

        enum ThemeMode
        {
            Light = 0,

            Dark = 1,

            HighContrast = 2,

            Null = 3
        }

        enum ExtensionType
        {
            ThemeResource = 0,
            StaticResource = 1,
            TemplateBinding = 2,
            Null = 3
        }

        #endregion
    }
}

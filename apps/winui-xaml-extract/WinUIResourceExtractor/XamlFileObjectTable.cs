using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Theme = WinUIResourceExtractor.Enums.ThemeMode;
using Ext = WinUIResourceExtractor.Enums.ExtensionType;


namespace WinUIResourceExtractor
{
    internal class XamlFileObjectTable
    {
        #region Public Methods

        public void Insert(string file, XamlFileObject fileObject)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(file);
            ArgumentNullException.ThrowIfNull(fileObject);

            if (TryGetFileIndex(file) != -1)
                return;

            XamlFiles.Add(file);
            int fileIndex = XamlFiles.Count - 1;

            xamlFileObjectMap[fileIndex] = fileObject;

            InsertStyles(fileIndex, fileObject);
            InsertStaticResources(fileIndex, fileObject);
            InsertThemeResources(fileIndex, fileObject);
        }

        public void FlagUsedResources()
        {
            // Flag all the Static and Theme resources that have
            // been directly or indirectly used in any style.
            FlagStyleResources();
            FlagStaticResourcesRecursively();
            FlagThemeResourcesRecursively();

            for(int i=0;i<AllStaticResources.Count;i++)
            {
                if (AllUsedStaticResources.Contains(i))
                {
                    int fileIndex = staticResourceFileMap[i];
                    xamlFileObjectMap[fileIndex].FlagStaticResource(AllStaticResources[i]);
                }
            }

            foreach(Theme mode in Enum.GetValues(typeof(Theme)))
            {
                if (mode == Theme.Null)
                    continue;
                for(int i = 0; i < AllThemeResourcesList[(int)mode].Count; i++)
                {
                    if (AllUsedThemeResourcesList[(int)mode].Contains(i))
                    {
                        int fileIndex = themeResourceFileMaps[(int)mode][i];
                        xamlFileObjectMap[fileIndex].FlagThemeResource(
                            AllThemeResourcesList[(int)mode][i], mode);
                    }
                }
            }
        }

        public XamlFileObject? GetFileObject(string file)
        {
            int index = TryGetFileIndex(file);
            if(index != -1)
            {
                return xamlFileObjectMap[index];
            }

            return null;
        }

        #endregion

        #region Private Methods

        private int TryGetFileIndex(string file)
        {
            for(int i=0;i<XamlFiles.Count;i++)
            {
                if(string.Equals(file, XamlFiles[i], StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            // File not found hence returning -1
            return -1;
        }

        private void InsertStyles(int fileIndex, XamlFileObject fileObject)
        {
            foreach(XmlNode style in fileObject.Styles)
            {
                styleFileMap[style] = fileIndex;
            }
        }

        private void InsertStaticResources(int fileIndex, XamlFileObject fileObject)
        {
            foreach(XmlNode node in fileObject.StaticResources)
            {
                string key = XamlUtils.GetKeyName(node);
                int index = AllStaticResources.Count;
                AllStaticResources.Add(node);
                staticResourceFileMap[index] = fileIndex;
                staticResourceKeyMap[index] = key;
            }
        }

        private void InsertThemeResources(int fileIndex, XamlFileObject fileObject)
        {
            foreach(Theme mode in Enum.GetValues(typeof(Theme)))
            {
                if (mode == Theme.Null)
                    continue;

                foreach(XmlNode node in fileObject.ThemeResources[(int)mode])
                {
                    string key = XamlUtils.GetKeyName(node);
                    int index = AllThemeResourcesList[(int)mode].Count;
                    AllThemeResourcesList[(int)mode].Add(node);
                    themeResourceFileMaps[(int)mode][index] = fileIndex;
                    themeResourceKeyMaps[(int)mode][index] = key;
                }
            }
        }

        private void FlagStyleResources()
        {
            foreach (XamlFileObject fileObject in xamlFileObjectMap.Values)
            {
                foreach (XmlNode style in fileObject.StyleResourcesDict.Keys)
                {
                    List<string> styleStaticResources = fileObject.StyleResourcesDict[style][(int)Ext.StaticResource];
                    List<string> styleThemeResources = fileObject.StyleResourcesDict[style][(int)Ext.ThemeResource];
                
                    // Searching style used StaticResources in AllStaticResources
                    if(styleStaticResources is not null)
                    {
                        foreach(string resource in styleStaticResources)
                        {
                            for(int i=0;i<AllStaticResources.Count;i++)
                            {
                                if (AllUsedStaticResources.Contains(i))
                                    continue;

                                if (string.Equals(staticResourceKeyMap[i], resource, StringComparison.OrdinalIgnoreCase))
                                {
                                    AllUsedStaticResources.Add(i);
                                    break;
                                }
                            }
                        }
                    }

                    // Searching style used StaticResources in AllStaticResources
                    if (styleThemeResources is not null)
                    {
                        foreach(string resource in styleThemeResources)
                        {
                            for(int i=0;i<AllStaticResources.Count;i++)
                            {
                                if (AllUsedStaticResources.Contains(i))
                                    continue;

                                if (string.Equals(staticResourceKeyMap[i], resource, StringComparison.OrdinalIgnoreCase))
                                {
                                    AllUsedStaticResources.Add(i);
                                    break;
                                }
                            }
                        }
                    }

                    // Searching style used ThemeResources in AllThemeResources
                    if (styleThemeResources is not null)
                    {
                        foreach(Theme mode in Enum.GetValues(typeof(Theme)))
                        {
                            if (mode == Theme.Null)
                                continue;

                            var themeResourceKeyMap = themeResourceKeyMaps[(int)mode];

                            foreach(string resource in styleThemeResources)
                            {
                                for(int i = 0; i < AllThemeResourcesList[(int)mode].Count; i++)
                                {
                                    if (AllUsedThemeResourcesList[(int)mode].Contains(i))
                                        continue;

                                    if (string.Equals(themeResourceKeyMap[i], resource, StringComparison.OrdinalIgnoreCase))
                                    {
                                        AllUsedThemeResourcesList[(int)mode].Add(i);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FlagStaticResourcesRecursively()
        {
            Queue<int> queue = new();

            for(int i=0;i< AllUsedStaticResources.Count;i++)
            {
                if (AllUsedStaticResources.Contains(i))
                    queue.Enqueue(i);
            }

            while(queue.Count>0)
            {
                int index = queue.Dequeue();
                XmlNode node = AllStaticResources[index];
                if(XamlUtils.TryGetUsedResource(node, out List<string> usedResources))
                {
                    foreach(string usedResource in usedResources)
                    {
                        for(int i=0;i<AllStaticResources.Count;i++)
                        {
                            if (AllUsedStaticResources.Contains(i))
                                continue;

                            if (string.Equals(staticResourceKeyMap[i], usedResource, StringComparison.OrdinalIgnoreCase))
                            {
                                AllUsedStaticResources.Add(i);
                                queue.Enqueue(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void FlagThemeResourcesRecursively()
        {
            foreach(Theme mode in Enum.GetValues(typeof(Theme)))
            {
                if (mode == Theme.Null)
                    continue;

                Queue<int> queue = new();

                for(int i=0; i < AllUsedThemeResourcesList[(int)mode].Count;i++)
                {
                    if (AllUsedThemeResourcesList[(int)mode].Contains(i))
                        queue.Enqueue(i);
                }

                while(queue.Count > 0)
                {
                    int index = queue.Dequeue();
                    XmlNode node = AllThemeResourcesList[(int)mode][index];
                    if(XamlUtils.TryGetUsedResource(node, out List<string> usedResources))
                    {
                        foreach(string resource in usedResources)
                        {
                            for(int i = 0; i < AllThemeResourcesList[(int)mode].Count;i++)
                            {
                                if (AllUsedThemeResourcesList[(int)mode].Contains(i))
                                    continue;

                                if (string.Equals(themeResourceKeyMaps[(int)mode][i], resource, StringComparison.OrdinalIgnoreCase))
                                {
                                    AllUsedThemeResourcesList[(int)mode].Add(i);
                                    queue.Enqueue(i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Members

        public List<string> XamlFiles { get; } = new();
        public Dictionary<int, XamlFileObject> xamlFileObjectMap = new();
        public Dictionary<XmlNode, int> styleFileMap = new();

        public List<XmlNode> AllStaticResources = new();
        public HashSet<int> AllUsedStaticResources = new();
        public Dictionary<int, int> staticResourceFileMap = new();
        public Dictionary<int, string> staticResourceKeyMap = new();
        
        public List<List<XmlNode>> AllThemeResourcesList 
            = ListUtils.Initialize<List<XmlNode>>(3);
        public List<HashSet<int>> AllUsedThemeResourcesList
            = ListUtils.Initialize<HashSet<int>>(3);
        public List<Dictionary<int, int>> themeResourceFileMaps 
            = ListUtils.Initialize<Dictionary<int, int>>(3);
        public List<Dictionary<int, string>> themeResourceKeyMaps
            = ListUtils.Initialize<Dictionary<int, string>>(3);

        #endregion
    }
}

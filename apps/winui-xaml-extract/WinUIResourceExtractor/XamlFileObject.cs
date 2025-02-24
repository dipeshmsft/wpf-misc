using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using ExtType = WinUIResourceExtractor.Enums.ExtensionType;
using Theme = WinUIResourceExtractor.Enums.ThemeMode;

namespace WinUIResourceExtractor
{
    internal class XamlFileObject
    {

        private XamlFileObject(string xamlFile)
        {
            _xamlFile = xamlFile;
            ProcessFileInternal();
        }

        #region Public Methods

        public static XamlFileObject Create(string xamlFile)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(xamlFile);
            XamlFileObject file = new(xamlFile);

            return file;
        }

        public bool TryGetResource()
        {
            return true;
        }

        public void Save(string outputDirectory, bool separateFiles = true)
        {
            XmlDocument usedDoc = XamlUtils.CreateEmptyXmlDocument(out List<XmlNode> usedDocColorNodes);
            XmlDocument unusedDoc = XamlUtils.CreateEmptyXmlDocument(out List<XmlNode> unusedDocColorNodes);

            foreach(Theme theme in Enum.GetValues(typeof(Theme)))
            {
                if (theme == Theme.Null)
                    continue;

                for(int i = 0; i < ThemeResources[(int)theme].Count;i++)
                {
                    if (UsedThemeResources[(int)theme][i])
                    {
                        XmlNode node = usedDoc.ImportNode(ThemeResources[(int)theme][i],true);
                        usedDocColorNodes[(int)theme].AppendChild(node);
                    }
                    else
                    {
                        XmlNode node = unusedDoc.ImportNode(ThemeResources[(int)theme][i], true);
                        unusedDocColorNodes[(int)theme].AppendChild(node);
                    }
                }
            }

            for (int i = 0; i < StaticResources.Count; i++)
            {
                if (UsedStaticResources[i])
                {
                    XmlNode node = usedDoc.ImportNode(StaticResources[i], true);
                    usedDoc.DocumentElement!.AppendChild(node);
                }
                else
                {
                    XmlNode node = unusedDoc.ImportNode(StaticResources[i], true);
                    unusedDoc.DocumentElement!.AppendChild(node);
                }
            }
        
            foreach(XmlNode style in Styles)
            {
                XmlNode node = usedDoc.ImportNode(style, true);
                usedDoc.DocumentElement!.AppendChild(node);
            }

            string baseFileName = Path.GetFileNameWithoutExtension(_xamlFile);
            if(separateFiles)
            {
                string usedDocPath = Path.Combine(outputDirectory, $"{baseFileName}.used.xaml");
                string unusedDocPath = Path.Combine(outputDirectory, $"{baseFileName}.unused.xaml");
                
                usedDoc.Save(usedDocPath);
                unusedDoc.Save(unusedDocPath);
            }
            else
            {
                string docPath = Path.Combine(outputDirectory, $"{baseFileName}.out.xaml");

                XmlNode node = usedDoc.ImportNode(unusedDoc.DocumentElement, true);
                XmlAttribute attribute = usedDoc.CreateAttribute("x:Key");
                attribute.Value = "UnusedResources";
                node.Attributes.Append(attribute);
                usedDoc.DocumentElement!.AppendChild(node);

                usedDoc.Save(docPath);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Reads a XAML file and extracts Styles, ThemeResources, StaticResources
        ///     Also extracts Static, Theme resources and TemplateBindings used in the Styles.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void ProcessFileInternal()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(_xamlFile, "_xamlFile is not set.");

            _xmlDoc = new();
            _xmlDoc.Load(_xamlFile);

            XmlElement root = _xmlDoc.DocumentElement 
                ?? throw new ArgumentException($"The XAML File : {_xamlFile} has no root element");

            foreach(XmlNode style in _xmlDoc.GetElementsByTagName("Style"))
            {
                var styleResources = ProcessStyle(style);
                Styles.Add(style);
                
                if(styleResources is not null)
                {
                    StyleResourcesDict[style] = styleResources;
                }
            }

            bool processedThemeDictionary = false;
            foreach(XmlNode node in root.ChildNodes)
            {
                if(node.Name.Contains("Style"))
                    continue;

                if(node.Name.Contains("ResourceDictionary.ThemeDictionaries"))
                {
                    Debug.Assert(processedThemeDictionary == false, 
                        $"There are more than one ThemeDictionaries in this file : {_xamlFile}");
                    ThemeResources = ProcessThemeDictionary(node);
                    processedThemeDictionary = true;
                }
                else
                {
                    StaticResources.Add(node);
                }
            }

            UsedStaticResources = ListUtils.Initialize<bool>(StaticResources.Count);
            UsedThemeResources[0] = ListUtils.Initialize<bool>(ThemeResources[0].Count);
            UsedThemeResources[1] = ListUtils.Initialize<bool>(ThemeResources[1].Count);
            UsedThemeResources[2] = ListUtils.Initialize<bool>(ThemeResources[2].Count);
        }

        private static List<List<XmlNode>> ProcessThemeDictionary(XmlNode themeDictionary)
        {
            List<List<XmlNode>> themeResources = ListUtils.NullInitialize<List<XmlNode>>(3);

            foreach (XmlNode child in themeDictionary)
            {
                Theme mode = XamlUtils.GetTheme(child);
                if (mode == Theme.Null)
                    continue;
                themeResources[(int)mode] = XamlUtils.AsList(child.ChildNodes);
            }

            return themeResources;
        }

        private static List<List<string>>? ProcessStyle(XmlNode style)
        {
            List< List<string> > styleResources = ListUtils.Initialize<List<string>>(3);

            if(style.SelectNodes("//@*") is not XmlNodeList attributes)
                return null;

            foreach(XmlAttribute attribute in attributes)
            {
                ExtType ext = XamlUtils.GetMarkupExtensionType(attribute.Value);

                if(ext != ExtType.Null)
                {
                    int start = attribute.Value.IndexOf(' ') + 1;
                    int length = attribute.Value.Length - (start + 1);
                    string val = attribute.Value.Substring(start, length);
                    styleResources[(int)ext].Add(val);
                }
            }

            return styleResources;
        }

        public void FlagStaticResource(XmlNode xmlNode)
        {
            for(int i=0;i<StaticResources.Count;i++)
            {
                if(xmlNode == StaticResources[i])
                {
                    UsedStaticResources[i] = true;
                    break;
                }
            }
        }

        public void FlagThemeResource(XmlNode xmlNode, Theme mode)
        {
            for(int i = 0; i < ThemeResources[(int)mode].Count; i++)
            {
                if(xmlNode == ThemeResources[(int)mode][i])
                {
                    UsedThemeResources[(int)mode][i] = true;
                    break;
                }
            }
        }

        #endregion

        #region Public Properties

        public string XamlFile 
        { 
            get => _xamlFile; 
            private set => _xamlFile = value;
        }

        public Dictionary<XmlNode, List<List<string>>> StyleResourcesDict { get; set; } = new();

        public List<XmlNode> Styles { get; set; } = new();

        public List<XmlNode> StaticResources { get; set; } = new();
        public List<List<XmlNode>> ThemeResources { get; private set; } = ListUtils.Initialize<List<XmlNode>>(3);

        public List<bool> UsedStaticResources { get; private set;  } = new();
        public List<List<bool>> UsedThemeResources { get; private set; } = ListUtils.Initialize<List<bool>>(3);

        #endregion

        #region Private Members

        private string _xamlFile;
        private XmlDocument? _xmlDoc;

        #endregion
    }
}

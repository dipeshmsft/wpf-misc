using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using ExtType = WinUIResourceExtractor.Enums.ExtensionType;
using Theme = WinUIResourceExtractor.Enums.ThemeMode;


namespace WinUIResourceExtractor
{
    public static class XamlUtils
    {
        public static List<XmlNode> AsList(XmlNodeList nodeList)
        {
            ArgumentNullException.ThrowIfNull(nodeList);
            List<XmlNode> list = new();

            foreach(XmlNode node in nodeList)
            {
                list.Add(node);
            }

            return list;
        }

        public static string GetKeyName(XmlNode node)
        {
            XmlAttributeCollection? attrs = node.Attributes;
            if (attrs is null)
                return "";

            foreach (XmlAttribute attr in attrs)
            {
                if (attr.Name.Contains("Key"))
                {
                    return attr.Value;
                }
            }

            return "";
        }

        public static Theme GetTheme(XmlNode node)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name.Contains("Key"))
                {
                    switch (attr.Value)
                    {
                        case "Default":
                            return Theme.Dark;
                        case "Light":
                            return Theme.Light;
                        case "HighContrast":
                            return Theme.HighContrast;
                    }
                }
            }

            return Theme.Null;
        }

        public static ExtType GetMarkupExtensionType(ReadOnlySpan<char> value)
        {

            if (value.Length == 0)
                return ExtType.Null;

            if (value.StartsWith("{"))
            {

                if (value.StartsWith("{ThemeResource", StringComparison.OrdinalIgnoreCase))
                {
                    return ExtType.ThemeResource;
                }

                if (value.StartsWith("{StaticResource", StringComparison.OrdinalIgnoreCase))
                {
                    return ExtType.StaticResource;
                }

                if (value.Contains("TemplateBinding", StringComparison.OrdinalIgnoreCase))
                {
                    return ExtType.TemplateBinding;
                }

            }

            return ExtType.Null;
        }
        
        public static bool TryGetUsedResource(XmlNode node, out List<string> usedResources)
        {
            usedResources = new List<string>();

            if (node.Name.Contains("StaticResource"))
            {
                XmlAttributeCollection? attrs = node.Attributes;
                if (attrs is not null)
                {
                    foreach (XmlAttribute attr in attrs)
                    {
                        if (attr.Name.Contains("ResourceKey"))
                        {
                            usedResources.Add(attr.Value);
                        }
                    }
                }
            }
            else
            {
                XmlAttributeCollection? attrs = node.Attributes;
                if (attrs is not null)
                {
                    foreach (XmlAttribute attr in attrs)
                    {
                        ExtType ext = XamlUtils.GetMarkupExtensionType(attr.Value);
                        if (ext is ExtType.Null)
                            continue;

                        if (ext is not ExtType.TemplateBinding)
                        {
                            string val = attr.Value.Substring(attr.Value.IndexOf(' ') + 1);
                            val = val.Substring(0, val.Length - 1);
                            usedResources.Add(val);
                        }
                    }
                }
            }

            return usedResources.Count != 0;
        }

        public static XmlDocument CreateEmptyXmlDocument(out List<XmlNode> colorDictionaryNodes)
        {
            XmlDocument doc = new();
            doc.LoadXml(RootElement);

            XmlNode themeDictionaryNode = doc.CreateElement("ResourceDictionary.ThemeDictionaries");
            colorDictionaryNodes = new();
            
            foreach(Theme theme in Enum.GetValues(typeof(Theme)))
            {
                if (theme == Theme.Null)
                    continue;

                XmlElement node = doc.CreateElement("ResourceDictionary");
                XmlAttribute attribute = doc.CreateAttribute("x:Key");
                attribute.Value = theme == Theme.Dark ? "Default" : theme.ToString();
                node.Attributes.Append(attribute);
                themeDictionaryNode.AppendChild(node);
                colorDictionaryNodes.Add(node);
            }

            doc.DocumentElement!.AppendChild(themeDictionaryNode);
            return doc;
        }

        public const string RootElement = "<ResourceDictionary xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></ResourceDictionary>";
    }

    public static class ListUtils
    {
        public static List<T> Initialize<T>(int capacity) where T : new()
        {
            List<T> list = new(capacity);
            for(int i=0;i<capacity;i++)
            {
                T t = new T();
                list.Add(t);
            }

            return list;
        }

        public static List<T> NullInitialize<T>(int capacity) where T : class
        {
            List<T> list = new(capacity);
            for(int i=0;i<capacity;i++)
            {
                list.Add(null!);
            }

            return list;
        }
    }
}

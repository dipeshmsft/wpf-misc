using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUIResourceExtractor
{
    public static class Enums
    {
        public enum ThemeMode
        {
            Light = 0,
            Dark = 1,
            HighContrast = 2,
            Null = 3
        }
        public enum ExtensionType
        {
            ThemeResource = 0,
            StaticResource = 1,
            TemplateBinding = 2,
            Null = 3
        }
    }
}

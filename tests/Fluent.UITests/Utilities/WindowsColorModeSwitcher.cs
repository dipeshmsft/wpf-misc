using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.UITests.TestUtilities
{
    //public static class WindowsSettingManager
    //{
    //    public static void SetColorMode(bool isLightMode)
    //    {
    //        using (var key = Registry.CurrentUser.OpenSubKey(PersonalizeKey, writable: true))
    //        {
    //            if (key == null)
    //                throw new InvalidOperationException("Could not open registry key for personalization settings.");

    //            // Set color mode for both apps and system
    //            key.SetValue("AppsUseLightTheme", isLightMode ? 1 : 0, RegistryValueKind.DWord);
    //            key.SetValue("SystemUsesLightTheme", isLightMode ? 1 : 0, RegistryValueKind.DWord);

    //            // Notify system about the changes
    //            NotifySystemColorModeChange();
    //        }
    //    }

    //    public static void SetAccentColor(uint argbColor)
    //    {
    //        // Update registry for AccentColor
    //        using (var key = Registry.CurrentUser.OpenSubKey(AccentColorKey, writable: true))
    //        {
    //            if (key == null)
    //                throw new InvalidOperationException("Could not open registry key for AccentColor.");

    //            key.SetValue("AccentColorMenu", argbColor, RegistryValueKind.DWord);
    //        }

    //        // Enable color prevalence
    //        using (var key = Registry.CurrentUser.OpenSubKey(PersonalizeKey, writable: true))
    //        {
    //            if (key == null)
    //                throw new InvalidOperationException("Could not open registry key for Personalize settings.");

    //            key.SetValue("ColorPrevalence", 1, RegistryValueKind.DWord);
    //        }

    //        // Notify the system of the change
    //        NotifyAccentColorChange();
    //    }

    //    public static void SetHighContrastMode(bool enable)
    //    {
    //        using (var key = Registry.CurrentUser.OpenSubKey(HighContrastKey, writable: true))
    //        {
    //            if (key == null)
    //                throw new InvalidOperationException("Could not open registry key for High Contrast settings.");

    //            key.SetValue("Flags", enable ? "1" : "0", RegistryValueKind.String);

    //            // Notify the system of the change
    //            NotifySettingsChange();
    //        }
    //    }

    //    public static bool IsHighContrastModeEnabled()
    //    {
    //        using (var key = Registry.CurrentUser.OpenSubKey(HighContrastKey, writable: false))
    //        {
    //            if (key == null)
    //                return false;

    //            var value = key.GetValue("Flags", "0").ToString();
    //            return value == "1";
    //        }
    //    }

    //    public static bool IsSystemColorModeLight()
    //    {
    //        var useLightTheme = Registry.GetValue(PersonalizeKey,
    //            "AppsUseLightTheme", null) as int?;

    //        if (useLightTheme == null)
    //        {
    //            useLightTheme = Registry.GetValue(PersonalizeKey,
    //                "SystemUsesLightTheme", null) as int?;
    //        }

    //        return useLightTheme != null && useLightTheme != 0;
    //    }

    //    private static void NotifySystemColorModeChange()
    //    {
    //        // This sends a broadcast message to update the theme
    //        const int HWND_BROADCAST = 0xffff;
    //        const int WM_SETTINGCHANGE = 0x1a;

    //        NativeMethods.SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "ImmersiveColorSet".ToIntPtr());
    //    }

    //    private static void NotifyAccentColorChange()
    //    {
    //        const int HWND_BROADCAST = 0xffff;
    //        const int WM_SETTINGCHANGE = 0x1a;

    //        NativeMethods.SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "ImmersiveColorSet".ToIntPtr());
    //    }

    //    private static void NotifySettingsChange()
    //    {
    //        const int HWND_BROADCAST = 0xffff;
    //        const int WM_SETTINGCHANGE = 0x1a;

    //        NativeMethods.SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "Accessibility".ToIntPtr());
    //    }

    //    private const string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    //    private const string AccentColorKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent";
    //    private const string HighContrastKey = @"Control Panel\Accessibility\HighContrast";
    //}

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SendMessage(int hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}

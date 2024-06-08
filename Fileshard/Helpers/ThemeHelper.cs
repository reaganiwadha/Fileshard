using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Fileshard.Frontend.Helpers
{
    public static class ThemeHelper
    {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
        const int DWWMA_CAPTION_COLOR = 35;


        public static void EnsureTitleBar(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).EnsureHandle();
            int[] colorstr = new int[] { 0x000000 };
            DwmSetWindowAttribute(hWnd, DWWMA_CAPTION_COLOR, colorstr, 4);
        }

        public static bool IsDarkTheme()
        {
            const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string RegistryValueName = "AppsUseLightTheme";

            using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (key != null)
                {
                    var registryValueObject = key.GetValue(RegistryValueName);
                    if (registryValueObject != null)
                    {
                        return (int)registryValueObject == 0;
                    }
                }
            }
            return false; // Default to light theme
        }
    }

}
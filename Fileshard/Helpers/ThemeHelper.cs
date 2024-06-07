using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Fileshard.Frontend.Helpers
{
    public static class ThemeHelper
    {
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
using System.Reflection;
using Microsoft.Win32;

namespace Helper
{
    public static class AutoStart
    {
        public static void Set(bool toggle,string name,string location)
        {
            if (toggle)
            {
                var key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (key != null) key.SetValue(name, location);
            }
            else
            {
                var key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (key != null && key.GetValue(name) != null) key.DeleteValue(name);
            }
        }

        public static bool Get(string name)
        {
            var key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

            return key != null && key.GetValue(name) != null;
        }
    }
}

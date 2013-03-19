using Microsoft.Win32;

namespace Helper {
  public static class AutoStart {
    public static void Set(bool toggle, string name, string location) {
      if (toggle) {
        RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
        if (key != null) key.SetValue(name, location);
      }
      else {
        RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
        if (key != null && key.GetValue(name) != null) key.DeleteValue(name);
      }
    }

    public static bool Get(string name) {
      RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

      return key != null && key.GetValue(name) != null;
    }
  }
}
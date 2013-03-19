using System;
using System.Windows.Forms;

namespace StreamNotifier {
  public partial class OAuthForm : Form {
    public string AccessToken;

    public OAuthForm(string url) {
      InitializeComponent();

      webBrowser.ScriptErrorsSuppressed = true;
      webBrowser.ScrollBarsEnabled = false;
      webBrowser.Navigating += webBrowser_Navigating;
      webBrowser.Navigate(url);
    }

    private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
      if (!e.Url.AbsoluteUri.Contains("#access_token=")) return;

      string[] x = e.Url.AbsoluteUri.Split(new[] {"#access_token="}, StringSplitOptions.None);
      AccessToken = x[1].Split(new[] {'&'})[0];
      DialogResult = DialogResult.OK;
      Close();
    }
  }
}
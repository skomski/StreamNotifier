using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Helper.Extensions;
using NLog;
using Newtonsoft.Json;
using StreamNotifier.Properties;

namespace StreamNotifier {
  internal class MainHandler {
    private const string ClientId = "n2cq188sa6pgyzyrljef3jq5oqyqp88";
    private const string RedirectUri = "http://google.com";
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private static bool _running;
    private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
    private readonly List<LiveStream> _channels = new List<LiveStream>();


    private readonly NotifyIcon _trayIcon = new NotifyIcon {
      Icon = Resources.tray_icon_icon,
      Text = Application.ProductName,
      Visible = true
    };

    private readonly Timer _updateTimer = new Timer {
      Interval = (int) Settings.Default.UpdateInterval.ToMinutes().TotalMilliseconds
    };

    private MainHandler() {
      var contextMenu = new ContextMenuStrip {ShowImageMargin = false};

      var closeMenuItem = new ToolStripButton(Resources.exitMessage) {
        Image = Resources.exit,
        Tag = true,
        AutoToolTip = false
      };
      closeMenuItem.Click += (s, a) => Application.Exit();

      var settingsMenuItem = new ToolStripButton(Resources.settingsMessage) {
        Image = Resources.settings,
        Tag = true,
        AutoToolTip = false
      };
      settingsMenuItem.Click += SettingsMenuItemClick;

      var updateMenuItem = new ToolStripButton(Resources.updateMessage) {
        Image = Resources.update,
        Tag = true,
        AutoToolTip = false
      };
      updateMenuItem.Click += (s, a) => _backgroundWorker.RunWorker();

      var separatorMenuItem = new ToolStripSeparator {Tag = true};

      contextMenu.Items.AddRange(new ToolStripItem[]
      {separatorMenuItem, updateMenuItem, settingsMenuItem, closeMenuItem});
      _trayIcon.ContextMenuStrip = contextMenu;

      _updateTimer.Tick += (s, a) => _backgroundWorker.RunWorker();
      _updateTimer.Start();

      _backgroundWorker.DoWork += (s, a) => {
        _trayIcon.Icon = Resources.sync_icon;
        try {
          PopulateChannels();
        }
        catch (WebException e) {
          var res = (HttpWebResponse) e.Response;
          if (res.StatusCode == HttpStatusCode.Unauthorized) {
            a.Result = false;
            return;
          }
          MessageBox.Show("Cannot retrieve twitch followers", "Error");
          a.Result = true;
        }
        a.Result = true;
      };

      _backgroundWorker.RunWorkerCompleted += (s, a) => {
        _trayIcon.Icon = Resources.tray_icon_icon;
        var result = (bool) a.Result;
        if (result == false) {
          Authenticate();
          _backgroundWorker.RunWorker();
        }
        else {
          UpdateStreamsUI();
        }
      };

      _backgroundWorker.RunWorker();
    }

    public static void Run() {
      if (_running) {
        throw new NotSupportedException("More then one MainHandler instance!");
      }

      new MainHandler();
      _running = true;
    }

    private void PopulateChannels() {
      var client = new WebClient();
      string response;


      response = client.DownloadString(
        new Uri(String.Format("https://api.twitch.tv/kraken/streams/followed?limit=100&oauth_token={0}",
                              Settings.Default.TwitchTVAccessToken)));


      var jsonRoot = JsonConvert.DeserializeObject<JsonStreamsRootObject>(response);

      _channels.Clear();
      foreach (Stream follow in jsonRoot.streams) {
        _channels.Add(new TwitchTVStream(follow.channel.name, follow.viewers, follow.channel.status, follow.channel.url));
      }
    }

    private void SettingsMenuItemClick(object sender, EventArgs e) {
      using (var settingsForm = new SettingsForm()) {
        settingsForm.ShowDialog();
      }

      _updateTimer.Interval = (int) Settings.Default.UpdateInterval.ToMinutes().TotalMilliseconds;

      _backgroundWorker.RunWorker();
    }

    private void UpdateStreamsUI() {
      var newStreamMessage = new StringBuilder();

      foreach (LiveStream stream in _channels) {
        if (stream.IsLive && !_trayIcon.ContextMenuStrip.Items.ContainsKey(stream.Identifier)) {
          var streamMenuItem = new ToolStripButton(stream.Name) {
            Name = stream.Identifier,
            Image = Resources.online,
            ToolTipText = stream.EventDescription
          };
          string streamName = stream.Name;
          streamMenuItem.Click += (s, a) => ViewStream(streamName);
          _trayIcon.ContextMenuStrip.Items.Insert(0, streamMenuItem);

          newStreamMessage.AppendLine(stream.Name + " - " + stream.EventDescription + " - " + stream.Viewer);
        }
        else if (stream.IsLive == false && _trayIcon.ContextMenuStrip.Items.ContainsKey(stream.Identifier)) {
          _trayIcon.ContextMenuStrip.Items.RemoveByKey(stream.Identifier);
        }
      }

      for (int index = 0; index < _trayIcon.ContextMenuStrip.Items.Count; index++) {
        ToolStripItem item = _trayIcon.ContextMenuStrip.Items[index];
        if (_channels.Exists(stream => stream.Identifier == item.Name) == false &&
            item.Tag == null) {
          _trayIcon.ContextMenuStrip.Items.Remove(item);
        }
      }

      _trayIcon.Text = _channels.Count(stream => stream.IsLive) + Resources.MainHandler_UpdateStreamsUI__streams_online_;

      if (newStreamMessage.Length > 0) {
        new SoundPlayer(Resources.ringin).Play();
        _trayIcon.ShowBalloonTip(5000, "New stream(s) online!", newStreamMessage.ToString(), ToolTipIcon.Info);
      }
    }

    public static string EscapeArguments(params string[] args)
    {
      StringBuilder arguments = new StringBuilder();
      Regex invalidChar = new Regex("[\x00\x0a\x0d]");//  these can not be escaped
      Regex needsQuotes = new Regex(@"\s|""");//          contains whitespace or two quote characters
      Regex escapeQuote = new Regex(@"(\\*)(""|$)");//    one or more '\' followed with a quote or end of string
      for (int carg = 0; args != null && carg < args.Length; carg++)
      {
        if (args[carg] == null) { throw new ArgumentNullException("args[" + carg + "]"); }
        if (invalidChar.IsMatch(args[carg])) { throw new ArgumentOutOfRangeException("args[" + carg + "]"); }
        if (args[carg] == String.Empty) { arguments.Append("\"\""); }
        else if (!needsQuotes.IsMatch(args[carg])) { arguments.Append(args[carg]); }
        else
        {
          arguments.Append('"');
          arguments.Append(escapeQuote.Replace(args[carg], m =>
          m.Groups[1].Value + m.Groups[1].Value +
          (m.Groups[2].Value == "\"" ? "\\\"" : "")
          ));
          arguments.Append('"');
        }
        if (carg + 1 < args.Length)
          arguments.Append(' ');
      }
      return arguments.ToString();
    }

    private void ViewStream(string name) {
      var client = new WebClient();
      string response;


      response = client.DownloadString(
        new Uri(String.Format("http://usher.twitch.tv/find/{0}.json?type=any&p=0",
                              name.ToLower())));


      var jsonRoot = JsonConvert.DeserializeObject<StreamSource[]>(response);

      var sources = jsonRoot.Where(t => t.type == "live").ToArray();

      string swfVfy =
        String.Format(
          "http://www-cdn.jtvnw.net/widgets/live_embed_player.r063553554ff850316ad413fafd5d0783c46f587b.swf?channel={0}&referer=&userAgent=&channel={0}",
          name.ToLower());
      var commandLine =
        String.Format(
          "/c C:\\rtmpdump.exe -r \"{0}/{1}\" --swfVfy \"{2}\" -j {3} -v -o - | \"C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe\" - --play-and-exit",
          sources[0].connect, sources[0].play, swfVfy, EscapeArguments(sources[0].token)
          );
      var process = new Process();
      var startInfo = new ProcessStartInfo {
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = "cmd.exe",
        Arguments = commandLine
      };
      process.StartInfo = startInfo;
      process.Start();
    }

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(_updateTimer != null);
      Contract.Invariant(_backgroundWorker != null);
    }

    internal static void Authenticate() {
      var authDialog = new OAuthForm(
        String.Format(
          "https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri={1}", ClientId,
          RedirectUri));
      if (authDialog.ShowDialog() == DialogResult.OK) {
        Settings.Default.TwitchTVAccessToken = authDialog.AccessToken;
        Settings.Default.Save();
      }
      else {
        MessageBox.Show("Could not authenticate your twitch account", "Error");
      }
    }

    public class StreamSource {
      public string node { get; set; }
      public string needed_info { get; set; }
      public string play { get; set; }
      public string meta_game { get; set; }
      public int video_height { get; set; }
      public double bitrate { get; set; }
      public int broadcast_part { get; set; }
      public string persistent { get; set; }
      public string cluster { get; set; }
      public string token { get; set; }
      public string connect { get; set; }
      public object broadcast_id { get; set; }
      public string type { get; set; }
      public string display { get; set; }
    }


    public class StreamSources {
      public List<StreamSource> sources { get; set; }
    }

    public class Channel {
      public string display_name { get; set; }
      public List<object> teams { get; set; }
      public string status { get; set; }
      public string created_at { get; set; }
      public string updated_at { get; set; }
      public bool? mature { get; set; }
      public int _id { get; set; }
      public string background { get; set; }
      public string banner { get; set; }
      public string logo { get; set; }
      public string name { get; set; }
      public string url { get; set; }
      public string video_banner { get; set; }
      public string game { get; set; }
    }

    public class JsonStreamsRootObject {
      public List<Stream> streams { get; set; }
    }

    public class Stream {
      public string broadcaster { get; set; }
      public object _id { get; set; }
      public int viewers { get; set; }
      public Channel channel { get; set; }
      public string name { get; set; }
      public string game { get; set; }
    }
  }
}
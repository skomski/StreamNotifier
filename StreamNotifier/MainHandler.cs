using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Helper.Extensions;
using NLog;
using Newtonsoft.Json;
using Prowlin;
using StreamNotifier.Properties;

namespace StreamNotifier {
  internal class MainHandler {
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
        PopulateChannels();
        _channels.ForEach(stream => stream.Update());
        _trayIcon.Icon = Resources.tray_icon_icon;
      };

    _backgroundWorker.RunWorkerCompleted += (s, a) => UpdateStreamsUI();

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

      try {
        response = client.DownloadString(
          new Uri(String.Format("https://api.twitch.tv/kraken/users/{0}/follows/channels?limit=100",
                        Settings.Default.twitchName)));
      }
      catch (WebException e) {
        MessageBox.Show("Cannot retrieve twitch followers for name: " + Settings.Default.twitchName, "Error");
        return;
      }

      var jsonRoot = JsonConvert.DeserializeObject<JsonRootObject>(response);

      foreach (var follow in jsonRoot.follows.Where(follow => !_channels.Exists(t => t.Name == follow.channel.name))) {
        _channels.Add(new TwitchTVStream(follow.channel.name));
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

      foreach (var stream in _channels) {
        if (stream.IsLive && stream.IsShowed == false) {
          var streamMenuItem = new ToolStripButton(stream.Name) {
            Name = stream.Identifier,
            Image = Resources.online,
            ToolTipText = stream.EventDescription
          };
          var streamUrl = stream.Url.AbsoluteUri;
          streamMenuItem.Click += (s, a) => Process.Start(streamUrl);
          _trayIcon.ContextMenuStrip.Items.Insert(0, streamMenuItem);

          stream.IsShowed = true;

          newStreamMessage.AppendLine(stream.Name + " - " + stream.EventDescription + " - " + stream.Viewer);
        }
        else if (stream.IsLive == false && stream.IsShowed) {
          stream.IsShowed = false;
          _trayIcon.ContextMenuStrip.Items.RemoveByKey(stream.Identifier);
        }
      }

      for (int index = 0; index < _trayIcon.ContextMenuStrip.Items.Count; index++) {
        var item = _trayIcon.ContextMenuStrip.Items[index];
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

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(_updateTimer != null);
      Contract.Invariant(_backgroundWorker != null);
    }

    public class JsonRootObject {
      public List<Follow> follows { get; set; }

      public class Follow {
        public Channel channel { get; set; }

        public class Channel {
          public string name { get; set; }
        }
      }
    }
  }
}
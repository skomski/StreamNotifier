using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using Helper.Extensions;
using NLog;
using Prowlin;
using StreamNotifier.Properties;

namespace StreamNotifier
{
    internal class MainHandler
    {
        static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();

        private readonly NotifyIcon _trayIcon = new NotifyIcon { Icon = Resources.tray_icon_icon, Text = Application.ProductName, Visible = true };
        private readonly Timer _updateTimer = new Timer {Interval = (int) Settings.Default.UpdateInterval.ToMinutes().TotalMilliseconds};

        private readonly ProwlClient _prowlClient = new ProwlClient();

        private static bool _running;

        public static void Run()
        {
            if (_running)
            {
                throw new NotSupportedException("More then one MainHandler instance!");
            }

            new MainHandler();
            _running = true;
     
        }
        private MainHandler()
        {
            var contextMenu = new ContextMenuStrip{ShowImageMargin = false};

            var closeMenuItem = new ToolStripButton(Resources.exitMessage) { Image = Resources.exit, Tag = true,AutoToolTip = false};
            closeMenuItem.Click += (s,a) => Application.Exit();

            var settingsMenuItem = new ToolStripButton(Resources.settingsMessage) { Image = Resources.settings, Tag = true ,AutoToolTip = false};
            settingsMenuItem.Click += SettingsMenuItemClick;

            var updateMenuItem = new ToolStripButton(Resources.updateMessage) { Image = Resources.update,Tag = true ,AutoToolTip = false};
            updateMenuItem.Click += (s,a) => _backgroundWorker.RunWorker();

            var clipboardMenuItem = new ToolStripButton(Resources.newStreamFromCLMessage) {Image = Resources.clipboard, Tag = true ,AutoToolTip = false,ToolTipText = Resources.SupportedStreamsTT};
            clipboardMenuItem.Click += ClipboardMenuItemClick;

            var separatorMenuItem = new ToolStripSeparator {Tag = true};

            contextMenu.Items.AddRange(new ToolStripItem[]{separatorMenuItem,clipboardMenuItem,updateMenuItem,settingsMenuItem,closeMenuItem});
            _trayIcon.ContextMenuStrip = contextMenu;

            _updateTimer.Tick += (s,a) => _backgroundWorker.RunWorker();
            _updateTimer.Start();

            _backgroundWorker.DoWork += (s, a) => Settings.Default.Streams.ForEach(stream => stream.Update());
            _backgroundWorker.RunWorkerCompleted += (s, a) => UpdateStreamsUI();

            if (Settings.Default.Streams == null)
            {
                Settings.Default.Streams = new List<LiveStream>{new LiveStream("TakeTV","3635",LiveStream.StreamVendor.Own3dTV)};
            }

            _backgroundWorker.RunWorker();
        }
        private void ClipboardMenuItemClick(object sender, EventArgs e)
        {
            var iData = Clipboard.GetDataObject();
            if (iData == null || !iData.GetDataPresent(DataFormats.Text))
            {
                MessageBox.Show(Resources.MainHandler_ClipboardMenuItemClick_No_text_found_in_clipboard_);
                return;
            }
            var clipboardString = (String)iData.GetData(DataFormats.Text);

            if (!Uri.IsWellFormedUriString(clipboardString,UriKind.Absolute))
            {
                MessageBox.Show(Resources.MainHandler_ClipboardMenuItemClick_No_URL_found_in_clipboard_);
                return;
            }

            try
            {
                var liveStream = new LiveStream(clipboardString);

                if (!Settings.Default.Streams.Exists(s => s.Identifier == liveStream.Identifier))
                {
                    Settings.Default.Streams.Add(liveStream);
                }
                else
                {
                    MessageBox.Show(Resources.alreadyCapturedMessage, Resources.exitMessage);
                }

                _backgroundWorker.RunWorker();

                Settings.Default.Save();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show(Resources.cantRecognizeStreamMessage,Resources.parseErrorTitle);
                _logger.Error("False URL: " + clipboardString);
            }
        }
        private void SettingsMenuItemClick(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }

            _updateTimer.Interval = (int)Settings.Default.UpdateInterval.ToMinutes().TotalMilliseconds;

            _backgroundWorker.RunWorker();
        }
        private void UpdateStreamsUI()
        {
            var newStreamMessage = new StringBuilder();

            foreach (var stream in Settings.Default.Streams)
            {
                if (stream.IsLive && stream.IsShowed == false)
                {
                    var streamMenuItem = new ToolStripButton(stream.Name) {Name = stream.Identifier, Image = Resources.online,ToolTipText = stream.EventDescription};
                    var streamURL = stream.URL.AbsoluteUri;
                    streamMenuItem.Click += (s, a) => Process.Start(streamURL);
                    _trayIcon.ContextMenuStrip.Items.Insert(0, streamMenuItem);

                    stream.IsShowed = true;

                    newStreamMessage.AppendLine(stream.Name + " - " + stream.EventDescription + " - " + stream.Viewer);

                    if(Settings.Default.ProwlAPIKey.NotEmpty())
                    {
                        _prowlClient.SendNotification(stream.Name + Resources.onlineMessage, stream.EventDescription,stream.URL.AbsoluteUri,Settings.Default.ProwlAPIKey);
                    }
                }
                else if (stream.IsLive == false && stream.IsShowed)
                {
                    stream.IsShowed = false;
                    _trayIcon.ContextMenuStrip.Items.RemoveByKey(stream.Identifier);
                }
            }

            for (var index = 0; index < _trayIcon.ContextMenuStrip.Items.Count; index++)
            {
                var item = _trayIcon.ContextMenuStrip.Items[index];
                if (Settings.Default.Streams.Exists(stream => stream.Identifier == item.Name) == false &&
                    item.Tag == null)
                {
                    _trayIcon.ContextMenuStrip.Items.Remove(item);
                }
            }

            _trayIcon.Text = Settings.Default.Streams.Count(stream => stream.IsLive) + Resources.MainHandler_UpdateStreamsUI__streams_online_;

            if(newStreamMessage.Length > 0)
            {
                new SoundPlayer(Resources.ringin).Play();
                _trayIcon.ShowBalloonTip(5000,"New stream(s) online!",newStreamMessage.ToString(),ToolTipIcon.Info);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_prowlClient != null);
            Contract.Invariant(_updateTimer != null);
            Contract.Invariant(_backgroundWorker != null);
        }
    }
}
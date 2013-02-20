using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Net;
using System.Windows.Forms;
using Helper;
using Helper.Extensions;
using NLog;
using StreamNotifier.Properties;

namespace StreamNotifier
{
    public partial class SettingsForm : Form
    {
        static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SettingsForm()
        {
            InitializeComponent();

            UpdateIntervalUpDown.Value = Settings.Default.UpdateInterval;
            twitchNameTextBox.Text = Settings.Default.twitchName; 
            AutoStartCheckBox.Checked = AutoStart.Get(Application.ProductName);

            var toolTip = new ToolTip();

            toolTip.SetToolTip(UpdateIntervalUpDown, "Update interval in minutes");
        }

        private void UpdateIntervalSliderValueChanged(object sender, EventArgs e)
        {
            Settings.Default.UpdateInterval = (int) UpdateIntervalUpDown.Value;
        }
        private void SettingsFormFormClosing(object sender, FormClosingEventArgs e) {
          Settings.Default.twitchName = twitchNameTextBox.Text;
          try {
            var client = new WebClient();
            string response = client.DownloadString(new Uri(String.Format("https://api.twitch.tv/kraken/users/{0}/follows/channels?limit=100", Settings.Default.twitchName)));
          }
          catch (WebException) {
            MessageBox.Show("Error", "Unknown TwitchTV name");
            return;
          }

          Settings.Default.Save();
        }
        private void AutostartCheckboxClick(object sender, EventArgs e)
        {
            AutoStart.Set(AutoStartCheckBox.Checked, Application.ProductName, Application.ExecutablePath);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(UpdateIntervalUpDown != null);
            Contract.Invariant(AutoStartCheckBox != null);
        }
    }
}

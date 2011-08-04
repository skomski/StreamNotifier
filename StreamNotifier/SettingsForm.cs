using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
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
            AutoStartCheckBox.Checked = AutoStart.Get(Application.ProductName);
            ProwlAPIKeyTextBox.Text = Settings.Default.ProwlAPIKey;

            var bindingList = new BindingList<LiveStream>(Settings.Default.Streams) {AllowRemove = true,AllowEdit = true,AllowNew = false};
            StreamGridView.DataSource = bindingList;

            ((DataGridViewComboBoxColumn)StreamGridView.Columns[2]).DataSource = Enum.GetValues(typeof(LiveStream.StreamVendor));
            StreamGridView.Columns[2].ValueType = typeof(LiveStream.StreamVendor);

            StreamGridView.DataError += (s, a) => { MessageBox.Show("Please enter something!"); _logger.Error(a.Exception); };

            var toolTip = new ToolTip();

            toolTip.SetToolTip(UpdateIntervalUpDown, "Update interval in minutes");
            toolTip.SetToolTip(ProwlAPIKeyTextBox, "Prowl APIKey");    
        }

        private void UpdateIntervalSliderValueChanged(object sender, EventArgs e)
        {
            Settings.Default.UpdateInterval = (int) UpdateIntervalUpDown.Value;
        }
        private void SettingsFormFormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Streams = new List<LiveStream>((BindingList<LiveStream>)StreamGridView.DataSource);
            Settings.Default.Save();
        }
        private void AutostartCheckboxClick(object sender, EventArgs e)
        {
            AutoStart.Set(AutoStartCheckBox.Checked, Application.ProductName, Application.ExecutablePath);
        }
        private void ProwlApikeyTextboxTextChanged(object sender, EventArgs e)
        {
            Settings.Default.ProwlAPIKey = ProwlAPIKeyTextBox.Text;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(UpdateIntervalUpDown != null);
            Contract.Invariant(StreamGridView != null);
            Contract.Invariant(AutoStartCheckBox != null);
            Contract.Invariant(ProwlAPIKeyTextBox != null);
        }
    }
}

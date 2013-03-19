using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using Helper;
using NLog;
using StreamNotifier.Properties;

namespace StreamNotifier {
  public partial class SettingsForm : Form {
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public SettingsForm() {
      InitializeComponent();

      UpdateIntervalUpDown.Value = Settings.Default.UpdateInterval;
      AutoStartCheckBox.Checked = AutoStart.Get(Application.ProductName);

      var toolTip = new ToolTip();

      toolTip.SetToolTip(UpdateIntervalUpDown, "Update interval in minutes");
    }

    private void UpdateIntervalSliderValueChanged(object sender, EventArgs e) {
      Settings.Default.UpdateInterval = (int) UpdateIntervalUpDown.Value;
    }

    private void SettingsFormFormClosing(object sender, FormClosingEventArgs e) {
    }

    private void AutostartCheckboxClick(object sender, EventArgs e) {
      AutoStart.Set(AutoStartCheckBox.Checked, Application.ProductName, Application.ExecutablePath);
    }

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(UpdateIntervalUpDown != null);
      Contract.Invariant(AutoStartCheckBox != null);
    }

    private void reauthButton_Click(object sender, EventArgs e) {
      MainHandler.Authenticate();
    }
  }
}
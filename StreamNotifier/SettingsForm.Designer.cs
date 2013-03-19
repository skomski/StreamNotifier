namespace StreamNotifier
{
    partial class SettingsForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
      this.UpdateIntervalUpDown = new System.Windows.Forms.NumericUpDown();
      this.AutoStartCheckBox = new System.Windows.Forms.CheckBox();
      this.updateIntervalLabel = new System.Windows.Forms.Label();
      this.reauthButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.UpdateIntervalUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // UpdateIntervalUpDown
      // 
      this.UpdateIntervalUpDown.Location = new System.Drawing.Point(98, 7);
      this.UpdateIntervalUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.UpdateIntervalUpDown.Name = "UpdateIntervalUpDown";
      this.UpdateIntervalUpDown.Size = new System.Drawing.Size(174, 20);
      this.UpdateIntervalUpDown.TabIndex = 3;
      this.UpdateIntervalUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.UpdateIntervalUpDown.ValueChanged += new System.EventHandler(this.UpdateIntervalSliderValueChanged);
      // 
      // AutoStartCheckBox
      // 
      this.AutoStartCheckBox.AutoSize = true;
      this.AutoStartCheckBox.Location = new System.Drawing.Point(155, 39);
      this.AutoStartCheckBox.Name = "AutoStartCheckBox";
      this.AutoStartCheckBox.Size = new System.Drawing.Size(117, 17);
      this.AutoStartCheckBox.TabIndex = 4;
      this.AutoStartCheckBox.Text = "Start with Windows";
      this.AutoStartCheckBox.UseVisualStyleBackColor = true;
      this.AutoStartCheckBox.Click += new System.EventHandler(this.AutostartCheckboxClick);
      // 
      // updateIntervalLabel
      // 
      this.updateIntervalLabel.AutoSize = true;
      this.updateIntervalLabel.Location = new System.Drawing.Point(12, 9);
      this.updateIntervalLabel.Name = "updateIntervalLabel";
      this.updateIntervalLabel.Size = new System.Drawing.Size(80, 13);
      this.updateIntervalLabel.TabIndex = 6;
      this.updateIntervalLabel.Text = "Update Interval";
      // 
      // reauthButton
      // 
      this.reauthButton.Location = new System.Drawing.Point(12, 33);
      this.reauthButton.Name = "reauthButton";
      this.reauthButton.Size = new System.Drawing.Size(133, 26);
      this.reauthButton.TabIndex = 7;
      this.reauthButton.Text = "Reauthenticate";
      this.reauthButton.UseVisualStyleBackColor = true;
      this.reauthButton.Click += new System.EventHandler(this.reauthButton_Click);
      // 
      // SettingsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 68);
      this.Controls.Add(this.reauthButton);
      this.Controls.Add(this.updateIntervalLabel);
      this.Controls.Add(this.AutoStartCheckBox);
      this.Controls.Add(this.UpdateIntervalUpDown);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.MinimumSize = new System.Drawing.Size(300, 50);
      this.Name = "SettingsForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "StreamNotifier";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.UpdateIntervalUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown UpdateIntervalUpDown;
        private System.Windows.Forms.CheckBox AutoStartCheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn identifierColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn vendorColumn;
        private System.Windows.Forms.Label updateIntervalLabel;
        private System.Windows.Forms.Button reauthButton;
    }
}


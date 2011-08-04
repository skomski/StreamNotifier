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
            this.components = new System.ComponentModel.Container();
            this.UpdateIntervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.AutoStartCheckBox = new System.Windows.Forms.CheckBox();
            this.ProwlAPIKeyTextBox = new System.Windows.Forms.TextBox();
            this.StreamGridView = new System.Windows.Forms.DataGridView();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.identifierColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vendorColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateIntervalUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StreamGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // update_interval_slider_
            // 
            this.UpdateIntervalUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateIntervalUpDown.Location = new System.Drawing.Point(135, 320);
            this.UpdateIntervalUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpdateIntervalUpDown.Name = "UpdateIntervalUpDown";
            this.UpdateIntervalUpDown.Size = new System.Drawing.Size(807, 20);
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
            this.AutoStartCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AutoStartCheckBox.AutoSize = true;
            this.AutoStartCheckBox.Location = new System.Drawing.Point(12, 323);
            this.AutoStartCheckBox.Name = "AutoStartCheckBox";
            this.AutoStartCheckBox.Size = new System.Drawing.Size(117, 17);
            this.AutoStartCheckBox.TabIndex = 4;
            this.AutoStartCheckBox.Text = "Start with Windows";
            this.AutoStartCheckBox.UseVisualStyleBackColor = true;
            this.AutoStartCheckBox.Click += new System.EventHandler(this.AutostartCheckboxClick);
            // 
            // ProwlAPIKeyTextBox
            // 
            this.ProwlAPIKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProwlAPIKeyTextBox.Location = new System.Drawing.Point(12, 346);
            this.ProwlAPIKeyTextBox.Name = "ProwlAPIKeyTextBox";
            this.ProwlAPIKeyTextBox.Size = new System.Drawing.Size(930, 20);
            this.ProwlAPIKeyTextBox.TabIndex = 5;
            this.ProwlAPIKeyTextBox.TextChanged += new System.EventHandler(this.ProwlApikeyTextboxTextChanged);
            // 
            // StreamGridView
            // 
            this.StreamGridView.AllowUserToOrderColumns = true;
            this.StreamGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StreamGridView.AutoGenerateColumns = false;
            this.StreamGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.StreamGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.StreamGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.StreamGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.StreamGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameColumn,
            this.identifierColumn,
            this.vendorColumn});
            this.StreamGridView.EnableHeadersVisualStyles = false;
            this.StreamGridView.Location = new System.Drawing.Point(12, 12);
            this.StreamGridView.MultiSelect = false;
            this.StreamGridView.Name = "StreamGridView";
            this.StreamGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.StreamGridView.ShowCellErrors = false;
            this.StreamGridView.ShowEditingIcon = false;
            this.StreamGridView.ShowRowErrors = false;
            this.StreamGridView.Size = new System.Drawing.Size(930, 303);
            this.StreamGridView.TabIndex = 7;
            // 
            // nameColumn
            // 
            this.nameColumn.DataPropertyName = "Name";
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.Name = "nameColumn";
            // 
            // identifierColumn
            // 
            this.identifierColumn.DataPropertyName = "Identifier";
            this.identifierColumn.HeaderText = "Identifier";
            this.identifierColumn.Name = "identifierColumn";
            // 
            // vendorColumn
            // 
            this.vendorColumn.DataPropertyName = "Vendor";
            this.vendorColumn.HeaderText = "Vendor";
            this.vendorColumn.Name = "vendorColumn";
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 375);
            this.Controls.Add(this.StreamGridView);
            this.Controls.Add(this.ProwlAPIKeyTextBox);
            this.Controls.Add(this.AutoStartCheckBox);
            this.Controls.Add(this.UpdateIntervalUpDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(300, 180);
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StreamNotifier";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.UpdateIntervalUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StreamGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown UpdateIntervalUpDown;
        private System.Windows.Forms.CheckBox AutoStartCheckBox;
        private System.Windows.Forms.TextBox ProwlAPIKeyTextBox;
        private System.Windows.Forms.DataGridView StreamGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn identifierColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn vendorColumn;
    }
}


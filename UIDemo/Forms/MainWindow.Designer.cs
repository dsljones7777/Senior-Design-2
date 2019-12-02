namespace UIDemo
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tagsTab = new System.Windows.Forms.TabPage();
            this.tagsGridControl = new UIDemo.GridControl();
            this.locationsTab = new System.Windows.Forms.TabPage();
            this.locationGridControl = new UIDemo.GridControl();
            this.lostTagsTab = new System.Windows.Forms.TabPage();
            this.lostTagsGridControl = new UIDemo.GridControl();
            this.guestsTab = new System.Windows.Forms.TabPage();
            this.guestTagsGridControl = new UIDemo.GridControl();
            this.allowedLocationsTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.allowedLocationsGridControl = new UIDemo.GridControl();
            this.allowedLocationTagNameLabel = new System.Windows.Forms.Label();
            this.devicesTab = new System.Windows.Forms.TabPage();
            this.devicesGridControl = new UIDemo.GridControl();
            this.usersTab = new System.Windows.Forms.TabPage();
            this.usersGridControl = new UIDemo.GridControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.writeTagButton = new System.Windows.Forms.Button();
            this.logoutButton = new System.Windows.Forms.Button();
            this.mainTabControl.SuspendLayout();
            this.tagsTab.SuspendLayout();
            this.locationsTab.SuspendLayout();
            this.lostTagsTab.SuspendLayout();
            this.guestsTab.SuspendLayout();
            this.allowedLocationsTab.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.devicesTab.SuspendLayout();
            this.usersTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.tagsTab);
            this.mainTabControl.Controls.Add(this.locationsTab);
            this.mainTabControl.Controls.Add(this.lostTagsTab);
            this.mainTabControl.Controls.Add(this.guestsTab);
            this.mainTabControl.Controls.Add(this.allowedLocationsTab);
            this.mainTabControl.Controls.Add(this.devicesTab);
            this.mainTabControl.Controls.Add(this.usersTab);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainTabControl.Location = new System.Drawing.Point(3, 3);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(821, 490);
            this.mainTabControl.TabIndex = 27;
            this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl1_TabIndexChanged);
            this.mainTabControl.TabIndexChanged += new System.EventHandler(this.tabControl1_TabIndexChanged);
            // 
            // tagsTab
            // 
            this.tagsTab.AutoScroll = true;
            this.tagsTab.BackColor = System.Drawing.SystemColors.Control;
            this.tagsTab.Controls.Add(this.tagsGridControl);
            this.tagsTab.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tagsTab.Location = new System.Drawing.Point(4, 26);
            this.tagsTab.Name = "tagsTab";
            this.tagsTab.Padding = new System.Windows.Forms.Padding(3);
            this.tagsTab.Size = new System.Drawing.Size(813, 460);
            this.tagsTab.TabIndex = 1;
            this.tagsTab.Text = "Tags";
            // 
            // tagsGridControl
            // 
            this.tagsGridControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tagsGridControl.BackColor = System.Drawing.Color.Gainsboro;
            this.tagsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tagsGridControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tagsGridControl.Location = new System.Drawing.Point(3, 3);
            this.tagsGridControl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tagsGridControl.Name = "tagsGridControl";
            this.tagsGridControl.Size = new System.Drawing.Size(807, 454);
            this.tagsGridControl.TabIndex = 1;
            // 
            // locationsTab
            // 
            this.locationsTab.Controls.Add(this.locationGridControl);
            this.locationsTab.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.locationsTab.Location = new System.Drawing.Point(4, 26);
            this.locationsTab.Name = "locationsTab";
            this.locationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.locationsTab.Size = new System.Drawing.Size(813, 460);
            this.locationsTab.TabIndex = 0;
            this.locationsTab.Text = "Locations";
            this.locationsTab.UseVisualStyleBackColor = true;
            this.locationsTab.Click += new System.EventHandler(this.locationsTab_Click);
            // 
            // locationGridControl
            // 
            this.locationGridControl.BackColor = System.Drawing.Color.Gainsboro;
            this.locationGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationGridControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.locationGridControl.Location = new System.Drawing.Point(3, 3);
            this.locationGridControl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.locationGridControl.Name = "locationGridControl";
            this.locationGridControl.Size = new System.Drawing.Size(807, 454);
            this.locationGridControl.TabIndex = 0;
            // 
            // lostTagsTab
            // 
            this.lostTagsTab.Controls.Add(this.lostTagsGridControl);
            this.lostTagsTab.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lostTagsTab.Location = new System.Drawing.Point(4, 26);
            this.lostTagsTab.Name = "lostTagsTab";
            this.lostTagsTab.Padding = new System.Windows.Forms.Padding(3);
            this.lostTagsTab.Size = new System.Drawing.Size(813, 460);
            this.lostTagsTab.TabIndex = 5;
            this.lostTagsTab.Text = "Lost Tags";
            this.lostTagsTab.UseVisualStyleBackColor = true;
            // 
            // lostTagsGridControl
            // 
            this.lostTagsGridControl.AutoSize = true;
            this.lostTagsGridControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.lostTagsGridControl.BackColor = System.Drawing.SystemColors.Control;
            this.lostTagsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lostTagsGridControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lostTagsGridControl.Location = new System.Drawing.Point(3, 3);
            this.lostTagsGridControl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.lostTagsGridControl.Name = "lostTagsGridControl";
            this.lostTagsGridControl.Size = new System.Drawing.Size(807, 454);
            this.lostTagsGridControl.TabIndex = 1;
            // 
            // guestsTab
            // 
            this.guestsTab.Controls.Add(this.guestTagsGridControl);
            this.guestsTab.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guestsTab.Location = new System.Drawing.Point(4, 26);
            this.guestsTab.Name = "guestsTab";
            this.guestsTab.Size = new System.Drawing.Size(813, 460);
            this.guestsTab.TabIndex = 6;
            this.guestsTab.Text = "Guests";
            this.guestsTab.UseVisualStyleBackColor = true;
            // 
            // guestTagsGridControl
            // 
            this.guestTagsGridControl.AutoSize = true;
            this.guestTagsGridControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.guestTagsGridControl.BackColor = System.Drawing.Color.Gainsboro;
            this.guestTagsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guestTagsGridControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guestTagsGridControl.Location = new System.Drawing.Point(0, 0);
            this.guestTagsGridControl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.guestTagsGridControl.Name = "guestTagsGridControl";
            this.guestTagsGridControl.Size = new System.Drawing.Size(813, 460);
            this.guestTagsGridControl.TabIndex = 1;
            // 
            // allowedLocationsTab
            // 
            this.allowedLocationsTab.Controls.Add(this.tableLayoutPanel4);
            this.allowedLocationsTab.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allowedLocationsTab.Location = new System.Drawing.Point(4, 26);
            this.allowedLocationsTab.Name = "allowedLocationsTab";
            this.allowedLocationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.allowedLocationsTab.Size = new System.Drawing.Size(813, 460);
            this.allowedLocationsTab.TabIndex = 2;
            this.allowedLocationsTab.Text = "Allowed Locations";
            this.allowedLocationsTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.Gainsboro;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.allowedLocationsGridControl, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.allowedLocationTagNameLabel, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(807, 454);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // allowedLocationsGridControl
            // 
            this.allowedLocationsGridControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.allowedLocationsGridControl.BackColor = System.Drawing.Color.Gainsboro;
            this.allowedLocationsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allowedLocationsGridControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allowedLocationsGridControl.Location = new System.Drawing.Point(2, 26);
            this.allowedLocationsGridControl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.allowedLocationsGridControl.Name = "allowedLocationsGridControl";
            this.allowedLocationsGridControl.Size = new System.Drawing.Size(803, 425);
            this.allowedLocationsGridControl.TabIndex = 1;
            // 
            // allowedLocationTagNameLabel
            // 
            this.allowedLocationTagNameLabel.AutoSize = true;
            this.allowedLocationTagNameLabel.Location = new System.Drawing.Point(3, 3);
            this.allowedLocationTagNameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.allowedLocationTagNameLabel.Name = "allowedLocationTagNameLabel";
            this.allowedLocationTagNameLabel.Size = new System.Drawing.Size(68, 17);
            this.allowedLocationTagNameLabel.TabIndex = 2;
            this.allowedLocationTagNameLabel.Text = "Tag Name";
            this.allowedLocationTagNameLabel.Visible = false;
            // 
            // devicesTab
            // 
            this.devicesTab.Controls.Add(this.devicesGridControl);
            this.devicesTab.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devicesTab.Location = new System.Drawing.Point(4, 26);
            this.devicesTab.Name = "devicesTab";
            this.devicesTab.Padding = new System.Windows.Forms.Padding(3);
            this.devicesTab.Size = new System.Drawing.Size(813, 460);
            this.devicesTab.TabIndex = 3;
            this.devicesTab.Text = "Devices";
            this.devicesTab.UseVisualStyleBackColor = true;
            // 
            // devicesGridControl
            // 
            this.devicesGridControl.AutoSize = true;
            this.devicesGridControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.devicesGridControl.BackColor = System.Drawing.Color.Gainsboro;
            this.devicesGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicesGridControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devicesGridControl.Location = new System.Drawing.Point(3, 3);
            this.devicesGridControl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.devicesGridControl.Name = "devicesGridControl";
            this.devicesGridControl.Size = new System.Drawing.Size(807, 454);
            this.devicesGridControl.TabIndex = 1;
            // 
            // usersTab
            // 
            this.usersTab.Controls.Add(this.usersGridControl);
            this.usersTab.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usersTab.Location = new System.Drawing.Point(4, 26);
            this.usersTab.Name = "usersTab";
            this.usersTab.Padding = new System.Windows.Forms.Padding(3);
            this.usersTab.Size = new System.Drawing.Size(813, 460);
            this.usersTab.TabIndex = 4;
            this.usersTab.Text = "Users";
            this.usersTab.UseVisualStyleBackColor = true;
            // 
            // usersGridControl
            // 
            this.usersGridControl.AutoSize = true;
            this.usersGridControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.usersGridControl.BackColor = System.Drawing.Color.Gainsboro;
            this.usersGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usersGridControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usersGridControl.Location = new System.Drawing.Point(3, 3);
            this.usersGridControl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.usersGridControl.Name = "usersGridControl";
            this.usersGridControl.Size = new System.Drawing.Size(807, 454);
            this.usersGridControl.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.mainTabControl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(827, 535);
            this.tableLayoutPanel1.TabIndex = 28;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.okButton, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cancelButton, 1, 0);
            this.tableLayoutPanel3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel3.Location = new System.Drawing.Point(658, 499);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 3, 9, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(160, 33);
            this.tableLayoutPanel3.TabIndex = 28;
            // 
            // okButton
            // 
            this.okButton.AutoSize = true;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(3, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(74, 27);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Visible = false;
            // 
            // cancelButton
            // 
            this.cancelButton.AutoSize = true;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(83, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(74, 27);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.writeTagButton, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.logoutButton, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel2.Location = new System.Drawing.Point(827, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(108, 535);
            this.tableLayoutPanel2.TabIndex = 29;
            // 
            // writeTagButton
            // 
            this.writeTagButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.writeTagButton.Location = new System.Drawing.Point(2, 2);
            this.writeTagButton.Margin = new System.Windows.Forms.Padding(2);
            this.writeTagButton.Name = "writeTagButton";
            this.writeTagButton.Size = new System.Drawing.Size(104, 65);
            this.writeTagButton.TabIndex = 26;
            this.writeTagButton.Text = "Write Tag";
            this.writeTagButton.UseVisualStyleBackColor = true;
            this.writeTagButton.Click += new System.EventHandler(this.writeTagButton_Click);
            // 
            // logoutButton
            // 
            this.logoutButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logoutButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logoutButton.Location = new System.Drawing.Point(3, 72);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(102, 59);
            this.logoutButton.TabIndex = 28;
            this.logoutButton.Text = "Logout";
            this.logoutButton.UseVisualStyleBackColor = true;
            this.logoutButton.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(935, 535);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainWindow";
            this.Text = "SecureID";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.mainTabControl.ResumeLayout(false);
            this.tagsTab.ResumeLayout(false);
            this.locationsTab.ResumeLayout(false);
            this.lostTagsTab.ResumeLayout(false);
            this.lostTagsTab.PerformLayout();
            this.guestsTab.ResumeLayout(false);
            this.guestsTab.PerformLayout();
            this.allowedLocationsTab.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.devicesTab.ResumeLayout(false);
            this.devicesTab.PerformLayout();
            this.usersTab.ResumeLayout(false);
            this.usersTab.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage locationsTab;
        private System.Windows.Forms.TabPage tagsTab;
        private System.Windows.Forms.TabPage lostTagsTab;
        private System.Windows.Forms.TabPage guestsTab;
        private System.Windows.Forms.TabPage allowedLocationsTab;
        private System.Windows.Forms.TabPage devicesTab;
        private System.Windows.Forms.TabPage usersTab;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private GridControl locationGridControl;
        private GridControl tagsGridControl;
        private GridControl lostTagsGridControl;
        private GridControl guestTagsGridControl;
        private GridControl allowedLocationsGridControl;
        private GridControl devicesGridControl;
        private GridControl usersGridControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label allowedLocationTagNameLabel;
        private System.Windows.Forms.Button writeTagButton;
        private System.Windows.Forms.Button logoutButton;
    }
}
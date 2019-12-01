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
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.writeTagButton = new System.Windows.Forms.Button();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.locationsTab = new System.Windows.Forms.TabPage();
            this.tagsTab = new System.Windows.Forms.TabPage();
            this.allowedLocationsTab = new System.Windows.Forms.TabPage();
            this.devicesTab = new System.Windows.Forms.TabPage();
            this.usersTab = new System.Windows.Forms.TabPage();
            this.lostTagsTab = new System.Windows.Forms.TabPage();
            this.guestsTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.logoutButton = new System.Windows.Forms.Button();
            this.locationGridControl = new UIDemo.GridControl();
            this.mainTabControl.SuspendLayout();
            this.locationsTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(820, 160);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 57);
            this.button3.TabIndex = 5;
            this.button3.Text = "Report Lost Tag";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(820, 234);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 57);
            this.button2.TabIndex = 4;
            this.button2.Text = "View Guests";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 241);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 58);
            this.button1.TabIndex = 3;
            this.button1.Text = "View Locations";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.viewLocationsButton_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(11, 350);
            this.button13.Margin = new System.Windows.Forms.Padding(2);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(104, 65);
            this.button13.TabIndex = 16;
            this.button13.Text = "View Devices";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.viewDevicesRPC);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(11, 125);
            this.button14.Margin = new System.Windows.Forms.Padding(2);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(104, 65);
            this.button14.TabIndex = 17;
            this.button14.Text = "View Tags";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.viewTagsButton_Clock);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(820, 11);
            this.button15.Margin = new System.Windows.Forms.Padding(2);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(104, 65);
            this.button15.TabIndex = 18;
            this.button15.Text = "View Lost Tags";
            this.button15.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(820, 80);
            this.button16.Margin = new System.Windows.Forms.Padding(2);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(104, 65);
            this.button16.TabIndex = 19;
            this.button16.Text = "Restore Lost Tag";
            this.button16.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(11, 11);
            this.button12.Margin = new System.Windows.Forms.Padding(2);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(104, 65);
            this.button12.TabIndex = 24;
            this.button12.Text = "View Users";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.viewUsers_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(11, 459);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(104, 65);
            this.button5.TabIndex = 25;
            this.button5.Text = "View Tag Locations";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.allowedLocations_Click);
            // 
            // writeTagButton
            // 
            this.writeTagButton.Location = new System.Drawing.Point(820, 324);
            this.writeTagButton.Margin = new System.Windows.Forms.Padding(2);
            this.writeTagButton.Name = "writeTagButton";
            this.writeTagButton.Size = new System.Drawing.Size(104, 65);
            this.writeTagButton.TabIndex = 26;
            this.writeTagButton.Text = "Write Tag";
            this.writeTagButton.UseVisualStyleBackColor = true;
            this.writeTagButton.Click += new System.EventHandler(this.writeTagButton_Click);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.locationsTab);
            this.mainTabControl.Controls.Add(this.tagsTab);
            this.mainTabControl.Controls.Add(this.lostTagsTab);
            this.mainTabControl.Controls.Add(this.guestsTab);
            this.mainTabControl.Controls.Add(this.allowedLocationsTab);
            this.mainTabControl.Controls.Add(this.devicesTab);
            this.mainTabControl.Controls.Add(this.usersTab);
            this.mainTabControl.Location = new System.Drawing.Point(3, 3);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(591, 238);
            this.mainTabControl.TabIndex = 27;
            this.mainTabControl.TabIndexChanged += new System.EventHandler(this.tabControl1_TabIndexChanged);
            // 
            // locationsTab
            // 
            this.locationsTab.Controls.Add(this.locationGridControl);
            this.locationsTab.Location = new System.Drawing.Point(4, 22);
            this.locationsTab.Name = "locationsTab";
            this.locationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.locationsTab.Size = new System.Drawing.Size(583, 212);
            this.locationsTab.TabIndex = 0;
            this.locationsTab.Text = "Locations";
            this.locationsTab.UseVisualStyleBackColor = true;
            this.locationsTab.Click += new System.EventHandler(this.locationsTab_Click);
            // 
            // tagsTab
            // 
            this.tagsTab.Location = new System.Drawing.Point(4, 22);
            this.tagsTab.Name = "tagsTab";
            this.tagsTab.Padding = new System.Windows.Forms.Padding(3);
            this.tagsTab.Size = new System.Drawing.Size(583, 212);
            this.tagsTab.TabIndex = 1;
            this.tagsTab.Text = "Tags";
            this.tagsTab.UseVisualStyleBackColor = true;
            // 
            // allowedLocationsTab
            // 
            this.allowedLocationsTab.Location = new System.Drawing.Point(4, 22);
            this.allowedLocationsTab.Name = "allowedLocationsTab";
            this.allowedLocationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.allowedLocationsTab.Size = new System.Drawing.Size(583, 212);
            this.allowedLocationsTab.TabIndex = 2;
            this.allowedLocationsTab.Text = "Allowed Locations";
            this.allowedLocationsTab.UseVisualStyleBackColor = true;
            // 
            // devicesTab
            // 
            this.devicesTab.Location = new System.Drawing.Point(4, 22);
            this.devicesTab.Name = "devicesTab";
            this.devicesTab.Padding = new System.Windows.Forms.Padding(3);
            this.devicesTab.Size = new System.Drawing.Size(583, 212);
            this.devicesTab.TabIndex = 3;
            this.devicesTab.Text = "Devices";
            this.devicesTab.UseVisualStyleBackColor = true;
            // 
            // usersTab
            // 
            this.usersTab.Location = new System.Drawing.Point(4, 22);
            this.usersTab.Name = "usersTab";
            this.usersTab.Padding = new System.Windows.Forms.Padding(3);
            this.usersTab.Size = new System.Drawing.Size(583, 212);
            this.usersTab.TabIndex = 4;
            this.usersTab.Text = "Users";
            this.usersTab.UseVisualStyleBackColor = true;
            // 
            // lostTagsTab
            // 
            this.lostTagsTab.Location = new System.Drawing.Point(4, 22);
            this.lostTagsTab.Name = "lostTagsTab";
            this.lostTagsTab.Padding = new System.Windows.Forms.Padding(3);
            this.lostTagsTab.Size = new System.Drawing.Size(583, 212);
            this.lostTagsTab.TabIndex = 5;
            this.lostTagsTab.Text = "Lost Tags";
            this.lostTagsTab.UseVisualStyleBackColor = true;
            // 
            // guestsTab
            // 
            this.guestsTab.Location = new System.Drawing.Point(4, 22);
            this.guestsTab.Name = "guestsTab";
            this.guestsTab.Size = new System.Drawing.Size(583, 212);
            this.guestsTab.TabIndex = 6;
            this.guestsTab.Text = "Guests";
            this.guestsTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.mainTabControl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.logoutButton, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(152, 18);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(597, 284);
            this.tableLayoutPanel1.TabIndex = 28;
            // 
            // logoutButton
            // 
            this.logoutButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.logoutButton.Location = new System.Drawing.Point(519, 247);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(75, 34);
            this.logoutButton.TabIndex = 28;
            this.logoutButton.Text = "Logout";
            this.logoutButton.UseVisualStyleBackColor = true;
            // 
            // locationGridControl
            // 
            this.locationGridControl.Location = new System.Drawing.Point(0, 0);
            this.locationGridControl.Name = "locationGridControl";
            this.locationGridControl.Size = new System.Drawing.Size(583, 212);
            this.locationGridControl.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 535);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.writeTagButton);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainWindow";
            this.Text = "SecureID";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.mainTabControl.ResumeLayout(false);
            this.locationsTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button writeTagButton;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage locationsTab;
        private System.Windows.Forms.TabPage tagsTab;
        private System.Windows.Forms.TabPage lostTagsTab;
        private System.Windows.Forms.TabPage guestsTab;
        private System.Windows.Forms.TabPage allowedLocationsTab;
        private System.Windows.Forms.TabPage devicesTab;
        private System.Windows.Forms.TabPage usersTab;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button logoutButton;
        private GridControl locationGridControl;
    }
}
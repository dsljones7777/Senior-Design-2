namespace UIDemo.User_Controls
{
    partial class TagControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lostLabel = new System.Windows.Forms.Label();
            this.lostCheckbox = new System.Windows.Forms.CheckBox();
            this.tagNameTextbox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.roleLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.guestCheckbox = new System.Windows.Forms.CheckBox();
            this.tagDataCombobox = new System.Windows.Forms.ComboBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lostLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lostCheckbox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tagNameTextbox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.usernameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.roleLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.guestCheckbox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tagDataCombobox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.refreshButton, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(351, 120);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lostLabel
            // 
            this.lostLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lostLabel.AutoSize = true;
            this.lostLabel.Location = new System.Drawing.Point(39, 98);
            this.lostLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lostLabel.Name = "lostLabel";
            this.lostLabel.Size = new System.Drawing.Size(32, 17);
            this.lostLabel.TabIndex = 8;
            this.lostLabel.Text = "Lost";
            // 
            // lostCheckbox
            // 
            this.lostCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lostCheckbox.AutoSize = true;
            this.lostCheckbox.Location = new System.Drawing.Point(77, 99);
            this.lostCheckbox.Name = "lostCheckbox";
            this.lostCheckbox.Size = new System.Drawing.Size(15, 14);
            this.lostCheckbox.TabIndex = 7;
            this.lostCheckbox.UseVisualStyleBackColor = true;
            // 
            // tagNameTextbox
            // 
            this.tagNameTextbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tagNameTextbox.Location = new System.Drawing.Point(77, 5);
            this.tagNameTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tagNameTextbox.Name = "tagNameTextbox";
            this.tagNameTextbox.Size = new System.Drawing.Size(240, 25);
            this.tagNameTextbox.TabIndex = 0;
            // 
            // usernameLabel
            // 
            this.usernameLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(3, 9);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(68, 17);
            this.usernameLabel.TabIndex = 3;
            this.usernameLabel.Text = "Tag Name";
            // 
            // roleLabel
            // 
            this.roleLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.roleLabel.AutoSize = true;
            this.roleLabel.Location = new System.Drawing.Point(5, 71);
            this.roleLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.roleLabel.Name = "roleLabel";
            this.roleLabel.Size = new System.Drawing.Size(66, 17);
            this.roleLabel.TabIndex = 5;
            this.roleLabel.Text = "Guest Tag";
            // 
            // passwordLabel
            // 
            this.passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(11, 42);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(60, 17);
            this.passwordLabel.TabIndex = 4;
            this.passwordLabel.Text = "Tag Data";
            // 
            // guestCheckbox
            // 
            this.guestCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.guestCheckbox.AutoSize = true;
            this.guestCheckbox.Location = new System.Drawing.Point(77, 72);
            this.guestCheckbox.Name = "guestCheckbox";
            this.guestCheckbox.Size = new System.Drawing.Size(15, 14);
            this.guestCheckbox.TabIndex = 6;
            this.guestCheckbox.UseVisualStyleBackColor = true;
            // 
            // tagDataCombobox
            // 
            this.tagDataCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagDataCombobox.FormattingEnabled = true;
            this.tagDataCombobox.Location = new System.Drawing.Point(77, 38);
            this.tagDataCombobox.Name = "tagDataCombobox";
            this.tagDataCombobox.Size = new System.Drawing.Size(240, 25);
            this.tagDataCombobox.TabIndex = 9;
            // 
            // refreshButton
            // 
            this.refreshButton.BackgroundImage = global::UIDemo.Properties.Resources.refresh;
            this.refreshButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.refreshButton.Location = new System.Drawing.Point(323, 38);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(25, 25);
            this.refreshButton.TabIndex = 10;
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // TagControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TagControl";
            this.Size = new System.Drawing.Size(354, 125);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox tagNameTextbox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.CheckBox guestCheckbox;
        private System.Windows.Forms.Label lostLabel;
        private System.Windows.Forms.CheckBox lostCheckbox;
        private System.Windows.Forms.Label roleLabel;
        private System.Windows.Forms.ComboBox tagDataCombobox;
        private System.Windows.Forms.Button refreshButton;
    }
}

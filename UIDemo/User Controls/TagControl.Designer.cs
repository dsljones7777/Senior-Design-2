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
            this.tagNameTextbox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.tagDataTextbox = new System.Windows.Forms.TextBox();
            this.guestCheckbox = new System.Windows.Forms.CheckBox();
            this.lostCheckbox = new System.Windows.Forms.CheckBox();
            this.roleLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lostCheckbox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tagNameTextbox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.usernameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.roleLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tagDataTextbox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.guestCheckbox, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(320, 124);
            this.tableLayoutPanel1.TabIndex = 1;
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
            // passwordLabel
            // 
            this.passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(11, 44);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(60, 17);
            this.passwordLabel.TabIndex = 4;
            this.passwordLabel.Text = "Tag Data";
            // 
            // tagDataTextbox
            // 
            this.tagDataTextbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tagDataTextbox.Location = new System.Drawing.Point(77, 40);
            this.tagDataTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tagDataTextbox.Name = "tagDataTextbox";
            this.tagDataTextbox.Size = new System.Drawing.Size(240, 25);
            this.tagDataTextbox.TabIndex = 1;
            this.tagDataTextbox.UseSystemPasswordChar = true;
            // 
            // guestCheckbox
            // 
            this.guestCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.guestCheckbox.AutoSize = true;
            this.guestCheckbox.Location = new System.Drawing.Point(77, 76);
            this.guestCheckbox.Name = "guestCheckbox";
            this.guestCheckbox.Size = new System.Drawing.Size(15, 14);
            this.guestCheckbox.TabIndex = 6;
            this.guestCheckbox.UseVisualStyleBackColor = true;
            // 
            // lostCheckbox
            // 
            this.lostCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lostCheckbox.AutoSize = true;
            this.lostCheckbox.Location = new System.Drawing.Point(77, 103);
            this.lostCheckbox.Name = "lostCheckbox";
            this.lostCheckbox.Size = new System.Drawing.Size(15, 14);
            this.lostCheckbox.TabIndex = 7;
            this.lostCheckbox.UseVisualStyleBackColor = true;
            // 
            // roleLabel
            // 
            this.roleLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.roleLabel.AutoSize = true;
            this.roleLabel.Location = new System.Drawing.Point(5, 75);
            this.roleLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.roleLabel.Name = "roleLabel";
            this.roleLabel.Size = new System.Drawing.Size(66, 17);
            this.roleLabel.TabIndex = 5;
            this.roleLabel.Text = "Guest Tag";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 102);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Lost";
            // 
            // TagControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TagControl";
            this.Size = new System.Drawing.Size(323, 129);
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
        private System.Windows.Forms.TextBox tagDataTextbox;
        private System.Windows.Forms.CheckBox guestCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox lostCheckbox;
        private System.Windows.Forms.Label roleLabel;
    }
}

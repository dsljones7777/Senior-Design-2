namespace UIDemo.User_Controls
{
    partial class WriteTag
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
            this.deviceSerialCombo = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tagBytesTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.asciiRadiobox = new System.Windows.Forms.RadioButton();
            this.hexRadioButton = new System.Windows.Forms.RadioButton();
            this.genRndBytesButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // deviceSerialCombo
            // 
            this.deviceSerialCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.deviceSerialCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceSerialCombo.FormattingEnabled = true;
            this.deviceSerialCombo.Location = new System.Drawing.Point(143, 24);
            this.deviceSerialCombo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.deviceSerialCombo.Name = "deviceSerialCombo";
            this.deviceSerialCombo.Size = new System.Drawing.Size(188, 25);
            this.deviceSerialCombo.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.deviceSerialCombo, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tagBytesTextbox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.genRndBytesButton, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(473, 140);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(77, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tag Data";
            // 
            // tagBytesTextbox
            // 
            this.tagBytesTextbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tagBytesTextbox.Location = new System.Drawing.Point(143, 92);
            this.tagBytesTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tagBytesTextbox.MaxLength = 12;
            this.tagBytesTextbox.Name = "tagBytesTextbox";
            this.tagBytesTextbox.Size = new System.Drawing.Size(188, 25);
            this.tagBytesTextbox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Device Serial Number";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.asciiRadiobox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.hexRadioButton, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(349, 75);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(15, 5, 5, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(61, 58);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // asciiRadiobox
            // 
            this.asciiRadiobox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.asciiRadiobox.AutoSize = true;
            this.asciiRadiobox.Checked = true;
            this.asciiRadiobox.Location = new System.Drawing.Point(3, 4);
            this.asciiRadiobox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.asciiRadiobox.Name = "asciiRadiobox";
            this.asciiRadiobox.Size = new System.Drawing.Size(55, 21);
            this.asciiRadiobox.TabIndex = 7;
            this.asciiRadiobox.TabStop = true;
            this.asciiRadiobox.Text = "ASCII";
            this.asciiRadiobox.UseVisualStyleBackColor = true;
            this.asciiRadiobox.CheckedChanged += new System.EventHandler(this.asciiRadiobox_CheckedChanged);
            // 
            // hexRadioButton
            // 
            this.hexRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.hexRadioButton.AutoSize = true;
            this.hexRadioButton.Location = new System.Drawing.Point(3, 33);
            this.hexRadioButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.hexRadioButton.Name = "hexRadioButton";
            this.hexRadioButton.Size = new System.Drawing.Size(48, 21);
            this.hexRadioButton.TabIndex = 5;
            this.hexRadioButton.Text = "Hex";
            this.hexRadioButton.UseVisualStyleBackColor = true;
            this.hexRadioButton.CheckedChanged += new System.EventHandler(this.hexRadioButton_CheckedChanged);
            // 
            // genRndBytesButton
            // 
            this.genRndBytesButton.Location = new System.Drawing.Point(349, 5);
            this.genRndBytesButton.Margin = new System.Windows.Forms.Padding(15, 5, 5, 5);
            this.genRndBytesButton.Name = "genRndBytesButton";
            this.genRndBytesButton.Size = new System.Drawing.Size(119, 60);
            this.genRndBytesButton.TabIndex = 5;
            this.genRndBytesButton.Text = "Generate Random Tag Data";
            this.genRndBytesButton.UseVisualStyleBackColor = true;
            this.genRndBytesButton.Click += new System.EventHandler(this.genRndBytesButton_Click);
            // 
            // WriteTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WriteTag";
            this.Size = new System.Drawing.Size(476, 144);
            this.Load += new System.EventHandler(this.WriteTag_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox deviceSerialCombo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tagBytesTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton asciiRadiobox;
        private System.Windows.Forms.RadioButton hexRadioButton;
        private System.Windows.Forms.Button genRndBytesButton;
    }
}

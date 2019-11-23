namespace UIDemo
{
    partial class LocationControl
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
            this.nameTextbox = new System.Windows.Forms.TextBox();
            this.serialInCombo = new System.Windows.Forms.ComboBox();
            this.serialOutCombo = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.deviceSerialLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameTextbox
            // 
            this.nameTextbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nameTextbox.Location = new System.Drawing.Point(213, 7);
            this.nameTextbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nameTextbox.Name = "nameTextbox";
            this.nameTextbox.Size = new System.Drawing.Size(359, 34);
            this.nameTextbox.TabIndex = 0;
            // 
            // serialInCombo
            // 
            this.serialInCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serialInCombo.FormattingEnabled = true;
            this.serialInCombo.Location = new System.Drawing.Point(213, 68);
            this.serialInCombo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.serialInCombo.Name = "serialInCombo";
            this.serialInCombo.Size = new System.Drawing.Size(359, 36);
            this.serialInCombo.TabIndex = 1;
            // 
            // serialOutCombo
            // 
            this.serialOutCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serialOutCombo.FormattingEnabled = true;
            this.serialOutCombo.Location = new System.Drawing.Point(213, 144);
            this.serialOutCombo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.serialOutCombo.Name = "serialOutCombo";
            this.serialOutCombo.Size = new System.Drawing.Size(359, 36);
            this.serialOutCombo.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.nameTextbox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.serialOutCombo, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.serialInCombo, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.deviceSerialLabel, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(576, 200);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 10, 4, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 28);
            this.label1.TabIndex = 3;
            this.label1.Text = "Location Name";
            // 
            // deviceSerialLabel
            // 
            this.deviceSerialLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.deviceSerialLabel.AutoSize = true;
            this.deviceSerialLabel.Location = new System.Drawing.Point(4, 58);
            this.deviceSerialLabel.Margin = new System.Windows.Forms.Padding(4, 10, 4, 10);
            this.deviceSerialLabel.Name = "deviceSerialLabel";
            this.deviceSerialLabel.Size = new System.Drawing.Size(201, 56);
            this.deviceSerialLabel.TabIndex = 4;
            this.deviceSerialLabel.Text = "Coming Into Location\r\nDevice Serial Number";
            this.deviceSerialLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 134);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 10, 4, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(200, 56);
            this.label3.TabIndex = 5;
            this.label3.Text = "Leaving Location\r\nDevice Serial Number";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(588, 212);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // LocationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "LocationControl";
            this.Size = new System.Drawing.Size(591, 215);
            this.Load += new System.EventHandler(this.LocationControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameTextbox;
        private System.Windows.Forms.ComboBox serialInCombo;
        private System.Windows.Forms.ComboBox serialOutCombo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label deviceSerialLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}

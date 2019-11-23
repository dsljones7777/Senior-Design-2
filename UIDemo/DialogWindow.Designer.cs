namespace UIDemo
{
    partial class DialogWindow
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
            this.formLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.captionLabel = new System.Windows.Forms.Label();
            this.formLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // formLayoutPanel
            // 
            this.formLayoutPanel.AutoSize = true;
            this.formLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.formLayoutPanel.ColumnCount = 2;
            this.formLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.formLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.formLayoutPanel.Controls.Add(this.cancelButton, 1, 2);
            this.formLayoutPanel.Controls.Add(this.okButton, 0, 2);
            this.formLayoutPanel.Controls.Add(this.captionLabel, 0, 0);
            this.formLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.formLayoutPanel.Name = "formLayoutPanel";
            this.formLayoutPanel.RowCount = 3;
            this.formLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.formLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.formLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.formLayoutPanel.Size = new System.Drawing.Size(261, 81);
            this.formLayoutPanel.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cancelButton.Location = new System.Drawing.Point(134, 43);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(124, 35);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.okButton.Location = new System.Drawing.Point(3, 43);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(125, 35);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // captionLabel
            // 
            this.captionLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.captionLabel.AutoSize = true;
            this.formLayoutPanel.SetColumnSpan(this.captionLabel, 2);
            this.captionLabel.Location = new System.Drawing.Point(130, 10);
            this.captionLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.captionLabel.Name = "captionLabel";
            this.captionLabel.Size = new System.Drawing.Size(0, 20);
            this.captionLabel.TabIndex = 2;
            // 
            // DialogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(785, 435);
            this.Controls.Add(this.formLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogWindow_FormClosed);
            this.Load += new System.EventHandler(this.DialogWindow_Load);
            this.formLayoutPanel.ResumeLayout(false);
            this.formLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel formLayoutPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label captionLabel;
    }
}
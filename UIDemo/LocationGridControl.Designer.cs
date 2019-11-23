namespace UIDemo
{
    partial class LocationGridControl
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
            this.controlGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.controlGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // controlGrid
            // 
            this.controlGrid.AllowUserToAddRows = false;
            this.controlGrid.AllowUserToDeleteRows = false;
            this.controlGrid.AllowUserToResizeColumns = false;
            this.controlGrid.AllowUserToResizeRows = false;
            this.controlGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.controlGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.controlGrid.Location = new System.Drawing.Point(0, 0);
            this.controlGrid.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.controlGrid.Name = "controlGrid";
            this.controlGrid.RowTemplate.Height = 28;
            this.controlGrid.Size = new System.Drawing.Size(878, 413);
            this.controlGrid.TabIndex = 0;
            // 
            // LocationGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.controlGrid);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "LocationGridControl";
            this.Size = new System.Drawing.Size(1077, 519);
            ((System.ComponentModel.ISupportInitialize)(this.controlGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView controlGrid;
    }
}

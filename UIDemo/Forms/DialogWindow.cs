using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIDemo
{
    public partial class DialogWindow : Form
    {
        public DialogWindow(string title, string caption, Control usrControl, bool showOk = true, bool showCancel = true)
        {
            InitializeComponent();
            this.Text = title;
            if (String.IsNullOrWhiteSpace(caption))
                captionLabel.Visible = false;
            captionLabel.Text = caption;
            formLayoutPanel.Controls.Add(usrControl, 0, 0);
        }

        public Func<bool> OkButtonHandler;

        private void okButton_Click(object sender, EventArgs e)
        {
            if (OkButtonHandler != null)
                if (!OkButtonHandler())
                    return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DialogWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                this.DialogResult = DialogResult.Cancel;
        }

        private void DialogWindow_Load(object sender, EventArgs e)
        {

        }
    }
}

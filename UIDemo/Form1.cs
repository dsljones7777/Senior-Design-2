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
    public partial class Form1 : Form
    {
        ServerConnection connection = new ServerConnection();
        public Form1()
        {
            InitializeComponent();
            connection.Connected += OnConnected;
            connection.FailedConnecting += OnConnectingError;
        }

        async private void Form1_Shown(object sender, EventArgs e)
        { 
            await connection.connect("127.0.0.1", 52437);
        }
        
        void OnConnectingError(object sender, Exception error)
        {
            Action uiAction = new Action(
                () => 
                {
                    this.Enabled = false;
                    DialogResult msgResult = MessageBox.Show(this, error.Message, "Failed to Connect", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (msgResult != DialogResult.Retry)
                        this.Close();
                    else
                        this.Enabled = true;
                });
            if (InvokeRequired)
                Invoke(uiAction);
            else
                uiAction.Invoke();
        }

        void OnConnected(object sender, EventArgs args)
        {
            Action uiAction = new Action(
                () =>
                {
                    this.Text = "Tag Center (Connected)";
                });
            if (InvokeRequired)
                Invoke(uiAction);
            else
                uiAction.Invoke();
        }
    }
}

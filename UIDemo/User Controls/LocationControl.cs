using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLib.Network;

namespace UIDemo
{
    public partial class LocationControl : UserControl
    {
        public LocationControl()
        {
            InitializeComponent();
        }

        public string LocationName
        {
            get
            {
                return nameTextbox.Text;
            }
            set
            {
                nameTextbox.Text = value;
            }
        }

        public string SerialIn
        {
            get
            {
                return serialInCombo.Text;
            }
            set
            {
                serialInCombo.Text = value;
            }
        }

        public string SerialOut
        {
            get
            {
                if (String.IsNullOrWhiteSpace(serialOutCombo.Text))
                    return null;
                return serialOutCombo.Text;
            }
            set
            {
                serialOutCombo.Text = value;
            }
        }

        private async void LocationControl_Load(object sender, EventArgs e)
        {
            GetUnconnectedDevicesRPC rpc = new GetUnconnectedDevicesRPC();
            try
            {
                rpc = (GetUnconnectedDevicesRPC) await rpc.executeAsync();
            }
            catch(Exception x)
            {
                MessageBox.Show(this, x.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            serialInCombo.DataSource = rpc.serialNumbers;
            if(rpc.serialNumbers != null)
            {
                var outSerials = rpc.serialNumbers.ToList();
                outSerials.Insert(0, "");
                serialOutCombo.DataSource = outSerials;
            }
        }
    }
}

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

namespace UIDemo.User_Controls
{
    public partial class WriteTag : UserControl
    {
        public WriteTag()
        {
            InitializeComponent();
        }

        public string DeviceSerialNumber
        {
            get
            {
                return deviceSerialCombo.Text;
            }
        }
        
        public byte[] TagBytes
        {
            get
            {
                if(asciiRadiobox.Checked)
                {
                    return Encoding.ASCII.GetBytes(tagBytesTextbox.Text);
                }
                else if (hexRadioButton.Checked)
                {
                    return TagControl.hexStringToByteArray(tagBytesTextbox.Text);
                }

                return null;
            }
        }

        private void hexRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (hexRadioButton.Checked)
                tagBytesTextbox.MaxLength = 24;
        }

        private void asciiRadiobox_CheckedChanged(object sender, EventArgs e)
        {
            if (asciiRadiobox.Checked)
                tagBytesTextbox.MaxLength = 12;
        }

        private async void WriteTag_Load(object sender, EventArgs e)
        {
            GetUnconnectedDevicesRPC rpc = new GetUnconnectedDevicesRPC();
            try
            {
                rpc = (GetUnconnectedDevicesRPC)await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            deviceSerialCombo.DataSource = rpc.serialNumbers;
        }
    }
}

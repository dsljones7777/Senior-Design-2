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
            await rpc.executeAsync();
            serialInCombo.DataSource = rpc.serialNumbers;
            serialOutCombo.DataSource = rpc.serialNumbers;
        }
    }
}

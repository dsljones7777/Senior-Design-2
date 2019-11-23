using SharedLib.Network;
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
    public partial class MainWindow : Form
    {
        public MainWindow(string usrname, bool allowAdminFunctions)
        {
            InitializeComponent();
        }

        private async void button11_Click(object sender, EventArgs e)
        {
            LocationControl locCtl = new LocationControl();
            DialogWindow locationWindow = new DialogWindow("Add New Location", null, locCtl);
            DialogResult result = locationWindow.ShowDialog(this);
            if(result != DialogResult.OK)
                return;
            SaveLocationRPC rpc = new SaveLocationRPC()
            {
                locationName = locCtl.LocationName,
                readerSerialIn = locCtl.SerialIn,
                readerSerialOut = locCtl.SerialOut
            };
            Task<UIClientConnection.UINetworkPacket> rpcCall = rpc.executeAsync();
            await rpcCall;
        }
    }
}

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
        GridControl gridCtl;
        private async void viewUsers_Click(object sender, EventArgs e)
        {
            //RPC call
            ViewUserRPC rpc = new ViewUserRPC();
            var task = rpc.executeAsync();
            await task;
            if (task.IsFaulted)
                return;
            DataTable tbl = new DataTable()
            {
                Columns =
                {
                    new DataColumn("User Name",typeof(string)),
                    new DataColumn("User Role",typeof(string))
                }
            };
            foreach(var x in rpc.userList)
            {
                string userRole;
                if (x.UserRole == Network.NetworkLib.Role.Admin)
                    userRole = "Administrator";
                else if (x.UserRole == Network.NetworkLib.Role.BaseUser)
                    userRole = "Generic User";
                else
                    continue;
                tbl.Rows.Add(x.Username, userRole);
            }
            gridCtl = new GridControl(true, true, true, true, true);
            gridCtl.hookAddNew(addNewUser);
            gridCtl.hookEdit(editUser);
            gridCtl.hookRemove(removeUser);
            gridCtl.loadDataSet(tbl);
            DialogWindow window = new DialogWindow("View Users", null, gridCtl, false, false);
            window.ShowDialog(this);
        }

        private void addNewUser(object sender, EventArgs e)
        {
            
        }

        private async void removeUser(object sender, EventArgs e)
        {
            DataRow[] users = gridCtl.getSelectedItems();
            if (users == null || users.Length == 0)
                return;
            DialogResult result = MessageBox.Show(this, "Are you sure you want to remove the selected users?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            foreach(DataRow user in users)
            {
                DeleteSystemUserRPC rpc = new DeleteSystemUserRPC()
                {
                    username = user["User Name"] as string
                };
                var task = rpc.executeAsync();
                await task;
                if (task.IsFaulted)
                    return;
            }
            MessageBox.Show(this, "The users were successfully removed from the system", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void editUser(object sender, EventArgs e)
        {
            DataRow[] users = gridCtl.getSelectedItems();
        }

        private void button17_Click(object sender, EventArgs e)
        {

        }
    }
}

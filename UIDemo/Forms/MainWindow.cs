using Network;
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
using UIDemo.User_Controls;

namespace UIDemo
{
    public partial class MainWindow : Form
    {
        DataTable tagTable = new DataTable()
        {
            Columns =
                {
                    new DataColumn("Tag Name",typeof(string)),
                    new DataColumn("Last Location",typeof(string)),
                    new DataColumn("Present",typeof(bool)),
                    new DataColumn("Tag Number",typeof(string))
                }
        };

        DataTable userTable = new DataTable()
        {
            Columns =
                {
                    new DataColumn("User Name",typeof(string)),
                    new DataColumn("User Role",typeof(string))
                }
        };

        DataTable locationTable = new DataTable()
        {
            Columns =
            {
                new DataColumn("Location Name",typeof(string)),
                new DataColumn("In RFID Reader Serial",typeof(string)),
                new DataColumn("Out RFID Reader Serial",typeof(string))
            }
        };

        GridControl gridCtl;

        LocationControl locationCtl;

        public MainWindow(string usrname, bool allowAdminFunctions)
        {
            InitializeComponent();
        }

        private async void viewUsers_Click(object sender, EventArgs e)
        {
            //RPC call
            ViewUserRPC rpc = new ViewUserRPC();
            try
            {
                rpc = (ViewUserRPC)await rpc.executeAsync();
            }
            catch
            {
                return;
            }
            userTable.Clear();
            foreach(var x in rpc.userList)
            {
                string userRole;
                if (x.UserRole == Network.NetworkLib.Role.Admin)
                    userRole = "Administrator";
                else if (x.UserRole == Network.NetworkLib.Role.BaseUser)
                    userRole = "Generic User";
                else
                    continue;
                userTable.Rows.Add(x.Username, userRole);
            }
            gridCtl = new GridControl(true, true, true, true, true);
            gridCtl.load(userTable, addNewUser, editUser, removeUser);
            DialogWindow window = new DialogWindow("View Users", null, gridCtl, false, false);
            window.ShowDialog(this);
        }

        private async void addNewUser(object sender, EventArgs e)
        {
            SystemUserControl ctl = new SystemUserControl("", NetworkLib.Role.BaseUser);
            DialogWindow window = new DialogWindow("Add New User", null, ctl, true, true);
            window.OkButtonHandler = 
                () => 
                {
                    if (!String.IsNullOrWhiteSpace(ctl.Password) && ctl.Password.Length >= 6)
                        return true;
                    MessageBox.Show(this, "The password must be at least 6 characters", "Bad Password", MessageBoxButtons.OK, MessageBoxIcon.Stop); ;
                    return false;
                };
            DialogResult result = window.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            NetworkLib.Role newRole = ctl.UserRole;
            AddUserRPC rpc = new AddUserRPC()
            {
                username = ctl.Username,
                pass = ctl.Password,
                userRole = ctl.UserRole
            };
            await rpc.executeAsync();
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

        private async void editUser(object sender, EventArgs e)
        {
            DataRow[] users = gridCtl.getSelectedItems();
            if (users == null || users.Length == 0)
                return;
            string role = users[0]["User Role"] as string;
            NetworkLib.Role usrRole;
            switch(role)
            {
                case "Administrator":
                    usrRole =  NetworkLib.Role.Admin;
                    break;
               default:
                    usrRole = NetworkLib.Role.BaseUser;
                    break;
            }
            SystemUserControl ctl = new SystemUserControl(users[0]["User Name"] as string, usrRole);
            ctl.disableNameEditing();
            DialogWindow window = new DialogWindow("Edit User", null,ctl, true, true);
            DialogResult result = window.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            NetworkLib.Role newRole = ctl.UserRole;

            EditUserRPC rpc = new EditUserRPC(ctl.Username, ctl.Password, newRole, newRole != usrRole);
            await rpc.executeAsync();
        }

        private async void viewTagsButton_Clock(object sender, EventArgs e)
        {
            //RPC call
            ViewTagsRPC rpc = new ViewTagsRPC();
            try
            {
                rpc = (ViewTagsRPC)await rpc.executeAsync();
            }
            catch
            {
                return;
            }
            tagTable.Clear();
            foreach (var x in rpc.tagList)
                tagTable.Rows.Add(x.TagName, x.LastLocation ,x.InLocation,x.TagNumber);
            gridCtl = new GridControl(true, true, true, true, true);
            gridCtl.load(tagTable, addNewTag, editTag, removeTags);
            DialogWindow window = new DialogWindow("View Tags", null, gridCtl, false, false);
            window.ShowDialog(this);
        }

        private void addNewTag(object sender, EventArgs e)
        {

        }

        private void removeTags(object sender, EventArgs e)
        {
            DataRow[] tags = gridCtl.getSelectedItems();
            if (tags == null || tags.Length == 0)
                return;
            DialogResult result = MessageBox.Show(this, "Are you sure you want to remove the selected tags?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            //foreach (DataRow tag in tags)
            //{
            //    DeleteTagRPC rpc = new DeleteTagRPC()
            //    {
            //        tagNumber = (string)tags["Tag Number"];
            //};
            //var task = rpc.executeAsync();
            //await task;
            //if (task.IsFaulted)
            //    return;
            //MessageBox.Show(this, "The users were successfully removed from the system", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void editTag(object sender, EventArgs e)
        {

        }

        private async void viewLocationsButton_Click(object sender, EventArgs e)
        {
            ViewLocationsRPC rpc = new ViewLocationsRPC();
            try
            {
                rpc = (ViewLocationsRPC)await rpc.executeAsync();
            }
            catch
            {
                return;
            }
            locationTable.Clear();
            foreach (var x in rpc.locationList)
                locationTable.Rows.Add(x.LocationName, x.ReaderSerialIn, x.ReaderSerialOut ?? "");
            gridCtl = new GridControl(true, false, true, true, true);
            gridCtl.load(locationTable, addNewLocation, editLocation, removeLocation);
            DialogWindow window = new DialogWindow("View Locations", null, gridCtl, false, false);
            window.ShowDialog(this);
        }

        private async void addNewLocation(object sender, EventArgs e)
        {
            locationCtl = new LocationControl();
            DialogWindow locationWindow = new DialogWindow("Add New Location", null, locationCtl);
            DialogResult result = locationWindow.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            SaveLocationRPC rpc = new SaveLocationRPC()
            {
                locationName = locationCtl.LocationName,
                readerSerialIn = locationCtl.SerialIn,
                readerSerialOut = locationCtl.SerialOut
            };
            await rpc.executeAsync();
        }

        private async void removeLocation(object sender, EventArgs e)
        {
            DataRow[] locations = gridCtl.getSelectedItems();
            if (locations == null || locations.Length == 0)
                return;
            DialogResult result = MessageBox.Show(this, "Are you sure you want to remove the selected locations?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            foreach (DataRow loc in locations)
            {
                DeleteLocationRPC rpc = new DeleteLocationRPC()
                {
                    locationName = loc["Location Name"] as string
                };
                var task = rpc.executeAsync();
                await task;
                if (task.IsFaulted)
                    return;
                gridCtl.removeRow(loc);
            }
            MessageBox.Show(locationCtl, "The locations were successfully removed from the system", "Locations Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void editLocation(object sender, EventArgs e)
        {

        }
    }
}

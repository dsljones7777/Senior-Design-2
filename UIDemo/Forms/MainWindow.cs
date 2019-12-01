using Network;
using SharedLib;
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
                    new DataColumn("Tag Number",typeof(string)),
                    new DataColumn("Lost",typeof(bool)),
                    new DataColumn("Guest", typeof(bool)),
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

        DataTable deviceTabls = new DataTable()
        {
            Columns =
            {
                new DataColumn("Device Serial",typeof(string)),
                new DataColumn("Connected",typeof(bool)),
                new DataColumn("Is System Device",typeof(bool))
            }
        };

        DataTable allowedLocationTable = new DataTable()
        {
            Columns =
            {
                new DataColumn("Location Name",typeof(string))
            }
        };

        GridControl gridCtl;

        LocationControl locationCtl;

        TagControl tagCtl;

       SystemUserControl userCtl;

        public MainWindow(string usrname, bool allowAdminFunctions)
        {
            InitializeComponent();
        }

        private async void viewUsers_Click(object sender, EventArgs e)
        {
            ViewUserRPC rpc = new ViewUserRPC();
            try
            {
                rpc = (ViewUserRPC)await rpc.executeAsync();
            }
            catch(Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            userCtl = new SystemUserControl("", NetworkLib.Role.BaseUser);
            DialogWindow window = new DialogWindow("Add New User", null, userCtl, true, true);
            window.OkButtonHandler = 
                () => 
                {
                    if (!String.IsNullOrWhiteSpace(userCtl.Password) && userCtl.Password.Length >= 6)
                        return true;
                    MessageBox.Show(userCtl, "The password must be at least 6 characters", "Bad Password", MessageBoxButtons.OK, MessageBoxIcon.Stop); ;
                    return false;
                };
            DialogResult result = window.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            NetworkLib.Role newRole = userCtl.UserRole;
            SaveSystemUserRPC rpc = new SaveSystemUserRPC()
            {
                username = userCtl.Username,
                password = userCtl.Password,
                role = (int)userCtl.UserRole
            };
            try
            {
                await rpc.executeAsync();
            }
            catch(Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }

        private async void removeUser(object sender, EventArgs e)
        {
            DataRow[] users = gridCtl.getSelectedItems();
            if (users == null || users.Length == 0)
                return;
            DialogResult result = MessageBox.Show(userCtl, "Are you sure you want to remove the selected users?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            foreach(DataRow user in users)
            {
                DeleteSystemUserRPC rpc = new DeleteSystemUserRPC()
                {
                    username = user["User Name"] as string
                };
                try
                {
                    await rpc.executeAsync();
                }
                catch (Exception except)
                {
                    MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            MessageBox.Show(userCtl, "The users were successfully removed from the system", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            try
            {
                await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private async void viewTagsButton_Clock(object sender, EventArgs e)
        {
            ViewTagsRPC rpc = new ViewTagsRPC();
            try
            {
                rpc = (ViewTagsRPC)await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tagTable.Clear();
            tagTable.DefaultView.RowFilter = "";
            foreach (DataColumn column in tagTable.Columns)
                column.ColumnMapping = MappingType.Element;
            foreach (var x in rpc.tagList)
            {
                string tagVal = BitConverter.ToString(x.TagNumber).Replace("-", "");
                tagTable.Rows.Add(x.TagName, x.LastLocation ?? "", x.InLocation, tagVal, x.LostTag, x.GuestTag);
            }
            gridCtl = new GridControl(true, false, true, true, true);
            gridCtl.load(tagTable, addNewTag, editTag, removeTags);
            DialogWindow window = new DialogWindow("View Tags", null, gridCtl, false, false);
            window.ShowDialog(this);
        }

        private async void addNewTag(object sender, EventArgs e)
        {
            tagCtl = new TagControl("",null,false,false);
            tagCtl.hideLostOption();
            tagCtl.loadPossibleNewTags();
            DialogWindow tagWindow = new DialogWindow("Add New Tag", null, tagCtl);
            tagWindow.OkButtonHandler = 
                () => 
                {
                    if(String.IsNullOrWhiteSpace(tagCtl.TagName))
                    {
                        MessageBox.Show(tagCtl, "You have specify the tag name", "Tag Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (String.IsNullOrWhiteSpace(tagCtl.TagString) || tagCtl.TagString.Length % 2 != 0)
                    {
                        MessageBox.Show(tagCtl, "You have specify the tag number and it must be an even length", "Tag Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    return true;
                };

            DialogResult result = tagWindow.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            SaveTagRPC rpc = new SaveTagRPC()
            {
                name = tagCtl.TagName,
                tagNumber = tagCtl.TagData,
                guest = tagCtl.IsGuest
            };
            try
            {
                await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private async void removeTags(object sender, EventArgs e)
        {
            DataRow[] tags = gridCtl.getSelectedItems();
            if (tags == null || tags.Length == 0)
                return;
            DialogResult result = MessageBox.Show(tagCtl, "Are you sure you want to remove the selected tags?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            foreach (DataRow tag in tags)
            {
                DeleteTagRPC rpc = new DeleteTagRPC()
                {
                    
                    name = tag["Tag Name"] as string
                };
                try
                {
                    await rpc.executeAsync();
                }
                catch (Exception except)
                {
                    MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                gridCtl.removeRow(tag);
            };
            MessageBox.Show(tagCtl, "The tags were successfully removed from the system", "Removed Tags", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void editTag(object sender, EventArgs e)
        {
            DataRow[] tags = gridCtl.getSelectedItems();
            if (tags == null || tags.Length == 0)
                return;
            bool isGuest = (bool)tags[0]["Guest"];
            bool isLost = (bool)tags[0]["Lost"];
            string name = (string)tags[0]["Tag Name"];
            string bytes = (string)tags[0]["Tag Number"];
            tagCtl = new TagControl(name, bytes, isLost, isGuest);
            tagCtl.disableTagByteEditing();
            DialogWindow window = new DialogWindow("Edit Tag", null, tagCtl, true, true);
            DialogResult result = window.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            EditTagRPC rpc = new EditTagRPC()
            {
                tagNumber = tagCtl.TagData
            };
            if (!String.Equals(name, tagCtl.TagName))
                rpc.name = tagCtl.TagName;
            if (isGuest != tagCtl.IsGuest)
                rpc.guest = tagCtl.IsGuest;
            if (isLost != tagCtl.IsLost)
                rpc.lost = tagCtl.IsLost;
            try
            {
                await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private async void viewLocationsButton_Click(object sender, EventArgs e)
        {
            ViewLocationsRPC rpc = new ViewLocationsRPC();
            try
            {
                rpc = (ViewLocationsRPC)await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
                try
                {
                    await rpc.executeAsync();
                }
                catch (Exception except)
                {
                    MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                gridCtl.removeRow(loc);
            }
            MessageBox.Show(locationCtl, "The locations were successfully removed from the system", "Locations Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void editLocation(object sender, EventArgs e)
        {
            DataRow[] locations = gridCtl.getSelectedItems();
            if (locations == null || locations.Length == 0)
                return;
            locationCtl = new LocationControl();
            locationCtl.SerialIn = locations[0]["In RFID Reader Serial"] as string;
            locationCtl.SerialOut = locations[0]["Out RFID Reader Serial"] as string;
            string currentLocation = locationCtl.LocationName = locations[0]["Location Name"] as string;
            DialogWindow locationWindow = new DialogWindow("Edit Location", null, locationCtl);
            DialogResult result = locationWindow.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            EditLocationRPC rpc = new EditLocationRPC()
            {
                currentLocationName = currentLocation,
                newLocationName = String.IsNullOrWhiteSpace(locationCtl.LocationName) ? null : locationCtl.LocationName,
                readerSerialIn = String.IsNullOrWhiteSpace(locationCtl.SerialIn) ? null : locationCtl.SerialIn,
                readerSerialOut = String.IsNullOrWhiteSpace(locationCtl.SerialOut) ? null : locationCtl.SerialOut
            };
            try
            {
                await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(gridCtl, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private async void viewDevicesRPC(object sender, EventArgs e)
        {
            GetAllDevicesRPC rpc = new GetAllDevicesRPC();
            try
            {
                rpc = (GetAllDevicesRPC)await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            deviceTabls.Clear();
            foreach (var x in rpc.devices)
                deviceTabls.Rows.Add(x.serialNumber, x.connected, x.inDB);
            gridCtl = new GridControl(false, false, false, false, false);
            gridCtl.load(deviceTabls,null,null,null);
            DialogWindow window = new DialogWindow("View Devices", null, gridCtl, true, false);
            window.ShowDialog(this);
        }

        class LocationEqualityComparer : IEqualityComparer<SharedLib.SharedModels.ViewAllowedLocationsModel>
        {
            public bool Equals(SharedModels.ViewAllowedLocationsModel x, SharedModels.ViewAllowedLocationsModel y)
            {
                return String.Equals(x.LocationName, y.LocationName, StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(SharedModels.ViewAllowedLocationsModel obj)
            {
                return obj.LocationName.GetHashCode();
            }
        }
        private async void allowedLocations_Click(object sender, EventArgs e)
        {
            ViewTagsRPC rpc = new ViewTagsRPC();
            try
            {
                rpc = (ViewTagsRPC)await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tagTable.Clear();
            tagTable.Columns["Last Location"].ColumnMapping = MappingType.Hidden;
            tagTable.Columns["Present"].ColumnMapping = MappingType.Hidden;
            tagTable.Columns["Lost"].ColumnMapping = MappingType.Hidden;
            foreach (var x in rpc.tagList)
            {
                string tagVal = BitConverter.ToString(x.TagNumber).Replace("-", "");
                tagTable.Rows.Add(x.TagName, x.LastLocation ?? "", x.InLocation, tagVal, x.LostTag, x.GuestTag);
            }
            gridCtl = new GridControl(true,true,false,false,false);
            gridCtl.load(tagTable,null,null,null);
            DialogWindow window = new DialogWindow("Select A Tag", null, gridCtl, true, true);
            window.ShowDialog(this);
            if (window.DialogResult != DialogResult.OK)
                return;
            DataRow[] selectedRows = gridCtl.getSelectedItems();
            if (selectedRows == null || selectedRows.Length == 0)
                return;
            ViewLocationsRPC locationsRPC = new ViewLocationsRPC();
            try
            {
                locationsRPC = (ViewLocationsRPC)await locationsRPC.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ViewAllowedLocationsRPC allowedRPC = new ViewAllowedLocationsRPC()
            {
                TagName = selectedRows[0]["Tag Name"] as string
            };
            try
            {
                allowedRPC = (ViewAllowedLocationsRPC)await allowedRPC.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            allowedLocationTable.Clear();
            List<DataRow> rowsToSelect = new List<DataRow>();
            foreach(var x in locationsRPC.locationList)
            {
                bool exists = allowedRPC.allowedLocationList.Contains(new SharedModels.ViewAllowedLocationsModel() { LocationName = x.LocationName }, new LocationEqualityComparer());
                DataRow row = allowedLocationTable.Rows.Add(x.LocationName);
                if (exists)
                    rowsToSelect.Add(row);
            }
            gridCtl = new GridControl(true, false, false, false, false, "Allowed");
            gridCtl.load(allowedLocationTable, null, null, null);
            gridCtl.selectRows(rowsToSelect);
            window = new DialogWindow("Select Tag Allowed Permissions", null, gridCtl, true,true);
            window.ShowDialog(this);
            if (window.DialogResult != DialogResult.OK)
                return;
            DataRow[] allowed = gridCtl.getSelectedItems();
            SaveAllowedLocationsRPC rpc2 = new SaveAllowedLocationsRPC()
            {
                locationNames = new List<string>(),
                tagName = selectedRows[0]["Tag Name"] as string
            };
            if(allowed != null)
                foreach(DataRow row in allowed)
                {
                    rpc2.locationNames.Add(row["Location Name"] as string);
                }
            try
            {
                await rpc2.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private async void writeTagButton_Click(object sender, EventArgs e)
        {
            WriteTag ctl = new WriteTag();
            DialogWindow window = new DialogWindow("Write A Tag", null, ctl, true, true);
            window.ShowDialog(this);
            if (window.DialogResult != DialogResult.OK)
                return;
            WriteTagRPC rpc = new WriteTagRPC()
            {
                newTagBytes = ctl.TagBytes,
                targetSerialNumber = ctl.DeviceSerialNumber
            };
            if (rpc.newTagBytes == null)
                return;
            if(rpc.newTagBytes.Length < 12)
            {
                byte[] tmp = new byte[12];
                Array.Copy(rpc.newTagBytes, tmp, rpc.newTagBytes.Length);
                rpc.newTagBytes = tmp;
            }
            try
            {
                await rpc.executeAsync();
            }
            catch(Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}

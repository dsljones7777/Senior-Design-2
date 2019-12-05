using Network;
using SharedLib;
using SharedLib.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        
        //DataTable locationTable = new DataTable()
        //{
        //    Columns =
        //    {
        //        new DataColumn("Location Name",typeof(string)),
        //        new DataColumn("In RFID Reader Serial",typeof(string)),
        //        new DataColumn("Out RFID Reader Serial",typeof(string))
        //    }
        //};

        //DataTable deviceTabls = new DataTable()
        //{
        //    Columns =
        //    {
        //        new DataColumn("Device Serial",typeof(string)),
        //        new DataColumn("Connected",typeof(bool)),
        //        new DataColumn("Is System Device",typeof(bool))
        //    }
        //};

        //DataTable allowedLocationTable = new DataTable()
        //{
        //    Columns =
        //    {
        //        new DataColumn("Location Name",typeof(string))
        //    }
        //};
        

       SystemUserControl userCtl;
       bool showAdminFunctions = false;

        public MainWindow(string usrname, bool allowAdminFunctions)
        {
            InitializeComponent();
            showAdminFunctions = allowAdminFunctions;
            if(!showAdminFunctions)
            {
                writeTagButton.Visible = false;
            }
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
            DataTable userTable = new DataTable()
            {
                Columns =
                {
                    new DataColumn("User Name",typeof(string)),
                    new DataColumn("User Role",typeof(string))
                }
            };

            foreach (var x in rpc.userList)
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
            usersGridControl.init(showAdminFunctions, false, showAdminFunctions, showAdminFunctions, showAdminFunctions);
            usersGridControl.load(userTable, addNewUser, editUser, removeUser);
        }

        private async void addNewUser(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
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
            }
            viewUsers_Click(null, null);
        }

        private async void removeUser(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
            DataRow[] users = usersGridControl.getSelectedItems();
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
                }
            }
            viewUsers_Click(null, null);
        }

        private async void editUser(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
            DataRow[] users = usersGridControl.getSelectedItems();
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
            }
            viewUsers_Click(null, null);
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
            foreach (var x in rpc.tagList)
            {
                string tagVal = BitConverter.ToString(x.TagNumber).Replace("-", "");
                tagTable.Rows.Add(x.TagName, x.LastLocation ?? "", x.InLocation, tagVal, x.LostTag, x.GuestTag);
            }
            tagsGridControl.init(true, false, showAdminFunctions, showAdminFunctions, true);
            tagsGridControl.load(tagTable, addNewTag, editTag, removeTags);
        }

        private async void addNewTag(object sender, EventArgs e)
        {
            TagControl tagCtl = new TagControl("",null,false,false);
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
            }
            viewTagsButton_Clock(null, null);
        }

        private async void removeTags(object sender, EventArgs e)
        {
            DataRow[] tags = tagsGridControl.getSelectedItems();
            if (tags == null || tags.Length == 0)
                return;
            DialogResult result = MessageBox.Show(this, "Are you sure you want to remove the selected tags?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    break;
                }
            };
            viewTagsButton_Clock(null, null);
        }

        private async void editTag(object sender, EventArgs e)
        {
            DataRow[] tags = tagsGridControl.getSelectedItems();
            if (tags == null || tags.Length == 0)
                return;
            bool isGuest = (bool)tags[0]["Guest"];
            bool isLost = (bool)tags[0]["Lost"];
            string name = (string)tags[0]["Tag Name"];
            string bytes = (string)tags[0]["Tag Number"];
            TagControl tagCtl = new TagControl(name, bytes, isLost, isGuest);
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
            }
            viewTagsButton_Clock(null, null);
        }

        private async void viewLocationsButton_Click(object sender, EventArgs e)
        {
            DataTable locationTable = new DataTable()
            {
                Columns =
                {
                    new DataColumn("Location Name",typeof(string)),
                    new DataColumn("In RFID Reader Serial",typeof(string)),
                    new DataColumn("Out RFID Reader Serial",typeof(string))
                }
            };
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
            foreach (var x in rpc.locationList)
                locationTable.Rows.Add(x.LocationName, x.ReaderSerialIn, x.ReaderSerialOut ?? "");
            locationGridControl.init(true, false,showAdminFunctions, showAdminFunctions,false);
            locationGridControl.load(locationTable, addNewLocation, editLocation, removeLocation);
        }

        private async void addNewLocation(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
            LocationControl locationCtl = new LocationControl();
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
            }
            viewLocationsButton_Click(null, null);
        }

        private async void removeLocation(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
            DataRow[] locations = locationGridControl.getSelectedItems();
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
                }
            }
            viewLocationsButton_Click(null, null);
        }

        private async void editLocation(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
            DataRow[] locations = locationGridControl.getSelectedItems();
            if (locations == null || locations.Length == 0)
                return;
            LocationControl locationCtl = new LocationControl();
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
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            viewLocationsButton_Click(null, null);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            tabControl1_TabIndexChanged(null, null);
        }

        private async void viewDevicesTab_Click(object sender, EventArgs e)
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
            DataTable deviceTables = new DataTable()
            {
                Columns =
                {
                    new DataColumn("Connected",typeof(bool)),
                    new DataColumn("Is System Device",typeof(bool)),
                    new DataColumn("Is Virtual Device",typeof(bool)),
                    new DataColumn("Device Serial",typeof(string)),
                }
            };
            foreach (DeviceStatus x in rpc.devices)
                deviceTables.Rows.Add(x.connected, x.inDB, x.isVirtual, x.serialNumber);
            devicesGridControl.setEditButtonName("Swap Real / Virtual Mode");
            devicesGridControl.init(showAdminFunctions, true, false,false, showAdminFunctions);
            devicesGridControl.load(deviceTables,null,swapVirtualMode,null);
            createVirtualButton.Visible = showAdminFunctions;
        }
        private async void swapVirtualMode(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
            DataRow[]  devices = devicesGridControl.getSelectedItems();
            if (devices == null || devices.Length == 0)
                return;
            bool isConnected = (bool)devices[0]["Connected"];
            bool isVirtual = (bool)devices[0]["Is Virtual Device"];
            if(!isConnected)
            {
                MessageBox.Show(this,"Cannot change a device to virtual or real mode when it is not connected","Cannot Swap Devices",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            ChangeDeviceModeRPC rpc = new ChangeDeviceModeRPC()
            {
                deviceSerial = devices[0]["Device Serial"] as string,
                virtualMode = !isVirtual
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
            viewDevicesTab_Click(sender, e);
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
            DataTable tagTable = new DataTable()
            {
                Columns =
                {
                    new DataColumn("Tag Name",typeof(string)),
                    new DataColumn("Last Location",typeof(string),null,MappingType.Hidden),
                    new DataColumn("Present",typeof(bool),null, MappingType.Hidden),
                    new DataColumn("Tag Number",typeof(string)),
                    new DataColumn("Lost",typeof(bool),null, MappingType.Hidden),
                    new DataColumn("Guest", typeof(bool)),
                }
            };
            rpc.tagList.Sort(
                (a,b) => 
                {
                    if (a.InLocation)
                        return -1;
                    if (b.InLocation)
                        return 1;
                    return 0;
                });
            foreach (var x in rpc.tagList)
            {
                string tagVal = BitConverter.ToString(x.TagNumber).Replace("-", "");
                tagTable.Rows.Add(x.TagName, x.LastLocation ?? "", x.InLocation, tagVal, x.LostTag, x.GuestTag);
            }
          
            allowedLocationsGridControl.init(true,true,false,false,true);
            if(showAdminFunctions)
                allowedLocationsGridControl.setEditButtonName("View / Edit\nAllowed Locations");
            else
                allowedLocationsGridControl.setEditButtonName("View Allowed Locations");
            allowedLocationsGridControl.load(tagTable,null,allowedLocationTagSelected,null);
            return;
            

        }

        private async void allowedLocationTagSelected(object sender, EventArgs e)
        {
            DataRow[] selected = allowedLocationsGridControl.getSelectedItems();
            if (selected == null || selected.Length == 0)
                return;
            DataTable allowedTable = new DataTable
            {
                Columns = { new DataColumn("Location Name", typeof(string)) }
            };
            ViewAllowedLocationsRPC rpc = new ViewAllowedLocationsRPC()
            {
                TagName = selected[0]["Tag Name"] as string
            };
            try
            {
                rpc = (ViewAllowedLocationsRPC)await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (var location in rpc.allowedLocationList)
                allowedTable.Rows.Add(location.LocationName);
            allowedLocationsGridControl.init(true, false, false, false, false, "Allowed");
            allowedTable = allowedLocationsGridControl.load(allowedTable, null, null, null);
            var enumerator = rpc.allowedLocationList.GetEnumerator();
            foreach(DataRow row in allowedTable.Rows)
            {
                enumerator.MoveNext();
                row["Allowed"] = enumerator.Current.TagAllowedInLoc;
            }
            okButton.Visible = showAdminFunctions;
            okButton.Click += AllowedLocationsOkButton_Click;
            cancelButton.Visible = true;
            cancelButton.Click += AllowedLocationsCancelButton_Click;
            mainTabControl.TabIndexChanged += allowedLocationsTabChangedCleanup;
            allowedLocationTagNameLabel.Visible = true;
            allowedLocationTagNameLabel.Text = "Allowed Locations for Tag: " + selected[0]["Tag Name"] as string;
        }

        private void allowedLocationsTabChangedCleanup(object sender, EventArgs e)
        {
            okButton.Visible = false;
            okButton.Click -= AllowedLocationsOkButton_Click;
            cancelButton.Click -= AllowedLocationsCancelButton_Click;
            cancelButton.Visible = false;
            mainTabControl.TabIndexChanged -= allowedLocationsTabChangedCleanup;
            allowedLocationTagNameLabel.Visible = false;
        }

        private async void AllowedLocationsOkButton_Click(object sender, EventArgs e)
        {
            DataRow[] selectedRows = allowedLocationsGridControl.getSelectedItems();
            
            SaveAllowedLocationsRPC rpc = new SaveAllowedLocationsRPC()
            {
                locationNames = new List<string>(),
                tagName = allowedLocationTagNameLabel.Text.Replace("Allowed Locations for Tag: ", "")
            };
            if (selectedRows != null)
                foreach (var row in selectedRows)
                    if ((bool)row["Allowed"])
                        rpc.locationNames.Add(row["Location Name"] as string);
            try
            {
                await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            allowedLocationsTabChangedCleanup(null, null);
            allowedLocations_Click(null, null);
        }

        private void AllowedLocationsCancelButton_Click(object sender, EventArgs e)
        {
            allowedLocationsTabChangedCleanup(null, null);
            allowedLocations_Click(null, null);
        }

        private async void writeTagButton_Click(object sender, EventArgs e)
        {
            if (!showAdminFunctions)
                return;
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

        

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            createVirtualButton.Visible = false;
            TabPage currentPage = mainTabControl.TabPages[mainTabControl.SelectedIndex];
            if (currentPage == tagsTab)
                viewTagsButton_Clock(null, null);
            else if (currentPage == locationsTab)
                viewLocationsButton_Click(null, null);
            else if (currentPage == usersTab)
                viewUsers_Click(null, null);
            else if (currentPage == allowedLocationsTab)
                allowedLocations_Click(null, null);
            else if (currentPage == guestsTab)
                guestTab_Click(sender, e);
            else if (currentPage == lostTagsTab)
                lostTab_Click(sender, e);
            else if (currentPage == devicesTab)
                viewDevicesTab_Click(sender, e);
            return;
        }

        private async void guestTab_Click(object sender, EventArgs e)
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
            foreach (var x in rpc.tagList)
            {
                string tagVal = BitConverter.ToString(x.TagNumber).Replace("-", "");
                if(!x.LostTag && x.GuestTag)
                    tagTable.Rows.Add(x.TagName, x.LastLocation ?? "", x.InLocation, tagVal);
            }
            guestTagsGridControl.init(true, false, false, false, true);
            guestTagsGridControl.load(tagTable, addNewTag, editTag, removeTags);
        }
        private async void lostTab_Click(object sender, EventArgs e)
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
            DataTable tagTable = new DataTable()
            {
                Columns =
                {
                    new DataColumn("Tag Name",typeof(string)),
                    new DataColumn("Last Location",typeof(string)),
                    new DataColumn("Present",typeof(bool)),
                    new DataColumn("Tag Number",typeof(string)),
                    new DataColumn("Guest",typeof(string))
                }
            };
            foreach (var x in rpc.tagList)
            {
                string tagVal = BitConverter.ToString(x.TagNumber).Replace("-", "");
                if (x.LostTag)
                    tagTable.Rows.Add(x.TagName, x.LastLocation ?? "", x.InLocation, tagVal,x.LostTag);
            }
            lostTagsGridControl.init(true, false, false, false, true);
            lostTagsGridControl.load(tagTable, addNewTag, editTag, removeTags);
        }

        private void locationsTab_Click(object sender, EventArgs e)
        {

        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void spawnVirtual(string serialNumber)
        {
            string connection = UIDemo.Properties.Settings.Default.HostIP + " " + UIDemo.Properties.Settings.Default.HostPort.ToString();
            string argString;
            if (serialNumber != null)
                argString = connection + " -serial \"" + serialNumber + "\" -s";
            else
                argString = connection + " -s";
#if DEBUG
            Process.Start("..\\..\\..\\Debug\\RFIDDeviceController.exe", argString);
#else
            Process.Start("RFIDDeviceController.exe", argString);
#endif
        }

        private void createVirtualButton_Click(object sender, EventArgs e)
        {
            DataRow [] selected = devicesGridControl.getSelectedItems();
            if(selected == null || selected.Length == 0)
                spawnVirtual(null);
            else
                foreach(var row in selected)
                    spawnVirtual(row["Device Serial"] as string);
            viewDevicesTab_Click(sender, e);
        }
    }
}

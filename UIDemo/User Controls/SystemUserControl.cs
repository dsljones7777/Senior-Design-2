using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Network.NetworkLib;

namespace UIDemo.User_Controls
{
    public partial class SystemUserControl : UserControl
    {
        public SystemUserControl(string usrName, Role role)
        {
            InitializeComponent();
            roleCombobox.DataSource = new List<string>()
            {
                "Generic User",
                "Administrator"
            };
            if (role == Role.BaseUser)
                roleCombobox.SelectedIndex = 0;
            else if(role == Role.Admin)
                roleCombobox.SelectedIndex = 1;
            else
                roleCombobox.SelectedIndex = 0;
            usernameTextbox.Text = usrName;
        }

        public string Username
        {
            get
            {
                return usernameTextbox.Text;
            }
        }

        public string Password
        {
            get
            {
                return passwordTextbox.Text;
            }
        }

        public Role UserRole
        {
            get
            {
                if (roleCombobox.SelectedIndex == 0)
                    return Role.BaseUser;
                else if (roleCombobox.SelectedIndex == 1)
                    return Role.Admin;
                return Role.BaseUser;
            }
        }

        public void disableNameEditing()
        {
            usernameTextbox.Enabled = false;
        }

    }
}

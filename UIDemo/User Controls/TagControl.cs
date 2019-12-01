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
    public partial class TagControl : UserControl
    {
        public TagControl(string tagName, string tagBytes, bool isLost, bool isGuest)
        {
            InitializeComponent();
            tagNameTextbox.Text = tagName;
            tagDataCombobox.DataSource = new List<string> { tagBytes ?? "" };
            tagDataCombobox.Text = tagBytes ?? "";
            lostCheckbox.Checked = isLost;
            guestCheckbox.Checked = isGuest;
        }

        public async void loadPossibleNewTags()
        {
            ViewTagsRPC rpc = new ViewTagsRPC()
            {
                nonSystemTagsOnly = true
            };
            try
            {
                rpc = (ViewTagsRPC)await rpc.executeAsync();
            }
            catch (Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tagDataCombobox.DataSource = rpc.tagList;
        }

        public string TagName
        {
            get
            {
                return tagNameTextbox.Text;
            }
        }

        internal static byte[] hexStringToByteArray(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                             .ToArray();
        }

        public string TagString
        {
            get
            {
                return tagDataCombobox.Text ;
            }
        }

        public byte[] TagData
        {
            get
            {
                if (String.IsNullOrWhiteSpace(tagDataCombobox.Text))
                    return null;
                return hexStringToByteArray(tagDataCombobox.Text);
            }
        }

        public bool IsGuest
        {
            get
            {
                return guestCheckbox.Checked;
            }
        }

        public bool IsLost
        {
            get
            {
                return lostCheckbox.Checked;
            }
        }

        public void hideLostOption()
        {
            lostCheckbox.Visible = false;
        }

        public void disableTagByteEditing()
        {
            tagDataCombobox.Enabled = false;
        }
    }
}

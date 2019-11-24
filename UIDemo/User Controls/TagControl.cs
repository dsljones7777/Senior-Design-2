using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIDemo.User_Controls
{
    public partial class TagControl : UserControl
    {
        public TagControl(string tagName, string tagBytes, bool isLost, bool isGuest)
        {
            InitializeComponent();
            tagNameTextbox.Text = tagName;
            tagDataTextbox.Text = tagBytes;
            lostCheckbox.Checked = isLost;
            guestCheckbox.Checked = isGuest;
        }

        public string TagName
        {
            get
            {
                return tagNameTextbox.Text;
            }
        }

        public byte[] TagData
        {
            get
            {
                if (String.IsNullOrWhiteSpace(tagDataTextbox.Text))
                    return null;
                return Convert.FromBase64String(tagDataTextbox.Text);
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
            tagDataTextbox.Enabled = false;
        }
    }
}

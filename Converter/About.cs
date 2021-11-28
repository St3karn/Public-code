using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Converter
{
    partial class About : Form
    {
        public bool darkModeChecked = false;

        public About()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.lbProductName.Text = AssemblyProduct;
            this.lbVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.lbCopyright.Text = AssemblyCopyright;
            this.tbDescription.Text = AssemblyDescription;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        #endregion

        private void TableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {
            if (darkModeChecked)
            {
                this.BackColor = Color.Black;
                tbDescription.BackColor = Color.Black;
                logoPictureBox.BackColor = Color.Black;
                lbProductName.BackColor = Color.Black;
                lbVersion.BackColor = Color.Black;
                lbCopyright.BackColor = Color.Black;
                lblAuthor.BackColor = Color.Black;
                lbContact.BackColor = Color.Black;

                tbDescription.ForeColor = Color.Cyan;
                lbProductName.ForeColor = Color.Cyan;
                lbVersion.ForeColor = Color.Cyan;
                lbCopyright.ForeColor = Color.Cyan;
                lblAuthor.ForeColor = Color.Cyan;
                lbContact.ForeColor = Color.Cyan;

                tableLayoutPanel.BackColor = Color.Black;
                okButton.BackColor = Color.Empty;
                okButton.ForeColor = Color.Cyan;
            }
            return;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            About.ActiveForm.Close();
        }
    }
}

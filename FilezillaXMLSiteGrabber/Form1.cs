using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace FilezillaXMLSiteGrabber
{
    public partial class Form1 : Form
    {
        private string filepath;
        private const string root_element = "FileZilla3";
        private const string servers_element = "Servers";
        private const string server_element = "Server";
        private const string name_element = "Name";

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (diaOpenXml.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepath = diaOpenXml.FileName;
                LoadSites();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportSites();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportSites();
        }

        void ExportSites()
        {
            if (!System.IO.File.Exists(filepath))
            {
                MessageBox.Show("File does not exist anymore!\r\nPlease select a new file.");
                return;
            }

            XElement root_old = XElement.Load(filepath);

            if (!ValidateXmlFile(root_old))
            {
                MessageBox.Show("The XML file selected does not appear to be a valid Filezilla XML file.\r\nIt might be that the file does not have any site manager entries.");
                return;
            }

            if (diaSaveXml.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XElement servers_new = new XElement(servers_element);
                XElement root_new = new XElement(root_element,
                    servers_new);

                foreach (string item in clstSites.CheckedItems)
                {
                    XElement server = (from server_old in root_old.Descendants(server_element)
                                       where server_old.Element(name_element).Value == item
                                       select server_old).FirstOrDefault();

                    servers_new.Add(server);
                }

                root_new.Save(diaSaveXml.FileName);
            }
        }



        void LoadSites()
        {
            XElement root = XElement.Load(filepath);

            if (!ValidateXmlFile(root))
            {
                MessageBox.Show("The XML file selected does not appear to be a valid Filezilla XML file.\r\nIt might be that the file does not have any site manager entries.");
                return;
            }

            IEnumerable<XElement> servers = root.Descendants(server_element);

            string[] server_names = (from server in servers
                                     select server.Element(name_element).Value).ToArray();

            clstSites.Items.AddRange(server_names);
        }

        bool ValidateXmlFile(XElement root)
        {
            if (root.Name != root_element)
                return false;

            if (root.Element(servers_element) == null)
                return false;

            if (root.Element(servers_element).Element(server_element) == null)
                return false;

            return true;
        }
    }


}

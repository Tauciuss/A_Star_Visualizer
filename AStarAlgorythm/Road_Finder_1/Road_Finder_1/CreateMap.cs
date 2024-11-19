using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Road_Finder_1
{
    public partial class CreateMap : Form
    {
        string path = "";
        public CreateMap()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    path = openFileDialog.FileName;
                    pathTextBox.Text = path;

                    //Devides cities to the object
                    string[] lines = File.ReadAllLines(path);

                    foreach (string line in lines)
                    {
                        cities_richText.Text += line + "\n";
                    }

                }
            }
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            if(path == null || path == "")
            {
                MessageBox.Show("There is no selected folder", "Error");
            }
            else { 
                if ( cities_richText.Text != null && cities_richText.Text != "")
                {
                    TextWriter writer = new StreamWriter(path);
                    writer.Write(cities_richText.Text);
                    writer.Close();
                }
                else
                {
                    MessageBox.Show("The fields are empty. You cannot save and empty file.", "Error" );
                }
            }

        }
    }
}

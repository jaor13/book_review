using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace book_review
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            sidePanel.Height = guna2Button1.Height;
            sidePanel.Top = guna2Button1.Top;
            dashboardCustomControl1.BringToFront();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void guna2TileButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void sidePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click_2(object sender, EventArgs e)
        {
            sidePanel.Height = guna2Button1.Height;
            sidePanel.Top = guna2Button1.Top;
            dashboardCustomControl1.BringToFront();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            sidePanel.Height = guna2Button2.Height;
            sidePanel.Top = guna2Button2.Top;
            usersUserControl1.BringToFront();

        }
    }
}

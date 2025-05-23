﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace book_review
{
    public partial class Dashboard : Form
    {
        private Color defaultForeColor = Color.FromArgb(130, 130, 130);
        private Color activeForeColor = Color.White;
        private Image defaultImage1, activeImage1, defaultImage2, activeImage2, defaultImage3, activeImage3, defaultImage4, activeImage4, defaultImage5,defaultImage6,activeImage6, activeImage5;

       


        public Dashboard()
        {

            InitializeComponent();
            if (this.reviewsUserControl1 != null)
            {
                this.reviewsUserControl1.ReviewDataChanged += ReviewsUserControl1_ReviewDataChanged;
            }



            sidePanel.Height = guna2Button1.Height;
            sidePanel.Top = guna2Button1.Top;
            dashboardCustomControl1.BringToFront();
            dashboardCustomControl1.Visible = true;
            usersUserControl1.Visible = false;
            booksUserControl1.Visible = false;
            authorsControl1.Visible = false;
            genreUserControl1.Visible = false;
            reviewsUserControl1.Visible = false;
          

            defaultImage1 = guna2Button1.Image;
            activeImage1 = Properties.Resources.a_dashboard;
            defaultImage2 = guna2Button2.Image;
            activeImage2 = Properties.Resources.icons8_users_501;
            defaultImage3 = guna2Button3.Image;
            activeImage3 = Properties.Resources.a_books_2;
            defaultImage4 = guna2Button4.Image;
            activeImage4 = Properties.Resources.a_author;
            defaultImage5 = guna2Button5.Image;
            activeImage5 = Properties.Resources.a_reviews;
            defaultImage6 = guna2Button6.Image;
            activeImage6 = Properties.Resources.a_books;


            guna2Button1.ForeColor = activeForeColor;
            guna2Button1.Image = activeImage1;
            guna2Button2.ForeColor = defaultForeColor;
            guna2Button2.Image = defaultImage2;
            guna2Button3.ForeColor = defaultForeColor;
            guna2Button3.Image = defaultImage3;
            guna2Button4.ForeColor = defaultForeColor;
            guna2Button4.Image = defaultImage4;
            guna2Button5.ForeColor = defaultForeColor;
            guna2Button5.Image = defaultImage5;
            guna2Button6.ForeColor = defaultForeColor;
            guna2Button6.Image = defaultImage6;

        }
        private void ReviewsUserControl1_ReviewDataChanged(object sender, EventArgs e)
        {
            if (this.booksUserControl1 != null && !this.booksUserControl1.IsDisposed)
            {
                this.booksUserControl1.RefreshBookGridData(); 
            }
        }
        private void Dashboard_Load(object sender, EventArgs e)
        {
            if (this.reviewsUserControl1 != null)
            {
                this.reviewsUserControl1.ReviewDataChanged -= ReviewsUserControl1_ReviewDataChanged;
                this.reviewsUserControl1.ReviewDataChanged += ReviewsUserControl1_ReviewDataChanged;
            }
        }

        public dashboardCustomControl DashboardControl
        {
            get { return dashboardCustomControl1; }
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

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            SetActiveButton(guna2Button4);
            sidePanel.Height = guna2Button4.Height;
            sidePanel.Top = guna2Button4.Top;
            //authorUserControl1.BringToFront();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
         
        }

        private void pieChart1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void dashboardCustomControl1_Load(object sender, EventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button5_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Button5_Click_2(object sender, EventArgs e)
        {
            SetActiveButton(guna2Button5);
            sidePanel.Height = guna2Button5.Height;
            sidePanel.Top = guna2Button5.Top;
            reviewsUserControl1.BringToFront();
            reviewsUserControl1.Visible = true;
            usersUserControl1.Visible = false;
            dashboardCustomControl1.Visible = false;
            booksUserControl1.Visible = false;
            authorsControl1.Visible = false;
            genreUserControl1.Visible = false;
        }

        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            SetActiveButton(guna2Button4);
            sidePanel.Height = guna2Button4.Height;
            sidePanel.Top = guna2Button4.Top;
            authorsControl1.BringToFront();
            authorsControl1.Visible = true;
            usersUserControl1.Visible = false;
            dashboardCustomControl1.Visible = false;
            booksUserControl1.Visible = false;
            genreUserControl1.Visible = false;
            reviewsUserControl1.Visible = false;

        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            SetActiveButton(guna2Button6);
            sidePanel.Height = guna2Button6.Height;
            sidePanel.Top = guna2Button6.Top;
            genreUserControl1.BringToFront();
            genreUserControl1.Visible = true;
            usersUserControl1.Visible = false;
            dashboardCustomControl1.Visible = false;
            booksUserControl1.Visible = false;
            authorsControl1.Visible = false;
            reviewsUserControl1.Visible = false;
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
            SetActiveButton(guna2Button3);
            sidePanel.Height = guna2Button3.Height;
            sidePanel.Top = guna2Button3.Top;
            booksUserControl1.BringToFront();
            booksUserControl1.Visible = true;
            dashboardCustomControl1.Visible = false;
            authorsControl1.Visible = false;
            usersUserControl1.Visible = false;
            genreUserControl1.Visible = false;
            reviewsUserControl1.Visible = false;
        }

        private void sidePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click_2(object sender, EventArgs e)
        {
            SetActiveButton(guna2Button1);
            sidePanel.Height = guna2Button1.Height;
            sidePanel.Top = guna2Button1.Top;
            dashboardCustomControl1.BringToFront();
            dashboardCustomControl1.Visible = true;
            usersUserControl1.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            SetActiveButton(guna2Button2);
            sidePanel.Height = guna2Button2.Height;
            sidePanel.Top = guna2Button2.Top;
            usersUserControl1.BringToFront();
            usersUserControl1.Visible = true;
            dashboardCustomControl1.Visible = false;
            booksUserControl1.Visible = false;
            authorsControl1.Visible = false;
            genreUserControl1.Visible = false;
            reviewsUserControl1.Visible = false;
        }

        private void SetActiveButton(Guna2Button activeButton)
        {
            // Reset all buttons to default state
            ResetButton(guna2Button1, defaultImage1);
            ResetButton(guna2Button2, defaultImage2);
            ResetButton(guna2Button3, defaultImage3);
            ResetButton(guna2Button4, defaultImage4);
            ResetButton(guna2Button5, defaultImage5);
            ResetButton(guna2Button6, defaultImage6);

            // Set the active button's fore color, image, and font
            activeButton.ForeColor = activeForeColor;
            if (activeButton == guna2Button1)
            {
                activeButton.Image = activeImage1;
            }
            else if (activeButton == guna2Button2)
            {
                activeButton.Image = activeImage2;
            }
            else if (activeButton == guna2Button3)
            {
                activeButton.Image = activeImage3;
            }
            else if (activeButton == guna2Button4)
            {
                activeButton.Image = activeImage4;
            }
            else if (activeButton == guna2Button5)
            {
                activeButton.Image = activeImage5;
            }
            else if (activeButton == guna2Button6)
            {
                activeButton.Image = activeImage6;
            }
        }

        private void ResetButton(Guna2Button button, Image defaultImage)
        {
            button.ForeColor = defaultForeColor;
            button.Image = defaultImage;
        }
    }
}

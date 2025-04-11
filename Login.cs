using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms.Enums;  


namespace book_review
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Login_FormClosed);
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }



        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string email = guna2TextBox1.Text; // Assuming guna2TextBox1 is for email
            string password = guna2TextBox2.Text;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM users WHERE email = @Email AND password = @Password AND account_status = 'active'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        // Close the reader
                        reader.Close();

                        // Hide the login form
                        this.Hide();

                        // Create and show the dashboard form
                        Dashboard dashboard = new Dashboard();
                        dashboard.Show();
                    }
                    else
                    {
                        // Close the reader
                        reader.Close();

                        // Reset and show an error message using Guna2MessageDialog
                        guna2MessageDialog1.Text = "Invalid email or password";
                        guna2MessageDialog1.Caption = "Login Failed";
                        guna2MessageDialog1.Buttons = MessageDialogButtons.OK;
                        guna2MessageDialog1.Icon = MessageDialogIcon.Error;
                        guna2MessageDialog1.Show();
                    }
                }
                catch (Exception ex)
                {
                    // Reset and show an error message using Guna2MessageDialog
                    guna2MessageDialog1.Text = "An error occurred: " + ex.Message;
                    guna2MessageDialog1.Caption = "Error";
                    guna2MessageDialog1.Buttons = MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = MessageDialogIcon.Error;
                    guna2MessageDialog1.Show();
                }
            }
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }



        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace book_review
{
    public partial class ForgotPass : Form
    {
        private bool isEmailValid = false; // To track if the email exists in the database

        public ForgotPass()
        {
            InitializeComponent();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add logic for real-time feedback (e.g., formatting) if needed
            // Do not validate the email here
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            // This method is for accepting the new password
            // You can add validation logic here if needed (e.g., password strength)
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!isEmailValid)
            {
                MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string email = guna2TextBox1.Text;
            string newPassword = guna2TextBox2.Text; // Assuming guna2TextBox2 is for the new password

            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE users SET password = @Password WHERE email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close(); // Close the ForgotPass form
                    }
                    else
                    {
                        MessageBox.Show("Failed to change password. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string email = guna2TextBox1.Text; // Assuming guna2TextBox1 is for email input

            if (string.IsNullOrEmpty(email))
            {
                isEmailValid = false;
                guna2TextBox1.ForeColor = System.Drawing.Color.Black; // Reset color
                MessageBox.Show("Please enter an email address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        isEmailValid = true;
                        guna2TextBox1.ForeColor = System.Drawing.Color.Green; // Indicate valid email
                        MessageBox.Show("Email exists in the system.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        isEmailValid = false;
                        guna2TextBox1.ForeColor = System.Drawing.Color.Red; // Indicate invalid email
                        MessageBox.Show("Email does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

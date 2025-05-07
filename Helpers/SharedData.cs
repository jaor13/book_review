using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace book_review.Helpers
{
    public static class SharedData
    {
        // Property to store the user count
        public static int UserCount { get; private set; }

        // Method to refresh the user count from the database
        public static void RefreshUserCount()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE role = 'user'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    UserCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while refreshing user count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
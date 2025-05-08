using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace book_review.Helpers
{
    public static class SharedData
    {
        // Property to store the user count
        public static int UserCount { get; private set; }
        public static int AuthorCount { get; private set; } 
        public static int BookCount { get; private set; } 

        public static int ReviewCount { get; private set; } 


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

        public static void RefreshAuthorCount() // New Method
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM authors";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    AuthorCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    // Consider logging this error
                    MessageBox.Show("An error occurred while refreshing author count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void RefreshBookCount() // New Method
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM books";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    BookCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    // Consider logging this error
                    MessageBox.Show("An error occurred while refreshing book count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void RefreshReviewCount() // New Method
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM reviews";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    ReviewCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    // Consider logging this error
                    MessageBox.Show("An error occurred while refreshing review count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




    }
}
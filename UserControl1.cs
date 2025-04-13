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
    public partial class dashboardCustomControl : UserControl
    {
        public dashboardCustomControl()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                this.Load += UserControl1_Load;
            }
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            // Double-check we're not in design mode
            if (IsInDesignMode())
            {
                return;
            }

            try
            {
                UpdateUserCount();
                UpdateBookCount();
                UpdateReviewCount();
                UpdateAuthorCount();
                LoadTopBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during load: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsInDesignMode()
        {
            // More reliable design mode detection
            return DesignMode || this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) != null
                || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }


        private void LoadTopBooks()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM book_review.bookpopularityview LIMIT 10";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    guna2DataGridView1.AutoGenerateColumns = false;
                    guna2DataGridView1.Columns.Clear();

                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ranking", HeaderText = "Ranking" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "book_info", HeaderText = "Book Info" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "popularity_score", HeaderText = "Popularity Score" });

                    guna2DataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching top books: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateUserCount()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE role = 'user'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    label4.Text = userCount.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching user count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateBookCount()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM books";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int bookCount = Convert.ToInt32(cmd.ExecuteScalar());
                    label6.Text = bookCount.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching book count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateReviewCount()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM reviews";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int reviewCount = Convert.ToInt32(cmd.ExecuteScalar());
                    label8.Text = reviewCount.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching review count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateAuthorCount()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM authors";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int authorCount = Convert.ToInt32(cmd.ExecuteScalar());
                    label10.Text = authorCount.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching author count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

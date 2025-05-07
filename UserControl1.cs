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
using Guna.UI2.WinForms;
using Guna.Charts.WinForms;
using System.Windows.Forms.DataVisualization.Charting;
using book_review.Helpers; 



namespace book_review
{
    public partial class dashboardCustomControl : UserControl
    {

        public dashboardCustomControl()
        {
            InitializeComponent();
            this.gunaLineDataset1 = new Guna.Charts.WinForms.GunaLineDataset();
            this.gunaDoughnutDataset1 = new Guna.Charts.WinForms.GunaDoughnutDataset();



            if (!DesignMode)
            {
                this.Load += UserControl1_Load;
            }



        }
        public void UpdateUserCountLabel()
        {
            // Update the label with the shared user count
            label4.Text = SharedData.UserCount.ToString();
        }



        public void RefreshUserCount()
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

                    // Update label4 on the UI thread
                    if (label4.InvokeRequired)
                    {
                        label4.Invoke(new Action(() => label4.Text = userCount.ToString()));

                    }
                    else
                    {
                        label4.Text = userCount.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching user count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                gunaChart1_Load(sender, e);
                gunaChart2_Load(sender, e); 
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

        private void UpdateUserCount() // This is your existing method
        {
            if (IsInDesignMode()) return; // Good to have this check here too

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE role = 'user'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int userCount = Convert.ToInt32(result);
                        // Ensure label4 is accessible and on the correct thread
                        if (label4 != null) // Check if label4 itself is not null
                        {
                            if (label4.InvokeRequired)
                            {
                                label4.Invoke(new Action(() => label4.Text = userCount.ToString()));
                            }
                            else
                            {
                                label4.Text = userCount.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!IsInDesignMode()) // Avoid MessageBox in DesignMode
                    {
                        MessageBox.Show("An error occurred while fetching user count: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

     

        private void gunaChart1_Load(object sender, EventArgs e)
        {
            // Prevent execution in Design Mode
            if (IsInDesignMode())
            {
                return;
            }

            // Check if the dataset component is initialized (good for runtime, might be redundant if IsInDesignMode() catches it)
            if (this.gunaLineDataset1 == null)
            {
                // Consider removing MessageBox for designer, or ensure IsInDesignMode() prevents this path.
                // For runtime, this is a valid check.
                // MessageBox.Show("Error: gunaLineDataset1 is not initialized. Please check the control's designer.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Check if the chart component is initialized
            if (this.gunaChart1 == null)
            {
                // MessageBox.Show("Error: gunaChart1 is not initialized. Please check the control's designer.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.gunaLineDataset1.Label = "Number of Reviews Over Time";


            string connectionString = null;
            try
            {
                connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    // MessageBox.Show("Connection string 'MySqlConnection' not found or is empty.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit if connection string is not valid
                }
            }
            catch (Exception)
            {
                // MessageBox.Show("Error accessing connection string: " + ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit on configuration error
            }


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT DATE(created_at) AS ReviewDate, COUNT(*) AS ReviewCount
                        FROM reviews
                        GROUP BY DATE(created_at)
                        ORDER BY ReviewDate";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    List<string> dates = new List<string>();
                    List<Guna.Charts.WinForms.LPoint> dataPoints = new List<Guna.Charts.WinForms.LPoint>();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("ReviewDate")) && !reader.IsDBNull(reader.GetOrdinal("ReviewCount")))
                        {
                            string date = reader.GetDateTime("ReviewDate").ToString("yyyy-MM-dd");
                            double reviewCount = reader.GetDouble("ReviewCount");

                            dates.Add(date);
                            dataPoints.Add(new Guna.Charts.WinForms.LPoint(date, reviewCount));
                        }
                    }

                    reader.Close();

                    gunaLineDataset1.DataPoints.Clear();
                    gunaLineDataset1.DataPoints.AddRange(dataPoints);

                    gunaChart1.Datasets.Clear();
                    if (!gunaChart1.Datasets.Contains(gunaLineDataset1))
                    {
                        gunaChart1.Datasets.Add(gunaLineDataset1);
                    }

                    gunaChart1.Update();
                }
                catch (Exception ex)
                {
                    // Avoid showing MessageBox if still in a designer-related path,
                    // though IsInDesignMode() at the top should prevent this.
                    if (!IsInDesignMode())
                    {
                        MessageBox.Show("An error occurred while loading the chart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void gunaChart2_Load(object sender, EventArgs e)
        {
            // Prevent execution in Design Mode
            if (IsInDesignMode())
            {
                return;
            }

            // Check if the dataset component is initialized (good for runtime)
            if (this.gunaDoughnutDataset1 == null)
            {
                // MessageBox.Show("Error: gunaDoughnutDataset1 is not initialized. Please check the control's designer.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Check if the chart component is initialized
            if (this.gunaChart2 == null)
            {
                // MessageBox.Show("Error: gunaChart2 is not initialized. Please check the control's designer.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = null;
            try
            {
                connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    // MessageBox.Show("Connection string 'MySqlConnection' not found or is empty.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit if connection string is not valid
                }
            }
            catch (Exception)
            {
                // MessageBox.Show("Error accessing connection string: " + ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit on configuration error
            }


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT g.genre_name, COUNT(bga.book_id) AS GenreCount
                        FROM book_genre_assignment bga
                        INNER JOIN genres g ON bga.genre_id = g.genre_id
                        GROUP BY g.genre_name
                        ORDER BY GenreCount DESC
                        LIMIT 5";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    List<string> genres = new List<string>();
                    List<double> genreCounts = new List<double>();

                    while (reader.Read())
                    {
                        // Validate data before adding to lists
                        if (!reader.IsDBNull(reader.GetOrdinal("genre_name")) && !reader.IsDBNull(reader.GetOrdinal("GenreCount")))
                        {
                            genres.Add(reader.GetString("genre_name"));
                            genreCounts.Add(reader.GetDouble("GenreCount"));
                        }
                    }

                    reader.Close();

                    gunaDoughnutDataset1.DataPoints.Clear();

                    for (int i = 0; i < genres.Count; i++)
                    {
                        gunaDoughnutDataset1.DataPoints.Add(new Guna.Charts.WinForms.LPoint(genres[i], genreCounts[i]));
                    }

                    gunaChart2.Datasets.Clear();
                    if (!gunaChart2.Datasets.Contains(gunaDoughnutDataset1))
                    {
                        gunaChart2.Datasets.Add(gunaDoughnutDataset1);
                    }

                    gunaChart2.Update();
                }
                catch (Exception ex)
                {
                    if (!IsInDesignMode())
                    {
                        MessageBox.Show("An error occurred while loading the chart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Ensure this is present
using book_review.Helpers;    // Assuming SharedData will be used for genre count later

namespace book_review
{
    public partial class genreUserControl : UserControl
    {
        private int? _selectedGenreId = null; // To store the ID of the selected genre

        public genreUserControl()
        {
            InitializeComponent();
            if (!IsInDesignMode())
            {
                // The UserControl8_Load event handler is already connected in your provided snippet.
                // We'll use that, or you can rename it to genreUserControl_Load if you prefer.
                // this.Load += genreUserControl_Load; // If you rename UserControl8_Load
                if (guna2DataGridView1 != null)
                {
                    guna2DataGridView1.CellClick += guna2DataGridView1_CellClick;
                }
            }
        }

        private bool IsInDesignMode()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return true;
            if (this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) != null)
                return true;
            Control parent = this.Parent;
            while (parent != null)
            {
                if (parent.Site != null && parent.Site.DesignMode)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }

        // This is the existing Load event handler from your snippet
        private void UserControl8_Load(object sender, EventArgs e)
        {
            if (!IsInDesignMode())
            {
                LoadGenreData();
                LoadGenresIntoProcedureComboBox(); // For guna2ComboBox1 (for the stored procedure)
            }
        }
        private void LoadGenresIntoProcedureComboBox()
        {
            if (IsInDesignMode() || guna2ComboBox1 == null) return;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            guna2ComboBox1.Items.Clear();
            // Add a placeholder item
            guna2ComboBox1.Items.Add(new reviewsUserControl.ComboboxItem { Text = "-- Select Genre --", Value = DBNull.Value });


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT genre_id, genre_name FROM genres ORDER BY genre_name;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            guna2ComboBox1.Items.Add(new reviewsUserControl.ComboboxItem { Text = reader["genre_name"].ToString(), Value = reader["genre_id"] });
                        }
                    }
                    if (guna2ComboBox1.Items.Count > 0)
                    {
                        guna2ComboBox1.SelectedIndex = 0; // Select the placeholder
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading genres into ComboBox: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FetchBooksByGenreProcedure(int genreId, int limitCount)
        {
            if (IsInDesignMode()) return;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string 'MySqlConnection' not found.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable dtBooksByGenre = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("GetBooksByGenre", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("genre_id", MySqlDbType.Int32)).Value = genreId;
                    cmd.Parameters.Add(new MySqlParameter("limit_count", MySqlDbType.Int32)).Value = limitCount;
                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dtBooksByGenre);
                        }

                        if (guna2DataGridView2 != null)
                        {
                            guna2DataGridView2.DataSource = null;
                            guna2DataGridView2.AutoGenerateColumns = false;

                            if (guna2DataGridView2.Columns["procBookIdCol"] == null)
                            {
                                guna2DataGridView2.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "procBookIdCol",
                                    DataPropertyName = "book_id", // Matches column from procedure's SELECT
                                    HeaderText = "Book ID"
                                });
                            }
                            if (guna2DataGridView2.Columns["procBookTitleCol"] == null)
                            {
                                guna2DataGridView2.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "procBookTitleCol",
                                    DataPropertyName = "book_title", 
                                    HeaderText = "Book Title",
                                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                                });
                            }
                            guna2DataGridView2.DataSource = dtBooksByGenre;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Database error calling GetBooksByGenre: " + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void LoadGenreData(string searchTerm = null)
        {
            if (IsInDesignMode()) return;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string 'MySqlConnection' not found or is empty.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT genre_id, genre_name FROM genres";

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query += " WHERE genre_name LIKE @searchTerm";
                    }
                    query += " ORDER BY genre_name;";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    }

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (guna2DataGridView1 != null)
                    {
                        guna2DataGridView1.AutoGenerateColumns = false;
                        if (guna2DataGridView1.Columns.Count == 0) // Only add columns if they don't exist
                        {
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "genre_id", HeaderText = "ID", Name = "genre_id", Visible = true });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "genre_name", HeaderText = "Genre Name", Name = "genre_name" });
                            guna2DataGridView1.Columns["genre_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                        guna2DataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading genre data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            guna2TextBox1.Clear(); // Genre Name
            _selectedGenreId = null;
            if (guna2DataGridView1 != null)
            {
                guna2DataGridView1.ClearSelection();
            }
        }

        private void PopulateInputFieldsFromRow(DataGridViewRow row)
        {
            if (row == null || row.Cells["genre_id"].Value == null)
            {
                ClearInputFields();
                return;
            }
            _selectedGenreId = Convert.ToInt32(row.Cells["genre_id"].Value);
            guna2TextBox1.Text = row.Cells["genre_name"].Value?.ToString();
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && guna2DataGridView1.Rows[e.RowIndex].Cells["genre_id"].Value != null)
            {
                PopulateInputFieldsFromRow(guna2DataGridView1.Rows[e.RowIndex]);
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e) // SEARCH GENRE
        {
            if (IsInDesignMode()) return;
            string searchTerm = guna2TextBox1.Text.Trim();
            LoadGenreData(searchTerm);

            if (guna2DataGridView1.Rows.Count > 0)
            {
                guna2DataGridView1.ClearSelection();
                guna2DataGridView1.Rows[0].Selected = true;
                if (guna2DataGridView1.Columns.GetFirstColumn(DataGridViewElementStates.Visible) != null)
                {
                    guna2DataGridView1.CurrentCell = guna2DataGridView1.Rows[0].Cells[guna2DataGridView1.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index];
                }
                PopulateInputFieldsFromRow(guna2DataGridView1.Rows[0]);
            }
            else
            {
                ClearInputFields();
                _selectedGenreId = null;
                MessageBox.Show("No genres found matching your search.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) // ADD GENRE
        {
            if (IsInDesignMode()) return;

            string genreName = guna2TextBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(genreName))
            {
                MessageBox.Show("Genre name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM genres WHERE genre_name = @genre_name;";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@genre_name", genreName);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("A genre with this name already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = "INSERT INTO genres (genre_name) VALUES (@genre_name);";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@genre_name", genreName);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Genre added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // TODO: If you add GenreCount to SharedData, refresh it here:
                    // SharedData.RefreshGenreCount();
                    // And notify the dashboard if needed:
                    // if (this.ParentForm is MainForm mainForm) mainForm.UpdateDashboardGenreCount();

                    LoadGenreData();
                    ClearInputFields();
                    LoadGenresIntoProcedureComboBox();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding genre: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) // UPDATE GENRE
        {
            if (IsInDesignMode()) return;
            if (!_selectedGenreId.HasValue)
            {
                MessageBox.Show("Please select a genre to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string genreName = guna2TextBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(genreName))
            {
                MessageBox.Show("Genre name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM genres WHERE genre_name = @genre_name AND genre_id != @genre_id;";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@genre_name", genreName);
                    checkCmd.Parameters.AddWithValue("@genre_id", _selectedGenreId.Value);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Another genre with this name already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = "UPDATE genres SET genre_name = @genre_name WHERE genre_id = @genre_id;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@genre_name", genreName);
                    cmd.Parameters.AddWithValue("@genre_id", _selectedGenreId.Value);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Genre updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGenreData();
                    ClearInputFields();
                    LoadGenresIntoProcedureComboBox();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating genre: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) // DELETE GENRE
        {
            if (IsInDesignMode()) return;
            if (!_selectedGenreId.HasValue)
            {
                MessageBox.Show("Please select a genre to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this genre? \nThis might affect books associated with this genre.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString)) return;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    MySqlTransaction transaction = null;
                    try
                    {
                        conn.Open();
                        transaction = conn.BeginTransaction();

                        string checkBooksQuery = "SELECT COUNT(*) FROM book_genre_assignment WHERE genre_id = @genre_id;";
                        MySqlCommand checkBooksCmd = new MySqlCommand(checkBooksQuery, conn, transaction);
                        checkBooksCmd.Parameters.AddWithValue("@genre_id", _selectedGenreId.Value);
                        int associatedBooksCount = Convert.ToInt32(checkBooksCmd.ExecuteScalar());

                        if (associatedBooksCount > 0)
                        {
                            if (MessageBox.Show($"This genre is associated with {associatedBooksCount} book(s). Deleting this genre will also remove these associations. Do you want to proceed?", "Dependency Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            {
                                transaction.Rollback();
                                return;
                            }
                            string deleteAssociationsQuery = "DELETE FROM book_genre_assignment WHERE genre_id = @genre_id;";
                            MySqlCommand deleteAssocCmd = new MySqlCommand(deleteAssociationsQuery, conn, transaction);
                            deleteAssocCmd.Parameters.AddWithValue("@genre_id", _selectedGenreId.Value);
                            deleteAssocCmd.ExecuteNonQuery();
                        }

                        string query = "DELETE FROM genres WHERE genre_id = @genre_id;";
                        MySqlCommand cmd = new MySqlCommand(query, conn, transaction);
                        cmd.Parameters.AddWithValue("@genre_id", _selectedGenreId.Value);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();

                        MessageBox.Show("Genre deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // TODO: If you add GenreCount to SharedData, refresh it here:
                        // SharedData.RefreshGenreCount();
                        // And notify the dashboard if needed:
                        // if (this.ParentForm is MainForm mainForm) mainForm.UpdateDashboardGenreCount();

                        LoadGenreData();
                        ClearInputFields();
                        LoadGenresIntoProcedureComboBox();
                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show("Error deleting genre: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            // Genre Name TextBox - No specific validation here on text changed.
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(guna2TextBox2.Text))
            {
                if (!int.TryParse(guna2TextBox2.Text, out _))
                {
                    // Optionally provide immediate feedback, e.g., change border color
                    // guna2TextBox2.BorderColor = Color.Red;
                }
                else
                {
                    // guna2TextBox2.BorderColor = Color.Silver; // Or your default color
                }
            }
        }

        private void guna2DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2ButtonFetchBooksByGenreProc_Click(object sender, EventArgs e)
        {
            if (IsInDesignMode()) return;

            int selectedGenreIdForProc = 0;
            if (guna2ComboBox1.SelectedItem is reviewsUserControl.ComboboxItem selectedItem && selectedItem.Value != DBNull.Value)
            {
                selectedGenreIdForProc = Convert.ToInt32(selectedItem.Value);
            }
            else
            {
                MessageBox.Show("Please select a genre from the dropdown.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2DataGridView2.DataSource = null; // Clear previous results
                return;
            }

            if (int.TryParse(guna2TextBox2.Text, out int limitCount))
            {
                if (limitCount <= 0)
                {
                    MessageBox.Show("Limit count must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    guna2DataGridView2.DataSource = null; // Clear previous results
                    return;
                }
                FetchBooksByGenreProcedure(selectedGenreIdForProc, limitCount);
            }
            else
            {
                MessageBox.Show("Please enter a valid number for the limit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2DataGridView2.DataSource = null; // Clear previous results
            }
        }
    }
}
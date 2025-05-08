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
using book_review.Helpers; 

namespace book_review
{
    public partial class reviewsUserControl : UserControl
    {
        private int? _selectedReviewId = null;

        // Helper class for ComboBox items (can be moved to a Helpers folder/namespace)
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() { return Text; }
        }
        public event EventHandler ReviewDataChanged; 
        protected virtual void OnReviewDataChanged(EventArgs e)
        {
            ReviewDataChanged?.Invoke(this, e);
        }

        public reviewsUserControl()
        {
            InitializeComponent();
            if (!IsInDesignMode())
            {
                this.Load += reviewsUserControl_Load; // Connect to a new Load event handler
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

        private void reviewsUserControl_Load(object sender, EventArgs e)
        {
            LoadUsersToComboBox();
            LoadBooksToComboBox();
            LoadReviewsData();
        }

        private void LoadUsersToComboBox()
        {
            if (IsInDesignMode() || guna2ComboBox2 == null) return;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add(new ComboboxItem { Text = "-- Select User --", Value = DBNull.Value }); // Optional placeholder

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT user_id, username FROM users ORDER BY username;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            guna2ComboBox2.Items.Add(new ComboboxItem { Text = reader["username"].ToString(), Value = reader["user_id"] });
                        }
                    }
                    guna2ComboBox2.SelectedIndex = 0; // Select placeholder
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading users: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadBooksToComboBox()
        {
            if (IsInDesignMode() || guna2ComboBox1 == null) return;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add(new ComboboxItem { Text = "-- Select Book --", Value = DBNull.Value }); // Optional placeholder

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT book_id, book_title FROM books ORDER BY book_title;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            guna2ComboBox1.Items.Add(new ComboboxItem { Text = reader["book_title"].ToString(), Value = reader["book_id"] });
                        }
                    }
                    guna2ComboBox1.SelectedIndex = 0; // Select placeholder
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading books: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadReviewsData(int? searchUserId = null, int? searchBookId = null)
        {
            if (IsInDesignMode()) return;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string 'MySqlConnection' not found.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            r.review_id, 
                            b.book_title, 
                            u.username, 
                            r.rating, 
                            r.review_comment, 
                            r.created_at, 
                            r.updated_at,
                            r.book_id,  -- Keep original IDs for populating ComboBoxes on selection
                            r.user_id   -- Keep original IDs for populating ComboBoxes on selection
                        FROM reviews r
                        JOIN books b ON r.book_id = b.book_id
                        JOIN users u ON r.user_id = u.user_id
                    ";

                    List<string> conditions = new List<string>();
                    MySqlCommand cmd = new MySqlCommand(); // Initialize here

                    if (searchUserId.HasValue)
                    {
                        conditions.Add("r.user_id = @searchUserId");
                        cmd.Parameters.AddWithValue("@searchUserId", searchUserId.Value);
                    }
                    if (searchBookId.HasValue)
                    {
                        conditions.Add("r.book_id = @searchBookId");
                        cmd.Parameters.AddWithValue("@searchBookId", searchBookId.Value);
                    }

                    if (conditions.Any())
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                    }
                    query += " ORDER BY r.created_at DESC;";

                    cmd.Connection = conn; // Assign connection
                    cmd.CommandText = query; // Assign command text

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (guna2DataGridView1 != null)
                    {
                        guna2DataGridView1.AutoGenerateColumns = false;
                        if (guna2DataGridView1.Columns.Count == 0)
                        {
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "review_id", HeaderText = "ID", Name = "review_id" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "book_title", HeaderText = "Book", Name = "book_title" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "username", HeaderText = "User", Name = "username" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "rating", HeaderText = "Rating", Name = "rating" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "review_comment", HeaderText = "Comment", Name = "review_comment", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "created_at", HeaderText = "Created", Name = "created_at", DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" } });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "updated_at", HeaderText = "Updated", Name = "updated_at", DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" } });
                            // Hidden columns to store original IDs for ComboBox selection
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "book_id", HeaderText = "BookID_Hidden", Name = "book_id_hidden", Visible = false });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "user_id", HeaderText = "UserID_Hidden", Name = "user_id_hidden", Visible = false });
                        }
                        guna2DataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading reviews: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            _selectedReviewId = null;
            guna2ComboBox2.SelectedIndex = 0; // Reset to placeholder or first item
            guna2ComboBox1.SelectedIndex = 0; // Reset to placeholder or first item
            guna2TextBox1.Clear(); // Comment
            guna2TextBox2.Clear(); // Rating
            if (guna2DataGridView1 != null) guna2DataGridView1.ClearSelection();
        }

        private void PopulateInputFieldsFromRow(DataGridViewRow row)
        {
            if (row == null || row.Cells["review_id"].Value == null)
            {
                ClearInputFields();
                return;
            }
            _selectedReviewId = Convert.ToInt32(row.Cells["review_id"].Value);

            // Select User in ComboBox
            if (row.Cells["user_id_hidden"].Value != DBNull.Value && row.Cells["user_id_hidden"].Value != null) // Check for DBNull before converting row value
            {
                int userIdToSelect = Convert.ToInt32(row.Cells["user_id_hidden"].Value);
                for (int i = 0; i < guna2ComboBox2.Items.Count; i++)
                {
                    if (guna2ComboBox2.Items[i] is ComboboxItem item)
                    {
                        // Check if the item's value is not DBNull before trying to convert and compare
                        if (item.Value != DBNull.Value && item.Value != null)
                        {
                            if (Convert.ToInt32(item.Value) == userIdToSelect)
                            {
                                guna2ComboBox2.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Handle case where user_id_hidden is null or DBNull, perhaps select the placeholder
                if (guna2ComboBox2.Items.Count > 0) guna2ComboBox2.SelectedIndex = 0;
            }


            // Select Book in ComboBox
            if (row.Cells["book_id_hidden"].Value != DBNull.Value && row.Cells["book_id_hidden"].Value != null) // Check for DBNull before converting row value
            {
                int bookIdToSelect = Convert.ToInt32(row.Cells["book_id_hidden"].Value);
                for (int i = 0; i < guna2ComboBox1.Items.Count; i++)
                {
                    if (guna2ComboBox1.Items[i] is ComboboxItem item)
                    {
                        // Check if the item's value is not DBNull before trying to convert and compare
                        if (item.Value != DBNull.Value && item.Value != null)
                        {
                            if (Convert.ToInt32(item.Value) == bookIdToSelect)
                            {
                                guna2ComboBox1.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Handle case where book_id_hidden is null or DBNull
                if (guna2ComboBox1.Items.Count > 0) guna2ComboBox1.SelectedIndex = 0;
            }

            guna2TextBox1.Text = row.Cells["review_comment"].Value?.ToString();
            guna2TextBox2.Text = row.Cells["rating"].Value?.ToString();
        }
        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && guna2DataGridView1.Rows[e.RowIndex].Cells["review_id"].Value != null)
            {
                PopulateInputFieldsFromRow(guna2DataGridView1.Rows[e.RowIndex]);
            }
        }

        // Original CellContentClick, can be removed if CellClick handles all needs
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }


        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e) { /* User selection changed */ }
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) { /* Book selection changed */ }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { /* Comment changed */ }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) // Rating
        {
            if (!string.IsNullOrWhiteSpace(guna2TextBox2.Text))
            {
                if (int.TryParse(guna2TextBox2.Text, out int ratingValue))
                {
                    if (ratingValue >= 1 && ratingValue <= 5)
                    {
                        guna2TextBox2.ForeColor = Color.Black;
                    }
                    else
                    {
                        // guna2TextBox2.ForeColor = Color.Red; // Visual cue
                    }
                }
                else
                {
                    // guna2TextBox2.ForeColor = Color.Red; // Visual cue
                }
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e) // SEARCH
        {
            if (IsInDesignMode()) return;

            int? searchUserId = null;
            if (guna2ComboBox2.SelectedItem is ComboboxItem userItem && userItem.Value != DBNull.Value)
            {
                searchUserId = Convert.ToInt32(userItem.Value);
            }

            int? searchBookId = null;
            if (guna2ComboBox1.SelectedItem is ComboboxItem bookItem && bookItem.Value != DBNull.Value)
            {
                searchBookId = Convert.ToInt32(bookItem.Value);
            }

            if (!searchUserId.HasValue && !searchBookId.HasValue)
            {
                // Load all if no specific search criteria from comboboxes, or prompt user
                LoadReviewsData();
                // MessageBox.Show("Please select a user or a book to search by.", "Search Criteria", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // return;
            }
            else
            {
                LoadReviewsData(searchUserId, searchBookId);
            }


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
                _selectedReviewId = null;
                MessageBox.Show("No reviews found matching your criteria.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) // ADD REVIEW
        {
            if (IsInDesignMode()) return;

            if (!(guna2ComboBox2.SelectedItem is ComboboxItem selectedUserItem) || selectedUserItem.Value == DBNull.Value)
            {
                MessageBox.Show("Please select a user.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int userId = Convert.ToInt32(selectedUserItem.Value);

            if (!(guna2ComboBox1.SelectedItem is ComboboxItem selectedBookItem) || selectedBookItem.Value == DBNull.Value)
            {
                MessageBox.Show("Please select a book.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int bookId = Convert.ToInt32(selectedBookItem.Value);

            if (!int.TryParse(guna2TextBox2.Text, out int rating) || rating < 1 || rating > 5)
            {
                MessageBox.Show("Rating must be a number between 1 and 5.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string comment = guna2TextBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(comment))
            {
                MessageBox.Show("Review comment cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Optional: Check if the user has already reviewed this book
                    string checkQuery = "SELECT COUNT(*) FROM reviews WHERE user_id = @user_id AND book_id = @book_id;";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@user_id", userId);
                    checkCmd.Parameters.AddWithValue("@book_id", bookId);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("This user has already reviewed this book.", "Duplicate Review", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    string query = "INSERT INTO reviews (book_id, user_id, rating, review_comment, created_at, updated_at) VALUES (@book_id, @user_id, @rating, @comment, NOW(), NOW());";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@book_id", bookId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@rating", rating);
                    cmd.Parameters.AddWithValue("@comment", comment);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Review added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadReviewsData();
                    ClearInputFields();
                    SharedData.RefreshReviewCount();
                    ((Dashboard)this.ParentForm)?.DashboardControl.UpdateReviewCountLabel();
                    OnReviewDataChanged(EventArgs.Empty); // Raise the event
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding review: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) // UPDATE REVIEW
        {
            if (IsInDesignMode()) return;
            if (!_selectedReviewId.HasValue)
            {
                MessageBox.Show("Please select a review to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // User and Book for a review typically don't change. If they can, add ComboBox validation here.
            // For this example, we assume only rating and comment can be updated.
            // int userId = Convert.ToInt32(((ComboboxItem)guna2ComboBox2.SelectedItem).Value);
            // int bookId = Convert.ToInt32(((ComboboxItem)guna2ComboBox1.SelectedItem).Value);

            if (!int.TryParse(guna2TextBox2.Text, out int rating) || rating < 1 || rating > 5)
            {
                MessageBox.Show("Rating must be a number between 1 and 5.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string comment = guna2TextBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(comment))
            {
                MessageBox.Show("Review comment cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE reviews SET rating = @rating, review_comment = @comment, updated_at = NOW() WHERE review_id = @review_id;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@rating", rating);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    cmd.Parameters.AddWithValue("@review_id", _selectedReviewId.Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Review updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadReviewsData();
                    ClearInputFields();
                    OnReviewDataChanged(EventArgs.Empty); // Raise the event
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating review: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) // DELETE REVIEW
        {
            if (IsInDesignMode()) return;
            if (!_selectedReviewId.HasValue)
            {
                MessageBox.Show("Please select a review to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this review?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString)) return;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM reviews WHERE review_id = @review_id;";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@review_id", _selectedReviewId.Value);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Review deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadReviewsData();
                        ClearInputFields();
                        SharedData.RefreshReviewCount();
                        ((Dashboard)this.ParentForm)?.DashboardControl.UpdateReviewCountLabel();
                        OnReviewDataChanged(EventArgs.Empty); // Raise the event
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting review: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
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
using System.Globalization; // For DateTime parsing
using System.Text.RegularExpressions; // For ISBN validation
using book_review.Helpers;


namespace book_review
{
    public partial class booksUserControl : UserControl
    {
        private int? _selectedBookId = null; // To store the ID of the selected book

        public booksUserControl()
        {
            InitializeComponent();
            if (!this.DesignMode)
            {
                this.Load += booksUserControl_Load;
                // Assuming guna2DataGridView1 is the name of your Guna DataGridView
                if (guna2DataGridView1 != null)
                {
                    guna2DataGridView1.CellClick += guna2DataGridView1_CellClick;
                }
            }
        }

        private void booksUserControl_Load(object sender, EventArgs e)
        {
            LoadBookData();
            LoadAuthorsToCheckListBox();
            LoadGenresToCheckListBox();
        }

        private bool IsInDesignMode()
        {
            return DesignMode || this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) != null || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }
        public void RefreshBookGridData()
        {
            if (!IsInDesignMode()) // Ensure IsInDesignMode() is defined in this class too
            {
                LoadBookData();
            }
        }

        private void LoadBookData(string searchTerm = null)
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
                    string query = @"
                        SELECT 
                            b.book_id, 
                            b.book_title,
                            b.published_date, 
                            b.book_isbn, 
                            b.average_rating, 
                            b.total_reviews, 
                            b.popularity_score,
                            GROUP_CONCAT(DISTINCT a.author_name SEPARATOR ', ') AS authors,
                            GROUP_CONCAT(DISTINCT g.genre_name SEPARATOR ', ') AS genres
                        FROM books b
                        LEFT JOIN book_authorships ba ON b.book_id = ba.book_id
                        LEFT JOIN authors a ON ba.author_id = a.author_id
                        LEFT JOIN book_genre_assignment bga ON b.book_id = bga.book_id
                        LEFT JOIN genres g ON bga.genre_id = g.genre_id
                    ";

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query += " WHERE b.book_title LIKE @searchTerm ";
                    }

                    query += " GROUP BY b.book_id, b.book_title, b.published_date, b.book_isbn, b.average_rating, b.total_reviews, b.popularity_score ORDER BY b.book_title;";

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
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "book_id", HeaderText = "ID", Name = "book_id", Visible = true }); // Often useful to have ID, can be hidden
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "book_title", HeaderText = "Title", Name = "book_title" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "authors", HeaderText = "Authors", Name = "authors" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "genres", HeaderText = "Genres", Name = "genres" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "published_date", HeaderText = "Published", Name = "published_date", DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "book_isbn", HeaderText = "ISBN", Name = "book_isbn" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "average_rating", HeaderText = "Rating", Name = "average_rating" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "total_reviews", HeaderText = "Reviews", Name = "total_reviews" });
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "popularity_score", HeaderText = "Popularity", Name = "popularity_score" });
                            guna2DataGridView1.Columns["book_title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                        guna2DataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading book data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadAuthorsToCheckListBox()
        {
            if (IsInDesignMode() || chkListBoxAuthors == null) return;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            chkListBoxAuthors.Items.Clear();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT author_id, author_name FROM authors ORDER BY author_name;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chkListBoxAuthors.Items.Add(new ComboboxItem { Text = reader["author_name"].ToString(), Value = reader["author_id"] });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading authors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadGenresToCheckListBox()
        {
            if (IsInDesignMode() || chkListBoxGenres == null) return;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            chkListBoxGenres.Items.Clear();
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
                            chkListBoxGenres.Items.Add(new ComboboxItem { Text = reader["genre_name"].ToString(), Value = reader["genre_id"] });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading genres: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Helper class for ComboBox/CheckedListBox items
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() { return Text; }
        }

        private List<int> GetSelectedIdsFromCheckedListBox(CheckedListBox clb)
        {
            List<int> selectedIds = new List<int>();
            foreach (var item in clb.CheckedItems)
            {
                if (item is ComboboxItem cbItem)
                {
                    selectedIds.Add(Convert.ToInt32(cbItem.Value));
                }
            }
            return selectedIds;
        }

        private void SetSelectedItemsInCheckedListBox(CheckedListBox clb, int bookId, string idColumnName, string tableName)
        {
            if (IsInDesignMode() || clb == null) return;

            for (int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemChecked(i, false);
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            List<int> associatedIds = new List<int>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT {idColumnName} FROM {tableName} WHERE book_id = @bookId;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@bookId", bookId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            associatedIds.Add(reader.GetInt32(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading associated {idColumnName}s: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            for (int i = 0; i < clb.Items.Count; i++)
            {
                if (clb.Items[i] is ComboboxItem cbItem)
                {
                    if (associatedIds.Contains(Convert.ToInt32(cbItem.Value)))
                    {
                        clb.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && guna2DataGridView1.Rows[e.RowIndex].Cells["book_id"].Value != null)
            {
                PopulateInputFieldsFromRow(guna2DataGridView1.Rows[e.RowIndex]);
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            // Book Title - No specific validation here, but can be added if needed
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            // Published Date Validation (YYYY-MM-DD)
            // This is a simple visual cue; actual parsing happens on save/update.
            if (!string.IsNullOrWhiteSpace(guna2TextBox2.Text))
            {
                if (DateTime.TryParseExact(guna2TextBox2.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    guna2TextBox2.ForeColor = Color.Black;
                }
                else
                {
                }
            }
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            // ISBN Validation (13 digits)
            // This is a simple visual cue; actual validation happens on save/update.
            if (!string.IsNullOrWhiteSpace(guna2TextBox3.Text))
            {
                if (Regex.IsMatch(guna2TextBox3.Text, @"^\d{13}$"))
                {
                    guna2TextBox3.ForeColor = Color.Black;
                }
                else
                {
                }
            }
        }

       
        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e) // Authors
        {
            // This would be for a single selection ComboBox. 
            // For multiple authors, use chkListBoxAuthors.
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) // Genres
        {
            // This would be for a single selection ComboBox.
            // For multiple genres, use chkListBoxGenres.
        }

        private void ClearInputFields()
        {
            guna2TextBox1.Clear(); // Title
            guna2TextBox2.Clear(); // Published Date
            guna2TextBox3.Clear(); // ISBN
            _selectedBookId = null;

            if (chkListBoxAuthors != null)
            {
                for (int i = 0; i < chkListBoxAuthors.Items.Count; i++)
                    chkListBoxAuthors.SetItemChecked(i, false);
            }
            if (chkListBoxGenres != null)
            {
                for (int i = 0; i < chkListBoxGenres.Items.Count; i++)
                    chkListBoxGenres.SetItemChecked(i, false);
            }
            guna2DataGridView1.ClearSelection();
        }

        private void guna2Button1_Click(object sender, EventArgs e) // ADD BOOK
        {
            if (IsInDesignMode()) return;

            string title = guna2TextBox1.Text.Trim();
            string publishedDateStr = guna2TextBox2.Text.Trim();
            string isbn = guna2TextBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Book title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!DateTime.TryParseExact(publishedDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime publishedDate))
            {
                MessageBox.Show("Invalid published date format. Please use YYYY-MM-DD.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!string.IsNullOrWhiteSpace(isbn) && !Regex.IsMatch(isbn, @"^\d{13}$")) // ISBN is optional or must be 13 digits
            {
                MessageBox.Show("ISBN must be 13 digits if provided.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<int> selectedAuthorIds = GetSelectedIdsFromCheckedListBox(chkListBoxAuthors);
            List<int> selectedGenreIds = GetSelectedIdsFromCheckedListBox(chkListBoxGenres);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    string queryBook = "INSERT INTO books (book_title, published_date, book_isbn) VALUES (@title, @published_date, @isbn); SELECT LAST_INSERT_ID();";
                    MySqlCommand cmdBook = new MySqlCommand(queryBook, conn, transaction);
                    cmdBook.Parameters.AddWithValue("@title", title);
                    cmdBook.Parameters.AddWithValue("@published_date", publishedDate);
                    cmdBook.Parameters.AddWithValue("@isbn", string.IsNullOrWhiteSpace(isbn) ? (object)DBNull.Value : isbn);

                    long newBookId = Convert.ToInt64(cmdBook.ExecuteScalar());

                    // Insert Authorships
                    if (selectedAuthorIds.Any())
                    {
                        StringBuilder authorQueryBuilder = new StringBuilder("INSERT INTO book_authorships (book_id, author_id) VALUES ");
                        for (int i = 0; i < selectedAuthorIds.Count; i++)
                        {
                            authorQueryBuilder.Append($"(@book_id_auth_{i}, @author_id_{i})");
                            if (i < selectedAuthorIds.Count - 1) authorQueryBuilder.Append(", ");
                        }
                        MySqlCommand cmdAuthors = new MySqlCommand(authorQueryBuilder.ToString(), conn, transaction);
                        for (int i = 0; i < selectedAuthorIds.Count; i++)
                        {
                            cmdAuthors.Parameters.AddWithValue($"@book_id_auth_{i}", newBookId);
                            cmdAuthors.Parameters.AddWithValue($"@author_id_{i}", selectedAuthorIds[i]);
                        }
                        cmdAuthors.ExecuteNonQuery();
                    }

                    // Insert Genres
                    if (selectedGenreIds.Any())
                    {
                        StringBuilder genreQueryBuilder = new StringBuilder("INSERT INTO book_genre_assignment (book_id, genre_id) VALUES ");
                        for (int i = 0; i < selectedGenreIds.Count; i++)
                        {
                            genreQueryBuilder.Append($"(@book_id_genre_{i}, @genre_id_{i})");
                            if (i < selectedGenreIds.Count - 1) genreQueryBuilder.Append(", ");
                        }
                        MySqlCommand cmdGenres = new MySqlCommand(genreQueryBuilder.ToString(), conn, transaction);
                        for (int i = 0; i < selectedGenreIds.Count; i++)
                        {
                            cmdGenres.Parameters.AddWithValue($"@book_id_genre_{i}", newBookId);
                            cmdGenres.Parameters.AddWithValue($"@genre_id_{i}", selectedGenreIds[i]);
                        }
                        cmdGenres.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBookData();
                    ClearInputFields();
                    SharedData.RefreshBookCount();
                    ((Dashboard)this.ParentForm)?.DashboardControl.UpdateBookCountLabel();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    MessageBox.Show("Error adding book: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) // UPDATE BOOK
        {
            if (IsInDesignMode()) return;
            if (!_selectedBookId.HasValue)
            {
                MessageBox.Show("Please select a book to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string title = guna2TextBox1.Text.Trim();
            string publishedDateStr = guna2TextBox2.Text.Trim();
            string isbn = guna2TextBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Book title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!DateTime.TryParseExact(publishedDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime publishedDate))
            {
                MessageBox.Show("Invalid published date format. Please use YYYY-MM-DD.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!string.IsNullOrWhiteSpace(isbn) && !Regex.IsMatch(isbn, @"^\d{13}$"))
            {
                MessageBox.Show("ISBN must be 13 digits if provided.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<int> selectedAuthorIds = GetSelectedIdsFromCheckedListBox(chkListBoxAuthors);
            List<int> selectedGenreIds = GetSelectedIdsFromCheckedListBox(chkListBoxGenres);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    // Update Book Details
                    string queryBook = "UPDATE books SET book_title = @title, published_date = @published_date, book_isbn = @isbn WHERE book_id = @book_id;";
                    MySqlCommand cmdBook = new MySqlCommand(queryBook, conn, transaction);
                    cmdBook.Parameters.AddWithValue("@title", title);
                    cmdBook.Parameters.AddWithValue("@published_date", publishedDate);
                    cmdBook.Parameters.AddWithValue("@isbn", string.IsNullOrWhiteSpace(isbn) ? (object)DBNull.Value : isbn);
                    cmdBook.Parameters.AddWithValue("@book_id", _selectedBookId.Value);
                    cmdBook.ExecuteNonQuery();

                    // Update Authorships (Delete old, Insert new)
                    MySqlCommand cmdDeleteAuthors = new MySqlCommand("DELETE FROM book_authorships WHERE book_id = @book_id;", conn, transaction);
                    cmdDeleteAuthors.Parameters.AddWithValue("@book_id", _selectedBookId.Value);
                    cmdDeleteAuthors.ExecuteNonQuery();

                    if (selectedAuthorIds.Any())
                    {
                        StringBuilder authorQueryBuilder = new StringBuilder("INSERT INTO book_authorships (book_id, author_id) VALUES ");
                        for (int i = 0; i < selectedAuthorIds.Count; i++)
                        {
                            authorQueryBuilder.Append($"(@book_id_auth_{i}, @author_id_{i})");
                            if (i < selectedAuthorIds.Count - 1) authorQueryBuilder.Append(", ");
                        }
                        MySqlCommand cmdAuthors = new MySqlCommand(authorQueryBuilder.ToString(), conn, transaction);
                        for (int i = 0; i < selectedAuthorIds.Count; i++)
                        {
                            cmdAuthors.Parameters.AddWithValue($"@book_id_auth_{i}", _selectedBookId.Value);
                            cmdAuthors.Parameters.AddWithValue($"@author_id_{i}", selectedAuthorIds[i]);
                        }
                        cmdAuthors.ExecuteNonQuery();
                    }

                    // Update Genres (Delete old, Insert new)
                    MySqlCommand cmdDeleteGenres = new MySqlCommand("DELETE FROM book_genre_assignment WHERE book_id = @book_id;", conn, transaction);
                    cmdDeleteGenres.Parameters.AddWithValue("@book_id", _selectedBookId.Value);
                    cmdDeleteGenres.ExecuteNonQuery();

                    if (selectedGenreIds.Any())
                    {
                        StringBuilder genreQueryBuilder = new StringBuilder("INSERT INTO book_genre_assignment (book_id, genre_id) VALUES ");
                        for (int i = 0; i < selectedGenreIds.Count; i++)
                        {
                            genreQueryBuilder.Append($"(@book_id_genre_{i}, @genre_id_{i})");
                            if (i < selectedGenreIds.Count - 1) genreQueryBuilder.Append(", ");
                        }
                        MySqlCommand cmdGenres = new MySqlCommand(genreQueryBuilder.ToString(), conn, transaction);
                        for (int i = 0; i < selectedGenreIds.Count; i++)
                        {
                            cmdGenres.Parameters.AddWithValue($"@book_id_genre_{i}", _selectedBookId.Value);
                            cmdGenres.Parameters.AddWithValue($"@genre_id_{i}", selectedGenreIds[i]);
                        }
                        cmdGenres.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBookData();
                    ClearInputFields();
                    SharedData.RefreshBookCount();
                    ((Dashboard)this.ParentForm)?.DashboardControl.UpdateBookCountLabel();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    MessageBox.Show("Error updating book: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) // DELETE BOOK
        {
            if (IsInDesignMode()) return;
            if (!_selectedBookId.HasValue)
            {
                MessageBox.Show("Please select a book to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this book? This will also remove its author and genre associations.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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

                        // Delete from book_authorships
                        MySqlCommand cmdDelAuthors = new MySqlCommand("DELETE FROM book_authorships WHERE book_id = @book_id;", conn, transaction);
                        cmdDelAuthors.Parameters.AddWithValue("@book_id", _selectedBookId.Value);
                        cmdDelAuthors.ExecuteNonQuery();

                        // Delete from book_genre_assignment
                        MySqlCommand cmdDelGenres = new MySqlCommand("DELETE FROM book_genre_assignment WHERE book_id = @book_id;", conn, transaction);
                        cmdDelGenres.Parameters.AddWithValue("@book_id", _selectedBookId.Value);
                        cmdDelGenres.ExecuteNonQuery();

                        // Delete from books
                        MySqlCommand cmdDelBook = new MySqlCommand("DELETE FROM books WHERE book_id = @book_id;", conn, transaction);
                        cmdDelBook.Parameters.AddWithValue("@book_id", _selectedBookId.Value);
                        cmdDelBook.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBookData();
                        ClearInputFields();
                        SharedData.RefreshBookCount();
                        ((Dashboard)this.ParentForm)?.DashboardControl.UpdateBookCountLabel();
                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show("Error deleting book: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PopulateInputFieldsFromRow(DataGridViewRow row)
        {
            if (row == null || row.Cells["book_id"].Value == null)
            {
                ClearInputFields(); // Or handle as an error/no selection
                return;
            }

            _selectedBookId = Convert.ToInt32(row.Cells["book_id"].Value);
            guna2TextBox1.Text = row.Cells["book_title"].Value?.ToString(); // Title

            if (DateTime.TryParse(row.Cells["published_date"].Value?.ToString(), out DateTime pubDate))
            {
                guna2TextBox2.Text = pubDate.ToString("yyyy-MM-dd"); // Published Date
            }
            else
            {
                guna2TextBox2.Text = "";
            }
            guna2TextBox3.Text = row.Cells["book_isbn"].Value?.ToString(); // ISBN

            // Populate CheckedListBoxes
            if (_selectedBookId.HasValue)
            {
                SetSelectedItemsInCheckedListBox(chkListBoxAuthors, _selectedBookId.Value, "author_id", "book_authorships");
                SetSelectedItemsInCheckedListBox(chkListBoxGenres, _selectedBookId.Value, "genre_id", "book_genre_assignment");
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e) // SEARCH BOOK
        {
            if (IsInDesignMode()) return;

            string searchTerm = guna2TextBox1.Text.Trim(); // Using Title textbox for search term as per previous context
                                                           // If you have a separate search box, use that instead.

            LoadBookData(searchTerm);

            // After loading data, check if any rows were returned
            if (guna2DataGridView1.Rows.Count > 0)
            {
                // Select the first row programmatically
                guna2DataGridView1.ClearSelection(); // Optional: clear previous selections
                guna2DataGridView1.Rows[0].Selected = true;
                // Ensure the CurrentCell is also set to make the selection visually active and trigger events if needed
                if (guna2DataGridView1.Columns.GetFirstColumn(DataGridViewElementStates.Visible) != null)
                {
                    guna2DataGridView1.CurrentCell = guna2DataGridView1.Rows[0].Cells[guna2DataGridView1.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index];
                }
                // Populate input fields with the data from the first found row
                PopulateInputFieldsFromRow(guna2DataGridView1.Rows[0]);
            }
            else
            {
                // No results found, clear input fields
                ClearInputFields();
                _selectedBookId = null; // Ensure no book is considered selected
                MessageBox.Show("No books found matching your search.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // This was the original UserControl6_Load, ensure its logic is moved or it's intentionally empty
        private void UserControl6_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        // This is the original CellContentClick, CellClick is generally preferred for row selection logic
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            if (chkListBoxAuthors != null)
            {
                for (int i = 0; i < chkListBoxAuthors.Items.Count; i++)
                {
                    chkListBoxAuthors.SetItemChecked(i, false);
                }
            }
            if (chkListBoxGenres != null)
            {
                for (int i = 0; i < chkListBoxGenres.Items.Count; i++)
                {
                    chkListBoxGenres.SetItemChecked(i, false);
                }
            }
        }
    }
}
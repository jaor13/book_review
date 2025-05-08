using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using book_review.Helpers;
using MySql.Data.MySqlClient; // Ensure this is present

namespace book_review
{
    public partial class authorsControl : UserControl
    {
        private int? _selectedAuthorId = null; // To store the ID of the selected author

        public authorsControl()
        {
            InitializeComponent();
            if (!IsInDesignMode()) // More robust DesignMode check
            {
                this.Load += authorsControl_Load;
                if (guna2DataGridView1 != null)
                {
                    guna2DataGridView1.CellClick += guna2DataGridView1_CellClick;
                }
            }
        }

        private bool IsInDesignMode()
        {
            // Standard design mode check
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return true;

            // Check for design-time service
            if (this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) != null)
                return true;

            // Check parent hierarchy for design mode
            Control parent = this.Parent;
            while (parent != null)
            {
                if (parent.Site != null && parent.Site.DesignMode)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }


        private void authorsControl_Load(object sender, EventArgs e)
        {
            LoadAuthorData();
        }

        private void LoadAuthorData(string searchTerm = null)
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
                    // Assuming your authors table has author_id and author_name
                    // Add other columns as needed (e.g., biography, birth_date)
                    string query = "SELECT author_id, author_name FROM authors";

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query += " WHERE author_name LIKE @searchTerm";
                    }
                    query += " ORDER BY author_name;";

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
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "author_id", HeaderText = "ID", Name = "author_id", Visible = true }); // Can be hidden if not needed by user
                            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "author_name", HeaderText = "Author Name", Name = "author_name" });
                            guna2DataGridView1.Columns["author_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                        guna2DataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading author data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            guna2TextBox1.Clear(); // Author Name
            _selectedAuthorId = null;
            if (guna2DataGridView1 != null)
            {
                guna2DataGridView1.ClearSelection();
            }
        }

        private void PopulateInputFieldsFromRow(DataGridViewRow row)
        {
            if (row == null || row.Cells["author_id"].Value == null)
            {
                ClearInputFields();
                return;
            }
            _selectedAuthorId = Convert.ToInt32(row.Cells["author_id"].Value);
            guna2TextBox1.Text = row.Cells["author_name"].Value?.ToString();
        }


        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && guna2DataGridView1.Rows[e.RowIndex].Cells["author_id"].Value != null)
            {
                PopulateInputFieldsFromRow(guna2DataGridView1.Rows[e.RowIndex]);
            }
        }

        // This is the original CellContentClick, CellClick is generally preferred for row selection logic
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // You can keep this if you have specific logic for content click,
            // but for simple row selection and populating fields, CellClick is often better.
        }

        private void guna2Button4_Click(object sender, EventArgs e) // SEARCH AUTHOR
        {
            if (IsInDesignMode()) return;
            // Assuming guna2TextBox1 (Author Name textbox) is also used for search input.
            // If you have a dedicated search textbox, use its Text property.
            string searchTerm = guna2TextBox1.Text.Trim();
            LoadAuthorData(searchTerm);

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
                _selectedAuthorId = null;
                MessageBox.Show("No authors found matching your search.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) // ADD AUTHOR
        {
            if (IsInDesignMode()) return;

            string authorName = guna2TextBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(authorName))
            {
                MessageBox.Show("Author name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Check if author already exists
                    string checkQuery = "SELECT COUNT(*) FROM authors WHERE author_name = @author_name;";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@author_name", authorName);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("An author with this name already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = "INSERT INTO authors (author_name) VALUES (@author_name);";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@author_name", authorName);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Author added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAuthorData();
                    ClearInputFields();
                    SharedData.RefreshAuthorCount(); 
                    ((Dashboard)this.ParentForm)?.DashboardControl.UpdateAuthorCountLabel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding author: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) // UPDATE AUTHOR
        {
            if (IsInDesignMode()) return;
            if (!_selectedAuthorId.HasValue)
            {
                MessageBox.Show("Please select an author to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string authorName = guna2TextBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(authorName))
            {
                MessageBox.Show("Author name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Check if the new author name already exists for a DIFFERENT author
                    string checkQuery = "SELECT COUNT(*) FROM authors WHERE author_name = @author_name AND author_id != @author_id;";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@author_name", authorName);
                    checkCmd.Parameters.AddWithValue("@author_id", _selectedAuthorId.Value);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Another author with this name already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = "UPDATE authors SET author_name = @author_name WHERE author_id = @author_id;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@author_name", authorName);
                    cmd.Parameters.AddWithValue("@author_id", _selectedAuthorId.Value);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Author updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAuthorData();
                    ClearInputFields();
                    SharedData.RefreshAuthorCount(); // Refresh shared author count
                    ((Dashboard)this.ParentForm)?.DashboardControl.UpdateAuthorCountLabel();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating author: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) // DELETE AUTHOR
        {
            if (IsInDesignMode()) return;
            if (!_selectedAuthorId.HasValue)
            {
                MessageBox.Show("Please select an author to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this author? \nThis might affect books associated with this author.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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

                        // Before deleting an author, check for dependencies in book_authorships
                        string checkBooksQuery = "SELECT COUNT(*) FROM book_authorships WHERE author_id = @author_id;";
                        MySqlCommand checkBooksCmd = new MySqlCommand(checkBooksQuery, conn, transaction);
                        checkBooksCmd.Parameters.AddWithValue("@author_id", _selectedAuthorId.Value);
                        int associatedBooksCount = Convert.ToInt32(checkBooksCmd.ExecuteScalar());

                        if (associatedBooksCount > 0)
                        {
                            if (MessageBox.Show($"This author is associated with {associatedBooksCount} book(s). Deleting this author will also remove these associations. Do you want to proceed?", "Dependency Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            {
                                transaction.Rollback(); // Rollback if user cancels
                                return;
                            }
                            // If user proceeds, delete associations first
                            string deleteAssociationsQuery = "DELETE FROM book_authorships WHERE author_id = @author_id;";
                            MySqlCommand deleteAssocCmd = new MySqlCommand(deleteAssociationsQuery, conn, transaction);
                            deleteAssocCmd.Parameters.AddWithValue("@author_id", _selectedAuthorId.Value);
                            deleteAssocCmd.ExecuteNonQuery();
                        }

                        // Now delete the author
                        string query = "DELETE FROM authors WHERE author_id = @author_id;";
                        MySqlCommand cmd = new MySqlCommand(query, conn, transaction);
                        cmd.Parameters.AddWithValue("@author_id", _selectedAuthorId.Value);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();

                        MessageBox.Show("Author deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAuthorData();
                        ClearInputFields();
                        SharedData.RefreshAuthorCount(); // Refresh shared author count
                        ((Dashboard)this.ParentForm)?.DashboardControl.UpdateAuthorCountLabel();

                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show("Error deleting author: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
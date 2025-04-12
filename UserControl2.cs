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
    public partial class usersUserControl : UserControl
    {
        public usersUserControl()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == guna2DataGridView1.Columns["Action"].Index && e.RowIndex >= 0)
            {
                string userId = guna2DataGridView1.Rows[e.RowIndex].Cells["user_id"].Value.ToString();
                string action = guna2DataGridView1.Rows[e.RowIndex].Cells["Action"].Value.ToString();

                if (action == "Delete")
                {
                    DeleteUser(userId);
                }
                else if (action == "Update")
                {
                    UpdateUser(userId);
                }
            }
        }

        private void LoadUserData()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT user_id, username, email, account_status, last_login, created_at, role FROM users";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Customize column headers
                    guna2DataGridView1.AutoGenerateColumns = false;
                    guna2DataGridView1.Columns.Clear();

                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "user_id", HeaderText = "User ID" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "username", HeaderText = "Username" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "email", HeaderText = "Email" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "account_status", HeaderText = "Account Status" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "last_login", HeaderText = "Last Login" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "created_at", HeaderText = "Created At" });
                    guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "role", HeaderText = "Role" });

                    // Add Action column
                    DataGridViewButtonColumn actionColumn = new DataGridViewButtonColumn();
                    actionColumn.Name = "Action";
                    actionColumn.HeaderText = "Action";
                    actionColumn.Text = "Delete/Update";
                    actionColumn.UseColumnTextForButtonValue = true;
                    guna2DataGridView1.Columns.Add(actionColumn);

                    guna2DataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteUser(string userId)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM users WHERE user_id = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUserData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateUser(string userId)
        {
            // Implement the update logic here
            MessageBox.Show("Update user functionality is not implemented yet.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}

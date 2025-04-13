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
            if (!DesignMode)
            {
                this.Load += UsersUserControl_Load;
            }
        }

        private void UsersUserControl_Load(object sender, EventArgs e)
        {
            LoadUserData();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void LoadUserData()
        {
            if (DesignMode || this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) != null)
            {
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Database connection successful.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string query = "SELECT user_id, username, email, account_status, last_login, created_at, role FROM users";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (guna2DataGridView1 != null)
                    {
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

                        guna2DataGridView1.DataSource = dataTable;
                    }
                    else
                    {
                        MessageBox.Show("guna2DataGridView1 is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading user data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

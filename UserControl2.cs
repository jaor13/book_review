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
using book_review.Helpers;
using System.IO; 
using Excel = Microsoft.Office.Interop.Excel; 
using System.Runtime.InteropServices; 





namespace book_review
{
    public partial class usersUserControl : UserControl
    {
        private int? _selectedUserId = null;



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
            this.guna2DataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.guna2DataGridView1_CellClick);
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

            // Populate ComboBox for Role
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("admin");
            guna2ComboBox1.Items.Add("user");
            if (guna2ComboBox1.Items.Count > 0)
            {
                guna2ComboBox1.SelectedIndex = 0;
            }

            // Populate ComboBox for Account Status
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add("active");
            guna2ComboBox2.Items.Add("inactive");
            if (guna2ComboBox2.Items.Count > 0)
            {
                guna2ComboBox2.SelectedIndex = 0;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT user_id, username,password, email, account_status, last_login, created_at, role FROM users";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (guna2DataGridView1 != null)
                    {
                        guna2DataGridView1.AutoGenerateColumns = false;
                        guna2DataGridView1.Columns.Clear();

                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "user_id", HeaderText = "User ID" });
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "username", HeaderText = "Username" });
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "email", HeaderText = "Email" });
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "password", HeaderText = "Password" });
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "account_status", HeaderText = "Account Status" });
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "last_login", HeaderText = "Last Login" });
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "created_at", HeaderText = "Created At" });
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "role", HeaderText = "Role" });

                        guna2DataGridView1.DataSource = dataTable;

                        // Customize column headers
                        guna2DataGridView1.AutoGenerateColumns = false;
                        guna2DataGridView1.Columns.Clear();

                        // User ID Column
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "user_id",
                            HeaderText = "User ID",
                            Name = "user_id" // Set the Name property
                        });
                        // Username Column
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "username",
                            HeaderText = "Username",
                            Name = "username" // Set the Name property
                        });
                        // Email Column
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "email",
                            HeaderText = "Email",
                            Name = "email" // Set the Name property
                        });

                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "password",
                            HeaderText = "Password",
                            Name = "password" // Set the Name property
                        });


                        // Account Status Column
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "account_status",
                            HeaderText = "Account Status",
                            Name = "account_status" // Set the Name property
                        });
                        // Last Login Column
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "last_login",
                            HeaderText = "Last Login",
                            Name = "last_login" // Set the Name property
                        });
                        // Created At Column
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "created_at",
                            HeaderText = "Created At",
                            Name = "created_at" // Set the Name property
                        });
                        // Role Column
                        guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "role",
                            HeaderText = "Role",
                            Name = "role" // Set the Name property
                        });

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

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a valid row is clicked (e.g., not the header row)
            if (e.RowIndex >= 0 && e.RowIndex < guna2DataGridView1.Rows.Count)
            {
                DataGridViewRow selectedRow = guna2DataGridView1.Rows[e.RowIndex];


                if (selectedRow.Cells["user_id"]?.Value != null && int.TryParse(selectedRow.Cells["user_id"].Value.ToString(), out int userId))
                {
                    _selectedUserId = userId;
                }
                else
                {
                    _selectedUserId = null; // Reset if user_id is not valid
                    // Optionally show an error or log this, as user_id should always be present and valid
                }

                // Populate TextBoxes
                // Username (TextBox1)
                if (selectedRow.Cells["username"].Value != null)
                {
                    guna2TextBox1.Text = selectedRow.Cells["username"].Value.ToString();
                }
                else
                {
                    guna2TextBox1.Text = string.Empty;
                }

                // Email (TextBox2)
                if (selectedRow.Cells["email"].Value != null)
                {
                    guna2TextBox2.Text = selectedRow.Cells["email"].Value.ToString();
                }
                else
                {
                    guna2TextBox2.Text = string.Empty;
                }

                if (selectedRow.Cells["password"]?.Value != null)
                {
                    guna2TextBox3.Text = selectedRow.Cells["password"].Value.ToString();
                }
                else
                {
                    guna2TextBox3.Text = string.Empty;
                }

                // Populate ComboBoxes
                // Role (ComboBox1)
                if (selectedRow.Cells["role"].Value != null)
                {
                    string roleValue = selectedRow.Cells["role"].Value.ToString();
                    if (guna2ComboBox1.Items.Contains(roleValue))
                    {
                        guna2ComboBox1.SelectedItem = roleValue;
                    }
                    else
                    {
                        guna2ComboBox1.SelectedIndex = -1;
                    }
                }
                else
                {
                    guna2ComboBox1.SelectedIndex = -1;
                }

                // Account Status (ComboBox2)
                if (selectedRow.Cells["account_status"].Value != null)
                {
                    string statusValue = selectedRow.Cells["account_status"].Value.ToString();
                    if (guna2ComboBox2.Items.Contains(statusValue))
                    {
                        guna2ComboBox2.SelectedItem = statusValue;
                    }
                    else
                    {
                        guna2ComboBox2.SelectedIndex = -1;
                    }
                }
                else
                {
                    guna2ComboBox2.SelectedIndex = -1;
                }
            }
        }

       

        private void guna2Button1_Click(object sender, EventArgs e) 
        {
            // Add user
            string username = guna2TextBox1.Text;
            string email = guna2TextBox2.Text;
            string password = guna2TextBox3.Text;
            string accountStatus = guna2ComboBox2.SelectedItem?.ToString();
            string role = guna2ComboBox1.SelectedItem?.ToString();

            // Basic Validation (example)
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(accountStatus) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            bool success = false;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO users (username, email, password, account_status, role, created_at, last_login)
                        VALUES (@username, @email, @password, @account_status, @role, NOW(), NULL)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password); // Storing plain text password - security risk
                    cmd.Parameters.AddWithValue("@account_status", accountStatus);
                    cmd.Parameters.AddWithValue("@role", role);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        success = true;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex) // Catch specific MySqlException
                {
                    if (ex.Number == 1062) // MySQL error code for duplicate entry (e.g., unique username constraint)
                    {
                        MessageBox.Show($"Failed to add user: The username '{username}' already exists. Please choose a different username.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("A database error occurred while adding the user: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    success = false; // Ensure success is false on DB error
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while adding the user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    success = false; // Ensure success is false on general error
                }
            }

            if (success)
            {
                MessageBox.Show("User added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUserData(); // Refresh the grid in this UserControl


                // Clear input fields after successful deletion
                guna2TextBox1.Clear();
                guna2TextBox2.Clear();
                guna2TextBox3.Clear();
                guna2ComboBox1.SelectedIndex = -1;
                guna2ComboBox2.SelectedIndex = -1;
                _selectedUserId = null;


                SharedData.RefreshUserCount();
                ((Dashboard)this.ParentForm)?.DashboardControl.UpdateUserCountLabel();


            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            // Search user by username
            string username = guna2TextBox1.Text;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM users WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        guna2TextBox2.Text = reader["email"].ToString();
                        guna2TextBox3.Text = ""; // Do not display the password
                        guna2ComboBox2.SelectedItem = reader["account_status"].ToString();
                        guna2ComboBox1.SelectedItem = reader["role"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("User not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while searching for the user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // Check if a user was selected
            if (!_selectedUserId.HasValue)
            {
                MessageBox.Show("Please select a user from the grid to update.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // These are the potentially new values from the form fields
            string newUsername = guna2TextBox1.Text;
            string email = guna2TextBox2.Text;
            string password = guna2TextBox3.Text; // SECURITY WARNING: Storing/updating plain text passwords is insecure
            string accountStatus = guna2ComboBox2.SelectedItem?.ToString();
            string role = guna2ComboBox1.SelectedItem?.ToString();

            // Basic validation
            if (string.IsNullOrWhiteSpace(newUsername))
            {
                MessageBox.Show("Username cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Add more validation as needed (e.g., email format, password complexity if not plain text)


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // MODIFIED QUERY: Now includes username in SET and uses user_id in WHERE
                    string query = @"
                        UPDATE users
                        SET username = @newUsername, 
                            email = @email, 
                            password = @password, 
                            account_status = @account_status, 
                            role = @role
                        WHERE user_id = @userId"; // Use user_id to identify the record

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Parameters for the SET clause
                    cmd.Parameters.AddWithValue("@newUsername", newUsername); // The new username from the textbox
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@account_status", accountStatus);
                    cmd.Parameters.AddWithValue("@role", role);

                    // Parameter for the WHERE clause
                    cmd.Parameters.AddWithValue("@userId", _selectedUserId.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUserData(); // Refresh the grid to show changes
                        // Optionally, you might want to clear the _selectedUserId or text fields
                        // _selectedUserId = null; 
                        // guna2TextBox1.Clear(); // etc.
                    }
                    else
                    {
                        // This could happen if the _selectedUserId didn't match any row (e.g., deleted by another process)
                        MessageBox.Show("User not found or no changes were made.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex) // Catch specific MySqlException for more detailed errors
                {
                    if (ex.Number == 1062) // MySQL error code for duplicate entry (e.g., unique username constraint)
                    {
                        MessageBox.Show($"Failed to update user: The username '{newUsername}' already exists. Please choose a different username.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("A database error occurred while updating the user: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating the user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) // This is your "Delete User" button
        {

            string usernameToDelete = guna2TextBox1.Text;

            if (string.IsNullOrWhiteSpace(usernameToDelete))
            {
                MessageBox.Show("Please enter a username or select a user to delete.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirmation before deleting
            DialogResult confirmation = MessageBox.Show($"Are you sure you want to delete the user '{usernameToDelete}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmation == DialogResult.No)
            {
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            bool success = false;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "DELETE FROM users WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", usernameToDelete);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        success = true;
                    }
                    else
                    {
                        MessageBox.Show($"User '{usernameToDelete}' not found or already deleted.", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while deleting the user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    success = false;
                }
            }

            if (success)
            {
                MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUserData(); // Refresh the grid in this UserControl

                // Clear input fields after successful deletion
                guna2TextBox1.Clear();
                guna2TextBox2.Clear();
                guna2TextBox3.Clear();
                guna2ComboBox1.SelectedIndex = -1;
                guna2ComboBox2.SelectedIndex = -1;
                _selectedUserId = null;

                SharedData.RefreshUserCount();

                ((Dashboard)this.ParentForm)?.DashboardControl.UpdateUserCountLabel();


            }
        }



        private void PopulateUserEngagementReportTemplate()
        {
            // --- 1. Select the specific Excel Template File ---
            string templateFilePath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Template (user-engagement-report.xlsx)|user-engagement-report.xlsx|All Excel Files (*.xlsx)|*.xlsx";
                openFileDialog.Title = "Select 'user-engagement-report.xlsx' Template";
                // Optionally, set an initial directory
                // openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return; // User cancelled
                }
                // Basic check for the expected filename, though path can vary
                if (!Path.GetFileName(openFileDialog.FileName).Equals("user-engagement-report.xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Please select the 'user-engagement-report.xlsx' file.", "Incorrect File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                templateFilePath = openFileDialog.FileName;
            }

            // --- 2. Generate Output Filepath with Timestamp ---
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string suggestedFileName = $"user-engagement-report-{timestamp}.xlsx";
            string outputFilePath = "";

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Save Populated User Engagement Report";
                saveFileDialog.FileName = suggestedFileName;
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return; // User cancelled
                }
                outputFilePath = saveFileDialog.FileName;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string 'MySqlConnection' not found.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable userEngagementData = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                    SELECT
                        u.username AS UserName,
                        COUNT(r.review_id) AS NumberOfReviewsSubmitted,
                        IFNULL(AVG(r.rating), 0) AS AverageRatingGiven
                    FROM
                        users u
                    LEFT JOIN
                        reviews r ON u.user_id = r.user_id
                    GROUP BY
                        u.user_id, u.username
                    ORDER BY
                        NumberOfReviewsSubmitted DESC, u.username ASC;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(userEngagementData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching user engagement data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (userEngagementData.Rows.Count == 0)
            {
                MessageBox.Show("No user engagement data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // --- 4. Populate Excel Template using Interop ---
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                excelApp.DisplayAlerts = false;

                // Create a copy of the template instead of opening it directly
                File.Copy(templateFilePath, outputFilePath, true);
                workbook = excelApp.Workbooks.Open(outputFilePath);

                // **Assuming your data sheet is the first one or has a known name**
                // If you know the sheet name, use it. Otherwise, index 1.
                string targetSheetName = "Sheet1"; // CHANGE THIS if your template's sheet has a different name
                try
                {
                    worksheet = (Excel.Worksheet)workbook.Sheets[targetSheetName];
                }
                catch
                {
                    // Fallback to first sheet if named sheet not found
                    try
                    {
                        worksheet = (Excel.Worksheet)workbook.Sheets[1];
                        MessageBox.Show($"Sheet '{targetSheetName}' not found. Using the first available sheet. Please verify the output.", "Sheet Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch
                    {
                        MessageBox.Show($"Could not access any worksheet in the template '{Path.GetFileName(templateFilePath)}'.", "Template Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        workbook.Close(false);
                        excelApp.Quit();
                        return;
                    }
                }

                // Write "Report Generated On" Date
                worksheet.Cells[3, 2] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Cell B3

                // Data starting row and columns as per your specification
                int dataStartRow = 6;
                int userNameCol = 1;      // Column A
                int reviewsCountCol = 2;  // Column B
                int avgRatingCol = 3;     // Column C

                // Write data from DataTable to the worksheet
                for (int i = 0; i < userEngagementData.Rows.Count; i++)
                {
                    worksheet.Cells[dataStartRow + i, userNameCol] = userEngagementData.Rows[i]["UserName"];
                    worksheet.Cells[dataStartRow + i, reviewsCountCol] = userEngagementData.Rows[i]["NumberOfReviewsSubmitted"];
                    worksheet.Cells[dataStartRow + i, avgRatingCol] = userEngagementData.Rows[i]["AverageRatingGiven"];

                    // Optional: Format the average rating cell if your template doesn't already do it
                    ((Excel.Range)worksheet.Cells[dataStartRow + i, avgRatingCol]).NumberFormat = "0.00";
                }

                workbook.Save();
                workbook.Close(true);
                excelApp.Quit();

                MessageBox.Show("User Engagement Report populated and saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during Excel operation: " + ex.Message, "Excel Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    workbook?.Close(false);
                    excelApp?.Quit();
                }
                catch { /* Ignore cleanup errors */ }
            }
            finally
            {
                // Properly release COM objects
                ReleaseComObject(worksheet);
                ReleaseComObject(workbook);
                ReleaseComObject(excelApp);
            }
        }

        private void ReleaseComObject(object obj)
        {
            try
            {
                if (obj != null && Marshal.IsComObject(obj))
                {
                    Marshal.ReleaseComObject(obj);
                }
                obj = null;
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            PopulateUserEngagementReportTemplate();
        }
    }
}

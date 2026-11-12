using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tenorio_Activity1
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "Welcome, " + Session.FullName;
            lblRole.Text = "Role: " + Session.Role;

            if (Session.Role != "Admin")
            {
                btnUsers.Visible = false;
            }

            ShowDashboard();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            ShowProducts();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowUsers();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?",
                "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();  
            }
        }

        private void HighlightActiveButton(Button active)
        {
            var buttons = new[] { btnDashboard, btnProducts, btnUsers };
            foreach (var btn in buttons)
            {
                if (btn != null) btn.BackColor = Color.FromArgb(52, 73, 94);
            }
            if (active != null) active.BackColor = Color.FromArgb(41, 128, 185);
        }

        private void ShowDashboard()
        {
            lblPageTitle.Text = "Dashboard";
            ClearContent();
            HighlightActiveButton(btnDashboard);

            int productCount = 0;
            int userCount = 0;
            int lowStockCount = 0;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM Products", conn))
                        productCount = Convert.ToInt32(cmd.ExecuteScalar());

                    using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM Users", conn))
                        userCount = Convert.ToInt32(cmd.ExecuteScalar());

                    using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM Products WHERE Stock < 10", conn))
                        lowStockCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { }

            var banner = new Panel
            {
                Size = new Size(800, 100),
                Location = new Point(20, 60),
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            var lblWelcomeTitle = new Label
            {
                Text = "Welcome back, " + Session.FullName + "!",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            var lblWelcomeSub = new Label
            {
                Text = "Logged in as " + Session.Role + " • " + DateTime.Now.ToString("dddd, MMMM d, yyyy"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(20, 55)
            };
            banner.Controls.Add(lblWelcomeTitle);
            banner.Controls.Add(lblWelcomeSub);
            pnlContent.Controls.Add(banner);

            AddStatCard("Products", productCount.ToString(), Color.FromArgb(52, 152, 219), 20, 180);
            AddStatCard("Users", userCount.ToString(), Color.FromArgb(46, 204, 113), 290, 180);
            AddStatCard("Low Stock (< 10)", lowStockCount.ToString(), Color.FromArgb(231, 76, 60), 560, 180);
        }

        private void AddStatCard(string title, string value, Color accentColor, int x, int y)
        {
            var card = new Panel
            {
                Size = new Size(250, 130),
                Location = new Point(x, y),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var stripe = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = accentColor
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(30, 80)
            };

            card.Controls.Add(stripe);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblTitle);
            pnlContent.Controls.Add(card);
        }

        private void ShowProducts()
        {
            lblPageTitle.Text = "Products";
            ClearContent();
            HighlightActiveButton(btnProducts);

            var toolbar = new Panel
            {
                Size = new Size(pnlContent.Width - 40, 50),
                Location = new Point(20, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            var btnAdd = new Button
            {
                Text = "+ Add Product",
                Size = new Size(130, 36),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => OpenProductForm(0);

            var btnEdit = new Button
            {
                Text = "✎ Edit",
                Size = new Size(100, 36),
                Location = new Point(140, 0),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += (s, e) => EditSelectedProduct();

            var btnDelete = new Button
            {
                Text = "🗑 Delete",
                Size = new Size(100, 36),
                Location = new Point(250, 0),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) => DeleteSelectedProduct();

            var txtSearch = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(toolbar.Width - 260, 3),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Text = "Search products..."
            };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search products...") { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; } };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search products..."; txtSearch.ForeColor = Color.Gray; } };
            txtSearch.TextChanged += (s, e) => LoadProducts(dgvProducts, txtSearch.Text == "Search products..." ? "" : txtSearch.Text);

            toolbar.Controls.Add(btnAdd);
            toolbar.Controls.Add(btnEdit);
            toolbar.Controls.Add(btnDelete);
            toolbar.Controls.Add(txtSearch);
            pnlContent.Controls.Add(toolbar);

            dgvProducts = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(pnlContent.Width - 40, pnlContent.Height - 140),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 32 }
            };

            dgvProducts.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvProducts.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) EditSelectedProduct();
            };

            pnlContent.Controls.Add(dgvProducts);
            LoadProducts(dgvProducts, "");
        }

        private DataGridView dgvProducts;

        private void LoadProducts(DataGridView dgv, string searchTerm)
        {
            if (dgv == null) return;
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("ProductID", "ID");
            dgv.Columns.Add("ProductName", "Product Name");
            dgv.Columns.Add("Category", "Category");
            dgv.Columns.Add("Price", "Price");
            dgv.Columns.Add("Stock", "Stock");
            dgv.Columns.Add("DateAdded", "Date Added");

            dgv.Columns["ProductID"].Width = 50;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT ProductID, ProductName, Category, Price, Stock, DateAdded FROM Products";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                        sql += " WHERE ProductName LIKE ? OR Category LIKE ?";
                    sql += " ORDER BY ProductID DESC";

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("?", "%" + searchTerm + "%");
                            cmd.Parameters.AddWithValue("?", "%" + searchTerm + "%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgv.Rows.Add(
                                    reader["ProductID"],
                                    reader["ProductName"],
                                    reader["Category"],
                                    Convert.ToDecimal(reader["Price"]).ToString("C2"),
                                    reader["Stock"],
                                    Convert.ToDateTime(reader["DateAdded"]).ToString("yyyy-MM-dd"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void OpenProductForm(int productId)
        {
            var form = new frmProduct(productId);
            if (form.ShowDialog() == DialogResult.OK)
                LoadProducts(dgvProducts, "");
        }

        private void EditSelectedProduct()
        {
            if (dgvProducts == null || dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["ProductID"].Value);
            OpenProductForm(id);
        }

        private void DeleteSelectedProduct()
        {
            if (dgvProducts == null || dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["ProductID"].Value);
            string name = dgvProducts.SelectedRows[0].Cells["ProductName"].Value.ToString();

            if (MessageBox.Show($"Delete product \"{name}\"?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new OleDbCommand("DELETE FROM Products WHERE ProductID = ?", conn))
                    {
                        cmd.Parameters.AddWithValue("?", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadProducts(dgvProducts, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product: " + ex.Message);
            }
        }

        private DataGridView dgvUsers;

        private void ShowUsers()
        {
            if (Session.Role != "Admin")
            {
                MessageBox.Show("Only Admins can access user management.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ShowDashboard();
                return;
            }

            lblPageTitle.Text = "Users";
            ClearContent();
            HighlightActiveButton(btnUsers);

            var toolbar = new Panel
            {
                Size = new Size(pnlContent.Width - 40, 50),
                Location = new Point(20, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var btnRefresh = new Button
            {
                Text = "↻ Refresh",
                Size = new Size(110, 36),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadUsers(dgvUsers);

            var btnChangeRole = new Button
            {
                Text = "Change Role",
                Size = new Size(120, 36),
                Location = new Point(120, 0),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnChangeRole.FlatAppearance.BorderSize = 0;
            btnChangeRole.Click += (s, e) => ChangeSelectedUserRole();

            var btnReset = new Button
            {
                Text = "Reset Password",
                Size = new Size(140, 36),
                Location = new Point(250, 0),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) => ResetSelectedUserPassword();

            toolbar.Controls.Add(btnRefresh);
            toolbar.Controls.Add(btnChangeRole);
            toolbar.Controls.Add(btnReset);
            pnlContent.Controls.Add(toolbar);

            dgvUsers = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(pnlContent.Width - 40, pnlContent.Height - 140),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 32 }
            };

            dgvUsers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            pnlContent.Controls.Add(dgvUsers);
            LoadUsers(dgvUsers);
        }

        private void LoadUsers(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("UserID", "ID");
            dgv.Columns.Add("Username", "Username");
            dgv.Columns.Add("FullName", "Full Name");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Role", "Role");
            dgv.Columns.Add("DateCreated", "Date Created");
            dgv.Columns["UserID"].Width = 50;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new OleDbCommand(
                        "SELECT UserID, Username, FullName, Email, Role, DateCreated FROM Users ORDER BY UserID", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dgv.Rows.Add(
                                reader["UserID"],
                                reader["Username"],
                                reader["FullName"],
                                reader["Email"],
                                reader["Role"],
                                reader["DateCreated"] == DBNull.Value ? "" :
                                    Convert.ToDateTime(reader["DateCreated"]).ToString("yyyy-MM-dd HH:mm"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void ChangeSelectedUserRole()
        {
            if (dgvUsers == null || dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user first.");
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserID"].Value);
            if (userId == Session.UserID)
            {
                MessageBox.Show("You can't change your own role.");
                return;
            }

            string currentRole = dgvUsers.SelectedRows[0].Cells["Role"].Value.ToString();
            string newRole = currentRole == "Admin" ? "Staff" : "Admin";

            if (MessageBox.Show($"Change this user's role from {currentRole} to {newRole}?",
                "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new OleDbCommand("UPDATE Users SET Role=? WHERE UserID=?", conn))
                    {
                        cmd.Parameters.AddWithValue("?", newRole);
                        cmd.Parameters.AddWithValue("?", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadUsers(dgvUsers);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error changing role: " + ex.Message);
            }
        }

        private void ResetSelectedUserPassword()
        {
            if (dgvUsers == null || dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user first.");
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserID"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();

            string newPassword = Prompt.ShowDialog(
                $"Enter new password for '{username}' (min 6 chars):", "Reset Password");
            if (string.IsNullOrWhiteSpace(newPassword)) return;

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new OleDbCommand("UPDATE Users SET [Password]=? WHERE UserID=?", conn))
                    {
                        cmd.Parameters.AddWithValue("?", PasswordHasher.HashPassword(newPassword));
                        cmd.Parameters.AddWithValue("?", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Password reset successfully. The user's new password is now active.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting password: " + ex.Message);
            }
        }

        private void ClearContent()
        {
            for (int i = pnlContent.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlContent.Controls[i] != lblPageTitle)
                {
                    pnlContent.Controls[i].Dispose();
                }
            }
        }
    }
}

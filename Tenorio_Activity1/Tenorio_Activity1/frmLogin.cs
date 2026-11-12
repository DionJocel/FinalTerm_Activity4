using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Tenorio_Activity1
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var register = new frmRegister();
            register.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter username and password.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT UserID, Username, Password, FullName, Role " +
                                   "FROM Users WHERE Username = ?";

                    using (var cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", txtUsername.Text.Trim());

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["Password"].ToString();

                                if (PasswordHasher.VerifyPassword(txtPassword.Text, storedHash))
                                {
                                    Session.UserID = Convert.ToInt32(reader["UserID"]);
                                    Session.Username = reader["Username"].ToString();
                                    Session.FullName = reader["FullName"].ToString();
                                    Session.Role = reader["Role"].ToString();

                                    MessageBox.Show($"Welcome, {Session.FullName}!",
                                        "Login Success", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                                    this.Hide();
                                    var main = new frmMain();
                                    main.FormClosed += (s, a) => this.Show();
                                    main.Show();
                                }
                                else
                                {
                                    MessageBox.Show("Invalid username or password.",
                                        "Login Failed", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                    txtPassword.Clear();
                                    txtPassword.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.",
                                    "Login Failed", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                txtPassword.Clear();
                                txtPassword.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

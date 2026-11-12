using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tenorio_Activity1
{
    public partial class frmProduct : Form
    {
        private int _productId;
        public frmProduct(int productId)
        {
            InitializeComponent();
            _productId = productId;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadProduct()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new OleDbCommand(
                        "SELECT ProductName, Category, Price, Stock FROM Products WHERE ProductID = ?", conn))
                    {
                        cmd.Parameters.AddWithValue("?", _productId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader["ProductName"].ToString();
                                cmbCategory.Text = reader["Category"].ToString();
                                txtPrice.Text = Convert.ToDecimal(reader["Price"]).ToString("0.00");
                                txtStock.Text = reader["Stock"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product: " + ex.Message);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Product name is required.");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Price must be a valid number >= 0.");
                return;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Stock must be a valid integer >= 0.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = _productId > 0
                        ? "UPDATE Products SET ProductName=?, Category=?, Price=?, Stock=? WHERE ProductID=?"
                        : "INSERT INTO Products (ProductName, Category, Price, Stock) VALUES (?, ?, ?, ?)";

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("?", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("?", cmbCategory.Text.Trim());
                        cmd.Parameters.AddWithValue("?", price);
                        cmd.Parameters.AddWithValue("?", stock);
                        if (_productId > 0)
                            cmd.Parameters.AddWithValue("?", _productId);

                        cmd.ExecuteNonQuery();
                    }
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving: " + ex.Message);
            }
        }

        private void frmProduct_Load_1(object sender, EventArgs e)
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.AddRange(new[] {
                "Shoes", "Hats", "Shirts", "Hoodies",
                "Pants", "Jackets", "Other"
            });

            if (_productId > 0)
            {
                this.Text = "Edit Product";
                LoadProduct();
            }
            else
            {
                this.Text = "Add Product";
                cmbCategory.SelectedIndex = 6;
            }
        }
    }
}

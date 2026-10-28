
using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace _4._1
{
    public partial class Form1 : Form
    {
        private readonly string connectionString =
            @"Server=localhost;Database=LoginDB;Integrated Security=True;TrustServerCertificate=True;";

        public Form1()
        {
            InitializeComponent();

            // Enter để đăng nhập, Esc để thoát
            AcceptButton = btnLogin;
            CancelButton = btnExit;

            // Mặc định ẩn mật khẩu
            chkShowPassword.Checked = false;
            txtPassword.PasswordChar = '*';
            txtPassword.UseSystemPasswordChar = false;
        }

        // Hiện hoặc ẩn mật khẩu
        private void chkShowPassword_CheckedChanged(
            object? sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                // Hiện mật khẩu
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                // Ẩn mật khẩu
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '*';
            }
        }

        // Xử lý đăng nhập
        private void btnLogin_Click(object? sender, EventArgs e)
        {
            errorProvider.SetError(txtUsername, "");
            errorProvider.SetError(txtPassword, "");

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider.SetError(
                    txtUsername, "Vui lòng nhập tên đăng nhập.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider.SetError(
                    txtPassword, "Vui lòng nhập mật khẩu.");
                isValid = false;
            }

            if (!isValid)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                    txtUsername.Focus();
                else
                    txtPassword.Focus();

                return;
            }

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT COUNT(1)
                        FROM dbo.Users
                        WHERE Username = @Username
                          AND PasswordHash = @Password";

                    using (SqlCommand command =
                        new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Username", SqlDbType.NVarChar, 100)
                            .Value = txtUsername.Text.Trim();

                        command.Parameters.Add(
                            "@Password", SqlDbType.NVarChar, 256)
                            .Value = txtPassword.Text;

                        int count = Convert.ToInt32(
                            command.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show(
                                "Đăng nhập thành công!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Tên đăng nhập hoặc mật khẩu không đúng.",
                                "Đăng nhập thất bại",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Lỗi SQL Server: " + ex.Message,
                    "Lỗi cơ sở dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Nút thoát
        private void btnExit_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
        }
    }
}

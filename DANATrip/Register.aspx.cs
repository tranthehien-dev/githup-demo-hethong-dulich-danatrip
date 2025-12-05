using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DANATrip
{
    public partial class DangKy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string ten = txtUser.Text.Trim();
            string email = txtEmail.Text.Trim();
            string pass = txtPass.Text.Trim();
            string confirm = txtConfirm.Text.Trim();
            string sdt = txtSDT.Text.Trim(); // mới

            if (ten == "" || email == "" || pass == "" || confirm == "" || sdt == "")
            {
                ShowMessage("Vui lòng nhập đầy đủ thông tin (bao gồm số điện thoại)!");
                return;
            }

            if (pass != confirm)
            {
                ShowMessage("Mật khẩu không khớp!");
                return;
            }

            // Kiểm tra định dạng số điện thoại (VN) - ví dụ: +8490... hoặc 090...
            var phoneRe = new Regex(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$");
            if (!phoneRe.IsMatch(sdt))
            {
                ShowMessage("Số điện thoại không hợp lệ. Vui lòng nhập định dạng VN (ví dụ 0905123456 hoặc +84905123456).");
                return;
            }

            // Hash mật khẩu trước khi lưu
            string hashedPass = HashPassword(pass);

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Kiểm tra email hoặc số điện thoại đã tồn tại chưa
                    string checkQuery = "SELECT COUNT(*) FROM NguoiDung WHERE Email = @Email OR SDT = @SDT";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    checkCmd.Parameters.AddWithValue("@SDT", sdt);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        ShowMessage("Email hoặc số điện thoại đã được sử dụng. Vui lòng sử dụng thông tin khác.");
                        return;
                    }

                    string sql = @"
                        INSERT INTO NguoiDung (MaNguoiDung, HoTen, Email, MatKhau, SDT, VaiTro, NgayTao)
                        VALUES (@Ma, @Ten, @Email, @MK, @SDT, 'User', GETDATE())";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Ma", "ND" + Guid.NewGuid().ToString("N").Substring(0, 6));
                    cmd.Parameters.AddWithValue("@Ten", ten);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@MK", hashedPass);
                    cmd.Parameters.AddWithValue("@SDT", sdt);

                    cmd.ExecuteNonQuery();
                }

                // Đăng ký thành công
                ShowMessage("Đăng ký thành công! Bạn sẽ được chuyển đến trang đăng nhập.");

                // Delay 1 giây để người dùng thấy thông báo
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "redirect",
                    "setTimeout(function(){ window.location='SignIn.aspx'; }, 1000);",
                    true
                );
            }
            catch (Exception ex)
            {
                ShowMessage("Có lỗi xảy ra: " + ex.Message);
            }
        }

        // Hash SHA256 giống DangNhap.aspx.cs
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        // Hàm hiển thị popup giống DangNhap.aspx.cs
        private void ShowMessage(string msg)
        {
            ClientScript.RegisterStartupScript(
                this.GetType(),
                "alert",
                $"alert('{msg}');",
                true
            );
        }
    }
}
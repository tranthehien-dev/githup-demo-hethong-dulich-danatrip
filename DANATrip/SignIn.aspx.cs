using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DANATrip
{
    public partial class DangNhap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string emailOrUser = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (emailOrUser == "" || password == "")
            {
                ShowMessage("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            string hashedPass = HashPassword(password);
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT MaNguoiDung, HoTen, Email, VaiTro, SDT
                FROM NguoiDung
                WHERE (Email = @keyword OR MaNguoiDung = @keyword)
                  AND MatKhau = @pass";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword", emailOrUser);
                cmd.Parameters.AddWithValue("@pass", hashedPass);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Session["MaNguoiDung"] = reader["MaNguoiDung"].ToString();
                    Session["HoTen"] = reader["HoTen"].ToString();
                    Session["Email"] = reader["Email"].ToString();
                    Session["VaiTro"] = reader["VaiTro"].ToString();
                    Session["SDT"] = reader["SDT"] == DBNull.Value ? "" : reader["SDT"].ToString();

                    Response.Redirect("Home.aspx", false);
                    // Kiểm tra ReturnUrl
                    string returnUrl = Session["ReturnUrl"] != null ? Session["ReturnUrl"].ToString() : "Home.aspx";

                    // Xóa session ReturnUrl sau khi dùng
                    Session["ReturnUrl"] = null;

                    // Redirect về trang lưu trong ReturnUrl
                    Response.Redirect(returnUrl, false);
                    Context.ApplicationInstance.CompleteRequest();

                }
                else
                {
                    ShowMessage("Email/Tên đăng nhập hoặc mật khẩu không đúng!");
                }
            }
        }


        // ===== Hàm hash mật khẩu (SHA256) =====
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

        // ===== Hiển thị lỗi (popup nhỏ hoặc Label) =====
        private void ShowMessage(string msg)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{msg}');", true);
        }
    }
}



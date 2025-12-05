using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DANATrip
{
    public partial class UserSettings : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("~/DangNhap.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Có thể load thêm thông tin nếu cần
            }
        }

        // Đổi mật khẩu
        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            lblChangePassMsg.CssClass = "msg";
            lblChangePassMsg.Text = "";

            string userId = Session["MaNguoiDung"].ToString();
            string currentPass = txtCurrentPass.Text.Trim();
            string newPass = txtNewPass.Text.Trim();
            string confirmNew = txtConfirmNewPass.Text.Trim();

            if (string.IsNullOrEmpty(currentPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmNew))
            {
                lblChangePassMsg.CssClass = "msg error";
                lblChangePassMsg.Text = "Vui lòng nhập đầy đủ thông tin.";
                return;
            }

            if (newPass != confirmNew)
            {
                lblChangePassMsg.CssClass = "msg error";
                lblChangePassMsg.Text = "Mật khẩu mới và xác nhận không khớp.";
                return;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cn.Open();

                    // Lấy mật khẩu hiện tại trong DB
                    cmd.CommandText = "SELECT MatKhau FROM NguoiDung WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@id", userId);

                    object dbPassObj = cmd.ExecuteScalar();
                    if (dbPassObj == null)
                    {
                        lblChangePassMsg.CssClass = "msg error";
                        lblChangePassMsg.Text = "Không tìm thấy tài khoản.";
                        return;
                    }

                    string dbHash = dbPassObj.ToString();
                    string inputCurrentHash = HashPassword(currentPass);

                    if (!string.Equals(dbHash, inputCurrentHash, StringComparison.OrdinalIgnoreCase))
                    {
                        lblChangePassMsg.CssClass = "msg error";
                        lblChangePassMsg.Text = "Mật khẩu hiện tại không đúng.";
                        return;
                    }

                    // Update mật khẩu mới
                    string newHash = HashPassword(newPass);
                    cmd.Parameters.Clear();
                    cmd.CommandText = "UPDATE NguoiDung SET MatKhau = @mk WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@mk", newHash);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }

                lblChangePassMsg.CssClass = "msg success";
                lblChangePassMsg.Text = "Đổi mật khẩu thành công.";
                txtCurrentPass.Text = txtNewPass.Text = txtConfirmNewPass.Text = "";
            }
            catch (Exception ex)
            {
                lblChangePassMsg.CssClass = "msg error";
                lblChangePassMsg.Text = "Có lỗi xảy ra: " + ex.Message;
            }
        }

        // Xóa tài khoản
        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            lblDeleteMsg.CssClass = "msg";
            lblDeleteMsg.Text = "";

            string userId = Session["MaNguoiDung"].ToString();
            string pass = txtDeletePass.Text.Trim();

            if (string.IsNullOrEmpty(pass))
            {
                lblDeleteMsg.CssClass = "msg error";
                lblDeleteMsg.Text = "Vui lòng nhập mật khẩu để xác nhận.";
                return;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cn.Open();

                    // 1. Kiểm tra mật khẩu
                    cmd.CommandText = "SELECT MatKhau FROM NguoiDung WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@id", userId);
                    object dbPassObj = cmd.ExecuteScalar();
                    if (dbPassObj == null)
                    {
                        lblDeleteMsg.CssClass = "msg error";
                        lblDeleteMsg.Text = "Không tìm thấy tài khoản.";
                        return;
                    }

                    string dbHash = dbPassObj.ToString();
                    string inputHash = HashPassword(pass);
                    if (!string.Equals(dbHash, inputHash, StringComparison.OrdinalIgnoreCase))
                    {
                        lblDeleteMsg.CssClass = "msg error";
                        lblDeleteMsg.Text = "Mật khẩu không đúng.";
                        return;
                    }

                    // 2. Kiểm tra còn booking liên quan không
                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT COUNT(*) FROM Booking WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@id", userId);

                    int bookingCount = Convert.ToInt32(cmd.ExecuteScalar());
                    if (bookingCount > 0)
                    {
                        lblDeleteMsg.CssClass = "msg error";
                        lblDeleteMsg.Text = "Tài khoản của bạn hiện đang có " + bookingCount +
                                            " đơn đặt tour, vì vậy không thể xóa tài khoản.\n" +
                                            "Vui lòng liên hệ quản trị viên nếu muốn xóa toàn bộ dữ liệu.";
                        return;
                    }

                    // 3. Không còn booking -> cho phép xóa tài khoản
                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM NguoiDung WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }

                // 4. Xóa session và chuyển về trang chủ
                Session.Clear();
                Response.Redirect("~/Home.aspx");
            }
            catch (Exception ex)
            {
                lblDeleteMsg.CssClass = "msg error";
                lblDeleteMsg.Text = "Có lỗi xảy ra: " + ex.Message;
            }
        }

        // Hash SHA256 giống Register / SignIn
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
    }
}
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DANATrip
{
    public partial class Contract : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // nothing special on load
            if (!IsPostBack)
            {
                lblMessage.Text = "";
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            lblMessage.CssClass = "msg";

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string subject = txtSubject.Text.Trim(); // not stored (kept for future)
            string body = txtMessageBody.Text.Trim();

            // Server-side validation
            if (name.Length < 2)
            {
                ShowError("Vui lòng nhập họ và tên hợp lệ.");
                return;
            }
            if (!IsValidEmail(email))
            {
                ShowError("Vui lòng nhập địa chỉ email hợp lệ.");
                return;
            }
            if (body.Length < 6)
            {
                ShowError("Nội dung tin nhắn quá ngắn.");
                return;
            }

            string maNguoiDung = null;
            if (Session["MaNguoiDung"] != null)
                maNguoiDung = Session["MaNguoiDung"].ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = @"INSERT INTO LienHe (MaLienHe, MaNguoiDung, Ten, Email, NoiDung, NgayGui)
                                   VALUES (NEWID(), @MaNguoiDung, @Ten, @Email, @NoiDung, @NgayGui)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNguoiDung", (object)maNguoiDung ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Ten", name);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@NoiDung", body);
                        cmd.Parameters.AddWithValue("@NgayGui", DateTime.Now);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                // success
                lblMessage.CssClass = "msg success";
                lblMessage.Text = "Cảm ơn! Yêu cầu của bạn đã được gửi. Chúng tôi sẽ liên hệ lại sớm.";
                // clear form
                txtName.Text = "";
                txtEmail.Text = "";
                txtSubject.Text = "";
                txtMessageBody.Text = "";
            }
            catch (Exception ex)
            {
                // log error in your real app
                ShowError("Có lỗi khi gửi yêu cầu. Vui lòng thử lại sau.");
            }
        }

        bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            try
            {
                var re = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                return re.IsMatch(email);
            }
            catch { return false; }
        }

        void ShowError(string message)
        {
            lblMessage.CssClass = "msg error";
            lblMessage.Text = message;
        }
    }
}
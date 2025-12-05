using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class DanhGiaDetail : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Bắt buộc đăng nhập
            if (Session["MaNguoiDung"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("SignIn.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string maTour = Request.QueryString["MaTour"];
                if (string.IsNullOrEmpty(maTour))
                {
                    Response.Redirect("DanhGia.aspx");
                    return;
                }

                string userId = Session["MaNguoiDung"].ToString();
                if (!UserCanReview(userId, maTour))
                {
                    lblMsg.CssClass = "msg error";
                    lblMsg.Text = "Bạn chỉ có thể đánh giá những tour mà bạn đã đặt và đã sử dụng.";
                    btnSubmit.Enabled = false;
                    return;
                }

                LoadTourInfo(maTour);
            }
        }

        private void LoadTourInfo(string maTour)
        {
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT TenTour FROM Tour WHERE MaTour = @id";
                cmd.Parameters.AddWithValue("@id", maTour);
                cn.Open();
                object name = cmd.ExecuteScalar();
                lblTenTour.Text = name == null ? "(Không tìm thấy tour)" : name.ToString();
            }
        }

        private bool UserCanReview(string maNguoiDung, string maTour)
        {
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT COUNT(*)
                    FROM Booking
                    WHERE MaNguoiDung = @uid
                      AND MaTour = @tour
                      AND TrangThai <> 'Chua Thanh Toan'";
                cmd.Parameters.AddWithValue("@uid", maNguoiDung);
                cmd.Parameters.AddWithValue("@tour", maTour);

                cn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("SignIn.aspx");
                return;
            }

            string maTour = Request.QueryString["MaTour"];
            if (string.IsNullOrEmpty(maTour))
            {
                lblMsg.CssClass = "msg error";
                lblMsg.Text = "Thiếu thông tin tour.";
                return;
            }

            string userId = Session["MaNguoiDung"].ToString();
            if (!UserCanReview(userId, maTour))
            {
                lblMsg.CssClass = "msg error";
                lblMsg.Text = "Bạn không có quyền đánh giá tour này.";
                return;
            }

            int sao = GetSelectedStar();
            if (sao < 1 || sao > 5)
            {
                lblMsg.CssClass = "msg error";
                lblMsg.Text = "Vui lòng chọn số sao từ 1 đến 5.";
                return;
            }

            string noiDung = txtNoiDung.Text.Trim();
            string maDanhGia = "DG" + Guid.NewGuid().ToString("N").Substring(0, 10);

            try
            {
                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO DanhGia (MaDanhGia, MaTour, MaNguoiDung, Sao, NoiDung, NgayDanhGia)
                        VALUES (@MaDanhGia, @MaTour, @MaNguoiDung, @Sao, @NoiDung, @NgayDanhGia)";
                    cmd.Parameters.AddWithValue("@MaDanhGia", maDanhGia);
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    cmd.Parameters.AddWithValue("@MaNguoiDung", userId);
                    cmd.Parameters.AddWithValue("@Sao", sao);
                    cmd.Parameters.AddWithValue("@NoiDung", (object)noiDung ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayDanhGia", DateTime.Now);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.CssClass = "msg success";
                lblMsg.Text = "Cảm ơn bạn đã gửi đánh giá!";
                txtNoiDung.Text = "";
                ClearStars();
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "msg error";
                lblMsg.Text = "Có lỗi khi lưu đánh giá: " + ex.Message;
            }
        }

        private int GetSelectedStar()
        {
            if (r5.Checked) return 5;
            if (r4.Checked) return 4;
            if (r3.Checked) return 3;
            if (r2.Checked) return 2;
            if (r1.Checked) return 1;
            return 0;
        }

        private void ClearStars()
        {
            r1.Checked = r2.Checked = r3.Checked = r4.Checked = r5.Checked = false;
        }
    }
}
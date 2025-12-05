using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DANATrip
{
    public partial class UserProfile : System.Web.UI.Page
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
                LoadProfile();
            }
        }

        void LoadProfile()
        {
            string userId = Session["MaNguoiDung"].ToString();
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                // Lấy chỉ cột tồn tại: HoTen, Email, SDT, NgayTao
                cmd.CommandText = @"SELECT HoTen, Email, SDT, NgayTao
                                    FROM NguoiDung WHERE MaNguoiDung = @id";
                cmd.Parameters.AddWithValue("@id", userId);
                cn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtHoTen.Text = rd["HoTen"]?.ToString() ?? "";
                        txtEmail.Text = rd["Email"]?.ToString() ?? "";
                        txtSDT.Text = rd["SDT"]?.ToString() ?? "";
                        lblHoTen.InnerText = rd["HoTen"]?.ToString() ?? "";
                        lblJoined.Text = rd["NgayTao"] != DBNull.Value ? Convert.ToDateTime(rd["NgayTao"]).ToString("yyyy") : "";
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string userId = Session["MaNguoiDung"].ToString();
            string hoTen = txtHoTen.Text.Trim();
            string sdt = txtSDT.Text.Trim();

            // validate phone if provided
            var phoneRe = new Regex(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$");
            if (!string.IsNullOrEmpty(sdt) && !phoneRe.IsMatch(sdt))
            {
                lblMsg.CssClass = "msg error";
                lblMsg.Text = "Số điện thoại không hợp lệ.";
                return;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    // UPDATE chỉ những cột tồn tại: HoTen, SDT
                    cmd.CommandText = @"UPDATE NguoiDung
                                        SET HoTen = @HoTen, SDT = @SDT
                                        WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@HoTen", hoTen);
                    cmd.Parameters.AddWithValue("@SDT", string.IsNullOrEmpty(sdt) ? (object)DBNull.Value : sdt);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.CssClass = "msg success";
                lblMsg.Text = "Cập nhật thông tin thành công.";
                lblHoTen.InnerText = hoTen;
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "msg error";
                lblMsg.Text = "Có lỗi khi cập nhật: " + ex.Message;
            }
        }
    }
}
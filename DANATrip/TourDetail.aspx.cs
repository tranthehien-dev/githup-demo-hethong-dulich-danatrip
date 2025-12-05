using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class TourDetail : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];
                if (string.IsNullOrEmpty(id)) Response.Redirect("Tour.aspx");

                LoadTour(id);
                LoadHighlights(id);
                LoadSchedule(id);
                LoadIncludes(id);
            }
        }

        void LoadTour(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        TenTour, ThoiLuong, PhuongTien, NgayKhoiHanh, GiaNguoiLon,
                        (SELECT TOP 1 UrlAnh FROM TourImages WHERE MaTour = t.MaTour) AS UrlAnh
                    FROM Tour t
                    WHERE MaTour = @id
                ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblTenTour.Text = dr["TenTour"].ToString();
                    lblThoiLuong.Text = dr["ThoiLuong"].ToString();
                    lblPhuongTien.Text = dr["PhuongTien"].ToString();
                    lblNgayKhoiHanh.Text = Convert.ToDateTime(dr["NgayKhoiHanh"]).ToString("dd/MM/yyyy HH:mm");
                    lblGia.Text = Convert.ToDecimal(dr["GiaNguoiLon"]).ToString("N0");

                    imgCover.ImageUrl = dr["UrlAnh"].ToString();
                }
            }
        }

        void LoadHighlights(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT TieuDe, MoTa FROM TourHighlights WHERE MaTour = @id";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@id", id);

                DataTable dt = new DataTable();
                da.Fill(dt);
                rptHighlights.DataSource = dt;
                rptHighlights.DataBind();
            }
        }

        void LoadSchedule(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT ThoiGian, TieuDe, MoTa FROM TourSchedule WHERE MaTour = @id ORDER BY ThuTu";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@id", id);

                DataTable dt = new DataTable();
                da.Fill(dt);
                rptSchedule.DataSource = dt;
                rptSchedule.DataBind();
            }
        }

        void LoadIncludes(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT NoiDung FROM TourIncludes WHERE MaTour = @id";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@id", id);

                DataTable dt = new DataTable();
                da.Fill(dt);
                rptIncludes.DataSource = dt;
                rptIncludes.DataBind();
            }
        }
        protected void btnDatTour_Click(object sender, EventArgs e)
        {
            // Kiểm tra đăng nhập
            if (Session["MaNguoiDung"] == null)
            {
                // Lưu trang hiện tại để quay lại sau khi login
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("SignIn.aspx");
                return;
            }

            // Lấy ID tour từ QueryString
            string maTour = Request.QueryString["id"];
            if (string.IsNullOrEmpty(maTour))
            {
                Response.Redirect("Tour.aspx"); // Quay về danh sách tour nếu không có id
                return;
            }

            // Chuyển sang trang Booking.aspx, truyền ID tour
            Response.Redirect("Booking.aspx?id=" + maTour);
        }

    }
}

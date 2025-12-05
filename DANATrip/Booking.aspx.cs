using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace DANATrip
{
    public partial class Booking : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Bắt buộc đăng nhập
            if (Session["MaNguoiDung"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("DangNhap.aspx");
                return;
            }

            if (!IsPostBack)
            {
                txtTen.Text = (Session["HoTen"] as string) ?? "";
                txtEmail.Text = (Session["Email"] as string) ?? "";
                txtSDT.Text = (Session["SDT"] as string) ?? "";

                string id = Request.QueryString["id"];
                if (string.IsNullOrEmpty(id))
                    Response.Redirect("Tour.aspx");

                LoadTour(id);
            }
        }

        void LoadTour(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                SELECT TenTour, GiaNguoiLon, GiaTreEm,
                    (SELECT TOP 1 UrlAnh FROM TourImages WHERE MaTour = t.MaTour) AS UrlAnh
                FROM Tour t WHERE MaTour = @id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblTenTour.Text = dr["TenTour"].ToString();
                    lblGia.Text = Convert.ToDecimal(dr["GiaNguoiLon"]).ToString("N0");
                    lblGia0.Text = Convert.ToDecimal(dr["GiaTreEm"]).ToString("N0");
                    imgTour.ImageUrl = dr["UrlAnh"].ToString();
                }
            }
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            lblError.Text = ""; // clear previous message

            string id = Request.QueryString["id"];
            string userID = Session["MaNguoiDung"].ToString();

            // Validate numeric quantities
            if (!int.TryParse(txtNL.Text, out int nl)) nl = 0;
            if (!int.TryParse(txtTE.Text, out int te)) te = 0;

            if (nl <= 0 && te <= 0)
            {
                lblError.Text = "Vui lòng nhập số lượng ít nhất 1 khách (người lớn hoặc trẻ em).";
                return;
            }

            // Validate phone (server-side): VN format +84 or 0 then 3/5/7/8/9 + 8 digits
            string sdt = txtSDT.Text?.Trim() ?? "";
            var phoneRe = new Regex(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$");
            if (!phoneRe.IsMatch(sdt))
            {
                lblError.Text = "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam (ví dụ 0905123456 hoặc +84905123456).";
                return;
            }

            // Parse both adult and child prices from labels
            decimal giaAdult = 0m;
            decimal giaChild = 0m;

            if (!string.IsNullOrEmpty(lblGia.Text))
                Decimal.TryParse(lblGia.Text.Replace(",", ""), out giaAdult);
            if (!string.IsNullOrEmpty(lblGia0.Text))
                Decimal.TryParse(lblGia0.Text.Replace(",", ""), out giaChild);

            decimal total = (nl * giaAdult) + (te * giaChild);

            int maBooking;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"INSERT INTO Booking 
                    (MaTour, MaNguoiDung, TenKhach, SDT, Email, SoNguoiLon, SoTreEm, TongTien, TrangThai)
                    OUTPUT INSERTED.MaBooking
                    VALUES 
                    (@MaTour, @MaNguoiDung, @Ten, @SDT, @Email, @NL, @TE, @TT, @TrangThai)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTour", id);
                cmd.Parameters.AddWithValue("@MaNguoiDung", userID);
                cmd.Parameters.AddWithValue("@Ten", txtTen.Text);
                cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@NL", nl);
                cmd.Parameters.AddWithValue("@TE", te);
                cmd.Parameters.AddWithValue("@TT", total);

                // Cả online và offline đều ở trạng thái "Chờ thanh toán"
                string trangThai = "Chờ Thanh Toán";
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                conn.Open();
                maBooking = (int)cmd.ExecuteScalar();
            }

            Session["MaBooking"] = maBooking;

            if (rblPayment.SelectedValue == "online")
            {
                Response.Redirect("OnlinePayment.aspx?bookingId=" + maBooking);
            }
            else
            {
                // thanh toán khi đến
                Response.Redirect("BookingSuccess.aspx");
            }
        }
    }
}
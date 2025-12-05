using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class BookingSuccess : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["MaBooking"] != null)
                {
                    int maBooking = Convert.ToInt32(Session["MaBooking"]);
                    LoadBookingInfo(maBooking);
                }
                else
                {
                    Response.Redirect("Tour.aspx"); // Không có booking → quay lại danh sách tour
                }
            }
        }

        void LoadBookingInfo(int maBooking)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                SELECT b.MaBooking, t.TenTour, t.NgayKhoiHanh, b.SoNguoiLon, b.SoTreEm, b.TongTien
                FROM Booking b
                INNER JOIN Tour t ON b.MaTour = t.MaTour
                WHERE b.MaBooking = @MaBooking";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBooking", maBooking);
                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblMaBooking.Text = dr["MaBooking"].ToString();
                    lblTenTour.Text = dr["TenTour"].ToString();
                    lblNgayKhoiHanh.Text = Convert.ToDateTime(dr["NgayKhoiHanh"]).ToString("dd/MM/yyyy HH:mm");
                    lblNL.Text = dr["SoNguoiLon"].ToString();
                    lblTE.Text = dr["SoTreEm"].ToString();
                    lblTongTien.Text = Convert.ToDecimal(dr["TongTien"]).ToString("N0");
                }
            }
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            // Trigger javascript saveBookingImage()
            string script = "saveBookingImage();";
            ClientScript.RegisterStartupScript(this.GetType(), "saveImage", script, true);
        }
    }
}

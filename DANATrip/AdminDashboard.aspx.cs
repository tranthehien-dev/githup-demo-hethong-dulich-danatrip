using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: check quyền admin
            if (!IsPostBack)
            {
                LoadSummaryCards();
                LoadRevenue7Days();
                LoadRecentBookings();
                LoadTopTours();
            }
        }

        void LoadSummaryCards()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                // 1. Tổng doanh thu (đơn đã thanh toán)
                cmd.CommandText = @"SELECT ISNULL(SUM(TongTien),0)
                                    FROM Booking
                                    WHERE TrangThai LIKE N'%Đã Thanh Toán%'";
                object total = cmd.ExecuteScalar();
                decimal tongDoanhThu = Convert.ToDecimal(total);
                lblTongDoanhThu.Text = tongDoanhThu.ToString("N0") + " ₫";

                // 2. Tổng số booking
                cmd.CommandText = @"SELECT COUNT(*) FROM Booking";
                int tongBooking = Convert.ToInt32(cmd.ExecuteScalar());
                lblTongBooking.Text = tongBooking.ToString();

                // 3. Người dùng mới 30 ngày (nếu bảng NguoiDung có NgayTao)
                try
                {
                    cmd.CommandText = @"
                        SELECT COUNT(*) FROM NguoiDung
                        WHERE NgayTao >= DATEADD(DAY,-30,GETDATE())";
                    int userMoi = Convert.ToInt32(cmd.ExecuteScalar());
                    lblUserMoi.Text = userMoi.ToString();
                }
                catch
                {
                    // nếu chưa có cột NgayTao
                    lblUserMoi.Text = "-";
                }

                // 4. Tour & địa điểm
                cmd.CommandText = "SELECT COUNT(*) FROM Tour";
                int soTour = Convert.ToInt32(cmd.ExecuteScalar());
                lblTongTour.Text = soTour.ToString();

                cmd.CommandText = "SELECT COUNT(*) FROM DiaDiem";
                int soDiaDiem = Convert.ToInt32(cmd.ExecuteScalar());
                lblTongDiaDiem.Text = soDiaDiem.ToString();

                lblTongTourDiaDiem.Text = (soTour + soDiaDiem).ToString();
                lblSoTourCircle.Text = soTour.ToString();

                // tour active / hidden
                int active = 0, hidden = 0;
                try
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Tour WHERE ISNULL(HienThi,1) = 1";
                    active = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = "SELECT COUNT(*) FROM Tour WHERE ISNULL(HienThi,1) = 0";
                    hidden = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch { }

                lblTourActive.Text = active.ToString();
                lblTourHidden.Text = hidden.ToString();
            }
        }

        void LoadRevenue7Days()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT CONVERT(VARCHAR(10), NgayDat, 103) AS Ngay,
                           SUM(TongTien) AS TongTien
                    FROM Booking
                    WHERE NgayDat >= DATEADD(DAY,-6, CAST(GETDATE() AS DATE))
                      AND TrangThai LIKE N'%Đã Thanh Toán%'
                    GROUP BY CONVERT(VARCHAR(10), NgayDat, 103)
                    ORDER BY MIN(NgayDat)";
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptRevenue7Days.DataSource = dt;
            rptRevenue7Days.DataBind();
        }

        void LoadRecentBookings()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TOP 5
                           b.MaBooking,
                           b.TenKhach,
                           b.NgayDat,
                           b.TrangThai,
                           t.TenTour
                    FROM Booking b
                    INNER JOIN Tour t ON b.MaTour = t.MaTour
                    ORDER BY b.NgayDat DESC, b.MaBooking DESC";
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptBookingRecent.DataSource = dt;
            rptBookingRecent.DataBind();
        }

        void LoadTopTours()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TOP 5
                           t.TenTour,
                           COUNT(*) AS SoLan
                    FROM Booking b
                    INNER JOIN Tour t ON b.MaTour = t.MaTour
                    GROUP BY t.TenTour
                    ORDER BY SoLan DESC";
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            // tính Percent theo max
            int max = 0;
            foreach (DataRow r in dt.Rows)
            {
                int soLan = Convert.ToInt32(r["SoLan"]);
                if (soLan > max) max = soLan;
            }
            if (max == 0) max = 1;

            dt.Columns.Add("Percent", typeof(int));
            foreach (DataRow r in dt.Rows)
            {
                int soLan = Convert.ToInt32(r["SoLan"]);
                int percent = (int)Math.Round(soLan * 100.0 / max);
                r["Percent"] = percent;
            }

            rptTopTours.DataSource = dt;
            rptTopTours.DataBind();
        }

        // dùng trong markup
        protected string GetBookingStatusCss(string trangThai)
        {
            if (string.IsNullOrEmpty(trangThai)) return "db-status-default";
            trangThai = trangThai.ToLower();

            if (trangThai.Contains("đã thanh toán") || trangThai.Contains("đã xác nhận"))
                return "db-status-success";
            if (trangThai.Contains("chờ") || trangThai.Contains("xử lý"))
                return "db-status-pending";
            if (trangThai.Contains("hủy"))
                return "db-status-cancel";

            return "db-status-default";
        }
    }
}
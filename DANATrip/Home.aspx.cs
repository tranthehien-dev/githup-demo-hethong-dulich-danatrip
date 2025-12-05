using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace DANATrip
{
    public partial class Home : Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPlaces();
                LoadFoods();
                LoadReviews();
            }
        }

        void LoadPlaces()
        {
            var dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"
            SELECT TOP 12 MaDiaDiem, TenDiaDiem, NoiDung,
                   COALESCE(HinhAnhChinh,'images/placeholder.jpg') AS HinhAnhChinh
            FROM DiaDiem
            WHERE ISNULL(HienThi, 1) = 1      -- chỉ lấy địa điểm đang hiển thị
              AND (TrangThai IS NULL OR TrangThai <> N'Tạm ẩn')  -- tùy bạn, có thể bỏ dòng này
            ORDER BY NEWID()";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            rptPlaces.DataSource = dt;
            rptPlaces.DataBind();
        }

        void LoadFoods()
        {
            var dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"SELECT TOP 12 a.MaMon, a.TenMon, a.MoTa,
                    COALESCE(a.HinhAnh,'images/placeholder.jpg') AS HinhAnh
                    FROM AmThuc a
                    WHERE ISNULL(a.HienThi,1) = 1
                    ORDER BY NEWID()";
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            rptFoods.DataSource = dt;
            rptFoods.DataBind();
        }
        void LoadReviews()
        {
            var dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TOP 12 
                           dg.MaDanhGia,
                           u.HoTen,
                           dg.NoiDung,
                           dg.Sao,
                           dg.NgayDanhGia,
                           t.TenTour
                    FROM DanhGia dg
                    LEFT JOIN NguoiDung u ON dg.MaNguoiDung = u.MaNguoiDung
                    INNER JOIN Tour t ON dg.MaTour = t.MaTour
                    WHERE ISNULL(dg.HienThi,1) = 1
                    ORDER BY dg.NgayDanhGia DESC";
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            rptReviews.DataSource = dt;
            rptReviews.DataBind();
        }
    }
}

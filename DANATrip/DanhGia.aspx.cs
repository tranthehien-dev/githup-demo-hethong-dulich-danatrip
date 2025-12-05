using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class DanhGia : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // KHÔNG bắt buộc đăng nhập: ai cũng xem được
            if (!IsPostBack)
            {
                LoadDanhGia();
            }
        }

        private void LoadDanhGia()
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT dg.MaDanhGia,
                           dg.MaTour,
                           t.TenTour,
                           nd.HoTen,
                           dg.Sao,
                           dg.NoiDung,
                           dg.NgayDanhGia
                    FROM DanhGia dg
                    INNER JOIN Tour t ON dg.MaTour = t.MaTour
                    LEFT JOIN NguoiDung nd ON dg.MaNguoiDung = nd.MaNguoiDung
                    WHERE ISNULL(dg.HienThi,1) = 1
                    ORDER BY dg.NgayDanhGia DESC";
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0)
            {
                rptDanhGia.Visible = false;
                lblEmpty.Text = "Chưa có đánh giá nào.";
            }
            else
            {
                rptDanhGia.Visible = true;
                rptDanhGia.DataSource = dt;
                rptDanhGia.DataBind();
                lblEmpty.Text = "";
            }
        }
    }
}
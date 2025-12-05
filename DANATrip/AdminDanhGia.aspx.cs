using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminDanhGia : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: check quyền admin
            if (!IsPostBack)
            {
                LoadReviews();
            }
        }

        void LoadReviews()
        {
            DataTable dt = new DataTable();
            string keyword = txtSearch.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT dg.MaDanhGia,
                           dg.MaTour,
                           dg.MaNguoiDung,
                           dg.Sao,
                           dg.NoiDung,
                           dg.NgayDanhGia,
                           ISNULL(dg.HienThi, 1) AS HienThi,
                           t.TenTour,
                           u.HoTen
                    FROM DanhGia dg
                    INNER JOIN Tour t ON dg.MaTour = t.MaTour
                    LEFT JOIN NguoiDung u ON dg.MaNguoiDung = u.MaNguoiDung
                    WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    cmd.CommandText += @"
                        AND (dg.MaDanhGia LIKE @kw
                             OR t.TenTour LIKE @kw
                             OR u.HoTen LIKE @kw
                             OR dg.NoiDung LIKE @kw)";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                cmd.CommandText += " ORDER BY dg.NgayDanhGia DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptReviews.DataSource = dt;
            rptReviews.DataBind();

            lblSummary.Text = $"Có {dt.Rows.Count} đánh giá.";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadReviews();
        }

        protected void rptReviews_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maDG = e.CommandArgument.ToString();

            if (e.CommandName == "Toggle")
            {
                ToggleVisibility(maDG);
            }
            else if (e.CommandName == "Delete")
            {
                DeleteReview(maDG);
                lblMsg.Text = "Đã xóa đánh giá.";
                lblMsg.CssClass = "ac-msg success";
            }

            LoadReviews();
        }

        void ToggleVisibility(string maDanhGia)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE DanhGia
                    SET HienThi = CASE WHEN ISNULL(HienThi,1) = 1 THEN 0 ELSE 1 END
                    WHERE MaDanhGia = @id";
                cmd.Parameters.AddWithValue("@id", maDanhGia);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        void DeleteReview(string maDanhGia)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM DanhGia WHERE MaDanhGia = @id";
                cmd.Parameters.AddWithValue("@id", maDanhGia);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
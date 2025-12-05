using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminTour : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: kiểm tra quyền admin
            if (!IsPostBack)
            {
                LoadTours();
            }
        }

        void LoadTours(string keyword = "")
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT MaTour,
                           TenTour,
                           LEFT(ISNULL(MoTaNgan, ''), 80) + CASE WHEN LEN(ISNULL(MoTaNgan,'')) > 80 THEN '...' ELSE '' END AS MoTaNgan,
                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                           ISNULL(HienThi, 1) AS HienThi
                    FROM Tour";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    cmd.CommandText += " WHERE TenTour LIKE @kw";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                cmd.CommandText += " ORDER BY MaTour";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptTours.DataSource = dt;
            rptTours.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadTours(txtSearch.Text.Trim());
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminTourEdit.aspx");
        }

        protected void rptTours_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maTour = e.CommandArgument.ToString();

            if (e.CommandName == "Edit")
            {
                Response.Redirect("AdminTourEdit.aspx?id=" + maTour);
            }
            else if (e.CommandName == "Delete")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM TourImages     WHERE MaTour = @id;
                        DELETE FROM TourHighlights WHERE MaTour = @id;
                        DELETE FROM TourSchedule   WHERE MaTour = @id;
                        DELETE FROM TourIncludes   WHERE MaTour = @id;
                        DELETE FROM TourTagMapping WHERE MaTour = @id;
                        DELETE FROM Tour           WHERE MaTour = @id;";
                    cmd.Parameters.AddWithValue("@id", maTour);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadTours(txtSearch.Text.Trim());
            }
        }

        protected void chkHienThi_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            RepeaterItem item = (RepeaterItem)chk.NamingContainer;
            HiddenField hf = (HiddenField)item.FindControl("hfMaTour");

            string maTour = hf.Value;
            bool hienThi = chk.Checked;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Tour SET HienThi = @ht WHERE MaTour = @id";
                cmd.Parameters.AddWithValue("@ht", hienThi);
                cmd.Parameters.AddWithValue("@id", maTour);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
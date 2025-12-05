using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminPlace : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: kiểm tra quyền admin, nếu không phải thì redirect

            if (!IsPostBack)
            {
                LoadPlaces();
            }
        }

        void LoadPlaces(string keyword = "")
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT MaDiaDiem,
                           TenDiaDiem,
                           LEFT(ISNULL(NoiDung, ''), 80) + CASE WHEN LEN(ISNULL(NoiDung,'')) > 80 THEN '...' ELSE '' END AS MoTaNgan,
                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                           ISNULL(HienThi, 1) AS HienThi
                    FROM DiaDiem";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    cmd.CommandText += " WHERE TenDiaDiem LIKE @kw";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                cmd.CommandText += " ORDER BY TenDiaDiem ASC";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptPlaces.DataSource = dt;
            rptPlaces.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadPlaces(txtSearch.Text.Trim());
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPlaceEdit.aspx"); // không có id -> thêm mới
        }

        protected void rptPlaces_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maDiaDiem = e.CommandArgument.ToString();

            if (e.CommandName == "Edit")
            {
                Response.Redirect("AdminPlaceEdit.aspx?id=" + maDiaDiem);
            }
            else if (e.CommandName == "Delete")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM DiaDiem WHERE MaDiaDiem = @id";
                    cmd.Parameters.AddWithValue("@id", maDiaDiem);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadPlaces(txtSearch.Text.Trim());
            }
        }

        protected void chkHienThi_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            RepeaterItem item = (RepeaterItem)chk.NamingContainer;
            HiddenField hf = (HiddenField)item.FindControl("hfMaDiaDiem");

            string maDiaDiem = hf.Value;
            bool hienThi = chk.Checked;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE DiaDiem SET HienThi = @ht WHERE MaDiaDiem = @id";
                cmd.Parameters.AddWithValue("@ht", hienThi);
                cmd.Parameters.AddWithValue("@id", maDiaDiem);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // không cần load lại nếu không muốn, vì checkbox đã phản ánh trạng thái mới
        }
    }
}
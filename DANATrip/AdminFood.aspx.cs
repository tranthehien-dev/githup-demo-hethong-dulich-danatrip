using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminFood : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: kiểm tra quyền admin
            if (!IsPostBack)
            {
                LoadFoods();
            }
        }

        void LoadFoods(string keyword = "")
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT MaMon,
                           TenMon,
                           LEFT(ISNULL(MoTa, ''), 80) + CASE WHEN LEN(ISNULL(MoTa,'')) > 80 THEN '...' ELSE '' END AS MoTaNgan,
                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                           ISNULL(HienThi, 1) AS HienThi
                    FROM AmThuc";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    cmd.CommandText += " WHERE TenMon LIKE @kw";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                cmd.CommandText += " ORDER BY TenMon ASC";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptFoods.DataSource = dt;
            rptFoods.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadFoods(txtSearch.Text.Trim());
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminFoodEdit.aspx");
        }

        protected void rptFoods_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maMon = e.CommandArgument.ToString();

            if (e.CommandName == "Edit")
            {
                Response.Redirect("AdminFoodEdit.aspx?mamon=" + maMon);
            }
            else if (e.CommandName == "Delete")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Xóa các bảng con trước nếu có FK
                    cmd.CommandText = @"
                        DELETE FROM AmThuc_HinhAnh WHERE MaMon = @id;
                        DELETE FROM AmThuc_NguyenLieu WHERE MaMon = @id;
                        DELETE FROM AmThuc_QuanAn WHERE MaMon = @id;
                        DELETE FROM AmThuc_QuyTrinhCheBien WHERE MaMon = @id;
                        DELETE FROM AmThuc WHERE MaMon = @id;";
                    cmd.Parameters.AddWithValue("@id", maMon);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadFoods(txtSearch.Text.Trim());
            }
        }

        protected void chkHienThi_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            RepeaterItem item = (RepeaterItem)chk.NamingContainer;
            HiddenField hf = (HiddenField)item.FindControl("hfMaMon");

            string maMon = hf.Value;
            bool hienThi = chk.Checked;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE AmThuc SET HienThi = @ht WHERE MaMon = @id";
                cmd.Parameters.AddWithValue("@ht", hienThi);
                cmd.Parameters.AddWithValue("@id", maMon);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
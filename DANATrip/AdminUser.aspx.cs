using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminUser : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: Kiểm tra quyền admin (chỉ admin mới vào được)
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        void LoadUsers(string keyword = "")
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT MaNguoiDung, HoTen, Email, VaiTro,
                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                           ISNULL(HienThi, 1) AS HienThi
                    FROM NguoiDung";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    cmd.CommandText += " WHERE HoTen LIKE @kw OR Email LIKE @kw";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                cmd.CommandText += " ORDER BY NgayTao DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptUsers.DataSource = dt;
            rptUsers.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text.Trim());
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminUserEdit.aspx");
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maNguoiDung = e.CommandArgument.ToString();

            if (e.CommandName == "Edit")
            {
                Response.Redirect("AdminUserEdit.aspx?id=" + maNguoiDung);
            }
            else if (e.CommandName == "Delete")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM NguoiDung WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@id", maNguoiDung);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadUsers(txtSearch.Text.Trim());
            }
        }

        protected void chkHienThi_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            RepeaterItem item = (RepeaterItem)chk.NamingContainer;
            HiddenField hf = (HiddenField)item.FindControl("hfMaNguoiDung");

            string maNguoiDung = hf.Value;
            bool hienThi = chk.Checked;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE NguoiDung
                    SET HienThi = @ht,
                        TrangThai = CASE WHEN @ht = 1 THEN N'Hoạt động' ELSE N'Bị khóa' END
                    WHERE MaNguoiDung = @id";
                cmd.Parameters.AddWithValue("@ht", hienThi);
                cmd.Parameters.AddWithValue("@id", maNguoiDung);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // không cần LoadUsers lại; checkbox đã phản ánh trạng thái
        }
    }
}
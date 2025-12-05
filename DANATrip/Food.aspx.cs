using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class AmThuc : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadAmThuc();
        }

        void LoadAmThuc(string keyword = "")
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                if (keyword == "")
                {
                    cmd.CommandText = @"SELECT MaMon, TenMon, MoTa, HinhAnh
                        FROM AmThuc
                        WHERE ISNULL(HienThi,1) = 1
                        ORDER BY TenMon ASC";
                }
                else
                {
                    cmd.CommandText = @"SELECT MaMon, TenMon, MoTa, HinhAnh
                        FROM AmThuc
                        WHERE ISNULL(HienThi,1) = 1
                          AND TenMon LIKE @kw
                        ORDER BY TenMon ASC";
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            rptAmThuc.DataSource = dt;
            rptAmThuc.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadAmThuc(txtSearch.Text.Trim());
        }
    }
}

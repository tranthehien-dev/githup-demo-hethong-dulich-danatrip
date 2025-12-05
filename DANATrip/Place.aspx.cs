using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class DiaDiem : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadDiaDiem();
        }

        void LoadDiaDiem(string keyword = "")
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                if (keyword == "")
                {
                    cmd.CommandText = @"SELECT MaDiaDiem, TenDiaDiem, NoiDung, HinhAnhChinh 
                        FROM DiaDiem 
                        WHERE ISNULL(HienThi, 1) = 1
                        ORDER BY TenDiaDiem ASC";
                }
                else
                {
                    cmd.CommandText = @"SELECT MaDiaDiem, TenDiaDiem, NoiDung, HinhAnhChinh 
                        FROM DiaDiem
                        WHERE ISNULL(HienThi, 1) = 1
                          AND TenDiaDiem LIKE @kw
                        ORDER BY TenDiaDiem ASC";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            rptDiaDiem.DataSource = dt;
            rptDiaDiem.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDiaDiem(txtSearch.Text.Trim());
        }
    }
}

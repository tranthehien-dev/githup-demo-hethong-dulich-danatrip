using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class Tour : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTours();
            }
        }

        void LoadTours(string keyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT t.MaTour, t.TenTour, t.GiaNguoiLon,
                           (SELECT TOP 1 UrlAnh FROM TourImages WHERE MaTour = t.MaTour) AS UrlAnh,
                           STUFF((SELECT ', ' + TenTag
                                  FROM TourTagMapping tm
                                  JOIN TourTag tg ON tm.MaTag = tg.MaTag
                                  WHERE tm.MaTour = t.MaTour
                                  FOR XML PATH('')), 1, 2, '') AS Tags
                    FROM Tour t
                    WHERE ISNULL(t.HienThi,1) = 1
                      AND t.TenTour LIKE @kw
                    ORDER BY t.MaTour";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptTours.DataSource = dt;
                rptTours.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadTours(txtSearch.Text.Trim());
        }
    }
}

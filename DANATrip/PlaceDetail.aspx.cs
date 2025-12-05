using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DANATrip
{
    public partial class DiaDiemChiTiet : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];
                if (string.IsNullOrEmpty(id))
                {
                    Response.Redirect("DiaDiem.aspx");
                    return;
                }

                LoadDiaDiem(id);
                LoadAlbum(id);
                LoadThamQuan(id);
                LoadThongTin(id);
                Load360(id);
                LoadTour(id);
            }
        }

        void LoadDiaDiem(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TenDiaDiem, NoiDung, HinhAnhChinh, ViTri, ISNULL(HienThi,1) AS HienThi FROM DiaDiem WHERE MaDiaDiem=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    bool hienThi = Convert.ToBoolean(rd["HienThi"]);
                    if (!hienThi)
                    {
                        Response.Redirect("Place.aspx");
                        return;
                    }
                    lblTenDiaDiem.InnerText = rd["TenDiaDiem"].ToString();
                    lblNoiDung.InnerText = rd["NoiDung"].ToString();
                    lblViTri.InnerText = rd["ViTri"].ToString();
                    //heroImg.Style["background-image"] = "url('" + rd["HinhAnhChinh"].ToString() + "')";
                }
                else
                {
                    Response.Redirect("Place.aspx");
                    return;
                }
            }
        }

        void LoadAlbum(string id)
        {
            rptAlbum.DataSource = GetData("SELECT UrlAnh FROM DiaDiem_HinhAnh WHERE MaDiaDiem='" + id + "'");
            rptAlbum.DataBind();
        }

        void LoadThamQuan(string id)
        {
            rptThamQuan.DataSource = GetData("SELECT * FROM DiaDiem_DiemThamQuan WHERE MaDiaDiem='" + id + "'");
            rptThamQuan.DataBind();
        }

        void LoadThongTin(string id)
        {
            rptThongTin.DataSource = GetData("SELECT * FROM DiaDiem_ThongTin WHERE MaDiaDiem='" + id + "'");
            rptThongTin.DataBind();
        }

        void Load360(string id)
        {
            DataTable dt = GetData("SELECT * FROM DiaDiem360 WHERE MaDiaDiem='" + id + "'");

            if (dt.Rows.Count > 0)
            {
                pnl360.Visible = true;
                rpt360.DataSource = dt;
                rpt360.DataBind();

                // Gán link đầu tiên cho iframe chính
                string firstLink = dt.Rows[0]["Link360"]?.ToString();
                if (!string.IsNullOrEmpty(firstLink))
                {
                    main360.Src = firstLink;
                }
            }
        }

        void LoadTour(string maDiaDiem)
        {
            string sql = @"
                SELECT t.MaTour, t.MaDiaDiem, t.TenTour, t.MoTaNgan, t.MoTaChiTiet,
                       t.GiaNguoiLon, t.NgayKhoiHanh,
                       d.HinhAnhChinh AS AnhTour
                FROM Tour t
                LEFT JOIN DiaDiem d ON t.MaDiaDiem = d.MaDiaDiem
                WHERE t.MaDiaDiem = @MaDiaDiem
                ORDER BY t.CreatedAt DESC";
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaDiaDiem", SqlDbType.NVarChar, 50).Value = maDiaDiem;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptTour.DataSource = dt;
            rptTour.DataBind();
        }

        DataTable GetData(string query)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}

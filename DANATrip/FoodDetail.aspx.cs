using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class AmThucChiTiet : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ✔ Chỉ nhận ?mamon=
                string maMon = Request.QueryString["mamon"];

                if (string.IsNullOrEmpty(maMon))
                {
                    Response.Redirect("AmThuc.aspx");
                    return;
                }

                LoadMonAn(maMon);
                LoadHinhAnh(maMon);
                LoadNguyenLieu(maMon);
                LoadQuyTrinh(maMon);
                LoadQuanAn(maMon);
            }
        }

        // ======================= LOAD MÓN ĂN =======================
        void LoadMonAn(string maMon)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TenMon, MoTa FROM AmThuc WHERE MaMon = @MaMon", con))
            {
                cmd.Parameters.AddWithValue("@MaMon", maMon);

                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    lblTenMon.Text = rd["TenMon"].ToString();
                    lblTenMon2.Text = rd["TenMon"].ToString();
                    lblMoTa.Text = rd["MoTa"].ToString();
                    ltMoTaChiTiet.Text = rd["MoTa"].ToString();
                }
            }
        }

        // ======================= LOAD HÌNH ẢNH =======================
        void LoadHinhAnh(string maMon)
        {
            rpHinhAnh.DataSource = GetData(
                "SELECT UrlAnh FROM AmThuc_HinhAnh WHERE MaMon = @MaMon",
                maMon
            );
            rpHinhAnh.DataBind();
        }

        // ======================= LOAD NGUYÊN LIỆU =======================
        void LoadNguyenLieu(string maMon)
        {
            rpNguyenLieu.DataSource = GetData(
                "SELECT TenNguyenLieu FROM AmThuc_NguyenLieu WHERE MaMon = @MaMon",
                maMon
            );
            rpNguyenLieu.DataBind();
        }

        // ======================= LOAD QUY TRÌNH CHẾ BIẾN =======================
        void LoadQuyTrinh(string maMon)
        {
            rpQuyTrinh.DataSource = GetData(
                "SELECT MaBuocCheBien AS ThuTu, MoTaBuoc, ThoiGian FROM AmThuc_QuyTrinhCheBien WHERE MaMon = @MaMon ORDER BY MaBuocCheBien",
                 maMon
            );
            rpQuyTrinh.DataBind();
        }

        // ======================= LOAD QUÁN ĂN =======================
        void LoadQuanAn(string maMon)
        {
            rpQuanAn.DataSource = GetData(
                "SELECT TenQuanAn, DiaChi, Sdt FROM AmThuc_QuanAn WHERE MaMon = @MaMon",
                 maMon
            );
            rpQuanAn.DataBind();
        }

        // ======================= HÀM LẤY DATA DÙNG CHUNG =======================
        DataTable GetData(string query, string maMon)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminFoodEdit : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        const string FoodImageFolder = "~/Images/Food/";

        bool IsEditMode => !string.IsNullOrEmpty(Request.QueryString["mamon"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: check quyền admin

            if (!IsPostBack)
            {
                if (IsEditMode)
                {
                    litTitle.Text = "Chỉnh sửa Món ăn";
                    txtMaMon.Enabled = false;
                    LoadMon(Request.QueryString["mamon"]);
                }
                else
                {
                    litTitle.Text = "Thêm món ăn mới";
                    chkHienThiEdit.Checked = true;
                }
            }
        }

        #region LOAD DATA

        void LoadMon(string maMon)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaMon, TenMon, MoTa, HinhAnh,
                                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                                           ISNULL(HienThi, 1) AS HienThi
                                    FROM AmThuc
                                    WHERE MaMon = @id";
                cmd.Parameters.AddWithValue("@id", maMon);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaMon.Text = rd["MaMon"].ToString();
                        txtTenMon.Text = rd["TenMon"].ToString();
                        txtMoTa.Text = rd["MoTa"].ToString();
                        txtHinhAnhChinh.Text = rd["HinhAnh"].ToString();
                        ddlTrangThai.SelectedValue = rd["TrangThai"].ToString();
                        chkHienThiEdit.Checked = Convert.ToBoolean(rd["HienThi"]);

                        string imgUrl = rd["HinhAnh"]?.ToString();
                        if (!string.IsNullOrEmpty(imgUrl))
                            imgHinhChinh.ImageUrl = imgUrl;
                    }
                }
            }

            LoadAlbum(maMon);
            LoadNguyenLieu(maMon);
            LoadQuyTrinh(maMon);
            LoadQuanAn(maMon);
        }

        void LoadAlbum(string maMon)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaAnh, MaMon, UrlAnh
                                    FROM AmThuc_HinhAnh
                                    WHERE MaMon = @id
                                    ORDER BY MaAnh";
                cmd.Parameters.AddWithValue("@id", maMon);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptAlbum.DataSource = dt;
                    rptAlbum.DataBind();
                }
            }
        }

        void LoadNguyenLieu(string maMon)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaNguyenLieu, TenNguyenLieu, MoTa, MaMon
                                    FROM AmThuc_NguyenLieu
                                    WHERE MaMon = @id
                                    ORDER BY MaNguyenLieu";
                cmd.Parameters.AddWithValue("@id", maMon);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptNguyenLieu.DataSource = dt;
                    rptNguyenLieu.DataBind();
                }
            }
        }

        void LoadQuyTrinh(string maMon)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaBuocCheBien, MaMon, MoTaBuoc, ThoiGian
                                    FROM AmThuc_QuyTrinhCheBien
                                    WHERE MaMon = @id
                                    ORDER BY MaBuocCheBien";
                cmd.Parameters.AddWithValue("@id", maMon);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptQuyTrinh.DataSource = dt;
                    rptQuyTrinh.DataBind();
                }
            }
        }

        void LoadQuanAn(string maMon)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaQuanAn, TenQuanAn, DiaChi, Sdt, MaMon
                                    FROM AmThuc_QuanAn
                                    WHERE MaMon = @id
                                    ORDER BY MaQuanAn";
                cmd.Parameters.AddWithValue("@id", maMon);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptQuanAn.DataSource = dt;
                    rptQuanAn.DataBind();
                }
            }
        }

        #endregion

        #region SAVE THÔNG TIN CHUNG

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMaMon.Text.Trim();
            string ten = txtTenMon.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                lblMsg.Text = "Mã món và Tên món không được để trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            // upload hình chính nếu có
            string imgPath = txtHinhAnhChinh.Text.Trim();
            if (fuHinhChinh.HasFile)
            {
                string fileName = Path.GetFileName(fuHinhChinh.FileName);
                string folderPhysical = Server.MapPath(FoodImageFolder);
                if (!Directory.Exists(folderPhysical)) Directory.CreateDirectory(folderPhysical);
                string fullPath = Path.Combine(folderPhysical, fileName);
                fuHinhChinh.SaveAs(fullPath);

                // LƯU DB không có ~
                imgPath = "Images/Food/" + fileName;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                if (IsEditMode)
                {
                    cmd.CommandText = @"
                        UPDATE AmThuc
                        SET TenMon = @Ten,
                            MoTa = @MoTa,
                            HinhAnh = @HinhAnh,
                            TrangThai = @TrangThai,
                            HienThi = @HienThi
                        WHERE MaMon = @Ma";
                }
                else
                {
                    cmd.CommandText = @"
                        INSERT INTO AmThuc (MaMon, TenMon, MoTa, HinhAnh, TrangThai, HienThi)
                        VALUES (@Ma, @Ten, @MoTa, @HinhAnh, @TrangThai, @HienThi)";
                }

                cmd.Parameters.AddWithValue("@Ma", ma);
                cmd.Parameters.AddWithValue("@Ten", ten);
                cmd.Parameters.AddWithValue("@MoTa", (object)txtMoTa.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)imgPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", ddlTrangThai.SelectedValue);
                cmd.Parameters.AddWithValue("@HienThi", chkHienThiEdit.Checked);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            imgHinhChinh.ImageUrl = imgPath;
            txtHinhAnhChinh.Text = imgPath;

            lblMsg.Text = "Lưu thông tin món ăn thành công.";
            lblMsg.CssClass = "msg success";

            if (!IsEditMode)
            {
                Response.Redirect("AdminFoodEdit.aspx?mamon=" + ma);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminFood.aspx");
        }

        #endregion

        #region ALBUM ẢNH

        string GenerateNewMaAnh()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT MAX(MaAnh) FROM AmThuc_HinhAnh";
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return "A1";

                string last = result.ToString(); // A1, A2,...
                string prefix = "";
                string digits = "";
                foreach (char c in last)
                {
                    if (!char.IsDigit(c)) prefix += c;
                    else digits += c;
                }
                if (string.IsNullOrEmpty(prefix)) prefix = "A";
                if (string.IsNullOrEmpty(digits)) digits = "0";

                int num = int.Parse(digits) + 1;
                return $"{prefix}{num}";
            }
        }

        protected void btnAddAlbum_Click(object sender, EventArgs e)
        {
            if (!fuAlbum.HasFile)
            {
                lblMsg.Text = "Vui lòng chọn file ảnh để thêm vào album.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maMon = txtMaMon.Text.Trim();
            if (string.IsNullOrEmpty(maMon))
            {
                lblMsg.Text = "Bạn cần lưu thông tin món ăn trước khi thêm ảnh.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string fileName = Path.GetFileName(fuAlbum.FileName);
            string folderPhysical = Server.MapPath(FoodImageFolder);
            if (!Directory.Exists(folderPhysical)) Directory.CreateDirectory(folderPhysical);
            string fullPath = Path.Combine(folderPhysical, fileName);
            fuAlbum.SaveAs(fullPath);

            string urlAnh = "Images/Food/" + fileName;
            string maAnhMoi = GenerateNewMaAnh();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO AmThuc_HinhAnh (MaAnh, MaMon, UrlAnh)
                    VALUES (@MaAnh, @MaMon, @UrlAnh)";
                cmd.Parameters.AddWithValue("@MaAnh", maAnhMoi);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                cmd.Parameters.AddWithValue("@UrlAnh", urlAnh);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm ảnh album thành công.";
            lblMsg.CssClass = "msg success";
            LoadAlbum(maMon);
        }

        protected void rptAlbum_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            string maAnh = e.CommandArgument.ToString();

            if (e.CommandName == "SaveImg")
            {
                TextBox txtUrl = (TextBox)e.Item.FindControl("txtUrlAnh");
                string url = txtUrl.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE AmThuc_HinhAnh SET UrlAnh = @url WHERE MaAnh = @id";
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@id", maAnh);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Cập nhật URL ảnh thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "DelImg")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM AmThuc_HinhAnh WHERE MaAnh = @id";
                    cmd.Parameters.AddWithValue("@id", maAnh);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa ảnh khỏi album thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadAlbum(maMon);
        }

        #endregion

        #region NGUYÊN LIỆU

        protected void btnAddNL_Click(object sender, EventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            if (string.IsNullOrEmpty(maMon))
            {
                lblMsg.Text = "Hãy lưu món ăn trước khi thêm nguyên liệu.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maNL = txtNewMaNL.Text.Trim();
            string tenNL = txtNewTenNL.Text.Trim();
            string moTa = txtNewMoTaNL.Text.Trim();

            if (string.IsNullOrEmpty(maNL) || string.IsNullOrEmpty(tenNL))
            {
                lblMsg.Text = "Mã nguyên liệu và Tên nguyên liệu không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO AmThuc_NguyenLieu (MaNguyenLieu, TenNguyenLieu, MoTa, MaMon)
                    VALUES (@MaNL, @TenNL, @MoTa, @MaMon)";
                cmd.Parameters.AddWithValue("@MaNL", maNL);
                cmd.Parameters.AddWithValue("@TenNL", tenNL);
                cmd.Parameters.AddWithValue("@MoTa", (object)moTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm nguyên liệu thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMaNL.Text = txtNewTenNL.Text = txtNewMoTaNL.Text = "";
            LoadNguyenLieu(maMon);
        }

        protected void rptNguyenLieu_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            string maNL = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtTen = (TextBox)e.Item.FindControl("txtTenNL");
                TextBox txtMoTa = (TextBox)e.Item.FindControl("txtMoTaNL");

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE AmThuc_NguyenLieu
                        SET TenNguyenLieu = @Ten, MoTa = @MoTa
                        WHERE MaNguyenLieu = @MaNL";
                    cmd.Parameters.AddWithValue("@Ten", txtTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@MoTa", (object)txtMoTa.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaNL", maNL);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu nguyên liệu thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM AmThuc_NguyenLieu WHERE MaNguyenLieu = @id";
                    cmd.Parameters.AddWithValue("@id", maNL);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa nguyên liệu thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadNguyenLieu(maMon);
        }

        #endregion

        #region QUY TRÌNH CHẾ BIẾN

        protected void btnAddBuoc_Click(object sender, EventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            if (string.IsNullOrEmpty(maMon))
            {
                lblMsg.Text = "Hãy lưu món ăn trước khi thêm bước chế biến.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maBuoc = txtNewMaBuoc.Text.Trim();
            string moTa = txtNewMoTaBuoc.Text.Trim();
            string thoiGianStr = txtNewThoiGian.Text.Trim();
            int thoiGian = 0;
            int.TryParse(thoiGianStr, out thoiGian);

            if (string.IsNullOrEmpty(maBuoc) || string.IsNullOrEmpty(moTa))
            {
                lblMsg.Text = "Mã bước và Mô tả bước không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO AmThuc_QuyTrinhCheBien (MaBuocCheBien, MaMon, MoTaBuoc, ThoiGian)
                    VALUES (@MaBuoc, @MaMon, @MoTa, @ThoiGian)";
                cmd.Parameters.AddWithValue("@MaBuoc", maBuoc);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                cmd.Parameters.AddWithValue("@MoTa", moTa);
                cmd.Parameters.AddWithValue("@ThoiGian", thoiGian);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm bước chế biến thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMaBuoc.Text = txtNewMoTaBuoc.Text = txtNewThoiGian.Text = "";
            LoadQuyTrinh(maMon);
        }

        protected void rptQuyTrinh_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            string maBuoc = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtMoTa = (TextBox)e.Item.FindControl("txtMoTaBuoc");
                TextBox txtThoiGian = (TextBox)e.Item.FindControl("txtThoiGian");

                int thoiGian = 0;
                int.TryParse(txtThoiGian.Text.Trim(), out thoiGian);

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE AmThuc_QuyTrinhCheBien
                        SET MoTaBuoc = @MoTa, ThoiGian = @ThoiGian
                        WHERE MaBuocCheBien = @MaBuoc AND MaMon = @MaMon";
                    cmd.Parameters.AddWithValue("@MoTa", txtMoTa.Text.Trim());
                    cmd.Parameters.AddWithValue("@ThoiGian", thoiGian);
                    cmd.Parameters.AddWithValue("@MaBuoc", maBuoc);
                    cmd.Parameters.AddWithValue("@MaMon", maMon);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu bước chế biến thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM AmThuc_QuyTrinhCheBien WHERE MaBuocCheBien = @MaBuoc AND MaMon = @MaMon";
                    cmd.Parameters.AddWithValue("@MaBuoc", maBuoc);
                    cmd.Parameters.AddWithValue("@MaMon", maMon);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa bước chế biến thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadQuyTrinh(maMon);
        }

        #endregion

        #region QUÁN ĂN

        protected void btnAddQA_Click(object sender, EventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            if (string.IsNullOrEmpty(maMon))
            {
                lblMsg.Text = "Hãy lưu món ăn trước khi thêm quán ăn.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maQA = txtNewMaQA.Text.Trim();
            string tenQA = txtNewTenQA.Text.Trim();
            string diaChi = txtNewDiaChiQA.Text.Trim();
            string sdt = txtNewSdtQA.Text.Trim();

            if (string.IsNullOrEmpty(maQA) || string.IsNullOrEmpty(tenQA))
            {
                lblMsg.Text = "Mã quán và Tên quán không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO AmThuc_QuanAn (MaQuanAn, TenQuanAn, DiaChi, Sdt, MaMon)
                    VALUES (@MaQA, @TenQA, @DiaChi, @Sdt, @MaMon)";
                cmd.Parameters.AddWithValue("@MaQA", maQA);
                cmd.Parameters.AddWithValue("@TenQA", tenQA);
                cmd.Parameters.AddWithValue("@DiaChi", (object)diaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sdt", (object)sdt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm quán ăn thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMaQA.Text = txtNewTenQA.Text = txtNewDiaChiQA.Text = txtNewSdtQA.Text = "";
            LoadQuanAn(maMon);
        }

        protected void rptQuanAn_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            string maQA = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtTen = (TextBox)e.Item.FindControl("txtTenQA");
                TextBox txtDiaChi = (TextBox)e.Item.FindControl("txtDiaChiQA");
                TextBox txtSdt = (TextBox)e.Item.FindControl("txtSdtQA");

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE AmThuc_QuanAn
                        SET TenQuanAn = @Ten, DiaChi = @DiaChi, Sdt = @Sdt
                        WHERE MaQuanAn = @MaQA";
                    cmd.Parameters.AddWithValue("@Ten", txtTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@DiaChi", (object)txtDiaChi.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sdt", (object)txtSdt.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaQA", maQA);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu quán ăn thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM AmThuc_QuanAn WHERE MaQuanAn = @id";
                    cmd.Parameters.AddWithValue("@id", maQA);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa quán ăn thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadQuanAn(maMon);
        }

        #endregion
    }
}
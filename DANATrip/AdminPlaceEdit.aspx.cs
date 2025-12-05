using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminPlaceEdit : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        const string MainImageFolder = "~/Images/Place/";
        const string AlbumFolder = "~/Images/Place/";
        const string DiemFolder = "~/Images/Place/";   // có thể tách riêng nếu muốn
        const string Thumb360Folder = "~/Images/360/";

        bool IsEditMode => !string.IsNullOrEmpty(Request.QueryString["id"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: kiểm tra quyền admin

            if (!IsPostBack)
            {
                if (IsEditMode)
                {
                    litTitle.Text = "Chỉnh sửa Địa điểm";
                    txtMaDiaDiem.Enabled = false;
                    LoadPlace(Request.QueryString["id"]);
                }
                else
                {
                    litTitle.Text = "Thêm Địa điểm mới";
                    chkHienThiEdit.Checked = true;
                }
            }
        }

        #region LOAD DATA

        void LoadPlace(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaDiaDiem, TenDiaDiem, NoiDung, HinhAnhChinh, ViTri,
                                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                                           ISNULL(HienThi, 1) AS HienThi
                                    FROM DiaDiem
                                    WHERE MaDiaDiem = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaDiaDiem.Text = rd["MaDiaDiem"].ToString();
                        txtTenDiaDiem.Text = rd["TenDiaDiem"].ToString();
                        txtNoiDung.Text = rd["NoiDung"].ToString();
                        txtHinhAnhChinh.Text = rd["HinhAnhChinh"].ToString();
                        txtViTri.Text = rd["ViTri"].ToString();
                        ddlTrangThai.SelectedValue = rd["TrangThai"].ToString();
                        chkHienThiEdit.Checked = Convert.ToBoolean(rd["HienThi"]);

                        string imgUrl = rd["HinhAnhChinh"]?.ToString();
                        if (!string.IsNullOrEmpty(imgUrl))
                            imgHinhChinh.ImageUrl = imgUrl;
                    }
                }
            }

            LoadAlbumImages(id);
            LoadThamQuan(id);
            LoadThongTin(id);
            Load360(id);
        }

        void LoadAlbumImages(string maDiaDiem)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaAnh, MaDiaDiem, UrlAnh
                                    FROM DiaDiem_HinhAnh
                                    WHERE MaDiaDiem = @id
                                    ORDER BY MaAnh";
                cmd.Parameters.AddWithValue("@id", maDiaDiem);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptAlbum.DataSource = dt;
                    rptAlbum.DataBind();
                }
            }
        }

        void LoadThamQuan(string maDiaDiem)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaDiem, MaDiaDiem, TenDiem, MoTa, HinhAnh
                                    FROM DiaDiem_DiemThamQuan
                                    WHERE MaDiaDiem = @id
                                    ORDER BY MaDiem";
                cmd.Parameters.AddWithValue("@id", maDiaDiem);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptThamQuan.DataSource = dt;
                    rptThamQuan.DataBind();
                }
            }
        }

        void LoadThongTin(string maDiaDiem)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaInfo, MaDiaDiem, TieuDe, NoiDung
                                    FROM DiaDiem_ThongTin
                                    WHERE MaDiaDiem = @id
                                    ORDER BY MaInfo";
                cmd.Parameters.AddWithValue("@id", maDiaDiem);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptThongTin.DataSource = dt;
                    rptThongTin.DataBind();
                }
            }
        }

        void Load360(string maDiaDiem)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT Ma360, MaDiaDiem, TieuDe, Link360, Thumbnail
                                    FROM DiaDiem360
                                    WHERE MaDiaDiem = @id
                                    ORDER BY Ma360";
                cmd.Parameters.AddWithValue("@id", maDiaDiem);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rpt360.DataSource = dt;
                    rpt360.DataBind();
                }
            }
        }

        #endregion

        #region SAVE THÔNG TIN CHUNG

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMaDiaDiem.Text.Trim();
            string ten = txtTenDiaDiem.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                lblMsg.Text = "Mã địa điểm và Tên địa điểm không được để trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            // upload hình chính nếu có
            string imgPath = txtHinhAnhChinh.Text.Trim();
            if (fuHinhChinh.HasFile)
            {
                string fileName = Path.GetFileName(fuHinhChinh.FileName);
                string folderPhysical = Server.MapPath("~/Images/Place/");
                if (!Directory.Exists(folderPhysical))
                    Directory.CreateDirectory(folderPhysical);

                string fullPath = Path.Combine(folderPhysical, fileName);
                fuHinhChinh.SaveAs(fullPath);

                // LƯU VÀO DB KHÔNG ~
                imgPath = "Images/Place/" + fileName;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                if (IsEditMode)
                {
                    cmd.CommandText = @"
                        UPDATE DiaDiem
                        SET TenDiaDiem = @Ten,
                            NoiDung = @NoiDung,
                            HinhAnhChinh = @HinhAnh,
                            ViTri = @ViTri,
                            TrangThai = @TrangThai,
                            HienThi = @HienThi
                        WHERE MaDiaDiem = @Ma";
                }
                else
                {
                    cmd.CommandText = @"
                        INSERT INTO DiaDiem (MaDiaDiem, TenDiaDiem, NoiDung, HinhAnhChinh, ViTri, TrangThai, HienThi)
                        VALUES (@Ma, @Ten, @NoiDung, @HinhAnh, @ViTri, @TrangThai, @HienThi)";
                }

                cmd.Parameters.AddWithValue("@Ma", ma);
                cmd.Parameters.AddWithValue("@Ten", ten);
                cmd.Parameters.AddWithValue("@NoiDung", (object)txtNoiDung.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)imgPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ViTri", (object)txtViTri.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", ddlTrangThai.SelectedValue);
                cmd.Parameters.AddWithValue("@HienThi", chkHienThiEdit.Checked);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            imgHinhChinh.ImageUrl = imgPath;
            txtHinhAnhChinh.Text = imgPath;

            lblMsg.Text = "Lưu thông tin địa điểm thành công.";
            lblMsg.CssClass = "msg success";

            if (!IsEditMode)
            {
                // chuyển sang chế độ edit với id mới
                Response.Redirect("AdminPlaceEdit.aspx?id=" + ma);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPlace.aspx");
        }

        #endregion

        #region ALBUM ẢNH

        protected void btnAddAlbum_Click(object sender, EventArgs e)
        {
            if (!fuAlbum.HasFile)
            {
                lblMsg.Text = "Vui lòng chọn file ảnh để thêm vào album.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            if (string.IsNullOrEmpty(maDiaDiem))
            {
                lblMsg.Text = "Bạn cần lưu thông tin địa điểm trước khi thêm ảnh album.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string fileName = Path.GetFileName(fuAlbum.FileName);
            string folderPhysical = Server.MapPath("~/Images/Place/");
            if (!Directory.Exists(folderPhysical))
                Directory.CreateDirectory(folderPhysical);

            string fullPath = Path.Combine(folderPhysical, fileName);
            fuAlbum.SaveAs(fullPath);
            string urlAnh = "Images/Place/" + fileName;
            string maAnhMoi = GenerateNewMaAnh();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            INSERT INTO DiaDiem_HinhAnh (MaAnh, MaDiaDiem, UrlAnh)
            VALUES (@MaAnh, @MaDiaDiem, @UrlAnh)";
                cmd.Parameters.AddWithValue("@MaAnh", maAnhMoi);
                cmd.Parameters.AddWithValue("@MaDiaDiem", maDiaDiem);
                cmd.Parameters.AddWithValue("@UrlAnh", urlAnh);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm ảnh album thành công.";
            lblMsg.CssClass = "msg success";
            LoadAlbumImages(maDiaDiem);
        }

        protected void rptAlbum_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            string maAnh = e.CommandArgument.ToString();

            if (e.CommandName == "SaveImg")
            {
                TextBox txtUrl = (TextBox)e.Item.FindControl("txtUrlAnh");
                string url = txtUrl.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE DiaDiem_HinhAnh SET UrlAnh = @url WHERE MaAnh = @id";
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
                    cmd.CommandText = "DELETE FROM DiaDiem_HinhAnh WHERE MaAnh = @id";
                    cmd.Parameters.AddWithValue("@id", maAnh);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa ảnh khỏi album thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadAlbumImages(maDiaDiem);
        }

        #endregion

        #region ĐIỂM THAM QUAN

        protected void btnAddDiem_Click(object sender, EventArgs e)
        {
            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            if (string.IsNullOrEmpty(maDiaDiem))
            {
                lblMsg.Text = "Hãy lưu địa điểm trước khi thêm điểm tham quan.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maDiem = txtNewMaDiem.Text.Trim();
            string tenDiem = txtNewTenDiem.Text.Trim();
            string moTa = txtNewMoTa.Text.Trim();

            if (string.IsNullOrEmpty(maDiem) || string.IsNullOrEmpty(tenDiem))
            {
                lblMsg.Text = "Mã điểm và Tên điểm không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string hinh = null;
            if (fuNewHinhDiem.HasFile)
            {
                string fileName = Path.GetFileName(fuNewHinhDiem.FileName);
                string folderPhysical = Server.MapPath("~/Images/Place/");
                if (!Directory.Exists(folderPhysical))
                    Directory.CreateDirectory(folderPhysical);

                string fullPath = Path.Combine(folderPhysical, fileName);
                fuNewHinhDiem.SaveAs(fullPath);

                hinh = "Images/Place/" + fileName;   // lưu vào DB
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO DiaDiem_DiemThamQuan (MaDiem, MaDiaDiem, TenDiem, MoTa, HinhAnh)
                    VALUES (@MaDiem, @MaDiaDiem, @TenDiem, @MoTa, @HinhAnh)";
                cmd.Parameters.AddWithValue("@MaDiem", maDiem);
                cmd.Parameters.AddWithValue("@MaDiaDiem", maDiaDiem);
                cmd.Parameters.AddWithValue("@TenDiem", tenDiem);
                cmd.Parameters.AddWithValue("@MoTa", (object)moTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)hinh ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm điểm tham quan thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMaDiem.Text = txtNewTenDiem.Text = txtNewMoTa.Text = "";
            LoadThamQuan(maDiaDiem);
        }

        protected void rptThamQuan_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            string maDiem = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtTen = (TextBox)e.Item.FindControl("txtTenDiem");
                TextBox txtMoTa = (TextBox)e.Item.FindControl("txtMoTa");
                TextBox txtHinh = (TextBox)e.Item.FindControl("txtHinhAnh");

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE DiaDiem_DiemThamQuan
                        SET TenDiem = @Ten, MoTa = @MoTa, HinhAnh = @Hinh
                        WHERE MaDiem = @MaDiem";
                    cmd.Parameters.AddWithValue("@Ten", txtTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@MoTa", (object)txtMoTa.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Hinh", (object)txtHinh.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaDiem", maDiem);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu điểm tham quan thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM DiaDiem_DiemThamQuan WHERE MaDiem = @id";
                    cmd.Parameters.AddWithValue("@id", maDiem);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa điểm tham quan thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadThamQuan(maDiaDiem);
        }

        #endregion

        #region THÔNG TIN HỮU ÍCH

        protected void btnAddInfo_Click(object sender, EventArgs e)
        {
            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            if (string.IsNullOrEmpty(maDiaDiem))
            {
                lblMsg.Text = "Hãy lưu địa điểm trước khi thêm thông tin.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maInfo = txtNewMaInfo.Text.Trim();
            string tieuDe = txtNewTieuDe.Text.Trim();
            string noiDung = txtNewNoiDungInfo.Text.Trim();

            if (string.IsNullOrEmpty(maInfo) || string.IsNullOrEmpty(tieuDe))
            {
                lblMsg.Text = "Mã info và Tiêu đề không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO DiaDiem_ThongTin (MaInfo, MaDiaDiem, TieuDe, NoiDung)
                    VALUES (@MaInfo, @MaDiaDiem, @TieuDe, @NoiDung)";
                cmd.Parameters.AddWithValue("@MaInfo", maInfo);
                cmd.Parameters.AddWithValue("@MaDiaDiem", maDiaDiem);
                cmd.Parameters.AddWithValue("@TieuDe", tieuDe);
                cmd.Parameters.AddWithValue("@NoiDung", (object)noiDung ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm thông tin thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMaInfo.Text = txtNewTieuDe.Text = txtNewNoiDungInfo.Text = "";
            LoadThongTin(maDiaDiem);
        }

        protected void rptThongTin_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            string maInfo = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtTitle = (TextBox)e.Item.FindControl("txtTieuDe");
                TextBox txtContent = (TextBox)e.Item.FindControl("txtNoiDungInfo");

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE DiaDiem_ThongTin
                        SET TieuDe = @TieuDe, NoiDung = @NoiDung
                        WHERE MaInfo = @MaInfo";
                    cmd.Parameters.AddWithValue("@TieuDe", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@NoiDung", (object)txtContent.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaInfo", maInfo);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu thông tin thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM DiaDiem_ThongTin WHERE MaInfo = @id";
                    cmd.Parameters.AddWithValue("@id", maInfo);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa thông tin thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadThongTin(maDiaDiem);
        }

        #endregion

        #region 360 VIEW

        protected void btnAdd360_Click(object sender, EventArgs e)
        {
            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            if (string.IsNullOrEmpty(maDiaDiem))
            {
                lblMsg.Text = "Hãy lưu địa điểm trước khi thêm 360.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string ma360 = txtNewMa360.Text.Trim();
            string tieuDe = txtNewTieuDe360.Text.Trim();
            string link360 = txtNewLink360.Text.Trim();

            if (string.IsNullOrEmpty(ma360) || string.IsNullOrEmpty(link360))
            {
                lblMsg.Text = "Mã 360 và Link360 không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string thumb = null;
            if (fuNewThumb360.HasFile)
            {
                string fileName = Path.GetFileName(fuNewThumb360.FileName);
                string folderPhysical = Server.MapPath("~/Images/360/");
                if (!Directory.Exists(folderPhysical))
                    Directory.CreateDirectory(folderPhysical);

                string fullPath = Path.Combine(folderPhysical, fileName);
                fuNewThumb360.SaveAs(fullPath);

                thumb = "Images/360/" + fileName;   // lưu vào DB
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO DiaDiem360 (Ma360, MaDiaDiem, TieuDe, Link360, Thumbnail)
                    VALUES (@Ma360, @MaDiaDiem, @TieuDe, @Link360, @Thumb)";
                cmd.Parameters.AddWithValue("@Ma360", ma360);
                cmd.Parameters.AddWithValue("@MaDiaDiem", maDiaDiem);
                cmd.Parameters.AddWithValue("@TieuDe", (object)tieuDe ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Link360", link360);
                cmd.Parameters.AddWithValue("@Thumb", (object)thumb ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm góc 360 thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMa360.Text = txtNewTieuDe360.Text = txtNewLink360.Text = "";
            Load360(maDiaDiem);
        }

        protected void rpt360_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maDiaDiem = txtMaDiaDiem.Text.Trim();
            string ma360 = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtTitle = (TextBox)e.Item.FindControl("txtTieuDe360");
                TextBox txtLink = (TextBox)e.Item.FindControl("txtLink360");
                TextBox txtThumb = (TextBox)e.Item.FindControl("txtThumb");

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE DiaDiem360
                        SET TieuDe = @TieuDe, Link360 = @Link, Thumbnail = @Thumb
                        WHERE Ma360 = @Ma360";
                    cmd.Parameters.AddWithValue("@TieuDe", (object)txtTitle.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Link", txtLink.Text.Trim());
                    cmd.Parameters.AddWithValue("@Thumb", (object)txtThumb.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ma360", ma360);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu 360 thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM DiaDiem360 WHERE Ma360 = @id";
                    cmd.Parameters.AddWithValue("@id", ma360);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa 360 thành công.";
                lblMsg.CssClass = "msg success";
            }

            Load360(maDiaDiem);
        }

        #endregion
        private string GenerateNewMaAnh()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                // Lấy MaAnh lớn nhất hiện có trong toàn bảng
                cmd.CommandText = "SELECT MAX(MaAnh) FROM DiaDiem_HinhAnh";
                conn.Open();
                object result = cmd.ExecuteScalar();

                // Nếu chưa có dữ liệu, bắt đầu từ A001
                if (result == null || result == DBNull.Value)
                {
                    return "A001";
                }

                string last = result.ToString(); // ví dụ: "A005" hoặc "IMG010"

                // Tách phần chữ và phần số
                string prefix = "";
                string digits = "";
                foreach (char c in last)
                {
                    if (!char.IsDigit(c))
                        prefix += c;
                    else
                        digits += c;
                }

                if (string.IsNullOrEmpty(prefix)) prefix = "A";
                if (string.IsNullOrEmpty(digits)) digits = "0";

                int num = int.Parse(digits) + 1;

                // Giữ 3 chữ số: 001, 002, 010, 101...
                return $"{prefix}{num:000}";
            }
        }
    }
}
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminTourEdit : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        const string TourImageFolder = "~/Images/Tour/";

        bool IsEditMode => !string.IsNullOrEmpty(Request.QueryString["id"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: check quyền admin

            if (!IsPostBack)
            {
                if (IsEditMode)
                {
                    litTitle.Text = "Chỉnh sửa Tour";
                    txtMaTour.Enabled = false;
                    LoadTour(Request.QueryString["id"]);
                }
                else
                {
                    litTitle.Text = "Thêm tour mới";
                    chkHienThiEdit.Checked = true;
                }
            }
        }

        #region LOAD DATA

        void LoadTour(string maTour)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT MaTour, MaDiaDiem, TenTour, MoTaNgan, MoTaChiTiet,
                           GiaNguoiLon, GiaTreEm, ThoiLuong, PhuongTien,
                           NgayKhoiHanh, SoCho, SoChoDaDat,
                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                           ISNULL(HienThi,1) AS HienThi
                    FROM Tour
                    WHERE MaTour = @id";
                cmd.Parameters.AddWithValue("@id", maTour);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaTour.Text = rd["MaTour"].ToString();
                        txtMaDiaDiem.Text = rd["MaDiaDiem"].ToString();
                        txtTenTour.Text = rd["TenTour"].ToString();
                        txtMoTaNgan.Text = rd["MoTaNgan"].ToString();
                        txtMoTaChiTiet.Text = rd["MoTaChiTiet"].ToString();
                        txtGiaNguoiLon.Text = rd["GiaNguoiLon"].ToString();
                        txtGiaTreEm.Text = rd["GiaTreEm"].ToString();
                        txtThoiLuong.Text = rd["ThoiLuong"].ToString();
                        txtPhuongTien.Text = rd["PhuongTien"].ToString();
                        txtSoCho.Text = rd["SoCho"].ToString();
                        txtSoChoDaDat.Text = rd["SoChoDaDat"].ToString();
                        ddlTrangThai.SelectedValue = rd["TrangThai"].ToString();
                        chkHienThiEdit.Checked = Convert.ToBoolean(rd["HienThi"]);

                        if (rd["NgayKhoiHanh"] != DBNull.Value)
                        {
                            DateTime d = Convert.ToDateTime(rd["NgayKhoiHanh"]);
                            txtNgayKhoiHanh.Text = d.ToString("dd/MM/yyyy HH:mm");
                        }
                    }
                }
            }

            LoadImages(maTour);
            LoadHighlights(maTour);
            LoadSchedule(maTour);
            LoadIncludes(maTour);
            LoadTags(maTour);
        }

        void LoadImages(string maTour)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaAnh, MaTour, UrlAnh
                                    FROM TourImages
                                    WHERE MaTour = @id
                                    ORDER BY MaAnh";
                cmd.Parameters.AddWithValue("@id", maTour);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptImages.DataSource = dt;
                    rptImages.DataBind();
                }
            }
        }

        void LoadHighlights(string maTour)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaHighlight, MaTour, TieuDe, MoTa
                                    FROM TourHighlights
                                    WHERE MaTour = @id
                                    ORDER BY MaHighlight";
                cmd.Parameters.AddWithValue("@id", maTour);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptHighlights.DataSource = dt;
                    rptHighlights.DataBind();
                }
            }
        }

        void LoadSchedule(string maTour)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaSchedule, MaTour, ThoiGian, TieuDe, MoTa, ThuTu
                                    FROM TourSchedule
                                    WHERE MaTour = @id
                                    ORDER BY ThuTu";
                cmd.Parameters.AddWithValue("@id", maTour);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptSchedule.DataSource = dt;
                    rptSchedule.DataBind();
                }
            }
        }

        void LoadIncludes(string maTour)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT MaInclude, MaTour, NoiDung
                                    FROM TourIncludes
                                    WHERE MaTour = @id
                                    ORDER BY MaInclude";
                cmd.Parameters.AddWithValue("@id", maTour);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptIncludes.DataSource = dt;
                    rptIncludes.DataBind();
                }
            }
        }

        void LoadTags(string maTour)
        {
            // all tags
            DataTable dtAll = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT MaTag, TenTag FROM TourTag ORDER BY MaTag";
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtAll);
                }
            }

            cblTags.DataSource = dtAll;
            cblTags.DataTextField = "TenTag";
            cblTags.DataValueField = "MaTag";
            cblTags.DataBind();

            if (string.IsNullOrEmpty(maTour)) return;

            // selected tags
            DataTable dtSel = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT MaTag FROM TourTagMapping WHERE MaTour = @MaTour";
                cmd.Parameters.AddWithValue("@MaTour", maTour);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtSel);
                }
            }

            foreach (ListItem item in cblTags.Items)
            {
                foreach (DataRow row in dtSel.Rows)
                {
                    if (item.Value == row["MaTag"].ToString())
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        #endregion

        #region SAVE THÔNG TIN CHUNG

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMaTour.Text.Trim();
            string ten = txtTenTour.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                lblMsg.Text = "Mã tour và Tên tour không được để trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            DateTime? ngayKH = null;
            if (!string.IsNullOrWhiteSpace(txtNgayKhoiHanh.Text))
            {
                if (DateTime.TryParseExact(
                    txtNgayKhoiHanh.Text.Trim(),
                    "dd/MM/yyyy HH:mm",
                    System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                    System.Globalization.DateTimeStyles.None,
                    out DateTime d))
                {
                    ngayKH = d;
                }
            }

            decimal giaNL = 0, giaTE = 0;
            decimal.TryParse(txtGiaNguoiLon.Text.Trim(), out giaNL);
            decimal.TryParse(txtGiaTreEm.Text.Trim(), out giaTE);

            int soCho = 0, soChoDD = 0;
            int.TryParse(txtSoCho.Text.Trim(), out soCho);
            int.TryParse(txtSoChoDaDat.Text.Trim(), out soChoDD);

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                if (IsEditMode)
                {
                    cmd.CommandText = @"
                        UPDATE Tour
                        SET MaDiaDiem  = @MaDiaDiem,
                            TenTour    = @TenTour,
                            MoTaNgan   = @MoTaNgan,
                            MoTaChiTiet= @MoTaChiTiet,
                            GiaNguoiLon= @GiaNL,
                            GiaTreEm   = @GiaTE,
                            ThoiLuong  = @ThoiLuong,
                            PhuongTien = @PhuongTien,
                            NgayKhoiHanh = @NgayKH,
                            SoCho      = @SoCho,
                            SoChoDaDat = @SoChoDaDat,
                            TrangThai  = @TrangThai,
                            HienThi    = @HienThi,
                            UpdatedAt  = GETDATE()
                        WHERE MaTour   = @Ma";
                }
                else
                {
                    cmd.CommandText = @"
                        INSERT INTO Tour
                            (MaTour, MaDiaDiem, TenTour, MoTaNgan, MoTaChiTiet,
                             GiaNguoiLon, GiaTreEm, ThoiLuong, PhuongTien,
                             NgayKhoiHanh, SoCho, SoChoDaDat, TrangThai, HienThi, CreatedAt)
                        VALUES
                            (@Ma, @MaDiaDiem, @TenTour, @MoTaNgan, @MoTaChiTiet,
                             @GiaNL, @GiaTE, @ThoiLuong, @PhuongTien,
                             @NgayKH, @SoCho, @SoChoDaDat, @TrangThai, @HienThi, GETDATE())";
                }

                cmd.Parameters.AddWithValue("@Ma", ma);
                cmd.Parameters.AddWithValue("@MaDiaDiem", (object)txtMaDiaDiem.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TenTour", ten);
                cmd.Parameters.AddWithValue("@MoTaNgan", (object)txtMoTaNgan.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MoTaChiTiet", (object)txtMoTaChiTiet.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GiaNL", giaNL);
                cmd.Parameters.AddWithValue("@GiaTE", giaTE);
                cmd.Parameters.AddWithValue("@ThoiLuong", (object)txtThoiLuong.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhuongTien", (object)txtPhuongTien.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayKH", (object)ngayKH ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoCho", soCho);
                cmd.Parameters.AddWithValue("@SoChoDaDat", soChoDD);
                cmd.Parameters.AddWithValue("@TrangThai", ddlTrangThai.SelectedValue);
                cmd.Parameters.AddWithValue("@HienThi", chkHienThiEdit.Checked);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Lưu tour thành công.";
            lblMsg.CssClass = "msg success";

            if (!IsEditMode)
            {
                Response.Redirect("AdminTourEdit.aspx?id=" + ma);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminTour.aspx");
        }

        #endregion

        #region TOUR IMAGES

        protected void btnAddTourImg_Click(object sender, EventArgs e)
        {
            if (!fuTourImg.HasFile)
            {
                lblMsg.Text = "Vui lòng chọn file ảnh.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maTour = txtMaTour.Text.Trim();
            if (string.IsNullOrEmpty(maTour))
            {
                lblMsg.Text = "Hãy lưu thông tin tour trước khi thêm ảnh.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string fileName = Path.GetFileName(fuTourImg.FileName);
            string folderPhysical = Server.MapPath(TourImageFolder);
            if (!Directory.Exists(folderPhysical)) Directory.CreateDirectory(folderPhysical);
            string fullPath = Path.Combine(folderPhysical, fileName);
            fuTourImg.SaveAs(fullPath);

            string urlAnh = "Images/Tour/" + fileName;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO TourImages (MaTour, UrlAnh)
                    VALUES (@MaTour, @UrlAnh)";
                cmd.Parameters.AddWithValue("@MaTour", maTour);
                cmd.Parameters.AddWithValue("@UrlAnh", urlAnh);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm ảnh tour thành công.";
            lblMsg.CssClass = "msg success";
            LoadImages(maTour);
        }

        protected void rptImages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            string maAnh = e.CommandArgument.ToString(); // MaAnh (INT identity)

            if (e.CommandName == "SaveImg")
            {
                TextBox txtUrl = (TextBox)e.Item.FindControl("txtUrlAnh");
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE TourImages SET UrlAnh = @url WHERE MaAnh = @id";
                    cmd.Parameters.AddWithValue("@url", txtUrl.Text.Trim());
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
                    cmd.CommandText = "DELETE FROM TourImages WHERE MaAnh = @id";
                    cmd.Parameters.AddWithValue("@id", maAnh);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                lblMsg.Text = "Xóa ảnh thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadImages(maTour);
        }

        #endregion

        #region HIGHLIGHTS

        protected void btnAddHL_Click(object sender, EventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            if (string.IsNullOrEmpty(maTour))
            {
                lblMsg.Text = "Hãy lưu tour trước khi thêm điểm nổi bật.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maHL = txtNewMaHighlight.Text.Trim();
            string tieuDe = txtNewTieuDeHL.Text.Trim();
            string moTa = txtNewMoTaHL.Text.Trim();

            if (string.IsNullOrEmpty(maHL) || string.IsNullOrEmpty(tieuDe))
            {
                lblMsg.Text = "Mã và Tiêu đề điểm nổi bật không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO TourHighlights (MaHighlight, MaTour, TieuDe, MoTa)
                    VALUES (@MaHL, @MaTour, @TieuDe, @MoTa)";
                cmd.Parameters.AddWithValue("@MaHL", maHL);
                cmd.Parameters.AddWithValue("@MaTour", maTour);
                cmd.Parameters.AddWithValue("@TieuDe", tieuDe);
                cmd.Parameters.AddWithValue("@MoTa", (object)moTa ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm điểm nổi bật thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMaHighlight.Text = txtNewTieuDeHL.Text = txtNewMoTaHL.Text = "";
            LoadHighlights(maTour);
        }

        protected void rptHighlights_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            string maHL = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtTieuDe = (TextBox)e.Item.FindControl("txtTieuDe");
                TextBox txtMoTa = (TextBox)e.Item.FindControl("txtMoTa");

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE TourHighlights
                        SET TieuDe = @TieuDe, MoTa = @MoTa
                        WHERE MaHighlight = @MaHL AND MaTour = @MaTour";
                    cmd.Parameters.AddWithValue("@TieuDe", txtTieuDe.Text.Trim());
                    cmd.Parameters.AddWithValue("@MoTa", (object)txtMoTa.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaHL", maHL);
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu điểm nổi bật thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM TourHighlights WHERE MaHighlight = @id AND MaTour = @MaTour";
                    cmd.Parameters.AddWithValue("@id", maHL);
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa điểm nổi bật thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadHighlights(maTour);
        }

        #endregion

        #region SCHEDULE

        protected void btnAddSchedule_Click(object sender, EventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            if (string.IsNullOrEmpty(maTour))
            {
                lblMsg.Text = "Hãy lưu tour trước khi thêm lịch trình.";
                lblMsg.CssClass = "msg error";
                return;
            }

            int thuTu = 0;
            int.TryParse(txtNewThuTu.Text.Trim(), out thuTu);

            string thoiGian = txtNewThoiGianLT.Text.Trim();
            string tieuDe = txtNewTieuDeLT.Text.Trim();
            string moTa = txtNewMoTaLT.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO TourSchedule (MaTour, ThoiGian, TieuDe, MoTa, ThuTu)
                    VALUES (@MaTour, @ThoiGian, @TieuDe, @MoTa, @ThuTu)";
                cmd.Parameters.AddWithValue("@MaTour", maTour);
                cmd.Parameters.AddWithValue("@ThoiGian", (object)thoiGian ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TieuDe", (object)tieuDe ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MoTa", (object)moTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ThuTu", thuTu);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm lịch trình thành công.";
            lblMsg.CssClass = "msg success";
            txtNewThuTu.Text = txtNewThoiGianLT.Text = txtNewTieuDeLT.Text = txtNewMoTaLT.Text = "";
            LoadSchedule(maTour);
        }

        protected void rptSchedule_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            string maSchedule = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtThuTu = (TextBox)e.Item.FindControl("txtThuTu");
                TextBox txtThoiGian = (TextBox)e.Item.FindControl("txtThoiGianLT");
                TextBox txtTieuDe = (TextBox)e.Item.FindControl("txtTieuDeLT");
                TextBox txtMoTa = (TextBox)e.Item.FindControl("txtMoTaLT");

                int thuTu = 0;
                int.TryParse(txtThuTu.Text.Trim(), out thuTu);

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE TourSchedule
                        SET ThuTu = @ThuTu,
                            ThoiGian = @ThoiGian,
                            TieuDe = @TieuDe,
                            MoTa = @MoTa
                        WHERE MaSchedule = @MaSchedule AND MaTour = @MaTour";
                    cmd.Parameters.AddWithValue("@ThuTu", thuTu);
                    cmd.Parameters.AddWithValue("@ThoiGian", (object)txtThoiGian.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TieuDe", (object)txtTieuDe.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MoTa", (object)txtMoTa.Text.Trim() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaSchedule", maSchedule);
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu lịch trình thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM TourSchedule WHERE MaSchedule = @id AND MaTour = @MaTour";
                    cmd.Parameters.AddWithValue("@id", maSchedule);
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa lịch trình thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadSchedule(maTour);
        }

        #endregion

        #region INCLUDES

        protected void btnAddInclude_Click(object sender, EventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            if (string.IsNullOrEmpty(maTour))
            {
                lblMsg.Text = "Hãy lưu tour trước khi thêm dịch vụ bao gồm.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string maInclude = txtNewMaInclude.Text.Trim();
            string noiDung = txtNewNoiDungInc.Text.Trim();

            if (string.IsNullOrEmpty(maInclude) || string.IsNullOrEmpty(noiDung))
            {
                lblMsg.Text = "Mã và Nội dung không được trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO TourIncludes (MaInclude, MaTour, NoiDung)
                    VALUES (@MaInc, @MaTour, @NoiDung)";
                cmd.Parameters.AddWithValue("@MaInc", maInclude);
                cmd.Parameters.AddWithValue("@MaTour", maTour);
                cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Thêm dịch vụ bao gồm thành công.";
            lblMsg.CssClass = "msg success";
            txtNewMaInclude.Text = txtNewNoiDungInc.Text = "";
            LoadIncludes(maTour);
        }

        protected void rptIncludes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            string maInclude = e.CommandArgument.ToString();

            if (e.CommandName == "Save")
            {
                TextBox txtNoiDung = (TextBox)e.Item.FindControl("txtNoiDungInc");

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE TourIncludes
                        SET NoiDung = @NoiDung
                        WHERE MaInclude = @MaInc AND MaTour = @MaTour";
                    cmd.Parameters.AddWithValue("@NoiDung", txtNoiDung.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaInc", maInclude);
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Lưu dịch vụ bao gồm thành công.";
                lblMsg.CssClass = "msg success";
            }
            else if (e.CommandName == "Del")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM TourIncludes WHERE MaInclude = @id AND MaTour = @MaTour";
                    cmd.Parameters.AddWithValue("@id", maInclude);
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMsg.Text = "Xóa dịch vụ bao gồm thành công.";
                lblMsg.CssClass = "msg success";
            }

            LoadIncludes(maTour);
        }

        #endregion

        #region TAGS

        protected void btnSaveTags_Click(object sender, EventArgs e)
        {
            string maTour = txtMaTour.Text.Trim();
            if (string.IsNullOrEmpty(maTour))
            {
                lblMsg.Text = "Hãy lưu tour trước khi lưu tags.";
                lblMsg.CssClass = "msg error";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                cmd.Transaction = tran;

                try
                {
                    cmd.CommandText = "DELETE FROM TourTagMapping WHERE MaTour = @MaTour";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@MaTour", maTour);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO TourTagMapping (MaTour, MaTag) VALUES (@MaTour, @MaTag)";
                    foreach (ListItem item in cblTags.Items)
                    {
                        if (!item.Selected) continue;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@MaTour", maTour);
                        cmd.Parameters.AddWithValue("@MaTag", int.Parse(item.Value));
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                    lblMsg.Text = "Lưu tags cho tour thành công.";
                    lblMsg.CssClass = "msg success";
                }
                catch
                {
                    tran.Rollback();
                    lblMsg.Text = "Có lỗi khi lưu tags.";
                    lblMsg.CssClass = "msg error";
                }
            }
        }

        #endregion
    }
}
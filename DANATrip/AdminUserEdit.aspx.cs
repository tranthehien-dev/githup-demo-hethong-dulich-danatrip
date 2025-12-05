using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DANATrip
{
    public partial class AdminUserEdit : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        bool IsEditMode => !string.IsNullOrEmpty(Request.QueryString["id"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: check quyền admin

            if (!IsPostBack)
            {
                if (IsEditMode)
                {
                    litTitle.Text = "Chỉnh sửa Người dùng";
                    txtMaNguoiDung.Enabled = false;
                    LoadUser(Request.QueryString["id"]);
                }
                else
                {
                    litTitle.Text = "Thêm Người dùng mới";
                    chkHienThi.Checked = true;
                    ddlTrangThai.SelectedValue = "Hoạt động";
                }
            }
        }

        void LoadUser(string id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT MaNguoiDung, HoTen, Email, MatKhau, VaiTro, NgayTao, SDT,
                           ISNULL(TrangThai, N'Hoạt động') AS TrangThai,
                           ISNULL(HienThi,1) AS HienThi
                    FROM NguoiDung
                    WHERE MaNguoiDung = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaNguoiDung.Text = rd["MaNguoiDung"].ToString();
                        txtHoTen.Text = rd["HoTen"].ToString();
                        txtEmail.Text = rd["Email"].ToString();
                        txtSDT.Text = rd["SDT"].ToString();
                        ddlVaiTro.SelectedValue = rd["VaiTro"].ToString();
                        ddlTrangThai.SelectedValue = rd["TrangThai"].ToString();
                        chkHienThi.Checked = Convert.ToBoolean(rd["HienThi"]);

                        if (rd["NgayTao"] != DBNull.Value)
                        {
                            lblNgayTao.Text = Convert.ToDateTime(rd["NgayTao"]).ToString("dd/MM/yyyy HH:mm");
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMaNguoiDung.Text.Trim();
            string ten = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(email))
            {
                lblMsg.Text = "Mã, Họ tên và Email không được để trống.";
                lblMsg.CssClass = "msg error";
                return;
            }

            string sdt = txtSDT.Text.Trim();
            string vaiTro = ddlVaiTro.SelectedValue;
            string trangThai = ddlTrangThai.SelectedValue;
            bool hienThi = chkHienThi.Checked;

            string newPassword = txtMatKhau.Text.Trim();
            string hashMatKhau = null;

            if (!string.IsNullOrEmpty(newPassword))
            {
                hashMatKhau = HashPassword(newPassword);
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                if (IsEditMode)
                {
                    // cập nhật
                    if (string.IsNullOrEmpty(hashMatKhau))
                    {
                        cmd.CommandText = @"
                            UPDATE NguoiDung
                            SET HoTen = @HoTen,
                                Email = @Email,
                                SDT = @SDT,
                                VaiTro = @VaiTro,
                                TrangThai = @TrangThai,
                                HienThi = @HienThi
                            WHERE MaNguoiDung = @Ma";
                    }
                    else
                    {
                        cmd.CommandText = @"
                            UPDATE NguoiDung
                            SET HoTen = @HoTen,
                                Email = @Email,
                                SDT = @SDT,
                                VaiTro = @VaiTro,
                                MatKhau = @MatKhau,
                                TrangThai = @TrangThai,
                                HienThi = @HienThi
                            WHERE MaNguoiDung = @Ma";
                        cmd.Parameters.AddWithValue("@MatKhau", hashMatKhau);
                    }
                }
                else
                {
                    // thêm mới
                    if (string.IsNullOrEmpty(hashMatKhau))
                    {
                        lblMsg.Text = "Vui lòng nhập mật khẩu cho tài khoản mới.";
                        lblMsg.CssClass = "msg error";
                        return;
                    }

                    cmd.CommandText = @"
                        INSERT INTO NguoiDung (MaNguoiDung, HoTen, Email, MatKhau, VaiTro, NgayTao, SDT, TrangThai, HienThi)
                        VALUES (@Ma, @HoTen, @Email, @MatKhau, @VaiTro, GETDATE(), @SDT, @TrangThai, @HienThi)";
                    cmd.Parameters.AddWithValue("@MatKhau", hashMatKhau);
                }

                cmd.Parameters.AddWithValue("@Ma", ma);
                cmd.Parameters.AddWithValue("@HoTen", ten);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@SDT", (object)sdt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VaiTro", vaiTro);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@HienThi", hienThi);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Lưu người dùng thành công.";
            lblMsg.CssClass = "msg success";

            if (!IsEditMode)
            {
                Response.Redirect("AdminUserEdit.aspx?id=" + ma);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminUser.aspx");
        }

        // Hash SHA256 đơn giản – giống cách bạn đang lưu (chuỗi hex)
        string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
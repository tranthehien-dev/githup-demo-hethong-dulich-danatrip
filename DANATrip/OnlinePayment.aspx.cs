using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class OnlinePayment : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        protected int MaBooking
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["bookingId"], out id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (MaBooking <= 0)
                {
                    lblError.Text = "Không tìm thấy thông tin đặt vé.";
                    btnConfirm.Enabled = false;
                    return;
                }

                LoadBookingInfo();
            }
        }

        private void LoadBookingInfo()
        {
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT b.TenKhach, b.SoNguoiLon, b.SoTreEm, b.TongTien, b.MaTour, t.TenTour,
                           (SELECT TOP 1 UrlAnh FROM TourImages WHERE MaTour = t.MaTour) AS UrlAnh
                    FROM Booking b
                    INNER JOIN Tour t ON b.MaTour = t.MaTour
                    WHERE b.MaBooking = @id";
                cmd.Parameters.AddWithValue("@id", MaBooking);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblTenKhach.Text = dr["TenKhach"].ToString();
                        int nl = Convert.ToInt32(dr["SoNguoiLon"]);
                        int te = Convert.ToInt32(dr["SoTreEm"]);
                        lblSL.Text = $"{nl} người lớn, {te} trẻ em";

                        decimal total = Convert.ToDecimal(dr["TongTien"]);
                        lblTongTien.Text = total.ToString("N0") + " ₫";

                        lblTenTour.Text = dr["TenTour"].ToString();
                        string urlAnh = dr["UrlAnh"] as string;
                        if (!string.IsNullOrEmpty(urlAnh))
                            imgTour.ImageUrl = urlAnh;

                        // Nội dung chuyển khoản: BOOKING + mã
                        lblNoiDungCK.Text = "BOOKING " + MaBooking;
                    }
                    else
                    {
                        lblError.Text = "Không tìm thấy thông tin đặt vé.";
                        btnConfirm.Enabled = false;
                    }
                }
            }
        }

        protected void ddlMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ẩn/hiện panel theo phương thức
            pnlQR.Visible = (ddlMethod.SelectedValue == "QR");
            pnlDomestic.Visible = (ddlMethod.SelectedValue == "DOM");
            pnlInternational.Visible = (ddlMethod.SelectedValue == "INT");
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (string.IsNullOrEmpty(ddlMethod.SelectedValue))
            {
                lblError.Text = "Vui lòng chọn phương thức thanh toán.";
                return;
            }

            // Validate đơn giản cho từng loại (vì chỉ là giả lập)
            if (ddlMethod.SelectedValue == "DOM")
            {
                if (string.IsNullOrWhiteSpace(txtBankName.Text) ||
                    string.IsNullOrWhiteSpace(txtAccountName.Text) ||
                    string.IsNullOrWhiteSpace(txtAccountNumber.Text))
                {
                    lblError.Text = "Vui lòng nhập đầy đủ thông tin thẻ nội địa.";
                    return;
                }
            }
            else if (ddlMethod.SelectedValue == "INT")
            {
                if (string.IsNullOrWhiteSpace(txtCardName.Text) ||
                    string.IsNullOrWhiteSpace(txtCardNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtExpire.Text) ||
                    string.IsNullOrWhiteSpace(txtCVV.Text))
                {
                    lblError.Text = "Vui lòng nhập đầy đủ thông tin thẻ quốc tế.";
                    return;
                }
            }
            // QR: giả lập, không cần validate thêm

            // Giả lập: cập nhật trạng thái thanh toán
            if (MaBooking > 0)
            {
                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Booking
                        SET TrangThai = N'Đã Thanh Toán'
                        WHERE MaBooking = @id";
                    cmd.Parameters.AddWithValue("@id", MaBooking);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Lưu mã booking vào Session (nếu BookingSuccess dùng)
            Session["MaBooking"] = MaBooking;
            Response.Redirect("BookingSuccess.aspx");
        }
    }
}
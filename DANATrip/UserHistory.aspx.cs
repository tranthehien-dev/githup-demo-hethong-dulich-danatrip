using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class UserHistory : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        // số booking mỗi trang
        int PageSize = 5;

        int CurrentPage
        {
            get
            {
                object o = ViewState["CurrentPage"];
                return (o == null) ? 0 : (int)o;
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("~/DangNhap.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CurrentPage = 0;
                LoadBookings();
            }
        }

        void LoadBookings()
        {
            string userId = Session["MaNguoiDung"].ToString();
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(connStr))
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT b.MaBooking, b.MaTour, t.TenTour, 
                           b.SoNguoiLon, b.SoTreEm, 
                           b.TongTien, b.NgayDat, b.TrangThai
                    FROM Booking b
                    LEFT JOIN Tour t ON b.MaTour = t.MaTour
                    WHERE b.MaNguoiDung = @id
                    ORDER BY b.NgayDat DESC, b.MaBooking DESC";
                cmd.Parameters.AddWithValue("@id", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                rptBookings.Visible = false;
                lblNoBooking.Text = "Bạn chưa đặt tour nào.";
                lnkPrev.Visible = lnkNext.Visible = false;
                lblPageInfo.Text = "";
                return;
            }

            // cấu hình phân trang cho Repeater
            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = dt.DefaultView;
            pds.AllowPaging = true;
            pds.PageSize = PageSize;

            // đảm bảo CurrentPage nằm trong phạm vi
            if (CurrentPage < 0) CurrentPage = 0;
            if (CurrentPage >= pds.PageCount) CurrentPage = pds.PageCount - 1;

            pds.CurrentPageIndex = CurrentPage;

            rptBookings.Visible = true;
            rptBookings.DataSource = pds;
            rptBookings.DataBind();
            lblNoBooking.Text = "";

            // cập nhật nút phân trang
            lnkPrev.Enabled = !pds.IsFirstPage;
            lnkNext.Enabled = !pds.IsLastPage;

            lnkPrev.Visible = lnkNext.Visible = (pds.PageCount > 1);
            lblPageInfo.Text = $"Trang {CurrentPage + 1}/{pds.PageCount}";
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            CurrentPage--;
            LoadBookings();
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            LoadBookings();
        }

        protected void rptBookings_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                string maBooking = e.CommandArgument.ToString();
                string userId = Session["MaNguoiDung"].ToString();

                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT MaBooking, MaTour, SoNguoiLon, SoTreEm, TongTien, TrangThai, NgayDat
                                        FROM Booking
                                        WHERE MaBooking = @mb AND MaNguoiDung = @uid";
                    cmd.Parameters.AddWithValue("@mb", maBooking);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cn.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                        {
                            lblMsg.CssClass = "msg error";
                            lblMsg.Text = "Không tìm thấy đặt tour hoặc bạn không có quyền thực hiện.";
                            return;
                        }

                        string trangThai = rd["TrangThai"] == DBNull.Value ? "" : rd["TrangThai"].ToString();
                        DateTime? ngayDat = rd["NgayDat"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rd["NgayDat"]);
                        int soNL = rd["SoNguoiLon"] == DBNull.Value ? 0 : Convert.ToInt32(rd["SoNguoiLon"]);
                        int soTE = rd["SoTreEm"] == DBNull.Value ? 0 : Convert.ToInt32(rd["SoTreEm"]);
                        string maTour = rd["MaTour"]?.ToString();

                        if (string.Equals(trangThai, "Đã hủy", StringComparison.OrdinalIgnoreCase))
                        {
                            lblMsg.CssClass = "msg error";
                            lblMsg.Text = "Đặt tour đã bị hủy trước đó.";
                            return;
                        }
                        if (ngayDat.HasValue && ngayDat.Value.Date < DateTime.Today)
                        {
                            lblMsg.CssClass = "msg error";
                            lblMsg.Text = "Không thể hủy đặt tour đã qua ngày khởi hành.";
                            return;
                        }

                        rd.Close();

                        using (SqlTransaction tx = cn.BeginTransaction())
                        {
                            try
                            {
                                using (SqlCommand cmd2 = cn.CreateCommand())
                                {
                                    cmd2.Transaction = tx;
                                    cmd2.CommandText = @"UPDATE Booking SET TrangThai = @st WHERE MaBooking = @mb";
                                    cmd2.Parameters.AddWithValue("@st", "Đã hủy");
                                    cmd2.Parameters.AddWithValue("@mb", maBooking);
                                    cmd2.ExecuteNonQuery();
                                }

                                try
                                {
                                    using (SqlCommand cmd3 = cn.CreateCommand())
                                    {
                                        cmd3.Transaction = tx;
                                        cmd3.CommandText = @"
                                            IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'SoChoDaDat' AND Object_ID = Object_ID(N'Tour'))
                                            BEGIN
                                                UPDATE Tour
                                                SET SoChoDaDat = CASE WHEN ISNULL(SoChoDaDat,0) - @num <= 0 THEN 0 ELSE SoChoDaDat - @num END
                                                WHERE MaTour = @maTour
                                            END";
                                        cmd3.Parameters.AddWithValue("@num", (soNL + soTE));
                                        cmd3.Parameters.AddWithValue("@maTour", maTour ?? (object)DBNull.Value);
                                        cmd3.ExecuteNonQuery();
                                    }
                                }
                                catch { }

                                tx.Commit();

                                lblMsg.CssClass = "msg success";
                                lblMsg.Text = "Hủy đặt tour thành công.";
                            }
                            catch (Exception ex)
                            {
                                tx.Rollback();
                                lblMsg.CssClass = "msg error";
                                lblMsg.Text = "Xảy ra lỗi khi hủy: " + ex.Message;
                            }
                        }
                    }
                }

                // Sau khi hủy, load lại dữ liệu để cập nhật danh sách
                LoadBookings();
            }
            else if (e.CommandName == "Review")
            {
                string maTour = e.CommandArgument.ToString();

                if (Session["MaNguoiDung"] == null)
                {
                    Session["ReturnUrl"] = "UserHistory.aspx";
                    Response.Redirect("SignIn.aspx");
                    return;
                }

                Response.Redirect("DanhGiaDetail.aspx?MaTour=" + maTour);
            }
        }
    }
}
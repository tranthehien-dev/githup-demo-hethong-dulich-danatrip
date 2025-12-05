using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminBooking : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        const int PageSize = 5;

        int CurrentPageIndex
        {
            get => (int)(ViewState["PageIndex"] ?? 0);
            set => ViewState["PageIndex"] = value;
        }
        // Lưu filter trạng thái hiện tại
        string CurrentStatusFilter
        {
            get => (string)(ViewState["StatusFilter"] ?? "ALL");
            set => ViewState["StatusFilter"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentStatusFilter = "ALL";
                CurrentPageIndex = 0;
                LoadBookings();
            }
        }

        void LoadBookings()
        {
            DataTable dt = new DataTable();
            string keyword = txtSearch.Text.Trim();
            string statusFilter = CurrentStatusFilter;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            SELECT  b.MaBooking,
                    b.TenKhach,
                    b.Email,
                    b.SDT,
                    b.TongTien,
                    b.TrangThai,
                    b.NgayDat,
                    t.TenTour
            FROM Booking b
            INNER JOIN Tour t ON b.MaTour = t.MaTour
            WHERE 1 = 1";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    cmd.CommandText += @"
                AND (CAST(b.MaBooking AS NVARCHAR(50)) LIKE @kw
                     OR b.TenKhach LIKE @kw
                     OR b.Email LIKE @kw
                     OR b.SDT LIKE @kw)";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                if (statusFilter == "PENDING")
                {
                    cmd.CommandText += " AND b.TrangThai LIKE N'%Chờ%'";
                }
                else if (statusFilter == "PAID")
                {
                    cmd.CommandText += " AND b.TrangThai LIKE N'%Đã Thanh Toán%'";
                }
                else if (statusFilter == "CANCELLED")
                {
                    cmd.CommandText += " AND b.TrangThai LIKE N'%hủy%'";
                }

                cmd.CommandText += " ORDER BY b.NgayDat DESC, b.MaBooking DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            // Thiết lập PagedDataSource
            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = dt.DefaultView;
            pds.AllowPaging = true;
            pds.PageSize = PageSize;

            // Giới hạn CurrentPageIndex
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            if (CurrentPageIndex > pds.PageCount - 1) CurrentPageIndex = pds.PageCount - 1;
            pds.CurrentPageIndex = CurrentPageIndex;

            rptBookings.DataSource = pds;
            rptBookings.DataBind();

            // Tạo data cho pager
            DataTable dtPager = new DataTable();
            dtPager.Columns.Add("PageIndex", typeof(int));
            dtPager.Columns.Add("PageText", typeof(string));
            dtPager.Columns.Add("IsCurrent", typeof(bool));

            for (int i = 0; i < pds.PageCount; i++)
            {
                var row = dtPager.NewRow();
                row["PageIndex"] = i;
                row["PageText"] = (i + 1).ToString();
                row["IsCurrent"] = (i == CurrentPageIndex);
                dtPager.Rows.Add(row);
            }

            dlPager.DataSource = dtPager;
            dlPager.DataBind();

            lblSummary.Text = $"Hiển thị {dt.Rows.Count} giao dịch, trang {CurrentPageIndex + 1}/{(pds.PageCount == 0 ? 1 : pds.PageCount)}.";
        }

        // CSS cho badge trạng thái
        protected string GetStatusCss(string trangThai)
        {
            if (string.IsNullOrEmpty(trangThai)) return string.Empty;
            trangThai = trangThai.ToLower();

            if (trangThai.Contains("đã thanh toán"))
                return "ab-status-success";
            if (trangThai.Contains("chờ"))
                return "ab-status-pending";
            if (trangThai.Contains("hủy"))
                return "ab-status-cancel";

            return "ab-status-default";
        }

        protected bool CanShowMarkPaid(string trangThai)
        {
            if (string.IsNullOrEmpty(trangThai)) return false;
            trangThai = trangThai.ToLower();
            // chỉ cho xác nhận nếu đang chờ thanh toán
            return trangThai.Contains("chờ");
        }

        protected bool CanShowCancel(string trangThai)
        {
            if (string.IsNullOrEmpty(trangThai)) return false;
            trangThai = trangThai.ToLower();
            // cho phép hủy nếu chưa hủy và chưa thanh toán
            return !trangThai.Contains("hủy") && !trangThai.Contains("đã thanh toán");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CurrentPageIndex = 0;
            LoadBookings();
        }

        protected void btnFilterAll_Click(object sender, EventArgs e)
        {
            CurrentStatusFilter = "ALL";
            CurrentPageIndex = 0;
            LoadBookings();
        }

        // Các filter khác tương tự
        protected void btnFilterPending_Click(object sender, EventArgs e)
        {
            CurrentStatusFilter = "PENDING";
            CurrentPageIndex = 0;
            LoadBookings();
        }

        protected void btnFilterPaid_Click(object sender, EventArgs e)
        {
            CurrentStatusFilter = "PAID";
            CurrentPageIndex = 0;
            LoadBookings();
        }

        protected void btnFilterCancelled_Click(object sender, EventArgs e)
        {
            CurrentStatusFilter = "CANCELLED";
            CurrentPageIndex = 0;
            LoadBookings();
        }

        protected void rptBookings_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int maBooking = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "MarkPaid")
            {
                UpdateBookingStatus(maBooking, "Đã Thanh Toán");
                lblMsg.Text = $"Đã xác nhận thanh toán cho đơn #{maBooking}.";
                lblMsg.CssClass = "ab-msg success";
            }
            else if (e.CommandName == "Cancel")
            {
                UpdateBookingStatus(maBooking, "Đã hủy");
                lblMsg.Text = $"Đã hủy đơn #{maBooking}.";
                lblMsg.CssClass = "ab-msg danger";
            }

            LoadBookings();
        }

        void UpdateBookingStatus(int maBooking, string newStatus)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE Booking
                    SET TrangThai = @status
                    WHERE MaBooking = @id";
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@id", maBooking);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        protected void dlPager_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                int newIndex = int.Parse(e.CommandArgument.ToString());
                CurrentPageIndex = newIndex;
                LoadBookings();
            }
        }
    }
}
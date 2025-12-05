using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DANATrip
{
    public partial class AdminContact : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        string CurrentStatusFilter
        {
            get => (string)(ViewState["StatusFilter"] ?? "ALL");
            set => ViewState["StatusFilter"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: kiểm tra quyền admin
            if (!IsPostBack)
            {
                CurrentStatusFilter = "ALL";
                LoadContacts();
            }
        }

        void LoadContacts()
        {
            DataTable dt = new DataTable();
            string keyword = txtSearch.Text.Trim();
            string statusFilter = CurrentStatusFilter;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT MaLienHe, MaNguoiDung, Ten, Email, NoiDung, NgayGui,
                           ISNULL(TrangThai, N'Chưa xử lý') AS TrangThai
                    FROM LienHe
                    WHERE 1 = 1";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    cmd.CommandText += @"
                        AND (Ten LIKE @kw
                             OR Email LIKE @kw
                             OR NoiDung LIKE @kw
                             OR MaLienHe LIKE @kw)";
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                }

                if (statusFilter == "PENDING")
                {
                    cmd.CommandText += " AND ISNULL(TrangThai, N'Chưa xử lý') = N'Chưa xử lý'";
                }
                else if (statusFilter == "DONE")
                {
                    cmd.CommandText += " AND TrangThai = N'Đã xử lý'";
                }

                cmd.CommandText += " ORDER BY NgayGui DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptContacts.DataSource = dt;
            rptContacts.DataBind();

            lblSummary.Text = $"Có {dt.Rows.Count} liên hệ.";
        }

        protected string GetStatusCss(object statusObj)
        {
            string s = (statusObj ?? "").ToString();
            if (string.IsNullOrEmpty(s) || s == "Chưa xử lý")
                return "ac-status-pending";
            if (s == "Đã xử lý")
                return "ac-status-done";
            return "ac-status-default";
        }

        protected bool CanShowMarkDone(object statusObj)
        {
            string s = (statusObj ?? "").ToString();
            return string.IsNullOrEmpty(s) || s == "Chưa xử lý";
        }

        protected bool CanShowMarkPending(object statusObj)
        {
            string s = (statusObj ?? "").ToString();
            return s == "Đã xử lý";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadContacts();
        }

        protected void btnFilterAll_Click(object sender, EventArgs e)
        {
            CurrentStatusFilter = "ALL";
            LoadContacts();
        }

        protected void btnFilterPending_Click(object sender, EventArgs e)
        {
            CurrentStatusFilter = "PENDING";
            LoadContacts();
        }

        protected void btnFilterDone_Click(object sender, EventArgs e)
        {
            CurrentStatusFilter = "DONE";
            LoadContacts();
        }

        protected void rptContacts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string maLienHe = e.CommandArgument.ToString();
            if (e.CommandName == "MarkDone")
            {
                UpdateStatus(maLienHe, "Đã xử lý");
                lblMsg.Text = "Đã đánh dấu liên hệ là Đã xử lý.";
                lblMsg.CssClass = "ac-msg success";
            }
            else if (e.CommandName == "MarkPending")
            {
                UpdateStatus(maLienHe, "Chưa xử lý");
                lblMsg.Text = "Đã đánh dấu liên hệ là Chưa xử lý.";
                lblMsg.CssClass = "ac-msg info";
            }

            LoadContacts();
        }

        void UpdateStatus(string maLienHe, string status)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE LienHe SET TrangThai = @st WHERE MaLienHe = @id";
                cmd.Parameters.AddWithValue("@st", status);
                cmd.Parameters.AddWithValue("@id", maLienHe);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
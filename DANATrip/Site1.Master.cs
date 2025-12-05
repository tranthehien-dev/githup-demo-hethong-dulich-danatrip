using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;

namespace DANATrip
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // default
                pnlUserSummary.Visible = false;
                pnlUserSidebar.Visible = false;
                pnlAdminSidebar.Visible = false;
                pnlGuest.Visible = false;
                lnkLogoutTop.Visible = false;

                var userId = Session["MaNguoiDung"] as string;
                var hoTen = Session["HoTen"] as string;
                var vaiTro = (Session["VaiTro"] ?? "").ToString();

                if (!string.IsNullOrEmpty(userId))
                {
                    pnlUserSummary.Visible = true;
                    lnkLogoutTop.Visible = true;
                    lblSidebarName.Text = string.IsNullOrEmpty(hoTen) ? "Người dùng" : hoTen;

                    if (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        pnlAdminSidebar.Visible = true;
                        pnlUserSidebar.Visible = false;
                    }
                    else
                    {
                        pnlUserSidebar.Visible = true;
                        pnlAdminSidebar.Visible = false;
                    }

                    LoadUserSummary(userId);

                    // Set active menu item based on current path
                    SetActiveSidebarItem(Request.Path ?? "");
                }
                else
                {
                    pnlGuest.Visible = true;
                }
            }
        }

        void SetActiveSidebarItem(string path)
        {
            // reset all
            liProfile.Attributes["class"] = "";
            liHistory.Attributes["class"] = "";
            liSettings.Attributes["class"] = "";
            liAdminDashboard.Attributes["class"] = "";
            liAdminPlaces.Attributes["class"] = "";
            liAdminFoods.Attributes["class"] = "";
            liAdminTours.Attributes["class"] = "";
            liAdminBooking.Attributes["class"] = "";
            liAdminUsers.Attributes["class"] = "";
            liAdminContact.Attributes["class"] = "";
            liAdminDanhGia.Attributes["class"] = "";

            string p = path.ToLower();

            // USER
            if (p.Contains("userprofile.aspx"))
                liProfile.Attributes["class"] = "active";
            else if (p.Contains("userhistory.aspx"))
                liHistory.Attributes["class"] = "active";
            else if (p.Contains("usersettings.aspx"))
                liSettings.Attributes["class"] = "active";

            // ADMIN
            else if (p.Contains("admindashboard.aspx"))
                liAdminDashboard.Attributes["class"] = "active";
            else if (p.Contains("adminplace.aspx"))
                liAdminPlaces.Attributes["class"] = "active";
            else if (p.Contains("adminfood.aspx"))
                liAdminFoods.Attributes["class"] = "active";
            else if (p.Contains("admintour.aspx"))
                liAdminTours.Attributes["class"] = "active";
            else if (p.Contains("adminbooking.aspx"))
                liAdminBooking.Attributes["class"] = "active";
            else if (p.Contains("adminuser.aspx"))
                liAdminUsers.Attributes["class"] = "active";
            else if (p.Contains("admincontact.aspx"))
                liAdminContact.Attributes["class"] = "active";
            else if (p.Contains("admindanhgia.aspx"))
                liAdminDanhGia.Attributes["class"] = "active";
        }

        void LoadUserSummary(string userId)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    // Lấy HoTen, Email, SDT
                    cmd.CommandText = "SELECT HoTen, Email, SDT FROM NguoiDung WHERE MaNguoiDung = @id";
                    cmd.Parameters.AddWithValue("@id", userId);
                    cn.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            var name = rd["HoTen"]?.ToString();
                            var email = rd["Email"]?.ToString();

                            if (!string.IsNullOrEmpty(name)) lblSidebarName.Text = name;
                            if (!string.IsNullOrEmpty(email)) lblSidebarEmail.Text = email;
                        }
                    }
                }
            }
            catch
            {
                // ignore DB errors gracefully
            }
        }

        protected void lnkLogoutTop_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Home.aspx");
        }
    }
}
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DANATrip
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Default state: guest visible, logged hidden
                userMenu.Visible = true;
                userLogged.Visible = false;

                pnlUserLinks.Visible = false;
                pnlAdminLinks.Visible = false;

                // If user is logged in (session HoTen set during login)
                if (Session["HoTen"] != null)
                {
                    userMenu.Visible = false;
                    userLogged.Visible = true;
                    lblUserName.InnerText = Session["HoTen"].ToString();

                    // Determine role. Expect "Admin" or "User" stored in Session["VaiTro"]
                    var roleObj = Session["VaiTro"];
                    string role = roleObj != null ? roleObj.ToString() : "User";

                    if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        pnlAdminLinks.Visible = true;
                        pnlUserLinks.Visible = false;
                    }
                    else
                    {
                        pnlUserLinks.Visible = true;
                        pnlAdminLinks.Visible = false;
                    }
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Home.aspx");
        }
    }
}
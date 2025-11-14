using System;

namespace hosonguoidung
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Dữ liệu giả lập – sau bạn thay bằng database
                lblHoTen.Text = "Nguyễn Văn A";
                lblNam.Text = "2023";

                txtHoTen.Text = "Nguyễn Văn A";
                txtSDT.Text = "0905123456";
                txtEmail.Text = "nva@gmail.com";
                txtDiaChi.Text = "Đà Nẵng";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ textbox
            string ten = txtHoTen.Text;
            string sdt = txtSDT.Text;
            string email = txtEmail.Text;
            string diachi = txtDiaChi.Text;

            // TODO: Lưu vào database

            Response.Write("<script>alert('Cập nhật thành công!');</script>");
        }
    }
}

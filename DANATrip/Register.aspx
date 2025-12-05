<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="DANATrip.DangKy" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>Đăng ký</title>
    <link href="Styles/styles.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">

        <div class="container">

            <!-- LEFT -->
            <div class="left">
                <div class="left-content">
                    <h1>Khám phá Đà Nẵng cùng<br />chúng tôi</h1>
                    <p>
                        Đăng ký để nhận những gợi ý du lịch độc quyền và lưu lại<br />
                        những địa điểm yêu thích của bạn.
                    </p>
                </div>
            </div>

            <!-- RIGHT -->
            <div class="right">
                <div class="form-box">
                    <h2>Tạo tài khoản</h2>

                    <label>Tên người dùng</label>
                    <asp:TextBox ID="txtUser" CssClass="input" runat="server" placeholder="Nhập tên người dùng của bạn"></asp:TextBox>

                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" CssClass="input" runat="server" placeholder="example@email.com"></asp:TextBox>

                    <!-- MỚI: Số điện thoại -->
                    <label>Số điện thoại</label>
                    <asp:TextBox ID="txtSDT" CssClass="input" runat="server" placeholder="VD: 0905123456"></asp:TextBox>

                    <label>Mật khẩu</label>
                    <div class="password-box">
                        <asp:TextBox ID="txtPass" CssClass="input" runat="server" TextMode="Password" placeholder="Nhập mật khẩu của bạn"></asp:TextBox>
                    </div>

                    <label>Xác nhận mật khẩu</label>
                    <div class="password-box">
                        <asp:TextBox ID="txtConfirm" CssClass="input" runat="server" TextMode="Password" placeholder="Nhập lại mật khẩu của bạn"></asp:TextBox>
                    </div>

                    <asp:Button ID="btnRegister" runat="server" Text="Đăng ký" CssClass="btn-register" OnClick="btnRegister_Click"/>

                    <div class="divider"></div>
                    <p class="or">Hoặc</p>

                    <p class="login-link">Đã có tài khoản? <a href="SignIn.aspx">Đăng nhập ngay</a></p>
                </div>
            </div>

        </div>

    </form>
</body>
</html>
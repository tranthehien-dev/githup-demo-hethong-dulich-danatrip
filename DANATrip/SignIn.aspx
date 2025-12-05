<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="DANATrip.DangNhap" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>DANA Trip - Đăng nhập</title>
    <link rel="stylesheet" href="Styles/styles.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">

            <div class="left-side">
                <div class="left-image"></div>
            </div>

            <div class="right-side">
                <div class="login-box">

                    <h2 class="brand">DANA Trip</h2>
                    <h1 class="title">Chào mừng trở lại!</h1>
                    <p class="subtitle">Cùng khám phá những trải nghiệm tuyệt vời đang chờ bạn.</p>

                    <div class="input-group">
                        <label>Email hoặc Tên đăng nhập</label>
                        <div class="input-field">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox" placeholder="Nhập email của bạn"></asp:TextBox>
                        </div>
                    </div>

                    <div class="input-group">
                        <label>Mật khẩu</label>
                        <div class="input-field">
                            <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" CssClass="textbox" placeholder="Nhập mật khẩu của bạn"></asp:TextBox>
                        </div>
                    </div>

                    <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" CssClass="btn-login" OnClick="btnLogin_Click"  />

                    <div class="divider"></div>
                    <p class="or">Hoặc</p>

                    <p class="register">Chưa có tài khoản? <a href="Register.aspx">Đăng ký</a></p>
                </div>
            </div>

        </div>
    </form>
</body>
</html>

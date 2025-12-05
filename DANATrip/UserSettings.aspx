<%@ Page Title="Cài đặt tài khoản" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UserSettings.aspx.cs" Inherits="DANATrip.UserSettings" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .settings-page.container {
            max-width: 760px;
            margin: 0 auto;
            padding: 10px 20px 40px;
        }
        .settings-header h1 {
            margin: 6px 0;
            font-size: 26px;
            color: #0b2a2b;
        }
        .settings-header .subtitle {
            color: #6f8b8b;
            margin-bottom: 18px;
        }
        .card {
            background: #fff;
            border-radius: 12px;
            padding: 18px 20px;
            box-shadow: 0 8px 22px rgba(6,20,25,0.04);
            margin-bottom: 18px;
        }
        .card h2 {
            margin: 0 0 10px;
            font-size: 18px;
        }
        .form-row {
            margin-top: 10px;
        }
        .form-row label {
            display: block;
            font-size: 13px;
            margin-bottom: 4px;
            color: #274646;
        }
        .input {
            width: 100%;
            padding: 9px 11px;
            border-radius: 8px;
            border: 1px solid #e1ecec;
            font-size: 14px;
            box-sizing: border-box;
        }
        .form-actions {
            margin-top: 14px;
            display: flex;
            gap: 10px;
            justify-content: flex-end;
        }
        .btn {
            padding: 9px 14px;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            font-size: 14px;
        }
        .btn-primary {
            background: #ff8a5b;
            color: #fff;
        }
        .btn-secondary {
            background: #f2f4f4;
            color: #333;
        }
        .btn-danger {
            background: #ff6b6b;
            color: #fff;
        }
        .msg {
            display: block;
            margin-top: 8px;
            font-size: 14px;
        }
        .msg.error { color: #d9534f; }
        .msg.success { color: #28a745; }

        .danger-text {
            color: #797979;
            font-size: 13px;
            margin-top: 6px;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="settings-page container">
        <div class="settings-header">
            <h1>Cài Đặt Tài Khoản</h1>
            <p class="subtitle">Quản lý bảo mật tài khoản của bạn.</p>
        </div>

        <!-- Đổi mật khẩu -->
        <div class="card">
            <h2>Đổi mật khẩu</h2>
            <asp:Label ID="lblChangePassMsg" runat="server" CssClass="msg" />

            <div class="form-row">
                <label>Mật khẩu hiện tại</label>
                <asp:TextBox ID="txtCurrentPass" runat="server" CssClass="input" TextMode="Password" />
            </div>

            <div class="form-row">
                <label>Mật khẩu mới</label>
                <asp:TextBox ID="txtNewPass" runat="server" CssClass="input" TextMode="Password" />
            </div>

            <div class="form-row">
                <label>Nhập lại mật khẩu mới</label>
                <asp:TextBox ID="txtConfirmNewPass" runat="server" CssClass="input" TextMode="Password" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnChangePass" runat="server" Text="Đổi mật khẩu"
                    CssClass="btn btn-primary" OnClick="btnChangePass_Click" />
            </div>
        </div>

        <!-- Xóa tài khoản -->
        <div class="card">
            <h2>Xóa tài khoản</h2>
            <p class="danger-text">
                Hành động này sẽ xóa vĩnh viễn tài khoản của bạn (nếu tài khoản không còn đơn đặt tour nào).
                Nếu tài khoản vẫn còn đơn đặt tour, hệ thống sẽ không cho phép xóa.
            </p>
            <asp:Label ID="lblDeleteMsg" runat="server" CssClass="msg" />

            <div class="form-row">
                <label>Nhập lại mật khẩu để xác nhận</label>
                <asp:TextBox ID="txtDeletePass" runat="server" CssClass="input" TextMode="Password" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnDeleteAccount" runat="server" Text="Xóa tài khoản"
                    CssClass="btn btn-danger"
                    OnClientClick="return confirm('Bạn có chắc chắn muốn xóa tài khoản? Hành động này không thể hoàn tác.');"
                    OnClick="btnDeleteAccount_Click" />
            </div>
        </div>
    </div>
</asp:Content>
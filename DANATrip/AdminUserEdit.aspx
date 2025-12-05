<%@ Page Title="Quản lý Người dùng" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminUserEdit.aspx.cs" Inherits="DANATrip.AdminUserEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-place-edit.css" />

    <div class="admin-page container">
        <h1 class="page-title">
            <asp:Literal ID="litTitle" runat="server" />
        </h1>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <div class="form-grid">
            <div class="form-group">
                <label>Mã người dùng</label>
                <asp:TextBox ID="txtMaNguoiDung" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Họ tên</label>
                <asp:TextBox ID="txtHoTen" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Số điện thoại</label>
                <asp:TextBox ID="txtSDT" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Mật khẩu (hash hoặc plain để hash ở code)</label>
                <asp:TextBox ID="txtMatKhau" runat="server" CssClass="input" TextMode="Password" />
                <small>Nếu để trống khi sửa sẽ giữ nguyên mật khẩu cũ.</small>
            </div>
            <div class="form-group">
                <label>Vai trò</label>
                <asp:DropDownList ID="ddlVaiTro" runat="server" CssClass="input">
                    <asp:ListItem Text="User" Value="User" />
                    <asp:ListItem Text="Admin" Value="Admin" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Trạng thái</label>
                <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="input">
                    <asp:ListItem Text="Hoạt động" Value="Hoạt động" />
                    <asp:ListItem Text="Bị khóa" Value="Bị khóa" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Kích hoạt / Hiển thị</label>
                <asp:CheckBox ID="chkHienThi" runat="server" Checked="true" />
            </div>
            <div class="form-group">
                <label>Ngày tạo</label>
                <asp:Label ID="lblNgayTao" runat="server" CssClass="muted" />
            </div>
        </div>

        <div style="margin-top:18px;">
            <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnBack" runat="server" Text="Quay lại" CssClass="btn" OnClick="btnBack_Click" CausesValidation="false" />
        </div>
    </div>
</asp:Content>
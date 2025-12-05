<%@ Page Title="Quản lý Người dùng" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminUser.aspx.cs" Inherits="DANATrip.AdminUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-place.css" />

    <div class="admin-page container">
        <h1 class="page-title">Quản lý Tài khoản Người dùng</h1>
        <p class="page-subtitle">Tìm kiếm, quản lý vai trò và trạng thái tài khoản.</p>

        <div class="toolbar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Tìm theo tên hoặc email..." />
            <asp:Button ID="btnSearch" runat="server" Text="Lọc" CssClass="btn btn-filter" OnClick="btnSearch_Click" />
            <asp:Button ID="btnAdd" runat="server" Text="Thêm người dùng mới" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand">
            <HeaderTemplate>
                <div class="table">
                    <div class="table-row table-header">
                        <div class="col col-check">Kích hoạt</div>
                        <div class="col col-name">Tên người dùng</div>
                        <div class="col col-short">Email</div>
                        <div class="col col-status">Vai trò</div>
                        <div class="col col-status">Trạng thái</div>
                        <div class="col col-actions">Hành động</div>
                    </div>
            </HeaderTemplate>

            <ItemTemplate>
                <div class="table-row">
                    <div class="col col-check">
                        <asp:CheckBox ID="chkHienThi" runat="server"
                            Checked='<%# Convert.ToBoolean(Eval("HienThi")) %>'
                            AutoPostBack="true"
                            OnCheckedChanged="chkHienThi_CheckedChanged" />
                        <asp:HiddenField ID="hfMaNguoiDung" runat="server" Value='<%# Eval("MaNguoiDung") %>' />
                    </div>
                    <div class="col col-name">
                        <%# Eval("HoTen") %>
                    </div>
                    <div class="col col-short">
                        <%# Eval("Email") %>
                    </div>
                    <div class="col col-status">
                        <%# Eval("VaiTro") %>
                    </div>
                    <div class="col col-status">
                        <%# Eval("TrangThai") %>
                    </div>
                    <div class="col col-actions">
                        <asp:LinkButton ID="lnkEdit" runat="server"
                            CommandName="Edit"
                            CommandArgument='<%# Eval("MaNguoiDung") %>'
                            CssClass="action-link">edit</asp:LinkButton>
                        &nbsp;|&nbsp;
                        <asp:LinkButton ID="lnkDelete" runat="server"
                            CommandName="Delete"
                            CommandArgument='<%# Eval("MaNguoiDung") %>'
                            CssClass="action-link delete"
                            OnClientClick="return confirm('Xóa tài khoản người dùng này?');">
                            delete
                        </asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
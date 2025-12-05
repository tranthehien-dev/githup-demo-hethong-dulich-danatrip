<%@ Page Title="Quản lý Tour" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminTour.aspx.cs" Inherits="DANATrip.AdminTour" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-place.css" />

    <div class="admin-page container">
        <h1 class="page-title">Quản lý Tour du lịch</h1>
        <p class="page-subtitle">Thêm, sửa, ẩn/hiện các tour du lịch tại Đà Nẵng.</p>

        <div class="toolbar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Tìm theo tên tour..." />
            <asp:Button ID="btnSearch" runat="server" Text="Lọc" CssClass="btn btn-filter" OnClick="btnSearch_Click" />
            <asp:Button ID="btnAdd" runat="server" Text="Thêm tour mới" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <asp:Repeater ID="rptTours" runat="server" OnItemCommand="rptTours_ItemCommand">
            <HeaderTemplate>
                <div class="table">
                    <div class="table-row table-header">
                        <div class="col col-check">Hiển thị</div>
                        <div class="col col-name">Tên tour</div>
                        <div class="col col-short">Mô tả ngắn</div>
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
                        <asp:HiddenField ID="hfMaTour" runat="server" Value='<%# Eval("MaTour") %>' />
                    </div>
                    <div class="col col-name">
                        <%# Eval("TenTour") %>
                    </div>
                    <div class="col col-short">
                        <%# Eval("MoTaNgan") %>
                    </div>
                    <div class="col col-status">
                        <%# Eval("TrangThai") %>
                    </div>
                    <div class="col col-actions">
                        <asp:LinkButton ID="lnkEdit" runat="server"
                            CommandName="Edit"
                            CommandArgument='<%# Eval("MaTour") %>'
                            CssClass="action-link">Chỉnh Sửa</asp:LinkButton>
                        &nbsp;|&nbsp;
                        <asp:LinkButton ID="lnkDelete" runat="server"
                            CommandName="Delete"
                            CommandArgument='<%# Eval("MaTour") %>'
                            CssClass="action-link delete"
                            OnClientClick="return confirm('Xóa tour này?');">
                            Xoá
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
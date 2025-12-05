<%@ Page Title="Quản lý Địa điểm" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AdminPlace.aspx.cs" Inherits="DANATrip.AdminPlace" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-place.css" />

    <div class="admin-page container">
        <h1 class="page-title">Quản lý Địa điểm Du lịch</h1>
        <p class="page-subtitle">Thêm, sửa, ẩn/hiện các địa điểm du lịch tại Đà Nẵng.</p>

        <div class="toolbar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Tìm theo tên địa điểm..." />
            <asp:Button ID="btnSearch" runat="server" Text="Lọc" CssClass="btn btn-filter" OnClick="btnSearch_Click" />
            <asp:Button ID="btnAdd" runat="server" Text="Thêm địa điểm mới" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <asp:Repeater ID="rptPlaces" runat="server" OnItemCommand="rptPlaces_ItemCommand">
            <HeaderTemplate>
                <div class="table">
                    <div class="table-row table-header">
                        <div class="col col-check">Hiển thị</div>
                        <div class="col col-name">Tên địa điểm</div>
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
                            OnCheckedChanged="chkHienThi_CheckedChanged"
                            CssClass="chk-hienthi"
                            />
                        <asp:HiddenField ID="hfMaDiaDiem" runat="server" Value='<%# Eval("MaDiaDiem") %>' />
                    </div>
                    <div class="col col-name">
                        <%# Eval("TenDiaDiem") %>
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
                            CommandArgument='<%# Eval("MaDiaDiem") %>'
                            CssClass="action-link">Chỉnh Sửa</asp:LinkButton>
                        &nbsp;|&nbsp;
                        <asp:LinkButton ID="lnkDelete" runat="server"
                            CommandName="Delete"
                            CommandArgument='<%# Eval("MaDiaDiem") %>'
                            CssClass="action-link delete"
                            OnClientClick="return confirm('Xóa địa điểm này? Toàn bộ dữ liệu liên quan sẽ bị ảnh hưởng.');">
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
<%@ Page Title="Quản lý Đánh giá" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminDanhGia.aspx.cs" Inherits="DANATrip.AdminDanhGia" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-contact.css" />

    <div class="ac-page container">
        <h1 class="ac-title">Quản lý Đánh giá</h1>
        <p class="ac-subtitle">Xem và quản lý các đánh giá tour của khách hàng.</p>

        <!-- Thanh tìm kiếm -->
        <div class="ac-filter-bar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="ac-search"
                         placeholder="Tìm theo tour, khách hàng, nội dung hoặc mã đánh giá..." />
            <asp:Button ID="btnSearch" runat="server" Text="Tìm" CssClass="ac-btn ac-btn-filter"
                        OnClick="btnSearch_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="ac-msg" />

        <!-- Bảng đánh giá -->
        <asp:Repeater ID="rptReviews" runat="server" OnItemCommand="rptReviews_ItemCommand">
            <HeaderTemplate>
                <div class="ac-table">
                    <div class="ac-row ac-row-header">
                        <div class="ac-col ac-col-code">Mã ĐG</div>
                        <div class="ac-col ac-col-name">Khách hàng</div>
                        <div class="ac-col ac-col-email">Tour</div>
                        <div class="ac-col ac-col-date">Ngày đánh giá</div>
                        <div class="ac-col ac-col-content">Nội dung</div>
                        <div class="ac-col ac-col-status">Số sao</div>
                        <div class="ac-col ac-col-actions">Hành động</div>
                    </div>
            </HeaderTemplate>

            <ItemTemplate>
                <div class="ac-row">
                    <div class="ac-col ac-col-code">
                        <%# Eval("MaDanhGia") %>
                    </div>
                    <div class="ac-col ac-col-name">
                        <%# Eval("HoTen") %><br />
                        <span class="ac-subtext"><%# Eval("MaNguoiDung") %></span>
                    </div>
                    <div class="ac-col ac-col-email">
                        <%# Eval("TenTour") %><br />
                        <span class="ac-subtext"><%# Eval("MaTour") %></span>
                    </div>
                    <div class="ac-col ac-col-date">
                        <%# Eval("NgayDanhGia", "{0:dd/MM/yyyy HH:mm}") %>
                    </div>
                    <div class="ac-col ac-col-content">
                        <%# Eval("NoiDung") %>
                    </div>
                    <div class="ac-col ac-col-status">
                        <%# Eval("Sao") %> ★<br />
                        <span class='ac-status <%# (bool)Eval("HienThi") ? "ac-status-done" : "ac-status-pending" %>'>
                            <%# (bool)Eval("HienThi") ? "Đang hiển thị" : "Đang ẩn" %>
                        </span>
                    </div>
                    <div class="ac-col ac-col-actions">
                        <asp:LinkButton ID="btnToggle" runat="server" CssClass="ac-link"
                            CommandName="Toggle" CommandArgument='<%# Eval("MaDanhGia") %>'>
                            <%# (bool)Eval("HienThi") ? "Ẩn đánh giá" : "Hiện đánh giá" %>
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnDelete" runat="server"
                            CssClass="ac-link ac-link-muted"
                            CommandName="Delete" CommandArgument='<%# Eval("MaDanhGia") %>'
                            OnClientClick="return confirm('Xóa đánh giá này?');">
                            Xóa đánh giá
                        </asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <div class="ac-footer">
            <asp:Label ID="lblSummary" runat="server" />
        </div>
    </div>
</asp:Content>
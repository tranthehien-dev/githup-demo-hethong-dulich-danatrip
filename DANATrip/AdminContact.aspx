<%@ Page Title="Quản lý Liên hệ" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminContact.aspx.cs" Inherits="DANATrip.AdminContact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-contact.css" />

    <div class="ac-page container">
        <h1 class="ac-title">Quản lý Liên hệ</h1>
        <p class="ac-subtitle">Xem và xử lý các yêu cầu liên hệ từ khách hàng.</p>

        <!-- Thanh tìm kiếm + filter -->
        <div class="ac-filter-bar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="ac-search"
                         placeholder="Tìm theo tên, email hoặc nội dung..." />
            <asp:Button ID="btnSearch" runat="server" Text="Tìm" CssClass="ac-btn ac-btn-filter"
                        OnClick="btnSearch_Click" />
        </div>

        <div class="ac-tabs">
            <asp:Button ID="btnFilterAll" runat="server" Text="Tất cả"
                        CssClass="ac-tab" OnClick="btnFilterAll_Click" />
            <asp:Button ID="btnFilterPending" runat="server" Text="Chưa xử lý"
                        CssClass="ac-tab" OnClick="btnFilterPending_Click" />
            <asp:Button ID="btnFilterDone" runat="server" Text="Đã xử lý"
                        CssClass="ac-tab" OnClick="btnFilterDone_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="ac-msg" />

        <!-- Bảng liên hệ -->
        <asp:Repeater ID="rptContacts" runat="server" OnItemCommand="rptContacts_ItemCommand">
            <HeaderTemplate>
                <div class="ac-table">
                    <div class="ac-row ac-row-header">
                        <div class="ac-col ac-col-code">Mã LH</div>
                        <div class="ac-col ac-col-name">Khách hàng</div>
                        <div class="ac-col ac-col-email">Email</div>
                        <div class="ac-col ac-col-date">Ngày gửi</div>
                        <div class="ac-col ac-col-content">Nội dung</div>
                        <div class="ac-col ac-col-status">Trạng thái</div>
                        <div class="ac-col ac-col-actions">Hành động</div>
                    </div>
            </HeaderTemplate>

            <ItemTemplate>
                <div class="ac-row">
                    <div class="ac-col ac-col-code">
                        <%# Eval("MaLienHe") %>
                        <asp:HiddenField ID="hfMaLienHe" runat="server" Value='<%# Eval("MaLienHe") %>' />
                    </div>
                    <div class="ac-col ac-col-name">
                        <%# Eval("Ten") %><br />
                        <span class="ac-subtext"><%# Eval("MaNguoiDung") %></span>
                    </div>
                    <div class="ac-col ac-col-email">
                        <%# Eval("Email") %>
                    </div>
                    <div class="ac-col ac-col-date">
                        <%# Eval("NgayGui", "{0:dd/MM/yyyy HH:mm}") %>
                    </div>
                    <div class="ac-col ac-col-content">
                        <%# Eval("NoiDung") %>
                    </div>
                    <div class="ac-col ac-col-status">
                        <span class='ac-status <%# GetStatusCss(Eval("TrangThai")) %>'>
                            <%# string.IsNullOrEmpty(Eval("TrangThai").ToString()) ? "Chưa xử lý" : Eval("TrangThai") %>
                        </span>
                    </div>
                    <div class="ac-col ac-col-actions">
                        <asp:LinkButton ID="btnMarkDone" runat="server" CssClass="ac-link"
                            CommandName="MarkDone" CommandArgument='<%# Eval("MaLienHe") %>'
                            Visible='<%# CanShowMarkDone(Eval("TrangThai")) %>'>
                            đánh dấu đã xử lý
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnMarkPending" runat="server" CssClass="ac-link ac-link-muted"
                            CommandName="MarkPending" CommandArgument='<%# Eval("MaLienHe") %>'
                            Visible='<%# CanShowMarkPending(Eval("TrangThai")) %>'>
                            đánh dấu chưa xử lý
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
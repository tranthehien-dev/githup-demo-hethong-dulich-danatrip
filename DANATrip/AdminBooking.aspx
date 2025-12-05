<%@ Page Title="Quản lý Vé và Thanh toán" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminBooking.aspx.cs" Inherits="DANATrip.AdminBooking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-booking.css" />

    <div class="ab-page container">
        <h1 class="ab-title">Quản lý Vé và Thanh toán</h1>
        <p class="ab-subtitle">Theo dõi và xử lý các giao dịch đặt tour du lịch.</p>

        <!-- Thanh tìm kiếm + filter -->
        <div class="ab-filter-bar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="ab-search"
                         placeholder="Tìm theo mã đơn, tên khách hàng, email..." />
            <asp:Button ID="btnSearch" runat="server" Text="Tìm" CssClass="ab-btn ab-btn-filter"
                        OnClick="btnSearch_Click" />
        </div>

        <div class="ab-tabs">
            <asp:Button ID="btnFilterAll" runat="server" Text="Tất cả"
                        CssClass="ab-tab" OnClick="btnFilterAll_Click" />
            <asp:Button ID="btnFilterPending" runat="server" Text="Chờ thanh toán"
                        CssClass="ab-tab" OnClick="btnFilterPending_Click" />
            <asp:Button ID="btnFilterPaid" runat="server" Text="Đã thanh toán"
                        CssClass="ab-tab" OnClick="btnFilterPaid_Click" />
            <asp:Button ID="btnFilterCancelled" runat="server" Text="Đã hủy"
                        CssClass="ab-tab" OnClick="btnFilterCancelled_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="ab-msg" />

        <!-- Bảng booking -->
        <asp:Repeater ID="rptBookings" runat="server" OnItemCommand="rptBookings_ItemCommand">
            <HeaderTemplate>
                <div class="ab-table">
                    <div class="ab-row ab-row-header">
                        <div class="ab-col ab-col-code">Mã ĐV</div>
                        <div class="ab-col ab-col-name">Khách hàng</div>
                        <div class="ab-col ab-col-tour">Tên tour</div>
                        <div class="ab-col ab-col-date">Ngày đặt</div>
                        <div class="ab-col ab-col-money">Số tiền</div>
                        <div class="ab-col ab-col-status">Trạng thái</div>
                        <div class="ab-col ab-col-actions">Hành động</div>
                    </div>
            </HeaderTemplate>

            <ItemTemplate>
                <div class="ab-row">
                    <div class="ab-col ab-col-code">
                        <%# "BK-" + Eval("MaBooking") %>
                        <asp:HiddenField ID="hfMaBooking" runat="server" Value='<%# Eval("MaBooking") %>' />
                    </div>
                    <div class="ab-col ab-col-name">
                        <%# Eval("TenKhach") %><br />
                        <span class="ab-subtext"><%# Eval("Email") %></span>
                    </div>
                    <div class="ab-col ab-col-tour">
                        <%# Eval("TenTour") %>
                    </div>
                    <div class="ab-col ab-col-date">
                        <%# Eval("NgayDat", "{0:dd/MM/yyyy HH:mm}") %>
                    </div>
                    <div class="ab-col ab-col-money">
                        <%# String.Format("{0:N0} ₫", Eval("TongTien")) %>
                    </div>
                    <div class="ab-col ab-col-status">
                        <span class='ab-status <%# GetStatusCss(Eval("TrangThai").ToString()) %>'>
                            <%# Eval("TrangThai") %>
                        </span>
                    </div>
                    <div class="ab-col ab-col-actions">
                        <asp:LinkButton ID="btnMarkPaid" runat="server" CssClass="ab-link"
                            CommandName="MarkPaid" CommandArgument='<%# Eval("MaBooking") %>'
                            Visible='<%# CanShowMarkPaid(Eval("TrangThai").ToString()) %>'>xác nhận thanh toán</asp:LinkButton>

                        <asp:LinkButton ID="btnCancel" runat="server" CssClass="ab-link ab-link-danger"
                            CommandName="Cancel" CommandArgument='<%# Eval("MaBooking") %>'
                            Visible='<%# CanShowCancel(Eval("TrangThai").ToString()) %>'
                            OnClientClick="return confirm('Hủy vé này?');">hủy vé</asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
        <!-- PAGER -->
        <div class="ab-pager">
            <asp:DataList ID="dlPager" runat="server" RepeatDirection="Horizontal"
                          OnItemCommand="dlPager_ItemCommand">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkPage" runat="server"
                        CommandName="Page"
                        CommandArgument='<%# Eval("PageIndex") %>'
                        CssClass='<%# (bool)Eval("IsCurrent") ? "ab-page-link ab-page-link-active" : "ab-page-link" %>'>
                        <%# Eval("PageText") %>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:DataList>
        </div>
        <div class="ab-footer">
            <asp:Label ID="lblSummary" runat="server" />
        </div>
    </div>
</asp:Content>
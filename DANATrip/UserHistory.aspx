<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UserHistory.aspx.cs" Inherits="DANATrip.UserHistory" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Styles/user-history.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="bookings-page container">
        <div class="page-header">
            <h1>Lịch Sử Đặt Tour</h1>
            <p class="subtitle">Quản lý các đặt tour của bạn. Bạn có thể hủy các đặt tour chưa thực hiện.</p>
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <asp:Repeater ID="rptBookings" runat="server" OnItemCommand="rptBookings_ItemCommand">
            <HeaderTemplate>
                <div class="bookings-list">
            </HeaderTemplate>

            <ItemTemplate>
                <div class="booking-card">
                    <div class="booking-left">
                        <div class="order-id">#<%# Eval("MaBooking") %></div>
                        <div class="tour-name"><%# Eval("TenTour") %></div>
                        <div class="meta"><%# Eval("SoNguoiLon") %> người lớn • <%# Eval("SoTreEm") %> trẻ em</div>
                    </div>

                    <div class="booking-right">
                        <div class="price"><%# Convert.ToDecimal(Eval("TongTien")).ToString("N0") %> ₫</div>
                        <div class="date"><%# Eval("NgayDat") != DBNull.Value ? String.Format("{0:dd-MM-yyyy}", Eval("NgayDat")) : "" %></div>
                        <div class="status">Trạng thái: <strong><%# Eval("TrangThai") ?? "Đang xử lý" %></strong></div>

                        <asp:LinkButton ID="lnkCancel" runat="server"
                            CommandName="Cancel" CommandArgument='<%# Eval("MaBooking") %>'
                            CssClass="btn-cancel"
                            OnClientClick="return confirm('Bạn có chắc muốn hủy đặt tour này?');">
                            Hủy đặt tour
                        </asp:LinkButton>

                        <asp:LinkButton ID="lnkReview" runat="server"
                            CommandName="Review"
                            CommandArgument='<%# Eval("MaTour") %>'
                            CssClass="btn-review">
                            Đánh giá
                        </asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <!-- Phân trang -->
        <div class="pager">
            <asp:LinkButton ID="lnkPrev" runat="server" OnClick="lnkPrev_Click">« Trước</asp:LinkButton>
            <asp:Label ID="lblPageInfo" runat="server" />
            <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click">Sau »</asp:LinkButton>
        </div>

        <asp:Label ID="lblNoBooking" runat="server" CssClass="muted" />
    </div>
</asp:Content>
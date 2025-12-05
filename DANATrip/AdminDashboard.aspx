<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="DANATrip.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-dashboard.css" />

    <div class="db-page container">
        <h1 class="db-title">Tổng quan hệ thống</h1>
        <p class="db-subtitle">Tóm tắt nhanh về doanh thu, tour, người dùng và hoạt động gần đây.</p>

        <!-- 1. THẺ TỔNG QUAN -->
        <div class="db-cards">
            <div class="db-card">
                <div class="db-card-label">Tổng doanh thu</div>
                <div class="db-card-value">
                    <asp:Label ID="lblTongDoanhThu" runat="server" Text="0 ₫" />
                </div>
                <div class="db-card-sub">
                    Tổng tiền các đơn <span class="db-pill">Đã Thanh Toán</span>
                </div>
            </div>

            <div class="db-card">
                <div class="db-card-label">Tổng số tour đã đặt</div>
                <div class="db-card-value">
                    <asp:Label ID="lblTongBooking" runat="server" Text="0" />
                </div>
                <div class="db-card-sub">
                    Số đơn trong bảng Booking
                </div>
            </div>

            <div class="db-card">
                <div class="db-card-label">Người dùng mới (30 ngày)</div>
                <div class="db-card-value">
                    <asp:Label ID="lblUserMoi" runat="server" Text="0" />
                </div>
                <div class="db-card-sub">
                    Đăng ký mới trong 30 ngày gần đây
                </div>
            </div>

            <div class="db-card">
                <div class="db-card-label">Tổng số tour / địa điểm</div>
                <div class="db-card-value">
                    <asp:Label ID="lblTongTourDiaDiem" runat="server" Text="0" />
                </div>
                <div class="db-card-sub">
                    Tour: <asp:Label ID="lblTongTour" runat="server" /> –
                    Địa điểm: <asp:Label ID="lblTongDiaDiem" runat="server" />
                </div>
            </div>
        </div>

        <!-- 2. HÀNG GIỮA: Doanh thu + Phân loại tour -->
        <div class="db-middle">
            <div class="db-panel db-panel-chart">
                <div class="db-panel-header">
                    <h2>Doanh thu 7 ngày gần đây</h2>
                </div>
                <div class="db-chart-placeholder">
                    <!-- Ở đây mình chỉ hiển thị dưới dạng từng dòng; sau này nếu muốn dùng chart JS thì gắn vào -->
                    <asp:Repeater ID="rptRevenue7Days" runat="server">
                        <ItemTemplate>
                            <div class="db-chart-row">
                                <div class="db-chart-date"><%# Eval("Ngay") %></div>
                                <div class="db-chart-value"><%# String.Format("{0:N0} ₫", Eval("TongTien")) %></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div class="db-panel db-panel-circle">
                <div class="db-panel-header">
                    <h2>Thống kê Tour</h2>
                </div>
                <div class="db-circle-wrapper">
                    <div class="db-circle">
                        <span class="db-circle-main">
                            <asp:Label ID="lblSoTourCircle" runat="server" Text="0" />
                        </span>
                        <span class="db-circle-sub">Tổng tour</span>
                    </div>
                </div>
                <div class="db-legend">
                    <div class="db-legend-item">
                        <span class="dot dot-blue"></span>
                        Đang hiển thị: <asp:Label ID="lblTourActive" runat="server" />
                    </div>
                    <div class="db-legend-item">
                        <span class="dot dot-gray"></span>
                        Bị ẩn / tạm ẩn: <asp:Label ID="lblTourHidden" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 3. HÀNG DƯỚI: Hoạt động gần đây + Top tour -->
        <div class="db-bottom">
            <div class="db-panel db-panel-table">
                <div class="db-panel-header">
                    <h2>Hoạt động đặt tour gần đây</h2>
                </div>
                <div class="db-table">
                    <div class="db-row db-row-header">
                        <div class="db-col db-col-code">Mã ĐV</div>
                        <div class="db-col db-col-tour">Tên tour</div>
                        <div class="db-col db-col-name">Khách hàng</div>
                        <div class="db-col db-col-date">Ngày đặt</div>
                        <div class="db-col db-col-status">Trạng thái</div>
                    </div>

                    <asp:Repeater ID="rptBookingRecent" runat="server">
                        <ItemTemplate>
                            <div class="db-row">
                                <div class="db-col db-col-code">
                                    <%# "BK-" + Eval("MaBooking") %>
                                </div>
                                <div class="db-col db-col-tour">
                                    <%# Eval("TenTour") %>
                                </div>
                                <div class="db-col db-col-name">
                                    <%# Eval("TenKhach") %>
                                </div>
                                <div class="db-col db-col-date">
                                    <%# Eval("NgayDat", "{0:dd/MM/yyyy HH:mm}") %>
                                </div>
                                <div class="db-col db-col-status">
                                    <span class='db-status <%# GetBookingStatusCss(Eval("TrangThai").ToString()) %>'>
                                        <%# Eval("TrangThai") %>
                                    </span>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div class="db-panel db-panel-top">
                <div class="db-panel-header">
                    <h2>Top tour được đặt nhiều</h2>
                </div>
                <asp:Repeater ID="rptTopTours" runat="server">
                    <ItemTemplate>
                        <div class="db-top-item">
                            <div class="db-top-title"><%# Eval("TenTour") %></div>
                            <div class="db-top-bar">
                                <div class="db-top-fill" style='width:<%# Eval("Percent") %>%'></div>
                            </div>
                            <div class="db-top-meta">
                                <%# Eval("SoLan") %> lượt đặt — <%# Eval("Percent") %>%
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
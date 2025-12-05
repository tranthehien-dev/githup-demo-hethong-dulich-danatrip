<%@ Page Title="Chi tiết Tour" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TourDetail.aspx.cs" Inherits="DANATrip.TourDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link rel="stylesheet" href="Styles/tourdetail.css" />

<div class="detail-container">

    <!-- Hình lớn -->
    <asp:Image ID="imgCover" runat="server" CssClass="cover-image" />

    <!-- Tiêu đề & Info -->
    <div class="tour-header">
        <h1 class="tour-title"><asp:Label ID="lblTenTour" runat="server" /></h1>

        <div class="tour-info-row">
            <div class="info-item">
                <span class="label">Thời lượng:</span> 
                <asp:Label ID="lblThoiLuong" runat="server" />
            </div>

            <div class="info-item">
                <span class="label">Phương tiện:</span> 
                <asp:Label ID="lblPhuongTien" runat="server" />
            </div>

            <div class="info-item">
                <span class="label">Ngày Khởi hành:</span> 
                <asp:Label ID="lblNgayKhoiHanh" runat="server" />
            </div>
        </div>

        <div class="tour-price">
            Giá: <asp:Label ID="lblGia" runat="server" /> ₫
        </div>
        <div class="btn-book-wrapper">
            <asp:Button ID="btnDatTour" runat="server" Text="Đặt Tour" CssClass="btn-book-tour" OnClick="btnDatTour_Click" />
        </div>
    </div>

    <!-- Nổi bật -->
    <div class="section">
        <h2 class="section-title">Điểm nổi bật</h2>
        <div class="highlight-list">
            <asp:Repeater ID="rptHighlights" runat="server">
                <ItemTemplate>
                    <div class="highlight-item">
                        <div class="highlight-name"><%# Eval("TieuDe") %></div>
                        <div class="highlight-desc"><%# Eval("MoTa") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Lịch trình -->
    <div class="section">
        <h2 class="section-title">Lịch trình</h2>
        <div class="schedule-list">
            <asp:Repeater ID="rptSchedule" runat="server">
                <ItemTemplate>
                    <div class="schedule-item">
                        <div class="time"><%# Eval("ThoiGian") %></div>
                        <div class="content">
                            <div class="schedule-title"><%# Eval("TieuDe") %></div>
                            <div class="schedule-desc"><%# Eval("MoTa") %></div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Bao gồm -->
    <div class="section">
        <h2 class="section-title">Dịch vụ bao gồm</h2>

        <ul class="include-list">
            <asp:Repeater ID="rptIncludes" runat="server">
                <ItemTemplate>
                    <li><%# Eval("NoiDung") %></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>

</div>

</asp:Content>

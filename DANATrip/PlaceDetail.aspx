<%@ Page Title="Chi Tiết Địa Điểm" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="PlaceDetail.aspx.cs" Inherits="DANATrip.DiaDiemChiTiet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<!-- HERO -->
<link rel="stylesheet" type="text/css" href="/Styles/chitietdiadiem.css" />
<section class="hero-detail">
    <div id="heroImg" class="background-detail"></div>
    <div class="overlay-detail">
        <h1 id="lblTenDiaDiem" runat="server"></h1>
        <p id="lblViTri" runat="server"></p>
    </div>
</section>

<div class="main-detail">

    <!-- LEFT CONTENT -->
    <div class="left-side-content">

        <!-- GALLERY -->
        <div class="gallery-container">
            <asp:Repeater ID="rptAlbum" runat="server">
                <ItemTemplate>
                    <div class="gallery-item" style='background-image:url(<%# Eval("UrlAnh") %>)'></div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- GIỚI THIỆU -->
        <h2 class="section-title">Giới thiệu</h2>
        <p id="lblNoiDung" class="detail-description" runat="server"></p>

        <!-- THAM QUAN -->
        <h2 class="section-title">Các điểm tham quan chính</h2>
        <div class="exploration-grid">
            <asp:Repeater ID="rptThamQuan" runat="server">
                <ItemTemplate>
                    <div class="exploration-card">

                        <div class="exploration-image"
                            style='background-image:url(<%# Eval("HinhAnh") %>)'>
                        </div>

                        <div class="exploration-info">
                            <h3><%# Eval("TenDiem") %></h3>
                            <p><%# Eval("MoTa") %></p>
                        </div>

                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- 360 VIEW (ĐƯA XUỐNG CUỐI TRANG) -->
    </div>


    <!-- RIGHT SIDEBAR -->
    <div class="right-sidebar">

        <h3>Thông tin hữu ích</h3>

        <asp:Repeater ID="rptThongTin" runat="server">
            <ItemTemplate>
                <div class="info-box">
                    <h4><%# Eval("TieuDe") %></h4>
                    <p><%# Eval("NoiDung") %></p>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <h3 class="tour-section-title">Tour gợi ý</h3>
        <asp:Repeater ID="rptTour" runat="server">
            <ItemTemplate>
                <div class="tour-item-box">
                    <div class="tour-details">
                        <h4><%# Eval("TenTour") %></h4>
                        <p><%# Eval("MoTaNgan") %></p>

                        <div class="bottom-row">
                            <a href='TourDetail.aspx?id=<%# Eval("MaTour") %>' class="btn-view-detail">Xem chi tiết</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
<asp:Panel ID="pnl360" runat="server" Visible="false">
    <div class="container" style="margin-top:20px; margin-bottom:40px;">
        <div class="section-360-header">
            <h2 class="section-title">Khám phá 360°</h2>
            <!-- Nút random địa điểm 360 khác -->
            <button type="button" class="btn-360-random" onclick="random360()">Địa điểm khác</button>
        </div>

        <!-- iframe chính dùng bản ghi đầu tiên -->
        <div class="box-360-view">
            <iframe id="main360" runat="server"
                    class="frame-360-view"
                    allowfullscreen
                    loading="lazy"
                    referrerpolicy="no-referrer-when-downgrade"></iframe>
        </div>

        <!-- Ẩn danh sách link 360 để JS dùng random -->
        <asp:Repeater ID="rpt360" runat="server">
            <ItemTemplate>
                <input type="hidden" class="link360-item" value="<%# Eval("Link360") %>" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>
<script>
    function random360() {
        // Lấy tất cả input hidden chứa link 360
        var items = document.querySelectorAll('.link360-item');
        if (!items || items.length === 0) return;

        // Lấy src hiện tại của iframe
        var frame = document.getElementById('<%= main360.ClientID %>');
        if (!frame) return;
        var current = frame.src;

        // Sinh index random khác với link hiện tại (nếu có thể)
        var availableIndexes = [];
        for (var i = 0; i < items.length; i++) {
            var link = items[i].value;
            if (link && link !== current) {
                availableIndexes.push(i);
            }
        }

        // Nếu chỉ có 1 link hoặc tất cả giống nhau, thì random bình thường
        if (availableIndexes.length === 0) {
            for (var j = 0; j < items.length; j++) {
                if (items[j].value) {
                    frame.src = items[j].value;
                    return;
                }
            }
        } else {
            var rnd = availableIndexes[Math.floor(Math.random() * availableIndexes.length)];
            frame.src = items[rnd].value;
        }
    }
</script>
</asp:Content>

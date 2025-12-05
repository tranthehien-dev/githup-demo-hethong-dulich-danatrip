<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tour.aspx.cs" Inherits="DANATrip.Tour" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link rel="stylesheet" href="Styles/tour.css" />

<div class="tour-container">

    <h1 class="page-title">Khám Phá Các Tour Du Lịch Đà Nẵng Hấp Dẫn</h1>
    <!-- Search box -->
    <div class="search-box">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Tìm kiếm tour theo tên hoặc từ khóa"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" CssClass="btn-search" Text="Tìm kiếm" OnClick="btnSearch_Click" />
    </div>

    <!-- Tour List -->
    <div class="tour-list" id="tourList">
        <asp:Repeater ID="rptTours" runat="server">
            <ItemTemplate>
                <div class="tour-card">
                    <div class="tour-image" style="background-image:url('<%# Eval("UrlAnh") %>')"></div>

                    <div class="tour-content">
                        <h3 class="tour-title"><%# Eval("TenTour") %></h3>

                        <div class="tour-tags">
                            <%# Eval("Tags") %>
                        </div>
                        <div class="bottom-row">
                            <div class="tour-price"><%# String.Format("{0:N0}₫", Eval("GiaNguoiLon")) %></div>
                            <a href='TourDetail.aspx?id=<%# Eval("MaTour") %>' class="btn-detail">Xem chi tiết</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

</div>

</asp:Content>
<%@ Page Title="Ẩm Thực" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Food.aspx.cs" Inherits="DANATrip.AmThuc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" type="text/css" href="/Styles/amthuc.css" />
    <!-- HERO -->
    <section class="place-hero">
        <h1>Những Món Ăn Nổi Tiếng</h1>

        <div class="search-area">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" 
                         placeholder="Tìm món ăn..." />
            <asp:Button ID="btnSearch" runat="server" Text="Tìm Kiếm" CssClass="btn-search"
                        OnClick="btnSearch_Click" />
        </div>
    </section>

    <!-- GRID -->
    <section class="container">
        <h2 class="section-title">Các Món Ăn Không Thể Bỏ Lỡ</h2>

        <div class="place-grid">
            <asp:Repeater ID="rptAmThuc" runat="server">
                <ItemTemplate>

                    <a href='FoodDetail.aspx?mamon=<%# Eval("MaMon") %>' class="place-card">

                        <div class="place-img"
                             style='background-image:url(<%# Eval("HinhAnh") %>)'>
                        </div>

                        <div class="place-info">
                            <h3><%# Eval("TenMon") %></h3>
                            <p class="muted small"><%# Eval("MoTa") %></p>
                        </div>

                    </a>

                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>

</asp:Content>

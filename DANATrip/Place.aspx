<%@ Page Title="Địa Điểm" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Place.aspx.cs" Inherits="DANATrip.DiaDiem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" type="text/css" href="/Styles/diadiem.css" />
    <section class="place-hero">
        <h1>Những Điểm Đến Hấp Dẫn</h1>

        <div class="search-area">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Tìm kiếm địa điểm..." />
            <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn-search" OnClick="btnSearch_Click" />
        </div>
    </section>

    <section class="container">
        <h2 class="section-title">Các Điểm Nổi Bật Không Thể Bỏ Lỡ</h2>

        <div class="place-grid">
            <asp:Repeater ID="rptDiaDiem" runat="server">
                <ItemTemplate>
                    <a href='PlaceDetail.aspx?id=<%# Eval("MaDiaDiem") %>' class="place-card">
                        
                        <div class="place-img" 
                             style='background-image:url(<%# Eval("HinhAnhChinh") %>)'>
                        </div>

                        <div class="place-info">
                            <h3><%# Eval("TenDiaDiem") %></h3>
                            <p class="muted small"><%# Eval("NoiDung") %></p>
                        </div>

                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>

</asp:Content>

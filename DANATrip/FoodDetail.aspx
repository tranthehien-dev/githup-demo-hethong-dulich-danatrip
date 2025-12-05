<%@ Page Title="Chi tiết món ăn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FoodDetail.aspx.cs" Inherits="DANATrip.AmThucChiTiet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" type="text/css" href="/Styles/amthucchitiet.css" />

    <div class="amthuc-container">

        <!-- Tên món ăn -->
        <h2 class="title-mon"><asp:Label ID="lblTenMon" runat="server"></asp:Label></h2>
        <p class="mota-mon"><asp:Label ID="lblMoTa" runat="server"></asp:Label></p>

        <!-- Hình ảnh món ăn -->
        <div class="image-wrapper">
            <asp:Repeater ID="rpHinhAnh" runat="server">
                <ItemTemplate>
                    <img class="anh-mon" src="<%# Eval("UrlAnh") %>" />
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- LAYOUT: nội dung chính (trái) + sidebar (phải) -->
        <div class="amthuc-layout">

            <!-- Left column: nội dung chính -->
            <main class="content-left">
                <!-- Giới thiệu -->
                <section class="gioithieu">
                    <h3>Giới thiệu về <asp:Label ID="lblTenMon2" runat="server"></asp:Label></h3>
                    <p><asp:Literal ID="ltMoTaChiTiet" runat="server"></asp:Literal></p>
                </section>

                <!-- Quy trình chế biến (đã chuyển về left, nằm dưới phần giới thiệu) -->
                <section class="quytrinh">
                    <h3>Quy trình chế biến</h3>

                    <asp:Repeater ID="rpQuyTrinh" runat="server">
                        <ItemTemplate>
                            <div class="step">
                                <div class="step-number"><%# Eval("ThuTu") %></div>
                                <div class="step-content">
                                    <h4><%# Eval("MoTaBuoc") %></h4>
                                    <p>Thời gian: <%# Eval("ThoiGian") %> phút</p>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </section>

                <!-- (Nếu bạn có vùng đánh giá/khách hàng) -->
                <div class="reviews">
                    <!-- ví dụ: nơi đặt các review repeater nếu có -->
                </div>
            </main>

            <!-- Right column: sidebar chứa nguyên liệu và quán ăn -->
            <aside class="sidebar">

                <!-- Nguyên liệu -->
                <section class="nguyenlieu-box">
                    <h3>Nguyên liệu chính</h3>
                    <ul>
                        <asp:Repeater ID="rpNguyenLieu" runat="server">
                            <ItemTemplate>
                                <li>• <%# Eval("TenNguyenLieu") %></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </section>

                <!-- Quán ăn nổi tiếng (giữ ở sidebar) -->
                <section class="quanan">
                    <h3>Quán ăn nổi tiếng</h3>

                    <asp:Repeater ID="rpQuanAn" runat="server">
                        <ItemTemplate>
                            <div class="quanan-item">
                                <h4><%# Eval("TenQuanAn") %></h4>
                                <span><%# Eval("DiaChi") %></span><br />
                                <span>Hotline: <%# Eval("Sdt") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </section>

            </aside>

        </div> <!-- .amthuc-layout -->

    </div> <!-- .amthuc-container -->

</asp:Content>
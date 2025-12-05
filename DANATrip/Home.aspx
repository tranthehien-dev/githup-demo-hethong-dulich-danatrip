<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="DANATrip.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <!-- Home-specific CSS -->
    <link href="<%= ResolveUrl("~/Styles/home.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hero">
        <div class="hero-inner container">
            <h1>Đà Nẵng: Nơi Hành Trình Kỳ Diệu Bắt Đầu</h1>
            <p>Khám phá thành phố đáng sống nhất Việt Nam với những bãi biển tuyệt đẹp và văn hóa độc đáo</p>
            <asp:LinkButton runat="server" ID="btnBookNow" CssClass="btn btn-cta">Đặt vé ngay</asp:LinkButton>
        </div>
    </section>

    <section class="intro container">
        <h2>Tại sao nên chọn Đà Nẵng?</h2>
        <p class="muted">Khám phá thành phố của những cây cầu, với bãi biển được mệnh danh là một trong những
bãi biển quyến rũ nhất hành tinh, nền ẩm thực phong phú và con người thân thiện, mến
khách.</p>
    </section>

    <!-- Slider: Những Điểm Đến -->
    <section class="container section-slider">
<div class="section-header"> <h3>Những Điểm Đến Không Thể Bỏ Lỡ</h3> <asp:HyperLink runat="server" ID="lnkMorePlaces" CssClass="see-more" NavigateUrl="~/Place.aspx">Xem thêm →</asp:HyperLink> </div>
        <div class="slider-wrapper">
            <div class="slider" id="places">
                <asp:Repeater ID="rptPlaces" runat="server">
<ItemTemplate>
<a class="card" href='<%# Eval("MaDiaDiem", "PlaceDetail.aspx?id={0}") %>'>
<div class="card-img" style='background-image:url(<%# Eval("HinhAnhChinh") ?? "images/placeholder.jpg" %>)'></div>
<div class="card-body">
<h4><%# Eval("TenDiaDiem") %></h4>
<p class="muted small"><%# Eval("NoiDung", "{0}") %></p>
</div>
</a>
</ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <!-- Slider: Những món ăn -->
    <section class="container section-slider">
<div class="section-header"> <h3>Những Món Ăn Nổi Tiếng</h3> <asp:HyperLink runat="server" ID="HyperLink1" CssClass="see-more" NavigateUrl="~/Food.aspx">Xem thêm →</asp:HyperLink> </div>
        <div class="slider-wrapper">
            <div class="slider" id="foods">
                <asp:Repeater ID="rptFoods" runat="server">
                    <ItemTemplate>
                        <a class="card" href='<%# Eval("MaMon", "FoodDetail.aspx?mamon={0}") %>'>
                            <div class="card-img" style='background-image:url(<%# Eval("HinhAnh") ?? "images/placeholder.jpg" %>)'></div>
                            <div class="card-body">
                                <h4><%# Eval("TenMon") %></h4>
                                <p class="muted small"><%# Eval("MoTa", "{0}") %></p>
                            </div>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <!-- Reviews -->
    <section class="container section-slider">
    <div class="section-header"> <h3>Du Khách Nói Gì Về Đà Nẵng</h3> <asp:HyperLink runat="server" ID="HyperLink2" CssClass="see-more" NavigateUrl="~/DanhGia.aspx">Xem thêm →</asp:HyperLink> </div>
        <div class="slider-wrapper">
            <div class="slider" id="reviews">
                <asp:Repeater ID="rptReviews" runat="server">
                    <ItemTemplate>
                        <div class="review-card">
                            <div class="avatar">
                                <img src="Images/user-placeholder.jpg" alt="user" />
                            </div>
                            <div class="review-body">
                                <h4><%# Eval("HoTen") %></h4>
                                <p class="review-tour">
                                    Tour: <%# Eval("TenTour") %>
                                </p>
                                <p class="review-stars-line">
                                    <span class="review-stars">
                                        <%# new string('★', Convert.ToInt32(Eval("Sao") ?? 0)) %>
                                    </span>
                                </p>
                                <p class="review-content">
                                    <%# Eval("NoiDung") %>
                                </p>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <script>
        function slideLeft(id) {
            const el = document.getElementById(id);
            el.scrollBy({ left: -320, behavior: 'smooth' });
        }
        function slideRight(id) {
            const el = document.getElementById(id);
            el.scrollBy({ left: 320, behavior: 'smooth' });
        }
        // optional: make each slider draggable
        document.querySelectorAll('.slider').forEach(sl => {
            let isDown = false, startX, scrollLeft;
            sl.addEventListener('mousedown', (e) => {
                isDown = true; sl.classList.add('active');
                startX = e.pageX - sl.offsetLeft;
                scrollLeft = sl.scrollLeft;
            });
            sl.addEventListener('mouseleave', () => { isDown = false; sl.classList.remove('active'); });
            sl.addEventListener('mouseup', () => { isDown = false; sl.classList.remove('active'); });
            sl.addEventListener('mousemove', (e) => {
                if (!isDown) return;
                e.preventDefault();
                const x = e.pageX - sl.offsetLeft;
                const walk = (x - startX) * 1;
                sl.scrollLeft = scrollLeft - walk;
            });
        });
    </script>
</asp:Content>
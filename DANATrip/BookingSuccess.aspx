<%@ Page Title="Đặt tour thành công" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookingSuccess.aspx.cs" Inherits="DANATrip.BookingSuccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link rel="stylesheet" href="Styles/bookingsuccess.css" />

<div class="success-container">
    <div class="success-layout">
        <!-- LEFT COLUMN -->
        <div class="success-left">
            <div class="success-header">
                <div class="check-badge">
                    <!-- bạn có thể đặt icon ở đây -->
                    <img src="Images/check-circle.svg" alt="ok" style="width:34px;height:34px;" />
                </div>
                <div>
                    <h1>Đặt Tour Thành Công!</h1>
                    <p class="lead">Cảm ơn bạn đã lựa chọn khám phá Đà Nẵng cùng chúng tôi. Chi tiết đặt tour của bạn như sau:</p>
                </div>
            </div>

            <div class="booking-card">
                <div class="top-row">
                    <div>
                        <div style="font-size:13px;color:var(--muted)">Mã đặt chỗ của bạn</div>
                        <div class="booking-code"><asp:Label ID="lblMaBooking" runat="server" /></div>
                    </div>
                    <div>
                        <button type="button" class="copy-btn" onclick="navigator.clipboard && navigator.clipboard.writeText(document.querySelector('.booking-code').innerText)">Sao chép</button>
                    </div>
                </div>

                <div class="details-list">
                    <div class="label">Chi tiết Tour</div>
                    <div class="value"><asp:Label ID="lblTenTour" runat="server" /></div>

                    <div class="label">Ngày & Giờ</div>
                    <div class="value"><asp:Label ID="lblNgayKhoiHanh" runat="server" /></div>

                    <div class="label">Số lượng khách</div>
                    <div class="value"><asp:Label ID="lblNL" runat="server" /> lớn, <asp:Label ID="lblTE" runat="server" /> trẻ em</div>

                    <div class="label">Tổng cộng</div>
                    <div class="value total-row"><asp:Label ID="lblTongTien" runat="server" /> VND</div>
                </div>
            </div>

            <div class="next-steps">
                <h3>Các bước tiếp theo</h3>

                <div class="step">
                    <div class="text">
                        <div class="title">Kiểm tra email của bạn</div>
                        <div>Chúng tôi đã gửi vé điện tử và thông tin chi tiết về tour đến email của bạn.</div>
                    </div>
                </div>

                <div class="step">
                    <div class="text">
                        <div class="title">Lưu lại mã đặt chỗ</div>
                        <div>Lưu mã đặt chỗ để tiện cho việc tra cứu và hỗ trợ sau này.</div>
                    </div>
                </div>

                <div class="step">
                    <div class="text">
                        <div class="title">Liên hệ khi cần</div>
                        <div>Nếu có bất kỳ câu hỏi nào, vui lòng liên hệ bộ phận hỗ trợ của chúng tôi.</div>
                    </div>
                </div>
            </div>

            <div class="action-buttons">
                <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn-secondary" NavigateUrl="Home.aspx">Về Trang Chủ</asp:HyperLink>
                <asp:HyperLink ID="lnkExplore" runat="server" CssClass="btn-secondary" NavigateUrl="Tour.aspx">Khám Phá Tour Khác</asp:HyperLink>
                <asp:Button ID="btnLuu" runat="server" Text="Lưu mã đặt chỗ" CssClass="btn-save" OnClick="btnLuu_Click" style="display:none;" />
            </div>

            <asp:Literal ID="ltMessage" runat="server"></asp:Literal>
        </div>

        <!-- RIGHT COLUMN: Hero image -->
        <div class="success-right">
            <img class="hero-image" src="Images/hero-danang.jpg" alt="Hình tour Đà Nẵng" />
        </div>
    </div>
</div>

<script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>
<script>
    function saveBookingImage() {
        var element = document.querySelector('.booking-card');
        html2canvas(element).then(function (canvas) {
            var link = document.createElement('a');
            link.download = 'Booking-' + document.querySelector('.booking-code').innerText.trim() + '.png';
            link.href = canvas.toDataURL();
            link.click();
        });
    }

    // Nếu bạn muốn nút Lưu hiển thị trigger JS, có thể bật btnLuu (bỏ style display:none trong HTML).
</script>

</asp:Content>
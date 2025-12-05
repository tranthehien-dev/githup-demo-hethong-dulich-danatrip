<%@ Page Title="Đặt Tour" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Booking.aspx.cs" Inherits="DANATrip.Booking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link rel="stylesheet" href="Styles/booking.css" />

<div class="booking-container">

    <h1 class="booking-title">Đặt Tour</h1>

    <div class="booking-grid">

        <!-- LEFT: Thông tin khách -->
        <div class="booking-left">
            <h2 class="section-title">Thông tin liên hệ</h2>

            <!-- place to show server-side validation errors -->
            <asp:Label ID="lblError" runat="server" ForeColor="red" CssClass="error" />

            <div class="form-group">
                <label>Họ và tên *</label>
                <asp:TextBox ID="txtTen" runat="server" CssClass="input"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Số điện thoại *</label>
                <asp:TextBox ID="txtSDT" runat="server" CssClass="input" placeholder="VD: 0905123456" />
            </div>

            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input"></asp:TextBox>
            </div>

            <h2 class="section-title">Số lượng khách</h2>

            <div class="form-group">
                <label>Người lớn</label>
                <asp:TextBox ID="txtNL" runat="server" CssClass="input" Text="1"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Trẻ em</label>
                <asp:TextBox ID="txtTE" runat="server" CssClass="input" Text="0"></asp:TextBox>
            </div>

            <h2 class="section-title">Phương thức thanh toán</h2>
            <div class="form-group">
                <asp:RadioButtonList ID="rblPayment" runat="server" RepeatDirection="Vertical" CssClass="input">
                    <asp:ListItem Value="offline" Selected="True">Thanh toán khi đến</asp:ListItem>
                    <asp:ListItem Value="online">Thanh toán Online</asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>

        <!-- RIGHT: Thông tin tour & tổng tiền -->
        <div class="booking-right">
            <h2 class="section-title">Thông tin Tour</h2>
            <div class="tour-card">
                <asp:Image ID="imgTour" runat="server" CssClass="tour-img" />
                <div class="tour-info">
                    <h3><asp:Label ID="lblTenTour" runat="server" /></h3>
                    <p>Giá người lớn: <span class="price"><asp:Label ID="lblGia" runat="server" /></span> ₫</p>
                    <p>Giá trẻ em: <span class="price"><asp:Label ID="lblGia0" runat="server" /></span> ₫</p>
                </div>
            </div>

            <h2 class="section-title">Tổng tiền</h2>
            <div class="total-price" id="totalPrice">0 ₫</div>

            <!-- OnClientClick will call client-side validator; return false sẽ ngăn postback -->
            <asp:Button ID="btnBook" runat="server" Text="Hoàn tất đặt tour"
                CssClass="btn-book" OnClick="btnBook_Click" OnClientClick="return validateAndConfirm();" />
        </div>

    </div>
</div>

<!-- client-side scripts: parse price, calcTotal, validate -->
<script>
    function parseNumber(text) {
        if (!text) return 0;
        return parseFloat(text.replace(/[^0-9\.,]/g, "").replace(/,/g, "")) || 0;
    }

    function calcTotal() {
        const lblAdult = document.getElementById("<%= lblGia.ClientID %>");
        const lblChild = document.getElementById("<%= lblGia0.ClientID %>");
        const adultPrice = parseNumber(lblAdult ? lblAdult.innerText : "0");
        const childPrice = parseNumber(lblChild ? lblChild.innerText : "0");

        const nlEl = document.getElementById("<%= txtNL.ClientID %>");
        const teEl = document.getElementById("<%= txtTE.ClientID %>");
        const nl = parseInt(nlEl ? nlEl.value : "0") || 0;
        const te = parseInt(teEl ? teEl.value : "0") || 0;

        const total = (nl * adultPrice) + (te * childPrice);

        document.getElementById("totalPrice").innerHTML = total.toLocaleString() + " ₫";
    }

    // validate phone VN: +84 or 0 prefix, then common mobile starts 3|5|7|8|9 and 8 more digits
    function isValidPhoneVN(phone) {
        if (!phone) return false;
        phone = phone.trim();
        var re = /^(?:\+84|0)(?:3|5|7|8|9)\d{8}$/;
        return re.test(phone);
    }

    function validateAndConfirm() {
        // clear server-side error label if present (will be refreshed on postback anyway)
        var clientError = "";

        var nlEl = document.getElementById("<%= txtNL.ClientID %>");
        var teEl = document.getElementById("<%= txtTE.ClientID %>");
        var sdtEl = document.getElementById("<%= txtSDT.ClientID %>");

        var nl = nlEl ? parseInt(nlEl.value) || 0 : 0;
        var te = teEl ? parseInt(teEl.value) || 0 : 0;
        var sdt = sdtEl ? sdtEl.value.trim() : "";

        if ((nl <= 0) && (te <= 0)) {
            clientError = "Vui lòng nhập số lượng ít nhất 1 khách (người lớn hoặc trẻ em).";
        } else if (!isValidPhoneVN(sdt)) {
            clientError = "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam (ví dụ 0905123456 hoặc +84905123456).";
        }

        if (clientError) {
            // show temporary alert and set lblError (server label) text via DOM if present
            alert(clientError);
            var lblErr = document.getElementById("<%= lblError.ClientID %>");
            if (lblErr) lblErr.innerText = clientError;
            return false; // prevent postback
        }

        // OK -> allow postback
        return true;
    }

    // Recalculate total when inputs change
    document.addEventListener("DOMContentLoaded", function () {
        calcTotal();
        var nlEl = document.getElementById("<%= txtNL.ClientID %>");
        var teEl = document.getElementById("<%= txtTE.ClientID %>");
        if (nlEl) nlEl.addEventListener("input", calcTotal);
        if (teEl) teEl.addEventListener("input", calcTotal);
    });
</script>
</asp:Content>
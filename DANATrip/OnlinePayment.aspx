<%@ Page Title="Thanh toán Online" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="OnlinePayment.aspx.cs" Inherits="DANATrip.OnlinePayment" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .pay-container {
            max-width: 1100px;
            margin: 20px auto 40px;
            padding: 0 20px;
        }
        .pay-title {
            font-size: 26px;
            margin-bottom: 10px;
            color: #0b2a2b;
        }
        .pay-grid {
            display: grid;
            grid-template-columns: 3fr 2fr;
            gap: 20px;
        }
        .pay-left, .pay-right {
            background: #f6fbfb;
            border-radius: 12px;
            padding: 18px 20px;
        }
        .section-title {
            font-size: 18px;
            margin-bottom: 10px;
            font-weight: 600;
        }
        .form-group {
            margin-bottom: 12px;
        }
        .form-group label {
            display: block;
            font-size: 13px;
            margin-bottom: 4px;
        }
        .input, select {
            width: 100%;
            padding: 8px 10px;
            border-radius: 8px;
            border: 1px solid #d9e5e5;
            box-sizing: border-box;
        }
        .pay-method-box {
            margin-top: 10px;
            background: #fff;
            padding: 12px 14px;
            border-radius: 10px;
            border: 1px solid #e3eded;
        }
        .qr-info {
            text-align: center;
        }
        .qr-code {
            width: 180px;
            height: 180px;
            background: #eee;
            margin: 10px auto;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
            color: #888;
        }
        .bank-row {
            margin-bottom: 6px;
            font-size: 14px;
        }
        .pay-right .tour-card {
            background: #fff;
            border-radius: 10px;
            padding: 10px 12px;
            display: flex;
            gap: 10px;
            margin-bottom: 12px;
        }
        .tour-img {
            width: 80px;
            height: 80px;
            object-fit: cover;
            border-radius: 8px;
            background: #ddd;
        }
        .tour-name {
            font-weight: 600;
            margin-bottom: 4px;
        }
        .price-line {
            font-size: 13px;
            color: #4e6666;
        }
        .total-label {
            margin-top: 8px;
            font-size: 14px;
            color: #4e6666;
        }
        .total-amount {
            font-size: 20px;
            font-weight: 700;
            color: #007bff;
        }
        .btn {
            padding: 9px 16px;
            border-radius: 999px;
            border: none;
            cursor: pointer;
            font-size: 14px;
        }
        .btn-primary {
            background: linear-gradient(180deg, #007bff, #0063d1);
            color: white;
            padding: 10px 22px;
            border-radius: 12px;
            border: none;
            cursor: pointer;
            font-weight: 600;
            box-shadow: 0 8px 20px rgba(3, 102, 214, 0.16);
            text-decoration: none
        }
        .msg {
            display: block;
            font-size: 13px;
            margin-top: 6px;
        }
        .msg.error { color: #d9534f; }
        .msg.success { color: #28a745; }

        @media (max-width: 900px) {
            .pay-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pay-container">
        <h1 class="pay-title">Thanh toán</h1>

        <asp:Label ID="lblError" runat="server" CssClass="msg error" />

        <div class="pay-grid">
            <!-- LEFT: Chọn phương thức + form -->
            <div class="pay-left">
                <h2 class="section-title">Chọn phương thức thanh toán</h2>

                <div class="form-group">
                    <label>Phương thức</label>
                    <asp:DropDownList ID="ddlMethod" runat="server" CssClass="input"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlMethod_SelectedIndexChanged">
                        <asp:ListItem Text="-- Chọn phương thức --" Value="" />
                        <asp:ListItem Text="Quét mã QR" Value="QR" />
                        <asp:ListItem Text="Thẻ nội địa" Value="DOM" />
                        <asp:ListItem Text="Thẻ quốc tế (Visa/MasterCard)" Value="INT" />
                    </asp:DropDownList>
                </div>

                <asp:Panel ID="pnlQR" runat="server" CssClass="pay-method-box" Visible="false">
                    <h3 class="section-title" style="font-size:16px;">Thanh toán bằng QR Code</h3>
                    <div class="qr-info">
                        <div class="qr-code">
                            QR CODE<br />GIẢ LẬP
                        </div>
                        <div class="bank-row"><strong>Ngân hàng:</strong> Vietcombank</div>
                        <div class="bank-row"><strong>Số tài khoản:</strong> 0123456789</div>
                        <div class="bank-row"><strong>Chủ tài khoản:</strong> CÔNG TY DU LỊCH DANA TRIP</div>
                        <div class="bank-row"><strong>Nội dung chuyển khoản:</strong> <asp:Label ID="lblNoiDungCK" runat="server" /></div>
                        <span class="msg">Vui lòng quét mã và chuyển khoản đúng nội dung để hệ thống xác nhận.</span>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlDomestic" runat="server" CssClass="pay-method-box" Visible="false">
                    <h3 class="section-title" style="font-size:16px;">Thẻ ngân hàng nội địa</h3>
                    <div class="form-group">
                        <label>Ngân hàng</label>
                        <asp:TextBox ID="txtBankName" runat="server" CssClass="input" Placeholder="VD: Vietcombank, ACB..." />
                    </div>
                    <div class="form-group">
                        <label>Tên chủ tài khoản</label>
                        <asp:TextBox ID="txtAccountName" runat="server" CssClass="input" />
                    </div>
                    <div class="form-group">
                        <label>Số tài khoản</label>
                        <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="input" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlInternational" runat="server" CssClass="pay-method-box" Visible="false">
                    <h3 class="section-title" style="font-size:16px;">Thẻ quốc tế (Visa/MasterCard)</h3>
                    <div class="form-group">
                        <label>Tên in trên thẻ</label>
                        <asp:TextBox ID="txtCardName" runat="server" CssClass="input" />
                    </div>
                    <div class="form-group">
                        <label>Số thẻ</label>
                        <asp:TextBox ID="txtCardNumber" runat="server" CssClass="input" Placeholder="xxxx xxxx xxxx xxxx" />
                    </div>
                    <div class="form-group">
                        <label>Ngày hết hạn (MM/YY)</label>
                        <asp:TextBox ID="txtExpire" runat="server" CssClass="input" Placeholder="MM/YY" />
                    </div>
                    <div class="form-group">
                        <label>CVV/CVC</label>
                        <asp:TextBox ID="txtCVV" runat="server" CssClass="input" TextMode="Password" />
                    </div>
                </asp:Panel>

                <div class="form-group" style="margin-top:16px;">
                    <asp:Button ID="btnConfirm" runat="server" Text="Xác nhận thanh toán"
                        CssClass="btn btn-primary" OnClick="btnConfirm_Click" />
                </div>
            </div>

            <!-- RIGHT: Thông tin tour + tổng tiền -->
            <div class="pay-right">
                <h2 class="section-title">Thông tin vé</h2>

                <div class="tour-card">
                    <asp:Image ID="imgTour" runat="server" CssClass="tour-img" />
                    <div>
                        <div class="tour-name">
                            <asp:Label ID="lblTenTour" runat="server" />
                        </div>
                        <div class="price-line">
                            Họ tên khách: <asp:Label ID="lblTenKhach" runat="server" />
                        </div>
                        <div class="price-line">
                            Số lượng: <asp:Label ID="lblSL" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="total-label">Tổng tiền</div>
                <div class="total-amount">
                    <asp:Label ID="lblTongTien" runat="server" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
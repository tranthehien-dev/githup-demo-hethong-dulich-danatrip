<%@ Page Title="Đánh giá tour" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DanhGiaDetail.aspx.cs" Inherits="DANATrip.DanhGiaDetail" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .review-page.container {
            max-width: 800px;
            margin: 20px auto 40px;
            padding: 0 20px;
        }
        .review-header h1 {
            margin: 6px 0 4px;
            font-size: 26px;
            color: #0b2a2b;
        }
        .review-header p {
            color: #6f8b8b;
            margin-bottom: 16px;
        }
        .review-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 8px 22px rgba(6,20,25,0.04);
            padding: 18px 20px;
            display:block
        }
        .tour-title {
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 10px;
        }
        .stars {
            display: inline-flex;
            gap: 6px;
            margin-top: 4px;
            direction: rtl;
            justify-content: flex-start;
        }
        .stars input[type="radio"] {
            display: none;
        }
        .stars label {
            font-size: 26px;
            cursor: pointer;
            color: #ccc;
            user-select: none;
        }
        .stars label:hover,
        .stars label:hover ~ label {
            color: #ffb400;
        }
        .stars input[type="radio"]:checked ~ label,
        .stars input[type="radio"]:checked ~ label ~ label {
            color: #ffb400;
        }
        .form-group {
            margin-top: 12px;
        }
        .form-group label {
            display: block;
            font-size: 13px;
            margin-bottom: 4px;
            color: #274646;
        }
        .textarea {
            width: 100%;
            min-height: 100px;
            border-radius: 8px;
            border: 1px solid #e1ecec;
            padding: 10px;
            box-sizing: border-box;
            resize: vertical;
            font-family: inherit;
        }
        .msg {
            display: block;
            margin-top: 8px;
            font-size: 14px;
        }
        .msg.error { color: #d9534f; }
        .msg.success { color: #28a745; }
        .btn {
            padding: 9px 16px;
            border-radius: 8px;
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

            .btn-primary:hover {
                filter: brightness(.92)
            }

        .review-footer {
            margin-top: 8px;
            font-size: 13px;
            color: #6f8b8b;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="review-page container">
        <div class="review-header">
            <h1>Đánh Giá Tour</h1>
            <p>Hãy chia sẻ trải nghiệm của bạn về tour này.</p>
        </div>

        <div class="review-card">
            <div class="tour-title">
                Tour: <asp:Label ID="lblTenTour" runat="server" Text="(Đang tải...)" />
            </div>

            <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

            <div class="form-group">
                <label>Chọn số sao *</label>
                <div class="stars">
                    <asp:RadioButton ID="r5" runat="server" GroupName="rating" />
                    <label for="<%= r5.ClientID %>">★</label>

                    <asp:RadioButton ID="r4" runat="server" GroupName="rating" />
                    <label for="<%= r4.ClientID %>">★</label>

                    <asp:RadioButton ID="r3" runat="server" GroupName="rating" />
                    <label for="<%= r3.ClientID %>">★</label>

                    <asp:RadioButton ID="r2" runat="server" GroupName="rating" />
                    <label for="<%= r2.ClientID %>">★</label>

                    <asp:RadioButton ID="r1" runat="server" GroupName="rating" />
                    <label for="<%= r1.ClientID %>">★</label>
                </div>
            </div>

            <div class="form-group">
                <label>Nội dung đánh giá (tùy chọn)</label>
                <asp:TextBox ID="txtNoiDung" runat="server" CssClass="textarea" TextMode="MultiLine" />
            </div>

            <div class="form-group">
                <asp:Button ID="btnSubmit" runat="server" Text="Gửi đánh giá"
                    CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
            </div>

            <div class="review-footer">
                Đánh giá của bạn sẽ được lưu lại cùng thời gian hiện tại.
            </div>
        </div>
    </div>
</asp:Content>
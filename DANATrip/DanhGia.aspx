<%@ Page Title="Đánh giá tour" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="DanhGia.aspx.cs" Inherits="DANATrip.DanhGia" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .review-list-page.container {
            max-width: 1000px;
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
        .review-cards {
            display: flex;
            flex-direction: column;
            gap: 14px;
        }
        .review-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 8px 22px rgba(6,20,25,0.04);
            padding: 14px 18px;
        }
        .review-tour-name {
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 4px;
        }
        .review-meta {
            font-size: 13px;
            color: #6f8b8b;
            margin-bottom: 6px;
        }
        .review-stars {
            color: #ffb400;
            margin-bottom: 6px;
            font-size: 14px;
        }
        .review-content {
            font-size: 14px;
        }
        .muted {
            color: #6f8b8b;
            margin-top: 10px;
            display: block;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="review-list-page container">
        <div class="review-header">
            <h1>Đánh Giá Từ Khách Hàng</h1>
            <p>Xem những cảm nhận của khách đã trải nghiệm các tour của DANA Trip.</p>
        </div>

        <asp:Repeater ID="rptDanhGia" runat="server">
            <HeaderTemplate>
                <div class="review-cards">
            </HeaderTemplate>

            <ItemTemplate>
                <div class="review-card">
                    <div class="review-tour-name">
                        <%# Eval("TenTour") %>
                    </div>
                    <div class="review-meta">
                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("HoTen")))
                            ? "Khách ẩn danh"
                            : "Bởi: " + Eval("HoTen") %>
                        &nbsp; | &nbsp;
                        Ngày đánh giá:
                        <%# Eval("NgayDanhGia") != DBNull.Value ? String.Format("{0:dd-MM-yyyy HH:mm}", Eval("NgayDanhGia")) : "" %>
                    </div>
                    <div class="review-stars">
                        <%# new string('★', Convert.ToInt32(Eval("Sao") ?? 0)) %>
                    </div>
                    <div class="review-content">
                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("NoiDung"))) ? "(Khách không ghi nội dung)" : Eval("NoiDung") %>
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Label ID="lblEmpty" runat="server" CssClass="muted" />
    </div>
</asp:Content>
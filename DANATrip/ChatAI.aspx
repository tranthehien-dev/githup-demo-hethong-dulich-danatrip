<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChatAI.aspx.cs" Inherits="DANATrip.ChatAI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/bot.css" />

    <div class="bot-wrapper container">
        <div class="bot-header">
            <div class="bot-header-left">
                <div class="bot-avatar"></div>
                <div>
                    <div class="bot-name">Danang Discovery Bot</div>
                    <div class="bot-status">Online</div>
                </div>
            </div>
            <div class="bot-header-right">
                <asp:LinkButton ID="btnRefresh" runat="server" CssClass="bot-header-link"
                    OnClick="btnRefresh_Click">refresh</asp:LinkButton>
                <asp:LinkButton ID="btnClose" runat="server" CssClass="bot-header-link"
                    PostBackUrl="~/Home.aspx">close</asp:LinkButton>
            </div>
        </div>

        <div class="bot-body">
            <asp:UpdatePanel ID="upChat" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="chatScroll" class="chat-list">
                        <asp:Repeater ID="rptChat" runat="server">
                            <ItemTemplate>
                                <%# GetBubble(Container.DataItem) %>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div class="bot-footer">
            <div class="bot-suggestions">
                <asp:LinkButton ID="btnSug1" runat="server" CssClass="chip"
                    OnClick="btnSuggestion_Click"
                    CommandArgument="Top 5 địa điểm nổi tiếng ở Đà Nẵng là gì?">
                    Top 5 địa điểm?
                </asp:LinkButton>
                <asp:LinkButton ID="btnSug2" runat="server" CssClass="chip"
                    OnClick="btnSuggestion_Click"
                    CommandArgument="Gợi ý lịch trình du lịch Đà Nẵng 3 ngày 2 đêm.">
                    Lịch trình 3N2Đ
                </asp:LinkButton>
                <asp:LinkButton ID="btnSug3" runat="server" CssClass="chip"
                    OnClick="btnSuggestion_Click"
                    CommandArgument="Những món ăn đặc sản phải thử ở Đà Nẵng.">
                    Ẩm thực nên thử
                </asp:LinkButton>
            </div>

            <asp:UpdatePanel ID="upInput" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="bot-input-row">
                        <asp:TextBox ID="txtMessage" runat="server" CssClass="bot-input"
                            placeholder="Nhập câu hỏi của bạn..." />
                        <asp:Button ID="btnSend" runat="server" Text="send"
                            CssClass="bot-send" OnClick="btnSend_Click" />
                    </div>
                    <asp:Label ID="lblError" runat="server" CssClass="bot-error" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <script>
        function scrollChatBottom() {
            var el = document.getElementById('chatScroll');
            if (el) el.scrollTop = el.scrollHeight;
        }
        if (typeof (Sys) !== "undefined") {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () { scrollChatBottom(); });
        } else {
            document.addEventListener("DOMContentLoaded", scrollChatBottom);
        }
    </script>
</asp:Content>
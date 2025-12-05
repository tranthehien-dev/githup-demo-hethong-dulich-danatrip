<%@ Page Title="Thông tin cá nhân" Language="C#" MasterPageFile="~/site1.master" AutoEventWireup="true" CodeBehind="UserProfile.aspx.cs" Inherits="DANATrip.UserProfile" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Styles/user-profile.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="profile-page container">
        <div class="profile-header">
            <h1>Thông Tin Cá Nhân</h1>
            <p class="subtitle">Quản lý thông tin hồ sơ của bạn để cá nhân hóa trải nghiệm.</p>
        </div>

        <div class="profile-grid">
            <section class="profile-left card">
                <div class="profile-top">
                    <div class="profile-meta">
                        <h2 id="lblHoTen" runat="server"></h2>
                        <div class="joined">Tham gia từ <asp:Label ID="lblJoined" runat="server" /></div>
                    </div>
                </div>

                <hr />

                <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

                <div class="form-row">
                    <label>Họ và Tên</label>
                    <asp:TextBox ID="txtHoTen" runat="server" CssClass="input" />
                </div>

                <div class="form-row two-cols">
                    <div>
                        <label>Số điện thoại</label>
                        <asp:TextBox ID="txtSDT" runat="server" CssClass="input" />
                    </div>
                    <div>
                        <label>Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" ReadOnly="true" />
                    </div>
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu thay đổi" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn btn-secondary" OnClientClick="history.back(); return false;" />
                </div>
            </section>
        </div>
    </div>
</asp:Content>
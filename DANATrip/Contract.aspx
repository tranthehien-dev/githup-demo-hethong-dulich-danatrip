<%@ Page Title="Liên hệ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contract.aspx.cs" Inherits="DANATrip.Contract" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Styles/contract.css") %>" rel="stylesheet" />
    <!-- Material Icons để hiển thị icon thay cho text -->
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="contract-page container">
        <header class="contract-header">
            <h1>Liên Hệ Với Chúng Tôi</h1>
            <p class="lead">Cần hỗ trợ? Chúng tôi luôn sẵn lòng lắng nghe bạn. Vui lòng điền vào biểu mẫu bên dưới.</p>
        </header>

        <div class="contract-grid">
            <!-- Left: contact form -->
            <div class="card form-card">
                <h3>Gửi yêu cầu của bạn</h3>

                <asp:Label ID="lblMessage" runat="server" CssClass="msg" />
                <div class="row two-cols">
                    <div class="form-field">
                        <label>Họ và Tên</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="input" placeholder="Nhập họ và tên của bạn"></asp:TextBox>
                    </div>
                    <div class="form-field">
                        <label>Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" placeholder="Nhập địa chỉ email của bạn"></asp:TextBox>
                    </div>
                </div>

                <div class="form-field">
                    <label>Chủ đề</label>
                    <asp:TextBox ID="txtSubject" runat="server" CssClass="input" placeholder="Nhập chủ đề tin nhắn"></asp:TextBox>
                </div>

                <div class="form-field">
                    <label>Nội dung tin nhắn</label>
                    <asp:TextBox ID="txtMessageBody" runat="server" TextMode="MultiLine" CssClass="textarea" Rows="6" placeholder="Nội dung bạn muốn gửi đến chúng tôi..."></asp:TextBox>
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnSend" runat="server" Text="Gửi Tin Nhắn" CssClass="btn btn-primary" OnClick="btnSend_Click" OnClientClick="return validateContactForm();" />
                </div>
            </div>

            <!-- Right: contact details + image -->
            <div class="right-col">
                <div class="card contact-card">
                    <h3>Kết Nối Trực Tiếp</h3>

                    <div class="contact-item">
                        <!-- đổi text 'call' thành icon material -->
                        <i class="material-icons icon" aria-hidden="true">call</i>
                        <div class="value">
                            <strong>Hỗ trợ du khách</strong>
                            <div class="muted">(+84) 236 3888 888</div>
                        </div>
                    </div>

                    <div class="contact-item">
                        <i class="material-icons icon" aria-hidden="true">email</i>
                        <div class="value">
                            <strong>Email chung</strong>
                            <div class="muted">support@danangtourism.vn</div>
                        </div>
                    </div>

                    <div class="contact-item">
                        <i class="material-icons icon" aria-hidden="true">location_on</i>
                        <div class="value">
                            <strong>Văn phòng</strong>
                            <div class="muted">12 Trần Phú, Hải Châu, Đà Nẵng</div>
                        </div>
                    </div>
                </div>

                <div class="image-card">
                    <img src="<%= ResolveUrl("~/Images/bg-danang2.jpg") %>" alt="contact image" />
                </div>
            </div>
        </div>
    </div>

    <script>
        function isValidEmail(email) {
            if (!email) return false;
            var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        }

        function validateContactForm() {
            var name = document.getElementById("<%= txtName.ClientID %>").value.trim();
            var email = document.getElementById("<%= txtEmail.ClientID %>").value.trim();
            var message = document.getElementById("<%= txtMessageBody.ClientID %>").value.trim();

            var err = "";
            if (name.length < 2) err = "Vui lòng nhập họ và tên đầy đủ.";
            else if (!isValidEmail(email)) err = "Vui lòng nhập email hợp lệ.";
            else if (message.length < 6) err = "Nội dung tin nhắn quá ngắn.";

            var lbl = document.getElementById("<%= lblMessage.ClientID %>");
            if (err) {
                if (lbl) {
                    lbl.innerText = err;
                    lbl.className = 'msg error';
                } else alert(err);
                return false;
            }

            if (lbl) {
                lbl.innerText = "Đang gửi...";
                lbl.className = 'msg info';
            }
            return true;
        }
    </script>
</asp:Content>
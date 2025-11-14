<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="hosonguoidung.Profile" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Thông Tin Cá Nhân</title>

    <!-- Bootstrap để làm giao diện đẹp -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>

<body style="background-color: #f5f5f5;">
    <form id="form1" runat="server">

        <div class="container mt-4">

            <h2 class="mb-2">Thông Tin Cá Nhân</h2>
            <p class="text-muted">Quản lý thông tin hồ sơ của bạn để cá nhân hóa trải nghiệm.</p>

            <!-- CARD THÔNG TIN -->
            <div class="card p-4 shadow-sm">

                <!-- Avatar + tên -->
                <div class="d-flex align-items-center mb-4">
                    <img src="https://i.imgur.com/QpYjFvW.jpeg" width="80" class="rounded-circle me-3" />
                    <div>
                        <h4 class="mb-1"><asp:Label ID="lblHoTen" runat="server"></asp:Label></h4>
                        <span class="badge bg-warning text-dark">Thành viên</span>
                        <p class="text-muted mb-0">Tham gia từ <asp:Label ID="lblNam" runat="server"></asp:Label></p>
                    </div>
                </div>

                <!-- Form -->
                <div class="row mb-3">
                    <div class="col-6">
                        <label>Họ và tên</label>
                        <asp:TextBox ID="txtHoTen" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>
                    <div class="col-6">
                        <label>Số điện thoại</label>
                        <asp:TextBox ID="txtSDT" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>
                </div>

                <div class="mb-3">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label>Địa chỉ</label>
                    <asp:TextBox ID="txtDiaChi" CssClass="form-control" runat="server"></asp:TextBox>
                </div>

                <!-- Buttons -->
                <div class="text-end">
                    <asp:Button ID="btnCancel" CssClass="btn btn-secondary me-2" Text="Hủy" runat="server" />
                    <asp:Button ID="btnSave" CssClass="btn btn-danger" Text="Lưu thay đổi" runat="server" OnClick="btnSave_Click" />
                </div>

            </div>

            <!-- LỊCH SỬ TOUR -->
            <h3 class="mt-5">Lịch Sử Đặt Tour</h3>

            <ul class="nav nav-tabs mb-3">
                <li class="nav-item"><a class="nav-link active">Sắp tới</a></li>
                <li class="nav-item"><a class="nav-link">Đã hoàn thành</a></li>
                <li class="nav-item"><a class="nav-link">Đã hủy</a></li>
            </ul>

            <div class="card text-center p-5 shadow-sm">
                <h4>Chưa có chuyến đi nào</h4>
                <p class="text-muted">Bạn chưa đặt tour nào sắp tới. Hãy bắt đầu hành trình ngay hôm nay!</p>
                <a href="Tour.aspx" class="btn btn-danger px-4">Khám phá các tour</a>
            </div>

        </div>

    </form>
</body>
</html>

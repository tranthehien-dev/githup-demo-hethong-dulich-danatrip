<%@ Page Title="Quản lý Món ăn" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AdminFoodEdit.aspx.cs" Inherits="DANATrip.AdminFoodEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-place-edit.css" />

    <div class="admin-page container">
        <h1 class="page-title">
            <asp:Literal ID="litTitle" runat="server" />
        </h1>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <!-- 1. THÔNG TIN CHUNG MÓN ĂN -->
        <div class="form-grid">
            <div class="form-group">
                <label>Mã món</label>
                <asp:TextBox ID="txtMaMon" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Tên món</label>
                <asp:TextBox ID="txtTenMon" runat="server" CssClass="input" />
            </div>
            <div class="form-group full">
                <label>Mô tả</label>
                <asp:TextBox ID="txtMoTa" runat="server" CssClass="input" TextMode="MultiLine" Rows="4" />
            </div>
            <div class="form-group">
                <label>Hình ảnh chính (upload hoặc URL)</label>
                <asp:Image ID="imgHinhChinh" runat="server" CssClass="place-main-img" />
                <asp:FileUpload ID="fuHinhChinh" runat="server" CssClass="input" />
                <small>Hoặc nhập trực tiếp URL:</small>
                <asp:TextBox ID="txtHinhAnhChinh" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Trạng thái</label>
                <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="input">
                    <asp:ListItem Text="Hoạt động" Value="Hoạt động" />
                    <asp:ListItem Text="Tạm ẩn" Value="Tạm ẩn" />
                    <asp:ListItem Text="Chờ duyệt" Value="Chờ duyệt" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Hiển thị trên site</label>
                <asp:CheckBox ID="chkHienThiEdit" runat="server" Checked="true" />
            </div>
        </div>

        <!-- 2. ALBUM HÌNH ẢNH -->
        <h2 class="section-subtitle">Album hình ảnh món ăn</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptAlbum" runat="server" OnItemCommand="rptAlbum_ItemCommand">
                <HeaderTemplate>
                    <div class="album-grid">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="album-item">
                        <img src="<%# Eval("UrlAnh") %>" alt="ảnh" class="album-thumb" />
                        <asp:TextBox ID="txtUrlAnh" runat="server"
                            CssClass="input input-url"
                            Text='<%# Eval("UrlAnh") %>' />
                        <asp:HiddenField ID="hfMaAnh" runat="server" Value='<%# Eval("MaAnh") %>' />
                        <asp:Button ID="btnSaveImg" runat="server" Text="Lưu"
                            CssClass="btn btn-small"
                            CommandName="SaveImg"
                            CommandArgument='<%# Eval("MaAnh") %>' />
                        <asp:Button ID="btnDelImg" runat="server" Text="Xóa"
                            CssClass="btn btn-small btn-danger"
                            CommandName="DelImg"
                            CommandArgument='<%# Eval("MaAnh") %>' />
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <label>Thêm ảnh mới (upload):</label>
                <asp:FileUpload ID="fuAlbum" runat="server" CssClass="input" />
                <asp:Button ID="btnAddAlbum" runat="server" Text="Thêm ảnh"
                    CssClass="btn btn-primary btn-small" OnClick="btnAddAlbum_Click" />
            </div>
        </div>

        <!-- 3. NGUYÊN LIỆU -->
        <h2 class="section-subtitle">Nguyên liệu chính</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptNguyenLieu" runat="server" OnItemCommand="rptNguyenLieu_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã NL</div>
                            <div class="info-col title">Tên nguyên liệu</div>
                            <div class="info-col desc">Mô tả</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMaNL" runat="server" CssClass="input input-small"
                                Text='<%# Eval("MaNguyenLieu") %>' Enabled="false" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtTenNL" runat="server" CssClass="input"
                                Text='<%# Eval("TenNguyenLieu") %>' />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtMoTaNL" runat="server" CssClass="input"
                                Text='<%# Eval("MoTa") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSaveNL" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaNguyenLieu") %>' />
                            <asp:Button ID="btnDelNL" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaNguyenLieu") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm nguyên liệu mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMaNL" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewTenNL" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewMoTaNL" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddNL" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddNL_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 4. QUY TRÌNH CHẾ BIẾN -->
        <h2 class="section-subtitle">Quy trình chế biến</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptQuyTrinh" runat="server" OnItemCommand="rptQuyTrinh_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã bước</div>
                            <div class="info-col title">Mô tả bước</div>
                            <div class="info-col title">Thời gian (phút)</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMaBuoc" runat="server" CssClass="input input-small"
                                Text='<%# Eval("MaBuocCheBien") %>' Enabled="false" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtMoTaBuoc" runat="server" CssClass="input"
                                Text='<%# Eval("MoTaBuoc") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtThoiGian" runat="server" CssClass="input input-small"
                                Text='<%# Eval("ThoiGian") %>' />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSaveBuoc" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaBuocCheBien") %>' />
                            <asp:Button ID="btnDelBuoc" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaBuocCheBien") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm bước chế biến mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMaBuoc" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewMoTaBuoc" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewThoiGian" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddBuoc" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddBuoc_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 5. QUÁN ĂN -->
        <h2 class="section-subtitle">Quán ăn nổi tiếng</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptQuanAn" runat="server" OnItemCommand="rptQuanAn_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã quán</div>
                            <div class="info-col title">Tên quán</div>
                            <div class="info-col desc">Địa chỉ</div>
                            <div class="info-col title">SĐT</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMaQA" runat="server" CssClass="input input-small"
                                Text='<%# Eval("MaQuanAn") %>' Enabled="false" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtTenQA" runat="server" CssClass="input"
                                Text='<%# Eval("TenQuanAn") %>' />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtDiaChiQA" runat="server" CssClass="input"
                                Text='<%# Eval("DiaChi") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtSdtQA" runat="server" CssClass="input"
                                Text='<%# Eval("Sdt") %>' />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSaveQA" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaQuanAn") %>' />
                            <asp:Button ID="btnDelQA" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaQuanAn") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm quán ăn mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMaQA" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewTenQA" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewDiaChiQA" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewSdtQA" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddQA" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddQA_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div style="margin-top:18px;">
            <asp:Button ID="btnSave" runat="server" Text="Lưu thông tin chung" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnBack" runat="server" Text="Quay lại" CssClass="btn" OnClick="btnBack_Click" CausesValidation="false" />
        </div>
    </div>
</asp:Content>
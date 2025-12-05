<%@ Page Title="ửa Địa điểm" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AdminPlaceEdit.aspx.cs" Inherits="DANATrip.AdminPlaceEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-place-edit.css" />

    <div class="admin-page container">
        <h1 class="page-title">
            <asp:Literal ID="litTitle" runat="server" />
        </h1>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <!-- 1. THÔNG TIN CHUNG -->
        <div class="form-grid">
            <div class="form-group">
                <label>Mã địa điểm</label>
                <asp:TextBox ID="txtMaDiaDiem" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Tên địa điểm</label>
                <asp:TextBox ID="txtTenDiaDiem" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Vị trí</label>
                <asp:TextBox ID="txtViTri" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Hình ảnh chính (upload hoặc URL)</label>
                <asp:Image ID="imgHinhChinh" runat="server" CssClass="place-main-img" />
                <asp:FileUpload ID="fuHinhChinh" runat="server" CssClass="input" />
                <small>Hoặc nhập trực tiếp URL:</small>
                <asp:TextBox ID="txtHinhAnhChinh" runat="server" CssClass="input" />
            </div>
            <div class="form-group full">
                <label>Nội dung giới thiệu</label>
                <asp:TextBox ID="txtNoiDung" runat="server" CssClass="input" TextMode="MultiLine" Rows="6" />
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
        <h2 class="section-subtitle">Album hình ảnh</h2>
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

        <!-- 3. ĐIỂM THAM QUAN CHÍNH -->
        <h2 class="section-subtitle">Các điểm tham quan chính</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptThamQuan" runat="server" OnItemCommand="rptThamQuan_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã điểm</div>
                            <div class="info-col title">Tên điểm</div>
                            <div class="info-col desc">Mô tả</div>
                            <div class="info-col img">Hình ảnh</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMaDiem" runat="server" CssClass="input input-small"
                                Text='<%# Eval("MaDiem") %>' Enabled="false" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtTenDiem" runat="server" CssClass="input"
                                Text='<%# Eval("TenDiem") %>' />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtMoTa" runat="server" CssClass="input"
                                Text='<%# Eval("MoTa") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col img">
                            <img src="<%# Eval("HinhAnh") %>" class="album-thumb" />
                            <asp:TextBox ID="txtHinhAnh" runat="server" CssClass="input input-url"
                                Text='<%# Eval("HinhAnh") %>' />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSaveThamQuan" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaDiem") %>' />
                            <asp:Button ID="btnDelThamQuan" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaDiem") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm điểm tham quan mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMaDiem" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewTenDiem" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewMoTa" runat="server" CssClass="input" TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col img">
                        <asp:FileUpload ID="fuNewHinhDiem" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddDiem" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddDiem_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 4. THÔNG TIN HỮU ÍCH -->
        <h2 class="section-subtitle">Thông tin hữu ích</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptThongTin" runat="server" OnItemCommand="rptThongTin_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã Info</div>
                            <div class="info-col title">Tiêu đề</div>
                            <div class="info-col desc">Nội dung</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMaInfo" runat="server" CssClass="input input-small"
                                Text='<%# Eval("MaInfo") %>' Enabled="false" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtTieuDe" runat="server" CssClass="input"
                                Text='<%# Eval("TieuDe") %>' />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtNoiDungInfo" runat="server" CssClass="input"
                                Text='<%# Eval("NoiDung") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSaveInfo" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaInfo") %>' />
                            <asp:Button ID="btnDelInfo" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaInfo") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm thông tin mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMaInfo" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewTieuDe" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewNoiDungInfo" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddInfo" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddInfo_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 5. 360 VIEW -->
        <h2 class="section-subtitle">Các góc nhìn 360°</h2>
        <div class="panel-box">
            <asp:Repeater ID="rpt360" runat="server" OnItemCommand="rpt360_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã 360</div>
                            <div class="info-col title">Tiêu đề</div>
                            <div class="info-col desc">Link 360 (Google Maps embed)</div>
                            <div class="info-col img">Thumbnail</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMa360" runat="server" CssClass="input input-small"
                                Text='<%# Eval("Ma360") %>' Enabled="false" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtTieuDe360" runat="server" CssClass="input"
                                Text='<%# Eval("TieuDe") %>' />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtLink360" runat="server" CssClass="input"
                                Text='<%# Eval("Link360") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col img">
                            <img src="<%# Eval("Thumbnail") %>" class="album-thumb" />
                            <asp:TextBox ID="txtThumb" runat="server" CssClass="input input-url"
                                Text='<%# Eval("Thumbnail") %>' />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSave360" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("Ma360") %>' />
                            <asp:Button ID="btnDel360" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("Ma360") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm góc 360 mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMa360" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewTieuDe360" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewLink360" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col img">
                        <asp:FileUpload ID="fuNewThumb360" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAdd360" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAdd360_Click" />
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
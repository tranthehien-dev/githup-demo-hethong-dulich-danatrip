<%@ Page Title="Quản lý Tour" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="AdminTourEdit.aspx.cs" Inherits="DANATrip.AdminTourEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="Styles/admin-place-edit.css" />

    <div class="admin-page container">
        <h1 class="page-title">
            <asp:Literal ID="litTitle" runat="server" />
        </h1>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" />

        <!-- 1. THÔNG TIN CHUNG TOUR -->
        <div class="form-grid">
            <div class="form-group">
                <label>Mã tour</label>
                <asp:TextBox ID="txtMaTour" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Mã địa điểm</label>
                <asp:TextBox ID="txtMaDiaDiem" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Tên tour</label>
                <asp:TextBox ID="txtTenTour" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Thời lượng</label>
                <asp:TextBox ID="txtThoiLuong" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Phương tiện</label>
                <asp:TextBox ID="txtPhuongTien" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Ngày khởi hành</label>
                <asp:TextBox ID="txtNgayKhoiHanh" runat="server" CssClass="input" placeholder="dd/MM/yyyy HH:mm" />
            </div>
            <div class="form-group">
                <label>Giá người lớn</label>
                <asp:TextBox ID="txtGiaNguoiLon" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Giá trẻ em</label>
                <asp:TextBox ID="txtGiaTreEm" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Số chỗ</label>
                <asp:TextBox ID="txtSoCho" runat="server" CssClass="input" />
            </div>
            <div class="form-group">
                <label>Số chỗ đã đặt</label>
                <asp:TextBox ID="txtSoChoDaDat" runat="server" CssClass="input" />
            </div>
            <div class="form-group full">
                <label>Mô tả ngắn</label>
                <asp:TextBox ID="txtMoTaNgan" runat="server" CssClass="input" TextMode="MultiLine" Rows="3" />
            </div>
            <div class="form-group full">
                <label>Mô tả chi tiết</label>
                <asp:TextBox ID="txtMoTaChiTiet" runat="server" CssClass="input" TextMode="MultiLine" Rows="5" />
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

        <!-- 2. ALBUM HÌNH ẢNH TOUR -->
        <h2 class="section-subtitle">Album hình ảnh tour</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptImages" runat="server" OnItemCommand="rptImages_ItemCommand">
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
                <asp:FileUpload ID="fuTourImg" runat="server" CssClass="input" />
                <asp:Button ID="btnAddTourImg" runat="server" Text="Thêm ảnh"
                    CssClass="btn btn-primary btn-small" OnClick="btnAddTourImg_Click" />
            </div>
        </div>

        <!-- 3. ĐIỂM NỔI BẬT -->
        <h2 class="section-subtitle">Điểm nổi bật</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptHighlights" runat="server" OnItemCommand="rptHighlights_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã</div>
                            <div class="info-col title">Tiêu đề</div>
                            <div class="info-col desc">Mô tả</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMaHighlight" runat="server" CssClass="input input-small"
                                Text='<%# Eval("MaHighlight") %>' Enabled="false" />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtTieuDe" runat="server" CssClass="input"
                                Text='<%# Eval("TieuDe") %>' />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtMoTa" runat="server" CssClass="input"
                                Text='<%# Eval("MoTa") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSaveHL" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaHighlight") %>' />
                            <asp:Button ID="btnDelHL" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaHighlight") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm điểm nổi bật mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMaHighlight" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewTieuDeHL" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewMoTaHL" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddHL" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddHL_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 4. LỊCH TRÌNH -->
        <h2 class="section-subtitle">Lịch trình</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptSchedule" runat="server" OnItemCommand="rptSchedule_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Thứ tự</div>
                            <div class="info-col title">Thời gian</div>
                            <div class="info-col title">Tiêu đề</div>
                            <div class="info-col desc">Mô tả</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtThuTu" runat="server" CssClass="input input-small"
                                Text='<%# Eval("ThuTu") %>' />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtThoiGianLT" runat="server" CssClass="input"
                                Text='<%# Eval("ThoiGian") %>' />
                        </div>
                        <div class="info-col title">
                            <asp:TextBox ID="txtTieuDeLT" runat="server" CssClass="input"
                                Text='<%# Eval("TieuDe") %>' />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtMoTaLT" runat="server" CssClass="input"
                                Text='<%# Eval("MoTa") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col act">
                            <asp:HiddenField ID="hfMaSchedule" runat="server" Value='<%# Eval("MaSchedule") %>' />
                            <asp:Button ID="btnSaveSch" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaSchedule") %>' />
                            <asp:Button ID="btnDelSch" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaSchedule") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm lịch trình mới</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewThuTu" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewThoiGianLT" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col title">
                        <asp:TextBox ID="txtNewTieuDeLT" runat="server" CssClass="input" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewMoTaLT" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddSchedule" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddSchedule_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 5. DỊCH VỤ BAO GỒM -->
        <h2 class="section-subtitle">Dịch vụ bao gồm</h2>
        <div class="panel-box">
            <asp:Repeater ID="rptIncludes" runat="server" OnItemCommand="rptIncludes_ItemCommand">
                <HeaderTemplate>
                    <div class="info-grid">
                        <div class="info-row info-header">
                            <div class="info-col code">Mã</div>
                            <div class="info-col desc">Nội dung</div>
                            <div class="info-col act">Hành động</div>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="info-row">
                        <div class="info-col code">
                            <asp:TextBox ID="txtMaInclude" runat="server" CssClass="input input-small"
                                Text='<%# Eval("MaInclude") %>' Enabled="false" />
                        </div>
                        <div class="info-col desc">
                            <asp:TextBox ID="txtNoiDungInc" runat="server" CssClass="input"
                                Text='<%# Eval("NoiDung") %>' TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="info-col act">
                            <asp:Button ID="btnSaveInc" runat="server" Text="Lưu"
                                CssClass="btn btn-small"
                                CommandName="Save"
                                CommandArgument='<%# Eval("MaInclude") %>' />
                            <asp:Button ID="btnDelInc" runat="server" Text="Xóa"
                                CssClass="btn btn-small btn-danger"
                                CommandName="Del"
                                CommandArgument='<%# Eval("MaInclude") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="album-add">
                <h4>Thêm dịch vụ bao gồm</h4>
                <div class="info-row">
                    <div class="info-col code">
                        <asp:TextBox ID="txtNewMaInclude" runat="server" CssClass="input input-small" />
                    </div>
                    <div class="info-col desc">
                        <asp:TextBox ID="txtNewNoiDungInc" runat="server" CssClass="input"
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="info-col act">
                        <asp:Button ID="btnAddInclude" runat="server" Text="Thêm"
                            CssClass="btn btn-primary btn-small" OnClick="btnAddInclude_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- 6. TAG TOUR -->
        <h2 class="section-subtitle">Thẻ (Tags) của tour</h2>
        <div class="panel-box">
            <p class="muted" style="margin-bottom:8px;">
                Chọn các thẻ mô tả tour (Khám phá, Biển, Văn hoá, 1 ngày...).
            </p>

            <asp:CheckBoxList ID="cblTags" runat="server" RepeatDirection="Horizontal"
                              RepeatLayout="Flow" CssClass="tag-list">
            </asp:CheckBoxList>

            <div style="margin-top:10px;">
                <asp:Button ID="btnSaveTags" runat="server" Text="Lưu Tags"
                            CssClass="btn btn-primary btn-small"
                            OnClick="btnSaveTags_Click" />
            </div>
        </div>

        <div style="margin-top:18px;">
            <asp:Button ID="btnSave" runat="server" Text="Lưu thông tin chung" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnBack" runat="server" Text="Quay lại" CssClass="btn" OnClick="btnBack_Click" CausesValidation="false" />
        </div>
    </div>
</asp:Content>
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;

namespace DANATrip
{
    public partial class ChatAI : Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        string ApiKey => ConfigurationManager.AppSettings["GeminiApiKey"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EnsureCurrentChatId();
                LoadChat();
            }
        }

        string CurrentChatId
        {
            get => (string)(Session["CurrentChatId"] ?? "");
            set => Session["CurrentChatId"] = value;
        }

        string CurrentUserId => Session["MaNguoiDung"] as string ?? "GUEST";

        void EnsureCurrentChatId()
        {
            if (string.IsNullOrEmpty(CurrentChatId))
            {
                CurrentChatId = "CH" + Guid.NewGuid().ToString("N").Substring(0, 10);
            }
        }

        void LoadChat()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            SELECT TinNhanNguoiDung, PhanHoiAI, ThoiGian,
                   MaTourGoiY, MaDiaDiemGoiY, MaMonGoiY
            FROM ChatHistory
            WHERE MaChat = @Chat
            ORDER BY Id";
                cmd.Parameters.AddWithValue("@Chat", CurrentChatId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            // nếu bot đang gõ thì thêm 1 dòng giả để Repeater render typing bubble
            if (BotIsTyping)
            {
                DataRow typingRow = dt.NewRow();
                typingRow["TinNhanNguoiDung"] = DBNull.Value;
                typingRow["PhanHoiAI"] = "__BOT_TYPING__"; // flag đặc biệt
                typingRow["ThoiGian"] = DateTime.Now;
                dt.Rows.Add(typingRow);
            }

            rptChat.DataSource = dt;
            rptChat.DataBind();
        }

        // Render HTML bubble
        // Render HTML bubble
        protected string GetBubble(object dataItem)
        {
            var row = (System.Data.DataRowView)dataItem;
            string userMsg = row["TinNhanNguoiDung"]?.ToString();
            string aiMsg = row["PhanHoiAI"]?.ToString();
            string tourIds = row["MaTourGoiY"]?.ToString();
            string placeIds = row["MaDiaDiemGoiY"]?.ToString();
            string foodIds = row["MaMonGoiY"]?.ToString();
            DateTime time = row["ThoiGian"] == DBNull.Value
                ? DateTime.Now
                : Convert.ToDateTime(row["ThoiGian"]);

            string html = "";

            // 0. Nếu là dòng "bot đang gõ" thì render typing bubble rồi return luôn
            if (aiMsg == "__BOT_TYPING__")
            {
                html += @"
<div class='bot-msg-row bot'>
  <div class='bot-msg-bubble bot'>
    <div class='bot-author'>Danang Discovery Bot</div>
    <div class='bot-typing'>
      <span class='dot-typing'></span>
      <span class='dot-typing'></span>
      <span class='dot-typing'></span>
    </div>
  </div>
</div>";
                return html;
            }

            // 1. Bong bóng USER
            if (!string.IsNullOrEmpty(userMsg))
            {
                html += $@"
<div class='bot-msg-row user'>
  <div class='bot-msg-bubble user'>
    <div class='bot-author'>You</div>
    {Server.HtmlEncode(userMsg)}
    <div class='bot-msg-meta'>{time:HH:mm}</div>
  </div>
</div>";
            }

            // 2. Bong bóng BOT (dùng Markdown basic)
            if (!string.IsNullOrEmpty(aiMsg))
            {
                string renderedAi = RenderMarkdownBasic(aiMsg);

                html += $@"
<div class='bot-msg-row bot'>
  <div class='bot-msg-bubble bot'>
    <div  class='bot-author'>Danang Discovery Bot</div>
    {renderedAi}
    <div class='bot-msg-meta'>{time:HH:mm}</div>
  </div>
</div>";
            }

            // 3. Card gợi ý...
            string cards = "";
            if (!string.IsNullOrEmpty(tourIds))
                cards += RenderTourCards(tourIds);
            if (!string.IsNullOrEmpty(placeIds))
                cards += RenderPlaceCards(placeIds);
            if (!string.IsNullOrEmpty(foodIds))
                cards += RenderFoodCards(foodIds);

            if (!string.IsNullOrEmpty(cards))
            {
                html += @"
<div class='bot-msg-row bot'>
  <div class='bot-msg-bubble bot' style='background:#f1f5f9'>
    " + cards + @"
  </div>
</div>";
            }

            return html;
        }

        // --- Events ---

        protected void btnSend_Click(object sender, EventArgs e)
        {
            HandleUserMessage(txtMessage.Text.Trim());
        }

        protected void btnSuggestion_Click(object sender, EventArgs e)
        {
            var btn = (System.Web.UI.WebControls.LinkButton)sender;
            HandleUserMessage(btn.CommandArgument);
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            CurrentChatId = "CH" + Guid.NewGuid().ToString("N").Substring(0, 10);
            txtMessage.Text = "";
            LoadChat();
            upChat.Update();
            upInput.Update();
        }

        // --- Core ---

        void HandleUserMessage(string message)
        {
            lblError.Text = "";
            if (string.IsNullOrWhiteSpace(message))
            {
                lblError.Text = "Vui lòng nhập câu hỏi.";
                return;
            }

            EnsureCurrentChatId();

            // 1. Lưu ngay tin nhắn của user vào DB (chưa có trả lời)
            int lastId;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            INSERT INTO ChatHistory 
                (MaChat, MaNguoiDung, TinNhanNguoiDung, PhanHoiAI, ThoiGian)
            VALUES 
                (@Chat, @UserId, @UserMsg, NULL, @Time);
            SELECT SCOPE_IDENTITY();";

                cmd.Parameters.AddWithValue("@Chat", CurrentChatId);
                cmd.Parameters.AddWithValue("@UserId", (object)CurrentUserId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserMsg", (object)message ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Time", DateTime.Now);

                conn.Open();
                lastId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // 2. Bật trạng thái "bot đang gõ" và reload UI
            BotIsTyping = true;
            txtMessage.Text = "";
            LoadChat();
            upChat.Update();
            upInput.Update();

            // 3. Gọi Gemini + DB để tìm tour/địa điểm/ẩm thực
            DataTable tours = FindToursByMessage(message);
            DataTable places = FindPlacesByMessage(message);
            DataTable foods = FindFoodsByMessage(message);

            string contextJson = BuildContextJson(tours, places, foods);

            string aiReply;
            try
            {
                var client = new GeminiClient(ApiKey);
                aiReply = client.Generate(message, contextJson);
            }
            catch (Exception ex)
            {
                aiReply = "Xin lỗi, có lỗi khi kết nối AI: " + ex.Message;
            }

            string maTourList = BuildIdList(tours, "MaTour");
            string maPlaceList = BuildIdList(places, "MaDiaDiem");
            string maFoodList = BuildIdList(foods, "MaMon");

            // 4. Cập nhật lại dòng vừa tạo với nội dung trả lời + gợi ý
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            UPDATE ChatHistory
            SET PhanHoiAI      = @AI,
                MaTourGoiY     = @Tours,
                MaDiaDiemGoiY  = @Places,
                MaMonGoiY      = @Foods
            WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@AI", (object)aiReply ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Tours", (object)maTourList ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Places", (object)maPlaceList ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Foods", (object)maFoodList ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", lastId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // 5. Tắt typing, reload lại chat
            BotIsTyping = false;
            LoadChat();
            upChat.Update();
            upInput.Update();
        }

        // --- DB search helpers ---

        DataTable FindToursByMessage(string message)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrWhiteSpace(message)) return dt;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TOP 5 
                           t.MaTour,
                           t.TenTour,
                           t.MoTaNgan,
                           d.TenDiaDiem,
                           (SELECT TOP 1 UrlAnh FROM TourImages WHERE MaTour = t.MaTour) AS UrlAnh
                    FROM Tour t
                    LEFT JOIN DiaDiem d ON t.MaDiaDiem = d.MaDiaDiem
                    WHERE ISNULL(t.HienThi,1) = 1
                      AND (
                            t.TenTour       LIKE @kw
                         OR t.MoTaNgan      LIKE @kw
                         OR d.TenDiaDiem    LIKE @kw
                      )
                    ORDER BY t.MaTour";
                cmd.Parameters.AddWithValue("@kw", "%" + message + "%");

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        DataTable FindPlacesByMessage(string message)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrWhiteSpace(message)) return dt;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TOP 5 
                           MaDiaDiem,
                           TenDiaDiem,
                           NoiDung,
                           HinhAnhChinh
                    FROM DiaDiem
                    WHERE ISNULL(HienThi,1) = 1
                      AND (TenDiaDiem LIKE @kw OR NoiDung LIKE @kw OR ViTri LIKE @kw)
                    ORDER BY MaDiaDiem";
                cmd.Parameters.AddWithValue("@kw", "%" + message + "%");

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        DataTable FindFoodsByMessage(string message)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrWhiteSpace(message)) return dt;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                // SỬA tên bảng/cột cho đúng với DB của bạn nếu khác
                cmd.CommandText = @"
                    SELECT TOP 5 
                           MaMon,
                           TenMon,
                           MoTa,
                           HinhAnh
                    FROM AmThuc
                    WHERE ISNULL(HienThi,1) = 1
                      AND (TenMon LIKE @kw OR MoTa LIKE @kw)
                    ORDER BY MaMon";
                cmd.Parameters.AddWithValue("@kw", "%" + message + "%");

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // --- Rendering cards ---

        string RenderTourCards(string idList)
        {
            var ids = (idList ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ids.Length == 0) return "";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                string inClause = string.Join(",", ids.Select((id, i) => "@id" + i));
                cmd.CommandText = $@"
                    SELECT MaTour, TenTour, MoTaNgan,
                           (SELECT TOP 1 UrlAnh FROM TourImages WHERE MaTour = t.MaTour) AS UrlAnh
                    FROM Tour t
                    WHERE MaTour IN ({inClause})";

                for (int i = 0; i < ids.Length; i++)
                    cmd.Parameters.AddWithValue("@id" + i, ids[i]);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0) return "";

            var sb = new System.Text.StringBuilder();
            sb.Append("<div class='bot-tour-list'>");
            foreach (DataRow r in dt.Rows)
            {
                string id = r["MaTour"].ToString();
                string ten = r["TenTour"].ToString();
                string mota = r["MoTaNgan"].ToString();
                string img = r["UrlAnh"]?.ToString();

                sb.Append("<div class='bot-tour-card'>");
                if (!string.IsNullOrEmpty(img))
                    sb.AppendFormat("<div class='bot-tour-img'><img src='{0}' alt='tour' /></div>",
                        ResolveUrl("~/" + img.TrimStart('/')));
                sb.Append("<div class='bot-tour-content'>");
                sb.AppendFormat("<div class='bot-tour-title'>{0}</div>", Server.HtmlEncode(ten));
                if (!string.IsNullOrEmpty(mota))
                    sb.AppendFormat("<div class='bot-tour-desc'>{0}</div>",
                        Server.HtmlEncode(mota.Length > 120 ? mota.Substring(0, 120) + "..." : mota));
                sb.AppendFormat("<a class='bot-tour-link' href='{0}'>Xem chi tiết tour</a>",
                    ResolveUrl("~/TourDetail.aspx?id=" + id));
                sb.Append("</div></div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        string RenderPlaceCards(string idList)
        {
            var ids = (idList ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ids.Length == 0) return "";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                string inClause = string.Join(",", ids.Select((id, i) => "@id" + i));
                cmd.CommandText = $@"
                    SELECT MaDiaDiem, TenDiaDiem, NoiDung, HinhAnhChinh
                    FROM DiaDiem
                    WHERE MaDiaDiem IN ({inClause})";

                for (int i = 0; i < ids.Length; i++)
                    cmd.Parameters.AddWithValue("@id" + i, ids[i]);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0) return "";

            var sb = new System.Text.StringBuilder();
            sb.Append("<div class='bot-tour-list'>");
            foreach (DataRow r in dt.Rows)
            {
                string id = r["MaDiaDiem"].ToString();
                string ten = r["TenDiaDiem"].ToString();
                string nd = r["NoiDung"].ToString();
                string img = r["HinhAnhChinh"]?.ToString();

                sb.Append("<div class='bot-tour-card'>");
                if (!string.IsNullOrEmpty(img))
                    sb.AppendFormat("<div class='bot-tour-img'><img src='{0}' alt='place' /></div>",
                        ResolveUrl("~/" + img.TrimStart('/')));
                sb.Append("<div class='bot-tour-content'>");
                sb.AppendFormat("<div class='bot-tour-title'>{0}</div>", Server.HtmlEncode(ten));
                if (!string.IsNullOrEmpty(nd))
                    sb.AppendFormat("<div class='bot-tour-desc'>{0}</div>",
                        Server.HtmlEncode(nd.Length > 120 ? nd.Substring(0, 120) + "..." : nd));
                sb.AppendFormat("<a class='bot-tour-link' href='{0}'>Xem chi tiết địa điểm</a>",
                    ResolveUrl("~/PlaceDetail.aspx?id=" + id)); // sửa nếu tên khác
                sb.Append("</div></div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        string RenderFoodCards(string idList)
        {
            var ids = (idList ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ids.Length == 0) return "";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                string inClause = string.Join(",", ids.Select((id, i) => "@id" + i));
                cmd.CommandText = $@"
                    SELECT MaMon, TenMon, MoTa, HinhAnh
                    FROM AmThuc
                    WHERE MaMon IN ({inClause})";

                for (int i = 0; i < ids.Length; i++)
                    cmd.Parameters.AddWithValue("@id" + i, ids[i]);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0) return "";

            var sb = new System.Text.StringBuilder();
            sb.Append("<div class='bot-tour-list'>");
            foreach (DataRow r in dt.Rows)
            {
                string id = r["MaMon"].ToString();
                string ten = r["TenMon"].ToString();
                string mo = r["MoTa"].ToString();
                string img = r["HinhAnh"]?.ToString();

                sb.Append("<div class='bot-tour-card'>");
                if (!string.IsNullOrEmpty(img))
                    sb.AppendFormat("<div class='bot-tour-img'><img src='{0}' alt='food' /></div>",
                        ResolveUrl("~/" + img.TrimStart('/')));
                sb.Append("<div class='bot-tour-content'>");
                sb.AppendFormat("<div class='bot-tour-title'>{0}</div>", Server.HtmlEncode(ten));
                if (!string.IsNullOrEmpty(mo))
                    sb.AppendFormat("<div class='bot-tour-desc'>{0}</div>",
                        Server.HtmlEncode(mo.Length > 120 ? mo.Substring(0, 120) + "..." : mo));
                // nếu có trang chi tiết món ăn:
                // sb.AppendFormat("<a class='bot-tour-link' href='{0}'>Xem chi tiết món ăn</a>", ResolveUrl("~/FoodDetail.aspx?id=" + id));
                sb.Append("</div></div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        // --- JSON helpers ---

        string EscapeJson(string s)
        {
            return (s ?? "")
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        string BuildIdList(DataTable dt, string colName)
        {
            if (dt == null || dt.Rows.Count == 0) return "";
            return string.Join(",", dt.AsEnumerable().Select(r => r[colName].ToString()));
        }

        string BuildContextJson(DataTable tours, DataTable places, DataTable foods)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("{");

            // tours
            sb.Append("\"tours\":[");
            if (tours != null)
            {
                for (int i = 0; i < tours.Rows.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    DataRow r = tours.Rows[i];
                    sb.Append("{");
                    sb.AppendFormat("\"MaTour\":\"{0}\",", EscapeJson(r["MaTour"].ToString()));
                    sb.AppendFormat("\"TenTour\":\"{0}\",", EscapeJson(r["TenTour"].ToString()));
                    sb.AppendFormat("\"TenDiaDiem\":\"{0}\",", EscapeJson(r["TenDiaDiem"].ToString()));
                    sb.AppendFormat("\"MoTaNgan\":\"{0}\"", EscapeJson(r["MoTaNgan"].ToString()));
                    sb.Append("}");
                }
            }
            sb.Append("],");

            // places
            sb.Append("\"places\":[");
            if (places != null)
            {
                for (int i = 0; i < places.Rows.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    DataRow r = places.Rows[i];
                    sb.Append("{");
                    sb.AppendFormat("\"MaDiaDiem\":\"{0}\",", EscapeJson(r["MaDiaDiem"].ToString()));
                    sb.AppendFormat("\"TenDiaDiem\":\"{0}\",", EscapeJson(r["TenDiaDiem"].ToString()));
                    sb.AppendFormat("\"NoiDung\":\"{0}\"", EscapeJson(r["NoiDung"].ToString()));
                    sb.Append("}");
                }
            }
            sb.Append("],");

            // foods
            sb.Append("\"foods\":[");
            if (foods != null)
            {
                for (int i = 0; i < foods.Rows.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    DataRow r = foods.Rows[i];
                    sb.Append("{");
                    sb.AppendFormat("\"MaMon\":\"{0}\",", EscapeJson(r["MaMon"].ToString()));
                    sb.AppendFormat("\"TenMon\":\"{0}\",", EscapeJson(r["TenMon"].ToString()));
                    sb.AppendFormat("\"MoTa\":\"{0}\"", EscapeJson(r["MoTa"].ToString()));
                    sb.Append("}");
                }
            }
            sb.Append("]");

            sb.Append("}");
            return sb.ToString();
        }
        string RenderMarkdownBasic(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            // B1: encode HTML để tránh XSS
            text = Server.HtmlEncode(text);

            // B2: chuyển **bold** thành <strong>...</strong>
            // pattern: **nội dung**
            var pattern = "\\*\\*(.+?)\\*\\*";
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                pattern,
                "<strong>$1</strong>");

            // B3: xuống dòng
            text = text.Replace("\r\n", "\n").Replace("\n", "<br/>");

            return text;
        }
        bool BotIsTyping
        {
            get => (bool?)(Session["BotIsTyping"] as bool?) ?? false;
            set => Session["BotIsTyping"] = value;
        }
    }
}
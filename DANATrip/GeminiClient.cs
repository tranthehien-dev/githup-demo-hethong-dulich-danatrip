using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DANATrip
{
    public class GeminiClient
    {
        private readonly string _apiKey;
        private static readonly HttpClient httpClient = new HttpClient();

        public GeminiClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public string Generate(string userMessage, string contextJson = null)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return "";

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            string systemPrompt = BuildSystemPrompt();

            if (!string.IsNullOrWhiteSpace(contextJson))
            {
                systemPrompt +=
                    "\n\nDỮ LIỆU DANATRIP (JSON):\n" + contextJson +
                    "\nHãy dùng thông tin này để trả lời bằng tiếng Việt, thân thiện, ưu tiên gợi ý tour/địa điểm/ẩm thực.";
            }

            var payload = new
            {
                contents = new[]
                {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = systemPrompt + "\n\nCâu hỏi của người dùng: " + userMessage }
                    }
                }
            }
            };

            string json = new JavaScriptSerializer().Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Gọi API kiểu đồng bộ
            var resp = httpClient.PostAsync(url, content).Result;
            string respText = resp.Content.ReadAsStringAsync().Result;

            if (!resp.IsSuccessStatusCode)
                return "Xin lỗi, hiện tại tôi không thể trả lời. (Lỗi API).";

            dynamic obj = new JavaScriptSerializer().DeserializeObject(respText);
            try
            {
                var candidates = obj["candidates"];
                if (candidates is Array && ((Array)candidates).Length > 0)
                {
                    var first = candidates[0];
                    var contentObj = first["content"];
                    var parts = contentObj["parts"];
                    if (parts is Array && ((Array)parts).Length > 0)
                    {
                        return parts[0]["text"];
                    }
                }
            }
            catch { }

            return "Xin lỗi, tôi chưa hiểu câu hỏi này.";
        }

        private string BuildSystemPrompt()
        {
            return
@"Bạn là Danang Discovery Bot cho website du lịch DANATRip.
Nhiệm vụ:
- Giải đáp câu hỏi về du lịch Đà Nẵng (địa điểm, tour, ẩm thực, trải nghiệm).
- Trả lời thân thiện, dễ hiểu, tiếng Việt, ưu tiên gọn nhưng đầy đủ ý.
- Số điện thoại liên hệ là 0932557128 và địa chỉ công ty là ở 125 Võ Như Hưng.
- Khi có danh sách tour/địa điểm/món ăn trong dữ liệu JSON, hãy dựa vào đó để gợi ý cụ thể tên tour, địa điểm, món ăn.";
        }
    }
}
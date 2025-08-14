using AI.CryptoAdvisor.Api.Data;
using AI.CryptoAdvisor.Api.Models;
using System.Text.Json;

namespace AI.CryptoAdvisor.Api.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        public ApiService(HttpClient httpClient, AppDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<string> FetchContent(
    string contentType,
    Func<HttpClient, Task<string>> fetchApiFunc,
    double cacheMinutes,
    string fallback
)
        {
            var cache = _dbContext.ContentCaches.FirstOrDefault(c => c.ContentType == contentType);
            if (cache != null && (DateTime.UtcNow - cache.FetchedAt).TotalMinutes < cacheMinutes)
                return cache.Data;

            try
            {
                var response = await fetchApiFunc(_httpClient);

                // בדיקה אם זה JSON
                try
                {
                    using var doc = JsonDocument.Parse(response);
                    // אם Parse מצליח => זה JSON, מחזירים כמו שהוא
                }
                catch
                {
                    // אם Parse נכשל => עטוף אותו במבנה JSON
                    if (contentType == "news" || contentType == "coins" || contentType == "memes")
                    {
                        // לדוגמה, חדשות / מטבעות / memes => מערך עם הודעה אחת
                        response = JsonSerializer.Serialize(new[] { response });
                    }
                    else
                    {
                        // לכל סוג אחר => מחזיר סטרינג רגיל
                        response = JsonSerializer.Serialize(response);
                    }
                }

                // שמירת Cache
                if (cache == null)
                {
                    cache = new ContentCache { ContentType = contentType };
                    _dbContext.ContentCaches.Add(cache);
                }
                cache.Data = response;
                cache.FetchedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return response;
            }
            catch
            {
                // במקרה של שגיאה => עטוף את fallback גם כן במבנה JSON
                return JsonSerializer.Serialize(new[] { fallback });
            }
        }

    }
}

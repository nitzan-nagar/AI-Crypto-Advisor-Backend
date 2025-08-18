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

                try
                {
                    using var doc = JsonDocument.Parse(response);
                }
                catch
                {
                    
                    return JsonSerializer.Serialize(new { message = fallback });
                }

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
                return JsonSerializer.Serialize(new { message = fallback });
            }
        }

    }
}

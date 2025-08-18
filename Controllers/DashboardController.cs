using AI.CryptoAdvisor.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AI.CryptoAdvisor.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _configuration;
        public DashboardController(ApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {


            var insightData = await _apiService.FetchContent(
                "insights",
                async (client) =>
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration["ApiKeys:OpenRouter"]);

                    var openAiUrl = "https://openrouter.ai/api/v1/chat/completions";

                    var requestBody = new
                    {
                        model = "openai/gpt-oss-20b:free",
                        messages = new[]
                        {
                            new { role = "user", content = "Generate a single line insight about cryptocurrency investment." }
                        }
                    };

                    var response = await client.PostAsJsonAsync(openAiUrl, requestBody);
                    Console.WriteLine(response);
                    response.EnsureSuccessStatusCode();

                    var resultJson = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(resultJson);

                    var insightText = jsonDoc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    return insightText;
                },
                cacheMinutes: 1440,
                fallback: "No AI insights available"
            );

            var memesData = await _apiService.FetchContent(
                "memes",
                async (client) =>
                {
                    var url = "https://www.reddit.com/r/cryptomemes/.json?limit=100&t=day";
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("CryptoDashboard/0.1");
                    return await client.GetStringAsync(url);
                },
                cacheMinutes: 5,
                fallback: "No AI memes available"
            );

            var newsData = await _apiService.FetchContent(
                "news",
                async (client) => await client.GetStringAsync($"https://cryptopanic.com/api/developer/v2/posts/?auth_token={_configuration["ApiKeys:cryptopanic"]}&filter=hot"),
                cacheMinutes: 10,
                fallback: "No news available at the moment. Please check back later."
            );

            var coinsData = await _apiService.FetchContent(
                "coins",
                async (client) =>
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("CryptoDashboard/1.0");
                    return await client.GetStringAsync($"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=10&page=1");
                },
                cacheMinutes: 5,
                fallback: "No coin data available at the moment. Please check back later."
            );

            return Ok(new
            {

                news = newsData,
                coins = coinsData,
                insight = insightData,
                memes = memesData
            });
        }
    }
}

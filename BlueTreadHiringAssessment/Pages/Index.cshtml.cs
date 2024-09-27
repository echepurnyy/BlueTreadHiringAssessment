using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using BlueTreadHiringAssessment.Classes;
namespace BlueTreadHiringAssessment.Pages
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        public string SearchQuery { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _config = configuration;
        }

        public void OnGet()
        {

        }
        public async Task OnPost(string SearchQuery) {
            var eventsRet = new List<Event>();

            var httpClient = _httpClientFactory.CreateClient();
            var req = new HttpRequestMessage(
            HttpMethod.Get,
            "https://app.ticketmaster.com/discovery/v2/events.json?keyword="+
            SearchQuery + "&countryCode=US&apikey=" + _config["TMApiKey"]);
            var httpResponseMessage = await httpClient.SendAsync(req);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                var x = await JsonSerializer.DeserializeAsync
                    <JsonDocument>(contentStream);
                JsonElement links = x.RootElement.GetProperty("_links");
                JsonElement events = x.RootElement.GetProperty("_embedded").GetProperty("events");
                foreach(JsonElement ev in events.EnumerateArray())
                {
                    eventsRet.Add(ev.Deserialize<Event>());
                }
                
            }
        }
    }
}

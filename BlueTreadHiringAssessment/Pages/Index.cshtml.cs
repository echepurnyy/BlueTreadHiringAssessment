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
        public async Task<IActionResult> OnPostSearch(string SearchQuery) {
            try
            {
                var attrsRet = new List<Attraction>();

                var httpClient = _httpClientFactory.CreateClient();
                var req = new HttpRequestMessage(
                HttpMethod.Get,
                "https://app.ticketmaster.com/discovery/v2/attractions.json?keyword=" +
                SearchQuery + "&preferredCountry=US&apikey=" + _config["TMApiKey"]);
                var httpResponseMessage = await httpClient.SendAsync(req);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var x = await JsonSerializer.DeserializeAsync
                        <JsonDocument>(contentStream);
                    JsonElement attrs = x.RootElement.GetProperty("_embedded").GetProperty("attractions");
                    foreach (JsonElement attr in attrs.EnumerateArray())
                    {
                        attrsRet.Add(attr.Deserialize<Attraction>());
                    }
                    return Partial("~/Pages/Partials/_SearchResults.cshtml", attrsRet);
                }
                return Partial("~/Pages/Partials/_SearchResults.cshtml");
            }
            catch (Exception ex)
            {
                //if no results are retrieved, return an empty view
                return Partial("~/Pages/Partials/_SearchResults.cshtml");
            }
        }

        public async Task<IActionResult> OnPostDetails(int EventId)
        {
            try
            {
                var eventsRet = new Event();

                var httpClient = _httpClientFactory.CreateClient();
                var req = new HttpRequestMessage(
                HttpMethod.Get,
                "https://app.ticketmaster.com/discovery/v2/events.json?size=1&id=" +
                EventId + "&apikey=" + _config["TMApiKey"]);
                var httpResponseMessage = await httpClient.SendAsync(req);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var x = await JsonSerializer.DeserializeAsync
                        <JsonDocument>(contentStream);
                    JsonElement events = x.RootElement.GetProperty("_embedded").GetProperty("events");
                    foreach (JsonElement ev in events.EnumerateArray())
                    {
                        //eventsRet.Add(ev.Deserialize<Event>());
                    }
                    return Partial("~/Pages/Partials/_EventDetails.cshtml", eventsRet);
                }
                return Partial("~/Pages/Partials/_EventDetails.cshtml");
            }
            catch (Exception ex)
            {
                //if no results are retrieved, return an empty view
                return Partial("~/Pages/Partials/_EventDetails.cshtml");
            }
        }
    }
}

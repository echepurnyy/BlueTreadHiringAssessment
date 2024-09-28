using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using BlueTreadHiringAssessment.Classes;
using System.Text.Json.Nodes;
using System.Collections.Generic;
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
                        var temp = attr.Deserialize<Attraction>();
                        if (temp != null)
                        {
                            temp.AttrLinks = new Dictionary<string, string>();
                            try
                            {
                                var extl = attr.GetProperty("externalLinks");
                                var linksList = extl.Deserialize<JsonNode>();
                                if (linksList != null)
                                {
                                    temp.AttrLinks.Add("youtube", linksList.GetLinkByName("youtube"));
                                    temp.AttrLinks.Add("twitter", linksList.GetLinkByName("twitter"));
                                    temp.AttrLinks.Add("spotify", linksList.GetLinkByName("spotify"));
                                    temp.AttrLinks.Add("homepage", linksList.GetLinkByName("homepage"));
                                }
                            } catch {
                                /*if no links are present, add blanks*/
                                temp.AttrLinks.Add("youtube", "");
                                temp.AttrLinks.Add("twitter", "");
                                temp.AttrLinks.Add("spotify", "");
                                temp.AttrLinks.Add("homepage", "");
                            }
                            //uppercase the attraction name, as per requirements
                            temp.Name = temp.Name.ToUpper().CleanSpecialChars();
                            attrsRet.Add(temp);
                        }
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

        

        public async Task<IActionResult> OnPostDetails(string AttrId)
        {
            try
            {
                var eventsRet = new List<Event>();

                var httpClient = _httpClientFactory.CreateClient();
                var req = new HttpRequestMessage(
                HttpMethod.Get,
                "https://app.ticketmaster.com/discovery/v2/events.json?attractionId=" + AttrId + 
                //"&startDateTime="+DateTime.Today.AddDays(-1).ToShortDateString()+
                "&apikey=" + _config["TMApiKey"]);
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
                        var row = ev.Deserialize<Event>();
                        if (row != null)
                        {
                            //get start date for the event
                            var date = ev.GetProperty("dates").GetProperty("start").GetProperty("localDate").GetString();
                            var dateConv = Convert.ToDateTime(date);
                            row.Date = dateConv.ToString("dddd, MMMM d, yyyy").ToUpper();
                            row.Name = row.Name.ToUpper();
                            //get venues, create a row for each
                            var venueList = ev.GetProperty("_embedded").GetProperty("venues").EnumerateArray();
                            foreach (var v in venueList)
                            {
                                var temp = (Event)row.Clone();
                                temp.VenueName = v.GetProperty("name").ToString().ToUpper().CleanSpecialChars();
                                temp.City = v.GetProperty("city").GetProperty("name").ToString().ToUpper();
                                temp.State = v.GetProperty("state").GetProperty("stateCode").ToString();
                                eventsRet.Add(temp);
                            }
                        }
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

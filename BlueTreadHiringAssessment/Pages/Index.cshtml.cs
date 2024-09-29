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

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _config = configuration;
        }

        /*The function that renders the app's landing page. 
         *As the page requires no data, there's no extra processing
         happening here.*/
        public void OnGet()
        {

        }

        /*The handle that receives the search data, sends it to the Ticketmaster API, 
         and returns a partial view with the search results.*/
        public async Task<IActionResult> OnPostSearch(string SearchQuery) {
            try
            {
                var attrsRet = new List<Attraction>();
                //HttpClient instance that consumes the search query and the Ticketmaster API key
                var httpClient = _httpClientFactory.CreateClient();
                var req = new HttpRequestMessage(
                HttpMethod.Get,
                "https://app.ticketmaster.com/discovery/v2/attractions.json?keyword=" +
                SearchQuery + "&preferredCountry=US&apikey=" + _config["TMApiKey"]);
                var httpResponseMessage = await httpClient.SendAsync(req);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    //on successful response, read it via content stream, which is then deserialized into a Json object
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var x = await JsonSerializer.DeserializeAsync
                        <JsonDocument>(contentStream);
                    JsonElement attrs = x.RootElement.GetProperty("_embedded").GetProperty("attractions");
                    foreach (JsonElement attr in attrs.EnumerateArray())
                    {
                        //the Json container which has the list of attractions is enumerated and processed
                        var temp = attr.Deserialize<Attraction>();
                        if (temp != null)
                        {
                            temp.AttrLinks = new Dictionary<string, string>();
                            try
                            {
                                //external links (X, YouTube, etc.) are processed separately,
                                //as they are housed within a list of lists of objects,
                                //or something convoluted and non-serializable like that
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

        /*The handle that receives a Ticketmater ID of a specific attraction,
         retrieves its upcoming events, and returns it as a partial view.*/
        public async Task<IActionResult> OnPostDetails(string AttrId)
        {
            try
            {
                var eventsRet = new List<Event>();
                //HttpClient instance that consumes the attraction ID and the Ticketmaster API key
                var httpClient = _httpClientFactory.CreateClient();
                var req = new HttpRequestMessage(
                HttpMethod.Get,
                "https://app.ticketmaster.com/discovery/v2/events.json?attractionId=" + AttrId + 
                /***Uncomment the below line of code to restrict the results to upcoming events (today and in the future) only.
                Be vary: you're unlikely to receive any results that way, for whatever reason.***/
                //"&startDateTime="+DateTime.Today.AddDays(-1).ToShortDateString()+
                "&apikey=" + _config["TMApiKey"]);
                var httpResponseMessage = await httpClient.SendAsync(req);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    //on successful response, read the data from the stream, and deserialize it into a Json object
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var x = await JsonSerializer.DeserializeAsync
                        <JsonDocument>(contentStream);
                    JsonElement events = x.RootElement.GetProperty("_embedded").GetProperty("events");
                    foreach (JsonElement ev in events.EnumerateArray())
                    {
                        //loop through the container with the event entries, and retrieve
                        //the relevant data (dates, venues) from the respective subcontainers
                        var row = ev.Deserialize<Event>();
                        if (row != null)
                        {
                            //get start date for the event
                            var date = ev.GetProperty("dates").GetProperty("start").GetProperty("localDate").GetString();
                            var dateConv = Convert.ToDateTime(date);
                            //fomat the date, as per requirements
                            row.Date = dateConv.ToString("dddd, MMMM d, yyyy").ToUpper();
                            //uppercase the event name, as per requirements
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

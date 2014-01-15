using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MashupApp.Controllers
{
    using FlickrNet;
    using free.localweather;
    using Google.Apis.Customsearch.v1;
    using Google.Apis.Services;

    public class MainController : Controller
    {
        private CustomsearchService googleService;

        private string googleSearchEngineId;

        private string googleApiKey;

        private Flickr flickr;

        private string rssApi;

        //
        // GET: /Main/

        public MainController()
        {
            this.googleApiKey = "AIzaSyC66KeMa7sfsdwC6mJWcHT4yiWNHzLdzOc";
            this.googleSearchEngineId = "004492732884117081044:th2qqajcg0c";
            this.rssApi = "https://news.google.com/news/feeds?q=";

            this.googleService = new Google.Apis.Customsearch.v1.CustomsearchService(
                new BaseClientService.Initializer { ApiKey = this.googleApiKey });

            this.flickr = new Flickr("1a5ca7d9ee95091bd1249fabd254b5ae", "f7e6199868b71ffa");
        }

        public ActionResult Index()
        {
            return this.View();
        }

        public PartialViewResult GoogleImg(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return this.PartialView("_googleImgPartial", new List<string>());
            }


            var listRequest = this.googleService.Cse.List(text);

            listRequest.Cx = this.googleSearchEngineId;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            listRequest.ImgSize = CseResource.ListRequest.ImgSizeEnum.Medium;
            
            var search = listRequest.Execute();

            var results = search.Items.Select(result => result.Link).ToList();
            return this.PartialView("_googleImgPartial", results);
        }

        public PartialViewResult FlickrImg(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return this.PartialView("_flickrPartial", new List<string>());
            }

            var options = new PhotoSearchOptions { Text = text, PerPage = 10, TagMode = TagMode.AllTags };
            var photos = this.flickr.PhotosSearch(options);

            return this.PartialView("_flickrPartial", photos.Select(photo => photo.Small320Url).ToList());
        }

        public PartialViewResult Weather(string text)
        {

            if (string.IsNullOrWhiteSpace(text))
            {
                return this.PartialView("_weatherPartial", new Current_Condition());
            }


            var input = new LocalWeatherInput();
            input.query = text;
            input.num_of_days = "2";
            input.format = "JSON";

            var api = new FreeAPI();
            var localWeather = api.GetLocalWeather(input);

            Current_Condition weatherModel = null;
            if (localWeather.data.current_Condition != null)
            {
                weatherModel = localWeather.data.current_Condition.FirstOrDefault();
            }

            return this.PartialView("_weatherPartial", weatherModel ?? new Current_Condition());
        }

        public PartialViewResult Rss(string text)
        {

            if (string.IsNullOrWhiteSpace(text))
            {
                return this.PartialView("_rssPartial");
            }

            return this.PartialView("_rssPartial");
        }

    }
}

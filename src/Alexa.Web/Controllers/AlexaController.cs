using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Alexa.Web.Controllers
{
    [Route("api/feed/[controller]")]
    public class AlexaController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var feed = new AlexaFeed();

            using (var xmlReader = XmlReader.Create("http://corsi.unica.it/informatica/feed/"))
            {
                var reader = SyndicationFeed.Load(xmlReader);

                foreach (var originalItem in reader.Items)
                {
                    var copy = new AlexaFeedItem
                    {
                        redirectionUrl = originalItem.Links[0].Uri.ToString(),
                        titleText = originalItem.Title.Text,
                        mainText = originalItem.Title.Text,
                        updatedDate = originalItem.PublishDate.DateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'.0Z'"),
                        uid = Guid.NewGuid().ToString()
                    };

                    feed.Items.Add(copy);
                }
            }

            return Json(feed.Items);
        }
    }

    public class AlexaFeed
    {
        public AlexaFeed()
        {
            Items = new List<AlexaFeedItem>();
        }

        public IList<AlexaFeedItem> Items { get; set; }
    }

    public class AlexaFeedItem
    {
        public string uid { get; set; }
        public string updatedDate { get; set; }
        public string titleText { get; set; }
        public string mainText { get; set; }
        public string redirectionUrl { get; set; }
    }
}

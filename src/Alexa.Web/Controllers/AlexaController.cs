using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    var strippedText = AlexaFeed.Strip(originalItem.Title.Text);
                    var copy = new AlexaFeedItem
                    {
                        redirectionUrl = originalItem.Links[0].Uri.ToString(),
                        titleText = strippedText,
                        mainText = strippedText,
                        updateDate = originalItem.PublishDate.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.0Z"),
                        uid = Guid.NewGuid().ToString()
                    };

                    feed.Items.Add(copy);
                }
            }

            return Json(feed.Items.OrderByDescending(y => y.updateDate));
        }
    }

    public class AlexaFeed
    {
        public AlexaFeed()
        {
            Items = new List<AlexaFeedItem>();
        }

        public static string Strip(string text)
        {
            var ouputString = text;

            ouputString = ouputString.Replace('(', ' ');
            ouputString = ouputString.Replace(')', ' ');
            ouputString = ouputString.Replace('/', ' ');
            ouputString = ouputString.Replace('\\', ' ');
            ouputString = ouputString.Replace('@', ' ');
            ouputString = ouputString.Replace('.', ' ');
            ouputString = ouputString.Replace(',', ' ');
            ouputString = ouputString.Replace('[', ' ');
            ouputString = ouputString.Replace(']', ' ');

            return ouputString;
        }

        public IList<AlexaFeedItem> Items { get; set; }
    }

    public class AlexaFeedItem
    {
        public string uid { get; set; }
        public string updateDate { get; set; }
        public string titleText { get; set; }
        public string mainText { get; set; }
        public string redirectionUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WikipediaShowCrawler
{
    public class EpisodeTitleExtractor
    {
        private HtmlNode EpisodeRow { get; }
        public EpisodeTitleExtractor(HtmlNode episodeRow)
        {
            EpisodeRow = episodeRow;
        }

        public string ExtractEpisodeTitle()
        {
            var tds = EpisodeRow.Descendants("td");
            var classTds = tds.Where(td => td.Attributes.Contains("class"));
            var summaryTd = classTds.FirstOrDefault(td => td.Attributes["class"].Value.Equals("summary"));
            var title = summaryTd?.InnerText.Trim(new[] { '"' });

            if (string.IsNullOrWhiteSpace(title))
            {
                title = EpisodeRow.Descendants("td").Skip(1).FirstOrDefault()?.InnerText.Trim(new[] { '"' });
            }
            return title.Replace("&amp;", "&");
        }
    }
}

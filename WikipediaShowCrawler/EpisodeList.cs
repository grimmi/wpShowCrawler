using System.Collections.Generic;

namespace WikipediaShowCrawler
{
    public class EpisodeList
    {
        public string ShowName { get; set; }
        public IEnumerable<Season> Seasons { get; set; }

        public EpisodeList()
        {

        }
    }
}
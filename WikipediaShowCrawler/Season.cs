using System.Collections.Generic;

namespace WikipediaShowCrawler
{
    public class Season
    {
        public int Number { get; set; }
        public IEnumerable<Episode> Episodes { get; set; }
    }
}
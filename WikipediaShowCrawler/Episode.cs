using System;

namespace WikipediaShowCrawler
{
    public class Episode
    {
        public Season Season { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public DateTime FirstAired { get; set; }
    }
}
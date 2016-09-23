using System.Net.Http;
using System.Threading.Tasks;

namespace WikipediaShowCrawler
{
    public class EpisodeListDownloader
    {
        private string showName;
        private string enWpPrefix = "https://en.wikipedia.org/api/rest_v1/page/html/";

        public EpisodeListDownloader(string showName)
        {
            this.showName = showName;
        }

        public async Task<EpisodeList> DownloadEpisodeList()
        {
            using (var client = new HttpClient())
            {
                var response =
                    await client.GetStringAsync(enWpPrefix + $"List_of_{showName.Replace(" ", "_")}_episodes");

                var parser = new HtmlListParser(showName, response);

                var list = parser.ParseResponse();
                return list;
            }
        }
    }
}
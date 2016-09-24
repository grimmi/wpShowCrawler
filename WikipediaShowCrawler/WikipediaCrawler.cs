using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TvShowManager;
using TvShowManager.Interfaces;

namespace WikipediaShowCrawler
{
    public class WikipediaCrawler : IEpisodeCrawler
    {
        private string showName;
        private string enWpPrefix = "https://en.wikipedia.org/api/rest_v1/page/html/";
        
        public async Task<EpisodeList> DownloadEpisodeListAsync(string showName)
        {
            this.showName = showName;
            using (var client = new HttpClient())
            {
                var response =
                    await client.GetStringAsync(enWpPrefix + $"List_of_{showName.Replace(" ", "_")}_episodes");

                var parser = new HtmlListParser(showName, response);

                var list = parser.ParseResponse();

                list = await CheckSpecialCases(list);

                return list;
            }
        }

        private async Task<EpisodeList> CheckSpecialCases(EpisodeList defaultList)
        {
            if (showName.Equals("The Simpsons"))
            {
                using (var client = new HttpClient())
                {
                    var addResponse =
                        await client.GetStringAsync(enWpPrefix + $"List_of_The_Simpsons_episodes_(seasons_1-20)");
                    var addParser = new HtmlListParser(showName, addResponse);
                    var addList = addParser.ParseResponse();

                    defaultList.Seasons = defaultList.Seasons.Select(s => new Season { Episodes = s.Episodes, Number = s.Number + 20 })
                    .Union(addList.Seasons).OrderBy(s => s.Number);
                }
            }
            return defaultList;
        }
    }
}
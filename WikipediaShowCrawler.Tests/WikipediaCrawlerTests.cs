using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WikipediaShowCrawler.Tests
{
    [TestFixture]
    public class WikipediaCrawlerTests
    {
        [Test]
        [TestCase("Modern Family", 3, 24, 8)]
        [TestCase("The Simpsons", 3, 24, 28)]
        [TestCase("Bob's Burgers", 6, 19, 7)]
        public async Task DownloadPage_ExistingWikipediaPageTitle_SuccessfullyDownloadPage(string showName,
            int seasonToCheck, int expectedEpisodes, int totalNumberOfSeasons)
        {
            var wpCrawler = new WikipediaCrawler();

            var episodeList = await wpCrawler.DownloadEpisodeListAsync(showName);

            Assert.False(episodeList.Seasons.SelectMany(s => s.Episodes).Any(e => e.FirstAired.Equals(new DateTime(1970, 1, 1))));
            Assert.False(episodeList.Seasons.SelectMany(s => s.Episodes).Any(e => string.IsNullOrWhiteSpace(e.Name)));
            Assert.AreEqual(totalNumberOfSeasons, episodeList.Seasons.Count());
            Assert.AreEqual(showName, episodeList.ShowName);
            Assert.AreEqual(expectedEpisodes, episodeList.Seasons.First(s => s.Number == seasonToCheck).Episodes.Count());
        }
    }
}

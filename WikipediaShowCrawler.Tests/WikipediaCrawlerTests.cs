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

        [Test]
        public async Task DownloadEpisodeListAsync_TheSimpsons_CorrectlyNumberSeasons()
        {
            var wpCrawler = new WikipediaCrawler();
            var episodeList = await wpCrawler.DownloadEpisodeListAsync("The Simpsons");

            var s10e01 = episodeList.Seasons.First(s => s.Number == 10).Episodes.First(e => e.Number == 1);
            Assert.AreEqual("Lard of the Dance", s10e01.Name);
            Assert.AreEqual(10, s10e01.Season.Number);

            var s21e01 = episodeList.Seasons.First(s => s.Number == 21).Episodes.First(e => e.Number == 1);
            Assert.AreEqual(21, s21e01.Season.Number);
            Assert.AreEqual("Homer the Whopper", s21e01.Name);
        }

        [Test]
        public async Task DownloadEpisodeListAsync_EpisodeListWithDescriptions_CorrectlyLoadList()
        {
            var wpCrawler = new WikipediaCrawler();
            var episodeList = await wpCrawler.DownloadEpisodeListAsync("Brooklyn Nine-Nine");

            var s01e01 = episodeList.Seasons.First().Episodes.First();
            var s01e10 = episodeList.Seasons.First().Episodes.First(e => e.Number == 10);
            Assert.AreEqual("Pilot", s01e01.Name);
            Assert.AreEqual("Thanksgiving", s01e10.Name);
            Assert.AreEqual(22, episodeList.Seasons.First().Episodes.Count());
            Assert.AreEqual(23, episodeList.Seasons.Skip(1).First().Episodes.Count());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WikipediaShowCrawler.Tests
{
    [TestFixture]
    public class EpisodeListDownloaderTests
    {
        [Test]
        [TestCase("Modern Family", 2, 24)]
        [TestCase("The Simpsons", 2, 22)]
        public async Task DownloadPage_ExistingWikipediaPageTitle_SuccessfullyDownloadPage(string showName,
            int expectedSeasons, int expectedEpisodes)
        {
            var epListDownloader = new EpisodeListDownloader(showName);

            var episodeList = await epListDownloader.DownloadEpisodeList();

            Assert.AreEqual(showName, episodeList.ShowName);
            Assert.AreEqual(expectedEpisodes, episodeList.Seasons.ElementAt(expectedSeasons).Episodes.Count());
        }
    }
}

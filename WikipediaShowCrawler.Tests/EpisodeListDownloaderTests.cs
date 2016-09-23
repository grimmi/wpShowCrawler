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
        public async Task DownloadPage_ExistingWikipediaPageTitle_SuccessfullyDownloadPage()
        {
            var epListDownloader = new EpisodeListDownloader("Modern Family");

            var episodeList = await epListDownloader.DownloadEpisodeList();

            Assert.AreEqual("Modern Family", episodeList.ShowName);
        }
    }
}

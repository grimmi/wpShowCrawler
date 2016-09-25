using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;
using TvShowManager;

namespace WikipediaShowCrawler
{
    internal class HtmlListParser
    {
        private readonly string showName;
        private HtmlDocument htmlList;
        public HtmlListParser(string showName, string response)
        {
            this.showName = showName;

            htmlList = new HtmlDocument();
            htmlList.LoadHtml(response);
        }

        public EpisodeList ParseResponse()
        {
            var seasons = ParseSeasons();
            return new EpisodeList { ShowName = showName, Seasons = seasons };
        }

        private IEnumerable<Season> ParseSeasons()
        {
            var seasonTables = GetSeasonTables(htmlList.DocumentNode);
            var seasonCounter = 1;
            foreach (var seasonTable in seasonTables)
            {
                Season season = null;
                try
                {
                    season = ParseSeason(seasonTable, seasonCounter++);
                    season.ShowName = showName;
                }
                catch (FormatException ex)
                {
                    seasonCounter--;
                    continue;
                }
                yield return season;
            }
        }

        private IEnumerable<HtmlNode> GetSeasonTables(HtmlNode document)
        {
            var seasonTables = document.Descendants("table")
                .Where(t => t.Attributes.Contains("class") && t.Attributes["class"].Value.Equals("wikitable plainrowheaders wikiepisodetable"));

            return seasonTables;
        }

        private Season ParseSeason(HtmlNode seasonTable, int seasonCounter)
        {
            var season = new Season { Number = seasonCounter };
            var episodes = new List<Episode>();
            var episodeRows = GetEpisodeRows(seasonTable);
            foreach (var episodeRow in episodeRows)
            {
                var episode = ParseEpisode(episodeRow);
                if (string.IsNullOrWhiteSpace(episode.Name)
                    || episode.Name.Equals("TBA")
                    || episode.FirstAired == default(DateTime)
                    || episode.FirstAired.Equals(new DateTime(1970, 1, 1)))
                {
                    continue;
                }
                episode.Season = season;
                episodes.Add(episode);
            }
            season.Episodes = episodes;
            return season;
        }

        private IEnumerable<HtmlNode> GetEpisodeRows(HtmlNode seasonTable)
        {
            var episodeRows = seasonTable.Descendants("tr").Skip(1).Where(row =>
            {
                return !row.ChildNodes.Any(
                    n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("description"));
            });
            return episodeRows;
        }

        private Episode ParseEpisode(HtmlNode episodeRow)
        {
            var number = ExtractEpisodeNumber(episodeRow);
            var title = ExtractEpisodeTitle(episodeRow);
            var date = ExtractEpisodeAirDate(episodeRow);

            return new Episode { FirstAired = date, Name = title, Number = number };
        }

        private DateTime ExtractEpisodeAirDate(HtmlNode episodeRow)
        {
            var dateString = episodeRow.Descendants("span").FirstOrDefault(n => n.Attributes.Contains("class")
                                                                           && n.Attributes["class"].Value.Contains("dtstart"))?.InnerHtml;

            var date = string.IsNullOrWhiteSpace(dateString)
                ? new DateTime(1970, 1, 1)
                : DateTime.ParseExact(dateString, "yyyy-MM-dd", null);

            if (date.Year == 1970 && date.Month == 1 && date.Day == 1)
            {
                var dateNode = episodeRow.Descendants("td").Skip(4).FirstOrDefault();
                dateString = dateNode.InnerText;
                date = string.IsNullOrWhiteSpace(dateString)
                      ? new DateTime(1970, 1, 1)
                      : DateTime.ParseExact(dateString, "MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"));
            }

            return date;
        }

        private string ExtractEpisodeTitle(HtmlNode episodeRow)
        {
            var title = episodeRow.Descendants("td").Skip(1).FirstOrDefault()?.InnerText.Trim(new [] {'"'});
                //.Descendants("a").FirstOrDefault()?.InnerHtml;
            return title;
        }

        private int ExtractEpisodeNumber(HtmlNode episodeRow)
        {
            var episodeString = episodeRow.Descendants("td").FirstOrDefault()?.InnerHtml;
            var episodeNumber = string.IsNullOrWhiteSpace(episodeString) ? 0 : int.Parse(episodeString);

            return episodeNumber;
        }
    }
}
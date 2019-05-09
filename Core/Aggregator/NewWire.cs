using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;
using LGFABot.Tools;
using LGFABot.Tools.Properties;
using LiteDB;

namespace LGFA.Core.Aggregator
{
    public class NewsWire : ModuleBase<SocketCommandContext>
    {

        public static async Task TradeNewsAsync(IMessageChannel channel, string system)
        {
            var web = new HtmlWeb();
            var url = Fetch.GetNewsUrl();
            HtmlDocument feed = null;
            HtmlNodeCollection nodes;
            var feedString = "//*[@id='newsfeed_page']/ol/li[1]";

            var xbox = "53" + "&typeid=7";
            var psn = "73" + "&typeid=7";

            if (system == "xbox")
            {
                feed = web.Load(url + "53" + "&typeid=7");
                nodes = feed.DocumentNode.SelectNodes(feedString);
                await RunFeed(nodes, channel, "xbox");

            }
            else if (system == "psn")
            {
                feed = web.Load(url + "73" + "&typeid=7");
                nodes = feed.DocumentNode.SelectNodes(feedString);
                await RunFeed(nodes, channel, "psn");
            }
        }

        private static async Task RunFeed(HtmlNodeCollection nodes, IMessageChannel channel, string system)
        {
            var tempDateTime = "";
            var teamTwoName = "";
            var systemIcon = "";
            if (system == "psn") systemIcon = "https://media.playstation.com/is/image/SCEA/navigation_home_ps-logo-us?$Icon$";
            else if (system == "xbox") systemIcon = "http://www.logospng.com/images/171/black-xbox-icon-171624.png";
            foreach (var item in nodes)
            {
                tempDateTime =
                    WebUtility.HtmlDecode(
                        item.SelectSingleNode(@"//*[@id='newsfeed_page']/ol/li[1]/div/abbr").InnerText);
                var line = WebUtility.HtmlDecode(item.SelectSingleNode(@"//*[@id='newsfeed_page']/ol/li[1]/div/h3")
                    .InnerText);
                var str = line;
                var removeThe = new string[] { "The  ", "the  " };

                foreach (var c in removeThe)
                {
                    str = str.Replace(c, String.Empty);
                }
                var splitStr = str.Split(new String[] { "to " }, StringSplitOptions.None);
                splitStr[1] = splitStr[1].Replace("  ", " ");
                var iconTemp = WebUtility.HtmlDecode(item
                    .SelectSingleNode(@"//*[@id='newsfeed_page']/ol/li[1]/a[2]/img").Attributes["src"].Value);
                var lastNews = DateTime.Parse(tempDateTime);

                var feedIcon = Fetch.GetTradeIcon();

                if (!SaveNews(lastNews, splitStr[0], splitStr[1], system))
                {
                    break;
                }
                else
                {
                    try
                    {
                        var table = "";
                        if (system == "xbox") table = "XboxTrade";
                        else if (system == "psn") table = "PsnTrade";
                        using (var newsDb = new LiteDatabase(@"News.db"))
                        {
                            var news = newsDb.GetCollection<News>(table);
                            var result = news.Find(x => x.date.Equals(lastNews));
                            foreach (var headline in result)
                            {
                                var builder = new EmbedBuilder()
                                    .WithColor(new Color(0xFF0019))
                                    .WithTimestamp(lastNews)
                                    .WithFooter(footer =>
                                    {
                                        footer
                                            .WithText("leaguegaming.com/fifa")
                                            .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                                    })
                                    .WithThumbnailUrl(
                                        "https://cdn0.iconfinder.com/data/icons/trading-outline/32/trading_outline_2._Location-512.png")
                                    .WithAuthor(author =>
                                    {
                                        author
                                            .WithName("Sky Sports Trade Center")
                                            .WithIconUrl(systemIcon);
                                    })
                                    .AddField(splitStr[0], ":arrow_right:", false)
                                    .AddField(splitStr[1], ":white_check_mark: ", false);
                                var embed = builder.Build();
                                await channel.SendMessageAsync(
                                        null,
                                        embed: embed)
                                    .ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }


        public static async Task WaiverNews(IMessageChannel channel, string system)
        {
            var waiverWeb = new HtmlWeb();
            var url = Fetch.GetNewsUrl();
            var systemIcon = "";
            var tempDateTime = "";
            HtmlDocument feed;
            HtmlNodeCollection nodes = null;
            if (system == "xbox")
            {
                feed = waiverWeb.Load(url + "53" + "&typeid=9");
                nodes = feed.DocumentNode.SelectNodes(@"//*[@id='newsfeed_page']/ol/li[1]/div/h3");
                systemIcon = "http://www.logospng.com/images/171/black-xbox-icon-171624.png";
            }
            else if (system == "psn")
            {
                feed = waiverWeb.Load(url + "73" + "&typeid=9");
                nodes = feed.DocumentNode.SelectNodes(@"//*[@id='newsfeed_page']/ol/li[1]");
                systemIcon = "https://media.playstation.com/is/image/SCEA/navigation_home_ps-logo-us?$Icon$";
            }

            foreach (var item in nodes)
            {
                tempDateTime = item.SelectSingleNode(@"//*[@id='newsfeed_page']/ol/li[1]/div/abbr").InnerText;
                var line = item.SelectSingleNode(@"//*[@id='newsfeed_page']/ol/li[1]/div/h3").InnerText;

                var str = line;
                var removeThe = new string[] {"The ", "the "};
                foreach (var s in removeThe)
                {
                    str = str.Replace(s, String.Empty);
                }
                var lastNews = DateTime.Parse(tempDateTime);
                if (!SaveWaiverNews(lastNews, str, system))
                {
                    break;
                }
                else
                {
                    try
                    {
                        var table = "";
                        if (system == "xbox") table = "XboxWaiver";
                        else if (system == "psn") table = "PsnWaiver";
                        using (var waiverDb = new LiteDatabase(@"News.db"))
                        {
                            var waiver = waiverDb.GetCollection<Waivers>(table);
                            var result = waiver.Find(d => d.dateTime.Equals(lastNews));
                            foreach (var headline in result)
                            {
                                var builder = new EmbedBuilder()
                                    .WithColor(new Color(0xFF0019))
                                    .WithTimestamp(lastNews)
                                    .WithFooter(footer =>
                                    {
                                        footer
                                            .WithText("leaguegaming.com/fifa")
                                            .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                                    })
                                    .WithAuthor(author =>
                                    {
                                        author
                                            .WithName("Sky Sports Waiver Wire")
                                            .WithIconUrl(systemIcon);
                                    })
                                    .WithDescription("**" +str + "**");
                                var embed = builder.Build();
                                await channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        public static bool SaveWaiverNews(DateTime dateStamp, string line, string system)
        {
            DateTime dbDateTime = new DateTime();
            var table = "";
            const int index = 1;
            if (system == "xbox") table = "XboxWaiver";
            else if (system == "psn") table = "PsnWaiver";
            using (var waiverDb = new LiteDatabase("News.db"))
            {
                var waiver = waiverDb.GetCollection<Waivers>(table);
                var result = waiver.Find(x => x.Id == 1);
                foreach (var dateTime in result)
                {
                    dbDateTime = dateTime.dateTime;
                }
            }

            using (var database = new LiteDatabase("News.db"))
            {
                var waiverCollection = database.GetCollection<Waivers>(table);
                waiverCollection.EnsureIndex(x => x.dateTime);
                int Id = 1;
                var waiverFeed = new Waivers
                {
                    Id = Id,
                    dateTime = dateStamp,
                    line = line
                };
                try
                {
                    var dateFound = waiverCollection.FindById(index);
                    var value = DateTime.Compare(dbDateTime, dateStamp);
                    if (dateFound == null)
                    {
                        waiverCollection.Insert(waiverFeed);
                        return true;
                    } else if (dateFound != null)
                    {
                        if (value < 0)
                        {
                            waiverCollection.Update(waiverFeed);
                            return true;
                        } else if (value == 0) return false;
                        else
                        {
                            return false;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return false;
        }
        private static bool SaveNews(DateTime date, string teamOne, string teamTwo, string system)
        {
            DateTime dbDateTime = new DateTime();
            var table = "";
            const int index = 1;
            if (system == "xbox")
            {
                table = "XboxTrade";
            }
            else if (system == "psn")
            {
                table = "PsnTrade";
            }
            using (var newsDb = new LiteDatabase(@"News.db"))
            {
                var news = newsDb.GetCollection<News>(table);
                var result = news.Find(x => x.Id == index);
                foreach (var datetime in result)
                {
                    dbDateTime = datetime.date;
                }

            }
            using (var database = new LiteDatabase("News.db"))
            {
                var newsCollection = database.GetCollection<News>(table);
                newsCollection.EnsureIndex(x => x.date);
                var newsFeed = new News
                {
                    Id = index,
                    date = date,
                    newsLineOne = teamOne,
                    newsLineTwo = teamTwo
                };
                try
                {
                    var datefound = newsCollection.FindById(index);
                    var value = DateTime.Compare(dbDateTime, date);
                    if (datefound == null)
                    {
                        newsCollection.Insert(newsFeed);
                        return true;
                    }
                    else if (datefound != null)
                    {
                        if (value < 0)
                        {
                            newsCollection.Update(newsFeed);
                            return true;
                        }
                        else if (value == 0) return false;
                        else return false;
                    }

                    //var value = DateTime.Compare(dbDateTime, date);
                    //if (value < 0)
                    //{
                    //    //if database time is earlier do nothing
                    //    return false;
                    //}
                    //else if (value == 0)
                    //{
                    //    newsCollection.Insert(newsFeed);
                    //    return true;
                    //}
                    //else
                    //{

                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return false;
        }

    }
}

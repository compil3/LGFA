using System;
using System.Threading.Tasks;
using Discord;
using FluentScheduler;
using LGFA.Core.Aggregator;

namespace LGFABot.Tools.Schedules
{
   public class News : Registry
    {
        public News(IMessageChannel channel)
        {
            Action newsUpdate = new Action(async ()=>
            {
                await NewsWire.TradeNewsAsync(channel, "xbox");
                await NewsWire.TradeNewsAsync(channel, "psn");
                await NewsWire.WaiverNews(channel, "xbox");
                await NewsWire.WaiverNews(channel, "psn");
            });
            this.Schedule(newsUpdate).ToRunNow().AndEvery(30).Seconds();
        }

        private static async Task SendMessageAsync(Embed message, IMessageChannel newsChan)
        {
            //await newsChan.SendMessageAsync(message);
            await newsChan.SendMessageAsync(
                    null,
                    embed: message)
                .ConfigureAwait(false);

        }
    }
}

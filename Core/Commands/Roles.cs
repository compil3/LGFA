using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace LGFABot.Core.Commands
{
    public class Roles : ModuleBase<SocketCommandContext>
    {
        
        public async Task Role([Remainder] string system)
        {
            var user = Context.User as SocketGuildUser;
            if (user.Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Accepted Rules")))
            {
                if (user.Roles.Contains(Context.Guild.Roles.FirstOrDefault(x=>x.Name=="Xbox")) || user.Roles.Contains(Context.Guild.Roles.FirstOrDefault(x=>x.Name=="PSN")))
                {
                    await user.RemoveRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "New Member"));
                }
            }

        }
    }
}

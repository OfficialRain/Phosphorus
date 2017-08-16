using Discord;
using Discord.WebSocket;
using NinjaNye.SearchExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phosphorus
{
    public partial class Tools
    {
        public static string[] GetArgs(string message)
        {
            if (!message.Contains(" "))
            {
                return new string[0];
            }
            else
            {
                List<string> templist = new List<string>();
                foreach (var item in message.Split(','))
                {
                    if (item != message.Split(',')[0])
                        templist.Add(item.Replace(",", string.Empty).Trim());
                    else
                    {
                        templist.Add(item.Substring(message.IndexOf(' ')).Split(',')[0].Replace(",", string.Empty).Trim());
                    }


                }

                return templist.ToArray();
            }
        }

        public static List<IUser> GetUser(List<string> identifiers, bool repeat, bool exact)
        {
            List<IUser> users = new List<IUser>();
            List<IUser> allusers = new List<IUser>();
            foreach (var guild in Program.Client.Guilds)
            {
                allusers.AddRange(guild.Users);
            }

            //oh no

            foreach (var item in identifiers)
            {
                var result = allusers.Search(x => GetNicknameOrUsername(x as SocketGuildUser).ToLower(), x => x.Username.ToLower())
                    .Containing(item.ToLower());

                if (result.Count() != 0)
                {
                    if (exact)
                    {
                        if (!repeat)
                            AddIfDoesntContainUser(users, new List<IUser>() { result.ToList()[0] });
                        else
                            users.Add(result.ToList()[0]);
                    }
                    else
                    {
                        if (!repeat)
                            AddIfDoesntContainUser(users, result.ToList());
                        else
                            users.AddRange(result.ToList());
                    }
                }
                else
                {
                    users.Add(null);
                }
            }

            return users;
        }

        public static List<IGuild> GetGuild(List<string> identifiers, bool repeat, bool exact)
        {
            List<IGuild> guilds = new List<IGuild>();

            foreach (var item in identifiers)
            {
                var result = Program.Client.Guilds.Search(x => x.Name.ToLower())
                    .Containing(item.ToLower());

                if (result.Count() != 0)
                {
                    if (exact)
                    {
                        if (!repeat)
                            AddIfDoesntContainGuild(guilds, new List<SocketGuild>() { result.ToList()[0] });
                        else
                            guilds.Add(result.ToList()[0]);
                    }
                    else
                    {
                        if (!repeat)
                            AddIfDoesntContainGuild(guilds, result.ToList());
                        else
                            guilds.AddRange(result.ToList());
                    }
                }
                else
                {
                    guilds.Add(null);
                }
            }

            return guilds;
        }
    }
}

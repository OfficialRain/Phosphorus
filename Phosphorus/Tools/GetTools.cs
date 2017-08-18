using Discord;
using Discord.WebSocket;
using NinjaNye.SearchExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phosphorus
{
    public partial class Tools
    {
		/// <summary>
		/// Splits a message into seperate parameters to be parsed by commands.
		/// </summary>
		/// <param name="message">The message to split into a string array</param>
		/// <returns></returns>
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

		/// <summary>
		/// Advanced user search.
		/// </summary>
		/// <param name="identifiers">List of strings to query in the user database.</param>
		/// <param name="repeat">If set to true, multiple instances of users from different guilds will be included in the return list.</param>
		/// <param name="exact"> If set to true, only the first match will be returned. </param>
		/// <returns>Returns a list of <see cref="IUser"/>.</returns>
		public static List<IUser> GetUser(List<string> identifiers, bool repeat, bool exact, KeyValuePair<bool, SocketGuild> localSwitch)
        {
            List<IUser> users = new List<IUser>();
            List<IUser> allusers = new List<IUser>();
			if(!localSwitch.Key)
			{
				foreach (var guild in Program.Client.Guilds)
				{
					allusers.AddRange(guild.Users);
				}
			}
			else
			{
				allusers.AddRange(localSwitch.Value.Users);
			}

			//oh no

			foreach (var item in identifiers)
            {
                var result = allusers.Search(x => GetDisplayName(x as SocketGuildUser).ToLower(), x => x.Username.ToLower())
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
		/// <summary>
		/// Advanced guild search.
		/// </summary>
		/// <param name="identifiers">List of strings to query in the guild database.</param>
		/// <param name="exact"> If set to true, only the first match will be returned. </param>
		/// <returns>Returns a list of <see cref="IGuild"/>.</returns>

		public static List<IGuild> GetGuild(List<string> identifiers, bool exact)
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
                            guilds.Add(result.ToList()[0]);
                    }
                    else
                    {
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

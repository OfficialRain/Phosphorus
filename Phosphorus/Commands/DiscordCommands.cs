using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Phosphorus.Core;

namespace Phosphorus
{
    public static class DiscordCommands
    {
        static Random RandomEngine = new Random();

		/// <summary>
		/// Initializes discord commands. The constructor for <see cref="DiscordCommand"/> adds itself to a list of triggers.
		/// </summary>
		public static void InitializeDiscordCommands()
        {
            DiscordCommand Ping = new DiscordCommand()
            {
                Aliases = new List<string>() { "ping", "pong" },
                Usage = new List<Usage>(),
                Category = "Misc.",
                Description = "Checks to see if the bot is online.",
                Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
                {
                    string[] pings = new string[] { "I am alive.", "Turtl'o'bot is my friend!", "JXBot is the best!", "Do you like cheese? 🧀", "Remember PhosphorusVB?", "Thanks for playing pong with me!", "Dab on them haters!", "What if haters dab back?", "`IndexOutOfRangeExcepti`- just kidding." };
                    if (args.Count() == 0)
                        await context.Channel.SendMessageAsync("**Pong!** " + pings[RandomEngine.Next(pings.Length)]);
                    else
                        try
                        {
                            await context.Channel.SendMessageAsync("**Pong!** " + pings[Convert.ToInt32(args[0])]);
                        }
                        catch (FormatException)
                        {
                            await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("Bad format", "Please don't try and fool me - you can only type a number here."));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("Need more pings!!!1", $"That's not a valid index. Try again with a lower index (right now we have {pings.Count().ToString()}!"));
                        }
                })
            };

            DiscordCommand Help = new DiscordCommand()
            {
                Aliases = new List<string>() { "help", "halp" },
                Usage = new List<Usage>() { new Usage() { Name = "command", ParameterType = ParameterType.Optional } },
                Category = "Misc.",
                Description = "Lists the possible Discord commands.",
                Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
                {
                    if (args.Count() == 0)
                    {
                        Config.HelpEmbed = new EmbedBuilder()
                        {
                            Color = Config.PhosphorusColor,
                            Author = new EmbedAuthorBuilder()
                            {
                                IconUrl = context.Client.CurrentUser.GetAvatarUrl(),
                                Name = $"{context.Client.CurrentUser.Username} Help"
                            },
                            Description = $"This is a list of commands that you can use in {context.Client.CurrentUser.Username}. Type in `p.help [command name]` to get more info on a command."
                        };
                    }
                    else
                    {
                        foreach (var arg in args)
                        {
                            var command = Config.DiscordCommands.FirstOrDefault(x => x.Aliases.Contains(arg));
                            if (command != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                StringBuilder sb2 = new StringBuilder();
                                command.Aliases.ForEach(x => sb.Append(x + " "));
                                foreach (var item in command.Usage)
                                {
                                    switch (item.ParameterType)
                                    {
                                        case ParameterType.Required:
                                            sb2.Append($"`<{item.Name}>`, ");
                                            break;
                                        case ParameterType.Optional:
                                            sb2.Append($"`[{item.Name}]`, ");
                                            break;
                                        case ParameterType.Infinite:
                                            sb2.Append($"`<{item.Name}1>`, `<{item.Name}2>`, `<{item.Name}3>`...");
                                            break;
                                        case ParameterType.InfiniteOptional:
                                            sb2.Append($"`[{item.Name}1]`, `[{item.Name}2]`, `[{item.Name}3]`...");
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                EmbedBuilder embed = new EmbedBuilder()
                                {
                                    Author = new EmbedAuthorBuilder()
                                    {
                                        IconUrl = context.Client.CurrentUser.GetAvatarUrl(),
                                        Name = $"{arg} Help"
                                    },
                                    Title = sb.ToString(),
                                    Description = $"**Description:** {command.Description}{Environment.NewLine}**Category:** {command.Category}{Environment.NewLine}**Permission Level:** {command.PermissionLevel.ToString()}{Environment.NewLine}**Usage:** `<p.{command.Aliases[0]}>` {sb2.ToString()}",
                                    Footer = new EmbedFooterBuilder()
                                    {
                                        Text = "`[optional parameter]` `<required parameter>`"
                                    }
                                };
                                await context.Channel.SendMessageAsync(context.Message.Content.StartsWith("p.halp") ? "\"halp\"? Really?" : "", embed: embed);
                            }
                            else
                            {
                                await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("Command not found", $"A command with the name `{arg}` cannot be found. Please try again with a different command or look at the full command list by typing in `p.help`."));
                            }
                        }
                    }
                })
            };

            DiscordCommand GetAvatar = new DiscordCommand()
            {
                Aliases = new List<string>() { "getavatar", "getpfp", "picture", "getprofilepicture", "pfp" },
                Usage = new List<Usage>() { new Usage() { Name = "user", ParameterType = ParameterType.Infinite } },
                Category = "Misc.",
                Description = "Gets the avatar of a user.",
                Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
                {
                    var users = Tools.GetUser(args.ToList(), false, true);

                    foreach (var user in users)
                    {
                        if (user != null)
                        {
                            EmbedBuilder embed = new EmbedBuilder()
                            {
                                Color = Tools.DominantPicture(user),
                                ImageUrl = user.GetAvatarUrl(size: 2048),
                                Title = user.Username + "'s Avatar"
                            };
                            await context.Channel.SendMessageAsync("", embed: embed);
                        }
                        else
                        {
                            await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("User not found", $"One of the users was not found."));
                        }
                    }
                })
            };

            DiscordCommand UserInfo = new DiscordCommand()
            {
                Aliases = new List<string>() { "uinfo", "userinfo", "getuinfo" },
                Usage = new List<Usage>() { new Usage() { Name = "guild", ParameterType = ParameterType.Required }, new Usage() { Name = "user", ParameterType = ParameterType.Infinite } },
                Category = "Staff",
                Description = "Retrieves various information about a user.",
                Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
                {
                    IGuild guild = Tools.GetGuild(new List<string>() { args.FirstOrDefault(x => x.StartsWith("from")).Substring(5) }, false, true)[0];

                    foreach (var arg in args)
                    {
                        if (arg.StartsWith("from"))
                            continue;
                        var usermini = Tools.GetUser(new List<string>() { arg }, true, true)[0];
                        if (usermini == null)
                        {
                            await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("User not found", $"One of the users was not found."));
                            return;
                        }

                        StringBuilder sb = new StringBuilder();
                        StringBuilder sb2 = new StringBuilder();
                        if (usermini.Game.HasValue)
                        {
                            if (usermini.Game.Value.StreamType == StreamType.Twitch)
                                sb.Append($"Streaming{Environment.NewLine}**Stream URL:** {usermini.Game.Value.StreamUrl}");
                        }
                        else
                        {
                            sb.Append(usermini.Status.ToString());
                        }
                        EmbedBuilder embedmini = new EmbedBuilder()
                        {
                            Color = Tools.DominantPicture(usermini),
                            Author = new EmbedAuthorBuilder()
                            {
                                IconUrl = usermini.GetAvatarUrl(),
                                Name = $"{usermini.Username}#{usermini.Discriminator}"
                            },
                            Footer = new EmbedFooterBuilder()
                            {
                                Text = $"Invoked by {context.Message.Author}",
                                IconUrl = context.Message.Author.GetAvatarUrl()
                            },
                            ThumbnailUrl = usermini.GetAvatarUrl(size: 2048),
                            Timestamp = context.Message.Timestamp
                        };

                        embedmini.AddInlineField("Identification", $"**Username:** {usermini.Username}{Environment.NewLine}**Discriminator:** {usermini.Discriminator}{Environment.NewLine}**ID:** {usermini.Id.ToString()}{Environment.NewLine}**Created at:** {usermini.CreatedAt}{Environment.NewLine}**Status:** {sb.ToString()}{Environment.NewLine}**Playing:** {Tools.GetGame(usermini)}");

                        if (guild != null)
                        {
                            var users = Tools.GetUser(new List<string>() { arg }, true, true).First(x => (x as SocketGuildUser).Guild == guild) as SocketGuildUser;
                            embedmini.AddInlineField($"On {users.Guild}", $"**Display Name:** {users.Nickname}{Environment.NewLine}**Joined at:** {users.JoinedAt}");
                        }
                        if (usermini.IsBot)
                            sb2.Append($"This user is a bot.{Environment.NewLine}");
                        if (usermini.IsWebhook)
                            sb2.Append($"This user is a webhook.{Environment.NewLine}");

                        if (sb2.ToString() != string.Empty)
                        {
                            embedmini.AddInlineField("Warnings", sb2.ToString());
                        }

                        await context.Channel.SendMessageAsync("", embed: embedmini);
                    }
                })
            };
        }
    }
}

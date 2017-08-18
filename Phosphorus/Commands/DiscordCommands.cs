using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Phosphorus.Core;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Phosphorus
{
	public static class DiscordCommands
    {
        static Random RandomEngine = new Random();

		/// <summary>
		/// Initializes discord commands. The constructor for <see cref="DiscordCommand"/> adds itself to a list of discord commands.
		/// </summary>
		public static void InitializeDiscordCommands()
        {
			DiscordCommand Ping = new DiscordCommand()
			{
				Aliases = new List<string>() { "ping", "pong" },
				Usage = new List<Usage>(),
				Category = "Misc.",
				Description = "Checks to see if the bot is online.",
				PermissionLevel = PermissionLevel.User,
                Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
                {
                    string[] pings = new string[] { "I am alive.", "Turtle'o'bot is my friend!", "JXBot is the best!", "Do you like cheese? 🧀", "Remember PhosphorusVB?", "Thanks for playing pong with me!", "Dab on them haters!", "What if haters dab back?", "`IndexOutOfRangeExcepti`- just kidding." };
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
                            await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("Need more pings!!!1", $"That's not a valid index. Try again with a lower index (right now we have {pings.Count()}!"));
                        }
				})
            };

            DiscordCommand Help = new DiscordCommand()
            {
                Aliases = new List<string>() { "help", "halp" },
                Usage = new List<Usage>() { new Usage() { Name = "command", ParameterType = ParameterType.Optional } },
                Category = "Misc.",
                Description = "Lists the possible Discord commands.",
				PermissionLevel = PermissionLevel.User,
				Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
                {
                    if (args.Count() == 0)
                    {
						List<string> list = new List<string>();
						EmbedBuilder embed = new EmbedBuilder()
						{
							Color = Config.PhosphorusColor,
							Author = new EmbedAuthorBuilder()
							{
								IconUrl = context.Client.CurrentUser.GetAvatarUrl(),
								Name = $"{context.Client.CurrentUser.Username} Help"
							},
							Description = $"This is a list of commands that you can use in {context.Client.CurrentUser.Username}. Type in `p.help [command name]` to get more info on a command. {Environment.NewLine}"
						};
						foreach (var item in Config.DiscordCommands)
							if (!list.Contains(item.Category))
								list.Add(item.Category);

						foreach (var item in list)
						{
							StringBuilder sb = new StringBuilder();
							foreach(var command in Config.DiscordCommands.Where(x => x.Category == item))
							{
								sb.AppendLine(command.Aliases[0]);
							}
							embed.AddInlineField(item, sb.ToString());
						}

						await context.Channel.SendMessageAsync("", embed: embed);
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
									Color = Config.PhosphorusColor,
                                    Description = $"**Description:** {command.Description}{Environment.NewLine}**Category:** {command.Category}{Environment.NewLine}**Permission Level:** {command.PermissionLevel.ToString()}{Environment.NewLine}**Usage:** `<p.{command.Aliases[0]}>` {sb2.ToString()}",
                                    Footer = new EmbedFooterBuilder()
                                    {
                                        Text = "<required parameter> [optional parameter]"
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
                Category = "User",
                Description = "Gets the avatar of a user.",
				PermissionLevel = PermissionLevel.User,
				Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
                {
					var users = Tools.GetUser(args.ToList(), false, true, new KeyValuePair<bool, SocketGuild>(true, null));

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

			DiscordCommand Off = new DiscordCommand()
			{
				Aliases = new List<string>() { "shutdown", "off", "exit" },
				Usage = new List<Usage>() { new Usage() { Name = "exit code", ParameterType = ParameterType.Optional} },
				Category = "Owner.",
				Description = "Shuts down the bot.",
				PermissionLevel = PermissionLevel.ApplicationOwner,
				Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
				{
					if(args.Count() > 0)
					{
						try
						{
							await context.Channel.SendMessageAsync($"Shutting down with exit code {args[0]}...");
							await context.Client.SetStatusAsync(UserStatus.Invisible);
							Environment.Exit(Convert.ToInt32(args[0]));
						}
						catch(FormatException)
						{
							await context.Channel.SendMessageAsync("", embed:Tools.ErrorEmbedCreator("Invalid exit code", $"{args[0]} isn't a valid application exit code. Please try again with a number."));
							return;
						}
					}
					else
					{
						await context.Channel.SendMessageAsync($"Shutting down with exit code 0...");
						await context.Client.SetStatusAsync(UserStatus.Invisible);
						Environment.Exit(0);
					}

					await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("Could not shut down", $"For some bizarre reason the bot cannot be shut down at this time."));
				})
			};

			DiscordCommand Do = new DiscordCommand()
			{
				Aliases = new List<string>() { "manage", "do", "deal", "act" },
				Usage = new List<Usage>() { new Usage() { Name = "user", ParameterType = ParameterType.Infinite } },
				Category = "Staff",
				Description = "Manages users.",
				PermissionLevel = PermissionLevel.Mod,
				Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
				{
					var users = Tools.GetUser(args.ToList(), false, true, new KeyValuePair<bool, SocketGuild>(true, context.Guild));
					StringBuilder sb = new StringBuilder();
					StringBuilder sb2 = new StringBuilder();

					users.ForEach(x => sb2.Append($"{x.Username}#{x.Discriminator} "));
					if(context.Guild.GetUser(context.Client.CurrentUser.Id).GuildPermissions.ManageNicknames)
					{
						sb.Append("`[n]ick` ");
					}
					if (context.Guild.GetUser(context.Client.CurrentUser.Id).GuildPermissions.ManageRoles)
					{
						//TODO: implement these features
						//sb.Append("`[m]ute` `[i]nterrogate` `[j]ail` ");
					}
					if (context.Guild.GetUser(context.Client.CurrentUser.Id).GuildPermissions.KickMembers)
					{
						sb.Append("`[k]ick` ");
					}
					if (context.Guild.GetUser(context.Client.CurrentUser.Id).GuildPermissions.BanMembers)
					{
						sb.Append("`[b]an` ");
					}
					if(sb.ToString() == string.Empty)
					{
						await context.Channel.SendMessageAsync("No action of mine is allowed by my permissions. Cancelling action.");
						return;
					}
					sb.Append("`cancel` ");

					await context.Channel.SendMessageAsync($"Take action against {sb2.ToString()}: {sb.ToString()}");

					DirectAccessToken token = new DirectAccessToken(context.Message.Author);
					token.AccessTokenInvoked += async (messageAsObject, e) =>
					{
						var message = (messageAsObject as SocketMessage);
						switch(message.Content.ToLower())
						{
							case "n":
							case "nick":
								{
									token.Dispose();
									await message.Channel.SendMessageAsync("Type in a nickname to change the names to or `cancel`.");
									DirectAccessToken nicktoken = new DirectAccessToken(context.User);
									nicktoken.AccessTokenInvoked += async (nickMessageAsObject, nickE) =>
									{
										var nickMessage = (nickMessageAsObject as SocketMessage);
										if (nickMessage.Content.ToLower() == "cancel")
										{
											await nickMessage.Channel.SendMessageAsync("Cancelled action.");
											nicktoken.Dispose();
											return;
										}
										foreach (var user in users)
										{
											await (user as SocketGuildUser).ModifyAsync(x => x.Nickname = nickMessage.Content);
										}
										await message.Channel.SendMessageAsync($"The user(s') nickname(s) have been changed to \"{nickMessage.Content}\".");
										nicktoken.Dispose();
									};
								}
								return;
							case "k":
							case "kick":
								{
									token.Dispose();
									await message.Channel.SendMessageAsync("Type in a reason to kick or `cancel`.");
									DirectAccessToken kicktoken = new DirectAccessToken(context.User);
									kicktoken.AccessTokenInvoked += async (kickMessageAsObject, nickE) =>
									{
										var kickMessage = (kickMessageAsObject as SocketMessage);
										if (kickMessage.Content.ToLower() == "cancel")
										{
											await kickMessage.Channel.SendMessageAsync("Cancelled action.");
											kicktoken.Dispose();
											return;
										}
										foreach (var user in users)
										{
											//too lazy to use context
											await (user as SocketGuildUser).KickAsync(kickMessage.Content);
										}
										await message.Channel.SendMessageAsync($"The user(s') have been kicked for \"{kickMessage.Content}\".");
										kicktoken.Dispose();
									};
								}
								return;

							case "b":
							case "ban":
								{
									token.Dispose();
									await message.Channel.SendMessageAsync("Type in a reason to ban or `cancel`.");
									DirectAccessToken bantoken = new DirectAccessToken(context.User);
									bantoken.AccessTokenInvoked += async (banMessageAsObject, nickE) =>
									{
										var banMessage = (banMessageAsObject as SocketMessage);
										if (banMessage.Content.ToLower() == "cancel")
										{
											await banMessage.Channel.SendMessageAsync("Cancelled action.");
											bantoken.Dispose();
											return;
										}
										foreach (var user in users)
										{
											//too lazy to use context
											await (message.Author as SocketGuildUser).Guild.AddBanAsync(user as IUser, reason: banMessage.Content);
										}
										await message.Channel.SendMessageAsync($"The user(s') have been banned for \"{banMessage.Content}\".");
										bantoken.Dispose();
									};
								}
								return;
							case "cancel":
								await message.Channel.SendMessageAsync("Cancelled action.");
								token.Dispose();
								return;
							default:
								await message.Channel.SendMessageAsync("Unknown action. Cancelled action.");
								token.Dispose();
								return;
						}
					};
				})
			};

			DiscordCommand Find = new DiscordCommand()
			{
				Aliases = new List<string>() { "find", "search" },
				Usage = new List<Usage>() { new Usage() { Name = "query", ParameterType = ParameterType.Required } },
				Category = "Staff",
				Description = "Queries a cross-server list of users with a specified search term.",
				PermissionLevel = PermissionLevel.Mod,
				Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
				{
					var users = Tools.GetUser(new List<string>() { args[0] }, false, false, new KeyValuePair<bool, SocketGuild>(false, null));
					StringBuilder sb = new StringBuilder();
					int i = 0;
					foreach(var user in users)
					{
						sb.Append($"**{user.Username}**#{user.Discriminator}{Environment.NewLine}");
						i++;
						if (i >= 15)
						{
							sb.Append($"{(users.Count - 15)} more results - please narrow your query.");
							break;
						}
					}

					EmbedBuilder embed = new EmbedBuilder()
					{
						Title = $"Search results for {args[0]}",
						Color = Config.PhosphorusColor,
						Description = sb.ToString(),
						Footer = new EmbedFooterBuilder()
						{
							Text = $"Invoked by {context.Message.Author.Username}"
						},
						Timestamp = context.Message.Timestamp
					};

					await context.Channel.SendMessageAsync("", embed: embed);
				})
			};

			DiscordCommand BulkDelete = new DiscordCommand()
			{
				Aliases = new List<string>() { "bulkdelete", "prune", "purge" },
				Usage = new List<Usage>() { new Usage() { Name = "messages to delete", ParameterType = ParameterType.Required } },
				Category = "Staff",
				Description = "Deletes a certain amount of messages.",
				PermissionLevel = PermissionLevel.Mod,
				Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
				{
					try
					{
						await context.Channel.DeleteMessagesAsync(await context.Channel.GetMessagesAsync(Convert.ToInt32(args[0])).Flatten());
						await context.Channel.SendMessageAsync($"<:checkmark:348180546519564293> Deleted { args[0]} messages.");
					}
					catch (FormatException)
					{
						await context.Channel.SendMessageAsync("", embed: Tools.ErrorEmbedCreator("Not a number", "Please specify a valid amount of messages to delete."));
					}
				})
			};

			DiscordCommand Info = new DiscordCommand()
			{
				Aliases = new List<string>() { "info", "stats" },
				Usage = new List<Usage>(),
				Category = "Misc.",
				Description = $"Returns various info about the runtime of {Program.Client.CurrentUser.Username}.",
				PermissionLevel = PermissionLevel.User,
				Code = new Func<SocketCommandContext, string[], Task>(async (context, args) =>
				{
					Process currentProcess = Process.GetCurrentProcess();
					TimeSpan time = DateTime.Now - currentProcess.StartTime;
					EmbedBuilder embed = new EmbedBuilder()
					{
						Title = $"{Program.Client.CurrentUser.Username} Info",
						Color = Config.PhosphorusColor,
						Timestamp = context.Message.Timestamp
					};
					embed.AddInlineField("Stats", $"**Library:** Discord.NET {DiscordConfig.Version}{Environment.NewLine}**Uptime:** {time.ToString().Substring(0, 8)}**Framework:** {RuntimeInformation.FrameworkDescription}{Environment.NewLine}**OS Description:** {RuntimeInformation.OSDescription}{Environment.NewLine}**Process Architecture:** {RuntimeInformation.ProcessArchitecture}{Environment.NewLine}**Core Count:** {Environment.ProcessorCount}{Environment.NewLine}**RAM Usage:** {currentProcess.WorkingSet64 / 1024 / 1024} MB{Environment.NewLine}**Threads:** {currentProcess.Threads.Count}");

					await context.Channel.SendMessageAsync("", embed: embed);
				})
			};
		}
	}
}
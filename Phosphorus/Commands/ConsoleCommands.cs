using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Phosphorus.Core;

namespace Phosphorus
{

    public static class ConsoleCommands
    {
        static Random RandomEngine = new Random();

		/// <summary>
		/// Initializes console commands. The constructor for <see cref="ConsoleCommand"/> adds itself to a list of ConsoleCommands.
		/// </summary>
		public static void InitializeConsoleCommands()
        {
            ConsoleCommand Ping = new ConsoleCommand()
            {
                Aliases = new List<string>() { "ping", "pong" },
                Usage = new List<Usage>(),
                Category = "Misc.",
                Description = "Checks to see if the console is working.",
                Code = new Func<string, string[], Task>(async (message, args) =>
                {
                    string[] pings = new string[] { "I am alive.", "Turtl'o'bot is my friend!", "JXBot is the best!", "Do you like cheese? 🧀", "Remember PhosphorusVB?", "Thanks for playing pong with me!", "Dab on them haters!", "What if haters dab back?", "`IndexOutOfBoundsExcepti`- just kidding." };
                    Console.WriteLine("**Pong!** " + pings[RandomEngine.Next(pings.Length)]);
                })
            };

            ConsoleCommand Clear = new ConsoleCommand()
            {
                Aliases = new List<string>() { "cls", "clear" },
                Usage = new List<Usage>(),
                Category = "Misc.",
                Description = "Clears the console.",
                Code = new Func<string, string[], Task>(async (message, args) =>
                {
                    Console.Clear();
                    Console.WriteLine("You can enter your commands here.");
                })
            };

            ConsoleCommand ChangeToken = new ConsoleCommand()
            {
                Aliases = new List<string>() { "changetoken", "swaptoken" },
                Usage = new List<Usage>() { new Usage() { Name = "token", ParameterType = ParameterType.Required } },
                Category = "Runtime.",
                Description = $"Hot-swaps token that {Program.Client.CurrentUser.Username} is operating on.",
                Code = new Func<string, string[], Task>(async (context, args) =>
                {
                    if (args.Count() > 0)
                    {
                        if (args[0].ToLower() == "cancel")
                        {
                            Tools.WriteLineWithColor("Cancelled token change.", ConsoleColor.Green);
                            return;
                        }
                        try
                        {
                            var Client = new DiscordSocketClient(new DiscordSocketConfig() { LogLevel = LogSeverity.Info, MessageCacheSize = 10000 });
                            await Client.LoginAsync(TokenType.Bot, args[0]);
                            await Client.LogoutAsync();
                        }
                        catch
                        {
                            Tools.WriteLineWithColor("That token was invalid. Please try again with a different token (the current instance of the bot has not been stopped) or type cancel.", ConsoleColor.Yellow);
                            return;
                        }

                        await new Program().MainAsync(args[0]);
                        Tools.WriteLineWithColor("Successfully changed token.", ConsoleColor.Green);
                    }
                    else
                    {
                        Tools.WriteLineWithColor("Please type in a token to switch execution to.", ConsoleColor.Yellow);
                    }
                })
            };

            ConsoleCommand SetGame = new ConsoleCommand()
            {
                Aliases = new List<string>() { "setgame" },
                Usage = new List<Usage>() { new Usage() { Name = "game name", ParameterType = ParameterType.Required } },
                Category = "Misc.",
                Description = $"Sets the playing status of {Program.Client.CurrentUser.Username}.",
                Code = new Func<string, string[], Task>(async (context, args) =>
                {
                    if (args.Count() > 0)
                    {
                        await Program.Client.SetGameAsync(args[0]);
                        Tools.WriteLineWithColor($"Game has been succesfully set to \"{args[0]}\"", ConsoleColor.Green);
                    }
                    else
                    {
                        Tools.WriteLineWithColor("Please type in a game to change the Playing status to.", ConsoleColor.Yellow);
                    }
                })
            };

            ConsoleCommand SetStatus = new ConsoleCommand()
            {
                Aliases = new List<string>() { "setstatus" },
                Usage = new List<Usage>() { new Usage() { Name = "status", ParameterType = ParameterType.Required } },
                Category = "Misc.",
                Description = $"Sets the status of {Program.Client.CurrentUser.Username} - online, idle, dnd, streaming and offline",
                Code = new Func<string, string[], Task>(async (context, args) =>
                {
                    if (args.Count() > 0)
                    {
                        switch (args[0].ToLower())
                        {
                            case "online":
                                await Program.Client.SetGameAsync(Program.Client.CurrentUser.Game.Value.Name, "", StreamType.NotStreaming);
                                await Program.Client.SetStatusAsync(UserStatus.Online);
                                Tools.WriteLineWithColor($"Status has been succesfully set to Online.", ConsoleColor.Green);
                                break;
                            case "idle":
                            case "afk":
                                await Program.Client.SetGameAsync(Program.Client.CurrentUser.Game.Value.Name, "", StreamType.NotStreaming);
                                await Program.Client.SetStatusAsync(UserStatus.Idle);
                                Tools.WriteLineWithColor($"Status has been succesfully set to Idle.", ConsoleColor.Yellow);
                                break;
                            case "dnd":
                            case "donotdisturb":
                                await Program.Client.SetGameAsync(Program.Client.CurrentUser.Game.Value.Name, "", StreamType.NotStreaming);
                                await Program.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                                Tools.WriteLineWithColor($"Status has been succesfully set to Do Not Disturb.", ConsoleColor.Red);
                                break;
                            case "offline":
                            case "invisible":
                                await Program.Client.SetGameAsync(Program.Client.CurrentUser.Game.Value.Name, "", StreamType.NotStreaming);
                                await Program.Client.SetStatusAsync(UserStatus.Invisible);
                                Tools.WriteLineWithColor($"Status has been succesfully set to Invisible.", ConsoleColor.Gray);
                                break;
                            case "streaming":
                                if (args.Count() == 1)
                                {
                                    Tools.WriteLineWithColor($"For Straming you must input a game.", ConsoleColor.Yellow);
                                }
                                else
                                {
                                    await Program.Client.SetGameAsync(args[1], "https://www.twitch.tv/reflectron", StreamType.Twitch);
                                    Tools.WriteLineWithColor($"Status has been succesfully set to Streaming.", ConsoleColor.DarkMagenta);
                                }
                                break;
                            default:
                                Tools.WriteLineWithColor("Please type in a valid status to change the status to - online, idle, dnd, streaming or offline.", ConsoleColor.Yellow);
                                break;
                        }
                    }
                    else
                    {
                        Tools.WriteLineWithColor("Please type in a status to change the status to.", ConsoleColor.Yellow);
                    }
                })
            };

            ConsoleCommand Help = new ConsoleCommand()
            {
                Aliases = new List<string>() { "help", "halp" },
                Usage = new List<Usage>() { new Usage() { Name = "command", ParameterType = ParameterType.Optional } },
                Category = "Misc.",
                Description = "Lists the possible Discord commands.",
                Code = new Func<string, string[], Task>(async (context, args) =>
                {
                    if (args.Count() == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"{Program.Client.CurrentUser.Username} Help");
                        Console.WriteLine($"This is a list of commands that you can use in the {Program.Client.CurrentUser.Username} Console. Type in help [command name] to get more info on a command.");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        List<string> cats = new List<string>();
                        foreach (var command in Config.ConsoleCommands)
                        {
                            if (!cats.Contains(command.Category))
                                cats.Add(command.Category);
                        }

                        foreach (var cat in cats)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var command in Config.ConsoleCommands.Where(x => x.Category == cat))
                                sb.Append(command.Aliases[0] + Environment.NewLine);
                            Tools.WriteLineWithColor(cat, ConsoleColor.Cyan);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(sb.ToString());
                        }
                    }
                    else
                    {
                        foreach (var arg in args)
                        {
                            var command = Config.ConsoleCommands.FirstOrDefault(x => x.Aliases.Contains(arg));
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
                                            sb2.Append($"<{item.Name}>, ");
                                            break;
                                        case ParameterType.Optional:
                                            sb2.Append($"[{item.Name}], ");
                                            break;
                                        case ParameterType.Infinite:
                                            sb2.Append($"<{item.Name}1>, <{item.Name}2>, <{item.Name}3>...");
                                            break;
                                        case ParameterType.InfiniteOptional:
                                            sb2.Append($"[{item.Name}1], [{item.Name}2], [{item.Name}3]...");
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                Tools.WriteLineWithColor(command.Aliases[0], ConsoleColor.Cyan);
                                Console.WriteLine($"Description: {command.Description}{Environment.NewLine}Category: {command.Category}{Environment.NewLine}Usage: <p.{command.Aliases[0]}> {sb2.ToString()}");
                                Tools.WriteLineWithColor("[optional parameter] <required parameter>", ConsoleColor.DarkGray);
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                            else
                            {
                                Tools.WriteLineWithColor($"A command with the name `{arg}` cannot be found. Please try again with a different command or look at the full command list by typing in `p.help`.", ConsoleColor.Yellow);
                            }
                        }
                    }
                })
            };
        }
    }
}

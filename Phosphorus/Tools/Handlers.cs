using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Phosphorus
{
    public partial class Tools
    {
        //this file won't be touched for a while

		/// <summary>
		/// Default command handler for DiscordCommands. Is linked to the MessageRecieved handler.
		/// </summary>
		/// <param name="message">Message object recieved from the MessageRecieved handler.</param>
        public async static Task HandleDiscordCommand(SocketMessage message)
        {
			await Task.Yield();
            CheckForTriggers(message as SocketUserMessage);
            if (message.Content.StartsWith(Config.Prefix))
            {
                string commandName = string.Empty;
                try { commandName = message.Content.Substring(Config.Prefix.Length, message.Content.IndexOf(' ') - Config.Prefix.Length); }
                catch (ArgumentOutOfRangeException) { commandName = message.Content.Substring(Config.Prefix.Length); }
				//by this point we definitely know that this is a command

				var context = new Discord.Commands.SocketCommandContext(Program.Client, message as SocketUserMessage);
                try
                {
                    var command = Config.DiscordCommands.FirstOrDefault(x => x.Aliases.Contains(commandName));
                    if (command != null)
                    {
						if(PermissionsCheck(message.Author as SocketGuildUser, command.Permissions))
						{
							await command.Code.Invoke(context, GetArgs(message.Content));
						}	
						else
						{
							await context.Channel.SendMessageAsync("", embed: ErrorEmbedCreator("Not enough permission", "You do not have the permission to execute this command."));
						}
					}
                    else
                    {
                        await message.Channel.SendMessageAsync($"A command with the name `{commandName}` does not exist. Please type in `p.help` to display a list of commands.");
                    }
                }
                catch (Exception ex)
                {
                    await context.Channel.SendMessageAsync("", embed: ErrorEmbedCreator(ex));
                    await new Program().Logger(new LogMessage(LogSeverity.Warning, $"Exception during execution of DiscordCommand \"{commandName}\"", ex.Message, ex));
                }
            }
        }

		/// <summary>
		/// Default command handler for ConsoleCommands. Is invoked upon <see cref="Program.Client.Ready"/> and is recursive.
		/// </summary>
		/// <param name="message">Text recieved from <see cref="Console.ReadLine()"/>.</param>
		public async static void HandleConsoleCommand(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string commandName = string.Empty;
            try { commandName = message.Substring(0, message.IndexOf(' ')); }
            catch (ArgumentOutOfRangeException) { commandName = message; }

            try
            {
                await Config.ConsoleCommands.First(command => command.Aliases.Contains(commandName)).Code.Invoke(message, Tools.GetArgs(message));
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"A command with the name `{commandName}` does not exist. Please type in `p.help` to display a list of commands.");
            }
            catch (Exception ex)
            {
                await new Program().Logger(new LogMessage(LogSeverity.Warning, $"Exception during execution of ConsoleCommand \"{commandName}\"", ex.Message, ex));
            }
            Console.ForegroundColor = ConsoleColor.White;
            HandleConsoleCommand(Console.ReadLine());
        }

		/// <summary>
		/// Default command handler for DiscordCommands. Is linked to the MessageRecieved handler.
		/// </summary>
		/// <param name="message">Message object recieved from the MessageRecieved handler.</param>
		/// <param name="requiresPrefix">Boolean, that if set to true will disable the masking of the first few characters associcated with the prefix.</param>
		public static void CheckForTriggers(SocketUserMessage message)
        {
            foreach (var meme in Config.Triggers)
            {
                if (meme.SearchType == Core.TriggerSearchType.FullMessage)
                {
                    if (message.Content.ToLower() == meme.Key.ToLower())
                        message.Channel.SendMessageAsync(meme.Response.Replace("{user}", MentionUtils.MentionUser(message.Author.Id)));
                }
                else
                {
                    if (message.Content.ToLower().Contains(meme.Key.ToLower()))
                        message.Channel.SendMessageAsync(meme.Response.Replace("{user}", MentionUtils.MentionUser(message.Author.Id)));
                }
            }
        }
    }
}
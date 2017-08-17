using Discord;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Phosphorus
{
    class Program
    {
        public static DiscordSocketClient Client;

        static void Main(string[] args)
           => new Program().MainAsync().GetAwaiter().GetResult();

		//gotta make it async!
        public async Task MainAsync(string token = null)
        {
            try
            {
                await Client.LogoutAsync();
            }
            catch
            {
                Debug.WriteLine("Could not log out");
            }
            Console.Clear();
            Client = new DiscordSocketClient(new DiscordSocketConfig() { LogLevel = LogSeverity.Info, MessageCacheSize = 10000 });
            Console.WriteLine("Enter your token:");
            StartAgain:
            try
            {
                await Client.LoginAsync(TokenType.Bot, string.IsNullOrEmpty(token) ? Console.ReadLine() : token);
                Console.Clear();
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("Your token was invalid. Enter your token:");
                goto StartAgain;
            }
            await Client.StartAsync();
            Client.Log += Logger;
			Client.MessageReceived += Core.DirectAccessToken.RaiseTokens;
			Client.MessageReceived += Tools.HandleDiscordCommand;
			Client.ChannelCreated += async (channel) =>
			{
				if (channel is SocketTextChannel)
					await (channel as SocketTextChannel).SendMessageAsync("first");
			};
            Client.Ready += async () =>
            {
                Core.Initialize();
                Console.WriteLine("You can enter commands here.");
                Config.PhosphorusColor = Tools.DominantPicture(Client.CurrentUser);
            };
            Tools.HandleConsoleCommand(Console.ReadLine());
            await Task.Delay(-1);
        }

        public Task Logger(LogMessage message)
        {
            ConsoleColor cc = new ConsoleColor();
            switch (message.Severity)
            {
                case (LogSeverity.Critical & LogSeverity.Error):
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case (LogSeverity.Verbose & LogSeverity.Debug):
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
    }
}
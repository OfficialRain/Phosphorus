using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static Phosphorus.Core;

namespace Phosphorus
{
    public static class Core
    {
        public class DiscordCommand
        {
            public DiscordCommand()
            {
                Config.DiscordCommands.Add(this);
            }
            public List<string> Aliases { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public List<Usage> Usage { get; set; }
            public PermissionLevel PermissionLevel { get; set; }
            public Func<Discord.Commands.SocketCommandContext, string[], Task> Code { get; set; }
        }

        public class ConsoleCommand
        {
            public ConsoleCommand()
            {
                Config.ConsoleCommands.Add(this);
            }
            public List<string> Aliases { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public List<Usage> Usage { get; set; }
            public PermissionLevel PermissionLevel { get; set; }
            public Func<string, string[], Task> Code { get; set; }
        }

        public class Usage
        {
            public ParameterType ParameterType { get; set; }
            public string Name { get; set; }
        }

        public class Trigger
        {
            public Trigger()
            {
                Config.Triggers.Add(this);
            }
            public string Key { get; set; }
            public string Response { get; set; }
            public TriggerSearchType SearchType { get; set; }
        }

        public static void Initialize()
        {
            DiscordCommands.InitializeDiscordCommands();
            ConsoleCommands.InitializeConsoleCommands();
            Triggers.InitializeTriggers();
        }

        public enum ParameterType { Required, Optional, Infinite, InfiniteOptional }
        public enum PermissionLevel { User, Trusted, Mod, Admin, Owner } //i don't really like the int based system of {Program.Client.CurrentUser.Username} 2, and enums are always cool
        public enum TriggerSearchType { FullMessage, SearchFor }
    }

    public static class Config
    {
        public static Collection<DiscordCommand> DiscordCommands = new Collection<DiscordCommand>();
        public static Collection<ConsoleCommand> ConsoleCommands = new Collection<ConsoleCommand>();
        public static Collection<Trigger> Triggers = new Collection<Trigger>();
        public const string Prefix = "p."; //there will be a system to change this later once i implement a TUI
        public static DateTime StartTime = DateTime.Now; //used for getting the uptime
        public static Discord.Color PhosphorusColor = new Discord.Color(0, 0, 0);
        public static Discord.EmbedBuilder HelpEmbed { get; set; }
    }
}

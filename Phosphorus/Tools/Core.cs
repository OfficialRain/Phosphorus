using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Discord.WebSocket;
using static Phosphorus.Core;

namespace Phosphorus
{
    public static class Core
    {
		/// <summary>
		/// Class that encapsulates a command for Discord.
		/// </summary>
        public class DiscordCommand
        {
            public DiscordCommand()
            {
                Config.DiscordCommands.Add(this);
            }
			/// <summary> List of all possible names that can be used to execute the command. </summary>
			public List<string> Aliases { get; set; }
			/// <summary> Description of the command used in the help dialog. </summary>
			public string Description { get; set; }
			/// <summary> Category of the command used in the help dialog. </summary>
			public string Category { get; set; }
			/// <summary> <see cref="List{Usage}"/> that dictates what parameters the command accets. Used in the help dialog (and possibly in the future to pass directly into the command?). </summary>
			public List<Usage> Usage { get; set; }
			/// <summary> Permisson level of the command required to execute it. </summary>
			public PermissionLevel PermissionLevel { get; set; }
			/// <summary> Code for the command to be invoked. </summary>
			public Func<Discord.Commands.SocketCommandContext, string[], Task> Code { get; set; }
        }

		/// <summary>
		/// Class that encapsulates a command for the CLI tools.
		/// </summary>
		public class ConsoleCommand
        {
            public ConsoleCommand()
            {
                Config.ConsoleCommands.Add(this);
            }
			/// <summary> List of all possible names that can be used to execute the command. </summary>
			public List<string> Aliases { get; set; }
			/// <summary> Description of the command used in the help dialog. </summary>
			public string Description { get; set; }
			/// <summary> Category of the command used in the help dialog. </summary>
			public string Category { get; set; }
			/// <summary> <see cref="List{Usage}"/> that dictates what parameters the command accets. Used in the help dialog (and possibly in the future to pass directly into the command?). </summary>
			public List<Usage> Usage { get; set; }
			//no PermissonLevel, you can guess why
			/// <summary> Code for the command to be invoked. </summary>
			public Func<string, string[], Task> Code { get; set; }
        }

		/// <summary>
		/// Class that encaptulates a defenition for a command parameter.
		/// </summary>
        public struct Usage
        {
			/// <summary> <see cref="ParameterType"/> that defines if a parameter is required and if it bulk action. </summary>
			public ParameterType ParameterType { get; set; }
			/// <summary> Name of the parameter. </summary>
			public string Name { get; set; }
        }

		public struct ManageInstance
		{
			public List<SocketUser> ManagedUsers { get; set; }
			public SocketUser Manager { get; set; }
		}

		/// <summary>
		/// Provider that allows commands to access raw messages directly from the chat.
		/// </summary>
		public class DirectAccessToken : IDisposable
		{
			public DirectAccessToken(SocketUser user)
			{
				Config.DirectAccessTokens.Add(this);
				User = user;
			}
			/// <summary> Event handler invoked for each token. </summary
			public event EventHandler AccessTokenInvoked;
			/// <summary> User associated with the token. </summary>
			public SocketUser User;

			/// <summary>
			/// Static method called upon message recieved to check all Direct Access Tokens.
			/// </summary>
			/// <param name="message">Message to be passed into the event</param>
			/// <returns></returns>
			public static async Task RaiseTokens(SocketMessage message)
			{
				try
				{
					foreach (var token in Config.DirectAccessTokens)
						if (message.Author == token.User)
						token.AccessTokenInvoked?.Invoke(message, EventArgs.Empty);
				}
				catch(InvalidOperationException) { }
			}

			#region IDisposable Support
			bool disposed = false;

			private bool disposedValue = false; // To detect redundant calls

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						Config.DirectAccessTokens.Remove(this);
					}

					// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
					// TODO: set large fields to null.
					AccessTokenInvoked = null;

					disposedValue = true;
				}
			}

			// This code added to correctly implement the disposable pattern.
			public void Dispose()
			{
				// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(true);
			}
			#endregion
		}

        public class Trigger
        {
            public Trigger()
            {
                Config.Triggers.Add(this);
            }
			/// <summary> String that can invoke the trigger. </summary>
			public string Key { get; set; }
			/// <summary> Response to when the trigger is invoked. </summary>
			public string Response { get; set; }
			/// <summary> <see cref="TriggerSearchType"/> that dictates whether a portion or the full message should invoke the trigger. </summary>
			public TriggerSearchType SearchType { get; set; }
        }

		/// <summary>
		/// Helper method for initializing commands and triggers.
		/// </summary>
        public static void Initialize()
        {
            DiscordCommands.InitializeDiscordCommands();
            ConsoleCommands.InitializeConsoleCommands();
            Triggers.InitializeTriggers();
        }

		/// <summary> Defines whether a <see cref="Usage"/> is required and whether it supports bulk action.</summary>
		public enum ParameterType { Required, Optional, Infinite, InfiniteOptional }
		/// <summary>	 a permission level for .</summary
		public enum PermissionLevel { User, Mod, Manager, Admin, GuildOwner, ApplicationOwner } //i don't really like the int based system of Phosphorus 2, and enums are always cool
        public enum TriggerSearchType { FullMessage, Contains }
    }
		
	/// <summary>
	/// Global properties for Phosphorus configuration.
	/// </summary>
    public static class Config
    {
		/// <summary> List of all initialized DiscordCommands. </summary>
		public static Collection<DiscordCommand> DiscordCommands = new Collection<DiscordCommand>();
		/// <summary> List of all initialized ConsoleCommands. </summary>
		public static Collection<ConsoleCommand> ConsoleCommands = new Collection<ConsoleCommand>();
		/// <summary> List of all initialized Triggers. </summary>
		public static Collection<Trigger> Triggers = new Collection<Trigger>();
		/// <summary> List of all initialized Direct Access Tokens. </summary>
		public static Collection<DirectAccessToken> DirectAccessTokens = new Collection<DirectAccessToken>();
		/// <summary> List of all instances of a Management action. </summary>
		public static List<ManageInstance> ManageInstances = new List<ManageInstance>();
		/// <summary> Global prefix for DiscordCommands. </summary>
		public const string Prefix = "p."; //there will be a system to change this later once i implement a TUI
		/// <summary> Average color of the current user's profile picture. Used in embeds where a user color is not applicable. </summary>
		public static Discord.Color PhosphorusColor = new Discord.Color(0, 0, 0);
	}
}	

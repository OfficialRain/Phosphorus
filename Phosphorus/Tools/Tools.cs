using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;

namespace Phosphorus
{
    public partial class Tools
    {
		/// <summary>
		/// Creates an <see cref="EmbedBuilder"/> with an error-message feel in case something goes wrong.
		/// </summary>
		/// <param name="title">Title of the embed. Error symbol will be prepended. </param>
		/// <param name="message">Message of the error. Note that markdown is enabled.</param>
		/// <returns> Returns an EmbedBuilder with a custom error message. </returns>
        public static EmbedBuilder ErrorEmbedCreator(string title, string message)
        {
            return new EmbedBuilder
            {
                Title = title,
                Description = message,
                Color = new Discord.Color(240, 71, 71)
            };
        }
		/// <summary>
		/// Convenience method that writes the specified message to the console with a specified color. Resets color to default when done.
		/// </summary>
		/// <param name="message">Message to print to the screen.</param>
		/// <param name="color"><see cref="ConsoleColor"/> that the text will appear in.</param>
		/// <param name="defaultColor">Optional parameter for the color to set the console back to.</param>
		public static void WriteLineWithColor(string message, ConsoleColor color, ConsoleColor defaultColor = ConsoleColor.DarkGray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        } 

		/// <summary>
		/// Convenience method to get the game from an <see cref="IUser"/>.
		/// </summary>
		/// <param name="user"><see cref="IUser"/> to get the game from.</param>
		/// <returns>Returns the name of the game the user is playing.</returns>
        public static string GetGame(IUser user)
        {
            if (user.Game.HasValue)
            {
                return user.Game.Value.Name;
            }
            else
            {
                return "None";
            }
        }

		/// <summary>
		/// Convenience method to get the nickname of a <see cref="SocketGuildUser"/>, or the username if the nickname is <c>null</c>.
		/// </summary>
		/// <param name="user"><see cref="SocketGuildUser"/> to get the display name from.</param>
		/// <returns>Returns a string of the current display name.</returns>
        public static string GetDisplayName(SocketGuildUser user)
        {
            if (string.IsNullOrEmpty(user.Nickname))
                return user.Username;
            else
                return user.Nickname;
        }

		/// <summary>
		/// Convenience method to add each item in a list of <see cref="IUser"/> to a <see cref="List{IUser}"/> only if the user is not present in the list.
		/// </summary>
		/// <param name="list">List to add the users to.</param>
		/// <param name="users">List of users to check and add to the list.</param>
		public static void AddIfDoesntContainUser(List<IUser> list, List<IUser> users)
        {
            foreach (var user in users)
                if (list.FirstOrDefault(x => x.Id == user.Id) == null)
                    list.Add(user);
        }

		/// <summary>
		/// Gets the average color of an <see cref="IUser"/>'s profile picture
		/// </summary>
		/// <param name="user">User to get average color from.</param>
		/// <returns>Returns a <see cref="Discord.Color"/> - NOT a <see cref="System.Drawing.Color"/>!</returns>
		public static Discord.Color DominantPicture(IUser user)
        {
            WebRequest request = WebRequest.Create(
                user.GetAvatarUrl(Discord.ImageFormat.Png, 64));
            WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
                response.GetResponseStream();
            Bitmap bm = new Bitmap(responseStream);


            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;
            long[] totals = new long[] { 0, 0, 0 };
            int width = bm.Width;
            int height = bm.Height;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int color = 0; color < 3; color++)
                        {
                            int idx = (y * stride) + x * 4 + color;

                            totals[color] += p[idx];
                        }
                    }
                }
            }
            int avgB = (int)totals[0] / (width * height);
            if (!(avgB + 15 > 255))
                avgB += 15;
            int avgG = (int)totals[1] / (width * height);
            if (!(avgG + 15 > 255))
                avgG += 20;
            int avgR = (int)totals[2] / (width * height);
            if (!(avgR + 15 > 255))
                avgR += 15;
            return new Discord.Color(avgR, avgG, avgB);
        }
    }
}

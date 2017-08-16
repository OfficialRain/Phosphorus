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
        public static EmbedBuilder ErrorEmbedCreator(string title, string message)
        {
            return new EmbedBuilder
            {
                Title = title,
                Description = message,
                Color = new Discord.Color(240, 71, 71)
            };
        }

        public static void WriteLineWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
        } 

        public static string GetGame(IUser usermini)
        {
            if (usermini.Game.HasValue)
            {
                return usermini.Game.Value.Name;
            }
            else
            {
                return "None";
            }
        }

        public static string GetNicknameOrUsername(SocketGuildUser user)
        {
            if (string.IsNullOrEmpty(user.Nickname))
                return user.Username;
            else
                return user.Nickname;
        }

        public static void AddIfDoesntContainUser(List<IUser> list, List<IUser> users)
        {
            foreach (var user in users)
                if (list.FirstOrDefault(x => x.Id == user.Id) == null)
                    list.Add(user);
        }

        public static void AddIfDoesntContainGuild(List<IGuild> list, List<SocketGuild> users)
        {
            foreach (var user in users)
                if (list.FirstOrDefault(x => x.Id == user.Id) == null)
                    list.Add(user);
        }

        public static Discord.Color DominantPicture(IUser user)
        {
            WebRequest request = WebRequest.Create(
                user.GetAvatarUrl(Discord.ImageFormat.Png, 64));
            WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
                response.GetResponseStream();
            Bitmap bm = new Bitmap(responseStream);


            BitmapData srcData = bm.LockBits(
            new Rectangle(0, 0, bm.Width, bm.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
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

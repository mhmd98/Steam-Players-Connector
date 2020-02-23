using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{
    class SteamIDConverter
    {
        /// <summary>
        /// Checks if the given steam32 id is a valid one
        /// </summary>
        /// <param name="steam32"></param>
        /// <returns>returns true if the id is a valid steam32 otherwise false</returns>
        public static bool IsValidSteam32(string steam32)
        {
            if (!string.IsNullOrWhiteSpace(steam32))
            {
                Regex regex = new Regex(@"STEAM_\d+:\d+:\d+");
                if (regex.IsMatch(steam32))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts steamid from the 32 format (the format used in status RCON command response) to a 64 format (the format used by the steam web API)
        /// </summary>
        /// <param name="steam32">The steam32 id string to convert</param>
        /// <returns>The 64 version of the id as a string on succcess,otherwise null</returns>
        public static string ConvertToSteam64(string steam32)
        {
            if (IsValidSteam32(steam32))
            {
                var split = steam32.Replace("STEAM_", "").Split(':');
                long firstLong;
                long secondLong;
                if (long.TryParse(split[2], out firstLong) && long.TryParse(split[1], out secondLong))
                {
                    return ((76561197960265728 + firstLong * 2) + secondLong).ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{
    /// <summary>
    /// Represents the full info of a player after retriving all the necessery info from the Steam web API
    /// </summary>
    public class FullPlayerInfo
    {
        /// <summary>
        /// The steam id of the player in steam32 format
        /// </summary>
        public string Steam32 { get; set; }
        /// <summary>
        /// The last saved name of the player in the rankme db
        /// </summary>
        public string NameDb { get; set; }
        /// <summary>
        /// The current name of the player taken from the steam web api
        /// </summary>
        public string CurrentName { get; set; }
        /// <summary>
        /// The last known IP of the player taken from the rankme db
        /// </summary>
        public string LastIp { get; set; }
        /// <summary>
        /// a 2 letters ISO country code based on the IP address
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// The steam id of the player in steam64 format
        /// </summary>
        public string Steam64 { get; set; }
        /// <summary>
        /// The profile URL of the playerk
        /// </summary>
        public string SteamUrl { get; set; }
        /// <summary>
        /// Indicates if the player is has been given a community ban on steam (true if banned)
        /// </summary>
        public bool CommunityBanned { get; set; }
        /// <summary>
        /// Indicates if the player has been given a VAC ban (true if banned)
        /// </summary>
        public bool VACBanned { get; set; }

        /// <summary>
        /// Indicates if the player has one or more game bans (true if one or more bans are present)
        /// </summary>
        private bool gameBanned;
        public bool GameBanned
        {
            get { return NumberOfGameBans > 0; }
            set { gameBanned = value; }
        }

        /// <summary>
        /// Indicates the number of game bans the player have on their account
        /// </summary>
        public int NumberOfGameBans { get; set; }

        /// <summary>
        /// Indicates the number of Vac bans the player have on their account
        /// </summary>
        public int NumberOfVACBans { get; set; }
        /// <summary>
        /// Indicates the number of days that have passed since the last ban (either VAC or game ban)
        /// </summary>
        public int DaysSinceLastBan { get; set; }
        
    }
}

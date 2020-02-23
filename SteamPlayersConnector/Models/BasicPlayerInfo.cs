using SQLite;
using System;

namespace SteamPlayersConnector
{
    /// <summary>
    /// Represents a simplifyed version of the player info table in the rankme database
    /// </summary>
    public class BasicPlayerInfo
    {
        /// <summary>
        /// The steam id of the player in steam32 format
        /// </summary>
        [Column("steam")]
        public string Steam32 { get; set; }
        /// <summary>
        /// The last saved name of the player in the rankme db
        /// </summary>
        [Column("name")]
        public string NameDB { get; set; }
        /// <summary>
        /// The last known IP of the player taken from the rankme db
        /// </summary>
        public string LastIp { get; set; }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{

    /// <summary>
    /// A class representation of the data recieved when calling GetPlayerBans method on the Steam web API
    /// </summary>
    [JsonObject(Title = "Player")]
    public class PlayersBanInfo
    {
        public string SteamId { get; set; }
        public bool CommunityBanned { get; set; }
        public bool VACBanned { get; set; }
        public int NumberOfVACBans { get; set; }
        public int DaysSinceLastBan { get; set; }
        public int NumberOfGameBans { get; set; }
        public string EconomyBan { get; set; }
    }
    [JsonObject(Title = "RootObject")]
    public class RootBanObject
    {
        public List<PlayersBanInfo> players { get; set; }
    }
}

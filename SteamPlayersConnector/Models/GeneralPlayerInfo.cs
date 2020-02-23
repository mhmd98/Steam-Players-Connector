using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{
    /// <summary>
    /// A class representation of the general players data received when calling GetPlayerSummaries method on the Steam web API
    /// </summary>
    [JsonObject(Title = "Player")]
    public class GeneralPlayerInfo
    {
        public string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public int lastlogoff { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public int personastate { get; set; }
        public string primaryclanid { get; set; }
        public int timecreated { get; set; }
        public int personastateflags { get; set; }
    }
    public class Response
    {
        public List<GeneralPlayerInfo> players { get; set; }
    }
    [JsonObject(Title = "RootObject")]
    public class GeneralPlayerRootObject
    {
        public Response response { get; set; }
    }
}

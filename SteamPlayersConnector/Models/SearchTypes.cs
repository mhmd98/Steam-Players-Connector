using SteamPlayersConnector.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SearchTypes
    {
        [Description("IP")]
        IP,
        [Description("Name In DB")]
        NameDB,
        [Description("Current Name")]
        CurrentName
    }
}

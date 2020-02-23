using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{
    /// <summary>
    /// A class representation of the parsed data in the geoip database
    /// </summary>
    public class GeoIPInfo
    {

        static List<GeoIPInfo> GeoIPList { get; set; }

        public uint IPStart { get; set; }
        public uint IPEnd { get; set; }
        public string CountryCode { get; set; }



        public static GeoIPInfo ParseCSV(string csvLine)
        {
            string[] values = csvLine.Split(',');
            GeoIPInfo geoIPInfo = new GeoIPInfo();
            geoIPInfo.IPStart = Convert.ToUInt32(values[0]);
            geoIPInfo.IPEnd = Convert.ToUInt32(values[1]);
            geoIPInfo.CountryCode = values[2];
            return geoIPInfo;
        }


        public static string GetCountryCodeFromIP(List<GeoIPInfo> geoIPInfoList, string ip)
        {
            if (!string.IsNullOrWhiteSpace(ip))
            {
                IPAddress IP;
                if (IPAddress.TryParse(ip, out IP))
                { 
                    byte[] bytes = IP.GetAddressBytes();
                    Array.Reverse(bytes); // flip big-endian(network order) to little-endian
                    uint intAddress = BitConverter.ToUInt32(bytes, 0);
                    foreach (var Info in geoIPInfoList)
                    {
                        if (intAddress >= Info.IPStart && intAddress <= Info.IPEnd)
                        {
                            return Info.CountryCode;
                        }
                    }
                    return "-";
                }
            }
            return "-";
        }
    }




}



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector.Classes
{
    class IPConverter
    {

        public string GetIPGeo(IPAddress ipAddress)
        {
            byte[] bytes = ipAddress.GetAddressBytes();
            Array.Reverse(bytes); // big-endian(network order) to little-endian
            uint intAddress = BitConverter.ToUInt32(bytes, 0);
            //var geoIP = new GeoIPInfo();
            var file = File.Open("IP2Geo.bin", FileMode.Open); // open geoip binary db
            var fileSize = file.Length; //  get file size to use in binary search
            using (var reader = new BinaryReader(file)) 
            {

                uint IPStart = reader.ReadUInt32(); //read the first ip range

                // binary search

                int minNum = 1;
                long maxNum = fileSize*10;

                while (minNum <= maxNum)
                {
                    int mid = (minNum + maxNum) / 2;
                    if (key == arr[mid])
                    {
                        return ++mid;
                    }
                    else if (key < arr[mid])
                    {
                        max = mid - 1;
                    }
                    else
                    {
                        min = mid + 1;
                    }
                }
                return "-";

            }
        }


        IPAddress ParseIP(string ipAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                return ip;

            }
            else
            {
                return null;
            }
        }
    }
}

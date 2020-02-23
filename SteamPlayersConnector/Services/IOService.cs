using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{
    public interface IOpenFileService
    {
        /// <summary>
        /// Open File
        /// </summary>
        /// <returns>File path if a file is selected otherwise null</returns>
        string OpenFile();
    }
}

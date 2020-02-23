using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayersConnector
{
    public class OpenFileService : IOpenFileService
    {

        /// <summary>
        /// Opens a file using the FileDialog
        /// </summary>
        /// <returns>file path if a file was choosing, empty string if the dialog was canceled and null on error</returns>
        public string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && !result.Value)
            {
                return string.Empty;
            }else if(result.HasValue && result.Value)
            {
                return openFileDialog.FileName;
            }
            return null;

        }



    }
}

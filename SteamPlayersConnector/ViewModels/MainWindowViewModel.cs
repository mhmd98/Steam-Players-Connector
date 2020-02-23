using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Windows;
using SQLite;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Configuration;

namespace SteamPlayersConnector.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        #region commands declerations
        public DelegateCommand<DragEventArgs> LoadDroppedFileCommand { get; private set; }
        public DelegateCommand LoadFileCommand { get; private set; }
        public DelegateCommand LoadNewFileCommand { get; private set; }
        public DelegateCommand ShowBannedOnlyCommand { get; private set; }
        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand LookUpInfoCommand { get; private set; }
        public DelegateCommand<string> OpenLinkCommand { get; private set; }
        #endregion

        #region properties


        private string _title = "Steam Players Connector";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private IOpenFileService _openFileService;

        private ObservableCollection<FullPlayerInfo> fullPlayersList;
        public ObservableCollection<FullPlayerInfo> FullPlayersList
        {
            get { return fullPlayersList; }
            set { SetProperty(ref fullPlayersList, value); }
        }
        private ObservableCollection<FullPlayerInfo> filteredPlayersList;
        public ObservableCollection<FullPlayerInfo> FilteredPlayersList
        {
            get { return filteredPlayersList; }
            set { SetProperty(ref filteredPlayersList, value); }
        }

        private ObservableCollection<FullPlayerInfo> searchPlayersList;
        private ObservableCollection<FullPlayerInfo> filterdListBackup;

        private bool isSearchButtonLoading;
        public bool IsSearchButtonLoading
        {
            get { return isSearchButtonLoading; }
            set { SetProperty(ref isSearchButtonLoading, value); }
        }


        private bool isBannedOnly;
        public bool IsBannedOnly
        {
            get { return isBannedOnly; }
            set { SetProperty(ref isBannedOnly, value); }
        }

        private bool isFileLoaded = false;
        public bool IsFileLoaded
        {
            get { return isFileLoaded; }
            set { SetProperty(ref isFileLoaded, value); }
        }

        public bool CanBrowse
        {
            get { return !IsFileLoaded; }
        }

        private string dbPath;
        public string DbPath
        {
            get { return dbPath; }
            set { SetProperty(ref dbPath, value); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private bool isProgressBarIndeterminate;
        public bool IsProgressBarIndeterminate
        {
            get { return isProgressBarIndeterminate; }
            set { SetProperty(ref isProgressBarIndeterminate, value); }
        }

        private int loadingProgressValue;
        public int LoadingProgressValue
        {
            get { return loadingProgressValue; }
            set { SetProperty(ref loadingProgressValue, value); }
        }

        private string loadingText;
        public string LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
        }

        public List<GeoIPInfo> GeoIPinfoList { get; set; }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { SetProperty(ref searchText, value); }
        }


        public string SteamKeyText
        {
            get { return Properties.Settings.Default.steamKey; }
            set
            {
                Properties.Settings.Default["steamKey"] = value;
                Properties.Settings.Default.Save();
            }
        }


        private DataGridCellInfo selectedCell;
        public DataGridCellInfo SelectedCell
        {
            get { return selectedCell; }
            set { SetProperty(ref selectedCell, value); }
        }

        private string playersListCount;
        public string PlayersListCount
        {
            get
            {
                if (FilteredPlayersList != null)
                {
                    return FilteredPlayersList.Count.ToString();
                }
                return "";
            }
        }


        private bool isRowSelected;
        public bool IsRowSelected
        {
            get { return isRowSelected; }
            set { SetProperty(ref isRowSelected, value); }
        }

        #endregion






        public MainWindowViewModel(IOpenFileService openFileService)
        {
            _openFileService = openFileService;
            LoadFileCommand = new DelegateCommand(LoadFile).ObservesCanExecute(() => CanBrowse);
            LoadDroppedFileCommand = new DelegateCommand<DragEventArgs>(LoadDroppedFile).ObservesCanExecute(() => CanBrowse);
            LoadNewFileCommand = new DelegateCommand(LoadNewFile).ObservesCanExecute(() => IsFileLoaded);
            OpenLinkCommand = new DelegateCommand<string>(OpenLink);
            OpenLinkCommand = new DelegateCommand<string>(OpenLink);
            ShowBannedOnlyCommand = new DelegateCommand(ShowBannedOnly);
            LookUpInfoCommand = new DelegateCommand(LookUpInfo);
            SearchCommand = new DelegateCommand(Search).ObservesCanExecute(() => IsFileLoaded);
            ClearCommand = new DelegateCommand(Clear).ObservesCanExecute(() => IsFileLoaded);
            // read the csv file line by line, parse and convert to geoip, and finaly add to list
            GeoIPinfoList = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "IP2LOCATION-LITE.CSV")).Select(v => GeoIPInfo.ParseCSV(v)).ToList(); // no checks for failure, if this fail then the program should 
        }


        #region commands methods
        async void LoadDroppedFile(DragEventArgs e)
        {
            if (e != null)
            {
                dbPath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                await LoadDataGrid();
            }
            else
            {
                MessageBox.Show("filed to load the file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        async void LoadFile()
        {
            var path = _openFileService.OpenFile();
            if (path == null)
            {
                MessageBox.Show("filed to load the file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (path != string.Empty)
                {
                    dbPath = path;
                    await LoadDataGrid();
                }
            }
        }

        void OpenLink(string url)
        {
            Process.Start(url);
        }

        void LoadNewFile()
        {
            IsFileLoaded = false;
        }

        void ShowBannedOnly()
        {
            if (isBannedOnly)
            {
                filterdListBackup = new ObservableCollection<FullPlayerInfo>(FilteredPlayersList);
                FilteredPlayersList = new ObservableCollection<FullPlayerInfo>(FilteredPlayersList.Where((x) => x.VACBanned == isBannedOnly || x.GameBanned == isBannedOnly).ToList());
            }
            else
            {
                FilteredPlayersList = filterdListBackup;
            }
            RaisePropertyChanged(nameof(PlayersListCount));
        }

        void LookUpInfo()
        {
            if (selectedCell.Column != null && selectedCell.Item is FullPlayerInfo)
            {
                var propertyName = selectedCell.Column.SortMemberPath.ToString();
                var z = selectedCell.Item.GetType().GetProperty(propertyName).PropertyType;
                if (!string.IsNullOrEmpty(propertyName) && z == typeof(string))
                {
                    SearchText = selectedCell.Item.GetType().GetProperty(propertyName).GetValue(selectedCell.Item, null).ToString();
                    searchPlayersList = new ObservableCollection<FullPlayerInfo>(FullPlayersList.Where((x) =>
                      x.GetType().GetProperty(propertyName).GetValue(x, null).ToString().ToLower().Contains(searchText.ToLower())));
                    FinishSearch();
                }
            }
        }

        void FinishSearch()
        {
            if (isBannedOnly)
            {
                filterdListBackup = searchPlayersList;
                FilteredPlayersList = new ObservableCollection<FullPlayerInfo>(searchPlayersList.Where((x) => x.VACBanned == true || x.GameBanned == true).ToList());
            }
            else
            {
                FilteredPlayersList = searchPlayersList;
                filterdListBackup = FilteredPlayersList;
            }

            RaisePropertyChanged(nameof(PlayersListCount));
            IsRowSelected = false;
        }

        void Clear()
        {
            SearchText = "";
            FilteredPlayersList = FullPlayersList;
            IsBannedOnly = false;
            RaisePropertyChanged(nameof(PlayersListCount));
        }

        void Search()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                searchText = searchText.ToLower();
                var type = fullPlayersList.GetType().GetGenericArguments()[0];
                var properties = type.GetProperties();
                searchPlayersList = new ObservableCollection<FullPlayerInfo>(
                    FullPlayersList.Where((x) =>
                 properties.Any(p =>
                  {
                      var value = p.GetValue(x);
                      return value != null && value.ToString().ToLower().Contains(searchText);
                  })));
                FinishSearch();
            }
        }
        #endregion


        async Task LoadDataGrid()
        {
            //reset the fields
            filterdListBackup = new ObservableCollection<FullPlayerInfo>();
            searchPlayersList = new ObservableCollection<FullPlayerInfo>();
            FullPlayersList = new ObservableCollection<FullPlayerInfo>();
            FilteredPlayersList = new ObservableCollection<FullPlayerInfo>();
            IsBannedOnly = false;
            SearchText = "";

            // configure the progress bar and show it
            LoadingText = "";
            IsProgressBarIndeterminate = true;
            IsBusy = true;

            try
            {
                await PopulateDataGrid();
            }
            catch (SQLiteException)
            {
                MessageBox.Show("The program encountered an error when trying to process the data in the Database file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("The program couldn't retrieve the info from the steam web API", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("The program encountered an unexpected error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Opens the file on dbpath, retrives and converts the info in the db and sends requests to the api for more info. Then populates the datagrid with the info
        /// Throws exception depending on the kind of failure
        /// </summary>
        /// <returns></returns>
        async Task PopulateDataGrid()
        {
            try
            {
                using (var sqliteConnection = new SQLiteConnection(dbPath))
                {
                    var list = sqliteConnection.Query<BasicPlayerInfo>("Select * From rankme");
                    FullPlayersList = new ObservableCollection<FullPlayerInfo>(await GetInfoFromSteamAPI(list));
                    FilteredPlayersList = fullPlayersList;
                    IsFileLoaded = true;
                }
                RaisePropertyChanged(nameof(PlayersListCount));
            }
            catch (Exception ex)
            {
                IsFileLoaded = false;
                throw ex;
            }
        }




        public async Task<List<FullPlayerInfo>> GetInfoFromSteamAPI(List<BasicPlayerInfo> basicPlayersInfoList)
        {
            var httpClient = new HttpClient();

            var baseUrl = @"https://api.steampowered.com/ISteamUser/{0}/{1}/?steamids={2}&key={3}";

            var generalInfoList = new List<GeneralPlayerInfo>(); // create a list for the data of the first request (general info about the player)
            var banInfoList = new List<PlayersBanInfo>(); // create a list for the data of the second request (the player's ban information)

            var subLists = new List<List<BasicPlayerInfo>>(); // a list to hold the result of splitting the main list into 100-items sized lists (steam api restrection on IDs per request)

            for (int i = 0; i < basicPlayersInfoList.Count; i += 100)
            {
                subLists.Add(basicPlayersInfoList.GetRange(i, Math.Min(100, basicPlayersInfoList.Count - i)));
            }

            IsProgressBarIndeterminate = false;
            LoadingText = "Retriving info from the Steam Web API";
            // loop through the sublists, extract the IDs, convert them then save them to the IDs string
            foreach (var list in subLists)
            {
                LoadingProgressValue = (subLists.IndexOf(list) * 100) / subLists.Count();
                string IDs = "";

                foreach (var player in list)
                {

                    string id = SteamIDConverter.ConvertToSteam64(player.Steam32);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        if (list.IndexOf(player) < list.Count - 1)
                        {
                            IDs += id + ",";
                        }
                        else
                        {
                            IDs += id;
                        }

                    }
                }


                var formattedUrl = string.Format(baseUrl, "GetPlayerSummaries", "v2", IDs, SteamKeyText);
                var generalInfoResponse = await httpClient.GetAsync(formattedUrl);
                if (generalInfoResponse.IsSuccessStatusCode)
                {
                    var content = await generalInfoResponse.Content.ReadAsStringAsync();
                    var generalPlayerRootList = JsonConvert.DeserializeObject<GeneralPlayerRootObject>(content);
                    generalInfoList.AddRange(generalPlayerRootList.response.players);
                }
                else
                {
                    throw new HttpRequestException();
                }

                formattedUrl = string.Format(baseUrl, "GetPlayerBans", "v1", IDs, SteamKeyText);
                var banInfoResponse = await httpClient.GetAsync(formattedUrl);
                if (banInfoResponse.IsSuccessStatusCode)
                {
                    var content = await banInfoResponse.Content.ReadAsStringAsync();
                    var generalPlayerRootList = JsonConvert.DeserializeObject<RootBanObject>(content);
                    banInfoList.AddRange(generalPlayerRootList.players);
                }
                else
                {
                    throw new HttpRequestException();
                }


            }
            // pick the relevant info from the general info list and the ban list and combine them to a complete info list using the steamid (in 64 format)
            var FullPlayersInfoList = (from generalInfo in generalInfoList
                                       join banInfo in banInfoList
                                        on generalInfo.steamid equals banInfo.SteamId
                                       select new
                                       {
                                           generalInfo.steamid,
                                           generalInfo.personaname,
                                           generalInfo.profileurl,
                                           banInfo.CommunityBanned,
                                           banInfo.VACBanned,
                                           banInfo.NumberOfGameBans,
                                           banInfo.NumberOfVACBans,
                                           banInfo.DaysSinceLastBan
                                       } into intermediateList
                                       join basicInfo in basicPlayersInfoList
                                       on intermediateList.steamid equals SteamIDConverter.ConvertToSteam64(basicInfo.Steam32)
                                       orderby intermediateList.personaname
                                       select new FullPlayerInfo()
                                       {
                                           Steam32 = basicInfo.Steam32, // the id used in the full list is 32 format
                                           NameDb = basicInfo.NameDB,
                                           CurrentName = intermediateList.personaname,
                                           LastIp = basicInfo.LastIp,
                                           Steam64 = intermediateList.steamid,
                                           SteamUrl = intermediateList.profileurl,
                                           CommunityBanned = intermediateList.CommunityBanned,
                                           VACBanned = intermediateList.VACBanned,
                                           NumberOfGameBans = intermediateList.NumberOfGameBans,
                                           NumberOfVACBans = intermediateList.NumberOfVACBans,
                                           DaysSinceLastBan = intermediateList.DaysSinceLastBan
                                       }).ToList();

            // set the country code for each player in the full list to later be able to view it in the datagrid
            foreach (var player in FullPlayersInfoList)
            {
                player.CountryCode = GeoIPInfo.GetCountryCodeFromIP(GeoIPinfoList, player.LastIp);
            }


            return FullPlayersInfoList;
        }


    }
}


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SportsController.Shared;

namespace SportsController.Basketball
{
    public partial class FormBasketball : Form
    {
        #region Variables

        Xpression _Xpression;
        readonly string xpressionDataFilePath = Environment.CurrentDirectory + "\\config\\bb\\xpressionData.json";
        readonly string scoreBridgeDataFilePath = Environment.CurrentDirectory + "\\config\\bb\\scoreBridgeData.json";
        readonly string eventDataFilePath = Environment.CurrentDirectory + "\\config\\bb\\eventData.json";
        readonly string customsDataFilePath = Environment.CurrentDirectory + "\\config\\bb\\customsData.json";
        string eventDataDirectoryPath = Environment.CurrentDirectory + "\\data\\bb";
        Scoreboard scoreboardData;
        ScoreBridge scoreBridge;
        List<TakeItem> takeItems;
        Timer _creditsTmr = new Timer();
        Timer _infoTmr = new Timer();
        bool _autoPoint = true;
        int _credits_namesPerPage = 0;
        int _credits_rowCount = 0;
        int _credits_rowIndex = 0;
        bool _shotClock_enabled = false;
        int _home_roster_id = 0;
        int _home_coach_id = 0;
        int _away_roster_id = 1;
        int _away_coach_id = 1;

        #endregion

        public FormBasketball()
        {
            InitializeComponent();
        }

        ~FormBasketball()
        {
            if (_creditsTmr != null)
                _creditsTmr.Dispose();
            if (_infoTmr != null)
                _infoTmr.Dispose();
        }

        private void FormBasketball_Load(object sender, EventArgs e)
        {
            SelectDirectory();

            // Create the defaults.
            _Xpression = new Xpression();
            XpressionData xpressionData = new XpressionData();
            EventData eventData = new EventData();
            scoreboardData = new Scoreboard();
            scoreBridge = new ScoreBridge(this);
            takeItems = new List<TakeItem>();

            // Assign SelectedObject.
            propGridXpression.SelectedObject = xpressionData;
            propGridXpression.CollapseAllGridItems();
            propGridEvent.SelectedObject = eventData;
            propGridEvent.CollapseAllGridItems();

            LoadCustomsData();
            LoadEventData();
            LoadXpressionData();
            LoadScoreBridgenData();
            btnShotClockDisable.PerformClick();
        }

        #region Main Functions

        private bool SelectDirectory()
        {
            bool response = false;

            using (FrmSelector frmSelector = new FrmSelector(eventDataDirectoryPath))
            {
                DialogResult frmResult = frmSelector.ShowDialog();
                if (frmResult == DialogResult.OK && !frmSelector.Canceled)
                {
                    response = true;
                    string directory = frmSelector.SelectedDirectory;
                    if (!directory.Equals(string.Empty)) eventDataDirectoryPath = directory;
                }
            }

            return response;
        }

        public string Truncate(string text, int maxLength, string suffix = "...")
        {
            string str = text;
            if (maxLength > 0)
            {
                int length = maxLength - suffix.Length;
                if (length <= 0)
                {
                    return str;
                }
                if ((text != null) && (text.Length > maxLength))
                {
                    return (text.Substring(0, length).TrimEnd(new char[0]) + suffix);
                }
            }
            return str;
        }

        static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.LastIndexOf(Find);
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public string GetMessageChunks(string text)
        {
            string[] _chunks = text.Wrap(90);
            string str;
            if (_chunks.Length == 1)
            {
                str = string.Format("{0}", _chunks[0]);
            }
            else if (_chunks.Length == 2)
            {
                str = string.Format("{0}\n{1}", _chunks[0], _chunks[1]);
            }
            else
            {
                str = string.Format("{0}\n{1}\n{2}", _chunks[0], _chunks[1], Truncate(string.Join(" ", _chunks.Skip(2)), 90));
            }
            return str;
        }

        public void SaveDataTable(DataTable dataTable, string path, int index = 0)
        {
            // Make sure a path was given
            if (string.IsNullOrWhiteSpace(path))
            {
                // Exit out of loading as we don't have all paths needed
                return;
            }

            // Save the data to a .xlsx
            Excel.SaveDataTable(dataTable, path, index);
        }

        public void SaveDataTable(DataTable dataTable, string path, string teamName, int index = 0)
        {
            // Make sure a path and team name was given
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(teamName) ||
                teamName.Trim().Equals("-Select-"))
            {
                // Exit out of loading as we don't have all paths needed
                return;
            }

            // Save the data to a CSV
            Excel.SaveDataTable(dataTable, path + "\\" + teamName + ".xlsx", index);
        }

        public List<TakeItem> GetAllTakeItemsOnLayer(int layer)
        {
            return takeItems.Where(item => item.Layer == layer).ToList();
        }

        public List<TakeItem> GetAllTakeItemsOnLayerExcept(int layer, int id)
        {
            return takeItems.Where(item => item.Layer == layer && item.ID != id).ToList();
        }

        public List<int> GetAllTakeIdsOnLayer(int layer)
        {
            return takeItems.Where(item => item.Layer == layer).Select(pair => pair.ID).ToList();
        }

        public void SetTakeItemsOffline(List<TakeItem> items)
        {
            foreach (TakeItem takeItem in items)
                takeItem.SetOffline();
        }

        public void SetTakeIdsOffline(List<int> takeIDs)
        {
            foreach (int takeID in takeIDs)
                _Xpression.SetTakeItemOffline(takeID);
        }

        public void SetLayerTakeIdsOffline(int layer)
        {
            // First get all of the take items on the layer
            List<TakeItem> takeItemss = GetAllTakeItemsOnLayer(layer);
            // Set all of the take itenms on this layer offline
            SetTakeItemsOffline(takeItemss);
        }

        public void SetLayerTakeIdsOfflineExcept(int layer, int id)
        {
            // First get all of the take items on the layer except the selected one
            List<TakeItem> takeItemss = GetAllTakeItemsOnLayerExcept(layer, id);
            // Set all of the other take itenms on this layer offline
            SetTakeItemsOffline(takeItemss);
        }

        // Load data for the scenes config window
        public void LoadXpressionData()
        {
            try
            {
                if (File.Exists(xpressionDataFilePath))
                {
                    using StreamReader r = new StreamReader(xpressionDataFilePath);
                    string json = r.ReadToEnd();
                    JObject obj = JObject.Parse(json);
                    JsonSerializer serializer = new JsonSerializer();
                    propGridXpression.SelectedObject = (XpressionData)serializer.Deserialize(new JTokenReader(obj), typeof(XpressionData));
                    propGridXpression.CollapseAllGridItems();
                    LoadFromXpression();
                }
                else
                {
                    SaveXpressionData();
                }
                scoreboardData.Reset();
                populateScoreboard();
            }
            catch { }
        }

        // Save the Xpression data to the config file
        public void SaveXpressionData()
        {
            object data = propGridXpression.SelectedObject;
            if (data != null)
            {
                using StreamWriter file = File.CreateText(xpressionDataFilePath);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data);
                // Reload all data from Xpression based off our new config
                LoadFromXpression();
            }
        }

        // Load data for the ScoreBridge config window
        public void LoadScoreBridgenData()
        {
            try
            {
                if (File.Exists(scoreBridgeDataFilePath))
                {
                    using StreamReader r = new StreamReader(scoreBridgeDataFilePath);
                    string json = r.ReadToEnd();
                    JObject obj = JObject.Parse(json);
                    JsonSerializer serializer = new JsonSerializer();
                    propertyGridScoreBridge.SelectedObject = (ScoreBridgeData)serializer.Deserialize(new JTokenReader(obj), typeof(ScoreBridgeData));
                    Invoke(new Action(() =>
                    {
                        scoreBridge.Update(propertyGridScoreBridge.SelectedObject);
                        if (scoreBridge.IsLive)
                        {
                            liveSyncToolStripMenuItem.Text = "ScoreBridge Sync ON";
                            liveSyncToolStripMenuItem.ForeColor = System.Drawing.Color.DarkGreen;
                        }
                        else
                        {
                            liveSyncToolStripMenuItem.Text = "ScoreBridge Sync OFF";
                            liveSyncToolStripMenuItem.ForeColor = System.Drawing.Color.DarkRed;
                        }
                        if (scoreBridge.URL == "") liveSyncToolStripMenuItem.Enabled = false;
                        else liveSyncToolStripMenuItem.Enabled = true;
                    }));
                }
                else
                {
                    SaveScoreBridgeData();
                }
            }
            catch { }
        }


        // Save ScoreBridge data to config file
        public void SaveScoreBridgeData()
        {
            object data = propertyGridScoreBridge.SelectedObject;
            if (data != null)
            {
                using StreamWriter file = File.CreateText(scoreBridgeDataFilePath);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data);
                Invoke(new Action(() =>
                {
                    if (scoreBridge.Update(propertyGridScoreBridge.SelectedObject))
                    {
                        liveSyncToolStripMenuItem.Text = "ScoreBridge Sync ON";
                        liveSyncToolStripMenuItem.ForeColor = System.Drawing.Color.DarkGreen;
                    }
                    else
                    {
                        liveSyncToolStripMenuItem.Text = "ScoreBridge Sync OFF";
                        liveSyncToolStripMenuItem.ForeColor = System.Drawing.Color.DarkRed;
                    }
                    if (scoreBridge.URL == "") liveSyncToolStripMenuItem.Enabled = false;
                    else liveSyncToolStripMenuItem.Enabled = true;
                }));
            }
        }

        // Load data for the event config window
        public void LoadEventData()
        {
            try
            {
                if (File.Exists(eventDataFilePath))
                {
                    using StreamReader r = new StreamReader(eventDataFilePath);
                    string json = r.ReadToEnd();
                    JObject obj = JObject.Parse(json);
                    JsonSerializer serializer = new JsonSerializer();
                    propGridEvent.SelectedObject = (EventData)serializer.Deserialize(new JTokenReader(obj), typeof(EventData));
                    propGridEvent.CollapseAllGridItems();
                    LoadFromDataFiles();
                }
                else
                {
                    SaveEventData();
                }
            }
            catch { }
        }

        // Save the event data to the config file
        public void SaveEventData()
        {
            object data = propGridEvent.SelectedObject;
            if (data != null)
            {
                using StreamWriter file = File.CreateText(eventDataFilePath);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data);
                LoadFromDataFiles();
                SyncEventData();
            }
        }

        public void LoadFromDataFiles()
        {
            try
            {
                // Make sure a path exists
                if (string.IsNullOrWhiteSpace(eventDataDirectoryPath))
                {
                    return;
                }

                LoadTeamsData();
                LoadL3Setup();
                LoadCreditsData();

                // Watch for system changes in the data directory
                fileSystemWatcherData = new FileSystemWatcher(eventDataDirectoryPath);
                fileSystemWatcherData.Changed += new FileSystemEventHandler(fileSystemWatcherData_Changed);
                fileSystemWatcherData.EnableRaisingEvents = true;
                fileSystemWatcherData.IncludeSubdirectories = true;
            }
            catch { }
        }

        public void fileSystemWatcherData_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fullPath = e.FullPath;
                string fileName = e.Name;
                WatcherChangeTypes changeType = e.ChangeType;
                // Only deal with changed or created files
                if (changeType == WatcherChangeTypes.Changed || changeType == WatcherChangeTypes.Created)
                {
                    Invoke(new Action(() =>
                    {
                        // Make sure a path exists
                        if (string.IsNullOrWhiteSpace(eventDataDirectoryPath))
                            return;

                        string _rostersFile = "Rosters.xlsx";
                        string _coachesFile = "Coaches.xlsx";
                        string _statsFile = "Stats.xlsx";
                        string _creditsFile = "Credits.xlsx";
                        string _lowerThirdsFile = "LowerThirds.xlsx";

                        if (fileName.ToLower().Equals(_rostersFile.ToLower()) || fileName.ToLower().Equals(_coachesFile.ToLower()) ||
                            fileName.ToLower().Equals(_statsFile.ToLower()))
                        {
                            LoadTeamsData();
                        }
                        else if (fileName.ToLower().Equals(_creditsFile.ToLower()))
                        {
                            LoadCreditsData();
                        }
                        else if (fileName.ToLower().Equals(_lowerThirdsFile.ToLower()))
                        {
                            LoadL3Setup();
                        }
                    }));
                }
            }
            catch { }
        }

        // Load data for the custom variables config window
        public void LoadCustomsData()
        {
            try
            {
                if (File.Exists(customsDataFilePath))
                {
                    using StreamReader r = new StreamReader(customsDataFilePath);
                    string json = r.ReadToEnd();
                    Dictionary<string, string> _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    dataGridCustoms.Columns.Clear();
                    dataGridCustoms.Columns.Add("Name", "Variable Name");
                    dataGridCustoms.Columns.Add("Value", "Variable Value");

                    foreach (KeyValuePair<string, string> item in _data)
                    {
                        dataGridCustoms.Rows.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    SaveCustomsData();
                }
            }
            catch { }
        }

        // Save the custom variables to the config file
        public void SaveCustomsData()
        {
            dataGridCustoms.RemoveEmptyRows();
            Dictionary<string, string> data = new Dictionary<string, string>();
            try
            {
                // Go through all properties to create table data
                foreach (DataGridViewRow row in dataGridCustoms.Rows)
                {
                    if (row.Cells.Count > 0)
                        data.Add((string)row.Cells["Name"].Value, (string)row.Cells["Value"].Value);
                }
            }
            catch { }
            finally
            {
                if (data.Count > 0)
                {
                    using StreamWriter file = File.CreateText(customsDataFilePath);
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, data);
                }
            }
        }


        // Load teams data from given paths
        public void LoadTeamsData()
        {
            try
            {
                // Check for team data locations
                string _rostersFile = string.Format("{0}\\Rosters.xlsx", eventDataDirectoryPath);
                string _statsFile = string.Format("{0}\\Stats.xlsx", eventDataDirectoryPath);
                string _coachesFile = string.Format("{0}\\Coaches.xlsx", eventDataDirectoryPath);
                // Get names for each team from dropdowns
                string _homeTeam = Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject).Trim();
                string _awayTeam = Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject).Trim();

                // Check if any of the data doesn't have a path set and if a home and away team are selected
                if (string.IsNullOrWhiteSpace(eventDataDirectoryPath) ||
                    string.IsNullOrWhiteSpace(_statsFile) ||
                    string.IsNullOrWhiteSpace(_coachesFile) ||
                    string.IsNullOrWhiteSpace(_homeTeam) || string.IsNullOrWhiteSpace(_awayTeam) ||
                    _homeTeam.Trim().Equals("-Select-") || _awayTeam.Trim().Equals("-Select-"))
                {
                    // Exit out of loading as we don't have all paths needed
                    return;
                }

                Dictionary<int, string> rosterWorkSheet = Excel.GetWorksheets(_rostersFile);
                if (rosterWorkSheet.Count > 1)
                {
                    try
                    {
                        _home_roster_id = rosterWorkSheet.Where(item => item.Value.ToLower() == _homeTeam.ToLower()).First().Key;
                        _away_roster_id = rosterWorkSheet.Where(item => item.Value.ToLower() == _awayTeam.ToLower()).First().Key;
                    }
                    catch { }
                }
                Dictionary<int, string> coachWorkSheet = Excel.GetWorksheets(_coachesFile);
                if (rosterWorkSheet.Count > 1)
                {
                    try
                    {
                        _home_coach_id = rosterWorkSheet.Where(item => item.Value.ToLower() == _homeTeam.ToLower()).First().Key;
                        _away_coach_id = rosterWorkSheet.Where(item => item.Value.ToLower() == _awayTeam.ToLower()).First().Key;
                    }
                    catch { }
                }

                // Load home team data grid views
                dataGridRosterHome.Columns.Clear();
                dataGridRosterHome.DataSource = Excel.GetDataTable(_rostersFile, _home_roster_id >= 0 ? _home_roster_id : 0);
                if (dataGridRosterHome.ColumnCount > 0 && dataGridRosterHome.Columns.Contains("Number"))
                    dataGridRosterHome.Sort(dataGridRosterHome.Columns["Number"], System.ComponentModel.ListSortDirection.Ascending);
                dataGridCoachesHome.Columns.Clear();
                dataGridCoachesHome.DataSource = Excel.GetDataTable(_coachesFile, _home_coach_id >= 0 ? _home_coach_id : 0);
                populateTeamLowerThirds(Teams.Home);

                // Load away team data grid views
                dataGridRosterAway.Columns.Clear();
                dataGridRosterAway.DataSource = Excel.GetDataTable(_rostersFile, _away_roster_id >= 0 ? _away_roster_id : 1);
                if (dataGridRosterAway.ColumnCount > 0 && dataGridRosterAway.Columns.Contains("Number"))
                    dataGridRosterAway.Sort(dataGridRosterAway.Columns["Number"], System.ComponentModel.ListSortDirection.Ascending);
                dataGridCoachesAway.Columns.Clear();
                dataGridCoachesAway.DataSource = Excel.GetDataTable(_coachesFile, _away_coach_id >= 0 ? _away_coach_id : 0);
                populateTeamLowerThirds(Teams.Away);

                // Load the team stats
                dataGridStats.Columns.Clear();
                dataGridStats.DataSource = Excel.GetDataTable(_statsFile);
            }
            catch { }
        }

        // Load teams data from given paths
        public void LoadL3Setup()
        {
            try
            {
                // Check for team data locations
                string _setupFile = string.Format("{0}\\LowerThirds.xlsx", eventDataDirectoryPath);

                // Check if any of the data doesn't have a path set and if a home and away team are selected
                if (string.IsNullOrWhiteSpace(eventDataDirectoryPath) || string.IsNullOrWhiteSpace(_setupFile))
                {
                    // Exit out of loading as we don't have all paths needed
                    return;
                }

                // Load the team lower-thirds
                dataGridL3Setup.Columns.Clear();
                dataGridL3Setup.DataSource = Excel.GetDataTable(_setupFile);
                foreach (DataGridViewColumn column in dataGridL3Setup.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                populateEventLowerThirds();
            }
            catch { }
        }

        // Load data for the custom variables config window
        public void LoadCreditsData()
        {
            try
            {
                // Check for team data locations
                string _creditsFile = string.Format("{0}\\Credits.xlsx", eventDataDirectoryPath);

                // Check if any of the data doesn't have a path set and if a home and away team are selected
                if (string.IsNullOrWhiteSpace(eventDataDirectoryPath) || string.IsNullOrWhiteSpace(_creditsFile))
                {
                    // Exit out of loading as we don't have all paths needed
                    return;
                }

                // Load the credits
                dataGridCredits.Columns.Clear();
                dataGridCredits.DataSource = Excel.GetDataTable(_creditsFile);
                foreach (DataGridViewColumn column in dataGridCredits.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            catch { }
        }

        // Convert any Custom Variables found in the string to their value
        public string ConvertCustoms(string source)
        {
            // Clear out any empty rows first
            dataGridCustoms.RemoveEmptyRows();
            try
            {
                // Go through all custom variables and replace with values in source string
                foreach (DataGridViewRow row in dataGridCustoms.Rows)
                {
                    string name = (string)row.Cells["Name"].Value;
                    string key = "{" + name + "}";
                    string value = (string)row.Cells["Value"].Value;
                    if (row.Cells.Count > 0 && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value) && source.Contains(key))
                    {
                        source = source.Replace(key, value);
                    }
                }
            }
            catch { }

            return source.Trim();
        }

        private DataGridViewRow GetDataGridViewRow(DataGridView dgv, string columnName, string cellValue, bool DisablewUserCanddRows = false)
        {
            DataGridViewRow _result = null;
            try
            {
                if (DisablewUserCanddRows)
                    dgv.AllowUserToAddRows = false;

                DataGridViewColumn nameColumn = dgv.Columns[columnName];
                if (nameColumn != null)
                {
                    int nameColumnIndex = nameColumn.Index;
                    _result = dgv.Rows
                        .Cast<DataGridViewRow>()
                        .Where(r => r.Cells[nameColumnIndex] != null && r.Cells[nameColumnIndex].Value.ToString().Equals(cellValue, StringComparison.OrdinalIgnoreCase))
                        .First();
                }

                if (DisablewUserCanddRows)
                    dgv.AllowUserToAddRows = true;
            }
            catch { }
            return _result;
        }

        private DataGridViewCell GetDataGridViewCell(DataGridView dgv, DataGridViewRow row, string columnName)
        {
            if (dgv == null || row == null || string.IsNullOrEmpty(columnName))
                return null;

            return row.Cells
                .Cast<DataGridViewCell>()
                .Where(c => dgv.Columns[c.ColumnIndex] != null && dgv.Columns[c.ColumnIndex].Name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                .First();
        }

        // Turn on take ID and update setus of others
        public bool TurnOnTakeItem(TakeItem takeItem)
        {
            bool _success = false;

            try
            {
                // Turn off all other buttons on the same layer
                if (takeItem != null)
                {
                    int id = takeItem.ID;
                    foreach (TakeItem item in GetAllTakeItemsOnLayer(takeItem.Layer))
                    {
                        // If it is not this button's takeItem but is online, turn it off
                        if (item.ID != id && item.IsOnline)
                            item.SetOffline();
                    }
                    // Set our take item online
                    _success = takeItem.SetOnline();
                }
            }
            catch { _success = false; }

            return _success;
        }

        // Turn off take ID and update setus of others
        public bool TurnOffTakeItem(TakeItem takeItem)
        {
            bool _success = false;

            try
            {
                // Turn off all other buttons on the same layer
                if (takeItem != null)
                {
                    // Set our take item offline
                    _success = takeItem.SetOffline();
                }
            }
            catch { _success = false; }

            return _success;
        }

        // Turn off all take items on given takeItems layer
        public bool TurnOffTakeItemLayer(TakeItem takeItem)
        {
            bool _success = true;
            try
            {
                // Turn off all other buttons on the same layer
                if (takeItem != null)
                {
                    foreach (TakeItem item in GetAllTakeItemsOnLayer(takeItem.Layer))
                    {
                        // If it is not this button's takeItem but is online, turn it off
                        if (item.IsOnline)
                            item.SetOffline();
                    }
                }
            }
            catch { _success = false; }

            return _success;
        }

        // Change take buttons to Online/Offline status and return new status
        public bool ToggleTakeButton(Button takeButton)
        {
            bool turnOnline;
            if (string.Equals("Offline", takeButton.Text))
            {
                takeButton.Text = "Online";
                takeButton.BackColor = System.Drawing.Color.DarkGreen;
                turnOnline = true;
            }
            else
            {
                takeButton.Text = "Offline";
                takeButton.BackColor = System.Drawing.Color.DarkRed;
                turnOnline = false;
            }

            return turnOnline;
        }

        public void ToggleLowerThirds(Button takeButton)
        {
            if (takeButton != btnTakeHomePlayerInfo && btnTakeHomePlayerInfo.Text == "Online")
                ToggleTakeButton(btnTakeHomePlayerInfo);
            if (takeButton != btnTakeHomeCoachInfo && btnTakeHomeCoachInfo.Text == "Online")
                ToggleTakeButton(btnTakeHomeCoachInfo);
            if (takeButton != btnTakeHomePlayerStats && btnTakeHomePlayerStats.Text == "Online")
                ToggleTakeButton(btnTakeHomePlayerStats);
            if (takeButton != btnTakeHomePlayerInfo && btnTakeHomePlayerInfo.Text == "Online")
                ToggleTakeButton(btnTakeHomePlayerInfo);
            if (takeButton != btnTakeHomeCoachInfo && btnTakeHomeCoachInfo.Text == "Online")
                ToggleTakeButton(btnTakeHomeCoachInfo);

            if (takeButton != btnTakeAwayPlayerInfo && btnTakeAwayPlayerInfo.Text == "Online")
                ToggleTakeButton(btnTakeAwayPlayerInfo);
            if (takeButton != btnTakeAwayCoachInfo && btnTakeAwayCoachInfo.Text == "Online")
                ToggleTakeButton(btnTakeAwayCoachInfo);
            if (takeButton != btnTakeAwayPlayerStats && btnTakeAwayPlayerStats.Text == "Online")
                ToggleTakeButton(btnTakeAwayPlayerStats);
            if (takeButton != btnTakeAwayPlayerInfo && btnTakeAwayPlayerInfo.Text == "Online")
                ToggleTakeButton(btnTakeAwayPlayerInfo);
            if (takeButton != btnTakeAwayCoachInfo && btnTakeAwayCoachInfo.Text == "Online")
                ToggleTakeButton(btnTakeAwayCoachInfo);

            if (takeButton != btnTakeCustomL3 && btnTakeCustomL3.Text == "Online")
                ToggleTakeButton(btnTakeCustomL3);

            if (takeButton != btnTakeL3Setup && btnTakeL3Setup.Text == "Online")
                ToggleTakeButton(btnTakeL3Setup);
        }

        public void ToggleBumpers(Button takeButton)
        {
            if (takeButton != btnTakeBumperScore && btnTakeBumperScore.Text == "Online")
                ToggleTakeButton(btnTakeBumperScore);
            if (takeButton != btnTakeBumperLocator && btnTakeBumperLocator.Text == "Online")
                ToggleTakeButton(btnTakeBumperLocator);
            if (takeButton != btnTakeBumperHeadToHead && btnTakeBumperHeadToHead.Text == "Online")
                ToggleTakeButton(btnTakeBumperHeadToHead);
            if (takeButton != btnTakeBumperStandings && btnTakeBumperStandings.Text == "Online")
                ToggleTakeButton(btnTakeBumperStandings);
        }

        public void CheckInfoBugStatus()
        {
            TakeItem infoItem = takeItems.Where(item => item.Name == "Scorebug_Info_ID").First();
            if (infoItem.IsOnline)
            {
                updateInfoLastEdited(0);
                TurnOnTakeItem(infoItem);
            }
        }

        public void LoadTakeID(string key, int defaultID, Button btn = null, bool checkLive = true)
        {
            try
            {
                // Get the take ID as an int
                if (!int.TryParse(ConvertCustoms(Globals.GetObjectValue(key, propGridXpression.SelectedObject)), out int takeID))
                {
                    // If failed to load, get the default value
                    takeID = defaultID;
                }
                // Add data to Take Items list   
                TakeItem takeItm = new TakeItem(key, takeID, btn, _Xpression.GetTakeItem(takeID));
                takeItems.Add(takeItm);
                // Get Xpression take item status
                if (btn != null && checkLive)
                {
                    if (takeItm.IsOnline)
                    {
                        // TakeItem is online
                        if (btn.Text == "Offline")
                            ToggleTakeButton(btn);
                    }
                    else
                    {
                        // TakeItem is offline
                        if (btn.Text == "Online")
                            ToggleTakeButton(btn);
                    }
                }
            }
            catch { }
        }

        // Sync Xpression to Data project
        public void LoadFromXpression()
        {
            // Set current form to disabled
            this.Visible = false;
            this.Enabled = false;
            FormLoading frmLoading = new FormLoading();
            frmLoading.Setup(0, 22); // The starting value and max value for the loading bar
            frmLoading.Visible = true;
            frmLoading.Enabled = true;

            try
            {
                // Get Widget Values
                int homePoints = _Xpression.GetCounterWidgetValue(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Score", propGridXpression.SelectedObject)));
                if (homePoints > -1)
                {
                    scoreboardData.HomePoints = homePoints;
                    lblHomePoints.Text = homePoints.ToString();
                }
                frmLoading.Increase();
                int awayPoints = _Xpression.GetCounterWidgetValue(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Score", propGridXpression.SelectedObject)));
                if (awayPoints > -1)
                {
                    scoreboardData.AwayPoints = awayPoints;
                    lblAwayPoints.Text = awayPoints.ToString();
                }
                frmLoading.Increase();
                int homeFouls = _Xpression.GetCounterWidgetValue(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Fouls", propGridXpression.SelectedObject)));
                if (homeFouls > -1)
                {
                    scoreboardData.HomeFouls = homeFouls;
                }
                frmLoading.Increase();
                int awayFouls = _Xpression.GetCounterWidgetValue(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Fouls", propGridXpression.SelectedObject)));
                if (awayFouls > -1)
                {
                    scoreboardData.AwayFouls = awayFouls;
                }
                frmLoading.Increase();
                int _tmrGameClock = _Xpression.GetClockWidgetTimerValue(ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject)));
                if (_tmrGameClock > 0)
                {
                    TimeSpan t = TimeSpan.FromMilliseconds(_tmrGameClock);
                    lblGameClock.Text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                    decimal.TryParse(string.Format("{0:D2}.{1:D2}", t.Minutes, t.Seconds), out decimal val);
                    numGameClock.Value = val;
                }
                frmLoading.Increase();
                int _tmrShotClock = _Xpression.GetClockWidgetTimerValue(ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject)));
                if (_tmrShotClock > 0)
                {
                    TimeSpan t = TimeSpan.FromMilliseconds(_tmrShotClock);
                    lblShotClock.Text = string.Format("{0:D2}", t.Seconds);
                    decimal.TryParse(lblShotClock.Text.Trim(), out decimal val);
                    numShotClock.Value = val;
                }
                frmLoading.Increase();
                cmbQtr.Items.Clear();
                cmbQtr.Items.AddRange(_Xpression.GetTextListWidgetValues(ConvertCustoms(Globals.GetObjectValue("Widget_Quarter", propGridXpression.SelectedObject))).ToArray());
                long qtrIndex = _Xpression.GetTextListWidgetItemIndex(ConvertCustoms(Globals.GetObjectValue("Widget_Quarter", propGridXpression.SelectedObject)));
                if (qtrIndex > -1)
                {
                    if (cmbQtr.Items.Count > qtrIndex)
                        cmbQtr.SelectedIndex = (int)qtrIndex;
                }
                frmLoading.Increase();

                // Get a default Xpression Data object for loading default IDs if no custom is found
                XpressionData _defaultXPN = new XpressionData();
                // Remove any existing takeItems and re-load them all
                takeItems.Clear();

                // Load Misc Take Items
                LoadTakeID("Misc_Network_Bug", _defaultXPN.Misc_Network_Bug, btnTakeNetwork);
                frmLoading.Increase();
                LoadTakeID("Misc_Replay_Bug", _defaultXPN.Misc_Replay_Bug, btnTakeReplay);
                frmLoading.Increase();

                // Load Bumper Take Items
                LoadTakeID("Bumper_Score_ID", _defaultXPN.Bumper_Score_ID, btnTakeBumperScore);
                frmLoading.Increase();
                LoadTakeID("Bumper_Locator_ID", _defaultXPN.Bumper_Locator_ID, btnTakeBumperLocator);
                frmLoading.Increase();
                LoadTakeID("Bumper_HeadToHead_ID", _defaultXPN.Bumper_HeadToHead_ID, btnTakeBumperHeadToHead);
                frmLoading.Increase();
                LoadTakeID("Bumper_Standings_ID", _defaultXPN.Bumper_Standings_ID, btnTakeBumperStandings);
                frmLoading.Increase();

                // Load Lower Third Take Items
                LoadTakeID("L3_EventExtras_ID", _defaultXPN.L3_EventExtras_ID, btnTakeL3Setup);
                frmLoading.Increase();
                LoadTakeID("L3_PlayerInfo_ID", _defaultXPN.L3_PlayerInfo_ID);
                frmLoading.Increase();
                LoadTakeID("L3_PlayerStats_ID", _defaultXPN.L3_PlayerStats_ID);
                frmLoading.Increase();
                LoadTakeID("L3_TeamInfo_ID", _defaultXPN.L3_TeamInfo_ID);
                frmLoading.Increase();
                LoadTakeID("L3_Custom_ID", _defaultXPN.L3_Custom_ID, btnTakeCustomL3);
                frmLoading.Increase();

                // Load Scorebug Take Items
                LoadTakeID("Scorebug_ID", _defaultXPN.Scorebug_ID, btnTakeScorebug);
                frmLoading.Increase();
                LoadTakeID("Scorebug_Info_ID", _defaultXPN.Scorebug_Info_ID, btnTakeInfobox);
                frmLoading.Increase();
                CheckInfoBugStatus();

                // Load Credits Take Items
                LoadTakeID("Credits_ID", _defaultXPN.Credits_ID, btnTakeCredits);
                frmLoading.Increase();
                LoadTakeID("Credits_Copyright_ID", _defaultXPN.Credits_Copyright_ID, btnTakeCredits);
                frmLoading.Increase();
            }
            catch { }

            // Everything is loaded so make sure the loading bar is maxed out then close the loading window
            frmLoading.Completed();
            frmLoading.Visible = false;
            frmLoading.Enabled = false;
            frmLoading.Dispose();
            // Rename this form
            this.Visible = true;
            this.Enabled = true;
            this.Focus();
        }

        // Sync data to Xpression project
        public void SyncXpression()
        {
            progBarSync.Value = 0;
            int widgetCount = 7;
            progBarSync.Maximum = 1 + widgetCount + takeItems.Count();

            string _homeName = ConvertCustoms(Globals.GetObjectValue("HomeName", propGridEvent.SelectedObject));
            string _awayName = ConvertCustoms(Globals.GetObjectValue("AwayName", propGridEvent.SelectedObject));
            string _homeSchool = ConvertCustoms(Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject));
            string _awaySchool = ConvertCustoms(Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject));
            string _homeAbbr = ConvertCustoms(Globals.GetObjectValue("HomeAbbr", propGridEvent.SelectedObject));
            string _awayAbbr = ConvertCustoms(Globals.GetObjectValue("AwayAbbr", propGridEvent.SelectedObject));

            // Sync Event Data to Xpression Widgets
            SyncEventData();

            // Get Widget Values
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Score", propGridXpression.SelectedObject)), scoreboardData.HomePoints);
            progBarSync.Value++;
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Score", propGridXpression.SelectedObject)), scoreboardData.AwayPoints);
            progBarSync.Value++;
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Fouls", propGridXpression.SelectedObject)), scoreboardData.HomeFouls);
            progBarSync.Value++;
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Fouls", propGridXpression.SelectedObject)), scoreboardData.AwayFouls);
            progBarSync.Value++;
            btnResetGameClock.PerformClick();
            progBarSync.Value++;
            btnResetShotClock.PerformClick();
            progBarSync.Value++;
            _Xpression.SetTextListWidgetValues(ConvertCustoms(Globals.GetObjectValue("Widget_Quarter", propGridXpression.SelectedObject)),
                cmbQtr.Items.Cast<string>().Select(item => item.ToString()).ToList());
            _Xpression.SetTextListWidgetItemIndex(ConvertCustoms(Globals.GetObjectValue("Widget_Quarter", propGridXpression.SelectedObject)), cmbQtr.SelectedIndex);
            progBarSync.Value++;

            foreach (TakeItem takeItem in takeItems)
            {
                if (int.TryParse(ConvertCustoms(Globals.GetObjectValue(takeItem.Name, propGridXpression.SelectedObject)), out int newID))
                {
                    takeItem.ID = newID;
                }

                if (_Xpression.GetTakeItemStatus(takeItem.ID))
                    _Xpression.SetTakeItemOnline(takeItem.ID);

                progBarSync.Value++;
            }

            progBarSync.Value = progBarSync.Maximum;
        }

        public void SyncEventData()
        {
            try
            {
                object eventData = propGridEvent.SelectedObject;
                foreach (PropertyInfo prop in eventData.GetType().GetProperties())
                {
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    if (type != null)
                    {
                        string name = prop.Name;
                        string value = ConvertCustoms(prop.GetValue(eventData, null).ToString());
                        // Set the event data to their own widgets
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                            _Xpression.SetTextListWidgetValues(name, new List<string> { value });
                    }
                }
            }
            catch { };
        }

        // Opens a folder browser and return the select path
        public string FolderBrowser(string defaultPath = "")
        {
            string path = "";

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (string.IsNullOrWhiteSpace(defaultPath))
                {
                    defaultPath = Environment.CurrentDirectory;
                }
                fbd.SelectedPath = defaultPath;

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
            }

            return path;
        }

        #endregion

        #region Scoreboard Tab

        private void updateInfoLastEdited(int value = 0)
        {
            scoreboardData.InfoLastEdited = value; // 0 = select box, 1 = text box
            if (value == 0)
            {
                lblCmbInfobox.BackColor = System.Drawing.Color.DarkGreen;
                lblTxtInfobox.BackColor = System.Drawing.Color.DarkRed;
            } else
            {
                lblCmbInfobox.BackColor = System.Drawing.Color.DarkRed;
                lblTxtInfobox.BackColor = System.Drawing.Color.DarkGreen;
            }
        }

        private void populateScoreboard()
        {
            // Fill in all scoreboard labels with proper data from scoreboard variable
            lblHomePoints.Text = scoreboardData.HomePoints.ToString();

            lblAwayPoints.Text = scoreboardData.AwayPoints.ToString();

            updateInfoboxDropdowns();
        }

        private void updateInfoboxDropdowns()
        {
            cmbInfobox.Items.Clear();
            cmbInfobox.Items.AddRange(ConvertCustoms(Globals.GetObjectValue("Scorebug_Info_Data", propGridXpression.SelectedObject)).Split(','));
            cmbInfobox.SelectedIndex = 0;
        }

        private void populateEventLowerThirds()
        {
            try
            {
                cmbL3SetupLeft.Items.Clear();
                cmbL3SetupMiddle.Items.Clear();
                cmbL3SetupRight.Items.Clear();
                foreach (DataGridViewRow row in dataGridL3Setup.Rows)
                {
                    if (row.Cells.Count > 1)
                    {
                        try
                        {
                            if (row.Cells[0].Value != null && row.Cells[1].Value != null) {
                                string _data = string.Format("{0} - {1}", row.Cells[0].Value.ToString().Trim(), row.Cells[1].Value.ToString().Trim());
                                cmbL3SetupLeft.Items.Add(_data);
                                cmbL3SetupMiddle.Items.Add(_data);
                                cmbL3SetupRight.Items.Add(_data);
                            }
                        } catch { }
                    }
                }
            }
            catch { return; }
        }

        private void populateTeamLowerThirds(Teams team)
        {
            void selectLast(ComboBox cmbBox = null, object selection = null)
            {
                if (cmbBox == null || selection == null) return;

                int index = cmbBox.Items.IndexOf(selection);
                if (index != -1)
                    cmbBox.SelectedIndex = index;
                else
                    cmbBox.SelectedIndex = 0;
            }

            if (team == Teams.None) return;

            if (team == Teams.Home)
            {
                object HomeL3PlayerNumber = cmbHomeL3PlayerNumber.SelectedItem;
                object HomeL3PlayerName = cmbHomeL3PlayerName.SelectedItem;
                object HomeL3PlayerStat1 = cmbHomeL3PlayerStat1.SelectedItem;
                object HomeL3PlayerStat2 = cmbHomeL3PlayerStat2.SelectedItem;
                object HomeL3PlayerStat3 = cmbHomeL3PlayerStat3.SelectedItem;
                object HomeL3CoachName = cmbHomeL3CoachName.SelectedItem;

                DataGridView _rosters = dataGridRosterHome;
                DataGridView _coaches = dataGridCoachesHome;

                _rosters.AllowUserToAddRows = false;
                _coaches.AllowUserToAddRows = false;

                cmbHomeL3PlayerNumber.Items.Clear();
                cmbHomeL3PlayerNumber.Items.Add("");
                cmbHomeL3PlayerNumber.Items.AddRange(_rosters.Rows
                             .OfType<DataGridViewRow>()
                             .Select(r => r.Cells[_rosters.Columns["Number"].Index].Value.ToString())
                             .ToArray());

                cmbHomeL3PlayerName.Items.Clear();
                cmbHomeL3PlayerName.Items.Add("");
                cmbHomeL3PlayerName.Items.AddRange(_rosters.Rows
                             .OfType<DataGridViewRow>()
                             .Select(r => r.Cells[_rosters.Columns["Name"].Index].Value.ToString())
                             .ToArray());

                string[] _columns = _rosters.Columns
                             .OfType<DataGridViewColumn>()
                             .Select(r => r.Name)
                             .ToArray();

                cmbHomeL3PlayerStat1.Items.Clear();
                cmbHomeL3PlayerStat1.Items.Add("");
                cmbHomeL3PlayerStat1.Items.AddRange(_columns);

                cmbHomeL3PlayerStat2.Items.Clear();
                cmbHomeL3PlayerStat2.Items.Add("");
                cmbHomeL3PlayerStat2.Items.AddRange(_columns);

                cmbHomeL3PlayerStat3.Items.Clear();
                cmbHomeL3PlayerStat3.Items.Add("");
                cmbHomeL3PlayerStat3.Items.AddRange(_columns);

                cmbHomeL3CoachName.Items.Clear();
                cmbHomeL3CoachName.Items.Add("");
                cmbHomeL3CoachName.Items.AddRange(_coaches.Rows
                             .OfType<DataGridViewRow>()
                             .Select(r => r.Cells[_coaches.Columns["Name"].Index].Value.ToString())
                             .ToArray());

                _rosters.AllowUserToAddRows = true;
                _coaches.AllowUserToAddRows = true;

                selectLast(cmbHomeL3PlayerNumber, HomeL3PlayerNumber);
                selectLast(cmbHomeL3PlayerName, HomeL3PlayerName);
                selectLast(cmbHomeL3PlayerStat1, HomeL3PlayerStat1);
                selectLast(cmbHomeL3PlayerStat2, HomeL3PlayerStat2);
                selectLast(cmbHomeL3PlayerStat3, HomeL3PlayerStat3);
                selectLast(cmbHomeL3CoachName, HomeL3CoachName);
            }
            else if (team == Teams.Away)
            {
                object AwayL3PlayerNumber = cmbAwayL3PlayerNumber.SelectedItem;
                object AwayL3PlayerName = cmbAwayL3PlayerName.SelectedItem;
                object AwayL3PlayerStat1 = cmbAwayL3PlayerStat1.SelectedItem;
                object AwayL3PlayerStat2 = cmbAwayL3PlayerStat2.SelectedItem;
                object AwayL3PlayerStat3 = cmbAwayL3PlayerStat3.SelectedItem;
                object AwayL3CoachName = cmbAwayL3CoachName.SelectedItem;

                DataGridView _rosters = dataGridRosterAway;
                DataGridView _coaches = dataGridCoachesAway;

                _rosters.AllowUserToAddRows = false;
                _coaches.AllowUserToAddRows = false;

                cmbAwayL3PlayerNumber.Items.Clear();
                cmbAwayL3PlayerNumber.Items.Add("");
                cmbAwayL3PlayerNumber.Items.AddRange(_rosters.Rows
                             .OfType<DataGridViewRow>()
                             .Select(r => r.Cells[_rosters.Columns["Number"].Index].Value.ToString())
                             .ToArray());

                cmbAwayL3PlayerName.Items.Clear();
                cmbAwayL3PlayerName.Items.Add("");
                cmbAwayL3PlayerName.Items.AddRange(_rosters.Rows
                             .OfType<DataGridViewRow>()
                             .Select(r => r.Cells[_rosters.Columns["Name"].Index].Value.ToString())
                             .ToArray());

                string[] _columns = _rosters.Columns
                             .OfType<DataGridViewColumn>()
                             .Select(r => r.Name)
                             .ToArray();

                cmbAwayL3PlayerStat1.Items.Clear();
                cmbAwayL3PlayerStat1.Items.Add("");
                cmbAwayL3PlayerStat1.Items.AddRange(_columns);

                cmbAwayL3PlayerStat2.Items.Clear();
                cmbAwayL3PlayerStat2.Items.Add("");
                cmbAwayL3PlayerStat2.Items.AddRange(_columns);

                cmbAwayL3PlayerStat3.Items.Clear();
                cmbAwayL3PlayerStat3.Items.Add("");
                cmbAwayL3PlayerStat3.Items.AddRange(_columns);

                cmbAwayL3CoachName.Items.Clear();
                cmbAwayL3CoachName.Items.Add("");
                cmbAwayL3CoachName.Items.AddRange(_coaches.Rows
                             .OfType<DataGridViewRow>()
                             .Select(r => r.Cells[_coaches.Columns["Name"].Index].Value.ToString())
                             .ToArray());
                cmbAwayL3CoachName.SelectedIndex = 0;

                _rosters.AllowUserToAddRows = true;
                _coaches.AllowUserToAddRows = true;

                selectLast(cmbAwayL3PlayerNumber, AwayL3PlayerNumber);
                selectLast(cmbAwayL3PlayerName, AwayL3PlayerName);
                selectLast(cmbAwayL3PlayerStat1, AwayL3PlayerStat1);
                selectLast(cmbAwayL3PlayerStat2, AwayL3PlayerStat2);
                selectLast(cmbAwayL3PlayerStat3, AwayL3PlayerStat3);
                selectLast(cmbAwayL3CoachName, AwayL3CoachName);
            }
        }

        private void PlayTeamPoint(Teams team, int points = 1)
        {
            string infoText = string.Empty;

            void Info_Timer_Tick(object tickSender, EventArgs tickE)
            {
                Invoke(new Action(() =>
                {
                    if (!infoText.Equals(string.Empty) && infoText.Equals(txtInfobox.Text)) {
                        TakeItem takeItem = takeItems.Where(item => item.Name == "Scorebug_Info_ID").First();
                        // Check if take item is online
                        if (takeItem.IsOnline)
                        {
                            TurnOffTakeItem(takeItem);
                            // Revert to w/e our select box is for next time we show the bug
                        }
                        updateInfoLastEdited(0);
                        txtInfobox.Text = string.Empty;
                    }
                }));

                _infoTmr.Stop();
                _infoTmr.Enabled = false;
                _infoTmr = null;
            }

            // Only update text if AutoPoint is set to True
            if (_autoPoint)
            {
                switch (team)
                {
                    case Teams.Home:
                        string _homeName = ConvertCustoms(Globals.GetObjectValue("HomeName", propGridEvent.SelectedObject));
                        txtInfobox.Text = string.Format("{0} +{1}", _homeName, points);
                        infoText = txtInfobox.Text;
                        break;
                    case Teams.Away:
                        string _awayName = ConvertCustoms(Globals.GetObjectValue("AwayName", propGridEvent.SelectedObject));
                        txtInfobox.Text = string.Format("{0} +{1}", _awayName, points);
                        infoText = txtInfobox.Text;
                        break;
                }

                updateInfoLastEdited(1);

                TakeItem takeItem = takeItems.Where(item => item.Name == "Scorebug_Info_ID").First();
                // Check if take item is offline
                if (!takeItem.IsOnline)
                {
                    btnTakeInfobox.PerformClick();

                    // Reset & Start the timer
                    if (_infoTmr != null)
                    {
                        _infoTmr.Stop();
                        _infoTmr.Enabled = false;
                        _infoTmr = null;
                    }
                    _infoTmr = new Timer
                    {
                        Interval = 2500
                    };
                    _infoTmr.Tick += new EventHandler(Info_Timer_Tick);
                    _infoTmr.Enabled = true;
                    _infoTmr.Start();
                }
            }
        }

        private bool TakePlayerStats(TakeItem takeItem, Teams team)
        {
            if (team == Teams.None)
                return false;

            try
            {
                // Get all of the home player stat settings
                string _number = "", _name = "", _custom = "", _school = "BRTF SPORTS", _team = "";
                if (team == Teams.Home)
                {
                    _number = ConvertCustoms(cmbHomeL3PlayerNumber.Text.Trim());
                    _name = ConvertCustoms(cmbHomeL3PlayerName.Text.Trim());
                    _custom = ConvertCustoms(txtHomeL3PlayerInfo.Text.Trim());
                    _school = ConvertCustoms(Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject));
                    _team = ConvertCustoms(Globals.GetObjectValue("HomeName", propGridEvent.SelectedObject));
                }
                else
                {
                    _number = ConvertCustoms(cmbAwayL3PlayerNumber.Text.Trim());
                    _name = ConvertCustoms(cmbAwayL3PlayerName.Text.Trim());
                    _custom = ConvertCustoms(txtAwayL3PlayerInfo.Text.Trim());
                    _school = ConvertCustoms(Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject));
                    _team = ConvertCustoms(Globals.GetObjectValue("AwayName", propGridEvent.SelectedObject));
                }

                // If no name or number is selected, exit out as we have nothing to show
                if (string.IsNullOrEmpty(_number) || string.IsNullOrEmpty(_name))
                    return false;

                DataGridView _rosters;
                if (team == Teams.Home)
                    _rosters = dataGridRosterHome;
                else
                    _rosters = dataGridRosterAway;

                if (_rosters == null)
                    return false;
                _rosters.AllowUserToAddRows = false;

                DataGridViewRow row = GetDataGridViewRow(_rosters, "Name", _name);

                // Failed to find player data
                if (row == null || row.Index < 0)
                    return false;

                string _info = "", _stat1 = "", _stat2 = "", _stat3 = "";
                if (team == Teams.Home)
                {
                    _stat1 = ConvertCustoms(cmbHomeL3PlayerStat1.Text.Trim());
                    _stat2 = ConvertCustoms(cmbHomeL3PlayerStat2.Text.Trim());
                    _stat3 = ConvertCustoms(cmbHomeL3PlayerStat3.Text.Trim());
                }
                else
                {
                    _stat1 = ConvertCustoms(cmbAwayL3PlayerStat1.Text.Trim());
                    _stat2 = ConvertCustoms(cmbAwayL3PlayerStat2.Text.Trim());
                    _stat3 = ConvertCustoms(cmbAwayL3PlayerStat3.Text.Trim());
                }

                if (_stat1 != string.Empty)
                {
                    DataGridViewCell cellStat = GetDataGridViewCell(_rosters, row, _stat1);
                    if (cellStat != null)
                        _info += _stat1 + ": " + cellStat.Value.ToString().Trim() + "   |   ";
                }

                if (_stat2 != string.Empty)
                {
                    DataGridViewCell cellStat = GetDataGridViewCell(_rosters, row, _stat2);
                    if (cellStat != null)
                        _info += _stat2 + ": " + cellStat.Value.ToString().Trim() + "   |   ";
                }

                if (_stat3 != string.Empty)
                {
                    DataGridViewCell cellStat = GetDataGridViewCell(_rosters, row, _stat3);
                    if (cellStat != null)
                        _info += _stat3 + ": " + cellStat.Value.ToString().Trim() + "   |   ";
                }

                if (string.IsNullOrEmpty(_custom))
                {
                    DataGridViewCell cellStat = GetDataGridViewCell(_rosters, row, "Position");
                    if (cellStat != null)
                        _custom += cellStat.Value.ToString().Trim();
                }

                _rosters.AllowUserToAddRows = true;

                // Edit TakeItem poroperties with new values
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerStats_Number", propGridXpression.SelectedObject)), "Text", Regex.Replace(_number, @"[^\d]", string.Empty).PadLeft(2, '0'));
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerStats_Name", propGridXpression.SelectedObject)), "Text", _name);
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerStats_Info", propGridXpression.SelectedObject)), "Text", ReplaceLastOccurrence(_info, "|", string.Empty).Trim());
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerStats_Highlight", propGridXpression.SelectedObject)), "Text", _custom);
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerStats_TeamQuad", propGridXpression.SelectedObject)), "Material", string.Format("{0}_Image", _school));

                return takeItem.SetOnline();
            }
            catch { return false; }
        }

        private bool TakePlayerInfo(TakeItem takeItem, Teams team)
        {
            if (team == Teams.None)
                return false;

            try
            {
                // Get all of the home player stat settings
                string _number = "", _name = "", _custom = "", _school = "BRTF SPORTS";
                if (team == Teams.Home)
                {
                    _number = ConvertCustoms(cmbHomeL3PlayerNumber.Text.Trim());
                    _name = ConvertCustoms(cmbHomeL3PlayerName.Text.Trim());
                    _custom = ConvertCustoms(txtHomeL3PlayerInfo.Text.Trim());
                    _school = ConvertCustoms(Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject));
                }
                else
                {
                    _number = ConvertCustoms(cmbAwayL3PlayerNumber.Text.Trim());
                    _name = ConvertCustoms(cmbAwayL3PlayerName.Text.Trim());
                    _custom = ConvertCustoms(txtAwayL3PlayerInfo.Text.Trim());
                    _school = ConvertCustoms(Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject));
                }

                // If no name or number is selected, exit out as we have nothing to show
                if (string.IsNullOrEmpty(_number) || string.IsNullOrEmpty(_name))
                    return false;

                // Edit TakeItem poroperties with new values
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerInfo_Number", propGridXpression.SelectedObject)), "Text", Regex.Replace(_number, @"[^\d]", string.Empty).PadLeft(2, '0'));
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerInfo_Name", propGridXpression.SelectedObject)), "Text", _name);
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerInfo_Info", propGridXpression.SelectedObject)), "Text", _custom);
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_PlayerInfo_TeamQuad", propGridXpression.SelectedObject)), "Material", string.Format("{0}_Image", _school));

                return takeItem.SetOnline();
            }
            catch { return false; }
        }

        private bool TakeCoachInfo(TakeItem takeItem, Teams team)
        {
            if (team == Teams.None)
                return false;

            try
            {
                // Get all of the home player stat settings
                string _name = "", _custom = "", _school = "BRTF SPORTS";
                if (team == Teams.Home)
                {
                    _name = ConvertCustoms(cmbHomeL3CoachName.Text.Trim());
                    _custom = ConvertCustoms(txtHomeL3CoachInfo.Text.Trim());
                    _school = ConvertCustoms(Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject));
                }
                else
                {
                    _name = ConvertCustoms(cmbAwayL3CoachName.Text.Trim());
                    _custom = ConvertCustoms(txtAwayL3CoachInfo.Text.Trim());
                    _school = ConvertCustoms(Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject));
                }

                // If no name or school is selected, exit out as we have nothing to show
                if (string.IsNullOrEmpty(_name) || string.IsNullOrEmpty(_school))
                    return false;

                // Edit TakeItem poroperties with new values
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_TeamInfo_Title", propGridXpression.SelectedObject)), "Text", _name);
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_TeamInfo_Info", propGridXpression.SelectedObject)), "Text", _custom);
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_TeamInfo_TeamQuad", propGridXpression.SelectedObject)), "Material", string.Format("{0}_Image", _school));

                return takeItem.SetOnline();
            }
            catch { return false; }
        }

        private bool TakeCustomL3(TakeItem takeItem)
        {
            try
            {
                // Get all of the home player stat settings
                string _custom = ConvertCustoms(txtTakeCustomL3.Text.Trim());
                string _message = GetMessageChunks(_custom);

                // Edit TakeItem poroperties with new values
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_Custom_Title", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("EventTitle", propGridEvent.SelectedObject)));
                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_Custom_Message", propGridXpression.SelectedObject)), "Text", _message);

                return takeItem.SetOnline();
            }
            catch { return false; }
        }

        private void btnScoreboardReset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to reset the scoreboard? This will restore all score data back to defaults.",
                                     "Confirm Reset!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                scoreboardData.Reset();
                populateScoreboard();
            }
        }

        private void btnPointsReset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to reset both teams Points to 0?",
                                     "Confirm Reset!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                scoreboardData.ResetPoints();
                lblHomePoints.Text = "0";
                lblAwayPoints.Text = "0";
            }
        }

        private void lblHomePoints_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lblHomePoints.Text, out int val))
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Score", propGridXpression.SelectedObject)), val);
        }

        public void HomeScoreUpdate(int points)
        {
            int homePoints = scoreboardData.HomePoints;
            if (homePoints < points)
                PlayTeamPoint(Teams.Home, points - homePoints);
            scoreboardData.HomePoints = points;
            Invoke(new Action(() =>
            {
                lblHomePoints.Text = points.ToString();
            }));
        }

        public void AwayScoreUpdate(int points)
        {
            int awayPoints = scoreboardData.AwayPoints;
            if (awayPoints < points)
                PlayTeamPoint(Teams.Away, points - awayPoints);
            scoreboardData.AwayPoints = points;
            Invoke(new Action(() =>
            {
                lblAwayPoints.Text = points.ToString();
            }));
        }

        public void HomeFoulsUpdate(int fouls)
        {
            scoreboardData.HomeFouls = fouls;
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Fouls", propGridXpression.SelectedObject)), fouls);
        }

        public void AwayFoulsUpdate(int fouls)
        {
            scoreboardData.AwayFouls = fouls;
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Fouls", propGridXpression.SelectedObject)), fouls);
        }

        public void QuaterUpdate(string quarter)
        {
            int index = cmbQtr.Items.IndexOf(quarter);
            if (index >= 0)
            {
                Invoke(new Action(() =>
                {
                    cmbQtr.SelectedIndex = index;
                }));
            }
            else
            {
                string qtr = quarter + "st";
                switch (quarter)
                {
                    case "1":
                        qtr = $"{quarter}st";
                        break;
                    case "2":
                        qtr = $"{quarter}nd";
                        break;
                    case "3":
                        qtr = $"{quarter}rd";
                        break;
                    case "4":
                        qtr = $"{quarter}th";
                        break;
                    default:
                        break;
                }

                index = cmbQtr.Items.IndexOf(qtr);
                if (index >= 0)
                {
                    Invoke(new Action(() =>
                    {
                        cmbQtr.SelectedIndex = index;
                    }));
                }
            }
        }

        private void btnHomePointsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.RemovePoint(Teams.Home);
            lblHomePoints.Text = val.ToString();
        }

        private void btnHomePointsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Home);
            lblHomePoints.Text = val.ToString();
            PlayTeamPoint(Teams.Home);
        }

        private void lblAwayPoints_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lblAwayPoints.Text, out int val))
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Score", propGridXpression.SelectedObject)), val);
        }

        private void btnAwayPointsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.RemovePoint(Teams.Away);
            lblAwayPoints.Text = val.ToString();
        }

        private void btnAwayPointsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Away);
            lblAwayPoints.Text = val.ToString();
            PlayTeamPoint(Teams.Away);
        }
        private void btnHomeInc1_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Home);
            lblHomePoints.Text = val.ToString();
            PlayTeamPoint(Teams.Home);
        }

        private void btnHomeInc2_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Home, 2);
            lblHomePoints.Text = val.ToString();
            PlayTeamPoint(Teams.Home, 2);
        }

        private void btnHomeInc3_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Home, 3);
            lblHomePoints.Text = val.ToString();
            PlayTeamPoint(Teams.Home, 3);
        }

        private void btnAwayInc1_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Away);
            lblAwayPoints.Text = val.ToString();
            PlayTeamPoint(Teams.Away);
        }

        private void btnAwayInc2_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Away, 2);
            lblAwayPoints.Text = val.ToString();
            PlayTeamPoint(Teams.Away, 2);
        }

        private void btnAwayInc3_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.AddPoint(Teams.Away, 3);
            lblAwayPoints.Text = val.ToString();
            PlayTeamPoint(Teams.Away, 2);
        }

        public void GameClockTick(int hours, int minutes, int seconds, int miliseconds)
        {
            Invoke(new Action(() =>
            {
                if (hours == 0 && minutes == 0 && seconds == 60)
                    _Xpression.EditClockWidgetFormat(ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject)), "SS");
                else if (hours == 0 && minutes == 0 && seconds == 10)
                    _Xpression.EditClockWidgetFormat(ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject)), "S.ZZ");

                if (minutes == 0 && seconds < 1)
                    lblGameClock.Text = string.Format("{0:D2}.{1:D2}", 0, 0);
                if (minutes == 0 && seconds < 10)
                    lblGameClock.Text = string.Format("{0:D2}.{1:D2}", seconds, miliseconds);
                else
                    lblGameClock.Text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            }));
        }

        public void ShotClockTick(int hours, int minutes, int seconds, int miliseconds)
        {
            Invoke(new Action(() =>
            {
                if (!_shotClock_enabled)
                {
                    int shotTime = (int)numShotClock.Value;
                    if (seconds == (shotTime - 5))
                    {
                        string dirName = ConvertCustoms(Globals.GetObjectValue("Scorebug_ShotClock_Director_IN", propGridXpression.SelectedObject));
                        if (!string.IsNullOrEmpty(dirName))
                        {
                            TakeItem takeItem = takeItems.Where(item => item.Name == "Scorebug_ID").First();
                            if (takeItem != null && takeItem.IsOnline)
                            {
                                if (_Xpression.PlaySceneDirector(takeItem.ID, dirName))
                                    _shotClock_enabled = true;
                            }
                        }
                    }
                } else
                {
                    if (hours == 0 && minutes == 0 && seconds == 10)
                        _Xpression.EditClockWidgetFormat(ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject)), "S.ZZ");
                }

                if (minutes == 0 && seconds < 1)
                    lblGameClock.Text = string.Format("{0:D2}", 0);
                if (minutes == 0 && seconds < 10)
                    lblGameClock.Text = string.Format("{0:D2}.{1:D2}", seconds, miliseconds);
                else
                    lblShotClock.Text = string.Format("{0:D2}", seconds);
            }));
        }

        private void TurnOnGameClock()
        {
            btnGameClock.Text = "Stop";
            btnGameClock.BackColor = System.Drawing.Color.DarkGreen;

            string _gameClockName = ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject));
            _Xpression.StartClockWidget(_gameClockName);
            if (string.Equals("Enabled", btnShotClockDisable.Text))
                TurnOnShotClock();
            _Xpression.SetClockWidgetCallback(_gameClockName, GameClockTick);

        }

        private void TurnOffGameClock()
        {
            btnGameClock.Text = "Start";
            btnGameClock.BackColor = System.Drawing.Color.DarkRed;

            _Xpression.StopClockWidget(ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject)));
            if (string.Equals("Enabled", btnShotClockDisable.Text))
                TurnOffShotClock();
        }

        private void ResetGameClock()
        {
            btnGameClock.Text = "Start";
            btnGameClock.BackColor = System.Drawing.Color.DarkRed;

            string _gameClockName = ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject));
            _Xpression.EditClockWidgetFormat(_gameClockName, "NN:SS");
            _Xpression.ResetClockWidget(_gameClockName);

            int _tmrValue = _Xpression.GetClockWidgetTimerValue(_gameClockName);
            TimeSpan t = TimeSpan.FromMilliseconds(_tmrValue);
            lblGameClock.Text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }

        private void TurnOnShotClock()
        {
            if (string.Equals("Disabled", btnShotClockDisable.Text))
                return;

            btnShotClock.Text = "Stop";
            btnShotClock.BackColor = System.Drawing.Color.DarkGreen;
            string _shotClockName = ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject));
            _Xpression.StartClockWidget(_shotClockName);
            _Xpression.SetClockWidgetCallback(_shotClockName, ShotClockTick);
        }

        private void TurnOffShotClock()
        {
            if (string.Equals("Disabled", btnShotClockDisable.Text))
                return;

            btnShotClock.Text = "Start";
            btnShotClock.BackColor = System.Drawing.Color.DarkRed;

            _Xpression.StopClockWidget(ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject)));
        }

        private void ResetShotClock()
        {
            if (string.Equals("Disabled", btnShotClockDisable.Text))
                return;

            btnShotClock.Text = "Start";
            btnShotClock.BackColor = System.Drawing.Color.DarkRed;

            // Only check for out animation if the shotClock is locally enabled
            if (_shotClock_enabled)
            {
                string dirName = ConvertCustoms(Globals.GetObjectValue("Scorebug_ShotClock_Director_OUT", propGridXpression.SelectedObject));
                if (!string.IsNullOrEmpty(dirName))
                {
                    TakeItem takeItem = takeItems.Where(item => item.Name == "Scorebug_ID").First();
                    if (takeItem != null && takeItem.IsOnline)
                        _Xpression.PlaySceneDirector(takeItem.ID, dirName);
                }
                _shotClock_enabled = false;
            }

            string _shotClockName = ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject));
            _Xpression.EditClockWidgetFormat(_shotClockName, "SS");
            _Xpression.ResetClockWidget(_shotClockName);

            int _tmrValue = _Xpression.GetClockWidgetTimerValue(_shotClockName);
            TimeSpan t = TimeSpan.FromMilliseconds(_tmrValue);
            lblShotClock.Text = string.Format("{0:D2}", t.Seconds);
        }

        private void numGameClock_ValueChanged(object sender, EventArgs e)
        {
            TurnOffGameClock();

            var val = numGameClock.Value;
            var minutes = Math.Truncate(val);
            string seconds = (val - Math.Truncate(val)).ToString().Replace("0.", "");
            string startTime = string.Format("00:{0:D2}:{1:D2}.000", minutes.ToString().PadLeft(2, '0'), seconds.ToString().PadLeft(2, '0'));
            _Xpression.EditClockWidgetStartTime(ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject)), startTime);
            GameClockTick(0, (int)minutes, int.Parse(seconds), 0);
        }

        private void btnGameClock_Click(object sender, EventArgs e)
        {
            if (string.Equals("Start", btnGameClock.Text))
            {
                TurnOnGameClock();
            }
            else
            {
                TurnOffGameClock();
            }
        }

        private void btnResetGameClock_Click(object sender, EventArgs e)
        {
            ResetGameClock();
        }

        private void btnGameClockDec_Click(object sender, EventArgs e)
        {
            string _gameClockName = ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject));
            // Get the current clock value
            TimeSpan spanCurTime = TimeSpan.FromMilliseconds(_Xpression.GetClockWidgetTimerValue(_gameClockName));
            spanCurTime -= TimeSpan.FromSeconds(1); // Remove a second
            lblGameClock.Text = string.Format("{0:D2}:{1:D2}", spanCurTime.Minutes, spanCurTime.Seconds);

            _Xpression.SetClockWidgetTimerValue(_gameClockName, (int)spanCurTime.TotalMilliseconds);
        }

        public void GameClockStart(string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan clock))
            {
                string _gameClockName = ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject));
                Invoke(new Action(() =>
                {
                    lblGameClock.Text = string.Format("{0:D2}:{1:D2}", clock.Minutes, clock.Seconds);
                }));

                if (_Xpression.SetClockWidgetTimerValue(_gameClockName, (int)clock.TotalMilliseconds))
                    TurnOnGameClock();
            }
        }

        public void GameClockStop(string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan clock))
            {
                TurnOffGameClock();

                string _gameClockName = ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject));
                Invoke(new Action(() =>
                {
                    lblGameClock.Text = string.Format("{0:D2}:{1:D2}", clock.Minutes, clock.Seconds);
                }));

                _Xpression.SetClockWidgetTimerValue(_gameClockName, (int)clock.TotalMilliseconds);
            }
        }

        public void GameClockUpdate(string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan clock))
            {
                string _gameClockName = ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject));
                Invoke(new Action(() =>
                {
                    lblGameClock.Text = string.Format("{0:D2}:{1:D2}", clock.Minutes, clock.Seconds);
                }));

                _Xpression.SetClockWidgetTimerValue(_gameClockName, (int)clock.TotalMilliseconds);
            }
        }

        public void ShotClockStart(string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan clock))
            {
                string _shotClockName = ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject));
                Invoke(new Action(() =>
                {
                    lblShotClock.Text = string.Format("{0:D2}", clock.Seconds);
                }));

                if (_Xpression.SetClockWidgetTimerValue(_shotClockName, (int)clock.TotalMilliseconds))
                    TurnOnShotClock();
            }
        }

        public void ShotClockStop(string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan clock))
            {
                TurnOffShotClock();

                string _shotClockName = ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject));
                Invoke(new Action(() =>
                {
                    lblShotClock.Text = string.Format("{0:D2}", clock.Seconds);
                }));

                _Xpression.SetClockWidgetTimerValue(_shotClockName, (int)clock.TotalMilliseconds);
            }
        }

        public void ShotClockUpdate(string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan clock))
            {
                string _shotClockName = ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject));
                Invoke(new Action(() =>
                {
                    lblShotClock.Text = string.Format("{0:D2}", clock.Seconds);
                }));

                _Xpression.SetClockWidgetTimerValue(_shotClockName, (int)clock.TotalMilliseconds);
            }
        }

        private void btnGameClockInc_Click(object sender, EventArgs e)
        {
            string _gameClockName = ConvertCustoms(Globals.GetObjectValue("Widget_GameClock", propGridXpression.SelectedObject));
            // Get the current clock value
            TimeSpan spanCurTime = TimeSpan.FromMilliseconds(_Xpression.GetClockWidgetTimerValue(_gameClockName));
            spanCurTime += TimeSpan.FromSeconds(1); // Add a second
            lblGameClock.Text = string.Format("{0:D2}:{1:D2}", spanCurTime.Minutes, spanCurTime.Seconds);

            _Xpression.SetClockWidgetTimerValue(_gameClockName, (int)spanCurTime.TotalMilliseconds);
        }

        private void numShotClock_ValueChanged(object sender, EventArgs e)
        {
            if (string.Equals("Disabled", btnShotClockDisable.Text))
                return;

            TurnOffShotClock();

            int seconds = (int)(numShotClock.Value);
            string startTime = string.Format("00:00:{0:D2}.000", seconds.ToString().PadLeft(2, '0'));
            _Xpression.EditClockWidgetStartTime(ConvertCustoms(Globals.GetObjectValue("Widget_ShotClock", propGridXpression.SelectedObject)), startTime);
            ShotClockTick(0, 0, seconds, 0);
        }

        private void btnShotClock_Click(object sender, EventArgs e)
        {
            if (string.Equals("Disabled", btnShotClockDisable.Text))
                return;

            if (string.Equals("Start", btnShotClock.Text))
            {
                TurnOnShotClock();
            }
            else
            {
                TurnOffShotClock();
            }
        }

        private void btnResetShotClock_Click(object sender, EventArgs e)
        {
            if (string.Equals("Enabled", btnShotClockDisable.Text))
                ResetShotClock();
        }

        private void btnShotClockDisable_Click(object sender, EventArgs e)
        {
            if (string.Equals("Disabled", btnShotClockDisable.Text))
            {
                btnShotClockDisable.Text = "Enabled";
                btnShotClockDisable.BackColor = System.Drawing.Color.DarkGreen;

                btnShotClock.Enabled = true;
                btnResetShotClock.Enabled = true;
                numShotClock.Enabled = true;
            }
            else
            {
                ResetShotClock();

                btnShotClockDisable.Text = "Disabled";
                btnShotClockDisable.BackColor = System.Drawing.Color.DarkRed;

                btnShotClock.Enabled = false;
                btnResetShotClock.Enabled = false;
                numShotClock.Enabled = false;
            }
        }

        private void cmbQtr_SelectedIndexChanged(object sender, EventArgs e)
        {
            _Xpression.SetTextListWidgetItemIndex(ConvertCustoms(Globals.GetObjectValue("Widget_Quarter", propGridXpression.SelectedObject)), cmbQtr.SelectedIndex);
        }

        private void lblCmbInfobox_Click(object sender, EventArgs e)
        {
            updateInfoLastEdited(0);
        }

        private void cmbInfobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateInfoLastEdited(0);
        }

        private void lblTxtInfobox_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInfobox.Text.Trim()))
                updateInfoLastEdited(1);
            else
                updateInfoLastEdited(0);
        }

        private void txtInfobox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInfobox.Text.Trim()))
                updateInfoLastEdited(1);
            else
                updateInfoLastEdited(0);
        }

        private void btnInfoboxClear_Click(object sender, EventArgs e)
        {
            txtInfobox.Text = string.Empty;
            updateInfoLastEdited(0);
        }

        private void btnAutoPoint_Click(object sender, EventArgs e)
        {
            _autoPoint = !_autoPoint;
            if (_autoPoint)
            {
                btnAutoPoint.Text = "Auto-Point Enabled";
                btnAutoPoint.BackColor = System.Drawing.Color.DarkGreen;
            }
            else
            {
                btnAutoPoint.Text = "Auto-Point Disabled";
                btnAutoPoint.BackColor = System.Drawing.Color.DarkRed;
            }
        }

        private void cmbHomeL3PlayerNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If live Take is on and the scene is already online, turn it off before editing
            if (string.Equals("Enabled", btnL3PlayerLiveTake.Text) && string.Equals("Online", btnTakeHomePlayerStats.Text))
            {
                btnTakeHomePlayerStats.PerformClick();
            }

            // Update Name dropdown
            cmbHomeL3PlayerName.SelectedIndex = cmbHomeL3PlayerNumber.SelectedIndex;

            // Check if we have a valid player selected
            if (cmbHomeL3PlayerName.SelectedIndex != 0)
            {
                // If live take is on, take the scene online
                if (string.Equals("Enabled", btnL3PlayerLiveTake.Text) && string.Equals("Offline", btnTakeHomePlayerStats.Text))
                {
                    btnTakeHomePlayerStats.PerformClick();
                }
            }
        }

        private void cmbHomeL3PlayerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update Number dropdown
            cmbHomeL3PlayerNumber.SelectedIndex = cmbHomeL3PlayerName.SelectedIndex;
        }

        private void btnHomeL3PlayerClear_Click(object sender, EventArgs e)
        {
            txtHomeL3PlayerInfo.Text = string.Empty;
        }

        private void cmbHomeL3CoachName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the proper DataGridView
            DataGridView dgv = dataGridCoachesHome;

            // If live Take is on and the scene is already online, turn it off before editing
            if (string.Equals("Enabled", btnL3CoachLiveTake.Text) && string.Equals("Online", btnTakeHomeCoachInfo.Text))
            {
                btnTakeHomeCoachInfo.PerformClick();
            }

            if (dgv.Rows.Count == 0 || cmbHomeL3CoachName.SelectedIndex == 0) return;

            dgv.AllowUserToAddRows = false;

            // Find coaches data based off their name
            int rowIndex = -1;

            DataGridViewRow row = dgv.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells["Name"].Value.ToString().Equals(cmbHomeL3CoachName.Text))
                .First();

            rowIndex = row.Index;
            if (rowIndex == -1) return;

            string coachName = row.Cells["Name"].Value.ToString();
            string coachTitle = row.Cells["Position"].Value.ToString();
            txtHomeL3CoachInfo.Text = coachTitle;

            // If live take is on, take the scene online
            if (string.Equals("Enabled", btnL3CoachLiveTake.Text) && string.Equals("Offline", btnTakeHomeCoachInfo.Text))
            {
                btnTakeHomeCoachInfo.PerformClick();
            }

            dgv.AllowUserToAddRows = true;
        }

        private void btnHomeL3CoachClear_Click(object sender, EventArgs e)
        {
            txtHomeL3CoachInfo.Text = string.Empty;
        }

        private void cmbAwayL3PlayerNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If live Take is on and the scene is already online, turn it off before editing
            if (string.Equals("Enabled", btnL3PlayerLiveTake.Text) && string.Equals("Online", btnTakeAwayPlayerStats.Text))
            {
                btnTakeAwayPlayerStats.PerformClick();
            }

            // Update Name dropdown
            cmbAwayL3PlayerName.SelectedIndex = cmbAwayL3PlayerNumber.SelectedIndex;

            // Check if we have a valid player selected
            if (cmbAwayL3PlayerName.SelectedIndex != 0)
            {
                // If live take is on, take the scene online
                if (string.Equals("Enabled", btnL3PlayerLiveTake.Text) && string.Equals("Offline", btnTakeAwayPlayerStats.Text))
                {
                    btnTakeAwayPlayerStats.PerformClick();
                }
            }
        }

        private void cmbAwayL3PlayerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update Number dropdown
            cmbAwayL3PlayerNumber.SelectedIndex = cmbAwayL3PlayerName.SelectedIndex;
        }

        private void btnAwayL3PlayerClear_Click(object sender, EventArgs e)
        {
            txtAwayL3PlayerInfo.Text = string.Empty;
        }

        private void cmbAwayL3CoachName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the proper DataGridView
            DataGridView dgv = dataGridCoachesAway;

            // If live Take is on and the scene is already online, turn it off before editing
            if (string.Equals("Enabled", btnL3CoachLiveTake.Text) && string.Equals("Online", btnTakeAwayCoachInfo.Text))
            {
                btnTakeAwayCoachInfo.PerformClick();
            }

            if (dgv.Rows.Count == 0 || cmbAwayL3CoachName.SelectedIndex == 0) return;

            dgv.AllowUserToAddRows = false;

            // Find coaches data based off their name
            int rowIndex = -1;

            DataGridViewRow row = dgv.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells["Name"].Value.ToString().Equals(cmbAwayL3CoachName.Text))
                .First();

            rowIndex = row.Index;
            if (rowIndex == -1) return;

            string coachName = row.Cells["Name"].Value.ToString();
            string coachTitle = row.Cells["Position"].Value.ToString();
            txtAwayL3CoachInfo.Text = coachTitle;


            // If live take is on, take the scene online
            if (string.Equals("Enabled", btnL3CoachLiveTake.Text) && string.Equals("Offline", btnTakeAwayCoachInfo.Text))
            {
                btnTakeAwayCoachInfo.PerformClick();
            }

            dgv.AllowUserToAddRows = true;
        }

        private void btnAwayL3CoachClear_Click(object sender, EventArgs e)
        {
            txtAwayL3CoachInfo.Text = string.Empty;
        }

        private void btnL3PlayerLiveTake_Click(object sender, EventArgs e)
        {
            Button takeButton = btnL3PlayerLiveTake;
            if (string.Equals("Disabled", takeButton.Text))
            {
                takeButton.Text = "Enabled";
                takeButton.BackColor = System.Drawing.Color.DarkGreen;

            }
            else
            {
                takeButton.Text = "Disabled";
                takeButton.BackColor = System.Drawing.Color.DarkRed;
            }
        }

        private void btnL3CoachLiveTake_Click(object sender, EventArgs e)
        {
            Button takeButton = btnL3CoachLiveTake;
            if (string.Equals("Disabled", takeButton.Text))
            {
                takeButton.Text = "Enabled";
                takeButton.BackColor = System.Drawing.Color.DarkGreen;
            }
            else
            {
                takeButton.Text = "Disabled";
                takeButton.BackColor = System.Drawing.Color.DarkRed;
            }
        }

        #endregion

        #region Stats Tab
        private void dataGridStats_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridStats.RemoveEmptyRows();
        }

        private void dataGridStats_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridStats.RemoveEmptyRows();
        }

        private void btnSaveStats_Click(object sender, EventArgs e)
        {
            string _statsFile = string.Format("{0}\\Stats.xlsx", eventDataDirectoryPath);

            if (!string.IsNullOrWhiteSpace(eventDataDirectoryPath) || !string.IsNullOrWhiteSpace(_statsFile))
            {
                dataGridStats.RemoveEmptyRows();
                SaveDataTable((DataTable)dataGridStats.DataSource, _statsFile, 0);
            }
        }

        #endregion

        #region Rosters Tab
        private void dataGridRosterHome_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridRosterHome.RemoveEmptyRows();
            populateTeamLowerThirds(Teams.Home);
        }

        private void dataGridRosterHome_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridRosterHome.RemoveEmptyRows();
        }

        private void dataGridRosterAway_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridRosterAway.RemoveEmptyRows();
            populateTeamLowerThirds(Teams.Away);
        }

        private void dataGridRosterAway_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridRosterAway.RemoveEmptyRows();
        }

        private void btnSaveRosters_Click(object sender, EventArgs e)
        {
            string _rostersFile = string.Format("{0}\\Rosters.xlsx", eventDataDirectoryPath);

            if (!string.IsNullOrWhiteSpace(eventDataDirectoryPath) || !string.IsNullOrWhiteSpace(_rostersFile))
            {
                dataGridRosterHome.RemoveEmptyRows();
                dataGridRosterHome.Sort(dataGridRosterHome.Columns["Number"], System.ComponentModel.ListSortDirection.Ascending);
                SaveDataTable((DataTable)dataGridRosterHome.DataSource, _rostersFile, _home_roster_id >= 0 ? _home_roster_id : 0);

                dataGridRosterAway.RemoveEmptyRows();
                dataGridRosterAway.Sort(dataGridRosterAway.Columns["Number"], System.ComponentModel.ListSortDirection.Ascending);
                SaveDataTable((DataTable)dataGridRosterAway.DataSource, _rostersFile, _away_roster_id >= 0 ? _away_roster_id : 0);
            }
        }

        #endregion

        #region Coaches Tab
        private void dataGridCoachesHome_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridCoachesHome.RemoveEmptyRows();
        }

        private void dataGridCoachesHome_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridCoachesHome.RemoveEmptyRows();
        }
        private void dataGridCoachesAway_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridCoachesAway.RemoveEmptyRows();
        }

        private void dataGridCoachesAway_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridCoachesAway.RemoveEmptyRows();
        }

        private void btnSaveCoaches_Click(object sender, EventArgs e)
        {
            string _coachesFile = string.Format("{0}\\Coaches.xlsx", eventDataDirectoryPath);

            if (!string.IsNullOrWhiteSpace(eventDataDirectoryPath) || !string.IsNullOrWhiteSpace(_coachesFile))
            {
                dataGridCoachesHome.RemoveEmptyRows();
                SaveDataTable((DataTable)dataGridCoachesHome.DataSource, _coachesFile, _home_coach_id >= 0 ? _home_coach_id : 0);

                dataGridCoachesAway.RemoveEmptyRows();
                SaveDataTable((DataTable)dataGridCoachesAway.DataSource, _coachesFile, _away_coach_id >= 0 ? _away_coach_id : 0);
            }
        }

        #endregion

        #region Lower Third Setup Tab

        private void dataGridL3Setup_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridL3Setup.RemoveEmptyRows();
        }

        private void dataGridL3Setup_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridL3Setup.RemoveEmptyRows();
            populateEventLowerThirds();
        }

        private void btnSaveL3Setup_Click(object sender, EventArgs e)
        {
            // Save the event extras to their .xlsx file
            string _lowerThirdsFile = string.Format("{0}\\LowerThirds.xlsx", eventDataDirectoryPath);

            if (!string.IsNullOrWhiteSpace(eventDataDirectoryPath) || !string.IsNullOrWhiteSpace(_lowerThirdsFile))
            {
                dataGridL3Setup.RemoveEmptyRows();
                SaveDataTable((DataTable)dataGridL3Setup.DataSource, ConvertCustoms(_lowerThirdsFile));
            }
        }

        #endregion

        #region Credits Tab

        private void dataGridCredits_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridCredits.Rows[e.RowIndex] != null && dataGridCredits.Rows[e.RowIndex].Cells[1] != null && dataGridCredits.Rows[e.RowIndex].Cells[1].Value != null)
                dataGridCredits.Rows[e.RowIndex].Cells[1].Value = dataGridCredits.Rows[e.RowIndex].Cells[1].Value.ToString().ToUpper().Trim();

            dataGridCredits.RemoveEmptyRows();
        }

        private void dataGridCredits_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridCredits.RemoveEmptyRows();
        }

        private void btnSaveCredits_Click(object sender, EventArgs e)
        {
            // Save the event extras to their .xlsx file
            string _creditsFile = string.Format("{0}\\Credits.xlsx", eventDataDirectoryPath);

            if (!string.IsNullOrWhiteSpace(eventDataDirectoryPath) || !string.IsNullOrWhiteSpace(_creditsFile))
            {
                dataGridCredits.RemoveEmptyRows();
                SaveDataTable((DataTable)dataGridCredits.DataSource, ConvertCustoms(_creditsFile));
            }
        }


        #endregion

        #region Main TakeItems Tab
        private void btnTakeScorebug_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Scorebug_ID").First();

            if (!takeItem.IsOnline)
            {
                TurnOnTakeItem(takeItem);
            }
            else
            {
                // Make sure anything on the info bug layer is turned off
                TakeItem infoItem = takeItems.Where(item => item.Name == "Scorebug_Info_ID").First();
                if (infoItem != null)
                    TurnOffTakeItemLayer(infoItem);

                TurnOffTakeItem(takeItem);
            }
        }

        private void btnTakeNetwork_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Misc_Network_Bug").First();

            if (!takeItem.IsOnline)
                TurnOnTakeItem(takeItem);
            else
                TurnOffTakeItem(takeItem);
        }

        private void btnTakeReplay_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Misc_Replay_Bug").First();

            if (!takeItem.IsOnline)
                TurnOnTakeItem(takeItem);
            else
                TurnOffTakeItem(takeItem);
        }

        private void btnTakeInfobox_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Scorebug_Info_ID").First();

            // Check if take item is online
            if (!takeItem.IsOnline)
            {
                // Make sure the base scorebug is online
                TakeItem scorebugItem = takeItems.Where(item => item.Name == "Scorebug_ID").First();
                if (scorebugItem != null)
                {
                    // Make sure the scorebug is online and if not make sure it went online
                    if (scorebugItem.IsOnline || TurnOnTakeItem(scorebugItem))
                    {
                        string boxText = ConvertCustoms(txtInfobox.Text.Trim());
                        if (scoreboardData.InfoLastEdited == 0 || string.IsNullOrEmpty(boxText))
                            takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Scorebug_Info_Name", propGridXpression.SelectedObject)), "Text", ConvertCustoms(cmbInfobox.Text.Trim()));
                        else
                            takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Scorebug_Info_Name", propGridXpression.SelectedObject)), "Text", boxText);

                        TurnOnTakeItem(takeItem);
                    }
                }
            }
            else
            {
                TurnOffTakeItem(takeItem);
                // Revert to w/e our select box is for next time we show the bug
                updateInfoLastEdited(0);
            }
        }

        private void btnTakeHomePlayerStats_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_PlayerStats_ID").First();

            if (ToggleTakeButton(btnTakeHomePlayerStats))
            {
                // Turn off all other lower thirds
                ToggleLowerThirds(btnTakeHomePlayerStats);
                // Take scene online and if it failed, turn off button
                if (!TakePlayerStats(takeItem, Teams.Home))
                    ToggleTakeButton(btnTakeHomePlayerStats);
            }
            else
            {
                takeItem.SetOffline();
            }
        }

        private void btnTakeHomePlayerInfo_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_PlayerInfo_ID").First();

            if (ToggleTakeButton(btnTakeHomePlayerInfo))
            {
                // Turn off all other lower thirds
                ToggleLowerThirds(btnTakeHomePlayerInfo);
                // Take scene online and if it failed, turn off button
                if (!TakePlayerInfo(takeItem, Teams.Home))
                    ToggleTakeButton(btnTakeHomePlayerInfo);
            }
            else
            {
                takeItem.SetOffline();
            }
        }

        private void btnTakeHomeCoachInfo_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_TeamInfo_ID").First();

            if (ToggleTakeButton(btnTakeHomeCoachInfo))
            {
                // Turn off all other lower thirds
                ToggleLowerThirds(btnTakeHomeCoachInfo);
                // Take scene online and if it failed, turn off button
                if (!TakeCoachInfo(takeItem, Teams.Home))
                    ToggleTakeButton(btnTakeHomeCoachInfo);
            }
            else
            {
                takeItem.SetOffline();
            }
        }

        private void btnTakeAwayPlayerStats_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_PlayerStats_ID").First();

            if (ToggleTakeButton(btnTakeAwayPlayerStats))
            {
                // Turn off all other lower thirds
                ToggleLowerThirds(btnTakeAwayPlayerStats);
                // Take scene online and if it failed, turn off button
                if (!TakePlayerStats(takeItem, Teams.Away))
                    ToggleTakeButton(btnTakeAwayPlayerStats);
            }
            else
            {
                takeItem.SetOffline();
            }
        }

        private void btnTakeAwayPlayerInfo_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_PlayerInfo_ID").First();

            if (ToggleTakeButton(btnTakeAwayPlayerInfo))
            {
                // Turn off all other lower thirds
                ToggleLowerThirds(btnTakeAwayPlayerInfo);
                // Take scene online and if it failed, turn off button
                if (!TakePlayerInfo(takeItem, Teams.Away))
                    ToggleTakeButton(btnTakeAwayPlayerInfo);
            }
            else
            {
                takeItem.SetOffline();
            }
        }

        private void btnTakeAwayCoachInfo_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_TeamInfo_ID").First();

            if (ToggleTakeButton(btnTakeAwayCoachInfo))
            {
                // Turn off all other lower thirds
                ToggleLowerThirds(btnTakeAwayCoachInfo);
                // Take scene online and if it failed, turn off button
                if (!TakeCoachInfo(takeItem, Teams.Away))
                    ToggleTakeButton(btnTakeAwayCoachInfo);
            }
            else
            {
                takeItem.SetOffline();
            }
        }

        private void btnTakeCustomL3_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_Custom_ID").First();

            if (ToggleTakeButton(btnTakeCustomL3))
            {
                // Turn off all other lower thirds
                ToggleLowerThirds(btnTakeCustomL3);
                // Take scene online and if it failed, turn off button
                if (!TakeCustomL3(takeItem))
                    ToggleTakeButton(btnTakeCustomL3);
            }
            else
            {
                takeItem.SetOffline();
            }
        }

        private void btnClearCustomL3_Click(object sender, EventArgs e)
        {
            txtTakeCustomL3.Text = string.Empty;
        }

        #endregion

        #region Bumpers Tab

        private void btnTakeBumperScore_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Bumper_Score_ID").First();

            // Make  sure the scene is offline
            if (!takeItem.IsOnline)
            {
                // Update properties and take scene onlines
                try
                {
                    // Edit TakeItem poroperties with new values
                    if (string.IsNullOrEmpty(txtInfobox.Text.Trim()))
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Score_Heading", propGridXpression.SelectedObject)), "Text", ConvertCustoms(cmbInfobox.Text));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Score_Heading", propGridXpression.SelectedObject)), "Text", ConvertCustoms(txtInfobox.Text.Trim()));
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Score_Info", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_Score_Info_Value", propGridXpression.SelectedObject)));
                    string hommeTeam = ConvertCustoms(Globals.GetObjectValue("Bumper_Score_HomeTeam", propGridXpression.SelectedObject));
                    string awayTeam = ConvertCustoms(Globals.GetObjectValue("Bumper_Score_AwayTeam", propGridXpression.SelectedObject));
                    if (!string.IsNullOrEmpty(hommeTeam) && !string.IsNullOrEmpty(awayTeam))
                    {
                        string _homeSchool = ConvertCustoms(Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject));
                        string _awaySchool = ConvertCustoms(Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject));
                        if (!string.IsNullOrEmpty(_homeSchool) && !string.IsNullOrEmpty(_awaySchool))
                        {
                            takeItem.EditProperty(hommeTeam, "Text", _homeSchool);
                            takeItem.EditProperty(awayTeam, "Text", _awaySchool);
                        }
                    }

                    // if we had no errors take online
                    TurnOnTakeItem(takeItem);
                }
                catch { return; }
            }
            else
            {
                // Try turning off layer items
                TurnOffTakeItemLayer(takeItem);
            }
        }

        private void btnTakeBumperLocator_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Bumper_Locator_ID").First();

            // Make  sure the scene is offline
            if (!takeItem.IsOnline)
            {
                // Update properties and take scene online
                try
                {
                    // Edit TakeItem poroperties with new values
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Locator_Heading", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("EventTitle", propGridEvent.SelectedObject)));
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Locator_Venue", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("EventVenue", propGridEvent.SelectedObject)));
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Locator_Location", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("EventLocation", propGridEvent.SelectedObject)));

                    // if we had no errors take online
                    TurnOnTakeItem(takeItem);
                }
                catch { return; }
            }
            else
            {
                // Try turning off layer items
                TurnOffTakeItemLayer(takeItem);
            }
        }

        private void btnTakeBumperHeadToHead_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Bumper_HeadToHead_ID").First();

            // Make  sure the scene is offline
            if (!takeItem.IsOnline)
            {
                // Update properties and take scene online
                try
                {
                    // Get the team school names
                    string _homeSchool = ConvertCustoms(Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject));
                    string _awaySchool = ConvertCustoms(Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject));

                    // Edit TakeItem poroperties with new values
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Heading", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Heading_Value", propGridXpression.SelectedObject)));

                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat1", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat1_Value", propGridXpression.SelectedObject)));
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat2", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat2_Value", propGridXpression.SelectedObject)));
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat3", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat3_Value", propGridXpression.SelectedObject)));

                    // Load the team stats into their properties
                    DataGridView _teamStats = dataGridStats;
                    if (_teamStats != null)
                    {
                        _teamStats.AllowUserToAddRows = false;
                        _teamStats.RemoveEmptyRows();

                        // Load the home team data
                        DataGridViewRow homeRow = GetDataGridViewRow(_teamStats, "School", _homeSchool);
                        // Check if the row was found
                        if (homeRow != null)
                        {
                            string _home_stat1 = ConvertCustoms((Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat1_Value", propGridXpression.SelectedObject)));
                            string _home_stat2 = ConvertCustoms((Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat2_Value", propGridXpression.SelectedObject)));
                            string _home_stat3 = ConvertCustoms((Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat3_Value", propGridXpression.SelectedObject)));

                            if (_home_stat1 != string.Empty)
                            {
                                DataGridViewCell cellStat = GetDataGridViewCell(_teamStats, homeRow, _home_stat1);
                                string _value = "";
                                if (cellStat != null)
                                    _value = cellStat.Value.ToString().Trim();
                                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Home_Stat1", propGridXpression.SelectedObject)), "Text", ConvertCustoms(_value));
                            }

                            if (_home_stat2 != string.Empty)
                            {
                                DataGridViewCell cellStat = GetDataGridViewCell(_teamStats, homeRow, _home_stat2);
                                string _value = "";
                                if (cellStat != null)
                                    _value = cellStat.Value.ToString().Trim();
                                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Home_Stat2", propGridXpression.SelectedObject)), "Text", ConvertCustoms(_value));
                            }

                            if (_home_stat3 != string.Empty)
                            {
                                DataGridViewCell cellStat = GetDataGridViewCell(_teamStats, homeRow, _home_stat3);
                                string _value = "";
                                if (cellStat != null)
                                    _value = cellStat.Value.ToString().Trim();
                                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Home_Stat3", propGridXpression.SelectedObject)), "Text", ConvertCustoms(_value));
                            }
                        }

                        // Load the away team data
                        DataGridViewRow awayRow = GetDataGridViewRow(_teamStats, "School", _awaySchool);
                        // Check if the row was found
                        if (awayRow != null && awayRow.Index >= 0)
                        {
                            string _away_stat1 = ConvertCustoms((Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat1_Value", propGridXpression.SelectedObject)));
                            string _away_stat2 = ConvertCustoms((Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat2_Value", propGridXpression.SelectedObject)));
                            string _away_stat3 = ConvertCustoms((Globals.GetObjectValue("Bumper_HeadToHead_Label_Stat3_Value", propGridXpression.SelectedObject)));

                            if (_away_stat1 != string.Empty)
                            {
                                DataGridViewCell cellStat = GetDataGridViewCell(_teamStats, awayRow, _away_stat1);
                                string _value = "";
                                if (cellStat != null)
                                    _value = cellStat.Value.ToString().Trim();
                                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Away_Stat1", propGridXpression.SelectedObject)), "Text", ConvertCustoms(_value));
                            }

                            if (_away_stat2 != string.Empty)
                            {
                                DataGridViewCell cellStat = GetDataGridViewCell(_teamStats, awayRow, _away_stat2);
                                string _value = "";
                                if (cellStat != null)
                                    _value = cellStat.Value.ToString().Trim();
                                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Away_Stat2", propGridXpression.SelectedObject)), "Text", ConvertCustoms(_value));
                            }

                            if (_away_stat3 != string.Empty)
                            {
                                DataGridViewCell cellStat = GetDataGridViewCell(_teamStats, awayRow, _away_stat3);
                                string _value = "";
                                if (cellStat != null)
                                    _value = cellStat.Value.ToString().Trim();
                                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_HeadToHead_Away_Stat3", propGridXpression.SelectedObject)), "Text", ConvertCustoms(_value));
                            }
                        }
                        _teamStats.AllowUserToAddRows = true;
                    }

                    // if we had no errors take online
                    TurnOnTakeItem(takeItem);
                }
                catch { return; }
            }
            else
            {
                // Try turning off layer items
                TurnOffTakeItemLayer(takeItem);
            }
        }

        private void btnTakeBumperStandings_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Bumper_Standings_ID").First();

            // Make  sure the scene is offline
            if (!takeItem.IsOnline)
            {
                // Update properties and take scene online
                try
                {
                    // Edit TakeItem poroperties with new values
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Standings_Heading", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_Standings_Heading_Value", propGridXpression.SelectedObject)));
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Standings_Title", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_Standings_Title_Value", propGridXpression.SelectedObject)));
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Standings_Background", propGridXpression.SelectedObject)), "Visible", Globals.GetObjectValue("Bumper_Standings_Background_Visible", propGridXpression.SelectedObject));

                    // Load the team stats into their properties
                    DataGridView _standings = dataGridStats;
                    if (_standings != null)
                    {
                        _standings.AllowUserToAddRows = false;
                        // Sort by Win percentage
                        _standings.Sort(_standings.Columns["Points"], System.ComponentModel.ListSortDirection.Descending);
                        _standings.Sort(_standings.Columns["Win %"], System.ComponentModel.ListSortDirection.Descending);

                        string _homeName = ConvertCustoms(Globals.GetObjectValue("HomeName", propGridEvent.SelectedObject));
                        string _awayName = ConvertCustoms(Globals.GetObjectValue("AwayName", propGridEvent.SelectedObject));
                        string _homeSchool = ConvertCustoms(Globals.GetObjectValue("HomeSchool", propGridEvent.SelectedObject));
                        string _awaySchool = ConvertCustoms(Globals.GetObjectValue("AwaySchool", propGridEvent.SelectedObject));
                        string _groupBase = ConvertCustoms((Globals.GetObjectValue("Bumper_Standings_GroupBase", propGridXpression.SelectedObject)));
                        string _schoolBase = ConvertCustoms((Globals.GetObjectValue("Bumper_Standings_SchoolBase", propGridXpression.SelectedObject)));
                        string _recordBase = ConvertCustoms((Globals.GetObjectValue("Bumper_Standings_RecordBase", propGridXpression.SelectedObject)));
                        string _pointsBase = ConvertCustoms((Globals.GetObjectValue("Bumper_Standings_PointsBase", propGridXpression.SelectedObject)));
                        if (!int.TryParse(ConvertCustoms(Globals.GetObjectValue("Bumper_Standings_MaxTeams", propGridXpression.SelectedObject)), out int _maxTeams))
                            _maxTeams = new XpressionData().Bumper_Standings_MaxTeams;

                        if (_maxTeams > 0 && !string.IsNullOrEmpty(_groupBase) && !string.IsNullOrEmpty(_schoolBase) &&
                            !string.IsNullOrEmpty(_recordBase) && !string.IsNullOrEmpty(_pointsBase))
                        {
                            // Loop through until we reach max teams
                            for (int i = 0; i < _maxTeams + 1; i++)
                            {
                                int _groupNum = i + 1;
                                try
                                {
                                    // Load the team data
                                    DataGridViewRow curRow = _standings.Rows[i];
                                    // Check if the row was found
                                    if (curRow != null)
                                    {
                                        DataGridViewCell _cellSchool = GetDataGridViewCell(_standings, curRow, "School");
                                        string _valueSchool = "";
                                        if (_cellSchool != null)
                                            _valueSchool = ConvertCustoms(_cellSchool.Value.ToString().Trim());

                                        DataGridViewCell _cellRecord = GetDataGridViewCell(_standings, curRow, "Record");
                                        string _valueRecord = "";
                                        if (_cellRecord != null)
                                            _valueRecord = ConvertCustoms(_cellRecord.Value.ToString().Trim());

                                        DataGridViewCell _cellPoints = GetDataGridViewCell(_standings, curRow, "Points");
                                        string _valuePoints = "";
                                        if (_cellPoints != null)
                                            _valuePoints = ConvertCustoms(_cellPoints.Value.ToString().Trim());

                                        // Make sure all data was found
                                        if (!string.IsNullOrEmpty(_valueSchool) && !string.IsNullOrEmpty(_valueRecord) && !string.IsNullOrEmpty(_valuePoints))
                                        {
                                            takeItem.EditProperty(string.Format("{0}{1}", _schoolBase, _groupNum), "Text", _valueSchool);
                                            takeItem.EditProperty(string.Format("{0}{1}", _recordBase, _groupNum), "Text", _valueRecord);
                                            takeItem.EditProperty(string.Format("{0}{1}", _pointsBase, _groupNum), "Text", _valuePoints);
                                            if (_homeName.Contains(_valueSchool, true) || _homeSchool.Contains(_valueSchool, true) ||
                                                _awayName.Contains(_valueSchool, true) || _awaySchool.Contains(_valueSchool, true))
                                                takeItem.EditProperty(string.Format("highlight{0}", _groupNum), "Visibility", "true");
                                            else
                                                takeItem.EditProperty(string.Format("highlight{0}", _groupNum), "Visibility", "false");

                                            takeItem.EditProperty(string.Format("{0}{1}", _groupBase, _groupNum), "Visibility", "true");
                                        }
                                        else // Invalid data for row. Hide it
                                            takeItem.EditProperty(string.Format("{0}{1}", _groupBase, _groupNum), "Visibility", "false");
                                    }
                                    else // Invalid data for row. Hide it
                                        takeItem.EditProperty(string.Format("{0}{1}", _groupBase, _groupNum), "Visibility", "false");

                                } // Something happened during loading, hide the row
                                catch { takeItem.EditProperty(string.Format("{0}{1}", _groupBase, _groupNum), "Visibility", "false"); }
                            }

                            // if we had no errors take online
                            TurnOnTakeItem(takeItem);
                        }
                        _standings.AllowUserToAddRows = true;
                    }
                }
                catch { return; }
            }
            else
            {
                // Try turning off layer items
                TurnOffTakeItemLayer(takeItem);
            }
        }

        private void btnTakeBumperCustom_Click(object sender, EventArgs e)
        {
            int takeID = (int)numTakeBumperCustom.Value;
            TakeItem takeItem = new TakeItem(string.Format("_custom_{0}", takeID), takeID, btnTakeBumperCustom, _Xpression.GetTakeItem(takeID));

            if (!takeItem.IsOnline)
                TurnOnTakeItem(takeItem);
            else
                TurnOffTakeItem(takeItem);
        }

        #endregion

        #region Lower Thirds Tab

        private (string, string) GetL3SetupLeftData()
        {
            string _title = "";
            string _subtitle = "";

            try
            {
                if (!chkL3SetupCustomLeft.Checked)
                {
                    // Get value from checkbox
                    int index = cmbL3SetupLeft.SelectedIndex;
                    if (dataGridL3Setup.Rows.Count >= index)
                    {
                        DataGridViewRow row = dataGridL3Setup.Rows[index];
                        _title = row.Cells[0].Value.ToString().Trim();
                        _subtitle = row.Cells[1].Value.ToString().Trim();
                    }
                }
                else
                {
                    _title = txtL3SetupLeftName.Text.Trim();
                    _subtitle = txtL3SetupLeftTitle.Text.Trim();
                }
            }
            catch { return (_title, _subtitle); }

            return (_title, _subtitle);
        }

        private (string, string) GetL3SetupMiddleData()
        {
            string _title = "";
            string _subtitle = "";

            try
            {
                if (!chkL3SetupCustomMiddle.Checked)
                {
                    // Get value from checkbox
                    int index = cmbL3SetupMiddle.SelectedIndex;
                    if (dataGridL3Setup.Rows.Count >= index)
                    {
                        DataGridViewRow row = dataGridL3Setup.Rows[index];
                        _title = row.Cells[0].Value.ToString().Trim();
                        _subtitle = row.Cells[1].Value.ToString().Trim();
                    }
                }
                else
                {
                    _title = txtL3SetupMiddleName.Text.Trim();
                    _subtitle = txtL3SetupMiddleTitle.Text.Trim();
                }
            }
            catch { return (_title, _subtitle); }

            return (_title, _subtitle);
        }

        private (string, string) GetL3SetupRightData()
        {
            string _title = "";
            string _subtitle = "";

            try
            {
                if (!chkL3SetupCustomRight.Checked)
                {
                    // Get value from checkbox
                    int index = cmbL3SetupRight.SelectedIndex;
                    if (dataGridL3Setup.Rows.Count >= index)
                    {
                        DataGridViewRow row = dataGridL3Setup.Rows[index];
                        _title = row.Cells[0].Value.ToString().Trim();
                        _subtitle = row.Cells[1].Value.ToString().Trim();
                    }
                }
                else
                {
                    _title = txtL3SetupRightName.Text.Trim();
                    _subtitle = txtL3SetupRightTitle.Text.Trim();
                }
            }
            catch { return (_title, _subtitle); }

            return (_title, _subtitle);
        }

        private void btnTakeL3Setup_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "L3_EventExtras_ID").First();

            // Make sure the take item isn't already online
            if (!takeItem.IsOnline)
            {
                // Update properties and take scene online
                try
                {
                    // Find out which sections are active
                    bool leftActive = chkL3SetupActiveLeft.Checked;
                    bool middleAcive = chkL3SetupActiveMiddle.Checked;
                    bool rightActive = chkL3SetupActiveRight.Checked;
                    (string, string) _leftData = GetL3SetupLeftData();
                    (string, string) _middleData = GetL3SetupMiddleData();
                    (string, string) _rightData = GetL3SetupRightData();
                    if (leftActive && !middleAcive && !rightActive)
                    {
                        // Show the single l3
                        if (string.IsNullOrEmpty(_leftData.Item1) && string.IsNullOrEmpty(_leftData.Item2))
                            return; // No data to display

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Title", propGridXpression.SelectedObject)), "Text", string.Empty);

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Title", propGridXpression.SelectedObject)), "Text", string.Empty);
                    }
                    else if (!leftActive && !middleAcive && rightActive)
                    {
                        // Show the double l3 with onlu right data
                        if (string.IsNullOrEmpty(_rightData.Item1) && string.IsNullOrEmpty(_rightData.Item2))
                            return; // No data to display

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Title", propGridXpression.SelectedObject)), "Text", string.Empty);

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Title", propGridXpression.SelectedObject)), "Text", string.Empty);

                    }
                    else if (!leftActive && middleAcive && !rightActive)
                    {
                        // Show the triple l3 with only mid data
                        if (string.IsNullOrEmpty(_middleData.Item1) && string.IsNullOrEmpty(_middleData.Item2))
                            return; // No data to display

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Title", propGridXpression.SelectedObject)), "Text", string.Empty);

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Title", propGridXpression.SelectedObject)), "Text", string.Empty);

                    }
                    else if (leftActive && middleAcive && !rightActive)
                    {
                        // Show the triple l3 with left and mid data
                        if (string.IsNullOrEmpty(_leftData.Item1) && string.IsNullOrEmpty(_leftData.Item2) &&
                            string.IsNullOrEmpty(_middleData.Item1) && string.IsNullOrEmpty(_middleData.Item2))
                            return; // No data to display

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Title", propGridXpression.SelectedObject)), "Text", string.Empty);

                    }
                    else if (leftActive && !middleAcive && rightActive)
                    {
                        // Show the double l3 with left and right data
                        if (string.IsNullOrEmpty(_leftData.Item1) && string.IsNullOrEmpty(_leftData.Item2) &&
                            string.IsNullOrEmpty(_rightData.Item1) && string.IsNullOrEmpty(_rightData.Item2))
                            return; // No data to display

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Title", propGridXpression.SelectedObject)), "Text", string.Empty);
                    }
                    else if (!leftActive && middleAcive && rightActive)
                    {
                        // Show the triple l3 with mid and right data
                        if (string.IsNullOrEmpty(_middleData.Item1) && string.IsNullOrEmpty(_middleData.Item2) &&
                            string.IsNullOrEmpty(_rightData.Item1) && string.IsNullOrEmpty(_rightData.Item2))
                            return; // No data to display

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Name", propGridXpression.SelectedObject)), "Text", string.Empty);
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Title", propGridXpression.SelectedObject)), "Text", string.Empty);

                    }
                    else if (leftActive && middleAcive && rightActive)
                    {
                        // Show the triple l3 with all data
                        if (string.IsNullOrEmpty(_leftData.Item1) && string.IsNullOrEmpty(_leftData.Item2) &&
                            string.IsNullOrEmpty(_middleData.Item1) && string.IsNullOrEmpty(_middleData.Item2) &&
                            string.IsNullOrEmpty(_rightData.Item1) && string.IsNullOrEmpty(_rightData.Item2))
                            return; // No data to display

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Left_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_leftData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Middle_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_middleData.Item2));

                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Name", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item1));
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Right_Title", propGridXpression.SelectedObject)), "Text",
                            ConvertCustoms(_rightData.Item2));
                    }
                    else
                    {
                        // Nothing was set to go active so close out
                        return;
                    }

                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("L3_EventExtras_Title", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("EventTitle", propGridEvent.SelectedObject)));

                    TurnOnTakeItem(takeItem);
                }
                catch { }
            }
            else
            {
                // Try turning off all lower third scene options
                TurnOffTakeItemLayer(takeItem);
            }
        }

        private void btnL3SetupClearLeft_Click(object sender, EventArgs e)
        {
            cmbL3SetupLeft.SelectedItem = null;
            chkL3SetupActiveLeft.Checked = false;
            chkL3SetupCustomLeft.Checked = false;
            txtL3SetupLeftName.Text = string.Empty;
            txtL3SetupLeftTitle.Text = string.Empty;
        }

        private void btnL3SetupClearMiddle_Click(object sender, EventArgs e)
        {
            cmbL3SetupMiddle.SelectedItem = null;
            chkL3SetupActiveMiddle.Checked = false;
            chkL3SetupCustomMiddle.Checked = false;
            txtL3SetupMiddleName.Text = string.Empty;
            txtL3SetupMiddleTitle.Text = string.Empty;
        }

        private void btnL3SetupClearRight_Click(object sender, EventArgs e)
        {
            cmbL3SetupRight.SelectedItem = null;
            chkL3SetupActiveRight.Checked = false;
            chkL3SetupCustomRight.Checked = false;
            txtL3SetupRightName.Text = string.Empty;
            txtL3SetupRightTitle.Text = string.Empty;
        }

        #endregion

        #region Credits Tab

        public void EndOfCreditsData()
        {
            _creditsTmr.Stop();
            _creditsTmr.Enabled = false;
            _creditsTmr = null;
            _credits_rowIndex = 0;
            progressBarCredits.Visible = false;
            progressBarCredits.Value = 0;
        }

        public void ShowCreditData(TakeItem takeItemMain, TakeItem takeItemCopyright)
        {
            try
            {
                if (_credits_rowIndex < _credits_rowCount)
                {
                    string creditsData = "";
                    for (int i = 0; i < _credits_namesPerPage; i++)
                    {
                        try
                        {
                            if (_credits_rowIndex < _credits_rowCount)
                            {
                                string title = dataGridCredits.Rows[_credits_rowIndex].Cells[0].Value.ToString().Replace('_', ' ');
                                string name = dataGridCredits.Rows[_credits_rowIndex].Cells[1].Value.ToString();
                                creditsData = string.Format("{0}{1} - {2}\n", creditsData, title, name);
                            }
                        }
                        catch { }
                        finally
                        {
                            _credits_rowIndex++;
                        }
                    }

                    takeItemMain.EditProperty(ConvertCustoms(Globals.GetObjectValue("Credits_Text", propGridXpression.SelectedObject)), "Text", ConvertCustoms(creditsData));
                    TurnOnTakeItem(takeItemMain);
                    progressBarCredits.Visible = true;
                    progressBarCredits.Value = _credits_rowIndex;
                }
                else if (chkCreditsCopyright.Checked)
                {
                    if (!takeItemCopyright.IsOnline)
                        TurnOnTakeItem(takeItemCopyright);
                    else
                    {
                        EndOfCreditsData();

                        // Try turning off all credit take items
                        TurnOffTakeItemLayer(takeItemMain);
                    }
                }
                else
                {
                    EndOfCreditsData();

                    // Try turning off all credit take items
                    TurnOffTakeItemLayer(takeItemMain);
                }
            }
            catch { };
        }

        private void StartCredits(TakeItem takeItemMain, TakeItem takeItemCopyright, bool withTimer = true)
        {
            dataGridCredits.RemoveEmptyRows();

            _credits_namesPerPage = (int)numCreditsNames.Value;
            _credits_rowCount = dataGridCredits.Rows.Count;
            _credits_rowIndex = 0;
            progressBarCredits.Value = 0;
            progressBarCredits.Maximum = _credits_rowCount;
            progressBarCredits.Visible = true;
            ShowCreditData(takeItemMain, takeItemCopyright);

            if (withTimer)
            {
                // Reset & Start the timer
                void Credits_Timer_Tick(object sender, EventArgs e)
                {
                    ShowCreditData(takeItemMain, takeItemCopyright);
                }

                _creditsTmr = new Timer
                {
                    Interval = (int)TimeSpan.FromSeconds((double)numCreditsSeconds.Value).TotalMilliseconds
                };
                _creditsTmr.Tick += new EventHandler(Credits_Timer_Tick);
                _creditsTmr.Enabled = true;
                _creditsTmr.Start();
            }
        }

        private void btnTakeCredits_Click(object sender, EventArgs e)
        {
            TakeItem takeItemMain = takeItems.Where(item => item.Name == "Credits_ID").First();
            TakeItem takeItemCopyright = takeItems.Where(item => item.Name == "Credits_Copyright_ID").First();

            // Make sure all of the scene options are offline
            if (!takeItemMain.IsOnline && !takeItemCopyright.IsOnline)
                StartCredits(takeItemMain, takeItemCopyright);
            else
            {
                EndOfCreditsData();

                // Try turning off all credit take items
                TurnOffTakeItemLayer(takeItemMain);
            }
        }

        private void btnTakeNextCredits_Click(object sender, EventArgs e)
        {
            TakeItem takeItemMain = takeItems.Where(item => item.Name == "Credits_ID").First();
            TakeItem takeItemCopyright = takeItems.Where(item => item.Name == "Credits_Copyright_ID").First();

            // Make sure all of the scene options are offline
            if (!takeItemMain.IsOnline && !takeItemCopyright.IsOnline)
                StartCredits(takeItemMain, takeItemCopyright, false);
            else
            {
                //Skip to the next credit
                if (_creditsTmr != null && _creditsTmr.Enabled)
                {
                    _creditsTmr.Stop();
                    _creditsTmr.Enabled = false;

                    void Credits_Timer_Tick(object tickSender, EventArgs tickE)
                    {
                        ShowCreditData(takeItemMain, takeItemCopyright);
                    }

                    // Reset & Start the timer
                    _creditsTmr = new Timer
                    {
                        Interval = (int)TimeSpan.FromSeconds((double)numCreditsSeconds.Value).TotalMilliseconds
                    };
                    _creditsTmr.Tick += new EventHandler(Credits_Timer_Tick);
                    _creditsTmr.Enabled = true;
                    _creditsTmr.Start();
                }
                ShowCreditData(takeItemMain, takeItemCopyright);
            }
        }

        private void btnTakeResetCredits_Click(object sender, EventArgs e)
        {
            EndOfCreditsData();
        }

        #endregion

        #region Config Scenes Tab
        private void btnConfigSaveXpressionData_Click(object sender, EventArgs e)
        {
            SaveXpressionData();
        }

        #endregion

        #region Config Score Bridge Tab
        private void btnConfigSaveScoreBridge_Click(object sender, EventArgs e)
        {
            SaveScoreBridgeData();
        }

        #endregion

        #region Config Event Tab
        private void btnConfigSaveEvent_Click(object sender, EventArgs e)
        {
            SaveEventData();
        }

        private void propGridScenes_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string categoryName = e.ChangedItem.Parent.Label.Trim();
            string name = e.ChangedItem.Label.Trim();
            object value = e.ChangedItem.Value;
            string text = value.ToString().Trim();
            switch (categoryName + "-" + name)
            {
                case "Scorebug - Info-Bug Info Data Options":
                    if (!string.IsNullOrEmpty(text))
                    {
                        updateInfoboxDropdowns();
                    }
                    break;
                default:
                    break;
            }
        }

        private void propGridEvent_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string categoryName = e.ChangedItem.Parent.Label.Trim();
            string name = e.ChangedItem.Label.Trim();
            object value = e.ChangedItem.Value;
            string text = value.ToString().Trim();
            switch (categoryName + "-" + name)
            {
                case "Home Team-School":
                    if (!string.IsNullOrEmpty(text))
                    {
                        LoadTeamsData();
                    }
                    break;
                case "Away Team-School":
                    if (!string.IsNullOrEmpty(text))
                    {
                        LoadTeamsData();
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Config Custom Variables Tab
        private void btnConfigSaveCustoms_Click(object sender, EventArgs e)
        {
            SaveCustomsData();
        }

        private void dataGridCustoms_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridCustoms.RemoveEmptyRows();
        }

        private void dataGridCustoms_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridCustoms.RemoveEmptyRows();
        }

        #endregion

        #region Config Sync Tab
        private void tabPageConfigSync_Enter(object sender, EventArgs e)
        {
            progBarSync.Value = 0;
        }

        private void btnSyncXpression_Click(object sender, EventArgs e)
        {
            SyncXpression();
        }

        #endregion

        #region ToolStrip

        private void configDirectoryProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show("Are you sure to select a new event data directory? Any unsaved changes will be lost.",
                                     "Confirm Reload!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                if (SelectDirectory())
                    LoadFromDataFiles();
            }
        }

        private void reloadProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show("Are you sure to reload all files? Any unsaved changes will be lost.",
                                     "Confirm Reload!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                LoadCustomsData();
                LoadEventData();
                LoadXpressionData();
                LoadScoreBridgenData();
            }
        }

        private void exitProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show(string.Format("Are you sure to exit {0}? Any unsaved changes will be lost.", this.ProductName),
                                     "Confirm Exit!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void loadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show("Are you sure to reload Widgets and TakeItems from Xpression? Unsaved changes may be lost.",
                                     "Confirm Load!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                LoadFromXpression();
            }
        }

        private void takeAllOfflineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show("Are you sure to turn all Take Items offline?",
                                     "Confirm Load!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                foreach (TakeItem takeItem in takeItems)
                    takeItem.SetOffline();
            }
        }

        private void liveSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (liveSyncToolStripMenuItem.Text == "ScoreBridge Sync ON")
            {
                if (scoreBridge.Stop())
                {
                    Invoke(new Action(() =>
                    {
                        liveSyncToolStripMenuItem.Text = "ScoreBridge Sync OFF";
                        liveSyncToolStripMenuItem.ForeColor = System.Drawing.Color.DarkRed;
                    }));
                }
            }
            else
            {
                if (scoreBridge.Start())
                {
                    Invoke(new Action(() =>
                    {
                        liveSyncToolStripMenuItem.Text = "ScoreBridge Sync ON";
                        liveSyncToolStripMenuItem.ForeColor = System.Drawing.Color.DarkGreen;
                    }));
                }
            }
        }

        #endregion
    }
}

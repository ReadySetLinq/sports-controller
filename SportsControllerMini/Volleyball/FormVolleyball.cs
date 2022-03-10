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
using SportsController.Volleyball;

namespace SportsControllerMini.Volleyball
{
    public partial class FormVolleyball : Form
    {
        #region Variables

        Xpression _Xpression;
        readonly string xpressionDataFilePath = Environment.CurrentDirectory + "\\config\\vb\\xpressionData.json";
        readonly string eventDataFilePath = Environment.CurrentDirectory + "\\config\\vb\\eventData.json";
        readonly string customsDataFilePath = Environment.CurrentDirectory + "\\config\\vb\\customsData.json";
        string eventDataDirectoryPath = Environment.CurrentDirectory + "\\data\\vb";
        Scoreboard scoreboardData;
        List<TakeItem> takeItems;
        Timer _creditsTmr = new Timer();
        Timer _infoTmr = new Timer();
        bool _autoPoint = true;
        int _credits_namesPerPage = 0;
        int _credits_rowCount = 0;
        int _credits_rowIndex = 0;
        int _home_roster_id = 0;
        int _home_coach_id = 0;
        int _away_roster_id = 1;
        int _away_coach_id = 1;

        #endregion

        public FormVolleyball()
        {
            InitializeComponent();
        }

        ~FormVolleyball()
        {
            if (_creditsTmr != null)
                _creditsTmr.Dispose();
            if (_infoTmr != null)
                _infoTmr.Dispose();
        }

        private void FormVolleyball_Load(object sender, EventArgs e)
        {
            SelectDirectory();

            // Create the defaults.
            _Xpression = new Xpression();
            XpressionData xpressionData = new XpressionData();
            EventData eventData = new EventData();
            scoreboardData = new Scoreboard();
            takeItems = new List<TakeItem>();

            // Assign SelectedObject.
            propGridXpression.SelectedObject = xpressionData;
            propGridXpression.CollapseAllGridItems();
            propGridEvent.SelectedObject = eventData;
            propGridEvent.CollapseAllGridItems();

            LoadCustomsData();
            LoadEventData();
            LoadXpressionData();
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
            foreach(int takeID in takeIDs)
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

                        string _creditsFile = "Credits.xlsx";

                        if (fileName.ToLower().Equals(_creditsFile.ToLower()))
                        {
                            LoadCreditsData();
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

        private DataGridViewRow GetDataGridViewRow (DataGridView dgv, string columnName, string cellValue, bool DisablewUserCanddRows = false)
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

        private DataGridViewCell GetDataGridViewCell (DataGridView dgv, DataGridViewRow row, string columnName)
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
            } catch { _success = false;  }

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
            if (takeButton != btnTakeCustomL3 && btnTakeCustomL3.Text == "Online")
                ToggleTakeButton(btnTakeCustomL3);

            if (takeButton != btnTakeL3Setup && btnTakeL3Setup.Text == "Online")
                ToggleTakeButton(btnTakeL3Setup);
        }

        public void CheckInfoBugStatus()
        {
            TakeItem infoItem = takeItems.Where(item => item.Name == "Scorebug_Info_ID").First();
            if (infoItem.IsOnline)
            {
                updateInfoLastEdited(0);
                TurnOnTakeItem(infoItem);
            }

            TakeItem redItem = takeItems.Where(item => item.Name == "Scorebug_RedCard_ID").First();
            if (redItem.IsOnline)
            {
                updateInfoLastEdited(1);
                TurnOnTakeItem(redItem);
            }

            TakeItem yellowItem = takeItems.Where(item => item.Name == "Scorebug_YellowCard_ID").First();
            if (yellowItem.IsOnline)
            {
                updateInfoLastEdited(1);
                TurnOnTakeItem(yellowItem);
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
            } catch { }
        }

        // Sync Xpression to Data project
        public void LoadFromXpression()
        {
            // Set current form to disabled
            this.Visible = false;
            this.Enabled = false;
            FormLoading frmLoading = new FormLoading();
            frmLoading.Setup(0, 18); // The starting value and max value for the loading bar
            frmLoading.Visible = true;
            frmLoading.Enabled = true;

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
            int homeSets = _Xpression.GetCounterWidgetValue(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Sets", propGridXpression.SelectedObject)));
            if (homeSets > -1)
            {
                scoreboardData.HomeSets = homeSets;
                lblHomeSets.Text = homeSets.ToString();
            }
            frmLoading.Increase();
            int awaySets = _Xpression.GetCounterWidgetValue(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Sets", propGridXpression.SelectedObject)));
            if (awaySets > -1)
            {
                scoreboardData.AwaySets = awaySets;
                lblAwaySets.Text = awaySets.ToString();
            }
            frmLoading.Increase();
            int serving = _Xpression.GetCounterWidgetValue(ConvertCustoms(Globals.GetObjectValue("Widget_Serving", propGridXpression.SelectedObject)));
            if (awaySets > -1)
            {
                scoreboardData.SetServing((Teams)serving);
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
            LoadTakeID("Bumper_Score_ID", _defaultXPN.Bumper_Score_ID, btnTakeBumperDefault);
            frmLoading.Increase();
            LoadTakeID("Bumper_Locator_ID", _defaultXPN.Bumper_Locator_ID, btnTakeBumperLocator);
            frmLoading.Increase();
            LoadTakeID("Bumper_Sets_ID", _defaultXPN.Bumper_Sets_ID, btnTakeBumperSets);
            frmLoading.Increase();

            // Load Lower Third Take Items
            LoadTakeID("L3_EventExtras_ID", _defaultXPN.L3_EventExtras_ID, btnTakeL3Setup);
            frmLoading.Increase();
            LoadTakeID("L3_Custom_ID", _defaultXPN.L3_Custom_ID, btnTakeCustomL3);
            frmLoading.Increase();

            // Load Scorebug Take Items
            LoadTakeID("Scorebug_ID", _defaultXPN.Scorebug_ID, btnTakeScorebug);
            frmLoading.Increase();
            LoadTakeID("Scorebug_Info_ID", _defaultXPN.Scorebug_Info_ID, btnTakeInfobox);
            frmLoading.Increase();
            LoadTakeID("Scorebug_RedCard_ID", _defaultXPN.Scorebug_RedCard_ID, btnTakeInfobox);
            frmLoading.Increase();
            LoadTakeID("Scorebug_YellowCard_ID", _defaultXPN.Scorebug_YellowCard_ID, btnTakeInfobox);
            frmLoading.Increase();
            CheckInfoBugStatus();

            // Load Credits Take Items
            LoadTakeID("Credits_ID", _defaultXPN.Credits_ID, btnTakeCredits);
            frmLoading.Increase();
            LoadTakeID("Credits_Copyright_ID", _defaultXPN.Credits_Copyright_ID, btnTakeCredits);
            frmLoading.Increase();

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
            int widgetCount = 5;
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
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Sets", propGridXpression.SelectedObject)), scoreboardData.HomeSets);
            progBarSync.Value++;
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Sets", propGridXpression.SelectedObject)), scoreboardData.AwaySets);
            progBarSync.Value++;
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Serving", propGridXpression.SelectedObject)), (int)scoreboardData.Serving);
            progBarSync.Value++;

            foreach(TakeItem takeItem in takeItems)
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
            } catch { };
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

        #region Config Scenes Tab
        private void btnConfigSaveXpressionData_Click(object sender, EventArgs e)
        {
            SaveXpressionData();
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

        #region Scoreboard Tab

        private void updateInfoLastEdited(int value = 0)
        {
            scoreboardData.InfoLastEdited = value; // 0 = select box, 1 = text box
            if (value == 0)
            {
                lblCmbInfobox.BackColor = System.Drawing.Color.DarkGreen;
                lblTxtInfobox.BackColor = System.Drawing.Color.DarkRed;
            }
            else
            {
                lblCmbInfobox.BackColor = System.Drawing.Color.DarkRed;
                lblTxtInfobox.BackColor = System.Drawing.Color.DarkGreen;
            }
        }

        private void populateScoreboard()
        {
            // Fill in all scoreboard labels with proper data from scoreboard variable
            lblHomePoints.Text = scoreboardData.HomePoints.ToString();
            lblHomeSets.Text = scoreboardData.HomeSets.ToString();
            lblHomeReds.Text = scoreboardData.HomeRedCards.ToString();
            lblHomeYellows.Text = scoreboardData.HomeYellowCards.ToString();

            lblAwayPoints.Text = scoreboardData.AwayPoints.ToString();
            lblAwaySets.Text = scoreboardData.AwaySets.ToString();
            lblAwayReds.Text = scoreboardData.AwayRedCards.ToString();
            lblAwayYellows.Text = scoreboardData.AwayYellowCards.ToString();

            updateInfoboxDropdowns();
        }

        private void updateInfoboxDropdowns()
        {
            cmbInfobox.Items.Clear();
            cmbInfobox.Items.AddRange(ConvertCustoms(Globals.GetObjectValue("Scorebug_Info_Data", propGridXpression.SelectedObject)).Split(','));
            cmbInfobox.SelectedIndex = 0;
        }

        private void UpdateSetResults()
        {
            int setNumber = scoreboardData.GetSetNumber();
            (int, int) setResults = scoreboardData.UpdateSetResults(setNumber);
            int index;
            switch (setNumber)
            {
                case 1:
                    numHomeSet1.Value = setResults.Item1;
                    numAwaySet1.Value = setResults.Item2;
                    index = cmbInfobox.Items.IndexOf("1st Set");
                    if (index >= 0)
                        cmbInfobox.SelectedIndex = index;
                    break;
                case 2:
                    numHomeSet2.Value = setResults.Item1;
                    numAwaySet2.Value = setResults.Item2;
                    index = cmbInfobox.Items.IndexOf("2nd Set");
                    if (index >= 0)
                        cmbInfobox.SelectedIndex = index;
                    break;
                case 3:
                    numHomeSet3.Value = setResults.Item1;
                    numAwaySet3.Value = setResults.Item2;
                    index = cmbInfobox.Items.IndexOf("3rd Set");
                    if (index >= 0)
                        cmbInfobox.SelectedIndex = index;
                    break;
                case 4:
                    numHomeSet4.Value = setResults.Item1;
                    numAwaySet4.Value = setResults.Item2;
                    index = cmbInfobox.Items.IndexOf("4th Set");
                    if (index >= 0)
                        cmbInfobox.SelectedIndex = index;
                    break;
                case 5:
                    numHomeSet5.Value = setResults.Item1;
                    numAwaySet5.Value = setResults.Item2;
                    index = cmbInfobox.Items.IndexOf("5th Set");
                    if (index >= 0)
                        cmbInfobox.SelectedIndex = index;
                    break;
            }
            updateInfoLastEdited(0);
        }

        private void PlayTeamPoint(Teams team)
        {
            string infoText = string.Empty;

            void Info_Timer_Tick(object tickSender, EventArgs tickE)
            {
                Invoke(new Action(() =>
                {
                    if (!infoText.Equals(string.Empty) && infoText.Equals(txtInfobox.Text))
                    {
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
                        txtInfobox.Text = string.Format("{0} Point", _homeName);
                        infoText = txtInfobox.Text;
                        break;
                    case Teams.Away:
                        string _awayName = ConvertCustoms(Globals.GetObjectValue("AwayName", propGridEvent.SelectedObject));
                        txtInfobox.Text = string.Format("{0} Point", _awayName);
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

            // Update the serving widget
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Serving", propGridXpression.SelectedObject)), (int)scoreboardData.Serving);
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

        private bool TakeCustomL3(TakeItem takeItem)
        {
            try
            {
                // Get all of the home player stat settings
                string  _custom = ConvertCustoms(txtTakeCustomL3.Text.Trim());
                string _message = "";
                string[] _chunks = _custom.Wrap(90);
                if (_chunks.Length == 1)
                {
                    _message = string.Format("{0}", _chunks[0]);
                }
                else if (_chunks.Length == 2)
                {
                    _message = string.Format("{0}\n{1}", _chunks[0], _chunks[1]);
                }
                else
                {
                    _message = string.Format("{0}\n{1}\n{2}", _chunks[0], _chunks[1], Truncate(string.Join(" ", _chunks.Skip(2)), 90));
                }

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
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Serving", propGridXpression.SelectedObject)), 0);
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
                scoreboardData.SetServing();
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Serving", propGridXpression.SelectedObject)), 0);
            }
        }

        private void lblHomePoints_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lblHomePoints.Text, out int val))
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Score", propGridXpression.SelectedObject)), val);
        }

        private void lblHomeSets_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lblHomeSets.Text, out int val))
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Sets", propGridXpression.SelectedObject)), val);
        }

        private void btnHomePointsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.SetPoint(Teams.Home, false);
            lblHomePoints.Text = val.ToString();
            scoreboardData.SetServing();
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Serving", propGridXpression.SelectedObject)), 0);
        }

        private void btnHomePointsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.SetPoint(Teams.Home);
            lblHomePoints.Text = val.ToString();
            PlayTeamPoint(Teams.Home);
        }

        private void lblAwayPoints_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lblAwayPoints.Text, out int val))
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Score", propGridXpression.SelectedObject)), val);
        }

        private void lblAwaySets_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lblAwaySets.Text, out int val))
                _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Sets", propGridXpression.SelectedObject)), val);
        }

        private void btnAwayPointsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.SetPoint(Teams.Away, false);
            lblAwayPoints.Text = val.ToString();
            scoreboardData.SetServing();
            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Serving", propGridXpression.SelectedObject)), 0);
        }

        private void btnAwayPointsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.SetPoint(Teams.Away);
            lblAwayPoints.Text = val.ToString();
            PlayTeamPoint(Teams.Away);
        }

        private void btnHomeSetsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.SetWin(Teams.Home, false, false);
            lblHomeSets.Text = val.ToString();

            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Sets", propGridXpression.SelectedObject)), val);
        }

        private void btnHomeSetsInc_Click(object sender, EventArgs e)
        {
            UpdateSetResults();

            int val = scoreboardData.SetWin(Teams.Home, true, false);
            lblHomeSets.Text = val.ToString();

            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Home_Sets", propGridXpression.SelectedObject)), val);
        }

        private void btnAwaySetsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.SetWin(Teams.Away, false, false);
            lblAwaySets.Text = val.ToString();

            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Sets", propGridXpression.SelectedObject)), val);
        }

        private void btnAwaySetsInc_Click(object sender, EventArgs e)
        {
            UpdateSetResults();

            int val = scoreboardData.SetWin(Teams.Away, true, false);
            lblAwaySets.Text = val.ToString();

            _Xpression.EditCounterWidget(ConvertCustoms(Globals.GetObjectValue("Widget_Away_Sets", propGridXpression.SelectedObject)), val);
        }

        private void btnHomeRedsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.RedCard(Teams.Home, false);
            lblHomeReds.Text = val.ToString();
        }

        private void btnHomeRedsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.RedCard(Teams.Home);
            lblHomeReds.Text = val.ToString();

            string _homeName = ConvertCustoms(Globals.GetObjectValue("HomeName", propGridEvent.SelectedObject));
            txtInfobox.Text = string.Format("{0} Red Card", _homeName);
            updateInfoLastEdited(1);
        }

        private void btnAwayRedsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.RedCard(Teams.Away, false);
            lblAwayReds.Text = val.ToString();
        }

        private void btnAwayRedsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.RedCard(Teams.Away);
            lblAwayReds.Text = val.ToString();

            string _AwayName = ConvertCustoms(Globals.GetObjectValue("AwayName", propGridEvent.SelectedObject));
            txtInfobox.Text = string.Format("{0} Red Card", _AwayName);
            updateInfoLastEdited(1);
        }

        private void btnHomeYellowsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.YellowCard(Teams.Home, false);
            lblHomeYellows.Text = val.ToString();
        }

        private void btnHomeYellowsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.YellowCard(Teams.Home);
            lblHomeYellows.Text = val.ToString();

            string _homeName = ConvertCustoms(Globals.GetObjectValue("HomeName", propGridEvent.SelectedObject));
            txtInfobox.Text = string.Format("{0} Yellow Card", _homeName);
            updateInfoLastEdited(1);
        }

        private void btnAwayYellowsDec_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.YellowCard(Teams.Away, false);
            lblAwayYellows.Text = val.ToString();
        }

        private void btnAwayYellowsInc_Click(object sender, EventArgs e)
        {
            int val = scoreboardData.YellowCard(Teams.Away);
            lblAwayYellows.Text = val.ToString();

            string _AwayName = ConvertCustoms(Globals.GetObjectValue("AwayName", propGridEvent.SelectedObject));
            txtInfobox.Text = string.Format("{0} Yellow Card", _AwayName);
            updateInfoLastEdited(1);
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

        #endregion

        #region Sets Tab
        private void btnSetResultsReset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to reset both all of the set results for each team back to 0?",
                                     "Confirm Reset!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.No)
                return;

            scoreboardData.ResetSetResults();

            numHomeSet1.Value = -1;
            numAwaySet1.Value = -1;

            numHomeSet2.Value = -1;
            numAwaySet2.Value = -1;

            numHomeSet3.Value = -1;
            numAwaySet3.Value = -1;

            numHomeSet4.Value = -1;
            numAwaySet4.Value = -1;

            numHomeSet5.Value = -1;
            numAwaySet5.Value = -1;
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
                TurnOnTakeItem(takeItem);
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

            // Check if we are actually showing a red/yellow card
            TakeItem redItem = takeItems.Where(item => item.Name == "Scorebug_RedCard_ID").First();
            TakeItem yellowItem = takeItems.Where(item => item.Name == "Scorebug_YellowCard_ID").First();

            // Make sure none of the "info" take items are online
            if (!takeItem.IsOnline && !redItem.IsOnline && !yellowItem.IsOnline)
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
                        {
                            // Show the combo box value
                            takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Scorebug_Info_Name", propGridXpression.SelectedObject)), "Text", ConvertCustoms(cmbInfobox.Text.Trim()));
                            TurnOnTakeItem(takeItem);
                        }
                        else
                        {
                            if (boxText.Contains("Red", true) && boxText.Contains("Card", true))
                            {
                                // Update Properties before trying to take online
                                string txt = boxText.Replace("Red Card", "").Trim();
                                redItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Scorebug_RedCard_Text", propGridXpression.SelectedObject)), "Text", txt);
                                TurnOnTakeItem(redItem);
                            }
                            else if (boxText.Contains("Yellow", true) && boxText.Contains("Card", true))
                            {
                                // Update Properties before trying to take online
                                string txt = boxText.Replace("Yellow Card", "").Trim();
                                yellowItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Scorebug_YellowCard_Text", propGridXpression.SelectedObject)), "Text", txt);
                                TurnOnTakeItem(yellowItem);
                            }
                            else
                            {
                                // Update Properties before trying to take online
                                takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Scorebug_Info_Name", propGridXpression.SelectedObject)), "Text", boxText);
                                TurnOnTakeItem(takeItem);
                            }
                        }
                    }
                }
            }
            else
            {
                // Turn off all of the "info" layer take items
                TurnOffTakeItemLayer(takeItem);
                // Revert to w/e our select box is for next time we show the bug
                updateInfoLastEdited(0);
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

        private void btnTakeBumperDefault_Click(object sender, EventArgs e)
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

        private void btnTakeBumperSets_Click(object sender, EventArgs e)
        {
            TakeItem takeItem = takeItems.Where(item => item.Name == "Bumper_Sets_ID").First();

            // Make  sure the scene is offline
            if (!takeItem.IsOnline)
            {
                // Update properties and take scene online
                try
                {
                    // Edit TakeItem poroperties with new values
                    takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Heading", propGridXpression.SelectedObject)), "Text", ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Heading_Value", propGridXpression.SelectedObject)));

                    if (numHomeSet1.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set1", propGridXpression.SelectedObject)), "Text", numHomeSet1.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set1", propGridXpression.SelectedObject)), "Text","");
                    if (numAwaySet1.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set1", propGridXpression.SelectedObject)), "Text", numAwaySet1.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set1", propGridXpression.SelectedObject)), "Text", "");
                    if (numHomeSet2.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set2", propGridXpression.SelectedObject)), "Text", numHomeSet2.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set2", propGridXpression.SelectedObject)), "Text", "");
                    if (numAwaySet2.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set2", propGridXpression.SelectedObject)), "Text", numAwaySet2.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set2", propGridXpression.SelectedObject)), "Text", "");
                    if (numHomeSet3.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set3", propGridXpression.SelectedObject)), "Text", numHomeSet3.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set3", propGridXpression.SelectedObject)), "Text", "");
                    if (numAwaySet3.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set3", propGridXpression.SelectedObject)), "Text", numAwaySet3.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set3", propGridXpression.SelectedObject)), "Text", "");
                    if (numHomeSet4.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set4", propGridXpression.SelectedObject)), "Text", numHomeSet4.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set4", propGridXpression.SelectedObject)), "Text", "");
                    if (numAwaySet4.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set4", propGridXpression.SelectedObject)), "Text", numAwaySet4.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set4", propGridXpression.SelectedObject)), "Text", "");
                    if (numHomeSet5.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set5", propGridXpression.SelectedObject)), "Text", numHomeSet5.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Home_Set5", propGridXpression.SelectedObject)), "Text", "");
                    if (numAwaySet5.Value >= 0)
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set5", propGridXpression.SelectedObject)), "Text", numAwaySet5.Value.ToString().PadLeft(2, '0'));
                    else
                        takeItem.EditProperty(ConvertCustoms(Globals.GetObjectValue("Bumper_Sets_Away_Set5", propGridXpression.SelectedObject)), "Text", "");

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
                _title = txtL3SetupLeftName.Text.Trim();
                _subtitle = txtL3SetupLeftTitle.Text.Trim();
            } catch { return (_title, _subtitle); }

            return (_title, _subtitle);
        }

        private (string, string) GetL3SetupMiddleData()
        {
            string _title = "";
            string _subtitle = "";

            try 
            {
                _title = txtL3SetupMiddleName.Text.Trim();
                _subtitle = txtL3SetupMiddleTitle.Text.Trim();
            } catch { return (_title, _subtitle); }

            return (_title, _subtitle);
        }

        private (string, string) GetL3SetupRightData()
        {
            string _title = "";
            string _subtitle = "";

            try 
            {
                _title = txtL3SetupRightName.Text.Trim();
                _subtitle = txtL3SetupRightTitle.Text.Trim();
            } catch { return (_title, _subtitle); }

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
            chkL3SetupActiveLeft.Checked = false;
            txtL3SetupLeftName.Text = string.Empty;
            txtL3SetupLeftTitle.Text = string.Empty;
        }

        private void btnL3SetupClearMiddle_Click(object sender, EventArgs e)
        {
            chkL3SetupActiveMiddle.Checked = false;
            txtL3SetupMiddleName.Text = string.Empty;
            txtL3SetupMiddleTitle.Text = string.Empty;
        }

        private void btnL3SetupClearRight_Click(object sender, EventArgs e)
        {
            chkL3SetupActiveRight.Checked = false;
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
                _creditsTmr = new Timer
                {
                    Interval = (int)TimeSpan.FromSeconds((double)numCreditsSeconds.Value).TotalMilliseconds
                };
                _creditsTmr.Tick += new EventHandler(Credits_Timer_Tick);
                _creditsTmr.Enabled = true;
                _creditsTmr.Start();

                void Credits_Timer_Tick(object sender, EventArgs e)
                {
                    ShowCreditData(takeItemMain, takeItemCopyright);
                }
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

                    // Reset & Start the timer
                    _creditsTmr = new Timer
                    {
                        Interval = (int)TimeSpan.FromSeconds((double)numCreditsSeconds.Value).TotalMilliseconds
                    };
                    _creditsTmr.Tick += new EventHandler(Credits_Timer_Tick);
                    _creditsTmr.Enabled = true;
                    _creditsTmr.Start();

                    void Credits_Timer_Tick(object tickSender, EventArgs tickE)
                    {
                        ShowCreditData(takeItemMain, takeItemCopyright);
                    }
                }
                ShowCreditData(takeItemMain, takeItemCopyright);
            }
        }

        private void btnTakeResetCredits_Click(object sender, EventArgs e)
        {
            EndOfCreditsData();
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

        #endregion

    }
}

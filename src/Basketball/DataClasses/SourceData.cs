using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SportsController.Basketball
{
    [DefaultProperty("StatsDirectory")]
    public class SourceData
    {

        #region Variables
        // File Directories
        string creditsFile = Environment.CurrentDirectory + "\\Data\\bb\\Credits.csv";
        string l3SetupFile = Environment.CurrentDirectory + "\\Data\\bb\\LowerThirds.csv";
        string statsFile = Environment.CurrentDirectory + "\\Data\\bb\\Stats\\WBB.csv";
        string standingsFile = Environment.CurrentDirectory + "\\Data\\bb\\Standings\\WBB.csv";
        string rostersPath = Environment.CurrentDirectory + "\\Data\\bb\\Rosters\\";
        string coachesPath = Environment.CurrentDirectory + "\\Data\\bb\\Coaches\\";

        #endregion

        [
            Category("File Locations"),
            DisplayName("Credits"),
            DefaultValue(""),
            Description("The .csv file containing the credits data."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string CreditsFile
        {
            get { return this.creditsFile; }
            set { this.creditsFile = value; }
        }

        [
            Category("File Locations"),
            DisplayName("L3 Setup"),
            DefaultValue(""),
            Description("The .csv file containing all of the event extras names and titles for lower thirds."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string L3SetupFile
        {
            get { return this.l3SetupFile; }
            set { this.l3SetupFile = value; }
        }

        [
            Category("File Locations"),
            DisplayName("Stats"),
            DefaultValue(""),
            Description("The .csv file containing all teams stats."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string StatsFile
        {
            get { return this.statsFile; }
            set { this.statsFile = value; }
        }

        [
            Category("File Locations"),
            DisplayName("Standings"),
            DefaultValue(""),
            Description("The .csv file for this events standings."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string StandingsFile
        {
            get { return this.standingsFile; }
            set { this.standingsFile = value; }
        }

        [
            Category("File Directories"),
            DisplayName("Rosters"),
            DefaultValue(""),
            Description("The directory containing .csv files for each teams rosters."),
            Editor(typeof(FolderNameEditor), typeof(UITypeEditor))
        ]
        public string RostersDirectory
        {
            get { return this.rostersPath; }
            set { this.rostersPath = value; }
        }

        [
            Category("File Directories"),
            DisplayName("Coaches"),
            DefaultValue(""),
            Description("The directory containing .csv files for each teams coaches."),
            Editor(typeof(FolderNameEditor), typeof(UITypeEditor))
        ]
        public string CoachesDirectory
        {
            get { return this.coachesPath; }
            set { this.coachesPath = value; }
        }

    }
}

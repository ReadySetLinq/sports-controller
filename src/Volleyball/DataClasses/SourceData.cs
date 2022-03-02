using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SportsController.Volleyball
{
    [DefaultProperty("StatsDirectory")]
    public class SourceData
    {

        #region Variables
        // File Directories

        #endregion

        [
            Category("File Locations"),
            DisplayName("Credits"),
            DefaultValue(""),
            Description("The .csv file containing the credits data."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string CreditsFile { get; set; } = Environment.CurrentDirectory + "\\Data\\vb\\Credits.csv";

        [
            Category("File Locations"),
            DisplayName("L3 Setup"),
            DefaultValue(""),
            Description("The .csv file containing all of the event extras names and titles for lower thirds."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string L3SetupFile { get; set; } = Environment.CurrentDirectory + "\\Data\\vb\\LowerThirds.csv";

        [
            Category("File Locations"),
            DisplayName("Stats"),
            DefaultValue(""),
            Description("The .csv file containing all teams stats."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string StatsFile { get; set; } = Environment.CurrentDirectory + "\\Data\\vb\\Stats\\WBB.csv";

        [
            Category("File Locations"),
            DisplayName("Standings"),
            DefaultValue(""),
            Description("The .csv file for this events standings."),
            Editor(typeof(FileNameEditor), typeof(UITypeEditor))
        ]
        public string StandingsFile { get; set; } = Environment.CurrentDirectory + "\\Data\\vb\\Standings\\WBB.csv";

        [
            Category("File Directories"),
            DisplayName("Rosters"),
            DefaultValue(""),
            Description("The directory containing .csv files for each teams rosters."),
            Editor(typeof(FolderNameEditor), typeof(UITypeEditor))
        ]
        public string RostersDirectory { get; set; } = Environment.CurrentDirectory + "\\Data\\vb\\Rosters\\";

        [
            Category("File Directories"),
            DisplayName("Coaches"),
            DefaultValue(""),
            Description("The directory containing .csv files for each teams coaches."),
            Editor(typeof(FolderNameEditor), typeof(UITypeEditor))
        ]
        public string CoachesDirectory { get; set; } = Environment.CurrentDirectory + "\\Data\\vb\\Coaches\\";

    }
}

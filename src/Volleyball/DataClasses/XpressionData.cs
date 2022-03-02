using System.ComponentModel;
using System.Drawing.Design;

namespace SportsController.Volleyball
{
    public class XpressionData
    {

        #region Widgets

        // Widget Variables
        private string _widget_Serving = "Serving";
        private string _widget_Home_Score = "Home Score";
        private string _widget_Home_Sets = "Home Sets";
        private string _widget_Away_Score = "Away Score";
        private string _widget_Away_Sets = "Away Sets";

        [
            Category("Widgets"),
            DisplayName("Serving"),
            DefaultValue("Serving"),
            Description("The serving team widget."),
        ]
        public string Widget_Serving
        {
            get { return _widget_Serving; }
            set { _widget_Serving = value; }
        }

        [
            Category("Widgets"),
            DisplayName("Home Team Points"),
            DefaultValue("Home Score"),
            Description("The Home Team points widget."),
        ]
        public string Widget_Home_Score
        {
            get { return _widget_Home_Score; }
            set { _widget_Home_Score = value; }
        }

        [
            Category("Widgets"),
            DisplayName("Home Team Sets"),
            DefaultValue("Home Sets"),
            Description("The Home Team sets widget."),
        ]
        public string Widget_Home_Sets
        {
            get { return _widget_Home_Sets; }
            set { _widget_Home_Sets = value; }
        }

        [
            Category("Widgets"),
            DisplayName("Away Team Points"),
            DefaultValue("Away Score"),
            Description("The Away Team points widget."),
        ]
        public string Widget_Away_Score
        {
            get { return _widget_Away_Score; }
            set { _widget_Away_Score = value; }
        }

        [
            Category("Widgets"),
            DisplayName("Away Team Sets"),
            DefaultValue("Away Sets"),
            Description("The Away Team sets widget."),
        ]
        public string Widget_Away_Sets
        {
            get { return _widget_Away_Sets; }
            set { _widget_Away_Sets = value; }
        }

        #endregion

        #region Misc

        // Misc Variables
        private int _misc_Network_Bug = 0001;
        private int _misc_Replay_Bug = 0002;

        [
            Category("Misc"),
            DisplayName("Network Bug Take ID"),
            DefaultValue(0001),
            Description("The Network Bug takeItem ID."),
        ]
        public int Misc_Network_Bug
        {
            get { return _misc_Network_Bug; }
            set { _misc_Network_Bug = value; }
        }

        [
            Category("Misc"),
            DisplayName("Replay Bug Take ID"),
            DefaultValue(0002),
            Description("The Replay Bug takeItem ID."),
        ]
        public int Misc_Replay_Bug
        {
            get { return _misc_Replay_Bug; }
            set { _misc_Replay_Bug = value; }
        }


        #endregion

        #region Bumpers

        private int _bumper_Locator_ID = 0101;
        private string _bumper_Locator_Heading = "txtHeading";
        private string _bumper_Locator_Venue = "txtVenue";
        private string _bumper_Locator_Location = "txtLocation";

        [
            Category("Bumpers - Locator"),
            DisplayName("Take ID"),
            DefaultValue(0101),
            Description("The Locator bumper takeItem ID."),
        ]
        public int Bumper_Locator_ID
        {
            get { return _bumper_Locator_ID; }
            set { _bumper_Locator_ID = value; }
        }

        [
            Category("Bumpers - Locator"),
            DisplayName("Heading Element"),
            DefaultValue("txtHeading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_Locator_Heading
        {
            get { return _bumper_Locator_Heading; }
            set { _bumper_Locator_Heading = value; }
        }

        [
            Category("Bumpers - Locator"),
            DisplayName("Title Element"),
            DefaultValue("txtVenue"),
            Description("The bumpers venue text box name."),
        ]
        public string Bumper_Locator_Venue
        {
            get { return _bumper_Locator_Venue; }
            set { _bumper_Locator_Venue = value; }
        }

        [
            Category("Bumpers - Locator"),
            DisplayName("Subtitle Element"),
            DefaultValue("txtLocation"),
            Description("The bumpers location text box name."),
        ]
        public string Bumper_Locator_Location
        {
            get { return _bumper_Locator_Location; }
            set { _bumper_Locator_Location = value; }
        }

        private int _bumper_Score_ID = 0102;
        private string _bumper_Score_Heading = "txtHeading";
        private string _bumper_Score_Info = "txtInfo";
        private string _bumper_Score_Info_Value = "";
        private string _bumper_Score_HomeTeam = "txtHomeTeam";
        private string _bumper_Score_AwayTeam = "txtAwayTeam";

        [
            Category("Bumpers - Score"),
            DisplayName("Take ID"),
            DefaultValue(0102),
            Description("The Score bumper takeItem ID."),
        ]
        public int Bumper_Score_ID
        {
            get { return _bumper_Score_ID; }
            set { _bumper_Score_ID = value; }
        }

        [
            Category("Bumpers - Score"),
            DisplayName("Heading Element"),
            DefaultValue("txtHeading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_Score_Heading
        {
            get { return _bumper_Score_Heading; }
            set { _bumper_Score_Heading = value; }
        }

        [
            Category("Bumpers - Score"),
            DisplayName("Info Element"),
            DefaultValue("txtInfo"),
            Description("The bumpers information text box name."),
        ]
        public string Bumper_Score_Info
        {
            get { return _bumper_Score_Info; }
            set { _bumper_Score_Info = value; }
        }

        [
            Category("Bumpers - Score"),
            DisplayName("Info Value"),
            DefaultValue(""),
            Description("The bumpers information text box value."),
        ]
        public string Bumper_Score_Info_Value
        {
            get { return _bumper_Score_Info_Value; }
            set { _bumper_Score_Info_Value = value; }
        }

        [
            Category("Bumpers - Score"),
            DisplayName("Home Team Element"),
            DefaultValue("txtHomeTeam"),
            Description("The bumpers home team text box name."),
        ]
        public string Bumper_Score_HomeTeam
        {
            get { return _bumper_Score_HomeTeam; }
            set { _bumper_Score_HomeTeam = value; }
        }

        [
            Category("Bumpers - Score"),
            DisplayName("Away Team Element"),
            DefaultValue("txtAwayTeam"),
            Description("The bumpers away team text box name."),
        ]
        public string Bumper_Score_AwayTeam
        {
            get { return _bumper_Score_AwayTeam; }
            set { _bumper_Score_AwayTeam = value; }
        }

        private int _bumper_HeadToHead_ID = 0106;
        private string _bumper_HeadToHead_Heading = "txtHeading";
        private string _bumper_HeadToHead_Heading_Value = "{EventTitle}";
        private string _bumper_HeadToHead_Label_Stat1 = "txtStat1";
        private string _bumper_HeadToHead_Label_Stat1_Value = "Record";
        private string _bumper_HeadToHead_Label_Stat2 = "txtStat2";
        private string _bumper_HeadToHead_Label_Stat2_Value = "Points";
        private string _bumper_HeadToHead_Label_Stat3 = "txtStat3";
        private string _bumper_HeadToHead_Label_Stat3_Value = "Streak";
        private string _bumper_HeadToHead_Home_Stat1 = "txtHomeStat1";
        private string _bumper_HeadToHead_Home_Stat2 = "txtHomeStat2";
        private string _bumper_HeadToHead_Home_Stat3 = "txtHomeStat3";
        private string _bumper_HeadToHead_Away_Stat1 = "txtAwayStat1";
        private string _bumper_HeadToHead_Away_Stat2 = "txtAwayStat2";
        private string _bumper_HeadToHead_Away_Stat3 = "txtAwayStat3";

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Take ID"),
            DefaultValue(0106),
            Description("The Head To Head bumper takeItem ID."),
        ]
        public int Bumper_HeadToHead_ID
        {
            get { return _bumper_HeadToHead_ID; }
            set { _bumper_HeadToHead_ID = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Heading Element"),
            DefaultValue("txtHeading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_HeadToHead_Heading
        {
            get { return _bumper_HeadToHead_Heading; }
            set { _bumper_HeadToHead_Heading = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Heading Value"),
            DefaultValue("{EventTitle}"),
            Description("The bumpers heading value."),
        ]
        public string Bumper_HeadToHead_Heading_Value
        {
            get { return _bumper_HeadToHead_Heading_Value; }
            set { _bumper_HeadToHead_Heading_Value = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("First Stat Element"),
            DefaultValue("txtStat1"),
            Description("The bumpers first stat label box name."),
        ]
        public string Bumper_HeadToHead_Label_Stat1
        {
            get { return _bumper_HeadToHead_Label_Stat1; }
            set { _bumper_HeadToHead_Label_Stat1 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("First Stat Value"),
            DefaultValue("Record"),
            Description("The bumpers first stat value. (Must be a column from the Stats tab or it will show up blank)"),
        ]
        public string Bumper_HeadToHead_Label_Stat1_Value
        {
            get { return _bumper_HeadToHead_Label_Stat1_Value; }
            set { _bumper_HeadToHead_Label_Stat1_Value = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Second Stat Element"),
            DefaultValue("txtStat2"),
            Description("The bumpers second stat label box name."),
        ]
        public string Bumper_HeadToHead_Label_Stat2
        {
            get { return _bumper_HeadToHead_Label_Stat2; }
            set { _bumper_HeadToHead_Label_Stat2 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Second Stat Value"),
            DefaultValue("Points"),
            Description("The bumpers second stat value. (Must be a column from the Stats tab or it will show up blank)"),
        ]
        public string Bumper_HeadToHead_Label_Stat2_Value
        {
            get { return _bumper_HeadToHead_Label_Stat2_Value; }
            set { _bumper_HeadToHead_Label_Stat2_Value = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Third Stat Element"),
            DefaultValue("txtStat3"),
            Description("The bumpers third stat label box name."),
        ]
        public string Bumper_HeadToHead_Label_Stat3
        {
            get { return _bumper_HeadToHead_Label_Stat3; }
            set { _bumper_HeadToHead_Label_Stat3 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Third Stat Value"),
            DefaultValue("Streak"),
            Description("The bumpers third stat value. (Must be a column from the Stats tab or it will show up blank)"),
        ]
        public string Bumper_HeadToHead_Label_Stat3_Value
        {
            get { return _bumper_HeadToHead_Label_Stat3_Value; }
            set { _bumper_HeadToHead_Label_Stat3_Value = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Home First Stat Element"),
            DefaultValue("txtHomeStat1"),
            Description("The bumpers first stat box name for the home team."),
        ]
        public string Bumper_HeadToHead_Home_Stat1
        {
            get { return _bumper_HeadToHead_Home_Stat1; }
            set { _bumper_HeadToHead_Home_Stat1 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Away First Stat Element"),
            DefaultValue("txtAwayStat1"),
            Description("The bumpers first stat box name for the away team."),
        ]
        public string Bumper_HeadToHead_Away_Stat1
        {
            get { return _bumper_HeadToHead_Away_Stat1; }
            set { _bumper_HeadToHead_Away_Stat1 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Home Second Stat Element"),
            DefaultValue("txtHomeStat2"),
            Description("The bumpers second stat box name for the home team."),
        ]
        public string Bumper_HeadToHead_Home_Stat2
        {
            get { return _bumper_HeadToHead_Home_Stat2; }
            set { _bumper_HeadToHead_Home_Stat2 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Away Second Stat Element"),
            DefaultValue("txtAwayStat2"),
            Description("The bumpers second stat box name for the away team."),
        ]
        public string Bumper_HeadToHead_Away_Stat2
        {
            get { return _bumper_HeadToHead_Away_Stat2; }
            set { _bumper_HeadToHead_Away_Stat2 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Home Third Stat Element"),
            DefaultValue("txtHomeStat3"),
            Description("The bumpers third stat box name for the home team."),
        ]
        public string Bumper_HeadToHead_Home_Stat3
        {
            get { return _bumper_HeadToHead_Home_Stat3; }
            set { _bumper_HeadToHead_Home_Stat3 = value; }
        }

        [
            Category("Bumpers - Head To Head"),
            DisplayName("Away Third Stat Element"),
            DefaultValue("txtAwayStat3"),
            Description("The bumpers third stat box name for the away team."),
        ]
        public string Bumper_HeadToHead_Away_Stat3
        {
            get { return _bumper_HeadToHead_Away_Stat3; }
            set { _bumper_HeadToHead_Away_Stat3 = value; }
        }

        private int _bumper_Sets_ID = 0104;
        private string _bumper_Sets_Heading = "txtHeading";
        private string _bumper_Sets_Heading_Value = "{EventTitle}";
        private string _bumper_Sets_Home_Set1 = "txtSetHome1";
        private string _bumper_Sets_Away_Set1 = "txtSetAway1";
        private string _bumper_Sets_Home_Set2 = "txtSetHome2";
        private string _bumper_Sets_Away_Set2 = "txtSetAway2";
        private string _bumper_Sets_Home_Set3 = "txtSetHome3";
        private string _bumper_Sets_Away_Set3 = "txtSetAway3";
        private string _bumper_Sets_Home_Set4 = "txtSetHome4";
        private string _bumper_Sets_Away_Set4 = "txtSetAway4";
        private string _bumper_Sets_Home_Set5 = "txtSetHome5";
        private string _bumper_Sets_Away_Set5 = "txtSetAway5";

        [
            Category("Bumpers - Sets"),
            DisplayName("Take ID"),
            DefaultValue(0104),
            Description("The Sets bumper takeItem ID."),
        ]
        public int Bumper_Sets_ID
        {
            get { return _bumper_Sets_ID; }
            set { _bumper_Sets_ID = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Heading Element"),
            DefaultValue("txtHeading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_Sets_Heading
        {
            get { return _bumper_Sets_Heading; }
            set { _bumper_Sets_Heading = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Heading Value"),
            DefaultValue("{EventTitle}"),
            Description("The bumpers heading value."),
        ]
        public string Bumper_Sets_Heading_Value
        {
            get { return _bumper_Sets_Heading_Value; }
            set { _bumper_Sets_Heading_Value = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Home Team Set1 Element"),
            DefaultValue("txtSetHome1"),
            Description("The home teams first set source box name."),
        ]
        public string Bumper_Sets_Home_Set1
        {
            get { return _bumper_Sets_Home_Set1; }
            set { _bumper_Sets_Home_Set1 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Away Team Set1 Element"),
            DefaultValue("txtSetAway1"),
            Description("The away teams first set source box name."),
        ]
        public string Bumper_Sets_Away_Set1
        {
            get { return _bumper_Sets_Away_Set1; }
            set { _bumper_Sets_Away_Set1 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Home Team Set2 Element"),
            DefaultValue("txtSetHome2"),
            Description("The home teams second set source box name."),
        ]
        public string Bumper_Sets_Home_Set2
        {
            get { return _bumper_Sets_Home_Set2; }
            set { _bumper_Sets_Home_Set2 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Away Team Set2 Element"),
            DefaultValue("txtSetAway2"),
            Description("The away teams second set source box name."),
        ]
        public string Bumper_Sets_Away_Set2
        {
            get { return _bumper_Sets_Away_Set2; }
            set { _bumper_Sets_Away_Set2 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Home Team Set3 Element"),
            DefaultValue("txtSetHome3"),
            Description("The home teams third set source box name."),
        ]
        public string Bumper_Sets_Home_Set3
        {
            get { return _bumper_Sets_Home_Set3; }
            set { _bumper_Sets_Home_Set3 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Away Team Set3 Element"),
            DefaultValue("txtSetAway3"),
            Description("The away teams third set source box name."),
        ]
        public string Bumper_Sets_Away_Set3
        {
            get { return _bumper_Sets_Away_Set3; }
            set { _bumper_Sets_Away_Set3 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Home Team Set4 Element"),
            DefaultValue("txtSetHome4"),
            Description("The home teams fourth set source box name."),
        ]
        public string Bumper_Sets_Home_Set4
        {
            get { return _bumper_Sets_Home_Set4; }
            set { _bumper_Sets_Home_Set4 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Away Team Set4 Element"),
            DefaultValue("txtSetAway4"),
            Description("The away teams fourth set source box name."),
        ]
        public string Bumper_Sets_Away_Set4
        {
            get { return _bumper_Sets_Away_Set4; }
            set { _bumper_Sets_Away_Set4 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Home Team Set5 Element"),
            DefaultValue("txtSetHome5"),
            Description("The home teams fith set source box name."),
        ]
        public string Bumper_Sets_Home_Set5
        {
            get { return _bumper_Sets_Home_Set5; }
            set { _bumper_Sets_Home_Set5 = value; }
        }

        [
            Category("Bumpers - Sets"),
            DisplayName("Away Team Set5 Element"),
            DefaultValue("txtSetAway5"),
            Description("The away teams fith set source box name."),
        ]
        public string Bumper_Sets_Away_Set5
        {
            get { return _bumper_Sets_Away_Set5; }
            set { _bumper_Sets_Away_Set5 = value; }
        }

        private int _bumper_Standings_ID = 0105;
        private string _bumper_Standings_Heading = "txtHeading";
        private string _bumper_Standings_Heading_Value = "{EventTitle}";
        private string _bumper_Standings_Title = "txtTitle";
        private string _bumper_Standings_Title_Value = "West";
        private string _bumper_Standings_Background = "bg";
        private string _bumper_Standings_Background_Visible = "True";
        private string _bumper_Standings_GroupBase = "group";
        private string _bumper_Standings_SchoolBase = "txtTeamSchool";
        private string _bumper_Standings_RecordBase = "txtTeamRecord";
        private string _bumper_Standings_PointsBase = "txtTeamPoints";
        private int _bumper_Standings_MaxTeams = 11;

        [
            Category("Bumpers - Standings"),
            DisplayName("Take ID"),
            DefaultValue(0105),
            Description("The Standings bumper takeItem ID."),
        ]
        public int Bumper_Standings_ID
        {
            get { return _bumper_Standings_ID; }
            set { _bumper_Standings_ID = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Heading Element"),
            DefaultValue("txtHeading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_Standings_Heading
        {
            get { return _bumper_Standings_Heading; }
            set { _bumper_Standings_Heading = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Heading Value"),
            DefaultValue("{EventTitle}"),
            Description("The bumpers heading value."),
        ]
        public string Bumper_Standings_Heading_Value
        {
            get { return _bumper_Standings_Heading_Value; }
            set { _bumper_Standings_Heading_Value = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Title Element"),
            DefaultValue("txtTitle"),
            Description("The bumpers title text box name."),
        ]
        public string Bumper_Standings_Title
        {
            get { return _bumper_Standings_Title; }
            set { _bumper_Standings_Title = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Title Value"),
            DefaultValue("West"),
            Description("The bumpers title value."),
        ]
        public string Bumper_Standings_Title_Value
        {
            get { return _bumper_Standings_Title_Value; }
            set { _bumper_Standings_Title_Value = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Background"),
            DefaultValue("bg"),
            Description("The bumpers background element name."),
        ]
        public string Bumper_Standings_Background
        {
            get { return _bumper_Standings_Background; }
            set { _bumper_Standings_Background = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Background Visible"),
            DefaultValue("True"),
            Description("If the background image should be shown or not (will have a transparent background if set to False)"),
        ]
        public string Bumper_Standings_Background_Visible
        {
            get { return _bumper_Standings_Background_Visible; }
            set
            {
                // Only accept True/False as answers (and Convert Yes/No to True/No)
                string val = value.Trim().ToLower();
                if (val.Equals("true") || val.Equals("false"))
                {
                    _bumper_Standings_Background_Visible = char.ToUpper(value[0]) + value.Substring(1);
                }
                else if (val.Equals("yes"))
                {
                    _bumper_Standings_Background_Visible = "True";
                }
                else if (val.Equals("no"))
                {
                    _bumper_Standings_Background_Visible = "False";
                }
            }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Groups Base"),
            DefaultValue("group"),
            Description("The base value of the group names without a number (Example: If you have group1, group2... the group base would be group)."),
        ]
        public string Bumper_Standings_GroupBase
        {
            get { return _bumper_Standings_GroupBase; }
            set { _bumper_Standings_GroupBase = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Team Name Base"),
            DefaultValue("txtTeamSchool"),
            Description("The base value of the schools name text box without a number (Example: If you have txtTeamSchool1, txtTeamSchool2... the base would be txtTeamSchool)."),
        ]
        public string Bumper_Standings_SchoolBase
        {
            get { return _bumper_Standings_SchoolBase; }
            set { _bumper_Standings_SchoolBase = value; }
        }
        [
            Category("Bumpers - Standings"),
            DisplayName("Team Record Base"),
            DefaultValue("txtTeamRecord"),
            Description("The base value of the teams record text box without a number (Example: If you have txtTeamRecord1, txtTeamRecord2... the base would be txtTeamRecord)."),
        ]
        public string Bumper_Standings_RecordBase
        {
            get { return _bumper_Standings_RecordBase; }
            set { _bumper_Standings_RecordBase = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Team Points Base"),
            DefaultValue("txtTeamPoints"),
            Description("The base value of the teams points text box without a number (Example: If you have txtTeamPoints1, txtTeamPoints2... the base would be txtTeamPoints)."),
        ]
        public string Bumper_Standings_PointsBase
        {
            get { return _bumper_Standings_PointsBase; }
            set { _bumper_Standings_PointsBase = value; }
        }

        [
            Category("Bumpers - Standings"),
            DisplayName("Max Teams"),
            DefaultValue(11),
            Description("The maximum amount of teams that can be shown (the amount of groups in the scene)."),
        ]
        public int Bumper_Standings_MaxTeams
        {
            get { return _bumper_Standings_MaxTeams; }
            set { _bumper_Standings_MaxTeams = value; }
        }


        #endregion

        #region Lower Thirds

        private int _l3_EventExtras_ID = 0204;
        private string _l3_EventExtras_Title = "txtTitle";
        private string _l3_EventExtras_Left_Name = "txtNameLeft";
        private string _l3_EventExtras_Left_Title = "txtTitleLeft";
        private string _l3_EventExtras_Middle_Name = "txtNameMiddle";
        private string _l3_EventExtras_Middle_Title = "txtTitleMiddle";
        private string _l3_EventExtras_Right_Name = "txtNameRight";
        private string _l3_EventExtras_Right_Title = "txtTitleRight";

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Take ID"),
            DefaultValue(0204),
            Description("The Lower Third takeItem ID."),
        ]
        public int L3_EventExtras_ID
        {
            get { return _l3_EventExtras_ID; }
            set { _l3_EventExtras_ID = value; }
        }

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Title"),
            DefaultValue("txtTitle"),
            Description("The title text text box name."),
        ]
        public string L3_EventExtras_Title
        {
            get { return _l3_EventExtras_Title; }
            set { _l3_EventExtras_Title = value; }
        }

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Left Name"),
            DefaultValue("txtNameLeft"),
            Description("The left name text box name."),
        ]
        public string L3_EventExtras_Left_Name
        {
            get { return _l3_EventExtras_Left_Name; }
            set { _l3_EventExtras_Left_Name = value; }
        }

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Left Title"),
            DefaultValue("txtTitleLeft"),
            Description("The left title text box name."),
        ]
        public string L3_EventExtras_Left_Title
        {
            get { return _l3_EventExtras_Left_Title; }
            set { _l3_EventExtras_Left_Title = value; }
        }

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Middle Name"),
            DefaultValue("txtNameMiddle"),
            Description("The middle name text box name."),
        ]
        public string L3_EventExtras_Middle_Name
        {
            get { return _l3_EventExtras_Middle_Name; }
            set { _l3_EventExtras_Middle_Name = value; }
        }

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Middle Title"),
            DefaultValue("txtTitleMiddle"),
            Description("The middle title text box name."),
        ]
        public string L3_EventExtras_Middle_Title
        {
            get { return _l3_EventExtras_Middle_Title; }
            set { _l3_EventExtras_Middle_Title = value; }
        }

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Right Name"),
            DefaultValue("txtNameRight"),
            Description("The right name text box name."),
        ]
        public string L3_EventExtras_Right_Name
        {
            get { return _l3_EventExtras_Right_Name; }
            set { _l3_EventExtras_Right_Name = value; }
        }

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Right Title"),
            DefaultValue("txtTitleRight"),
            Description("The right title text box name."),
        ]
        public string L3_EventExtras_Right_Title
        {
            get { return _l3_EventExtras_Right_Title; }
            set { _l3_EventExtras_Right_Title = value; }
        }

        private int _l3_PlayerInfo_ID = 0201;
        private string _l3_PlayerInfo_TeamQuad = "TeamLogo";
        private string _l3_PlayerInfo_Name = "PlayerName";
        private string _l3_PlayerInfo_Number = "PlayerNumber";
        private string _l3_PlayerInfo_Info = "txtInfo";

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Take ID"),
            DefaultValue(0201),
            Description("The player info takeItem ID."),
        ]
        public int L3_PlayerInfo_ID
        {
            get { return _l3_PlayerInfo_ID; }
            set { _l3_PlayerInfo_ID = value; }
        }

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Team quad"),
            DefaultValue("TeamLogo"),
            Description("The player info team background quad."),
        ]
        public string L3_PlayerInfo_TeamQuad
        {
            get { return _l3_PlayerInfo_TeamQuad; }
            set { _l3_PlayerInfo_TeamQuad = value; }
        }

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Player Name"),
            DefaultValue("PlayerName"),
            Description("The players name text box name."),
        ]
        public string L3_PlayerInfo_Name
        {
            get { return _l3_PlayerInfo_Name ; }
            set { _l3_PlayerInfo_Name = value; }
        }

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Player Number"),
            DefaultValue("PlayerNumber"),
            Description("The players number text box name."),
        ]
        public string L3_PlayerInfo_Number
        {
            get { return _l3_PlayerInfo_Number; }
            set { _l3_PlayerInfo_Number = value; }
        }

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Info"),
            DefaultValue("txtInfo"),
            Description("The players information text box name."),
        ]
        public string L3_PlayerInfo_Info
        {
            get { return _l3_PlayerInfo_Info; }
            set { _l3_PlayerInfo_Info = value; }
        }

        private int _l3_PlayerStats_ID = 0202;
        private string _l3_PlayerStats_TeamQuad = "TeamLogo";
        private string _l3_PlayerStats_Name = "PlayerName";
        private string _l3_PlayerStats_Number = "PlayerNumber";
        private string _l3_PlayerStats_Info = "txtInfo";
        private string _l3_PlayerStats_Highlight = "txtHighlight";

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Take ID"),
            DefaultValue(0202),
            Description("The player stats takeItem ID."),
        ]
        public int L3_PlayerStats_ID
        {
            get { return _l3_PlayerStats_ID; }
            set { _l3_PlayerStats_ID = value; }
        }

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Team quad"),
            DefaultValue("TeamLogo"),
            Description("The player stats team background quad."),
        ]
        public string L3_PlayerStats_TeamQuad
        {
            get { return _l3_PlayerStats_TeamQuad; }
            set { _l3_PlayerStats_TeamQuad = value; }
        }

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Player Name"),
            DefaultValue("PlayerName"),
            Description("The players name text box name."),
        ]
        public string L3_PlayerStats_Name
        {
            get { return _l3_PlayerStats_Name; }
            set { _l3_PlayerStats_Name = value; }
        }

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Player Number"),
            DefaultValue("PlayerNumber"),
            Description("The players number text box name."),
        ]
        public string L3_PlayerStats_Number
        {
            get { return _l3_PlayerStats_Number; }
            set { _l3_PlayerStats_Number = value; }
        }

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Info"),
            DefaultValue("txtInfo"),
            Description("The information text box name."),
        ]
        public string L3_PlayerStats_Info
        {
            get { return _l3_PlayerStats_Info; }
            set { _l3_PlayerStats_Info = value; }
        }

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Highlight"),
            DefaultValue("txtHighlight"),
            Description("The bottom highlight text box name."),
        ]
        public string L3_PlayerStats_Highlight
        {
            get { return _l3_PlayerStats_Highlight; }
            set { _l3_PlayerStats_Highlight = value; }
        }

        private int _l3_TeamInfo_ID = 0203;
        private string _l3_TeamInfo_TeamQuad = "TeamLogo";
        private string _l3_TeamInfo_Title = "txtTitle";
        private string _l3_TeamInfo_Info = "txtInfo";

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Take ID"),
            DefaultValue(0203),
            Description("The team info takeItem ID."),
        ]
        public int L3_TeamInfo_ID
        {
            get { return _l3_TeamInfo_ID; }
            set { _l3_TeamInfo_ID = value; }
        }

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Team quad"),
            DefaultValue("quadTeam"),
            Description("The team info team background quad."),
        ]
        public string L3_TeamInfo_TeamQuad
        {
            get { return _l3_TeamInfo_TeamQuad; }
            set { _l3_TeamInfo_TeamQuad = value; }
        }

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Title"),
            DefaultValue("txtTitle"),
            Description("The teams title text box name."),
        ]
        public string L3_TeamInfo_Title
        {
            get { return _l3_TeamInfo_Title; }
            set { _l3_TeamInfo_Title = value; }
        }

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Info"),
            DefaultValue("txtInfo"),
            Description("The teams information text box name."),
        ]
        public string L3_TeamInfo_Info
        {
            get { return _l3_TeamInfo_Info; }
            set { _l3_TeamInfo_Info = value; }
        }

        private int _l3_Custom_ID = 0204;
        private string _l3_Custom_Title = "txtTitle";
        private string _l3_Custom_Message = "txtMessage";

        [
            Category("Lower Thirds - Custom"),
            DisplayName("Take ID"),
            DefaultValue(0204),
            Description("The custom lower third takeItem ID."),
        ]
        public int L3_Custom_ID
        {
            get { return _l3_Custom_ID; }
            set { _l3_Custom_ID = value; }
        }

        [
            Category("Lower Thirds - Custom"),
            DisplayName("Title Box"),
            DefaultValue("txtTitle"),
            Description("The custom lower third title text box name."),
        ]
        public string L3_Custom_Title
        {
            get { return _l3_Custom_Title; }
            set { _l3_Custom_Title = value; }
        }

        [
            Category("Lower Thirds - Custom"),
            DisplayName("Message Box"),
            DefaultValue("txtMessage"),
            Description("The custom lower third message text box name."),
        ]
        public string L3_Custom_Message
        {
            get { return _l3_Custom_Message; }
            set { _l3_Custom_Message = value; }
        }

        #endregion

        #region Scorebug

        private int _scorebug_ID = 0003;

        [
            Category("Scorebug - Main"),
            DisplayName("Take ID"),
            DefaultValue(0003),
            Description("The main scorebug takeItem ID."),
        ]
        public int Scorebug_ID
        {
            get { return _scorebug_ID; }
            set { _scorebug_ID = value; }
        }

        private int _scorebug_Info_ID = 0004;
        private string _scorebug_Info_Name = "txtInfo";
        private string _scorebug_Info_Data = "Coming Up,1st Set,2nd Set,3rd Set,4th Set,5th Set,{HomeName} Match Point,{HomeName} Set,{HomeName} Red Card,{HomeName} Yellow Card,{AwayName} Match Point,{AwayName} Set,{AwayName} Red Card,{AwayName} Yellow Card,Final";

        [
            Category("Scorebug - Info"),
            DisplayName("Information bug TakeID"),
            DefaultValue(0004),
            Description("The info bug take ID."),
        ]
        public int Scorebug_Info_ID
        {
            get { return _scorebug_Info_ID; }
            set { _scorebug_Info_ID = value; }
        }

        [
            Category("Scorebug - Info"),
            DisplayName("Info text element"),
            DefaultValue("txtInfo"),
            Description("The info box text name."),
        ]
        public string Scorebug_Info_Name
        {
            get { return _scorebug_Info_Name; }
            set { _scorebug_Info_Name = value; }
        }

        [
            Category("Scorebug - Info"),
            DisplayName("Bug Info Data Options"),
            DefaultValue("Coming Up,1st Set,2nd Set,3rd Set,4th Set,5th Set,{HomeName} Match Point,{HomeName} Set,{AwayName} Match Point,{AwayName} Set,Final"),
            Description("A comma-seperated list of options to populate the bug info widget with.")
        ]
        public string Scorebug_Info_Data
        {
            get { return this._scorebug_Info_Data; }
            set { this._scorebug_Info_Data = value; }
        }

        private int _scorebug_YellowCard_ID = 0005;
        private string _scorebug_YellowCard_Text = "txtInfo";

        [
            Category("Scorebug - Yellow Card"),
            DisplayName("Yellow Card TakeID"),
            DefaultValue(0005),
            Description("The yellow card take ID."),
        ]
        public int Scorebug_YellowCard_ID
        {
            get { return _scorebug_YellowCard_ID; }
            set { _scorebug_YellowCard_ID = value; }
        }

        [
            Category("Scorebug - Yellow Card"),
            DisplayName("Info text element"),
            DefaultValue("txtInfo"),
            Description("The info box text name."),
        ]
        public string Scorebug_YellowCard_Text
        {
            get { return _scorebug_YellowCard_Text; }
            set { _scorebug_YellowCard_Text = value; }
        }

        private int _scorebug_RedCard_ID = 0006;
        private string _scorebug_RedCard_Text = "txtInfo";
        
        [
            Category("Scorebug - Red Card"),
            DisplayName("Red Card TakeID"),
            DefaultValue(0006),
            Description("The red card take ID."),
        ]
        public int Scorebug_RedCard_ID
        {
            get { return _scorebug_RedCard_ID; }
            set { _scorebug_RedCard_ID = value; }
        }

        [
            Category("Scorebug - Red Card"),
            DisplayName("Info text element"),
            DefaultValue("txtInfo"),
            Description("The info box text name."),
        ]
        public string Scorebug_RedCard_Text
        {
            get { return _scorebug_RedCard_Text; }
            set { _scorebug_RedCard_Text = value; }
        }

        #endregion

        #region Credits

        // Credits Variables
        private int _credits_ID = 901;
        private string _credits_Text = "txtText";
        private int _credits_copyright_id = 902;

        [
            Category("Credits"),
            DisplayName("Credits Main TakeID"),
            DefaultValue(901),
            Description("The main credits take ID."),
        ]
        public int Credits_ID
        {
            get { return _credits_ID; }
            set { _credits_ID = value; }
        }

        [
            Category("Credits"),
            DisplayName("Text Box"),
            DefaultValue("txtText"),
            Description("The text box element name."),
        ]
        public string Credits_Text
        {
            get { return _credits_Text; }
            set { _credits_Text = value; }
        }

        [
            Category("Credits"),
            DisplayName("Credits Copyright TakeID"),
            DefaultValue(902),
            Description("The copyright credits page take ID."),
        ]
        public int Credits_Copyright_ID
        {
            get { return _credits_copyright_id; }
            set { _credits_copyright_id = value; }
        }

        #endregion

    }
}

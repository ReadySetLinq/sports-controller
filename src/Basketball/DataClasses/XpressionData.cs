using System.ComponentModel;

namespace SportsController.Basketball
{
    public class XpressionData
    {

        #region Widgets
        [
            Category("Widgets"),
            DisplayName("Home Team Points"),
            DefaultValue("Home Score"),
            Description("The Home Team points widget."),
        ]
        public string Widget_Home_Score { get; set; } = "Home Score";
        [
            Category("Widgets"),
            DisplayName("Away Team Points"),
            DefaultValue("Away Score"),
            Description("The Away Team points widget."),
        ]
        public string Widget_Away_Score { get; set; } = "Away Score";

        [
            Category("Widgets"),
            DisplayName("Home Team Fouls"),
            DefaultValue("Home Fouls"),
            Description("The Home Team fouls widget."),
        ]
        public string Widget_Home_Fouls { get; set; } = "Home Fouls";
        [
            Category("Widgets"),
            DisplayName("Away Team Fouls"),
            DefaultValue("Away Fouls"),
            Description("The Away Team fouls widget."),
        ]
        public string Widget_Away_Fouls { get; set; } = "Away Fouls";

        [
            Category("Widgets"),
            DisplayName("Game Timer"),
            DefaultValue("GameClock"),
            Description("The Game clock widget."),
        ]
        public string Widget_GameClock { get; set; } = "GameClock";


        [
            Category("Widgets"),
            DisplayName("Shot Timer"),
            DefaultValue("ShotClock"),
            Description("The Shot clock widget."),
        ]
        public string Widget_ShotClock { get; set; } = "ShotClock";

        [
            Category("Widgets"),
            DisplayName("Quarter Text List"),
            DefaultValue("Quarter"),
            Description("The Quarter text list widget."),
        ]
        public string Widget_Quarter { get; set; } = "Quarter";

        #endregion

        #region Misc
        [
            Category("Misc"),
            DisplayName("Network Bug Take ID"),
            DefaultValue(0001),
            Description("The Network Bug takeItem ID."),
        ]
        public int Misc_Network_Bug { get; set; } = 0001;

        [
            Category("Misc"),
            DisplayName("Replay Bug Take ID"),
            DefaultValue(0002),
            Description("The Replay Bug takeItem ID."),
        ]
        public int Misc_Replay_Bug { get; set; } = 0002;


        #endregion

        #region Bumpers
        [
            Category("Bumpers - Locator"),
            DisplayName("Take ID"),
            DefaultValue(0101),
            Description("The Locator bumper takeItem ID."),
        ]
        public int Bumper_Locator_ID { get; set; } = 0101;

        [
            Category("Bumpers - Locator"),
            DisplayName("Heading Element"),
            DefaultValue("txtHeading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_Locator_Heading { get; set; } = "txtHeading";

        [
            Category("Bumpers - Locator"),
            DisplayName("Title Element"),
            DefaultValue("txtVenue"),
            Description("The bumpers venue text box name."),
        ]
        public string Bumper_Locator_Venue { get; set; } = "txtVenue";

        [
            Category("Bumpers - Locator"),
            DisplayName("Subtitle Element"),
            DefaultValue("txtLocation"),
            Description("The bumpers location text box name."),
        ]
        public string Bumper_Locator_Location { get; set; } = "txtLocation";

        [
            Category("Bumpers - Score"),
            DisplayName("Take ID"),
            DefaultValue(0102),
            Description("The Score bumper takeItem ID."),
        ]
        public int Bumper_Score_ID { get; set; } = 0102;

        [
            Category("Bumpers - Score"),
            DisplayName("Heading Element"),
            DefaultValue("txtHeading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_Score_Heading { get; set; } = "txtHeading";

        [
            Category("Bumpers - Score"),
            DisplayName("Info Element"),
            DefaultValue("txtInfo"),
            Description("The bumpers information text box name."),
        ]
        public string Bumper_Score_Info { get; set; } = "txtInfo";

        [
            Category("Bumpers - Score"),
            DisplayName("Info Value"),
            DefaultValue(""),
            Description("The bumpers information text box value."),
        ]
        public string Bumper_Score_Info_Value { get; set; } = "";

        [
            Category("Bumpers - Score"),
            DisplayName("Home Team Element"),
            DefaultValue("txtHomeTeam"),
            Description("The bumpers home team text box name."),
        ]
        public string Bumper_Score_HomeTeam { get; set; } = "txtHomeTeam";

        [
            Category("Bumpers - Score"),
            DisplayName("Away Team Element"),
            DefaultValue("txtAwayTeam"),
            Description("The bumpers away team text box name."),
        ]
        public string Bumper_Score_AwayTeam { get; set; } = "txtAwayTeam";

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
        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Take ID"),
            DefaultValue(0204),
            Description("The Lower Third takeItem ID."),
        ]
        public int L3_EventExtras_ID { get; set; } = 0205;

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Title"),
            DefaultValue("txtTitle"),
            Description("The title text text box name."),
        ]
        public string L3_EventExtras_Title { get; set; } = "txtTitle";

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Left Name"),
            DefaultValue("txtNameLeft"),
            Description("The left name text box name."),
        ]
        public string L3_EventExtras_Left_Name { get; set; } = "txtNameLeft";

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Left Title"),
            DefaultValue("txtTitleLeft"),
            Description("The left title text box name."),
        ]
        public string L3_EventExtras_Left_Title { get; set; } = "txtTitleLeft";

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Middle Name"),
            DefaultValue("txtNameMiddle"),
            Description("The middle name text box name."),
        ]
        public string L3_EventExtras_Middle_Name { get; set; } = "txtNameMiddle";

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Middle Title"),
            DefaultValue("txtTitleMiddle"),
            Description("The middle title text box name."),
        ]
        public string L3_EventExtras_Middle_Title { get; set; } = "txtTitleMiddle";

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Right Name"),
            DefaultValue("txtNameRight"),
            Description("The right name text box name."),
        ]
        public string L3_EventExtras_Right_Name { get; set; } = "txtNameRight";

        [
            Category("Lower Thirds - Event Extras"),
            DisplayName("Right Title"),
            DefaultValue("txtTitleRight"),
            Description("The right title text box name."),
        ]
        public string L3_EventExtras_Right_Title { get; set; } = "txtTitleRight";

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Take ID"),
            DefaultValue(0201),
            Description("The player info takeItem ID."),
        ]
        public int L3_PlayerInfo_ID { get; set; } = 0201;

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Team quad"),
            DefaultValue("TeamLogo"),
            Description("The player info team background quad."),
        ]
        public string L3_PlayerInfo_TeamQuad { get; set; } = "TeamLogo";

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Player Name"),
            DefaultValue("PlayerName"),
            Description("The players name text box name."),
        ]
        public string L3_PlayerInfo_Name { get; set; } = "PlayerName";

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Player Number"),
            DefaultValue("PlayerNumber"),
            Description("The players number text box name."),
        ]
        public string L3_PlayerInfo_Number { get; set; } = "PlayerNumber";

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Info"),
            DefaultValue("txtInfo"),
            Description("The players information text box name."),
        ]
        public string L3_PlayerInfo_Info { get; set; } = "txtInfo";

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Take ID"),
            DefaultValue(0202),
            Description("The player stats takeItem ID."),
        ]
        public int L3_PlayerStats_ID { get; set; } = 0202;

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Team quad"),
            DefaultValue("TeamLogo"),
            Description("The player stats team background quad."),
        ]
        public string L3_PlayerStats_TeamQuad { get; set; } = "TeamLogo";

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Player Name"),
            DefaultValue("PlayerName"),
            Description("The players name text box name."),
        ]
        public string L3_PlayerStats_Name { get; set; } = "PlayerName";

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Player Number"),
            DefaultValue("PlayerNumber"),
            Description("The players number text box name."),
        ]
        public string L3_PlayerStats_Number { get; set; } = "PlayerNumber";

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Info"),
            DefaultValue("txtInfo"),
            Description("The information text box name."),
        ]
        public string L3_PlayerStats_Info { get; set; } = "txtInfo";

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Highlight"),
            DefaultValue("txtHighlight"),
            Description("The bottom highlight text box name."),
        ]
        public string L3_PlayerStats_Highlight { get; set; } = "txtHighlight";

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Take ID"),
            DefaultValue(0203),
            Description("The team info takeItem ID."),
        ]
        public int L3_TeamInfo_ID { get; set; } = 0203;

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Team quad"),
            DefaultValue("quadTeam"),
            Description("The team info team background quad."),
        ]
        public string L3_TeamInfo_TeamQuad { get; set; } = "TeamLogo";

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Title"),
            DefaultValue("txtTitle"),
            Description("The teams title text box name."),
        ]
        public string L3_TeamInfo_Title { get; set; } = "txtTitle";

        [
            Category("Lower Thirds - Team Stats"),
            DisplayName("Info"),
            DefaultValue("txtInfo"),
            Description("The teams information text box name."),
        ]
        public string L3_TeamInfo_Info { get; set; } = "txtInfo";

        [
            Category("Lower Thirds - Custom"),
            DisplayName("Take ID"),
            DefaultValue(0204),
            Description("The custom lower third takeItem ID."),
        ]
        public int L3_Custom_ID { get; set; } = 0204;

        [
            Category("Lower Thirds - Custom"),
            DisplayName("Title Box"),
            DefaultValue("txtTitle"),
            Description("The custom lower third title text box name."),
        ]
        public string L3_Custom_Title { get; set; } = "txtTitle";

        [
            Category("Lower Thirds - Custom"),
            DisplayName("Message Box"),
            DefaultValue("txtMessage"),
            Description("The custom lower third message text box name."),
        ]
        public string L3_Custom_Message { get; set; } = "txtMessage";

        #endregion

        #region Scorebug

        [
            Category("Scorebug - Main"),
            DisplayName("Take ID"),
            DefaultValue(0003),
            Description("The main scorebug takeItem ID."),
        ]
        public int Scorebug_ID { get; set; } = 0003;

        [
            Category("Scorebug - Main"),
            DisplayName("Shotclock In Scene Director"),
            DefaultValue("Shotclock-IN"),
            Description("The scene director name for the shotclock 'out' animation. (Leave blank if you are not using the shotclock)"),
        ]
        public string Scorebug_ShotClock_Director_IN { get; set; } = "Shotclock-IN";

        [
            Category("Scorebug - Main"),
            DisplayName("Shotclock Out Scene Director"),
            DefaultValue("Shotclock-OUT"),
            Description("The scene director name for the shotclock 'out' animation. (Leave blank if you are not using the shotclock)"),
        ]
        public string Scorebug_ShotClock_Director_OUT { get; set; } = "Shotclock-OUT";

        [
            Category("Scorebug - Info"),
            DisplayName("Information bug TakeID"),
            DefaultValue(0004),
            Description("The info bug take ID."),
        ]
        public int Scorebug_Info_ID { get; set; } = 0004;

        [
            Category("Scorebug - Info"),
            DisplayName("Info text element"),
            DefaultValue("txtInfo"),
            Description("The info box text name."),
        ]
        public string Scorebug_Info_Name { get; set; } = "txtInfo";

        [
            Category("Scorebug - Info"),
            DisplayName("Bug Info Data Options"),
            DefaultValue("Coming Up,1st,End of the 1st,2nd,End of the 2nd,Halftime,3rd,End of the 3rd,4th,End of the 5th,OT,End of OT,{HomeName} Victory,{AwayName} Victory,Final"),
            Description("A comma-seperated list of options to populate the bug info widget with.")
        ]
        public string Scorebug_Info_Data { get; set; } = "Coming Up,1st,End of the 1st,2nd,End of the 2nd,Halftime,3rd,End of the 3rd,4th,End of the 5th,OT,End of OT,{HomeName} Victory,{AwayName} Victory,Final";

        #endregion

        #region Credits
        [
            Category("Credits"),
            DisplayName("Credits Main TakeID"),
            DefaultValue(901),
            Description("The main credits take ID."),
        ]
        public int Credits_ID { get; set; } = 901;

        [
            Category("Credits"),
            DisplayName("Text Box"),
            DefaultValue("txtText"),
            Description("The text box element name."),
        ]
        public string Credits_Text { get; set; } = "txtText";

        [
            Category("Credits"),
            DisplayName("Credits Copyright TakeID"),
            DefaultValue(902),
            Description("The copyright credits page take ID."),
        ]
        public int Credits_Copyright_ID { get; set; } = 902;

        #endregion

    }
}

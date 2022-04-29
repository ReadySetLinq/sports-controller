using System.ComponentModel;

namespace SportsController.Basketball
{
    public class vMixData
    {

        #region Overlays

        private int Overlay_layer(int value)
        {
            if (value < 0) return 0;
            else if (value <= 4) return value;
            
            return 4;
        }

        private int overlays_Network = 4;
        [
            Category("Overlay Layers"),
            DisplayName("Network Bug"),
            DefaultValue(4),
            Description("The Network Bug overlay layer."),
        ]
        public int Overlays_Network
        {
            get => overlays_Network;
            set => overlays_Network = Overlay_layer(value);
        }

        private int overlays_Replay = 3;
        [
            Category("Overlay Layers"),
            DisplayName("Replay Bug"),
            DefaultValue(3),
            Description("The Replay Bug overlay layer."),
        ]
        public int Overlays_Replay
        {
            get => overlays_Replay;
            set => overlays_Replay = Overlay_layer(value);
        }

        private int overlays_Scorebug = 3;
        [
            Category("Overlay Layers"),
            DisplayName("Score Bug"),
            DefaultValue(3),
            Description("The Score Bug overlay layer."),
        ]
        public int Overlays_Scorebug
        {
            get => overlays_Scorebug;
            set => overlays_Scorebug = Overlay_layer(value);
        }

        private int overlays_LowerThird = 2;
        [
            Category("Overlay Layers"),
            DisplayName("Lower Thirds"),
            DefaultValue(3),
            Description("All Lower Thirds overlay layer."),
        ]
        public int Overlays_LowerThird
        {
            get => overlays_LowerThird;
            set => overlays_LowerThird = Overlay_layer(value);
        }

        private int overlays_Bumper = 3;
        [
            Category("Overlay Layers"),
            DisplayName("Bumpers"),
            DefaultValue(3),
            Description("All Bumpers overlay layer."),
        ]
        public int Overlays_Bumper
        {
            get => overlays_Bumper;
            set => overlays_Bumper = Overlay_layer(value);
        }

        #endregion

        #region Widgets
        [
            Category("Widgets"),
            DisplayName("Input Name"),
            DefaultValue("Widgets"),
            Description("The input name where all widget data is stored."),
        ]
        public string Widget_Input_Name { get; set; } = "Widgets";

        [
            Category("Widgets"),
            DisplayName("Home Team Points"),
            DefaultValue("homeScore"),
            Description("The Home Team points widget."),
        ]
        public string Widget_Home_Score { get; set; } = "homeScore";
        [
            Category("Widgets"),
            DisplayName("Away Team Points"),
            DefaultValue("awayScore"),
            Description("The Away Team points widget."),
        ]
        public string Widget_Away_Score { get; set; } = "awayScore";

        [
            Category("Widgets"),
            DisplayName("Home Team Fouls"),
            DefaultValue("homeFouls"),
            Description("The Home Team fouls widget."),
        ]
        public string Widget_Home_Fouls { get; set; } = "homeFouls";
        [
            Category("Widgets"),
            DisplayName("Away Team Fouls"),
            DefaultValue("awayFouls"),
            Description("The Away Team fouls widget."),
        ]
        public string Widget_Away_Fouls { get; set; } = "awayFouls";

        [
            Category("Widgets"),
            DisplayName("Game Timer"),
            DefaultValue("gameClock"),
            Description("The Game clock widget."),
        ]
        public string Widget_GameClock { get; set; } = "gameClock";


        [
            Category("Widgets"),
            DisplayName("Shot Timer"),
            DefaultValue("shotClock"),
            Description("The Shot clock widget."),
        ]
        public string Widget_ShotClock { get; set; } = "shotClock";

        [
            Category("Widgets"),
            DisplayName("Quarter Text List"),
            DefaultValue("quarter"),
            Description("The Quarter text list widget."),
        ]
        public string Widget_Quarter { get; set; } = "quarter";

        #endregion

        #region Misc
        [
            Category("Misc"),
            DisplayName("Network Bug Input Name"),
            DefaultValue("bugNetwork"),
            Description("The Network Bug takeItem Name."),
        ]
        public string Misc_Network_Bug { get; set; } = "bugNetwork";

        [
            Category("Misc"),
            DisplayName("Replay Bug Take ID"),
            DefaultValue("bugReplay"),
            Description("The Replay Bug takeItem ID."),
        ]
        public string Misc_Replay_Bug { get; set; } = "bugReplay";


        #endregion

        #region Bumpers
        [
            Category("Bumpers - Locator"),
            DisplayName("Locator Bumper Input Name"),
            DefaultValue("bumper_Locator"),
            Description("The Locator Bumper Input Name."),
        ]
        public string Bumper_Locator_ID { get; set; } = "bumper_Locator";

        [
            Category("Bumpers - Locator"),
            DisplayName("Heading Element"),
            DefaultValue("heading"),
            Description("The bumpers heading text box name."),
        ]
        public string Bumper_Locator_Heading { get; set; } = "location";

        [
            Category("Bumpers - Locator"),
            DisplayName("Venue Element"),
            DefaultValue("venue"),
            Description("The bumpers venue text box name."),
        ]
        public string Bumper_Locator_Venue { get; set; } = "venue";

        [
            Category("Bumpers - Locator"),
            DisplayName("Subtitle Element"),
            DefaultValue("Location"),
            Description("The bumpers location text box name."),
        ]
        public string Bumper_Locator_Location { get; set; } = "location";

        [
            Category("Bumpers - Score"),
            DisplayName("Score Bumper Input Name"),
            DefaultValue("bugScore_BottomLeft"),
            Description("The Score Bumper Input Name."),
        ]
        public string Bumper_Score_Name { get; set; } = "bugScore_BottomLeft";

        [
            Category("Bumpers - Score"),
            DisplayName("Info Element"),
            DefaultValue("info"),
            Description("The bumpers information text box name."),
        ]
        public string Bumper_Score_Info { get; set; } = "info";

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
            DefaultValue("homeAbbr"),
            Description("The bumpers home team text box name."),
        ]
        public string Bumper_Score_HomeTeam { get; set; } = "homeAbbr";

        [
            Category("Bumpers - Score"),
            DisplayName("Away Team Element"),
            DefaultValue("awayAbbr"),
            Description("The bumpers away team text box name."),
        ]
        public string Bumper_Score_AwayTeam { get; set; } = "awayAbbr";

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
            DisplayName("Home Take ID"),
            DefaultValue(0201),
            Description("The home team player info takeItem ID."),
        ]
        public int L3_PlayerInfo_Home_ID { get; set; } = 0201;

        [
            Category("Lower Thirds - Player Info"),
            DisplayName("Away Take ID"),
            DefaultValue(0201),
            Description("The away team player info takeItem ID."),
        ]
        public int L3_PlayerInfo_Away_ID { get; set; } = 0201;

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
            DisplayName("Home Take ID"),
            DefaultValue(0202),
            Description("The home team player stats takeItem ID."),
        ]
        public int L3_PlayerStats_Home_ID { get; set; } = 0202;

        [
            Category("Lower Thirds - Player Stats"),
            DisplayName("Away Take ID"),
            DefaultValue(0202),
            Description("The away team player stats takeItem ID."),
        ]
        public int L3_PlayerStats_Away_ID { get; set; } = 0202;

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
            Category("Lower Thirds - Team Info"),
            DisplayName("Home Take ID"),
            DefaultValue(0203),
            Description("The home team info takeItem ID."),
        ]
        public int L3_TeamInfo_Home_ID { get; set; } = 0203;

        [
            Category("Lower Thirds - Team Info"),
            DisplayName("Away Take ID"),
            DefaultValue(0203),
            Description("The away team info takeItem ID."),
        ]
        public int L3_TeamInfo_Away_ID { get; set; } = 0203;

        [
            Category("Lower Thirds - Team Info"),
            DisplayName("Team quad"),
            DefaultValue("quadTeam"),
            Description("The team info team background quad."),
        ]
        public string L3_TeamInfo_TeamQuad { get; set; } = "TeamLogo";

        [
            Category("Lower Thirds - Team Info"),
            DisplayName("Title"),
            DefaultValue("txtTitle"),
            Description("The teams title text box name."),
        ]
        public string L3_TeamInfo_Title { get; set; } = "txtTitle";

        [
            Category("Lower Thirds - Team Info"),
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
            DisplayName("Name"),
            DefaultValue("bugScore"),
            Description("The main scorebug input name."),
        ]
        public string Scorebug_Name { get; set; } = "bugScore";

        [
            Category("Scorebug - Main"),
            DisplayName("Shotclock"),
            DefaultValue("shotClock"),
            Description("The shot clock text object to hide/show. (Leave blank if you are not using the shotclock)"),
        ]
        public string Scorebug_ShotClock { get; set; } = "shotClock";

        [
            Category("Scorebug - Main"),
            DisplayName("Information"),
            DefaultValue("info"),
            Description("The info text object."),
        ]
        public string Scorebug_Info_ID { get; set; } = "info";

        [
            Category("Scorebug - Info"),
            DisplayName("Bug Info Data Options"),
            DefaultValue("Coming Up,1st,End of the 1st,2nd,End of the 2nd,Halftime,3rd,End of the 3rd,4th,End of the 5th,OT,End of OT,{HomeName} Victory,{AwayName} Victory,Final"),
            Description("A comma-seperated list of options to populate the bug info widget with.")
        ]
        public string Scorebug_Info_Data { get; set; } = "Coming Up,1st,End of the 1st,2nd,End of the 2nd,Halftime,3rd,End of the 3rd,4th,End of the 5th,OT,End of OT,{HomeName} Victory,{AwayName} Victory,Final";

        #endregion

    }
}

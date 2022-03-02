using System.ComponentModel;

namespace SportsController.Basketball
{
    public class ScoreBridgeData
    {
        [
            Category("ScoreBridge Config"),
            DisplayName("URL"),
            DefaultValue(""),
            Description("The URL for ScoreBridge data."),
        ]
        public string URL { get; set; } = "";
        [
            Category("ScoreBridge Config"),
            DisplayName("Update econds"),
            DefaultValue(500),
            Description("The amount of seconds between checking for new ScoreBridge data."),
        ]
        public int Update_Seconds { get; set; } = 500;

        [
            Category("ScoreBridge Data"),
            DisplayName("Home Team Points"),
            DefaultValue("HomeScore"),
            Description("The Home Team points property."),
        ]
        public string Home_Score { get; set; } = "homeScore";

        [
            Category("ScoreBridge Data"),
            DisplayName("Away Team Points"),
            DefaultValue("AwayScore"),
            Description("The Away Team points property."),
        ]
        public string Away_Score { get; set; } = "awayScore";

        [
            Category("ScoreBridge Data"),
            DisplayName("Home Team Fouls"),
            DefaultValue("HomeFouls"),
            Description("The Home Team points property."),
        ]
        public string Home_Fouls { get; set; } = "homeFouls";

        [
            Category("ScoreBridge Data"),
            DisplayName("Away Team Fouls"),
            DefaultValue("AwayFouls"),
            Description("The Away Team points property."),
        ]
        public string Away_Fouls { get; set; } = "awayFouls";

        [
            Category("ScoreBridge Data"),
            DisplayName("Game Clock Timer"),
            DefaultValue("GameClock"),
            Description("The Game clock property."),
        ]
        public string Game_Clock { get; set; } = "gameClock";


        [
            Category("ScoreBridge Data"),
            DisplayName("Shot Clock Timer"),
            DefaultValue("ShotClock"),
            Description("The Shot clock property."),
        ]
        public string Shot_Clock { get; set; } = "shotClock";

        [
            Category("ScoreBridge Data"),
            DisplayName("Quarter Text List"),
            DefaultValue("Quarter"),
            Description("The Quarter property."),
        ]
        public string Quarter { get; set; } = "quarter";

    }
}

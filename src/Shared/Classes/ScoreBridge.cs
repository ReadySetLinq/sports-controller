using System.Net;
using System.Timers;
using Newtonsoft.Json.Linq;
using SportsController.Basketball;
using SportsController.Volleyball;

namespace SportsController.Shared
{
    public class LastScoreBridge
    {
        public int HomeScore { get; set; } = 0;
        public int AwayScore { get; set; } = 0;
        public int HomeFouls { get; set; } = 0;
        public int AwayFouls { get; set; } = 0;
        public string GameClock { get; set; } = "";
        public string ShotClock { get; set; } = "";
        public string Quarter { get; set; } = "";

        public void Reset()
        {
            HomeScore = 0;
            AwayScore = 0;
            HomeFouls = 0;
            AwayFouls = 0;
            GameClock = "";
            ShotClock = "";
            Quarter = "";
        }
    }

    public class ScoreBridge
    {
        public string URL { get; set; } = "";
        public int UpdateSeconds { get; set; } = 500;
        public string HomeScore { get; set; } = "";
        public string AwayScore { get; set; } = "";
        public string HomeFouls { get; set; } = "";
        public string AwayFouls { get; set; } = "";
        public string GameClock { get; set; } = "";
        public string ShotClock { get; set; } = "";
        public string Quarter { get; set; } = "";
        public bool IsLive { get; set; } = false;

        private FormBasketball frmBasketball = null;
        private FormVolleyball frmVolleyball = null;
        private readonly Timer fetchTimer = new Timer();
        private readonly LastScoreBridge oldData = new LastScoreBridge();

        public ScoreBridge(FormBasketball frm)
        {
            frmBasketball = frm;
            frmVolleyball = null;
            Setup();
        }

        public ScoreBridge(FormVolleyball frm)
        {
            frmBasketball = null;
            frmVolleyball = frm;
            Setup();
        }

        private void Setup()
        {
            URL = "";
            UpdateSeconds = 500;
            HomeScore = "";
            AwayScore = "";
            GameClock = "";
            ShotClock = "";
            Quarter = "";

            fetchTimer.Elapsed += new ElapsedEventHandler(GetHTTPData);
            fetchTimer.Interval = UpdateSeconds;

            oldData.Reset();

            IsLive = false;
        }

        public bool Start()
        {
            if (IsLive) return true;

            oldData.GameClock = "";
            oldData.ShotClock = "";

            IsLive = true;
            fetchTimer.Start();

            return IsLive;
        }

        public bool Stop()
        {
            if (!IsLive) return true;

            IsLive = false;
            fetchTimer.Stop();

            return !IsLive;
        }

        public bool Update(object data)
        {
            URL = Globals.GetObjectValue("URL", data);
            if (int.TryParse(Globals.GetObjectValue("Update_Seconds", data), out int updateSeconds))
            {
                UpdateSeconds = updateSeconds;
                fetchTimer.Interval = UpdateSeconds;
            }
            HomeScore = Globals.GetObjectValue("Home_Score", data);
            AwayScore = Globals.GetObjectValue("Away_Score", data);
            HomeFouls = Globals.GetObjectValue("Home_Fouls", data);
            AwayFouls = Globals.GetObjectValue("Away_Fouls", data);
            GameClock = Globals.GetObjectValue("Game_Clock", data);
            ShotClock = Globals.GetObjectValue("Shot_Clock", data);
            Quarter = Globals.GetObjectValue("Quarter", data);

            if (URL == "") return Stop();
            return IsLive;
        }

        private void GetHTTPData(object source, ElapsedEventArgs e)
        {
            if (!IsLive) return;

            try
            {
                using WebClient wc = new WebClient();
                string json = wc.DownloadString(URL);
                JObject obj = JObject.Parse(json);

                if (obj.TryGetValue(HomeScore, out JToken homeScore))
                {
                    int value = homeScore.ToObject<int>();
                    if (value != oldData.HomeScore)
                    {
                        if (frmBasketball != null) frmBasketball.HomeScoreUpdate(value);
                        oldData.HomeScore = value;
                    }
                }
                if (obj.TryGetValue(AwayScore, out JToken awayScore))
                {
                    int value = awayScore.ToObject<int>();
                    if (value != oldData.AwayScore)
                    {
                        if (frmBasketball != null) frmBasketball.AwayScoreUpdate(value);
                        oldData.AwayScore = value;
                    }
                }
                if (obj.TryGetValue(HomeFouls, out JToken homeFouls))
                {
                    int value = homeFouls.ToObject<int>();
                    if (value != oldData.HomeFouls)
                    {
                        if (frmBasketball != null) frmBasketball.HomeFoulsUpdate(value);
                        oldData.HomeFouls = value;
                    }
                }
                if (obj.TryGetValue(AwayFouls, out JToken awayFouls))
                {
                    int value = awayFouls.ToObject<int>();
                    if (value != oldData.AwayFouls)
                    {
                        if (frmBasketball != null) frmBasketball.AwayFoulsUpdate(value);
                        oldData.AwayFouls = value;
                    }
                }

                if (obj.TryGetValue(Quarter, out JToken quarter))
                {
                    string value = quarter.ToObject<string>();
                    if (value != oldData.Quarter)
                    {
                        if (frmBasketball != null) frmBasketball.QuaterUpdate(value);
                        oldData.Quarter = value;
                    }
                }

                if (obj.TryGetValue(GameClock, out JToken gameClock))
                {
                    string value = string.Format("{00:00:00:00}", gameClock.ToObject<string>());
                    if (value != oldData.GameClock)
                    {
                        if (frmBasketball != null) frmBasketball.GameClockUpdate(value);
                        oldData.GameClock = value;
                    }
                }
                if (obj.TryGetValue(ShotClock, out JToken shotClock))
                {
                    string value = string.Format("{00:00:00:00}", shotClock.ToObject<string>());
                    if (value != oldData.ShotClock)
                    {
                        if (frmBasketball != null) frmBasketball.ShotClockUpdate(value);
                        oldData.ShotClock = value;
                    }
                }

            } catch {  }
        }

    }
}

namespace SportsController.Basketball
{

    public enum Teams
    {
        None = 0,
        Home = 1,
        Away = 2
    }

    public class Scoreboard
    {
        #region Variables
        // Home Data
        public int HomePoints { get; set; } = 0;

        public int HomeFouls { get; set; } = 0;

        // Away Data
        public int AwayPoints { get; set; } = 0;

        public int AwayFouls { get; set; } = 0;

        // General Data
        public Teams Timeout { get; set; } = Teams.None;

        public Teams Substitution { get; set; } = Teams.None;

        public int InfoIndex { get; set; } = 0;

        public int InfoLastEdited { get; set; } = 0;

        #endregion

        // Reset all of the scoreboard data
        public void Reset()
        {
            HomePoints = 0;
            AwayPoints = 0;
            HomeFouls = 0;
            AwayFouls = 0;
            Timeout = Teams.None;
            Substitution = Teams.None;
            InfoLastEdited = 0;
        }

        public void ResetPoints()
        {
            HomePoints = 0;
            AwayPoints = 0;
        }

        public int AddPoint(Teams team = Teams.None, int amount = 1)
        {
            int _response = 0;
            switch (team)
            {
                case Teams.Home:
                    HomePoints += amount;
                    _response = HomePoints;
                    break;
                case Teams.Away:
                    AwayPoints += amount;
                    _response = AwayPoints;
                    break;
                default:
                    break;
            }

            return _response;
        }

        public int RemovePoint(Teams team = Teams.None, int amount = 1)
        {
            int _response = 0;
            switch (team)
            {
                case Teams.Home:
                    if (HomePoints > 0)
                        HomePoints -= amount;
                    _response = HomePoints;
                    break;
                case Teams.Away:
                    if (AwayPoints > 0)
                        AwayPoints -= amount;
                    _response = AwayPoints;
                    break;
                default:
                    break;
            }

            return _response;
        }
    }
}

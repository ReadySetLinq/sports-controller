using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsController.Volleyball
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
        public int HomePoints { get; set; } = 0;
        public int HomeSets { get; set; } = 0;
        public int HomeRedCards { get; private set; } = 0;
        public int HomeYellowCards { get; private set; } = 0;

        public int AwayPoints { get; set; } = 0;
        public int AwaySets { get; set; } = 0;
        public int AwayRedCards { get; private set; } = 0;
        public int AwayYellowCards { get; private set; } = 0;

        public Teams Serving { get; private set; } = Teams.None;
        public Teams Timeout { get; private set; } = Teams.None;
        public Teams Substitution { get; private set; } = Teams.None;

        public int InfoIndex { get; set; } = 0;
        public int InfoLastEdited { get; set; } = 0;

        public SetResults Results { get; set; } = new SetResults();

        #endregion

        // Reset all of the scoreboard data
        public void Reset()
        {
            HomePoints = 0;
            HomeSets = 0;
            HomeRedCards = 0;
            HomeYellowCards = 0;
            AwayPoints = 0;
            AwaySets = 0;
            AwayRedCards = 0;
            AwayYellowCards = 0;
            Results.Reset();
            Serving = Teams.None;
            Timeout = Teams.None;
            Substitution = Teams.None;
            InfoLastEdited = 0;
        }

        public void ResetPoints()
        {
            HomePoints = 0;
            AwayPoints = 0;
        }
        
        public int SetPoint(Teams team = Teams.None, bool increase = true)
        {
            int _response = 0;
            switch (team)
            {
                case Teams.Home:
                    if (increase)
                        HomePoints++;
                    else
                    {
                        if (HomePoints > 0)
                            HomePoints--;
                    }
                    _response = HomePoints;
                    break;
                case Teams.Away:
                    if (increase)
                        AwayPoints++;
                    else
                    {
                        if (AwayPoints > 0)
                            AwayPoints--;
                    }
                    _response = AwayPoints;
                    break;
                default:
                    break;
            }
            Serving = team;

            return _response;
        }

        public int SetWin(Teams team = Teams.None, bool increase = true, bool resetPoints = false)
        {
            int _response = 0;

            switch (team)
            {
                case Teams.Home:
                    if (increase)
                        HomeSets++;
                    else
                    {
                        if (HomeSets > 0)
                            HomeSets--;
                    }
                    _response = HomeSets;
                    break;
                case Teams.Away:
                    if (increase)
                        AwaySets++;
                    else
                    {
                        if (AwaySets > 0)
                            AwaySets--;
                    }
                    _response = AwaySets;
                    break;
                default:
                    break;
            }
            Serving = Teams.None;

            if (resetPoints)
            {
                HomePoints = 0;
                AwayPoints = 0;
            }

            return _response;
        }

        public int GetSetNumber()
        {
            return 1 + (HomeSets + AwaySets);
        }

        public (int, int) UpdateSetResults(int setNumber)
        {
            (int, int) _results = (HomePoints, AwayPoints);

            switch (setNumber)
            {
                case 1:
                    Results.SetOne(_results.Item1, _results.Item2);
                    break;
                case 2:
                    Results.SetTwo(_results.Item1, _results.Item2);
                    break;
                case 3:
                    Results.SetThree(_results.Item1, _results.Item2);
                    break;
                case 4:
                    Results.SetFour(_results.Item1, _results.Item2);
                    break;
                case 5:
                    Results.SetFive(_results.Item1, _results.Item2);
                    break;
            }

            return _results;
        }

        public void ResetSetResults()
        {
            Results.Reset();
        }

        public int SetServing(Teams team = Teams.None)
        {
            Serving = team;
            return (int)Serving;
        }

        public int RedCard(Teams team = Teams.None, bool increase = true)
        {
            int _response = 0;
            switch (team)
            {
                case Teams.Home:
                    if (increase)
                        HomeRedCards++;
                    else
                    {
                        if (HomeRedCards > 0)
                            HomeRedCards--;
                    }
                    _response = HomeRedCards;
                    break;
                case Teams.Away:
                    if (increase)
                        AwayRedCards++;
                    else
                    {
                        if (AwayRedCards > 0)
                            AwayRedCards--;
                    }
                    _response = AwayRedCards;
                    break;
                default:
                    break;
            }

            return _response;
        }

        public int YellowCard(Teams team = Teams.None, bool increase = true)
        {
            int _response = 0;
            switch (team)
            {
                case Teams.Home:
                    if (increase)
                        HomeYellowCards++;
                    else
                    {
                        if (HomeYellowCards > 0)
                            HomeYellowCards--;
                    }
                    _response = HomeYellowCards;
                    break;
                case Teams.Away:
                    if (increase)
                        AwayYellowCards++;
                    else
                    {
                        if (AwayYellowCards > 0)
                            AwayYellowCards--;
                    }
                    _response = AwayYellowCards;
                    break;
                default:
                    break;
            }

            return _response;
        }

        public (int, int) GetSetResult(int setNumber)
        {
            return Results.GetSetResult(setNumber);
        }

        public Teams GetSetWinner(int setNumber)
        {
            return Results.GetSetWinner(setNumber);
        }

        public void UpdateSet(int setNumber, Teams team, bool increase)
        {
            switch(setNumber)
            {
                case 1:
                    if (team == Teams.Home)
                    {

                    }
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsController.Volleyball
{
    public class SetResults
    {
        #region Variables
        private readonly Dictionary<int, int> _setOne = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _setTwo = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _setThree = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _setFour = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _setFive = new Dictionary<int, int>();

        #endregion

        public void Reset()
        {
            _setOne.Clear();
            _setTwo.Clear();
            _setThree.Clear();
            _setFour.Clear();
            _setFive.Clear();
        }

        public void SetOne(int home, int away)
        {
            _setOne.Clear();
            _setOne.Add(home, away);
        }

        public void SetTwo(int home, int away)
        {
            _setTwo.Clear();
            _setTwo.Add(home, away);
        }

        public void SetThree(int home, int away)
        {
            _setThree.Clear();
            _setThree.Add(home, away);
        }

        public void SetFour(int home, int away)
        {
            _setFour.Clear();
            _setFour.Add(home, away);
        }

        public void SetFive(int home, int away)
        {
            _setFive.Clear();
            _setFive.Add(home, away);
        }

        public (int, int) GetSetResult(int setNumber)
        {
            Dictionary<int, int> _response = new Dictionary<int, int>();
            switch (setNumber)
            {
                case 1:
                    _response = _setOne;
                    break;
                case 2:
                    _response = _setTwo;
                    break;
                case 3:
                    _response = _setThree;
                    break;
                case 4:
                    _response = _setFour;
                    break;
                case 5:
                    _response = _setFive;
                    break;
            }

            return (_response.ElementAt(0).Key, _response.ElementAt(0).Value);
        }

        public Teams GetSetWinner(int setNumber)
        {
            int homeResult = 0;
            int awayResult = 0;
            switch (setNumber)
            {
                case 1:
                    homeResult = _setOne.ElementAt(0).Key;
                    awayResult = _setOne.ElementAt(0).Value;
                    break;
                case 2:
                    homeResult = _setTwo.ElementAt(0).Key;
                    awayResult = _setTwo.ElementAt(0).Value;
                    break;
                case 3:
                    homeResult = _setThree.ElementAt(0).Key;
                    awayResult = _setThree.ElementAt(0).Value;
                    break;
                case 4:
                    homeResult = _setFour.ElementAt(0).Key;
                    awayResult = _setFour.ElementAt(0).Value;
                    break;
                case 5:
                    homeResult = _setFive.ElementAt(0).Key;
                    awayResult = _setFive.ElementAt(0).Value;
                    break;
            }

            if (homeResult > awayResult)
                return Teams.Home;
            else if (awayResult > homeResult)
                return Teams.Away;
            else
                return Teams.None;
        }

        public int GetTeamSets(Teams team)
        {
            int _setWins = 0;

            for (int setNumber = 1; setNumber < 6; setNumber++)
            {
                int homeResult = 0;
                int awayResult = 0;
                switch (setNumber)
                {
                    case 1:
                        homeResult = _setOne.ElementAt(0).Key;
                        awayResult = _setOne.ElementAt(0).Value;
                        break;
                    case 2:
                        homeResult = _setTwo.ElementAt(0).Key;
                        awayResult = _setTwo.ElementAt(0).Value;
                        break;
                    case 3:
                        homeResult = _setThree.ElementAt(0).Key;
                        awayResult = _setThree.ElementAt(0).Value;
                        break;
                    case 4:
                        homeResult = _setFour.ElementAt(0).Key;
                        awayResult = _setFour.ElementAt(0).Value;
                        break;
                    case 5:
                        homeResult = _setFive.ElementAt(0).Key;
                        awayResult = _setFive.ElementAt(0).Value;
                        break;
                }

                if (homeResult > awayResult && team == Teams.Home)
                {
                    _setWins++;
                }
                else if (awayResult > homeResult && team == Teams.Away)
                {
                    _setWins++;
                }
            }

            return _setWins;
        }
    }
}

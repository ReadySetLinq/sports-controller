
using System.Collections.Generic;

namespace SportsController.Shared.Classes
{

    public class InputItem
    {
        public string Key { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string State { get; set; }
        public int Position { get; set; }
        public int Duration { get; set; }
        public bool Loop { get; set; }
        public int SelectedIndex { get; set; }
        public string Value { get; set; }
        public int Layer { get; set; }
        public bool IsOnline { get => State == "Running"; }
        public Dictionary<string, string> Text { get; set; }
        public Dictionary<string, string> Image { get; set; }

        public InputItem (string key, int number, string type, string title, string shortTitle, string state, int position, int duration, bool loop, int selectedIndex)
        {
            Key = key;
            Number = number;
            Type = type;
            Title = title;
            ShortTitle = shortTitle;
            State = state;
            Position = position;
            Duration = duration;
            Loop = loop;
            SelectedIndex = selectedIndex;
            Value = "";
            Layer = 1;
            Text = new Dictionary<string, string>();
            Image = new Dictionary<string, string>();
        }

        public InputItem(string key, string number, string type, string title, string shortTitle, string state, string position, string duration, string loop, string selectedIndex)
        {
            Key = key;
            if (int.TryParse(number, out int _number))
                Number = _number;
            Type = type;
            Title = title;
            ShortTitle = shortTitle;
            State = state;
            if (int.TryParse(position, out int _position))
                Position = _position;
            if (int.TryParse(duration, out int _duration))
                Duration = _duration;
            if (bool.TryParse(loop, out bool _loop))
                Loop = _loop;
            if (int.TryParse(selectedIndex, out int _selectedIndex))
                SelectedIndex = _selectedIndex;
            Value = "";
            Layer = 1;
            Text = new Dictionary<string, string>();
            Image = new Dictionary<string, string>();
        }

        public bool SetOnline()
        {
            SetOffline();

            vMix.OverlayInputIn(Layer, Title);
            State = "Running";

            return true;
        }

        public bool SetOffline()
        {
            vMix.OverlayInputOut(Layer);
            State = "Paused";

            return true;
        }

        public void EditText(string objName, string value)
        {
            vMix.SetTitleText(Title, value, objName);
        }

        public void EditImage(string fileName)
        {
            vMix.SetTitleImage(Title, fileName);
        }
    }
}

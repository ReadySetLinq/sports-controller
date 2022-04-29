using SportsController.Shared.Classes;
using System;
using System.Windows.Forms;

namespace SportsController.Shared
{
    public class Input
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public Button UIbutton { get; set; }
        public InputItem Item { get; set; }

        public Input(string name, string cID, Button btn, InputItem item)
        {
            Name = name;
            ID = cID;
            Item = item;
            UIbutton = btn;
        }

        public int Layer
        {
            get
            {
                if (Item != null)
                    return Item.Layer;
                else
                    return -1;
            }
        }

        public bool IsOnline
        {
            get
            {
                if (Item != null)
                    return Item.IsOnline;
                else
                    return false;
            }
        }

        public bool SetOnline()
        {
            bool _success = false;
            if (Item != null)
                _success = Item.SetOnline();

            if (_success && UIbutton != null)
            {
                UIbutton.Text = "Online";
                UIbutton.BackColor = System.Drawing.Color.DarkGreen;
            }

            return _success;
        }

        public bool SetOffline()
        {
            bool _success = false;
            if (Item != null)
                _success = Item.SetOffline();

            if (_success && UIbutton != null)
            {
                UIbutton.Text = "Offline";
                UIbutton.BackColor = System.Drawing.Color.DarkRed;
            }

            return _success;
        }

        public void EditText(string objName, string value)
        {
            Item.EditText(objName, value);
        }

        public void EditImage(string value)
        {
            Item.EditImage(value);
        }

    }
}

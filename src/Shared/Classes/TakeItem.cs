using System;
using System.Windows.Forms;
using XPression;

namespace SportsController.Shared
{
    public class TakeItem
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Button UIbutton { get; set; }
        public xpTakeItem Item { get; set; }

        public TakeItem(string name, int cID, Button btn, xpTakeItem item)
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
                _success = Item.Execute();

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

        public void EditProperty(string objName, string propName, string value)
        {
            try
            {
                xpTakeItem takeItem = Item;
                if (takeItem != null && takeItem.GetPublishedObjectByName(objName, out xpPublishedObject publishedObject))
                {
                    int propCount = publishedObject.PropertyCount;
                    // Loop through all properties until we find the one with our selected name
                    for (int propID = 0; propID < propCount; propID++)
                    {
                        publishedObject.GetPropertyInfo(propID, out string tempName, out PropertyType propType);
                        // Check if name is what we are looking for
                        if (tempName.Equals(propName, StringComparison.OrdinalIgnoreCase))
                        {
                            switch (propType)
                            {
                                case PropertyType.pt_String:
                                    publishedObject.SetPropertyString(propID, value.Trim());
                                    break;
                                case PropertyType.pt_Boolean:
                                    bool val;
                                    if (bool.TryParse(value.Trim(), out val))
                                        publishedObject.SetPropertyBool(propID, val);
                                    break;
                                case PropertyType.pt_Material:
                                    int face = 0;
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        if (takeItem.Project.GetMaterialByName(value, out xpMaterial material))
                                            publishedObject.SetPropertyMaterial(propID, face, material);
                                    }
                                    break;
                            }
                        }
                    }
                    takeItem.UpdateThumbnail();
                }
            }
            catch { };
        }

    }
}

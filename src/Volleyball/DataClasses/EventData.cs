using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SportsController.Volleyball
{
    [DefaultProperty("Event")]
    public class EventData
    {
        #region Variables
        // Event Variables
        string eventTitle = "OCAA Volleyball";
        string eventMessage = "Knights Volleyball";
        string eventVenue = "Niagara College";
        string eventLocation = "Welland, ON";
        // Home Team Variables
        string homeSchool = "Niagara";
        string homeAbbr = "NIA";
        string homeName = "Knights";
        string homeLocation = "Welland, ON";
        // Away Team Variables
        string awaySchool = "Niagara";
        string awayAbbr = "NIA";
        string awayName = "Knights";
        string awayLocation = "Welland, ON";

        #endregion

        [
            Category("Event"),
            DisplayName("Title"),
            DefaultValue("OCAA Volleyball"),
            Description("The event title/name."),
        ]
        public string EventTitle
        {
            get { return eventTitle; }
            set { eventTitle = value; }
        }

        [
            Category("Event"),
            DisplayName("Message"),
            DefaultValue("Knights Volleyball"),
            Description("The event message/welcomer."),
        ]
        public string EventMessage
        {
            get { return eventMessage; }
            set { eventMessage = value; }
        }

        [
            Category("Event"),
            DisplayName("Venue"),
            DefaultValue("Niagara College"),
            Description("The events venue name."),
        ]
        public string EventVenue
        {
            get { return eventVenue; }
            set { eventVenue = value; }
        }

        [
            Category("Event"),
            DisplayName("Location"),
            DefaultValue("Welland, ON"),
            Description("Where the event is."),
        ]
        public string EventLocation
        {
            get { return eventLocation; }
            set { eventLocation = value; }
        }

        [
            Category("Home Team"),
            DisplayName("School"),
            DefaultValue("Niagara"),
            Description("The home school full name.")
        ]
        public string HomeSchool
        {
            get { return this.homeSchool; }
            set { this.homeSchool = value; }
        }

        [
            Category("Home Team"),
            DisplayName("Abbreviation"),
            DefaultValue("NIA"),
            Description("The home school abbreviation (used for scorebug).")
        ]
        public string HomeAbbr
        {
            get { return this.homeAbbr; }
            set { this.homeAbbr = value; }
        }

        [
            Category("Home Team"),
            DisplayName("Name"),
            DefaultValue("Knights"),
            Description("The home school team name.")
        ]
        public string HomeName
        {
            get { return this.homeName; }
            set { this.homeName = value; }
        }

        [
            Category("Home Team"),
            DisplayName("Location"),
            DefaultValue("Welland, ON"),
            Description("The home team school location.")
        ]
        public string HomeLocation
        {
            get { return this.homeLocation; }
            set { this.homeLocation = value; }
        }

        [
            Category("Away Team"),
            DisplayName("School"),
            DefaultValue("Niagara"),
            Description("The away school full name.")
        ]
        public string AwaySchool
        {
            get { return this.awaySchool; }
            set { this.awaySchool = value; }
        }

        [
            Category("Away Team"),
            DisplayName("Abbreviation"),
            DefaultValue("NIA"),
            Description("The away school abbreviation (used for scorebug).")
        ]
        public string AwayAbbr
        {
            get { return this.awayAbbr; }
            set { this.awayAbbr = value; }
        }

        [
            Category("Away Team"),
            DisplayName("Name"),
            DefaultValue("Knights"),
            Description("The away school team name.")
        ]
        public string AwayName
        {
            get { return this.awayName; }
            set { this.awayName = value; }
        }

        [
            Category("Away Team"),
            DisplayName("Location"),
            DefaultValue("Welland, ON"),
            Description("The away team school location.")
        ]
        public string AwayLocation
        {
            get { return this.awayLocation; }
            set { this.awayLocation = value; }
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SportsController.Basketball
{
    [DefaultProperty("Event")]
    public class EventData
    {
        [
            Category("Event"),
            DisplayName("Title"),
            DefaultValue("OCAA Volleyball"),
            Description("The event title/name."),
        ]
        public string EventTitle { get; set; } = "OCAA Basketball";

        [
            Category("Event"),
            DisplayName("Message"),
            DefaultValue("Knights Volleyball"),
            Description("The event message/welcomer."),
        ]
        public string EventMessage { get; set; } = "Knights Basketball";

        [
            Category("Event"),
            DisplayName("Venue"),
            DefaultValue("Niagara College"),
            Description("The events venue name."),
        ]
        public string EventVenue { get; set; } = "Niagara College";

        [
            Category("Event"),
            DisplayName("Location"),
            DefaultValue("Welland, ON"),
            Description("Where the event is."),
        ]
        public string EventLocation { get; set; } = "Welland, ON";

        [
            Category("Home Team"),
            DisplayName("School"),
            DefaultValue("Niagara"),
            Description("The home school full name.")
        ]
        public string HomeSchool { get; set; } = "Niagara";

        [
            Category("Home Team"),
            DisplayName("Abbreviation"),
            DefaultValue("NIA"),
            Description("The home school abbreviation (used for scorebug).")
        ]
        public string HomeAbbr { get; set; } = "NIA";

        [
            Category("Home Team"),
            DisplayName("Name"),
            DefaultValue("Knights"),
            Description("The home school team name.")
        ]
        public string HomeName { get; set; } = "Knights";

        [
            Category("Home Team"),
            DisplayName("Location"),
            DefaultValue("Welland, ON"),
            Description("The home team school location.")
        ]
        public string HomeLocation { get; set; } = "Welland, ON";

        [
            Category("Away Team"),
            DisplayName("School"),
            DefaultValue("Niagara"),
            Description("The away school full name.")
        ]
        public string AwaySchool { get; set; } = "Niagara";

        [
            Category("Away Team"),
            DisplayName("Abbreviation"),
            DefaultValue("NIA"),
            Description("The away school abbreviation (used for scorebug).")
        ]
        public string AwayAbbr { get; set; } = "NIA";

        [
            Category("Away Team"),
            DisplayName("Name"),
            DefaultValue("Knights"),
            Description("The away school team name.")
        ]
        public string AwayName { get; set; } = "Knights";

        [
            Category("Away Team"),
            DisplayName("Location"),
            DefaultValue("Welland, ON"),
            Description("The away team school location.")
        ]
        public string AwayLocation { get; set; } = "Welland, ON";
    }
}

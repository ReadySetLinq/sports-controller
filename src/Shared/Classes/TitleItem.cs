namespace SportsController.Shared
{
    public class TitleItem
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public TitleItem(string index, string name, string value = "")
        {
            Index = index;
            Name = name;
            Value = value;
        }
    }
}

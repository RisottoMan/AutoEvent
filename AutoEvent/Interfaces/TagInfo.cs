using System.ComponentModel;

namespace AutoEvent.Interfaces
{
    public class TagInfo
    {
        public TagInfo() { }

        public TagInfo(string name, string color)
        {
            Name = name;
            Color = color;
        }
        [Description("Tag text")]
        public string Name { get; set; }

        [Description("Tag color")]
        public string Color { get; set; }
    }
}

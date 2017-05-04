using System.Collections.Generic;

namespace ImageProcessor.ViewModels
{
    public class Game
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public IEnumerable<Pack> Packs { get; set; }
    }

    public class Pack
    {
        public string Name { get; set; }
        public IEnumerable<Section> Sections { get; set; }
    }

    public class Section
    {
        public string Name { get; set; }
        public IEnumerable<Level> Levels { get; set; }
    }

    public class Level
    {
        public int Number { get; set; }
        public int? Flows { get; set; }
        public int? Bridges { get; set; }
    }
}

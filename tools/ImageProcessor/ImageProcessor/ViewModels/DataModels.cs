using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageProcessor.ViewModels
{
    public class Game
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("logo")]
        public string Logo { get; set; }
        [JsonProperty("packs")]
        public IEnumerable<Pack> Packs { get; set; }
    }

    public class Pack
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sections")]
        public IEnumerable<Section> Sections { get; set; }
    }

    public class Section
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("levels")]
        public IEnumerable<Level> Levels { get; set; }
    }

    public class Level
    {
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("flows")]
        public int? Flows { get; set; }
        [JsonProperty("bridges")]
        public int? Bridges { get; set; }
        [JsonProperty("isSolved")]
        public bool? IsSolved { get; set; }
        [JsonProperty("toCheck")]
        public bool? ToCheck { get; set; }
    }

    public struct Crop
    {
        public string Name { get; set; }
        public int Start { get; set; }
    }
}

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CodeTourVS
{
    public class Tour
    {
        [JsonIgnore()]
        public string File { get; set; }

        [JsonProperty("$schema")]
        public string Schema { get; set; } = "https://cdn.jsdelivr.net/gh/vsls-contrib/code-tour/schema.json";

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("steps")]
        public IEnumerable<Step> Steps { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; } = true;
    }

    public class Step
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("selection")]
        public Selection Selection { get; set; }

        [JsonProperty("directory")]
        public string Directory { get; set; }

        [JsonIgnore]
        public Tour Tour { get; set; }

        [JsonIgnore]
        public string AbsoluteFile
        {
            get
            {
                // TODO: Support directory as well
                if (!string.IsNullOrEmpty(File))
                {
                    var folder = Path.GetDirectoryName(Tour.File);
                    var absolute = Path.Combine(folder, "..\\", File).Replace("/", "\\");
                    var file = new FileInfo(absolute);

                    return file.FullName;
                }

                return Path.Combine(Path.GetTempPath(), Title ?? Tour.Title ?? "Code Tour").Replace("/", "\\");
            }
        }
    }

    public class Selection
    {
        [JsonProperty("start")]
        public Start Start { get; set; }

        [JsonProperty("end")]
        public End End { get; set; }
    }

    public class Start
    {
        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("character")]
        public int Character { get; set; }
    }

    public class End
    {
        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("character")]
        public int Character { get; set; }
    }

}
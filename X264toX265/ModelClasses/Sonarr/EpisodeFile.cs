using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Sonarr
{
    class EpisodeFile
    {
        [JsonProperty(PropertyName = "seriesId")]
        public int SeriesID { get; private set; }
        [JsonProperty(PropertyName = "id")]
        public int ID { get; private set; }
        [JsonProperty(PropertyName = "seasonNumber")]
        public int SeasonNumber { get; private set; }
        [JsonProperty(PropertyName = "path")]
        public string Path { get; private set; }
        [JsonProperty(PropertyName = "relativePath")]
        public string RelativePath { get; private set; }
        [JsonProperty(PropertyName = "size")]
        public long Size { get; private set; }
        [JsonProperty(PropertyName = "quality")]
        public Quality Quality { get; private set; }
        [JsonProperty(PropertyName = "mediaInfo")]
        public MediaInfo MediaInfo { get; private set; }
        public bool ConversionRequired { get; set; } = false;
    }
}

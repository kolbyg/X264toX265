using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Sonarr
{
    class Series
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; private set; }
        [JsonProperty(PropertyName = "episodeCount")]
        public int EpisodeCount { get; private set; }
        [JsonProperty(PropertyName = "seasonCount")]
        public int SeasonCount { get; private set; }
        [JsonProperty(PropertyName = "episodeFileCount")]
        public int EpisodeFileCount { get; private set; }
        [JsonProperty(PropertyName = "sizeOnDisk")]
        public long SizeOnDisk { get; private set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; private set; }
        [JsonProperty(PropertyName = "seasons")]
        public Season[] Seasons { get; private set; }
        [JsonProperty(PropertyName = "path")]
        public string Path { get; private set; }
        [JsonProperty(PropertyName = "imdbId")]
        public string IMDBID { get; private set; }
        [JsonProperty(PropertyName = "cleanTitle")]
        public string CleanTitle { get; private set; }
        [JsonProperty(PropertyName = "qualityProfileId")]
        public int QualityProfileID { get; private set; }
        [JsonProperty(PropertyName = "id")]
        public int ID { get; private set; }
        public List<EpisodeFile> Episodes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Sonarr
{
    class SeasonStatistics
    {
        [JsonProperty(PropertyName = "episodeFileCount")]
        public int EpisodeFileCount { get; private set; }
        [JsonProperty(PropertyName = "episodeCount")]
        public int EpisodeCount { get; private set; }
        [JsonProperty(PropertyName = "totalEpisodeCount")]
        public int TotalEpisodeCount { get; private set; }
        [JsonProperty(PropertyName = "sizeOnDisk")]
        public long SizeOnDisk { get; private set; }
    }
}

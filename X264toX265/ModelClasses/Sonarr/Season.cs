using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Sonarr
{
    class Season
    {
        [JsonProperty(PropertyName = "seasonNumber")]
        public int SeasonNumber { get; private set; }
        [JsonProperty(PropertyName = "monitored")]
        public bool Monitored { get; private set; }
        [JsonProperty(PropertyName = "statistics")]
        public SeasonStatistics SeasonStatistics { get; private set; }
    }
}

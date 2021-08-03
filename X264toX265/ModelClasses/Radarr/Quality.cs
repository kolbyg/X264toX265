using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Radarr
{
    class Quality
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "source")]
        public string Source { get; private set; }
        [JsonProperty(PropertyName = "resolution")]
        public string Resolution { get; private set; }
        [JsonProperty(PropertyName = "modifier")]
        public string Modifier { get; private set; }
    }
}

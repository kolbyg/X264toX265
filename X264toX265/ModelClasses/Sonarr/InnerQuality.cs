using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Sonarr
{
    class InnerQuality
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "source")]
        public string Source { get; private set; }
        [JsonProperty(PropertyName = "reoslution")]
        public int Resolution { get; private set; }

    }
}

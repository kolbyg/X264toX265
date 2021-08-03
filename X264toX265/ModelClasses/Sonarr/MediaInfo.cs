using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Sonarr
{
    class MediaInfo
    {
        [JsonProperty(PropertyName = "audioCodec")]
        public string AudioCodec { get; private set; }
        [JsonProperty(PropertyName = "videoCodec")]
        public string VideoCodec { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class MovieFile
    {
        [JsonProperty(PropertyName = "movieID")]
        public int MovieID { get; private set; }
        [JsonProperty(PropertyName = "size")]
        public long Size { get; private set; }
        [JsonProperty(PropertyName = "quality")]
        public QualityProfile QualityProfile { get; private set; }
        [JsonProperty(PropertyName = "mediaInfo")]
        public MediaInfo MediaInfo { get; private set; }

    }
}

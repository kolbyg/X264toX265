using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class MovieFile
    {
        [JsonProperty(PropertyName = "movieID")]
        int MovieID;
        [JsonProperty(PropertyName = "size")]
        long Size;
        [JsonProperty(PropertyName = "quality")]
        QualityProfile QualityProfile;
        [JsonProperty(PropertyName = "mediaInfo")]
        MediaInfo MediaInfo;

    }
}

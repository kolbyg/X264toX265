using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class Quality
    {
        [JsonProperty(PropertyName = "id")]
        int ID;
        [JsonProperty(PropertyName = "name")]
        string Name;
        [JsonProperty(PropertyName = "source")]
        string Source;
        [JsonProperty(PropertyName = "resolution")]
        string Resolution;
        [JsonProperty(PropertyName = "modifier")]
        string Modifier;
    }
}

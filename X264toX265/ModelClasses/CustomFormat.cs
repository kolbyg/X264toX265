using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class CustomFormat
    {
        [JsonProperty(PropertyName = "name")]
        string Name;
        [JsonProperty(PropertyName = "id")]
        int ID;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class QualityProfile
    {
        [JsonProperty(PropertyName = "quality")]
        Quality Quality;
        [JsonProperty(PropertyName = "customFormats")]
        List<CustomFormat> CustomFormats;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class QualityProfile
    {
        [JsonProperty(PropertyName = "quality")]
        public Quality Quality { get; private set; }
        //[JsonProperty(PropertyName = "customFormats")]
        //public List<CustomFormat> CustomFormats { get; private set; }
    }
}

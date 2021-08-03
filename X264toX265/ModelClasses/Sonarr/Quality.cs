using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses.Sonarr
{
    class Quality
    {
        [JsonProperty(PropertyName = "quality")]
        public InnerQuality OuterQuality { get; private set; }
    }
}

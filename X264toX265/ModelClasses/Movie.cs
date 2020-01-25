using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class Movie
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; private set; }
        [JsonProperty(PropertyName = "cleanTitle")]
        public string CleanTitle { get; private set; }
        [JsonProperty(PropertyName = "sizeOnDisk")]
        public long SizeOnDisk { get; private set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; private set; }
        [JsonProperty(PropertyName = "downloaded")]
        public bool Downloaded { get; private set; }
        [JsonProperty(PropertyName = "hasFile")]
        public bool HasFile { get; private set; }
        [JsonProperty(PropertyName = "path")]
        public string Path { get; private set; }
        [JsonProperty(PropertyName = "imdbId")]
        public string IMDBID { get; private set; }
        [JsonProperty(PropertyName = "movieFile")]
        public MovieFile MovieFiles { get; private set; }
        [JsonProperty(PropertyName = "qualityProfileId")]
        public int QualityProfileID { get; private set; }
        [JsonProperty(PropertyName = "id")]
        public int ID { get; private set; }
        public bool ConversionRequired { get; set; } = false;
    }
}

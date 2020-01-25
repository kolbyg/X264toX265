using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class Movie
    {
        [JsonProperty(PropertyName = "title")]
        string Title;
        [JsonProperty(PropertyName = "cleanTitle")]
        string CleanTitle;
        [JsonProperty(PropertyName = "sizeOnDisk")]
        long SizeOnDisk;
        [JsonProperty(PropertyName = "status")]
        string Status;
        [JsonProperty(PropertyName = "downloaded")]
        bool Downloaded;
        [JsonProperty(PropertyName = "hasFile")]
        bool HasFile;
        [JsonProperty(PropertyName = "path")]
        string Path;
        [JsonProperty(PropertyName = "imdbId")]
        string IMDBID;
        [JsonProperty(PropertyName = "movieFile")]
        MovieFile MovieFiles;
        [JsonProperty(PropertyName = "qualityProfileId")]
        int QualityProfileID;
        [JsonProperty(PropertyName = "id")]
        int ID;

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class MediaInfo
    {
        [JsonProperty(PropertyName = "containerFormat")]
        string ContainerFormat;
        [JsonProperty(PropertyName = "videoFormat")]
        string VideoFormat;
        [JsonProperty(PropertyName = "videoCodecID")]
        string VideoCodecID;
        [JsonProperty(PropertyName = "videoProfile")]
        string VideoProfile;
        [JsonProperty(PropertyName = "videoBitrate")]
        string VideoBitrate;
        [JsonProperty(PropertyName = "videoBitdepth")]
        string VideoBitdepth;
        [JsonProperty(PropertyName = "width")]
        string Width;
        [JsonProperty(PropertyName = "height")]
        string Height;
        [JsonProperty(PropertyName = "audioFormat")]
        string AudioFormat;
        [JsonProperty(PropertyName = "audioCodecID")]
        string AudioCodecID;
        [JsonProperty(PropertyName = "audioBitrate")]
        string AudioBitrate;
        [JsonProperty(PropertyName = "videoFps")]
        string VideoFPS;
    }
}

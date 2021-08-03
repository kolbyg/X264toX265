using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class MediaInfo
    {
        [JsonProperty(PropertyName = "videoCodec")]
        public string VideoCodec { get; private set; }
        [JsonProperty(PropertyName = "videoBitrate")]
        public int VideoBitrate { get; private set; }
        [JsonProperty(PropertyName = "videoBitdepth")]
        public string VideoBitdepth { get; private set; }
        [JsonProperty(PropertyName = "width")]
        public string Width { get; private set; }
        [JsonProperty(PropertyName = "height")]
        public string Height { get; private set; }
        [JsonProperty(PropertyName = "resolution")]
        public string Resolution { get; private set; }
        [JsonProperty(PropertyName = "audioFormat")]
        public string AudioFormat { get; private set; }
        [JsonProperty(PropertyName = "audioCodec")]
        public string AudioCodec { get; private set; }
        [JsonProperty(PropertyName = "audioBitrate")]
        public string AudioBitrate { get; private set; }
        [JsonProperty(PropertyName = "videoFps")]
        public string VideoFPS { get; private set; }
    }
}

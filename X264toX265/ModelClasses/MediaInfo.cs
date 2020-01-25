using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace X264toX265.ModelClasses
{
    class MediaInfo
    {
        [JsonProperty(PropertyName = "containerFormat")]
        public string ContainerFormat { get; private set; }
        [JsonProperty(PropertyName = "videoFormat")]
        public string VideoFormat { get; private set; }
        [JsonProperty(PropertyName = "videoCodecID")]
        public string VideoCodecID { get; private set; }
        [JsonProperty(PropertyName = "videoProfile")]
        public string VideoProfile { get; private set; }
        [JsonProperty(PropertyName = "videoBitrate")]
        public string VideoBitrate { get; private set; }
        [JsonProperty(PropertyName = "videoBitdepth")]
        public string VideoBitdepth { get; private set; }
        [JsonProperty(PropertyName = "width")]
        public string Width { get; private set; }
        [JsonProperty(PropertyName = "height")]
        public string Height { get; private set; }
        [JsonProperty(PropertyName = "audioFormat")]
        public string AudioFormat { get; private set; }
        [JsonProperty(PropertyName = "audioCodecID")]
        public string AudioCodecID { get; private set; }
        [JsonProperty(PropertyName = "audioBitrate")]
        public string AudioBitrate { get; private set; }
        [JsonProperty(PropertyName = "videoFps")]
        public string VideoFPS { get; private set; }
    }
}

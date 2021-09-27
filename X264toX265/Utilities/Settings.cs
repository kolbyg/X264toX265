using System;
using System.Collections.Generic;
using System.Text;

namespace X264toX265.Utilities
{
    class Settings
    {
        public string RadarrDBPath { get; set; } = "currently_unused";
        public string SonarrDBPath { get; set; } = "currently_unused";
        public string RadarrAPIKey { get; set; } = "INSERT_KEY";
        public string SonarrAPIKey { get; set; } = "INSERT_KEY";
        public string RadarrURL { get; set; } = "http://radarr-server.domain.com:7878";
        public string SonarrURL { get; set; } = "http://sonar-server.domain.com:8989";
        public string FFmpegLocation { get; set; } = "\\lib\\ffmpeg.exe";
        public string ConversionOutputDir { get; set; } = "\\output";
        public long MaxOutputDirSize { get; set; } = 100000000000;
        public int MaxUnattendedMovies { get; set; } = 10;
        public int MaxUnattendedEpisodes { get; set; } = 20;
        public FFmpegSettings FFmpegSettings { get; set; } = new FFmpegSettings();

    }
    class FFmpegSettings
    {
        public int MaxBitrate { get; set; } = 10000;
        public int CRF { get; set; } = 28;
        public int Preset { get; set; } = 5;
        public int PixelFormat { get; set; } = 0;
        public int Profile { get; set; } = 0;
        public int EncoderLibrary { get; set; } = 2;
        public int AudioFormat { get; set; } = 0;
    }
}

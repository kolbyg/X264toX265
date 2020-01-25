using System;
using System.Collections.Generic;
using System.Text;

namespace X264toX265.MediaOperations
{
    public class ConversionOptions
    {
        public readonly string[] PresetNames = {"ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo"};
        public readonly string[] PixelFormats = { "yuv420p" };
        public readonly string[] Profiles = { "main", "main10" };
        public readonly string[] EncoderLibraries = { "hevc_nvenc", "libx265" };
        public readonly string[] AudioFormats = { "copy" };
        public int MaxBitrate = 10000;
        public int CRF = 28;
        public int Preset = 5;
        public int PixelFormat = 0;
        public int Profile = 0;
        public int EncoderLibrary = 0;
        public int AudioFormat = 0;
    }
    class ConvertFile
    {
        public int InvokeFFmpeg(string SourcePath, string DestinationPath, ConversionOptions Options)
        {
            string _ffmpegCommandBase = $"-i {SourcePath} -c:v {Options.EncoderLibraries[Options.EncoderLibrary]} -crf{Options.CRF} -profile:v {Options.Profiles[Options.Profile]} -pixel_format {Options.PixelFormats[Options.PixelFormat]} -preset {Options.PresetNames[Options.Preset]} -c:a {Options.AudioFormats[Options.AudioFormat]} {DestinationPath}";
            return 0;
        }
    }
}

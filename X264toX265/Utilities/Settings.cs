using System;
using System.Collections.Generic;
using System.Text;

namespace X264toX265.Utilities
{
    class Settings
    {
        public string FFmpegLocation { get; set; } = "\\lib\\ffmpeg.exe";
        public string ConversionOutputDir { get; set; } = "\\output";
        public long MaxOutputDirSize { get; set; } = 100000000000;
        public int MaxUnattendedMovies { get; set; } = 10;
        public int MaxUnattendedEpisodes { get; set; } = 20;
        public API API { get; set; } = new API();
        public Transcoder Transcoder { get; set; } = new Transcoder();

    }
    public class API
    {
        public Radarr Radarr { get; set; } = new Radarr();
        public Sonarr Sonarr { get; set; } = new Sonarr();
    }
    public class Radarr
    {
        public string URL { get; set; } = "http://radarr-server.domain.com:7878";
        public string APIKey { get; set; } = "INSERT_KEY";
    }

    public class Sonarr
    {
        public string URL { get; set; } = "http://sonar-server.domain.com:8989";
        public string APIKey { get; set; } = "INSERT_KEY";
    }
    public class Transcoder
    {
        public EncoderLibrary EncoderLibrary { get; set; } = EncoderLibrary.hevc_amf;
        public AudioFormat AudioFormat { get; set; } = AudioFormat.copy;
        public int MaxBitrate { get; set; } = 10000;
        public NVENC NVENC { get; set; } = new NVENC();
        public AMF AMF { get; set; } = new AMF();
    }
    public enum AudioFormat
    {
        copy
    }
    public enum EncoderLibrary
    {
        hevc_nvenc, 
        libx265,
        hevc_amf
    }
    #region AMF
    public class AMF
    {
        public AMFProfile Profile { get; set; } = AMFProfile.main;
        public AMFQuality Quality { get; set; } = AMFQuality.balanced;
        public AMFRC RC { get; set; } = AMFRC.cqp;
        public int qp_p = 24;
        public int qp_i = 24;
    }
    public enum AMFProfile
    {
        main
    }
    public enum AMFQuality
    {
        balanced,
        speed,
        quality
    }
    public enum AMFRC
    {
        cqp,
        cbr
    }
    #endregion
    #region NVENC
    public class NVENC
    {
        public NVENCPreset Preset { get; set; } = NVENCPreset.p4;
        public NVENCProfile Profile { get; set; } = NVENCProfile.main;
        public NVENCRC RC { get; set; } = NVENCRC.constqp;
        public int cq { get; set; } = 24;
    }
    public enum NVENCRC
    {
        constqp,
        vbr,
        cbr
    }
    public enum NVENCProfile
    {
        main,
        main10,
        rnext
    }
    public enum NVENCPreset
    {
        slow,
        medium,
        fast,
        lossless,
        p1,
        p2,
        p3,
        p4,
        p5,
        p6,
        p7
    }
    #endregion
}

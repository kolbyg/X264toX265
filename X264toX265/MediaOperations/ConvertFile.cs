using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using NLog;
using System.Threading.Tasks;

namespace X264toX265.MediaOperations
{
    public class ConversionOptions
    {
        public readonly string[] PresetNames = {"ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo"};
        public readonly string[] PixelFormats = { "yuv420p" };
        public readonly string[] Profiles = { "main", "main10" };
        public readonly string[] EncoderLibraries = { "hevc_nvenc", "libx265", "hevc_amf" };
        public readonly string[] AudioFormats = { "copy" };
        public int MaxBitrate { get; set; }
        public int CRF { get; set; }
        public int Preset { get; set; }
        public int PixelFormat { get; set; }
        public int Profile { get; set; }
        public int EncoderLibrary { get; set; }
        public int AudioFormat { get; set; }
        public ConversionOptions()
        {
            MaxBitrate = Utilities.Utilities.CurrentSettings.FFmpegSettings.MaxBitrate;
            CRF = Utilities.Utilities.CurrentSettings.FFmpegSettings.CRF;
            Preset = Utilities.Utilities.CurrentSettings.FFmpegSettings.Preset;
            PixelFormat = Utilities.Utilities.CurrentSettings.FFmpegSettings.PixelFormat;
            Profile = Utilities.Utilities.CurrentSettings.FFmpegSettings.Profile;
            EncoderLibrary = Utilities.Utilities.CurrentSettings.FFmpegSettings.EncoderLibrary;
            AudioFormat = Utilities.Utilities.CurrentSettings.FFmpegSettings.AudioFormat;
        }
    }
    class ConvertFile
    {
        private static int InvokeFFmpeg(string SourcePath, string DestinationPath, ConversionOptions Options)
        {
            Utilities.Utilities.Logger.Debug("Preparing FFmpeg...");
            string _ffmpegCommandBase = $"-i \"{SourcePath}\" -c:v {Options.EncoderLibraries[Options.EncoderLibrary]} -crf {Options.CRF} -profile:v {Options.Profiles[Options.Profile]} -pixel_format {Options.PixelFormats[Options.PixelFormat]} -preset {Options.PresetNames[Options.Preset]} -c:a {Options.AudioFormats[Options.AudioFormat]} \"{DestinationPath}\"";
            Utilities.Utilities.Logger.Debug($"FFmpeg location is {Utilities.Utilities.CurrentSettings.FFmpegLocation}, converion string: {_ffmpegCommandBase}");
            if (!File.Exists(SourcePath))
            {
                throw new Exception($"Source file passed to ffmpeg does not exist. {SourcePath}");
            }
            Process p = new Process();
            p.StartInfo.Arguments = _ffmpegCommandBase;
            p.StartInfo.WorkingDirectory = Utilities.Utilities.CurrentSettings.FFmpegLocation;
            p.StartInfo.FileName = $"{Utilities.Utilities.CurrentSettings.FFmpegLocation}\\ffmpeg.exe";
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.RedirectStandardOutput = false;
            Utilities.Utilities.Logger.Debug("Invoking FFmpeg executible...");
            p.Start();
            p.WaitForExit();

            return 0;
        }
        public static List<ModelClasses.Radarr.Movie> CreateConversionQueue(List<ModelClasses.Radarr.Movie> MovieList)
        {
            Utilities.Utilities.Logger.Info("Preparing movie conversion queue...");
            try
            {
                List<ModelClasses.Radarr.Movie> _ConversionQueue = new List<ModelClasses.Radarr.Movie>();
                foreach (ModelClasses.Radarr.Movie movie in MovieList)
                {
                    Utilities.Utilities.Logger.Debug("Now processing movie: " + movie.Title);
                    if (movie.ConversionRequired)
                    {
                        _ConversionQueue.Add(movie);
                        Utilities.Utilities.Logger.Debug("Added " + movie.Title + " to conversion queue. Current codec is " + movie.MovieFiles.MediaInfo.VideoCodec);
                    }
                }
                return _ConversionQueue;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static List<ModelClasses.Sonarr.EpisodeFile> CreateConversionQueue(List<ModelClasses.Sonarr.Series> SeriesList)
        {
            Utilities.Utilities.Logger.Info("Preparing episode conversion queue...");
            try
            {
                List<ModelClasses.Sonarr.EpisodeFile> _ConversionQueue = new List<ModelClasses.Sonarr.EpisodeFile>();
                foreach (ModelClasses.Sonarr.Series series in SeriesList)
                {
                    Utilities.Utilities.Logger.Info("Now processing series: " + series.Title);
                    foreach (ModelClasses.Sonarr.EpisodeFile episode in series.Episodes)
                    {
                        Utilities.Utilities.Logger.Info("Now processing episode ID: " + episode.ID);
                        if (episode.ConversionRequired)
                        {
                            _ConversionQueue.Add(episode);
                            Utilities.Utilities.Logger.Debug("Added episode ID " + episode.ID + " in " + series.Title + " to conversion queue. Current codec is " + episode.MediaInfo.VideoCodec);
                        }
                    }
                }
                return _ConversionQueue;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static bool ConvertFiles(List<ModelClasses.Radarr.Movie> ConversionList)
        {
            try
            {
                Utilities.Utilities.Logger.Debug($"Begining conversion of {ConversionList.Count} movies");
                foreach(ModelClasses.Radarr.Movie movie in ConversionList)
                {
                    int _SourceBitrate = movie.MovieFiles.MediaInfo.VideoBitrate;
                    long _SourceSize = movie.MovieFiles.Size;
                    int _CurrentConvertPass = 0;
                    Utilities.Utilities.Logger.Debug($"Current movie is {movie.CleanTitle}");
                    Utilities.Utilities.Logger.Debug($"Source bitrate is {_SourceBitrate}");
                    Utilities.Utilities.Logger.Debug($"Source filesize is {_SourceSize}");
                    //while (_CurrentConvertPass < 3)
                    //{ TODO add auto downgrading quality based on filesize
                        Utilities.Utilities.Logger.Debug("Creating Conversion Options");
                        ConversionOptions _Options = new ConversionOptions();
                        switch (_CurrentConvertPass)
                        {
                            case 0:
                                break;
                            case 1:
                                Utilities.Utilities.Logger.Debug("Increasing CRF due to second pass");
                                _Options.CRF += 2;
                                break;
                            case 2:
                                Utilities.Utilities.Logger.Debug("Increasing CRF due to third pass");
                                _Options.CRF += 4;
                                break;
                        }
                        InvokeFFmpeg($"{movie.Path}\\{movie.MovieFiles.RelativePath}", $"{Utilities.Utilities.CurrentSettings.ConversionOutputDir}\\{movie.Title}-CONVERTED.mkv", _Options);
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                Utilities.Utilities.Logger.Debug(ex.StackTrace);
                return false;
            }
        }
        public static bool ConvertFiles(List<ModelClasses.Sonarr.EpisodeFile> ConversionList)
        {
            try
            {
                Utilities.Utilities.Logger.Debug($"Begining conversion of {ConversionList.Count} episodes");
                foreach (ModelClasses.Sonarr.EpisodeFile episode in ConversionList)
                {
                    //int _SourceBitrate = episode.MovieFiles.MediaInfo.VideoBitrate;
                    long _SourceSize = episode.Size;
                    int _CurrentConvertPass = 0;
                    //logger.Debug($"Current series is {.CleanTitle}");//todo, LINQ to lookup series
                    Utilities.Utilities.Logger.Debug($"Current episode ID is {episode.ID}");
                    //logger.Debug($"Source bitrate is {_SourceBitrate}");
                    Utilities.Utilities.Logger.Debug($"Source filesize is {_SourceSize}");
                    //while (_CurrentConvertPass < 3)
                    //{ TODO add auto downgrading quality based on filesize
                        Utilities.Utilities.Logger.Debug("Creating Conversion Options");
                        ConversionOptions _Options = new ConversionOptions();
                        switch (_CurrentConvertPass)
                        {
                            case 0:
                                break;
                            case 1:
                                Utilities.Utilities.Logger.Debug("Increasing CRF due to second pass");
                                _Options.CRF += 2;
                                break;
                            case 2:
                                Utilities.Utilities.Logger.Debug("Increasing CRF due to third pass");
                                _Options.CRF += 4;
                                break;
                        }
                        InvokeFFmpeg($"{episode.Path}", $"{Utilities.Utilities.CurrentSettings.ConversionOutputDir}\\{episode.RelativePath}-CONVERTED.mkv", _Options);
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                Utilities.Utilities.Logger.Debug(ex.StackTrace);
                return false;
            }
        }
    }
}

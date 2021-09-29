using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
        static Logger logger = LogManager.GetCurrentClassLogger();
        private static string GetFFmpegBaseCommand(string SourcePath, string DestinationPath, ConversionOptions Options)
        {
            logger.Debug("Creating FFmpeg command string");
            string _ffmpegCommandBase = "";
            _ffmpegCommandBase += $"-i \"{SourcePath}\" ";
            switch (Options.EncoderLibrary)
            {
                case 0:
                    logger.Debug("Using NVENC");
                    _ffmpegCommandBase += $"-c:v {Options.EncoderLibraries[Options.EncoderLibrary]} -profile:v {Options.Profiles[Options.Profile]} -rc constqp -qp {Options.CRF} ";
                    break;
                case 1:
                    logger.Error("Encoder not yet implemented.");
                    Environment.Exit(0);
                    break;
                case 2:
                    logger.Debug("Using AMF");
                    _ffmpegCommandBase += $"-c:v {Options.EncoderLibraries[Options.EncoderLibrary]} -profile:v {Options.Profiles[Options.Profile]} -rc 0 -quality 5 -qp_p {Options.CRF} -qp_i {Options.CRF} ";
                    break;
            }
            _ffmpegCommandBase += $"-c:a {Options.AudioFormats[Options.AudioFormat]} ";
            _ffmpegCommandBase += $"\"{DestinationPath}\"";
            logger.Debug("FFmpeg command is: " + _ffmpegCommandBase);
            return _ffmpegCommandBase;
        }
        private static int InvokeFFmpeg(string SourcePath, string DestinationPath, ConversionOptions Options)
        {
            logger.Debug("Preparing FFmpeg...");

            string _ffmpegCommandBase = GetFFmpegBaseCommand(SourcePath, DestinationPath, Options);//$"-i \"{SourcePath}\" -c:v {Options.EncoderLibraries[Options.EncoderLibrary]} -crf {Options.CRF} -profile:v {Options.Profiles[Options.Profile]} -pixel_format {Options.PixelFormats[Options.PixelFormat]} -preset {Options.PresetNames[Options.Preset]} -c:a {Options.AudioFormats[Options.AudioFormat]} \"{DestinationPath}\"";
            logger.Debug($"FFmpeg location is {Utilities.Utilities.CurrentSettings.FFmpegLocation}, converion string: {_ffmpegCommandBase}");
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
            logger.Debug("Invoking FFmpeg executible...");
            p.Start();
            p.WaitForExit();

            return 0;
        }
        public static List<ModelClasses.Radarr.Movie> CreateConversionQueue(List<ModelClasses.Radarr.Movie> MovieList)
        {
            logger.Info("Preparing movie conversion queue...");
            try
            {
                List<ModelClasses.Radarr.Movie> _ConversionQueue = new List<ModelClasses.Radarr.Movie>();
                foreach (ModelClasses.Radarr.Movie movie in MovieList)
                {
                    logger.Debug("Now processing movie: " + movie.Title);
                    if (movie.ConversionRequired)
                    {
                        _ConversionQueue.Add(movie);
                        logger.Debug("Added " + movie.Title + " to conversion queue. Current codec is " + movie.MovieFiles.MediaInfo.VideoCodec);
                    }
                }
                return _ConversionQueue;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static List<ModelClasses.Sonarr.EpisodeFile> CreateConversionQueue(List<ModelClasses.Sonarr.Series> SeriesList)
        {
            logger.Info("Preparing episode conversion queue...");
            try
            {
                List<ModelClasses.Sonarr.EpisodeFile> _ConversionQueue = new List<ModelClasses.Sonarr.EpisodeFile>();
                foreach (ModelClasses.Sonarr.Series series in SeriesList)
                {
                    logger.Debug("Now processing series: " + series.Title);
                    foreach (ModelClasses.Sonarr.EpisodeFile episode in series.Episodes)
                    {
                        logger.Debug("Now processing episode ID: " + episode.ID);
                        if (episode.ConversionRequired)
                        {
                            _ConversionQueue.Add(episode);
                            logger.Debug("Added episode ID " + episode.ID + " in " + series.Title + " to conversion queue. Current codec is " + episode.MediaInfo.VideoCodec);
                        }
                    }
                }
                return _ConversionQueue;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static bool CheckConversionResults(long SourceFilesize, string DestinationFile)
        {
            ///This doesnt work right now and I dont know why. All the logic below is fine, but the CRF that is passed to ffmpeg doesnt actually affect the filesize, so its pointless. I'll fix it one day.
            //I was dumb and used the wrong ffmpeg commands, this appears to work now
            
            long DestinationFilesize;
            FileInfo fi = new FileInfo(DestinationFile);
            DestinationFilesize = fi.Length;
            logger.Debug("Source file is " + SourceFilesize + " bytes");
            logger.Debug("Destination file is " + DestinationFilesize + " bytes");

            //Bunch-o-if's, This is probably horrible, I don't care
            if (DestinationFilesize <= 50000000) //make sure its not tiny AF (probably would be an error)
            {
                logger.Debug("Failing due to tiny file");
                return false;
            }
            if (DestinationFilesize > SourceFilesize) //make sure its not larger than the source
            {
                logger.Debug("Failing due to larger than source");
                return false;
            }
            if ((DestinationFilesize * 1.3) > SourceFilesize) //make sure its more than slightly smaller
            {
                logger.Debug("Failing due to not smaller enough");
                return false;
            }
            
            logger.Debug("Transcode succeeded.");
            return true; //if none of the above apply, return true.
        }
        public static bool CheckConversionDirSize()
        {
            DirectoryInfo di = new DirectoryInfo(Utilities.Utilities.CurrentSettings.ConversionOutputDir);
            long size = 0;
            foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            logger.Info("Output dir size is currently " + size);
            if (size >= Utilities.Utilities.CurrentSettings.MaxOutputDirSize)
                return true;
            else return false;

        }
        private static void CreateDirStructure()
        {
            if (!Directory.Exists(Utilities.Utilities.CurrentSettings.ConversionOutputDir)) Directory.CreateDirectory(Utilities.Utilities.CurrentSettings.ConversionOutputDir);
            string movies = Utilities.Utilities.CurrentSettings.ConversionOutputDir + "\\movies";
            if (!Directory.Exists(movies)) Directory.CreateDirectory(movies);
            string shows = Utilities.Utilities.CurrentSettings.ConversionOutputDir + "\\shows";
            if (!Directory.Exists(shows)) Directory.CreateDirectory(shows);
        }
        public static bool ConvertFiles(List<ModelClasses.Radarr.Movie> ConversionList)
        {
            try
            {
                CreateDirStructure();
                logger.Info($"Begining conversion of {ConversionList.Count} movies");
                foreach(ModelClasses.Radarr.Movie movie in ConversionList)
                {
                    int _SourceBitrate = movie.MovieFiles.MediaInfo.VideoBitrate;
                    long _SourceSize = movie.MovieFiles.Size;
                    int _CurrentConvertPass = 0;
                    logger.Debug($"Current movie is {movie.CleanTitle}");
                    logger.Debug($"Source bitrate is {_SourceBitrate}");
                    logger.Debug($"Source filesize is {_SourceSize}");
                    while (_CurrentConvertPass < 3)
                    { 
                        logger.Debug("Creating Conversion Options");
                        ConversionOptions _Options = new ConversionOptions();
                        switch (_CurrentConvertPass)
                        {
                            case 0:
                                break;
                            case 1:
                                logger.Debug("Increasing CRF due to second pass");
                                _Options.CRF += 4;
                                break;
                            case 2:
                                logger.Debug("Increasing CRF due to third pass");
                                _Options.CRF += 8;
                                break;
                        }
                        if (CheckConversionDirSize())
                        {
                            logger.Warn("Output directory full, exiting...");
                            return false;
                        }
                    string _SourcePath = $"{movie.Path}\\{movie.MovieFiles.RelativePath}";
                    string _DestinationPath = $"{Utilities.Utilities.CurrentSettings.ConversionOutputDir}\\{movie.MovieFiles.RelativePath.Remove(movie.MovieFiles.RelativePath.LastIndexOf('.'))}-CONVERTED.mkv";
                    InvokeFFmpeg(_SourcePath, _DestinationPath, _Options);
                        if (CheckConversionResults(_SourceSize, _DestinationPath))
                        {
                            logger.Debug("Conversion successful, breaking from loop.");
                            break;
                        }
                        else
                        {
                            logger.Warn("Conversion failed to produce a viable file, rerunning...");
                            File.Delete(_DestinationPath);
                            _CurrentConvertPass++;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                logger.Debug(ex.StackTrace);
                return false;
            }
        }
        public static bool ConvertFiles(List<ModelClasses.Sonarr.EpisodeFile> ConversionList)
        {
            try
            {
                CreateDirStructure();
                logger.Debug($"Begining conversion of {ConversionList.Count} episodes");
                foreach (ModelClasses.Sonarr.EpisodeFile episode in ConversionList)
                {
                    string seriesName = Utilities.Utilities.Series.Find(x => x.ID == episode.SeriesID).CleanTitle;
                    string seriesOutputDir = $"{Utilities.Utilities.CurrentSettings.ConversionOutputDir}\\shows\\{seriesName}";
                    if (!Directory.Exists(seriesOutputDir)) Directory.CreateDirectory(seriesOutputDir);
                    //int _SourceBitrate = episode.MovieFiles.MediaInfo.VideoBitrate;
                    long _SourceSize = episode.Size;
                    int _CurrentConvertPass = 0;
                    //logger.Debug($"Current series is {.CleanTitle}");//todo, LINQ to lookup series
                    logger.Debug($"Current episode ID is {episode.ID}");
                    //logger.Debug($"Source bitrate is {_SourceBitrate}");
                    logger.Debug($"Source filesize is {_SourceSize}");
                    while (_CurrentConvertPass < 3)
                    {
                        logger.Debug("Creating Conversion Options");
                        ConversionOptions _Options = new ConversionOptions();
                        switch (_CurrentConvertPass)
                        {
                            case 0:
                                break;
                            case 1:
                                logger.Debug("Increasing CRF due to second pass");
                                _Options.CRF += 4;
                                break;
                            case 2:
                                logger.Debug("Increasing CRF due to third pass");
                                _Options.CRF += 8;
                                break;
                        }
                        if (CheckConversionDirSize())
                        {
                            logger.Warn("Output directory full, exiting...");
                            return false;
                        }
                        string _SourcePath = $"{episode.Path}";
                        string _DestinationPath = $"{seriesOutputDir}\\{episode.RelativePath.Remove(episode.RelativePath.LastIndexOf('.')).Substring(episode.RelativePath.LastIndexOf('\\'))}-CONVERTED.mkv";
                        InvokeFFmpeg(_SourcePath, _DestinationPath, _Options);
                        if (CheckConversionResults(_SourceSize, _DestinationPath))
                        {
                            logger.Debug("Conversion successful, breaking from loop.");
                            break;
                        }
                        else
                        {
                            logger.Warn("Conversion failed to produce a viable file, rerunning...");
                            File.Delete(_DestinationPath);
                            _CurrentConvertPass++;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                logger.Debug(ex.StackTrace);
                return false;
            }
        }
    }
}

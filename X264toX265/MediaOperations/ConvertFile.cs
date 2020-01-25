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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static int InvokeFFmpeg(string SourcePath, string DestinationPath, ConversionOptions Options)
        {
            string _ffmpegCommandBase = $"-i \"{SourcePath}\" -c:v {Options.EncoderLibraries[Options.EncoderLibrary]} -crf {Options.CRF} -profile:v {Options.Profiles[Options.Profile]} -pixel_format {Options.PixelFormats[Options.PixelFormat]} -preset {Options.PresetNames[Options.Preset]} -c:a {Options.AudioFormats[Options.AudioFormat]} \"{DestinationPath}\"";
            logger.Debug($"FFmpeg location is {Utilities.Utilities.CurrentSettings.FFmpegLocation}, converion string: {_ffmpegCommandBase}");
            if(!File.Exists(SourcePath))
            {
                throw new Exception($"Source file passed to ffmpeg does not exist. {SourcePath}");
            }
            if (!Directory.Exists(DestinationPath.Remove(DestinationPath.LastIndexOf(@"\"))))
                Directory.CreateDirectory(DestinationPath);
            //await Task.Run(() =>
            //{
                Process p = new Process();
                p.StartInfo.Arguments = _ffmpegCommandBase;
                p.StartInfo.WorkingDirectory = Utilities.Utilities.CurrentSettings.FFmpegLocation;
                p.StartInfo.FileName = $"{Utilities.Utilities.CurrentSettings.FFmpegLocation}\\ffmpeg.exe";
            p.StartInfo.CreateNoWindow = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.Start();
                p.WaitForExit();

                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd(); //ffmpeg seems to toss everything into stderr for some reason

                logger.Debug(output);
                logger.Debug(error);
            //});


            return 0;
        }
        public static List<ModelClasses.Movie> CreateConversionQueue(List<ModelClasses.Movie> MovieList)
        {
            try
            {
                List<ModelClasses.Movie> _ConversionQueue = new List<ModelClasses.Movie>();
                foreach (ModelClasses.Movie movie in MovieList)
                {
                    if (movie.ConversionRequired)
                        _ConversionQueue.Add(movie);
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
        public static bool ConvertFiles(List<ModelClasses.Movie> ConversionList)
        {
            try
            {
                logger.Debug($"Begining conversion of {ConversionList.Count} movies");
                foreach(ModelClasses.Movie movie in ConversionList)
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
                                _Options.CRF += 2;
                                break;
                            case 2:
                                logger.Debug("Increasing CRF due to third pass");
                                _Options.CRF += 4;
                                break;
                        }
                        logger.Info("Invoking FFmpeg");
                        InvokeFFmpeg($"{movie.Path}\\{movie.MovieFiles.RelativePath}", $"{Utilities.Utilities.CurrentSettings.ConversionOutputDir}\\{movie.Title}-CONVERTED.mkv", _Options);
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

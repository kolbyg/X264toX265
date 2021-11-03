using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLog;
using System.Threading.Tasks;
using System.Diagnostics;

namespace X264toX265.Controllers
{
    class ConversionController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        string[] EpExclude;
        string[] MvExclude;
        long bytesSaved = 0;
        public ConversionController()
        {

        }
        public void BeginConversion(bool IsForced, bool ExportList)
        {
            logger.Debug("Beginning API calls...");
            logger.Debug("Loading all data into lists");
            logger.Debug("Being retrieving series list from Sonarr...");
            Globals.AllSeries = LoadSeries();
            logger.Debug("Being retrieving movie list from Radarr...");
            Globals.AllMovies = LoadMovies();
            logger.Debug("Loading exclusions");
            LoadMvExcludeList();
            LoadEpExcludeList();
            logger.Debug("Set conversion flags");
            logger.Info("Determining which, if any movies require conversion...");
            SetMovieConversionFlags(Globals.AllMovies);
            logger.Info("Determining which, if any episodes require conversion...");
            SetSeriesConversionFlags(Globals.AllSeries);
            logger.Debug("Creating Conversion Queues");
            List<ModelClasses.Radarr.Movie> MoviesToConvert = CreateConversionQueue(Globals.AllMovies);
            List<ModelClasses.Sonarr.EpisodeFile> EpisodesToConvert = CreateConversionQueue(Globals.AllSeries);
            logger.Info($"There are {MoviesToConvert.Count.ToString()} movies to convert");
            logger.Info($"There are {EpisodesToConvert.Count.ToString()} episodes to convert");

            if (ExportList)
            {
                logger.Info("List requested, dumping all media marked for conversion to log, conversion will not run");
                logger.Info("***BEGIN EXPORT***");
                logger.Info("***BEGIN SERIES***");
                foreach(ModelClasses.Radarr.Movie movie in MoviesToConvert)
                {
                    if(movie.ConversionRequired)
                    logger.Info($"TITLE: {movie.Title} - CURRENT CODEC: {movie.MovieFiles.MediaInfo.VideoCodec}");
                }
                logger.Info("***END SERIES***");
                logger.Info("***BEGIN MOVIES***");
                foreach(ModelClasses.Sonarr.EpisodeFile ep in EpisodesToConvert)
                {
                    string showName = Globals.AllSeries.Find(x => x.ID == ep.SeriesID).Title;
                    if (ep.ConversionRequired)
                        logger.Info($"SERIES: {showName} - SEASON: {ep.SeasonNumber} - EP ID: {ep.ID} - CURRENT CODEC: {ep.MediaInfo.VideoCodec}");
                }
                logger.Info("***END MOVIES***");
                logger.Info("***END EXPORT***");
                Environment.Exit(0);
            }

            if (MoviesToConvert.Count < Globals.Settings.MaxUnattendedMovies || IsForced)
            {
                logger.Info("Beginning movie conversion...");
                ConvertFiles(MoviesToConvert);
            }
            else
                logger.Warn("Too many movies in the queue, increase MaxUnattendedMovies or run with --Force. Movie processing will be skipped.");

            if (EpisodesToConvert.Count < Globals.Settings.MaxUnattendedEpisodes || IsForced)
            {
                logger.Info($"Beginning episode conversion...");
                ConvertFiles(EpisodesToConvert);
            }
            else
            {
                logger.Warn("Too many episodes in the queue, increase MaxUnattendeEpisodes or run with --Force. Episode processing will be skipped.");
            }

        }
        private void SetMovieConversionFlags(List<ModelClasses.Radarr.Movie> MovieList)
        {
            try
            {
                foreach (ModelClasses.Radarr.Movie movie in MovieList)
                {
                    logger.Debug("Now Processing: " + movie.Title);
                    if (!movie.HasFile)
                    {
                        logger.Debug("Movie has no downloaded files, processing skipped");
                        continue; //movie doesnt actually exist yet
                    }
                    if (CheckMVExcludeList(movie.IMDBID, movie.ID))
                    {
                        logger.Info("Movie is on exclude list, skipping...");
                        continue;
                    }
                    int _MediaInfoCodec = ModelClasses.CodecTypes.GetCodecID(movie.MovieFiles.MediaInfo.VideoCodec);
                    logger.Debug("MediaInfo Codec: " + ModelClasses.CodecTypes.CodecNames[_MediaInfoCodec]);

                    if (_MediaInfoCodec > 0)
                    {
                        logger.Debug("Conversion IS required");
                        logger.Info($"Marking \"{movie.Title}\" as requiring conversion");
                        movie.ConversionRequired = true; //Mediainfo reports the file as not HEVC, conversion will be required.
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
            }
        }
        private void SetSeriesConversionFlags(List<ModelClasses.Sonarr.Series> SeriesList)
        {
            try
            {
                foreach (ModelClasses.Sonarr.Series series in SeriesList)
                {
                    logger.Debug("Now Processing Series: " + series.Title);
                    /**if (!series.HasFile)
                    {
                        logger.Debug("Movie has no downloaded files, processing skipped");
                        continue; //movie doesnt actually exist yet
                    }*/
                    foreach (ModelClasses.Sonarr.EpisodeFile episode in series.Episodes)
                    {
                        logger.Debug("Now Processing Episode ID: " + episode.ID);
                        int _MediaInfoCodec = ModelClasses.CodecTypes.GetCodecID(episode.MediaInfo.VideoCodec);
                        logger.Debug("MediaInfo Codec: " + ModelClasses.CodecTypes.CodecNames[_MediaInfoCodec]);

                        if (CheckEPExcludeList(episode.ID))
                        {
                            logger.Info("Episode is on exclude list, skipping...");
                            continue;
                        }
                        if (_MediaInfoCodec > 0)
                        {
                            logger.Debug("Conversion IS required");
                            logger.Info($"Marking episode ID {episode.ID} in \"{series.Title}\" as requiring conversion");
                            episode.ConversionRequired = true; //Mediainfo reports the file as not HEVC, conversion will be required.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
            }
        }
        private void LoadEpExcludeList()
        {
            try
            {
                EpExclude = File.ReadAllLines(Environment.CurrentDirectory + "\\ep_exclude.txt");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        private void LoadMvExcludeList()
        {
            try
            {
                MvExclude = File.ReadAllLines(Environment.CurrentDirectory + "\\mv_exclude.txt");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        private bool CheckEPExcludeList(int EpisodeID)
        {
            if (EpExclude == null || EpExclude.Length == 0) return false;
            foreach (string i in EpExclude)
            {
                if (EpisodeID.ToString() == i)
                {
                    return true;

                }
            }
            return false;
        }
        private bool CheckMVExcludeList(string IMDbID, int MovieID)
        {
            if (MvExclude == null || MvExclude.Length == 0) return false;
            foreach (string i in MvExclude)
            {
                if (MovieID.ToString() == i)
                {
                    return true;
                }
                if(IMDbID == i)
                {
                    return true;
                }
            }
            return false;
        }
        private List<ModelClasses.Radarr.Movie> LoadMovies()
        {
            try
            {
                logger.Debug("Retrieving movies json output from the Radarr API");
                string _radarrJsonMovies = Net.APIController.RetrieveMovies(Globals.Settings.API.Radarr.URL, Globals.Settings.API.Radarr.APIKey);
                logger.Debug("Parsing Movie JSON and loading it into a list");
                return File_Operations.Json.ParseMovies(_radarrJsonMovies);
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return null;
            }
        }
        private List<ModelClasses.Sonarr.Series> LoadSeries()
        {
            try
            {
                logger.Debug("Retrieving series json output from the Sonarr API");
                string _sonarrJsonSeries = Net.APIController.RetrieveSeries(Globals.Settings.API.Sonarr.URL, Globals.Settings.API.Sonarr.APIKey);
                logger.Debug("Parsing Series JSON");
                List<ModelClasses.Sonarr.Series> AllShows = File_Operations.Json.ParseSeries(_sonarrJsonSeries);
                logger.Debug("Retrieving episode lists");
                foreach(ModelClasses.Sonarr.Series series in AllShows)
                {
                    logger.Debug("Processing " + series.Title);
                    string _sonarrJsonEpisode = Net.APIController.RetrieveEpisodes(Globals.Settings.API.Sonarr.URL, Globals.Settings.API.Sonarr.APIKey, series.ID);
                    logger.Debug("Parsing Episode JSON");
                    series.Episodes = File_Operations.Json.ParseEpisode(_sonarrJsonEpisode);
                }
                return AllShows;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return null;
            }
        }
        private List<ModelClasses.Radarr.Movie> CreateConversionQueue(List<ModelClasses.Radarr.Movie> MovieList)
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
        private List<ModelClasses.Sonarr.EpisodeFile> CreateConversionQueue(List<ModelClasses.Sonarr.Series> SeriesList)
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

        private string GetFFmpegBaseCommand(string SourcePath, string DestinationPath, Transcoder TranscoderOptions)
        {
            logger.Debug("Creating FFmpeg command string");
            string _ffmpegCommandBase = "";
            _ffmpegCommandBase += $"-i \"{SourcePath}\" ";
            switch (TranscoderOptions.EncoderLibrary)
            {
                case EncoderLibrary.hevc_nvenc:
                    logger.Debug("Using NVENC");
                    _ffmpegCommandBase += $"-c:v {TranscoderOptions.EncoderLibrary} -profile:v {TranscoderOptions.NVENC.Profile} -rc:v {TranscoderOptions.NVENC.RC} -cq:v {TranscoderOptions.NVENC.cq} ";
                    break;
                case EncoderLibrary.libx265:
                    logger.Error("Encoder not yet implemented.");
                    Environment.Exit(0);
                    break;
                case EncoderLibrary.hevc_amf:
                    logger.Debug("Using AMF");
                    _ffmpegCommandBase += $"-c:v {TranscoderOptions.EncoderLibrary} -profile:v {TranscoderOptions.AMF.Profile} -rc:v {TranscoderOptions.AMF.RC} -quality:v {TranscoderOptions.AMF.Quality} -qp_p {TranscoderOptions.AMF.qp_p} -qp_i {TranscoderOptions.AMF.qp_i} ";
                    break;
            }
            _ffmpegCommandBase += $"-c:a {TranscoderOptions.AudioFormat} ";
            _ffmpegCommandBase += $"-c:s copy ";
            _ffmpegCommandBase += $"\"{DestinationPath}\"";
            logger.Debug("FFmpeg command is: " + _ffmpegCommandBase);
            return _ffmpegCommandBase;
        }
        private int InvokeFFmpeg(string SourcePath, string DestinationPath, Transcoder TranscoderOptions)
        {
            logger.Debug("Preparing FFmpeg...");
            CreateDirStructure();
            string _ffmpegCommandBase = GetFFmpegBaseCommand(SourcePath, DestinationPath, TranscoderOptions);//$"-i \"{SourcePath}\" -c:v {Options.EncoderLibraries[Options.EncoderLibrary]} -crf {Options.CRF} -profile:v {Options.Profiles[Options.Profile]} -pixel_format {Options.PixelFormats[Options.PixelFormat]} -preset {Options.PresetNames[Options.Preset]} -c:a {Options.AudioFormats[Options.AudioFormat]} \"{DestinationPath}\"";
            logger.Debug($"FFmpeg location is {Globals.Settings.FFmpegLocation}, converion string: {_ffmpegCommandBase}");
            if (!File.Exists(SourcePath))
            {
                throw new Exception($"Source file passed to ffmpeg does not exist. {SourcePath}");
            }
            Process p = new Process();
            p.StartInfo.Arguments = _ffmpegCommandBase;
            p.StartInfo.WorkingDirectory = Globals.Settings.FFmpegLocation;
            p.StartInfo.FileName = $"{Globals.Settings.FFmpegLocation}\\ffmpeg.exe";
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.RedirectStandardOutput = false;
            logger.Debug("Invoking FFmpeg executible...");
            p.Start();
            p.WaitForExit();

            return 0;
        }
        public bool CheckConversionResults(long SourceFilesize, string DestinationFile)
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
            bytesSaved += (SourceFilesize - DestinationFilesize);
            logger.Debug("Bytes saved (running total): " + bytesSaved);
            return true; //if none of the above apply, return true.
        }
        public bool CheckConversionDirSize()
        {
            DirectoryInfo di = new DirectoryInfo(Globals.Settings.ConversionOutputDir);
            long size = 0;
            foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            logger.Info("Output dir size is currently " + size);
            if (size >= Globals.Settings.MaxOutputDirSize)
                return true;
            else return false;

        }
        private void CreateDirStructure()
        {
            if (!Directory.Exists(Globals.Settings.ConversionOutputDir)) Directory.CreateDirectory(Globals.Settings.ConversionOutputDir);
            string movies = Globals.Settings.ConversionOutputDir + "\\movies";
            if (!Directory.Exists(movies)) Directory.CreateDirectory(movies);
            string shows = Globals.Settings.ConversionOutputDir + "\\shows";
            if (!Directory.Exists(shows)) Directory.CreateDirectory(shows);
        }
        private void DecreaseQuality(Transcoder Options, int Amount)
        {
            switch (Options.EncoderLibrary)
            {
                case EncoderLibrary.hevc_amf:
                    Options.AMF.qp_i += Amount;
                    Options.AMF.qp_p += Amount;
                    break;
                case EncoderLibrary.libx265:
                    break;
                case EncoderLibrary.hevc_nvenc:
                    Options.NVENC.cq += Amount;
                    break;
            }
        }



        public bool ConvertFiles(List<ModelClasses.Radarr.Movie> ConversionList)
        {
            try
            {
                logger.Info($"Begining conversion of {ConversionList.Count} movies");
                foreach (ModelClasses.Radarr.Movie movie in ConversionList)
                {
                    int _SourceBitrate = movie.MovieFiles.MediaInfo.VideoBitrate;
                    long _SourceSize = movie.MovieFiles.Size;
                    int _CurrentConvertPass = 0;
                    logger.Debug($"Current movie is {movie.CleanTitle}");
                    logger.Debug($"Source bitrate is {_SourceBitrate}");
                    logger.Debug($"Source filesize is {_SourceSize}");
                    Transcoder _Transcoder = (Transcoder)((ICloneable)Globals.Settings.Transcoder).Clone();
                    while (_CurrentConvertPass < 3)
                    {
                        logger.Debug("Creating Conversion Options");
                        switch (_CurrentConvertPass)
                        {
                            case 0:
                                break;
                            case 1:
                                logger.Debug("Increasing CRF due to second pass");
                                DecreaseQuality(_Transcoder, 4);
                                break;
                            case 2:
                                logger.Debug("Increasing CRF due to third pass");
                                DecreaseQuality(_Transcoder, 8);
                                break;
                        }
                        if (CheckConversionDirSize())
                        {
                            logger.Warn("Output directory full, exiting...");
                            return false;
                        }
                        string _SourcePath = $"{movie.Path}\\{movie.MovieFiles.RelativePath}";
                        string _DestinationPath = $"{Globals.Settings.ConversionOutputDir}\\movies\\{movie.MovieFiles.RelativePath.Remove(movie.MovieFiles.RelativePath.LastIndexOf('.'))}-CONVERTED.mkv";
                        InvokeFFmpeg(_SourcePath, _DestinationPath, _Transcoder);
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
        public bool ConvertFiles(List<ModelClasses.Sonarr.EpisodeFile> ConversionList)
        {
            try
            {
                CreateDirStructure();
                logger.Debug($"Begining conversion of {ConversionList.Count} episodes");
                foreach (ModelClasses.Sonarr.EpisodeFile episode in ConversionList)
                {
                    string seriesName = Globals.AllSeries.Find(x => x.ID == episode.SeriesID).CleanTitle;
                    string seriesOutputDir = $"{Globals.Settings.ConversionOutputDir}\\shows\\{seriesName}";
                    if (!Directory.Exists(seriesOutputDir)) Directory.CreateDirectory(seriesOutputDir);
                    //int _SourceBitrate = episode.MovieFiles.MediaInfo.VideoBitrate;
                    long _SourceSize = episode.Size;
                    int _CurrentConvertPass = 0;
                    //logger.Debug($"Current series is {.CleanTitle}");//todo, LINQ to lookup series
                    logger.Debug($"Current episode ID is {episode.ID}");
                    //logger.Debug($"Source bitrate is {_SourceBitrate}");
                    logger.Debug($"Source filesize is {_SourceSize}");
                    Transcoder _Transcoder = (Transcoder)((ICloneable)Globals.Settings.Transcoder).Clone();
                    while (_CurrentConvertPass < 3)
                    {
                        logger.Debug("Creating Conversion Options");
                        switch (_CurrentConvertPass)
                        {
                            case 0:
                                break;
                            case 1:
                                logger.Debug("Increasing CRF due to second pass");
                                DecreaseQuality(_Transcoder, 4);
                                break;
                            case 2:
                                logger.Debug("Increasing CRF due to third pass");
                                DecreaseQuality(_Transcoder, 8);
                                break;
                        }
                        if (CheckConversionDirSize())
                        {
                            logger.Warn("Output directory full, exiting...");
                            return false;
                        }
                        string _SourcePath = $"{episode.Path}";
                        string _DestinationPath = $"{seriesOutputDir}\\{episode.RelativePath.Remove(episode.RelativePath.LastIndexOf('.')).Substring(episode.RelativePath.LastIndexOf('\\'))}-CONVERTED.mkv";
                        InvokeFFmpeg(_SourcePath, _DestinationPath, _Transcoder);
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

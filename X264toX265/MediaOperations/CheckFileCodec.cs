using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace X264toX265.MediaOperations
{
    class CheckFileCodec
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Compares the mediainfo result of each media file to the codec in the qualityprofile. Sometimes Radarr/Sonarr are dumb and don't properly tag the media based on mediainfo
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void GetMovieConversionList(List<ModelClasses.Radarr.Movie> MovieList)
        {
            try
            {
                foreach(ModelClasses.Radarr.Movie movie in MovieList)
                {
                    logger.Debug("Now Processing: " + movie.Title);
                    if (!movie.HasFile) {
                        logger.Debug("Movie has no downloaded files, processing skipped");
                        continue; //movie doesnt actually exist yet
                    }

                    int _MediaInfoCodec = ModelClasses.CodecTypes.GetCodecID(movie.MovieFiles.MediaInfo.VideoCodec);
                    logger.Debug("MediaInfo Codec: " + ModelClasses.CodecTypes.CodecNames[_MediaInfoCodec]);

                    if (_MediaInfoCodec > 0) {
                        logger.Debug("Conversion IS required");
                        logger.Info($"Marking \"{movie.Title}\" as requiring conversion");
                        movie.ConversionRequired = true; //Mediainfo reports the file as not HEVC, conversion will be required.
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
            }
        }
        public static void GetEpisodeConversionList(List<ModelClasses.Sonarr.Series> SeriesList)
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
                    foreach(ModelClasses.Sonarr.EpisodeFile episode in series.Episodes)
                    {
                        logger.Debug("Now Processing Episode ID: " + episode.ID);
                        int _MediaInfoCodec = ModelClasses.CodecTypes.GetCodecID(episode.MediaInfo.VideoCodec);
                        logger.Debug("MediaInfo Codec: " + ModelClasses.CodecTypes.CodecNames[_MediaInfoCodec]);

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
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using X264toX265.ModelClasses;

namespace X264toX265.MediaOperations
{
    class CheckFileCodec
    {
        /// <summary>
        /// Compares the mediainfo result of each media file to the codec in the qualityprofile. Sometimes Radarr/Sonarr are dumb and don't properly tag the media based on mediainfo
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void GetMovieConversionList(List<Movie> MovieList)
        {
            try
            {
                foreach(Movie movie in MovieList)
                {
                    Utilities.Utilities.Logger.Debug("Now Processing: " + movie.Title);
                    if (!movie.HasFile) {
                        Utilities.Utilities.Logger.Debug("Movie has no downloaded files, processing skipped");
                        continue; //movie doesnt actually exist yet
                    }

                    int _MediaInfoCodec = CodecTypes.GetCodecID(movie.MovieFiles.MediaInfo.VideoCodec);
                    Utilities.Utilities.Logger.Debug("MediaInfo Codec: " + CodecTypes.CodecNames[_MediaInfoCodec]);

                    if (_MediaInfoCodec > 0) {
                        Utilities.Utilities.Logger.Debug("Conversion IS required");
                        Utilities.Utilities.Logger.Info($"Marking \"{movie.Title}\" as requiring conversion");
                        movie.ConversionRequired = true; //Mediainfo reports the file as not HEVC, conversion will be required.
                    }
                }
            }
            catch(Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
            }
        }
        public static void GetEpisodeConversionList(List<ModelClasses.Sonarr.Series> SeriesList)
        {
            try
            {
                foreach (ModelClasses.Sonarr.Series series in SeriesList)
                {
                    Utilities.Utilities.Logger.Debug("Now Processing Series: " + series.Title);
                    /**if (!series.HasFile)
                    {
                        Utilities.Utilities.Logger.Debug("Movie has no downloaded files, processing skipped");
                        continue; //movie doesnt actually exist yet
                    }*/
                    foreach(ModelClasses.Sonarr.EpisodeFile episode in series.Episodes)
                    {
                        Utilities.Utilities.Logger.Debug("Now Processing Episode ID: " + episode.ID);
                        int _MediaInfoCodec = CodecTypes.GetCodecID(episode.MediaInfo.VideoCodec);
                        Utilities.Utilities.Logger.Debug("MediaInfo Codec: " + CodecTypes.CodecNames[_MediaInfoCodec]);

                        if (_MediaInfoCodec > 0)
                        {
                            Utilities.Utilities.Logger.Debug("Conversion IS required");
                            Utilities.Utilities.Logger.Info($"Marking episode ID {episode.ID} in \"{series.Title}\" as requiring conversion");
                            episode.ConversionRequired = true; //Mediainfo reports the file as not HEVC, conversion will be required.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
            }
        }
    }
}

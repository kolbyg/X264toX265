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
        public static List<Movie> GetMovieConversionList(List<Movie> MovieList)
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

                    int _MediaInfoCodec = CodecTypes.GetCodecID(movie.MovieFiles.MediaInfo.VideoFormat);
                    int _CustomFormatCodec = CodecTypes.GetCodecID(movie.MovieFiles.QualityProfile.CustomFormats[0].Name);
                    Utilities.Utilities.Logger.Debug("MediaInfo Codec: " + CodecTypes.CodecNames[_MediaInfoCodec]);
                    Utilities.Utilities.Logger.Debug("CustomFormat Codec: " + CodecTypes.CodecNames[_CustomFormatCodec]);

                    bool _ConversionRequired = false;

                    if (_MediaInfoCodec > 0) {
                        Utilities.Utilities.Logger.Debug("Conversion IS required");
                        Utilities.Utilities.Logger.Info($"Marking \"{movie.Title}\" as requiring conversion");
                        movie.ConversionRequired = true; //Mediainfo reports the file as not HEVC, conversion will be required.
                    }

                    if (_MediaInfoCodec == _CustomFormatCodec)
                    {
                        Utilities.Utilities.Logger.Debug("Mediainfo matches CustomFormat, no extra work required");
                        continue; //Both mediainfo and the quality profile match, so no extra work needed
                    }
                    else if (_MediaInfoCodec < _CustomFormatCodec && !_ConversionRequired)
                    {
                        Utilities.Utilities.Logger.Warn("The Mediainfo codec is better than what is set in Customformats, this will be fixed");
                        //The file is better quality than the profile and we are not converting the file, fix the profile, leave the file
                        //TODO
                    }
                    else
                    {
                        //The file is worse than the custom profile, but conversion is required anyways, log the error but no extra work is needed
                        Utilities.Utilities.Logger.Warn($"{movie.CleanTitle} has a custom format tag that is a better codec than the actual file. The file is scheduled for conversion anyways, but you may want to look into this if it happens often.");
                    }
                }
                return MovieList;
            }
            catch(Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
    }
}

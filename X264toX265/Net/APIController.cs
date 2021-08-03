using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace X264toX265.Net
{
    class APIController
    {
        public static string RetrieveMovies(string RadarrURL, string RadarrAPIKey)
        {
            try
            {
                Utilities.Utilities.Logger.Debug("Starting REST call to Radarr API located at " + RadarrURL);
                RestClient client = new RestClient(RadarrURL);
                RestRequest request = new RestRequest("api/v3/movie?apiKey=" + RadarrAPIKey, DataFormat.Json);

                var response = client.Get(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static string RetrieveSeries(string SonarrURL, string SonarrAPIKey)
        {
            try
            {
                Utilities.Utilities.Logger.Debug("Starting REST call to Sonarr API located at " + SonarrURL);
                RestClient client = new RestClient(SonarrURL);
                RestRequest request = new RestRequest("api/series?apiKey=" + SonarrAPIKey, DataFormat.Json);

                var response = client.Get(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static string RetrieveEpisodes(string SonarrURL, string SonarrAPIKey, int SeriesID)
        {
            try
            {
                Utilities.Utilities.Logger.Debug("Starting REST call to Sonarr API located at " + SonarrURL);
                RestClient client = new RestClient(SonarrURL);
                RestRequest request = new RestRequest("api/episodefile?seriesId=" + SeriesID + "&apiKey=" + SonarrAPIKey, DataFormat.Json);

                var response = client.Get(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
    }
}

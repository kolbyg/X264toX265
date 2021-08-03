using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace X264toX265.Net
{
    class RadarrOperations
    {
        public static string RetrieveMovies(string RadarrURL, string RadarrAPIKey)
        {
            try
            {
                RestClient client = new RestClient(RadarrURL);
                RestRequest request = new RestRequest("api/v3/movie?apiKey=" + RadarrAPIKey, DataFormat.Json);

                var response = client.Get(request);
                return response.Content;
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

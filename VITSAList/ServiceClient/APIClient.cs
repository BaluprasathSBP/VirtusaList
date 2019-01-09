using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace VITSAList
{
    public class APIClient
    {
        public string baseUrl = "https://swapi.co/api/";
        public Response m_Response;

        /// <summary>
        /// Gets the planets.
        /// </summary>
        /// <returns>The planets.</returns>
        public async Task<PlanetForAPI> GetPlanets()
        {
            var url = Path.Combine(baseUrl, "planets");
            var getResult = await PerformActionAsync<PlanetForAPI>("GET", url);
            if (getResult != null)
            {
                return getResult;
            }
            return null;
        }


        #region HTTPClient Action

        /// <summary>
        /// Performs the action async.
        /// </summary>
        /// <returns>The action async.</returns>
        /// <param name="action">Action.</param>
        /// <param name="url">URL.</param>       
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private async Task<T> PerformActionAsync<T>(string action, string url) where T : class
        {
            // Exceptions are handled by the caller

            using (var request = new HttpRequestMessage())
            {

                var m_Client = new HttpClient();
                m_Client.BaseAddress = new Uri(url);
                var responseAsync = m_Client.GetAsync("");
                using (var response = responseAsync.GetAwaiter().GetResult())
                {
                    if (ProcessResponse(response))
                    {
                        try
                        {
                            return JSONSerializer<T>.DeSerialize(m_Response.Content);
                        }
                        catch (Exception exception)
                        {
                            throw new Exception(string.Concat("Could not deserialize the response body.", m_Response.StatusCode, m_Response.Content), exception);
                        }
                    }

                    return default(T);
                }
            }
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <returns><c>true</c>, if response was processed, <c>false</c> otherwise.</returns>
        /// <param name="response">Response.</param>
        /// <param name="okToReadContent">If set to <c>true</c> ok to read content.</param>
        private bool ProcessResponse(HttpResponseMessage response, bool okToReadContent = true)
        {
            // Exceptions are handled by the caller
            m_Response = new Response();
            m_Response.StatusCode = response.StatusCode;

            // Copy the response to Property ClientResponse to use it for non-ok results
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.PartialContent)
            {
                if (okToReadContent)
                {
                    m_Response.Content = ReadResponseContent(response).Result;
                }

                return true;
            }
            // Not found will return web page error, so do not attempt to process it
            else
            {
                var responseData = ReadResponseContent(response).Result;

                m_Response.Content = responseData;

                // NotFound will result in an Http page, so do not attempt to parse it.
                if (!string.IsNullOrEmpty(responseData) && response.StatusCode != HttpStatusCode.NotFound)
                {
                    m_Response.Message = "Error has occured";
                }
            }

            return false;
        }

        private async Task<string> ReadResponseContent(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion
    }

    /// <summary>
    /// Response format 
    /// </summary>
    public class Response
    {
        public HttpStatusCode StatusCode;
        public string Content;
        public string Message;
    }



}

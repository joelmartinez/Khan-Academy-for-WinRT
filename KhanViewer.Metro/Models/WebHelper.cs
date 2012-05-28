using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace KhanViewer.Models
{
    public static class WebHelper
    {
        public static void Get(string url, Action<string> action, Action<Exception> error)
        {
            Get(new Uri(url), action, error);
        }

        public static async void Get(Uri uri, Action<string> action, Action<Exception> error)
        {
            var request = WebRequest.CreateHttp(uri);

            request.BeginGetResponse(i =>
            {
                try
                {
                    var response = request.EndGetResponse(i);
                    var sreader = new StreamReader(response.GetResponseStream());
                    var result = sreader.ReadToEnd();
                    action(result);
                }
                catch (Exception ex)
                {
                    error(ex);
                }
            }, null);
        }

        public static void Json<T>(string url, Action<T> action, Action<Exception> error)
        {
            Json<T>(new Uri(url), action, error);
        }

        public static async void Json<T>(Uri uri, Action<T> action, Action<Exception> error)
        {
            try
            {
                var http = new HttpClient();
                http.MaxResponseContentBufferSize = Int32.MaxValue;
                HttpResponseMessage response = await http.GetAsync(uri);

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                var deserialized = (T)serializer.ReadObject(await response.Content.ReadAsStreamAsync());

                action(deserialized);
            }
            catch (Exception ex)
            {
                error(ex);
            }
        }
    }
}

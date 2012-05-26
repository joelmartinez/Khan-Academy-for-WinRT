using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace KhanViewer.Models
{
    public static class JsonHelper
    {
        public static async Task<T> DeserializeObject<T>(string url) where T : class
        {
            return await DeserializeObject<T>(new Uri(url));
        }

        public static async Task<T> DeserializeObject<T>(Uri url) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            try
            {
                HttpResponseMessage response = await new HttpClient().GetAsync(url);
                return serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as T;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public static class WebHelper
    {
        public static void Get(string url, Action<string> action, Action<Exception> error)
        {
            Get(new Uri(url), action, error);
        }

        public static void Get(Uri uri, Action<string> action, Action<Exception> error)
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

        public static void Json<T>(Uri uri, Action<T> action, Action<Exception> error)
        {
            Get(uri, json =>
            {
                try
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    using (var stream = new MemoryStream(bytes))
                    {
                        var deserialized = serializer.ReadObject(stream);

                        action((T)deserialized);
                    }
                }
                catch (Exception ex)
                {
                    error(ex);
                }
            }, error);
        }
    }
}

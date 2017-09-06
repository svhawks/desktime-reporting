using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DesktimeReporting.Data
{
    public abstract class Repository
    {

        private readonly HttpClient httpClient = new HttpClient();

        public T GetObject<T>(string url)
        {
            var json = GetJson(url);

            return (T) JsonConvert.DeserializeObject(json.ToString(), typeof(T));
        }

        public async Task<T> GetObjectAsync<T>(string url)
        {
            var json = await GetJsonAsync(url);

            return (T) JsonConvert.DeserializeObject(json.ToString(), typeof(T));
        }

        public async Task<JObject> GetJsonAsync(string url)
        {
            Console.WriteLine($"Requesting {url}");
            var content = await httpClient.GetStringAsync(url);
            JObject json;

            try
            {
                json = JObject.Parse(content);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error {e} post");
                json = JObject.Parse(content);
            }
            // Console.WriteLine($"Got {json} post");

            return json;
        }

        public JObject GetJson(string url)
        {
            Console.WriteLine($"Requesting {url}");
            var content = httpClient.GetStringAsync(url).Result;
            JObject json;

            try
            {
                json = JObject.Parse(content);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error {e} post");
                json = JObject.Parse(content);
            }
            // Console.WriteLine($"Got {json} post");

            return json;
        }

    }
}

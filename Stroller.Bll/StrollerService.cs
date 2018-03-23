using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stroller.Bll
{
    public abstract class StrollerService
    {
        protected async Task<T> ExecuteGetService<T>(string function)
        {
            using (var client = new HttpClient())
            {
                var path = "http://" + Properties.Settings.Default.IpAddress + ":" + Properties.Settings.Default.Port +
                           "/api/" + function;

                var response = await client.GetAsync(path);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }

                return await response.Content.ReadAsAsync<T>();
            }
        }

        protected async Task ExecutePostService<T>(T data, string function)
        {
            using (var client = new HttpClient())
            {
                var path = "http://" + Properties.Settings.Default.IpAddress + ":" + Properties.Settings.Default.Port +
                           "/api/" + function;
                var response = await client.PostAsJsonAsync(path, data);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
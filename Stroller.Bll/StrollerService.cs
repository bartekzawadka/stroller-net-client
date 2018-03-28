using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Stroller.Bll
{
    public abstract class StrollerService
    {
        protected async Task<T> ExecuteGetService<T>(string function, CancellationToken cancellationToken, Action<T> operateResultAction = null)
        {
            using (var client = new HttpClient())
            {
                var path = "http://" + Properties.Settings.Default.IpAddress + ":" + Properties.Settings.Default.Port +
                           "/api/" + function;

                var response = await client.GetAsync(path, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }

                var result = await response.Content.ReadAsAsync<T>(cancellationToken);

                operateResultAction?.Invoke(result);

                return result;
            }
        }

        protected async Task ExecutePostService<T>(T data, CancellationToken cancellationToken, string function)
        {
            using (var client = new HttpClient())
            {
                var path = "http://" + Properties.Settings.Default.IpAddress + ":" + Properties.Settings.Default.Port +
                           "/api/" + function;
                var response = await client.PostAsJsonAsync(path, data, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        protected async Task<T2> ExecutePostService<T1, T2>(T1 data, CancellationToken cancellationToken, string function)
        {
            using (var client = new HttpClient())
            {
                var path = "http://" + Properties.Settings.Default.IpAddress + ":" + Properties.Settings.Default.Port +
                           "/api/" + function;
                var response = await client.PostAsJsonAsync(path, data, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }

                return await response.Content.ReadAsAsync<T2>(cancellationToken);
            }
        }
    }
}
using Newtonsoft.Json;
using NugetGymphonyAGM.Models;
using System.Net.Http.Headers;
using System.Text;

namespace MvcGymphony.Services
{
    public class ServiceSalas
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceSalas( IConfiguration configuration, IHttpContextAccessor contextAccessor )
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiGymphony");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.contextAccessor = contextAccessor;
        }

        private async Task<T> CallApiAsync<T>( string request, string token )
        {
            using ( HttpClient client = new HttpClient() )
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if ( response.IsSuccessStatusCode == true )
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Salas>> GetTodasSalasAsync()
        {
            string request = "api/Salas";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<Salas>>(request, token) ?? new List<Salas>();
        }

        public async Task<bool> CrearSalaAsync( Salas sala )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Salas";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(sala);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> EliminarSalaAsync( int id )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Salas/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.DeleteAsync(request);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> EditarSalaAsync( Salas sala )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Salas";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(sala);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(request, content);

                return response.IsSuccessStatusCode;
            }
        }
    }
}

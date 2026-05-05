using Newtonsoft.Json;
using NugetGymphonyAGM.Models;
using System.Net.Http.Headers;
using System.Text;

namespace MvcGymphony.Services
{
    public class ServiceClases
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceClases( IConfiguration configuration, IHttpContextAccessor contextAccessor )
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

        public async Task<List<Clases>> GetTodasClasesAsync()
        {
            string request = "api/Clases";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<Clases>>(request, token) ?? new List<Clases>();
        }

        public async Task<bool> CrearClaseAsync( Clases clase )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Clases";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(clase);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> EditarClaseAsync( Clases clase )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Clases";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(clase);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(request, content);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> EliminarClaseAsync( int id )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Clases/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.DeleteAsync(request);

                return response.IsSuccessStatusCode;
            }
        }
    }
}

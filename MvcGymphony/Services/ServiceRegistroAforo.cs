using Newtonsoft.Json.Linq;
using NugetGymphonyAGM.Models;
using System.Net.Http.Headers;
using System.Text;

namespace MvcGymphony.Services
{
    public class ServiceRegistroAforo
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceRegistroAforo( IConfiguration configuration, IHttpContextAccessor contextAccessor )
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

        public async Task<List<DatosHoraPico>> GetHorasPicoAsync()
        {
            string request = "api/RegistroAforo/GetHorasPico";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<DatosHoraPico>>(request, token) ?? new List<DatosHoraPico>();
        }

        public async Task<int> GetAforoActualAsync()
        {
            string request = "api/RegistroAforo/GetAforoActual";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<int>(request, token);
        }

        public async Task<(bool success, string operacion, string message)> RegistrarAccesoAsync( int idSocio )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/RegistroAforo/RegistrarAcceso/" + idSocio;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                string data = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject jsonObject = JObject.Parse(data);

                    if ( response.IsSuccessStatusCode )
                    {
                        string operacion = jsonObject.GetValue("tipo")?.ToString();
                        string mensaje = jsonObject.GetValue("mensaje")?.ToString();

                        return (true, operacion, mensaje);
                    }
                    else
                    {
                        string message = jsonObject.GetValue("mensaje")?.ToString() ?? "Error al procesar el acceso.";
                        return (false, "", message);
                    }
                }
                catch
                {
                    return (false, "", "Error de servidor al leer el código QR.");
                }
            }
        }
    }
}

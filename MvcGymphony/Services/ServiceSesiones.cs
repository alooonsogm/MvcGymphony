using MvcGymphony.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetGymphonyAGM.Models;
using System.Net.Http.Headers;
using System.Text;

namespace MvcGymphony.Services
{
    public class ServiceSesiones
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceSesiones( IConfiguration configuration, IHttpContextAccessor contextAccessor )
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

        public async Task<List<DatosSesion>> GetSesionesNuevasAsync()
        {
            string request = "api/Sesiones/GetSesionesNuevas";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN").Value;
            List<DatosSesion> sesiones = await this.CallApiAsync<List<DatosSesion>>(request, token);
            return sesiones;
        }

        public async Task<List<int>> GetSesionesReservadasClienteNumeroAsync()
        {
            string request = "api/Sesiones/GetSesionesReservadasSocioNumero";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN").Value;
            List<int> sesionesNumero = await this.CallApiAsync<List<int>>(request, token);
            return sesionesNumero ?? new List<int>();
        }

        public async Task<DatosSesion> FindDatosSesionAsync( int idSesion )
        {
            string request = "api/Sesiones/FindDatosSesion/" + idSesion;
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;

            DatosSesion sesion = await this.CallApiAsync<DatosSesion>(request, token);
            return sesion;
        }

        public async Task<List<DatosSesion>> GetMisSesionesCompletasAsync()
        {
            string request = "api/Sesiones/GetMisFuturasSesiones";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<DatosSesion>>(request, token);
        }

        public async Task<List<DatosSesion>> GetTodasSesionesAsync()
        {
            string request = "api/Sesiones";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<DatosSesion>>(request, token) ?? new List<DatosSesion>();
        }

        public async Task<(bool esValida, string mensaje)> ValidarSesionAsync( ValidarSesionDTO model )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Sesiones/ValidarSesion";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);
                string data = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject jsonObject = JObject.Parse(data);

                    if ( response.IsSuccessStatusCode )
                    {
                        bool esValida = jsonObject.GetValue("esValida").Value<bool>();
                        string mensaje = jsonObject.GetValue("mensaje").ToString();
                        return (esValida, mensaje);
                    }
                    else
                    {
                        string mensaje = jsonObject.GetValue("mensaje").ToString();
                        return (false, mensaje);
                    }
                }
                catch
                {
                    return (false, "Error inesperado al leer la validación del servidor.");
                }
            }
        }

        public async Task<bool> CrearSesionAsync( Sesion sesion )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Sesiones";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(sesion);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> EditarSesionAsync( Sesion sesion )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Sesiones";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(sesion);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(request, content);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> EliminarSesionAsync( int id )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Sesiones/" + id;
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

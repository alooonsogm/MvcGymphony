using NugetGymphonyAGM.Models;
using System.Net.Http.Headers;

namespace MvcGymphony.Services
{
    public class ServiceHorarioUsuario
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceHorarioUsuario( IConfiguration configuration, IHttpContextAccessor contextAccessor )
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

        public async Task<List<HorarioEmpleados>> GetHorariosDeEntrenadorPerfilAsync()
        {
            string request = "api/HorarioUsuario/GetHorariosDeEntrenadorPerfil";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;

            List<HorarioEmpleados> horarios = await this.CallApiAsync<List<HorarioEmpleados>>(request, token);
            return horarios;
        }
    }
}

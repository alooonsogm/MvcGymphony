using NugetGymphonyAGM.Models;
using System.Net.Http.Headers;

namespace MvcGymphony.Services
{
    public class ServiceUsuarios
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceUsuarios( IConfiguration configuration, IHttpContextAccessor contextAccessor )
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

        public async Task<List<Usuario>> GetUsuariosPorSesionAsync( int idSesion )
        {
            string request ="api/Usuarios/GetUsuariosPorSesion/" + idSesion;
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;

            List<Usuario> users = await this.CallApiAsync<List<Usuario>>(request, token);
            return users;
        }

        public async Task<Usuario> GetMiPerfilAsync()
        {
            string request = "api/Usuarios/GetMiPerfil";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;

            Usuario perfil = await this.CallApiAsync<Usuario>(request, token);
            return perfil;
        }

        public async Task<List<Usuario>> GetTodosEntrenadoresAsync()
        {
            string request = "api/Usuarios/GetEntrenadores";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<Usuario>>(request, token) ?? new List<Usuario>();
        }
    }
}

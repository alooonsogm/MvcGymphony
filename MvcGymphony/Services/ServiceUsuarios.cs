using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetGymphonyAGM.Models;
using System.Net.Http.Headers;
using System.Text;

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

        public async Task<List<VistaSocio>> GetSociosConEstadoAsync()
        {
            string request = "api/Usuarios/GetSociosConEstado";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;

            return await this.CallApiAsync<List<VistaSocio>>(request, token) ?? new List<VistaSocio>();
        }

        public async Task<List<VistaUsuario>> GetUsuariosPorRolAsync( string rol )
        {
            string request = "api/Usuarios/GetUsuariosPorRol/" + rol;
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;

            return await this.CallApiAsync<List<VistaUsuario>>(request, token) ?? new List<VistaUsuario>();
        }

        public async Task<bool> CrearSocioAsync( string email, string password, string nombre, string apellidos, string telefono, DateOnly fechaNacimiento, string dni, IFormFile foto )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Usuarios/RegistroSocio";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                using ( MultipartFormDataContent content = new MultipartFormDataContent() )
                {
                    content.Add(new StringContent(email), "Email");
                    content.Add(new StringContent(password), "Password");
                    content.Add(new StringContent(nombre), "Nombre");
                    content.Add(new StringContent(apellidos), "Apellidos");
                    content.Add(new StringContent(telefono), "Telefono");
                    content.Add(new StringContent(fechaNacimiento.ToString("yyyy-MM-dd")), "FechaNacimiento");
                    content.Add(new StringContent(dni), "Dni");

                    if ( foto != null )
                    {
                        Stream stream = foto.OpenReadStream();
                        StreamContent fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
                        content.Add(fileContent, "RutaFoto", foto.FileName);
                    }

                    HttpResponseMessage response = await client.PostAsync(request, content);

                    return response.IsSuccessStatusCode;
                }
            }
        }

        public async Task<(bool success, string message)> DarDeBajaSocioAsync( int idSocio )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Usuarios/DarDeBajaSocio/" + idSocio;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);

                string data = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject jsonObject = JObject.Parse(data);
                    string mensaje = jsonObject.GetValue("mensaje").ToString();
                    return (response.IsSuccessStatusCode, mensaje);
                }
                catch
                {
                    return (response.IsSuccessStatusCode, "Error inesperado al leer la respuesta del servidor.");
                }
            }
        }

        public async Task<(bool success, string message)> DarDeAltaSocioAsync( int idSocio )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Usuarios/DarDeAltaSocio/" + idSocio;
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
                    string mensaje = jsonObject.GetValue("mensaje").ToString();
                    return (response.IsSuccessStatusCode, mensaje);
                }
                catch
                {
                    return (response.IsSuccessStatusCode, "Error inesperado al leer la respuesta del servidor.");
                }
            }
        }

        public async Task<bool> EliminarSocioAsync( int id )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Usuarios/DeleteSocio/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.DeleteAsync(request);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> CrearEntrenadorAsync( string email, string password, string nombre, string apellidos, string telefono, DateOnly fechaNacimiento, string dni, IFormFile foto, List<int> diasSemana, List<TimeOnly> horasInicio, List<TimeOnly> horasFin )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Usuarios/RegistroEntrenador";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                using ( MultipartFormDataContent content = new MultipartFormDataContent() )
                {
                    content.Add(new StringContent(email), "Usuario.Email");
                    content.Add(new StringContent(password), "Usuario.Password");
                    content.Add(new StringContent(nombre), "Usuario.Nombre");
                    content.Add(new StringContent(apellidos), "Usuario.Apellidos");
                    content.Add(new StringContent(telefono), "Usuario.Telefono");
                    content.Add(new StringContent(fechaNacimiento.ToString("yyyy-MM-dd")), "Usuario.FechaNacimiento");
                    content.Add(new StringContent(dni), "Usuario.Dni");

                    if ( foto != null )
                    {
                        Stream stream = foto.OpenReadStream();
                        StreamContent fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);

                        content.Add(fileContent, "Usuario.RutaFoto", foto.FileName);
                    }

                    if ( diasSemana != null )
                    {
                        for ( int i = 0 ; i < diasSemana.Count ; i++ )
                        {
                            content.Add(new StringContent(diasSemana[i].ToString()), "DiasSemana");
                            content.Add(new StringContent(horasInicio[i].ToString("HH:mm")), "HorasInicio");
                            content.Add(new StringContent(horasFin[i].ToString("HH:mm")), "HorasFin");
                        }
                    }

                    HttpResponseMessage response = await client.PostAsync(request, content);

                    return response.IsSuccessStatusCode;
                }
            }
        }

        public async Task<(bool success, bool hasSessions, List<SustitutoItem> sustitutos, string message)> ValidarBorradoEntrenadorAsync( int idEntrenador )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Usuarios/ValidarBorradoEntrenador/" + idEntrenador;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.GetAsync(request);
                string data = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject jsonObject = JObject.Parse(data);

                    if ( response.IsSuccessStatusCode )
                    {
                        bool success = jsonObject.GetValue("success").Value<bool>();
                        bool hasSessions = jsonObject.GetValue("hasSessions").Value<bool>();
                        List<SustitutoItem> sustitutos = JsonConvert.DeserializeObject<List<SustitutoItem>>(jsonObject["sustitutos"].ToString());

                        return (success, hasSessions, sustitutos, "");
                    }
                    else
                    {
                        string message = jsonObject.GetValue("message")?.ToString() ?? "Error de validación.";
                        return (false, false, null, message);
                    }
                }
                catch
                {
                    return (false, false, null, "Error inesperado al leer la respuesta del servidor.");
                }
            }
        }

        public async Task<(bool success, string message)> EliminarEntrenadorAsync( int idBorrar, int? idSustituto )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Usuarios/DeleteEntrenadorSustituyendo/" + idBorrar;
                if ( idSustituto.HasValue )
                {
                    request += "?idEntrenadorSustituto=" + idSustituto.Value;
                }

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.DeleteAsync(request);
                string data = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject jsonObject = JObject.Parse(data);
                    string mensaje = jsonObject.GetValue("mensaje")?.ToString() ?? "Operación completada";
                    return (response.IsSuccessStatusCode, mensaje);
                }
                catch
                {
                    return (response.IsSuccessStatusCode, "Error inesperado al leer la respuesta del servidor.");
                }
            }
        }

        public async Task<List<DatosEvolucion>> GetEvolucionSociosAsync()
        {
            string request = "api/Usuarios/GetEvolucionSocios";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<DatosEvolucion>>(request, token) ?? new List<DatosEvolucion>();
        }

        public async Task<List<string>> GetMisDiasAsistenciaAsync()
        {
            string request = "api/Usuarios/GetMisDiasAsistencia";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN")?.Value;
            return await this.CallApiAsync<List<string>>(request, token) ?? new List<string>();
        }
    }
    public class SustitutoItem
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }
}

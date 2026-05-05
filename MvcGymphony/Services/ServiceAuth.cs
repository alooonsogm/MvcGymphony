using MvcGymphony.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcGymphony.Services
{
    public class ServiceAuth
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceAuth( IConfiguration configuration, IHttpContextAccessor contextAccessor )
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiGymphony");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.contextAccessor = contextAccessor;
        }

        public async Task<(string token, string mensajeError)> LogInAsync( string email, string pass )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/Auth/Login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                LoginModelDTO model = new LoginModelDTO
                {
                    Email = email,
                    Password = pass
                };

                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                if ( response.IsSuccessStatusCode == true )
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return (token, null);
                }
                else
                {
                    string errorData = await response.Content.ReadAsStringAsync();
                    string mensajeApi = "Error al intentar iniciar sesión.";

                    try
                    {
                        JObject errorJson = JObject.Parse(errorData);
                        if ( errorJson["mensaje"] != null )
                        {
                            mensajeApi = errorJson.GetValue("mensaje").ToString();
                        }
                    }
                    catch
                    {
                    }

                    return (null, mensajeApi);
                }
            }
        }
    }
}

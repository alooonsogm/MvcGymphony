using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcGymphony.Services
{
    public class ServiceReservaSesiones
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceReservaSesiones( IConfiguration configuration, IHttpContextAccessor contextAccessor )
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiGymphony");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.contextAccessor = contextAccessor;
        }

        public async Task<string> ReservarPlazaAsync( int idSesion )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/ReservaSesiones/ReservarPlaza/" + idSesion;
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
                    return jsonObject.GetValue("mensaje").ToString();
                }
                catch
                {
                    return "Error inesperado de conexión al reservar.";
                }
            }
        }

        public async Task<string> AnularReservaAsync( int idSesion )
        {
            using ( HttpClient client = new HttpClient() )
            {
                string request = "api/ReservaSesiones/AnularReserva/" + idSesion;
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
                    return jsonObject.GetValue("mensaje").ToString();
                }
                catch
                {
                    return "Error inesperado de conexión al anular.";
                }
            }
        }
    }
}

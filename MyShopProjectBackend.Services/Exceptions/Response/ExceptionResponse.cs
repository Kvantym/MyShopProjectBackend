using Newtonsoft.Json;

namespace MyShopProjectBackend.Services.Exceptions.Response
{
    public class ExceptionResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}

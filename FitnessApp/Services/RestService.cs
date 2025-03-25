using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnesApp.Services
{
    public class RestService
    {

        public HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public RestService()
        {
            _client = new HttpClient()
            {
                //Set localhost to something else for Android because just because :)
#if ANDROID
                BaseAddress = new Uri("http://10.0.2.2:5251")
#else
                BaseAddress = new Uri("http://localhost:5251")
#endif
            };
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskManagerAPIGateway.Helpers
{
    public class CustomRouter
    {
        public List<ApiConfig> Routes { get; set; }

        public static ApiConfig GetUrl(string apiName)
        {
            var routes = LoadRoutes();

            return routes.Routes.FirstOrDefault(o => o.ApiName == apiName);
        }

        private static CustomRouter LoadRoutes()
        {
            using StreamReader reader = new StreamReader(".//routes.json");
            var json = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<CustomRouter>(json);
            return result;
        }
    }

    public class ApiConfig
    {
        public string ApiName { get; set; }
        public Destination Destination { get; set; }
    }

    public class Destination
    {
        public string Url { get; set; }
        public bool RequiresAuthentication { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskManager.Services;
using TaskManagerAPIGateway.GraphQL;

namespace TaskManagerAPIGateway.Controllers
{
    [Route("graphql")]
    public class GraphQLController: Controller
    {
        private GraphQLService.GraphQLServiceClient _client = null;

        protected GraphQLService.GraphQLServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    var channel = GrpcChannel.ForAddress("https://localhost:5001");
                    _client = new GraphQLService.GraphQLServiceClient(channel);
                }

                return _client;
            }
        }

        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            var request = new Request
            {
                Query = query.Query,
            };

            if (query.Variables != null)
            {
                var inputs = query.Variables.ToInputs();
                var jsonInput = "{" + BuildJson(inputs) + "}";
                request.Inputs = jsonInput;
            }
            var result = Client.Execute(request);
            var data = String.Empty;
            await foreach (var response in result.ResponseStream.ReadAllAsync())
            {
                if (response.Errors != null && response.Errors.Any())
                {
                    return Ok(response);
                }
                data += response.Data;
            }
            
            return Ok(data);
        }

        public static string BuildJson(IEnumerable resultData)
        {
            var ans = "";
            foreach (var data in resultData)
            {
                var key = data.GetPropertyValue("Key");
                var value = data.GetPropertyValue("Value");

                if (key == null)
                {
                    continue;
                }
                //var type = value.GetType();

                if (value is List<object> list)
                {
                    ans += JsonConvert.SerializeObject(key);
                    ans += ": ";
                    ans += "[";
                    foreach (var l in list)
                    {
                        ans += "{";
                        ans += BuildJson(l as IEnumerable);
                        ans += "}";
                        ans += ",";
                    }
                    if (ans.EndsWith(","))
                    {
                        ans = ans.Remove(ans.Length - 1);
                    }
                    ans += "]";
                }
                else if (value is Dictionary<string, object> array)
                {
                    ans += JsonConvert.SerializeObject(key);
                    ans += ": ";
                    ans += "{";
                    ans += BuildJson(array);
                    ans += "}";
                }
                else
                {
                    ans += BuildData(data);
                }

                ans += ",";
            }
            if (ans.EndsWith(","))
            {
                ans = ans.Remove(ans.Length - 1);
            }

            return ans;
        }

        private static string BuildData(object data)
        {
            var ans = "";
            var key = data.GetPropertyValue("Key");
            var value = data.GetPropertyValue("Value");
            ans += JsonConvert.SerializeObject(key);
            ans += ": ";
            ans += JsonConvert.SerializeObject(value);
            return ans;
        }
    }
}

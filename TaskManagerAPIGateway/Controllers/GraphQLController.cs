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
                var jsonInput = JsonConvert.SerializeObject(inputs);
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
    }
}

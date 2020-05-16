using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
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
                Query = query.Query
            };
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

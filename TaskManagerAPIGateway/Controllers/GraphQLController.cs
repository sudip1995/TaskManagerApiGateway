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
using TaskManagerAPIGateway.Helpers;

namespace TaskManagerAPIGateway.Controllers
{
    [Route("graphql/{apiName}")]
    public class GraphQLController: Controller
    {
        public async Task<IActionResult> Post(string apiName, [FromBody] GraphQLQuery query)
        {
            var client = GrpcClientPool.CreateInstance(apiName);

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

            var result = client.Execute(request);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using TaskManager.Services;

namespace TaskManagerAPIGateway.Helpers
{
    public static class GrpcClientPool
    {
        public static Dictionary<string, ChannelBase> Clients = new Dictionary<string, ChannelBase>();
        public static GraphQLService.GraphQLServiceClient CreateInstance(string apiName)
        {
            ChannelBase channel;
            if (Clients.ContainsKey(apiName))
            {
                channel = Clients[apiName];
            }
            else
            {
                var apiConfig = CustomRouter.GetUrl(apiName);
                if (apiConfig == null)
                {
                    throw new Exception($"No route found for api - {apiName}");
                }
                channel = GrpcChannel.ForAddress(apiConfig.Destination.Url);
                Clients[apiName] = channel;
            }

            return new GraphQLService.GraphQLServiceClient(channel);
        }

    }
}

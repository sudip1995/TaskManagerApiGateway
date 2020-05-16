using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace TaskManagerAPIGateway.GraphQL
{
    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}

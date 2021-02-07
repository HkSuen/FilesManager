using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Comm.Extends
{
    public static class HttpRequestExtends
    {
        private static string Reqpuest_Id = "Request-Id";
        private static string RequestIdAdd(HttpRequest Request)
        {
            string RequestId = Guid.NewGuid().ToString("N");
            Request.Headers.Add(Reqpuest_Id, RequestId);
            return RequestId;
        }
        public static string RequestId(this HttpRequest Request)
        {
            StringValues RequestId = string.Empty;
            if (!Request.Headers.TryGetValue(Reqpuest_Id, out RequestId))
            {
                RequestId = RequestIdAdd(Request);
            };
            return RequestId;
        }
        public static async Task<Dictionary<string, string>> ToDictionary(this IFormCollection Form)
        {
            return Form.ToDictionary(x => x.Key, x => x.Value.ToString());
        }
    }
}

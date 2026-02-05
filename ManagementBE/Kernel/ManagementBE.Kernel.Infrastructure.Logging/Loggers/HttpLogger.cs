//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Features;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Newtonsoft.Json;
//using ManagementBE.Kernel.Core.Enums;
//using ManagementBE.Kernel.Core.Persistence;
//using ManagementBE.Kernel.Domain.Logs;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ManagementBE.Kernel.Infrastructure.Logging.Loggers
//{
//    /// <summary>
//    /// HttpLogger
//    /// </summary>
//    public class HttpLogger : IAsyncActionFilter
//    {
//        /// <summary>
//        /// define http Log Repository
//        /// </summary>
//        private IHttpLogRepository _httpLogRepository { set; get; }
//        /// <summary>
//        /// Http Logger constructor 
//        /// </summary>
//        /// <param name="httpLogRepository"></param>
//        public HttpLogger(IHttpLogRepository httpLogRepository)
//        {
//            _httpLogRepository = httpLogRepository;
//        }
//        /// <summary>
//        /// On Action Execution Async Logger
//        /// </summary>
//        /// <param name="context"></param>
//        /// <param name="next"></param>
//        /// <returns></returns>
//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
            
//        }

//        private void LogError(HttpLog log, Exception exception)
//        {
           
//        }

//        private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
//        {
//            Dictionary<string, string> pairs = new Dictionary<string, string>();
//            foreach (var header in headers)
//            {
//                pairs.Add(header.Key, header.Value);
//            }
//            return pairs;
//        }

//        private List<KeyValuePair<string, string>> FormatQueries(string queryString)
//        {
//            List<KeyValuePair<string, string>> pairs =
//                 new List<KeyValuePair<string, string>>();
//            string key, value;
//            foreach (var query in queryString.TrimStart('?').Split("&"))
//            {
//                var items = query.Split("=");
//                key = items.Count() >= 1 ? items[0] : string.Empty;
//                value = items.Count() >= 2 ? items[1] : string.Empty;
//                if (!String.IsNullOrEmpty(key))
//                {
//                    pairs.Add(new KeyValuePair<string, string>(key, value));
//                }
//            }
//            return pairs;
//        }

//        private async Task<string> ReadBodyFromRequest(HttpRequest request)
//        {
//            // Ensure the request's body can be read multiple times 
//            // (for the next middlewares in the pipeline).
//            request.EnableBuffering();
//            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
//            var requestBody = await streamReader.ReadToEndAsync();
//            // Reset the request's body stream position for 
//            // next middleware in the pipeline.
//            request.Body.Position = 0;
//            return requestBody;
//        }


//    }
//}

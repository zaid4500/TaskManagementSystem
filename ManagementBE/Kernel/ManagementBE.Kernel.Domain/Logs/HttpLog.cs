using ManagementBE.Kernel.Domain.Entities.Base;
using System;

namespace ManagementBE.Kernel.Domain.Logs
{
    public class HttpLog : Entity<long>
    {
        public DateTime RequestDateTime { get; set; }
        public string? IPAddress { get; set; }
        public string? Node { get; set; }
        public string? RequestMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestQuery { get; set; }
        public string? RequestQueries { get; set; }
        public string? RequestScheme { get; set; }
        public string? RequestHost { get; set; }
        public string? RequestHeaders { get; set; }
        public string? RequestBody { get; set; }
        public string? RequestContentType { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public double? ActionPerionTime { get; set; }
        public string? ResponseStatus { get; set; }
        public string? ResponseHeaders { get; set; }
        public string? ResponseBody { get; set; }
        public string? ResponseContentType { get; set; }
        public bool? IsExceptionActionLevel { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? ExceptionStackTrace { get; set; }
        public string? ExceptionDetails { get; set; }
        public string? Username { get; set; }
    }
}

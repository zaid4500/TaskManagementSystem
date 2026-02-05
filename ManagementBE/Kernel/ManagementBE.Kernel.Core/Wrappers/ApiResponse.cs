using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Wrappers
{
    public class ApiResponse<T>
    {
        public ApiResponse()
        {

        }
        public ApiResponse(int statusCode, string details = "Internal Server Error")
        {
            StatusCode = statusCode;
            Message = details;
        }

        public bool IsSuccess { get; set; }
        public T Result { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Helpers
{
    public static class FileExtensions
    {
        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return "";
            }
        }

        public static string GetContentType(string fileName)
        {
            string contentType = "application/octet-stream";

            // Get the file extension
            string fileExtension = Path.GetExtension(fileName).ToLower();

            // Content type mappings based on file extensions
            switch (fileExtension)
            {
                case ".png":
                    contentType = "image/png";
                    break;
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".xls":
                case ".xlsx":
                case ".csv":
                    contentType = "application/vnd.ms-excel";
                    break;
                    // Add more content type mappings as needed...
            }

            return contentType;
        }
    }
}

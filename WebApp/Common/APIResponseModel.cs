using System.Collections.Generic;
using System.Net;

namespace VFL.Renderer.Common
{
    
    public class HttpErrorResponse
    {
        public string type { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public Dictionary<string, string[]> errors { get; set; }

        public string detail { get; set; }
        public string traceId { get; set; }
    }

    public class Errors
    {
        public string[] Number { get; set; }
      
    }

    public class APIBaseResponseModel : HttpErrorResponse
    {
        public string frontendErrorMessage { get; set; }
        public string developerErrorMessage { get; set; }
        public bool isException { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

}

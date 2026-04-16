using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFL.Renderer.OSApi.Common
{
    public class BaseResponse
    {
        public string frontendErrorMessage { get; set; }
        public string developerErrorMessage { get; set; }
        public bool isException { get; set; }
        public int StatusCode { get; set; }
        
    }
}

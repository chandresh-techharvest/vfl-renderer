using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.OSApi.Common;

namespace VFL.Renderer.OSApi.Registration.Model
{
    public class RegistrationResponse : BaseResponse
    {
        public RegistrationResponse(RegistrationDataResponse data)
        {
            this.data = data;
        }

        public RegistrationDataResponse data { get; set; }
    }

    public class RegistrationDataResponse
    {
        public bool isUserEmailExists { get; set; }
        public bool isRegistered { get; set; }
        public bool isSent { get; set; }
        public bool isResend { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}

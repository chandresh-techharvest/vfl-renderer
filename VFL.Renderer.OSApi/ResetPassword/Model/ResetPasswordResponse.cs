using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.OSApi.Common;

namespace VFL.Renderer.OSApi.ResetPassword.Model
{
    public class ResetPasswordResponse : BaseResponse
    {
        public ResetPasswordDataResponse data { get; set; }
    }

    public class ResetPasswordDataResponse
    {
        public bool isReset { get; set; }
        public bool isSent { get; set; }
    }
}

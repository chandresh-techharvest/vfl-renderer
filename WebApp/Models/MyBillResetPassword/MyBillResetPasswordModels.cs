namespace VFL.Renderer.Models.MyBillResetPassword
{
    /// <summary>
    /// Request model for MyBill forgot password - uses BAN instead of email
    /// </summary>
    public class MyBillForgotPasswordRequest
    {
        public string ban { get; set; }
        public string verificationPageUrl { get; set; }
    }

    /// <summary>
    /// Request model for MyBill reset password
    /// </summary>
    public class MyBillResetPasswordRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public string SecurityToken { get; set; }
    }

    /// <summary>
    /// Response data for submit request
    /// </summary>
    public class SubmitRequestData
    {
        public bool isSent { get; set; }
    }

    /// <summary>
    /// Response data for reset password
    /// </summary>
    public class ResetPasswordData
    {
        public bool isReset { get; set; }
    }

    /// <summary>
    /// Response model for MyBill submit request
    /// </summary>
    public class MyBillSubmitRequestResponse
    {
        public SubmitRequestData data { get; set; }
    }

    /// <summary>
    /// Response model for MyBill reset password
    /// </summary>
    public class MyBillResetPasswordResponse
    {
        public ResetPasswordData data { get; set; }
    }
}

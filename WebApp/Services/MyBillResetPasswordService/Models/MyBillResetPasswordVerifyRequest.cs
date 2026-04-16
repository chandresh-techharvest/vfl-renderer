namespace VFL.Renderer.Services.MyBillResetPasswordService.Models
{
    /// <summary>
    /// Request model for MyBill reset password verification
    /// </summary>
    public class MyBillResetPasswordVerifyRequest
    {
        public string password { get; set; }
        public string token { get; set; }
        public string username { get; set; }
    }
}

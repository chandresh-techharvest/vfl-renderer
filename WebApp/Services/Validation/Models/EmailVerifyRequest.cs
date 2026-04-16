namespace VFL.Renderer.Services.Validation.Models
{
    public class EmailVerifyRequest
    {
        public string email { get; set; }
    }

    public class NumberVerifyRequest
    {
        public string number { get; set; }
    }

    public class NumberRequest
    {
        public string Phone { get; set; }
    }
}

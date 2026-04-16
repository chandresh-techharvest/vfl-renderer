namespace VFL.Renderer.Common
{
    public class ApiResponse<T> : APIBaseResponseModel
    {
       public T data { get; set; }
    }
}

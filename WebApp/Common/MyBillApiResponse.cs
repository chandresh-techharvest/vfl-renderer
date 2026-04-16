using System.Net;

namespace VFL.Renderer.Common
{
    /// <summary>
    /// Standardized API response wrapper for MyBill endpoints
    /// Use this for consistent response format across all controllers
    /// </summary>
    /// <typeparam name="T">The type of data payload</typeparam>
    public class MyBillApiResponse<T>
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Human-readable message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The response data payload
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Create a successful response
        /// </summary>
        public static MyBillApiResponse<T> Success(T data, string message = null)
        {
            return new MyBillApiResponse<T>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = message ?? "Success",
                Data = data
            };
        }

        /// <summary>
        /// Create a successful response with custom status code
        /// </summary>
        public static MyBillApiResponse<T> Success(T data, int statusCode, string message = null)
        {
            return new MyBillApiResponse<T>
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message ?? "Success",
                Data = data
            };
        }

        /// <summary>
        /// Create an error response
        /// </summary>
        public static MyBillApiResponse<T> Error(string message, int statusCode = 400)
        {
            return new MyBillApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Data = default
            };
        }

        /// <summary>
        /// Create an unauthorized error response
        /// </summary>
        public static MyBillApiResponse<T> Unauthorized(string message = null)
        {
            return new MyBillApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = 401,
                Message = message ?? MyBillErrorMessages.NotAuthenticated,
                Data = default
            };
        }

        /// <summary>
        /// Create a not found error response
        /// </summary>
        public static MyBillApiResponse<T> NotFound(string message = null)
        {
            return new MyBillApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = message ?? "Resource not found",
                Data = default
            };
        }

        /// <summary>
        /// Create a server error response
        /// </summary>
        public static MyBillApiResponse<T> ServerError(string message = null)
        {
            return new MyBillApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = message ?? MyBillErrorMessages.ServerError,
                Data = default
            };
        }
    }

    /// <summary>
    /// Non-generic version for responses without data payload
    /// </summary>
    public class MyBillApiResponse : MyBillApiResponse<object>
    {
        /// <summary>
        /// Create a successful response without data
        /// </summary>
        public static new MyBillApiResponse Success(string message = null)
        {
            return new MyBillApiResponse
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = message ?? "Success",
                Data = null
            };
        }

        /// <summary>
        /// Create an error response without data
        /// </summary>
        public static new MyBillApiResponse Error(string message, int statusCode = 400)
        {
            return new MyBillApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Data = null
            };
        }
    }
}

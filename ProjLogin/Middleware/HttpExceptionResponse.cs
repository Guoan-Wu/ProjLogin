using System.Net;

namespace ProjLogin.Middleware
{
    public class HttpExceptionResponse
    {
        private string _message = string.Empty;

        public HttpStatusCode StatusCode { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ExceptionType { get; set; }
        public string Message { get => _message; set => _message = value; }
        public string Details { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
    public static class HttpExcepResponExtensions
    {
        public static void SetSystemException(this HttpExceptionResponse obj,Exception e)
        {
            obj.StatusCode = HttpStatusCode.InternalServerError;
            obj.ExceptionType = "SystemException";
            obj.Message = e.Message;
            obj.Details = e.ToString();
        }
        public static void SetBusinessException(this HttpExceptionResponse obj, BusinessException e)
        {
            obj.StatusCode = e.StatusCode;
            obj.ExceptionType = "BusinessException";
            obj.Message = e.Message;
            obj.Details = e.ToString();
        }
    }
}

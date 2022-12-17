using System.Net;

namespace ProjLogin.Middleware
{
    public class HttpExceptionResponse
    {
        private string _message = string.Empty;

        public HttpStatusCode StatusCode { get; set; }
        public string ExceptionType { get; set; }
        public string Message { get => _message; set => _message = value; }
        public string Details { get; set; }
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

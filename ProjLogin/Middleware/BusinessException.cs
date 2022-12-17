using System.Net;

namespace ProjLogin.Middleware
{
    public class BusinessException:Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public BusinessException(HttpStatusCode code, string message):base(message) 
        {
            StatusCode = code;
        }
    }
}

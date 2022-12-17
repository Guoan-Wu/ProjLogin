using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ProjLogin.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (BusinessException businException)
            {                
                context.Response.StatusCode = (int)businException.StatusCode;
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                HttpExceptionResponse result = new();
                result.SetBusinessException(businException);
                await context.Response.WriteAsync(
                  JsonConvert.SerializeObject(result,
                  new JsonSerializerSettings
                  {
                      ContractResolver = new CamelCasePropertyNamesContractResolver()
                  }), Encoding.UTF8);

            }
            catch (Exception e)
            {                
                const int statusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                HttpExceptionResponse result = new();
                result.SetSystemException(e);
                await context.Response.WriteAsync(
                  JsonConvert.SerializeObject(result,
                  new JsonSerializerSettings
                  {
                      ContractResolver = new CamelCasePropertyNamesContractResolver()
                  }), Encoding.UTF8);

            }
        }
    }
    public static class ExceptionHandlerMiddlewareExtensions
    {
        //[ExceptionHandler]
        public static IApplicationBuilder UseMyExceptionHandler(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}

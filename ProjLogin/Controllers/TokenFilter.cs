using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjLogin.DTO;
using ProjLogin.Encrypt;
using ProjLogin.Services;

namespace ProjLogin.Controllers
{
    public class TokenFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("run OnActionExecuting.");
            var headers = context.HttpContext.Request.Headers;
            var token = headers["Authorization"];
            var cont = token.ToString().Substring(7);//6
            var temp = context.ActionArguments["dto"];
            if (temp == null) { 
                context.Result = new BadRequestObjectResult("No password in the token");
                return; 
            }
            string password = ((ResetPasswordDTO)temp).OldPassword;
     
            string? errorMsg;
            if (!UserLogic.VerifyToken(cont, password, out errorMsg))
            {
                context.Result = new BadRequestObjectResult(errorMsg);
            }
            return;
        }
    }
}

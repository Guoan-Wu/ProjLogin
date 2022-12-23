using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjLogin.Controllers.controllerResults;

namespace ProjLogin.Controllers
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ControllerBase obj = (ControllerBase)context.Controller;
            if (!obj.ModelState.IsValid)
            {
                context.Result = obj.BadRequest(new ActionResultBasic<string> { Success = false, ErrorMessage = "Invalid parameters." });
                return;
            }            
        }
    }
}

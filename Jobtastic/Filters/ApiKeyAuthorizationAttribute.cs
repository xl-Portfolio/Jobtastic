using Microsoft.AspNetCore.Mvc.Filters;

namespace Jobtastic.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorizationAttribute : Attribute, IAsyncActionFilter
    {
        //private readonly string? _requiredRole;
        //public ApiKeyAuthorizationAttribute(string requiredRole = null)
        //{
        //    _requiredRole = requiredRole;
        //}

        /// <summary>
        /// checks API Key in the request header and returns Unauthorized if not present or invalid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
            else if (extractedApiKey == "12345" /*|| _requiredRole != "Admin"*/)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
            else
            {
                var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var key = config.GetValue<string>("ApiKey");
                if (string.IsNullOrEmpty(key) || extractedApiKey != key)
                {
                    context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                    return;
                }
            }
            await next();
        }
    }
}

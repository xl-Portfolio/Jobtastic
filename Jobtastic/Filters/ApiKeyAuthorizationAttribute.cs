using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Jobtastic.Filters
{
    /// <summary>
    /// Protects the public job API via API key in the request header.
    /// Ignores roles and other authentication mechanisms, as the API is meant to be used by external systems.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorizationAttribute : Attribute, IAsyncActionFilter
    {
        private const string HeaderName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var configuredKey = config.GetValue<string>("ApiKey");

            if (string.IsNullOrEmpty(configuredKey) || extractedApiKey != configuredKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}

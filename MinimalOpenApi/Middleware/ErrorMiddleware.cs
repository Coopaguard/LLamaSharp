using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Authentication;

namespace MinimalOpenApi.Middleware;

public class ErrorMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        { 
            switch(ex)
            {
                case AuthenticationException authenticationException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsJsonAsync(new ExceptionResult(ex.Message));
                    break;
            }
        }
    }

}

public record ExceptionResult(string Message, string? Details = null);

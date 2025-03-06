using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System.Security.Cryptography;

namespace MinimalOpenApi.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _valideKey;
    private readonly bool _shouldValidKey;

    public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        var cfgKey = configuration.GetValue<string>("OPENAI_API_KEY");
        if (!string.IsNullOrWhiteSpace(cfgKey))
        {
            _shouldValidKey = true;
            _valideKey = cfgKey;
        }
        else 
        { 
            _shouldValidKey = false;
        }
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if(_shouldValidKey && token != _valideKey)
        {
            throw new AuthenticationException();
        }

        await _next(context);
    }
}

using Microsoft.AspNetCore.Mvc;
using MinimalOpenApi.Dto;

namespace MinimalOpenApi.Endpoints;

public static class ChatRoutes
{
    public static WebApplication MapChatRoutes(this WebApplication app)
    {
        app.MapPost("/api/generate", ([FromBody] GeneratedRequestDto requestDto) =>
        {
            return "Hi";
        })
        .WithName("Generate")
        .WithOpenApi();

        app.MapPost("/api/chat", () =>
        {
            return "Hi";
        })
        .WithName("Chat")
        .WithOpenApi();

        return app;
    }
}

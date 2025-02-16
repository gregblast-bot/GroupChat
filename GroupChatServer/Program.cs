using Grpc.GroupChatServer.Services;

namespace Grpc.GroupChatServer.Main;

/// <summary>
/// This program processes inputs from the client and then relays those messages to other clients in the group chat.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddSingleton<MessageStreamingService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<GroupChatService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}

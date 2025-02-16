using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.AspNetCore.Server;
using Grpc.GroupChatServer.Services;
using Grpc.GroupChatServer.Main;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Grpc.GroupChatServer;
using Grpc.GroupChatClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace GroupChatServer.Tests;

public class GroupChatServiceTests
{
    private readonly GrpcChannel _channel;
    private readonly WebApplication? _app;
    private readonly MessageStreamingService _messageStreamingService;

    public GroupChatServiceTests()
    {
        Grpc.GroupChatServer.Main.Program server = new();

        // The port number must match the port of the gRPC server.
        _channel = GrpcChannel.ForAddress("https://localhost:7186");
    }

    public async Task SimulateConversation()
    {
        var client1 = new Grpc.GroupChatClient.GroupChat.GroupChatClient(_channel);
        var client2 = new Grpc.GroupChatClient.GroupChat.GroupChatClient(_channel);

        var client1Stream = client1.EnterChat(new Grpc.GroupChatClient.Client { Name = "Client1", Group = "Group1" });
        var client2Stream = client2.EnterChat(new Grpc.GroupChatClient.Client { Name = "Client2", Group = "Group1" });

        await client1.SendMessageAsync(new Grpc.GroupChatClient.GroupChatMessage { Message = "Hello from Client1", Group = "Group1" });
        await client2.SendMessageAsync(new Grpc.GroupChatClient.GroupChatMessage { Message = "Hello from Client2", Group = "Group1" });

        var client1Messages = await ReadMessagesAsync(client1Stream.ResponseStream);
        var client2Messages = await ReadMessagesAsync(client2Stream.ResponseStream);

        Assert.Contains(client1Messages, m => m.Message == "Hello from Client1");
        Assert.Contains(client1Messages, m => m.Message == "Hello from Client2");
        Assert.Contains(client2Messages, m => m.Message == "Hello from Client1");
        Assert.Contains(client2Messages, m => m.Message == "Hello from Client2");
    }

    private async Task<List<Grpc.GroupChatClient.GroupChatMessage>> ReadMessagesAsync(IAsyncStreamReader<Grpc.GroupChatClient.GroupChatMessage> responseStream)
    {
        var messages = new List<Grpc.GroupChatClient.GroupChatMessage>();
        while (await responseStream.MoveNext())
        {
            messages.Add(responseStream.Current);
        }
        return messages;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _app.DisposeAsync();
    }
}
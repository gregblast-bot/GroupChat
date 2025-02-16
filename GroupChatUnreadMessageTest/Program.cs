using Grpc.Core;
using Grpc.Net.Client;
using Grpc.GroupChatServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GroupChatServer.Tests;

public class GroupChatUnreadMessage
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        // The port number must match the port of the gRPC server.
        GrpcChannel? _channel = GrpcChannel.ForAddress("http://localhost:5156");

        var alice = new Grpc.GroupChatClient.GroupChat.GroupChatClient(_channel);
        var chad = new Grpc.GroupChatClient.GroupChat.GroupChatClient(_channel);
        var bob = new Grpc.GroupChatClient.GroupChat.GroupChatClient(_channel);

        var aliceStream = alice.EnterChat(new Grpc.GroupChatClient.Client { Name = "Alice", Group = "Group1" }); // Alice enters the chatroom
        Thread.Sleep(1000); // Sleep for 1 second
        alice.SendMessage(new Grpc.GroupChatClient.GroupChatMessage { Message = "Hello from Alice!", Group = "Group1" }); // Alice sends a message
        Thread.Sleep(5000); // Sleep for 5 seconds while Chad and Bob enter the chatroom
        var chadStream = chad.EnterChat(new Grpc.GroupChatClient.Client { Name = "Chad", Group = "Group1" }); // Chad enters the chatroom
        Thread.Sleep(1000); // Sleep for 1 second
        var bobStream = bob.EnterChat(new Grpc.GroupChatClient.Client { Name = "Bob", Group = "Group1" }); // Bob enters the chatroom
    }
}

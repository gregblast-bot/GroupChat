using Grpc.Core;
using System.Xml.Linq;

namespace Grpc.GroupChatServer.Services;

/// <summary>
/// This service processes inputs from the client and 
/// relays those messages to other clients in the group chat.
/// </summary>
public class GroupChatService : GroupChat.GroupChatBase
{
    private readonly ILogger<GroupChatService> _logger;
    private readonly MessageStreamingService _streamingService;
    
    public GroupChatService(ILogger<GroupChatService> logger, MessageStreamingService streamingService)
    {
        _logger = logger;
        _streamingService = streamingService;
    }

    /// <summary>
    /// Subscribe to a chatroom and notify relevant clients.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="responseStream"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task EnterChat(Client request, IServerStreamWriter<GroupChatMessage> responseStream, ServerCallContext context)
    {
        string name = request.Name;
        string group = request.Group;

        _logger.LogInformation($"{name} has entered the {group} chatroom.");

        _streamingService.Subscribe(group, responseStream);

        // Send unread messages to the new client.
        var unreadMessages = _streamingService.GetUnreadMessages(group);
        foreach (var message in unreadMessages)
        {
            _logger.LogInformation($"{name} received unread message... {message}.");
            await responseStream.WriteAsync(message);
        }

        await _streamingService.SendMessage(new GroupChatMessage { Message = $"User with name {name} entered the {group} chatroom.", Group = request.Group });

        waitForMessages();
    }

    /// <summary>
    /// Send a message to the chatroom.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns>A Task of MessageResponse.</returns>
    public override async Task<MessageResponse> SendMessage(GroupChatMessage request, ServerCallContext context)
    {
        _logger.LogInformation($"{request.Message} is received by {request.Group}.");

        await _streamingService.SendMessage(request);
        return new MessageResponse() { Ok = true };
    }

    /// <summary>
    /// Wait for messages to be sent to the client.
    /// </summary>
    private void waitForMessages()
    {
        while (true)
        {
            Thread.Sleep(10000);
        }
    }
}

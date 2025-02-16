using Grpc.Core;
using System.Collections.Concurrent;

namespace Grpc.GroupChatServer.Services;

/// <summary>
/// This service processes inputs from the client for the server.
/// </summary>
public class MessageStreamingService
{
    private readonly ConcurrentDictionary<string, List<IServerStreamWriter<GroupChatMessage>>> _groupStreams;
    private readonly ConcurrentDictionary<string, List<GroupChatMessage>> _groupMessages;

    public MessageStreamingService()
    {
        _groupStreams = new ConcurrentDictionary<string, List<IServerStreamWriter<GroupChatMessage>>>();
        _groupMessages = new ConcurrentDictionary<string, List<GroupChatMessage>>();
    }

    /// <summary>
    /// Subscribe to a chatroom.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="stream"></param>
    public void Subscribe(string group, IServerStreamWriter<GroupChatMessage> stream)
    {
        // Create a new group stream if it doesn't exist.
        if (!_groupStreams.ContainsKey(group))
        {
            _groupStreams[group] = new List<IServerStreamWriter<GroupChatMessage>>();
        }
        _groupStreams[group].Add(stream);
    }

    /// <summary>
    /// Send a message to the chatroom.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessage(GroupChatMessage message)
    {
        // Store the message in the group's message list.
        if (!_groupMessages.ContainsKey(message.Group))
        {
            _groupMessages[message.Group] = new List<GroupChatMessage>();
        }
        _groupMessages[message.Group].Add(message);

        // Only send the message to the group if the client is a member of said group.
        if (_groupStreams.TryGetValue(message.Group, out var streams))
        {
            await Parallel.ForEachAsync(streams, async (stream, ctx) =>
            {
                await stream.WriteAsync(message);
            });
        }
    }

    /// <summary>
    /// Get unread messages for a user that has yet to join the chatroom.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public List<GroupChatMessage> GetUnreadMessages(string group)
    {
        if (_groupMessages.TryGetValue(group, out var messages))
        {
            return messages;
        }
        return new List<GroupChatMessage>();
    }
}

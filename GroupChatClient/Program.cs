using Grpc.Core;
using Grpc.Net.Client;
using Grpc.GroupChatClient;

/// <summary>
/// This program processes inputs from the client and sends messages to the server.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("https://localhost:7186");

        // Set a client on the specified channel.
        var client = new GroupChat.GroupChatClient(channel);

        // Prompt for a user name and a chat room to join.
        Console.Write("Name: ");
        var userName = Console.ReadLine();

        Console.Write("Chatroom: ");
        var chatroomName = Console.ReadLine();

        // Enter the chat room.
        var reply = client.EnterChat(new Client { Name = userName, Group = chatroomName });

        StartWriteThread(reply);

        ProcessInput(client, userName, chatroomName);
    }

    /// <summary>
    /// Create a thread to read messages from the server.
    /// </summary>
    public static void StartWriteThread(AsyncServerStreamingCall<GroupChatMessage> reply)
    {
        var writeThread = new Thread(async () =>
        {
            while (true)
            {
                await foreach (var message in reply.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(message.Message);
                }
            }
        });

        writeThread.Start();
    }

    /// <summary>
    /// Continue processing inputs from the client while the client is active.
    /// </summary>
    public static void ProcessInput(GroupChat.GroupChatClient client, string userName, string chatroomName)
    {
        while (true)
        {
            var message = Console.ReadLine();

            Console.SetCursorPosition(0, Console.CursorTop - 1);
            clearLine();

            client.SendMessage(new GroupChatMessage { Message = $"{DateTime.Now}, {chatroomName}, {userName}: {message}", Group = chatroomName });
        }
    }

    /// <summary>
    /// Clears the current line in the console and resets the cursor position.
    /// </summary>
    static void clearLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
}

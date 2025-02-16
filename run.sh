# Start the server in the background
dotnet run --project GroupChatServer &

# Wait for the server to start
sleep 5

# Start multiple clients
dotnet run --project GroupChatClient

# Wait for all background processes to finish
wait
import subprocess
import time
import sys

def start_server():
    if sys.platform == "win32":
        server_process = subprocess.Popen(['dotnet', 'run', '--project', 'GroupChatServer'], creationflags=subprocess.CREATE_NEW_CONSOLE)
    else:
        server_process = subprocess.Popen(['xterm', '-e', 'dotnet run --project GroupChatServer'])
    time.sleep(5)  # Wait for the server to start
    return server_process

def start_client():
    if sys.platform == "win32":
        client_process = subprocess.Popen(['dotnet', 'run', '--project', 'GroupChatUnreadMessageTest'], creationflags=subprocess.CREATE_NEW_CONSOLE)
    else:
        client_process = subprocess.Popen(['xterm', '-e', 'dotnet run --project GroupChatUnreadMessageTest'])
    return client_process

def main():
    server_process = start_server()
    client_process = start_client()

    time.sleep(60) # Wait one minute before terminating

    server_process.terminate()
    client_process.terminate()

if __name__ == '__main__':
    main()
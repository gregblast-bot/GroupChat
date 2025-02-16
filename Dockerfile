# Use the official .NET SDK image as a base image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the csproj files and restore any dependencies
COPY *.sln ./
COPY GroupChatServer/*.csproj ./GroupChatServer/
COPY GroupChatClient/*.csproj ./GroupChatClient/
COPY GroupChatUnreadMessageTest/*.csproj ./GroupChatUnreadMessageTest/
COPY GroupChatDifferentGroupTest/*.csproj ./GroupChatDifferentGroupTest/
RUN dotnet restore

# Copy the rest of the application code
COPY GroupChatServer/. ./GroupChatServer/
COPY GroupChatClient/. ./GroupChatClient/
COPY GroupChatUnreadMessageTest/. ./GroupChatUnreadMessageTest/
COPY GroupChatDifferentGroupTest/. ./GroupChatDifferentGroupTest/

# Build the application
RUN dotnet build

# Use the official .NET SDK image as a base image for the final stage
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set the working directory
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build-env /app/GroupChatServer/bin/Debug/net9.0/ ./
COPY --from=build-env /app/GroupChatClient/bin/Debug/net9.0/ ./

# Start the server application, only can have one entrypoint and server alone is useless...
ENTRYPOINT ["dotnet", "GroupChatServer.dll"]

##################################################################################################################
# Things I've tried
##################################################################################################################
# Copy the shell script
#COPY run.sh /app/

# Make the shell script executable
#RUN chmod +x /app/run.sh

# Set the entry point to the shell script to run a client and server
#ENTRYPOINT ["/bin/bash", "/app/run.sh"]

# Install Python and xterm
#RUN apt-get update && \
    #apt-get install -y python3 python3-pip xterm && \
    #apt-get clean && \
    #rm -rf /var/lib/apt/lists/*

# Copy Python scripts
#COPY *.py /app/

# Start the application and run the Python scripts... not able to open a new server console to see logs
#CMD ["sh", "-c", "python3 /app/GroupChatDifferentGroupTest.py"]

# Start the server application, only can have one entrypoint and server alone is useless...
#ENTRYPOINT ["dotnet", "GroupChatServer.dll"]

# Start the client application, useless without the server...
#ENTRYPOINT ["dotnet", "GroupChatClient.dll"]
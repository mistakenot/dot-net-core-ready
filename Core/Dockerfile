FROM microsoft/dotnet:latest
COPY ./ /app
WORKDIR /app
 
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
  
CMD dotnet run --urls http://0.0.0.0:$PORT
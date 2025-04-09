FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . . 

WORKDIR /app/src/Program.cs
RUN dotnet publish -c Debug -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Program.cs.dll"]
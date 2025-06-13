FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY *.sln

COPY /app/*.csproj./app/

COPY /app/app

WORKDIR /app

RUN dotnet restore

RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0

ENV ASPNETCORE_URLS=http://+:5000

ENV ASPNETCORE_ENVIRONMENT=Development

WORKDIR /app

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ScheduleApp.dll"]

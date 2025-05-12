# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as separate steps for better caching
COPY ScheduleApp/ScheduleApp.csproj ./ScheduleApp/
RUN dotnet restore ./ScheduleApp/ScheduleApp.csproj

# Copy everything else and build
COPY ScheduleApp/ ./ScheduleApp/
WORKDIR /app/ScheduleApp
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 80
ENTRYPOINT ["dotnet", "ScheduleApp.dll"]

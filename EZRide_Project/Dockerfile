FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# Restore and publish using the relative path inside the /src folder
RUN dotnet restore "EZRide_Project/EZRide_Project.csproj"
RUN dotnet publish "EZRide_Project/EZRide_Project.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EZRide_Project.dll"]

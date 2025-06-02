FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EZRide_Project/ ./EZRide_Project/

WORKDIR /src/EZRide_Project

RUN dotnet restore "EZRide_Project.csproj"
RUN dotnet publish "EZRide_Project.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EZRide_Project.dll"]

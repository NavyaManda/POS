FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/APIGateway.API/APIGateway.API.csproj", "APIGateway.API/"]
RUN dotnet restore "APIGateway.API/APIGateway.API.csproj"

COPY . .
WORKDIR "/src/APIGateway.API"
RUN dotnet build "APIGateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIGateway.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "APIGateway.API.dll"]

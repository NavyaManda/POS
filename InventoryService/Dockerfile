FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/InventoryService.API/InventoryService.API.csproj", "src/InventoryService.API/"]
RUN dotnet restore "src/InventoryService.API/InventoryService.API.csproj"

COPY . .
WORKDIR "/src/src/InventoryService.API"
RUN dotnet build "InventoryService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "InventoryService.API.dll"]

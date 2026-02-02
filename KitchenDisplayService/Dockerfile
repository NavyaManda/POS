FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/KitchenDisplayService.API/KitchenDisplayService.API.csproj", "src/KitchenDisplayService.API/"]
RUN dotnet restore "src/KitchenDisplayService.API/KitchenDisplayService.API.csproj"

COPY . .
WORKDIR "/src/src/KitchenDisplayService.API"
RUN dotnet build "KitchenDisplayService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KitchenDisplayService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "KitchenDisplayService.API.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/MenuService.API/MenuService.API.csproj", "src/MenuService.API/"]
RUN dotnet restore "src/MenuService.API/MenuService.API.csproj"

COPY . .
WORKDIR "/src/src/MenuService.API"
RUN dotnet build "MenuService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MenuService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "MenuService.API.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MenuService/src/MenuService.API/MenuService.API.csproj", "MenuService.API/"]
RUN dotnet restore "MenuService.API/MenuService.API.csproj"

COPY MenuService/src/MenuService.API/ MenuService.API/
WORKDIR "/src/MenuService.API"
RUN dotnet build "MenuService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MenuService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002
ENTRYPOINT ["dotnet", "MenuService.API.dll"]

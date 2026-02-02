FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/LoyaltyService.API/LoyaltyService.API.csproj", "src/LoyaltyService.API/"]
RUN dotnet restore "src/LoyaltyService.API/LoyaltyService.API.csproj"

COPY . .
WORKDIR "/src/src/LoyaltyService.API"
RUN dotnet build "LoyaltyService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoyaltyService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "LoyaltyService.API.dll"]

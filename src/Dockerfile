FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Adapters/Driver/FoodTotem.Demand.API/FoodTotem.Demand.API.csproj", "Adapters/Driver/FoodTotem.Demand.API/"]
COPY ["Core/FoodTotem.Demand.UseCase/FoodTotem.Demand.UseCase.csproj", "Core/FoodTotem.Demand.UseCase/"]
COPY ["Core/FoodTotem.Demand.Domain/FoodTotem.Demand.Domain.csproj", "Core/FoodTotem.Demand.Domain/"]
COPY ["Core/FoodTotem.Domain.Core/FoodTotem.Domain.Core.csproj", "Core/FoodTotem.Domain.Core/"]
COPY ["Core/FoodTotem.Data.Core/FoodTotem.Data.Core.csproj", "Core/FoodTotem.Data.Core/"]
COPY ["Adapters/Driven/FoodTotem.Demand.Gateways.MongoDB/FoodTotem.Demand.Gateways.MongoDB.csproj", "Adapters/Driven/FoodTotem.Demand.Gateways.MongoDB/"]
COPY ["Adapters/Driven/FoodTotem.Demand.Gateways.Http/FoodTotem.Demand.Gateways.Http.csproj", "Adapters/Driven/FoodTotem.Demand.Gateways.Http/"]

RUN dotnet restore "Adapters/Driver/FoodTotem.Demand.API/FoodTotem.Demand.API.csproj"
COPY . .
WORKDIR "/src/Adapters/Driver/FoodTotem.Demand.API"
RUN dotnet build "FoodTotem.Demand.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodTotem.Demand.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodTotem.Demand.API.dll"]
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY NetCoreMicroserviceSample.Api/*.csproj NetCoreMicroserviceSample.Api/
RUN cd NetCoreMicroserviceSample.Api && dotnet restore

# copy everything else and build app
COPY . .
RUN cd NetCoreMicroserviceSample.Api && dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/NetCoreMicroserviceSample.Api/out ./
ENTRYPOINT ["dotnet", "NetCoreMicroserviceSample.Api.dll"]

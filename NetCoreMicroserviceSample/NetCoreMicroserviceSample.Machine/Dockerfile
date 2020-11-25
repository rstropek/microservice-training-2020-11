FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY NetCoreMicroserviceSample.Machine/*.csproj NetCoreMicroserviceSample.Machine/
RUN cd NetCoreMicroserviceSample.Machine && dotnet restore

# copy everything else and build app
COPY . .
RUN cd NetCoreMicroserviceSample.Machine && dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/NetCoreMicroserviceSample.Machine/out ./
ENTRYPOINT ["dotnet", "NetCoreMicroserviceSample.Machine.dll"]

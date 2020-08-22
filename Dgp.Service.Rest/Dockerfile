# Source of original Dockerfile: https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files/

# Run from the solution directory, i.e. directory that holds DiscGolfPad.sln
# Build: docker build -t dgp-api:local . --file Dgp.Service.Rest/Dockerfile
# Run: docker run -d -p 8080:80 --name dgp-api dgp-api:local
# Windows nanoserver images won't work correctly using localhost, use the following to get container IP 
#	docker inspect -f "{{ .NetworkSettings.Networks.nat.IPAddress }}" dgp-api

# Cleanup: docker system prune

# Windows build image
#FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
## Linux build image
FROM microsoft/dotnet:2.2.100-sdk-stretch AS build

WORKDIR ./
COPY ./DiscGolfPad.sln ./
COPY ./nuget.config ./

# Copy requried projects and dotnet restore
COPY ./Dgp.Domain.Core/Dgp.Domain.Core.csproj ./Dgp.Domain.Core/Dgp.Domain.Core.csproj
COPY ./Dgp.Data.AzureTables/Dgp.Data.AzureTables.csproj ./Dgp.Data.AzureTables/Dgp.Data.AzureTables.csproj
COPY ./Dgp.Service.Rest/Dgp.Service.Rest.csproj ./Dgp.Service.Rest/Dgp.Service.Rest.csproj
RUN dotnet restore ./Dgp.Domain.Core
RUN dotnet restore ./Dgp.Data.AzureTables
RUN dotnet restore ./Dgp.Service.Rest
COPY ./ ./

# Build and publish
RUN dotnet build ./Dgp.Service.Rest/Dgp.Service.Rest.csproj -c Release
RUN dotnet publish "./Dgp.Service.Rest/Dgp.Service.Rest.csproj" -c Release -o "../dist" --no-restore

# Windows runtime image
#FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803
# Linux runtime image
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-stretch-slim
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Development
ENV EnvironmentName Development
ENTRYPOINT ["dotnet", "Dgp.Service.Rest.dll"]
COPY --from=build /dist .

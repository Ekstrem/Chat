FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# NuGet config — use only nuget.org + local packages
COPY src/nuget.config .
COPY local-nupkg/ /local-nupkg/
RUN sed -i '/<add key="github-ekstrem"/d' nuget.config && \
    sed -i '/<github-ekstrem>/,/<\/github-ekstrem>/d' nuget.config && \
    sed -i 's|../../DigiTFactory.Libraries/nupkg|/local-nupkg|g' nuget.config

# Copy csproj files and restore
COPY src/Chat.Api/Chat.Api.csproj Chat.Api/
COPY src/Chat.Application/Chat.Application.csproj Chat.Application/
COPY src/Chat.Domain/Chat.Domain.csproj Chat.Domain/
COPY src/Chat.DomainServices/Chat.DomainServices.csproj Chat.DomainServices/
COPY src/Chat.InternalContracts/Chat.InternalContracts.csproj Chat.InternalContracts/
COPY src/Chat.Storage/Chat.Storage.csproj Chat.Storage/
RUN dotnet restore Chat.Api/Chat.Api.csproj

# Copy source and build
COPY src/ .
RUN dotnet publish Chat.Api/Chat.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
ARG DEPLOYMENT_PROFILE=Single
ENV ASPNETCORE_ENVIRONMENT=${DEPLOYMENT_PROFILE}
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Chat.Api.dll"]

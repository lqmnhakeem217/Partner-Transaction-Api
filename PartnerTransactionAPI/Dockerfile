# ARG for cross-platform base images
ARG BASE_IMAGE=mcr.microsoft.com/dotnet/aspnet:6.0
ARG SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:6.0

# Base runtime image
FROM ${BASE_IMAGE} AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM ${SDK_IMAGE} AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore
COPY ["PartnerTransactionAPI/PartnerTransactionAPI.csproj", "PartnerTransactionAPI/"]
RUN dotnet restore "./PartnerTransactionAPI/PartnerTransactionAPI.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/PartnerTransactionAPI"
RUN dotnet build "./PartnerTransactionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PartnerTransactionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app

# Create Logs folder for log4net
RUN mkdir -p /app/Logs
VOLUME ["/app/Logs"]

COPY --from=publish /app/publish .
COPY PartnerTransactionAPI/log4net.config ./log4net.config

ENTRYPOINT ["dotnet", "PartnerTransactionAPI.dll"]

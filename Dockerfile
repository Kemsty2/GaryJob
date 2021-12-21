#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/GaryJob.Api/GaryJob.Api.csproj", "GaryJob.Api/"]
COPY ["src/GaryJob.Core/GaryJob.Core.csproj", "GaryJob.Core/"]
COPY ["src/GaryJob.Persistence/GaryJob.Persistence.csproj", "GaryJob.Persistence/"]
COPY ["src/GaryJob.Workflows/GaryJob.Workflows.csproj", "GaryJob.Workflows/"]
RUN dotnet restore "src/GaryJob.Api/GaryJob.Api.csproj"
COPY . .
WORKDIR "/src/GaryJob.Api"
RUN dotnet build "GaryJob.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GaryJob.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GaryJob.Api.dll"]
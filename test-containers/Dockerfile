#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HelmChartTestWeb/Server/HelmChartTestWeb.Server.csproj", "HelmChartTestWeb/Server/"]
COPY ["HelmChartTestWeb/Client/HelmChartTestWeb.Client.csproj", "HelmChartTestWeb/Client/"]
COPY ["HelmChartTestWeb/Shared/HelmChartTestWeb.Shared.csproj", "HelmChartTestWeb/Shared/"]
RUN dotnet restore "HelmChartTestWeb/Server/HelmChartTestWeb.Server.csproj"
COPY . .
WORKDIR "/src/HelmChartTestWeb/Server"
RUN dotnet build "HelmChartTestWeb.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelmChartTestWeb.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelmChartTestWeb.Server.dll"]
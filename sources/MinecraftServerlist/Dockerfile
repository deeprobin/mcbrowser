#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.2-alpine3.14 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build
WORKDIR /src
COPY ["sources/MinecraftServerlist/MinecraftServerlist.csproj", "sources/MinecraftServerlist/"]
COPY ["sources/MinecraftServerlist.Common/MinecraftServerlist.Common.csproj", "sources/MinecraftServerlist.Common/"]
COPY ["sources/MinecraftServerlist.PublicApi/MinecraftServerlist.PublicApi.csproj", "sources/MinecraftServerlist.PublicApi/"]
COPY ["sources/MinecraftServerlist.BlazorFrontend/MinecraftServerlist.BlazorFrontend.csproj", "sources/MinecraftServerlist.BlazorFrontend/"]
COPY ["sources/MinecraftServerlist.InternalApi.Client/MinecraftServerlist.InternalApi.Client.csproj", "sources/MinecraftServerlist.InternalApi.Client/"]
COPY ["sources/MinecraftServerlist.InternalApi.Common/MinecraftServerlist.InternalApi.Common.csproj", "sources/MinecraftServerlist.InternalApi.Common/"]
COPY ["sources/MinecraftServerlist.Data/MinecraftServerlist.Data.csproj", "sources/MinecraftServerlist.Data/"]
COPY ["sources/MinecraftServerlist.InternalApi/MinecraftServerlist.InternalApi.csproj", "sources/MinecraftServerlist.InternalApi/"]
COPY ["sources/MinecraftServerlist.Services/MinecraftServerlist.Services.csproj", "sources/MinecraftServerlist.Services/"]
COPY ["sources/MinecraftServerlist.Ping/MinecraftServerlist.Ping.csproj", "sources/MinecraftServerlist.Ping/"]
COPY ["sources/MinecraftServerlist.Vote/MinecraftServerlist.Votifier.csproj", "sources/MinecraftServerlist.Vote/"]
COPY ["sources/MinecraftServerlist.Data.Development/MinecraftServerlist.Data.Development.csproj", "sources/MinecraftServerlist.Data.Development/"]
RUN dotnet restore "sources/MinecraftServerlist/MinecraftServerlist.csproj"
COPY . .
WORKDIR "/src/sources/MinecraftServerlist"
RUN dotnet build "MinecraftServerlist.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinecraftServerlist.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinecraftServerlist.dll"]
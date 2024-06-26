FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7268
EXPOSE 5227
EXPOSE 27017

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Binbi.Parser.API/Binbi.Parser.API.csproj", "Binbi.Parser.API/"]
RUN dotnet restore "Binbi.Parser.API/Binbi.Parser.API.csproj"
COPY . .
WORKDIR "/src/Binbi.Parser.API"
RUN dotnet build "Binbi.Parser.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Binbi.Parser.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Binbi.Parser.API.dll"]

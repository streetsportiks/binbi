FROM python:3.10.6

ENV PYTHONDONTWRITEBYCODE=1
ENV PYTHONUNBUFFERED=1

WORKDIR /code

RUN pip install --upgrade pip
COPY requrement.txt /code/
RUN pip install -r requrement.txt

COPY . /code/

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7268
EXPOSE 5227
EXPOSE 27017

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["binbi/Binbi.Parser/Binbi.Parser.gRPC/Binbi.Parser.gRPC.csproj", "Binbi.Parser.gRPC/"]
COPY ["binbi/Binbi.Parser/Binbi.Parser.DB/Binbi.Parser.DB.csproj", "Binbi.Parser.DB/"]
COPY ["binbi/Binbi.Parser/Binbi.Parser.Common/Binbi.Parser.Common.csproj", "Binbi.Parser.Common/"]
COPY ["binbi/Binbi.Parser/Binbi.Parser.gRPC/Protos/parser.proto", "Binbi.Parser.gRPC/Protos/parser.proto"]
COPY ["binbi/Binbi.Parser/Binbi.Parser.gRPC/Protos/report.proto", "Binbi.Parser.gRPC/Protos/report.proto"]

RUN dotnet restore "Binbi.Parser.gRPC/Binbi.Parser.gRPC.csproj"

COPY binbi /src/binbi

WORKDIR "/src/binbi/Binbi.Parser/Binbi.Parser.gRPC"

RUN dotnet build "Binbi.Parser.gRPC.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Binbi.Parser.gRPC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Binbi.Parser.gRPC.dll"]

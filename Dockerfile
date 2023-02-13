FROM ghcr.io/shimat/opencvsharp/ubuntu22-dotnet6-opencv4.7.0:20230114 AS base
WORKDIR /app
ENV DOTNET_ENVIRONMENT "PRODUCTION"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["Rzd.ChatBot/Rzd.ChatBot.csproj", "Rzd.ChatBot/"]
RUN dotnet restore "Rzd.ChatBot/Rzd.ChatBot.csproj"
#RUN dotnet add 'Rzd.ChatBot/Rzd.ChatBot.csproj' package OpenCvSharp4_.runtime.ubuntu.20.04-x64 --version 4.7.0.20230115
COPY . .
WORKDIR "/src/Rzd.ChatBot"
RUN dotnet build "Rzd.ChatBot.csproj" --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rzd.ChatBot.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY Rzd.ChatBot/appsettings.Production.json .
#COPY Rzd.ChatBot/appsettings.Development.json ./Rzd.ChatBot/appsettings.Production.json
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rzd.ChatBot.dll"]

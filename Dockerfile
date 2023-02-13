FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV DOTNET_ENVIRONMENT "PRODUCTION"

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Rzd.ChatBot/Rzd.ChatBot.csproj", "Rzd.ChatBot/"]
RUN dotnet restore "Rzd.ChatBot/Rzd.ChatBot.csproj"
COPY . .
WORKDIR "/src/Rzd.ChatBot"
RUN dotnet build "Rzd.ChatBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rzd.ChatBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY localization.yml .
COPY appsettings.Production.json .
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rzd.ChatBot.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Minecraft.Web.API/Minecraft.Web.API.csproj", "Minecraft.Web.API/"]
RUN dotnet restore "Minecraft.Web.API/Minecraft.Web.API.csproj"
COPY . .
WORKDIR "/src/Minecraft.Web.API"
RUN dotnet build "Minecraft.Web.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Minecraft.Web.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Minecraft.Web.API.dll"]

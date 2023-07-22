FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/IdentityDotnet7.Api/IdentityDotnet7.Api.csproj", "src/IdentityDotnet7.Api/"]
RUN dotnet restore "src/IdentityDotnet7.Api/IdentityDotnet7.Api.csproj"
COPY . .
WORKDIR "/src/src/IdentityDotnet7.Api"
RUN dotnet build "IdentityDotnet7.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityDotnet7.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityDotnet7.Api.dll"]

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/out .

# OPTIONAL: Ensure appsettings.json is copied (if not already in out/)
COPY appsettings.json /app/

ENV ASPNETCORE_URLS=http://+:5175
EXPOSE 5175
ENTRYPOINT ["dotnet", "wizard-svc.dll"]

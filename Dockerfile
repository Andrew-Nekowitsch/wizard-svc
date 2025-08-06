# Use a specific preview version
FROM mcr.microsoft.com/dotnet/sdk:9.0.100-preview.6 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0.0-preview.6 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:5175
EXPOSE 5175

ENTRYPOINT ["dotnet", "wizard-svc.dll"]

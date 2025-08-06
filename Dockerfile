# Match SDK version 9.0.201
FROM mcr.microsoft.com/dotnet/sdk:9.0.201 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

# Match runtime to 9.0.3
FROM mcr.microsoft.com/dotnet/aspnet:9.0.3 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:5175
EXPOSE 5175

ENTRYPOINT ["dotnet", "wizard-svc.dll"]

# Use the official .NET 9.0 SDK (Preview) image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use the runtime image for smaller final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Set environment variables if needed
ENV ASPNETCORE_URLS=http://+:5175
EXPOSE 5175

ENTRYPOINT ["dotnet", "wizard-svc.dll"]
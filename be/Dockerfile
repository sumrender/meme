# Use the official .NET runtime image for .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Final image to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app .
COPY ./Memes /app/Memes
ENTRYPOINT ["dotnet", "Backend.dll"]



# docker build -t meme-gen-api .
# docker run -d -p 8080:8080 --name meme-gen-api-container meme-gen-api
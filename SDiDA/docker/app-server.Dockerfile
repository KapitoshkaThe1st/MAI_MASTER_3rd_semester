# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

# COPY . /build/

# RUN ls /build/

# WORKDIR /build
# RUN dotnet build --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0

# COPY --from=builder ApplicationAPI/bin/ /app/bin
COPY ./published/ /app/

EXPOSE 5001

ENTRYPOINT ["dotnet", "/app/ApplicationAPI.dll"]
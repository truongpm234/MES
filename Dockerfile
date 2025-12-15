# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy file Solution
COPY AMMS.sln .

# 2. Copy các file .csproj
COPY AMMS.API/*.csproj ./AMMS.API/
COPY AMMS.Application/*.csproj ./AMMS.Application/
COPY AMMS.Domain/*.csproj ./AMMS.Domain/
COPY AMMS.Infrastructure/*.csproj ./AMMS.Infrastructure/
COPY AMMS.Shared/*.csproj ./AMMS.Shared/

# 3. Restore dependencies
RUN dotnet restore

# 4. Copy toàn bộ source code
COPY . .

# 5. Build project
WORKDIR /src/AMMS.API
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy các file đã publish
COPY --from=publish /app/publish .

# ===> FIX QUAN TRỌNG Ở ĐÂY <===
# Tạo một file appsettings.json rỗng để thỏa mãn điều kiện (optional: false) trong Program.cs
# Vì trên Render file này thường không tồn tại do gitignore.
RUN echo "{}" > appsettings.json

# Cấu hình môi trường
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "AMMS.API.dll"]
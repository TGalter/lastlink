FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["src/AdvanceRequests.Api/AdvanceRequests.Api.csproj", "src/AdvanceRequests.Api/"]
COPY ["src/AdvanceRequests.Application/AdvanceRequests.Application.csproj", "src/AdvanceRequests.Application/"]
COPY ["src/AdvanceRequests.Domain/AdvanceRequests.Domain.csproj", "src/AdvanceRequests.Domain/"]
COPY ["src/AdvanceRequests.Infrastructure/AdvanceRequests.Infrastructure.csproj", "src/AdvanceRequests.Infrastructure/"]

RUN dotnet restore "src/AdvanceRequests.Api/AdvanceRequests.Api.csproj"

COPY . .
RUN dotnet publish "src/AdvanceRequests.Api/AdvanceRequests.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AdvanceRequests.Api.dll"]
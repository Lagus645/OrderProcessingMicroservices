FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY . .
RUN dotnet restore "src/NotificationService/NotificationService.WebApi/NotificationService.WebApi.csproj"
RUN dotnet publish "src/NotificationService/NotificationService.WebApi/NotificationService.WebApi.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5200
ENTRYPOINT ["dotnet", "NotificationService.WebApi.dll"]

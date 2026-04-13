FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CaseInvestigationManagementSystem.csproj", "./"]

RUN dotnet restore "CaseInvestigationManagementSystem.csproj"
RUN apt-get update && apt-get install -y libgssapi-krb5-2

COPY . .

WORKDIR /src
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CaseInvestigationManagementSystem.dll"]
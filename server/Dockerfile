#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
#FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY . ./

RUN dotnet restore "./preference.csproj"
RUN dotnet publish "preference.csproj" -c Release -o out

#FROM microsoft/dotnet:aspnetcore-runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "preference.dll"]

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
#WORKDIR /app
#EXPOSE 5000
#ENV ASPNETCORE_URLS=http://*:5000

#FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
#WORKDIR /src
#COPY ["preference.csproj", "./"]
#RUN dotnet restore "./preference.csproj"
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "preference.csproj" -c Release -o /app/build

#FROM build AS publish
#RUN dotnet publish "preference.csproj" -c Release -o /app/publish

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "preference.dll"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

COPY . ./
WORKDIR /Quotes

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build-env /Quotes/out .
ENTRYPOINT ["dotnet", "Quotes.dll"]
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
COPY . .
RUN dotnet publish -c Release ./src/sql-d.cli/ -o ./output -r linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final 
EXPOSE 50110
WORKDIR /home/app
COPY --from=build /build/output .
ENTRYPOINT ["dotnet", "SqlD.CLI.dll"]

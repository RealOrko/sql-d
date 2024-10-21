FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
COPY . .
RUN dotnet publish -c Release ./src/SqlD.CLI/ -o ./output -r linux-x64

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final 
WORKDIR /home/app
COPY --from=build /build/output .
ENTRYPOINT ["dotnet", "SqlD.CLI.dll"]

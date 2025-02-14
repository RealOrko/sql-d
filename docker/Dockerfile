FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /code
ADD . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
  dotnet publish -c Release -o /output /code/src/sql-d.ui/SqlD.UI.csproj /p:GeneratePackageOnBuild=false
COPY docker/appsettings.json /output/appsettings.json

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS run
RUN mkdir -p /var/lib/sql-d/
RUN chown app:app /var/lib/sql-d/
#USER app
EXPOSE 5000 50095 50100
ENV ASPNETCORE_URLS=http://+:5000
WORKDIR /app
COPY --from=build --chown=app:app /output /app
ENTRYPOINT ["/app/SqlD.UI"]

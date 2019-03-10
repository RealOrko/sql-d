FROM microsoft/dotnet:sdk AS build-env

WORKDIR /app

RUN apt-get update
COPY ./src/ /app/src/
RUN mkdir /app/build
RUN dotnet publish /app/src/sql-d.start.linux-x64/SqlD.Start.linux-x64.csproj -r linux-x64 -c Release -o /app/build/ --self-contained

FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000
COPY --from=build-env /app/build/ /app/
ENTRYPOINT ["/app/SqlD.Start.linux-x64"]

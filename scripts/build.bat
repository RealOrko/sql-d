@ECHO off

ECHO.
ECHO [SQL-D]:BUILD/
ECHO.

dotnet restore .\tests\sqld\SqlD.Tests.csproj
dotnet build .\tests\sqld\SqlD.Tests.csproj
dotnet test .\tests\sqld\SqlD.Tests.csproj

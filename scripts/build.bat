@ECHO off

ECHO.
ECHO [SQL-D]:BUILD/
ECHO.

dotnet restore .\tests\sql-d\SqlD.Tests.csproj
dotnet build .\tests\sql-d\SqlD.Tests.csproj
dotnet test .\tests\sql-d\SqlD.Tests.csproj

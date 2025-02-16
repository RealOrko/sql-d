@ECHO off

ECHO.
ECHO [SQL-D]:BUILD/SQLD/WINDOWS
ECHO.

dotnet restore .\tests\sql-d\SqlD.Tests.csproj
dotnet build .\tests\sql-d\SqlD.Tests.csproj
dotnet test .\tests\sql-d\SqlD.Tests.csproj

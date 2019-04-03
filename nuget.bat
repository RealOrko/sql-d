@ECHO off

CALL kill.bat

ECHO.
ECHO [SQL-D]:PUBLISH-NUGET/
ECHO.

SET VERSION=1.0.4

dotnet nuget push ./build/sql-d/sql-d.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json -k %NUGET_API_PUBLISH_KEY%

dotnet nuget push ./build/sql-d/sql-d.start.linux-x64.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json -k %NUGET_API_PUBLISH_KEY%

dotnet nuget push ./build/sql-d/sql-d.start.osx-x64.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json -k %NUGET_API_PUBLISH_KEY%

dotnet nuget push ./build/sql-d/sql-d.start.linu-x64.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json -k %NUGET_API_PUBLISH_KEY%

dotnet nuget push ./build/sql-d/sql-d.ui.linux-x64.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json -k %NUGET_API_PUBLISH_KEY%

dotnet nuget push ./build/sql-d/sql-d.ui.osx-x64.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json -k %NUGET_API_PUBLISH_KEY%

dotnet nuget push ./build/sql-d/sql-d.ui.linu-x64.%VERSION%.nupkg -s https://api.nuget.org/v3/index.json -k %NUGET_API_PUBLISH_KEY%

@ECHO off

CALL kill.bat

@ECHO on

ECHO.
ECHO [SQL-D]:PUBLISH/
ECHO.

IF EXIST .\build ( RMDIR /S /Q .\build )
IF NOT EXIST .\build ( MKDIR .\build )

SET Configuration=Release
SET TargetFramework=netcoreapp2.2

rem SET LibProjectPath=./src/sql-d/SqlD.csproj
rem dotnet build %LibProjectPath% || EXIT /B 1
rem dotnet pack %LibProjectPath% -o ../../build || EXIT /B 1
rem 	
rem SET StartLinuxX64ProjectPath=./src/sql-d.start.linux-x64/SqlD.Start.linux-x64.csproj
rem dotnet publish %StartLinuxX64ProjectPath% -r linux-x64 --self-contained || EXIT /B 1
rem dotnet pack %StartLinuxX64ProjectPath% -o ../../build --include-symbols || EXIT /B 1
rem 	
rem SET StartOsxX64ProjectPath=./src/sql-d.start.osx-x64/SqlD.Start.osx-x64.csproj
rem dotnet publish %StartOsxX64ProjectPath% -r osx-x64 --self-contained || EXIT /B 1
rem dotnet pack %StartOsxX64ProjectPath% -o ../../build --include-symbols || EXIT /B 1
rem 
rem SET StartWinX64ProjectPath=./src/sql-d.start.win-x64/SqlD.Start.win-x64.csproj
rem dotnet publish %StartWinX64ProjectPath% -r win-x64 --self-contained || EXIT /B 1
rem dotnet pack %StartWinX64ProjectPath% -o ../../build --include-symbols || EXIT /B 1
rem 
SET UIWinX64ProjectPath=./src/sql-d.ui/SqlD.UI.csproj
dotnet publish %UIWinX64ProjectPath% -r win-x64 --self-contained || EXIT /B 1
dotnet pack %UIWinX64ProjectPath% -o ../../build || EXIT /B 1

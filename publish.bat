@ECHO off

CALL kill.bat

ECHO.
ECHO [SQL-D]:PUBLISH/
ECHO.

IF EXIST .\build ( RMDIR /S /Q .\build )
IF NOT EXIST .\build ( MKDIR .\build )

SET Configuration=Release
SET TargetFramework=netcoreapp2.2

SET LibProjectPath=./src/sql-d/SqlD.csproj
dotnet build %LibProjectPath% || EXIT /B 1
dotnet pack %LibProjectPath% -o ../../build --include-symbols || EXIT /B 1
	
SET StartLinuxX64ProjectPath=./src/sql-d.start.linux-x64/SqlD.Start.linux-x64.csproj
dotnet publish %StartLinuxX64ProjectPath% -r linux-x64 --self-contained || EXIT /B 1
dotnet pack %StartLinuxX64ProjectPath% -o ../../build --include-symbols || EXIT /B 1
	
SET StartOsxX64ProjectPath=./src/sql-d.start.osx-x64/SqlD.Start.osx-x64.csproj
dotnet publish %StartOsxX64ProjectPath% -r osx-x64 --self-contained || EXIT /B 1
dotnet pack %StartOsxX64ProjectPath% -o ../../build --include-symbols || EXIT /B 1

SET StartWinX64ProjectPath=./src/sql-d.start.win-x64/SqlD.Start.win-x64.csproj
dotnet publish %StartWinX64ProjectPath% -r win-x64 --self-contained || EXIT /B 1
dotnet pack %StartWinX64ProjectPath% -o ../../build --include-symbols || EXIT /B 1

SET UIWinX64ProjectPath=./src/sql-d.ui/SqlD.UI.csproj
dotnet publish %UIWinX64ProjectPath% -r win-x64 --self-contained || EXIT /B 1
xcopy /S /Q src\sql-d.ui\bin\Release\netcoreapp2.2\Views src\sql-d.ui\bin\Release\netcoreapp2.2\win-x64\publish\Views\
dotnet pack %UIWinX64ProjectPath% -o ../../build --include-symbols || EXIT /B 1

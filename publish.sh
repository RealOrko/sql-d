#!/usr/bin/env bash

set -xe

export PATH=$PATH:./.dotnet

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

echo ''
echo '[SQL-D]:PUBLISH/'
echo ''

rm -rf $(pwd)/build
mkdir $(pwd)/build

Configuration=Release
TargetFramework=netcoreapp3.1

export LibProjectPath=./src/sql-d/SqlD.csproj
dotnet build $LibProjectPath
dotnet pack $LibProjectPath -o $(pwd)/build
	
export StartLinuxX64ProjectPath=./src/sql-d.start.linux-x64/SqlD.Start.linux-x64.csproj
dotnet publish $StartLinuxX64ProjectPath -c $Configuration -r linux-x64 -f $TargetFramework --self-contained
dotnet pack $StartLinuxX64ProjectPath -c $Configuration -o $(pwd)/build --include-symbols
	
export StartOsxX64ProjectPath=./src/sql-d.start.osx-x64/SqlD.Start.osx-x64.csproj
dotnet publish $StartOsxX64ProjectPath -c $Configuration -r osx-x64 -f $TargetFramework --self-contained
dotnet pack $StartOsxX64ProjectPath -c $Configuration -o $(pwd)/build --include-symbols

export StartWinX64ProjectPath=./src/sql-d.start.win-x64/SqlD.Start.win-x64.csproj
dotnet publish $StartWinX64ProjectPath -r win-x64 -f $TargetFramework --self-contained
dotnet pack $StartWinX64ProjectPath -o $(pwd)/build --include-symbols

dotnet add ./src/sql-d.ui/SqlD.UI.csproj package sql-d --source $(pwd)/build --package-directory $(pwd)/build 
dotnet add ./src/sql-d.ui/SqlD.UI.csproj package "sql-d.start.linux-x64" --source $(pwd)/build --package-directory $(pwd)/build
dotnet add ./src/sql-d.ui/SqlD.UI.csproj package "sql-d.start.osx-x64" --source $(pwd)/build --package-directory $(pwd)/build
dotnet add ./src/sql-d.ui/SqlD.UI.csproj package "sql-d.start.win-x64" --source $(pwd)/build --package-directory $(pwd)/build

export UIProjectPath=./src/sql-d.ui/SqlD.UI.csproj
dotnet restore $UIProjectPath -s $(pwd)/build
dotnet build $UIProjectPath
	
dotnet add ./src/sql-d.ui.linux-x64/SqlD.UI.linux-x64.csproj package sql-d --source $(pwd)/build --package-directory $(pwd)/build 
dotnet add ./src/sql-d.ui.linux-x64/SqlD.UI.linux-x64.csproj package "sql-d.start.linux-x64" --source $(pwd)/build --package-directory $(pwd)/build

export UILinuxX64ProjectPath=./src/sql-d.ui.linux-x64/SqlD.UI.linux-x64.csproj
dotnet publish $UILinuxX64ProjectPath -c $Configuration -r linux-x64 -f $TargetFramework --self-contained
cp -rf "src/sql-d.ui.linux-x64/bin/Release/${TargetFramework}/linux-x64/Views" "src/sql-d.ui.linux-x64/bin/Release/${TargetFramework}/linux-x64/publish/Views/"
cp -rf "src/sql-d.ui.linux-x64/bin/Release/${TargetFramework}/linux-x64/sql-d.start" "src/sql-d.ui.linux-x64/bin/Release/${TargetFramework}/linux-x64/publish/sql-d.start/"
dotnet pack $UILinuxX64ProjectPath -c $Configuration -o $(pwd)/build --include-symbols
	
dotnet add ./src/sql-d.ui.osx-x64/SqlD.UI.osx-x64.csproj package sql-d --source $(pwd)/build --package-directory $(pwd)/build 
dotnet add ./src/sql-d.ui.osx-x64/SqlD.UI.osx-x64.csproj package "sql-d.start.osx-x64" --source $(pwd)/build --package-directory $(pwd)/build

export UIOsxX64ProjectPath=./src/sql-d.ui.osx-x64/SqlD.UI.osx-x64.csproj
dotnet publish $UIOsxX64ProjectPath -c $Configuration -r osx-x64 -f $TargetFramework --self-contained 
cp -rf "src/sql-d.ui.osx-x64/bin/Release/${TargetFramework}/osx-x64/Views" "src/sql-d.ui.osx-x64/bin/Release/${TargetFramework}/osx-x64/publish/Views/"
cp -rf "src/sql-d.ui.osx-x64/bin/Release/${TargetFramework}/osx-x64/sql-d.start" "src/sql-d.ui.osx-x64/bin/Release/${TargetFramework}/osx-x64/publish/sql-d.start/"
dotnet pack $UIOsxX64ProjectPath -c $Configuration -o $(pwd)/build --include-symbols 

dotnet add ./src/sql-d.ui.win-x64/SqlD.UI.win-x64.csproj package sql-d --source $(pwd)/build --package-directory $(pwd)/build 
dotnet add ./src/sql-d.ui.win-x64/SqlD.UI.win-x64.csproj package "sql-d.start.win-x64" --source $(pwd)/build --package-directory $(pwd)/build

export UIWinX64ProjectPath=./src/sql-d.ui.win-x64/SqlD.UI.win-x64.csproj
dotnet publish $UIWinX64ProjectPath -c $Configuration -r win-x64 -f $TargetFramework --self-contained 
cp -rf "src/sql-d.ui.win-x64/bin/Release/${TargetFramework}/win-x64/Views" "src/sql-d.ui.win-x64/bin/Release/${TargetFramework}/win-x64/publish/Views/"
cp -rf "src/sql-d.ui.win-x64/bin/Release/${TargetFramework}/win-x64/sql-d.start" "src/sql-d.ui.win-x64/bin/Release/${TargetFramework}/win-x64/publish/sql-d.start/"
dotnet pack $UIWinX64ProjectPath -c $Configuration -o $(pwd)/build --include-symbols 

@echo off
cd /d "%~dp0"

pushd ..\..\Frontend
call npm install
call npm run build
popd

cd ..\MusicJam.Runtime
start dotnet run -c=Release

timeout 2

cd ..\MusicJam.Studio
start dotnet run -c=Release

timeout 2

cd ..\MusicJam.ApiGateway
start dotnet run -c=Release

timeout 3

start https://localhost:5000

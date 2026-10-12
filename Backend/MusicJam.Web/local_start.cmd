@echo off
cd /d "%~dp0"

pushd ..\..\Frontend
call npm install
call npm run build
popd

start dotnet run -c=Release

timeout 3

start https://localhost.musicjam.fun:5555
start https://localhost.musicjam.fun:5555/swagger

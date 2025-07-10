@echo off
cd ClientApp
dotnet build
cd ..

start cmd /k "cd ServerApp && dotnet run"
start cmd /k "cd ClientApp && bin\Debug\net9.0\ClientApp.exe"
start cmd /k "cd ClientApp && bin\Debug\net9.0\ClientApp.exe"
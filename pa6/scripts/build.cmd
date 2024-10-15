SET PATH_TO_SERVER=%~dp0..\chat\server
SET PATH_TO_CLIENT=%~dp0..\chat\client\client

cd %PATH_TO_SERVER%
dotnet build

cd %PATH_TO_CLIENT%
dotnet build
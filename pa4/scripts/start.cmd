SET PATH_TO_APP=%~dp0..\Valuator\
SET PATH_TO_NGINX=%~dp0..\nginx\ 
SET PATH_TO_RANK_CALCULATOR=%~dp0..\RankCalculator\
SET PATH_TO_EVENTS_LOGGER=%~dp0..\EventsLogger\

start docker run -p 4222:4222 -ti nats:latest

cd %PATH_TO_APP%
dotnet build
start /d %PATH_TO_APP% dotnet run --no-build --urls "http://localhost:5001"
start /d %PATH_TO_APP% dotnet run --no-build --urls "http://localhost:5002"

cd %PATH_TO_RANK_CALCULATOR%
dotnet build
start /d %PATH_TO_RANK_CALCULATOR% dotnet run --no-build
start /d %PATH_TO_RANK_CALCULATOR% dotnet run --no-build

cd %PATH_TO_EVENTS_LOGGER%
dotnet build
start /d %PATH_TO_EVENTS_LOGGER% dotnet run --no-build
start /d %PATH_TO_EVENTS_LOGGER% dotnet run --no-build
	
start /d %PATH_TO_NGINX% nginx.exe
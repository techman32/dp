SET PATH_TO_APP=%~dp0..\Valuator\
SET PATH_TO_NGINX=%~dp0..\nginx\ 
SET PATH_TO_RANK_CALCULATOR=%~dp0..\RankCalculator\
SET PATH_TO_EVENTS_LOGGER=%~dp0..\EventsLogger\

setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

start docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest
start docker run -d --name redis-stack-other -p 6002:6379 -p 8004:8001 redis/redis-stack:latest
start docker run -d --name redis-stack-eu -p 6001:6379 -p 8003:8001 redis/redis-stack:latest
start docker run -d --name redis-stack-rus -p 6000:6379 -p 8004:8001 redis/redis-stack:latest


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
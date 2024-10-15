SET PATH_TO_CHAIN=%~dp0..\Chain\

cd %PATH_TO_CHAIN%
start "1" dotnet run 7000 localhost 7001 true
start "2" dotnet run 7001 localhost 7002
start "3" dotnet run 7002 localhost 7003
start "4" dotnet run 7003 localhost 7004
start "5" dotnet run 7004 localhost 7005
start "6" dotnet run 7005 localhost 7000
taskkill /f /im valuator.exe
taskkill /f /im rankcalculator.exe	
taskkill /f /im eventslogger.exe
taskkill /f /im "nginx.exe" /t

FOR /f "tokens=*" %%i IN ('docker ps -q') DO docker stop %%i
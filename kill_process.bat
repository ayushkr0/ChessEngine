@echo off
echo Killing ChessEngine processes...
taskkill /f /im ChessEngine.exe 2>nul
taskkill /f /im dotnet.exe 2>nul
taskkill /f /pid 19244 2>nul
echo Cleaning build output...
dotnet clean
echo Rebuilding project...
dotnet build
echo Done!
pause
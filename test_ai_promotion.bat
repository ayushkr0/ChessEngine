@echo off
echo Testing AI Promotion Implementation...
echo.

echo Compiling test...
csc /out:test_ai.exe TestAIPromotion.cs ChessBoard.cs

if %errorlevel% neq 0 (
    echo Compilation failed!
    pause
    exit /b 1
)

echo Running AI promotion tests...
test_ai.exe

echo.
echo Test completed.
pause
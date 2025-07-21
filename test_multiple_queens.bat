@echo off
echo Compiling Multiple Queens Test...
csc /out:TestMultipleQueens.exe TestMultipleQueensSupport.cs RunMultipleQueensTest.cs ChessBoard.cs Piece.cs

if %ERRORLEVEL% NEQ 0 (
    echo Compilation failed!
    pause
    exit /b 1
)

echo Running Multiple Queens Test...
TestMultipleQueens.exe

if %ERRORLEVEL% NEQ 0 (
    echo Test failed!
    pause
    exit /b 1
)

echo Test completed successfully!
pause
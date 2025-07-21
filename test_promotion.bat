@echo off
echo Compiling verification program...
csc verify_promotion.cs ChessBoard.cs
if exist verify_promotion.exe (
    echo Running verification...
    verify_promotion.exe
    del verify_promotion.exe
) else (
    echo Compilation failed
)
pause
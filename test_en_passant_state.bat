@echo off
echo Compiling En Passant State Management Test...
csc TestEnPassantStateManagement.cs ChessBoard.cs -out:TestEnPassantState.exe
if %errorlevel% equ 0 (
    echo Running En Passant State Management Tests...
    TestEnPassantState.exe
) else (
    echo Compilation failed!
)
pause
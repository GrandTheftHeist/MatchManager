@echo off
:loop
call build.cmd
timeout /t 4 /nobreak >nul
goto loop

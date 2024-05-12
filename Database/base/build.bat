@echo off
echo ---------------------------------
echo Building db_base:latest
echo ---------------------------------
call docker build -t db_base:latest .
pause > nul
exit
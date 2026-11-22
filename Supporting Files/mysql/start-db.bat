@echo off

echo ====================================
echo Starting Local MySQL
echo ====================================

docker compose up -d

if %errorlevel% neq 0 (
    echo.
    echo Failed to start Docker Compose.
    pause
    exit /b 1
)

echo.
echo Waiting for MySQL...

:wait

docker exec local-mysql mysqladmin ping -h localhost -prootpassword >nul 2>&1

if %errorlevel% neq 0 (
    timeout /t 2 >nul
    goto wait
)

echo.
echo MySQL is ready.
echo.
echo Host: localhost
echo Port: 3306
echo Database: KD_InfloTechTest
echo Username: root
echo Password: rootpassword
echo.

pause
@echo off
setlocal

echo ====================================
echo Remove Local MySQL
echo ====================================
echo This will permanently delete the MySQL data volume.
echo.

choice /C YN /N /M "Continue? [Y/N] "
if errorlevel 2 (
    echo.
    echo Teardown cancelled.
    exit /b 0
)

docker compose down --volumes --rmi local

if %errorlevel% neq 0 (
    echo.
    echo Failed to tear down Docker Compose.
    pause
    exit /b 1
)

echo.
echo MySQL containers, network, local image, and volumes removed.
echo.
pause
@echo off
setlocal

echo ============================================
echo  Seoul Stay - IIS Express LAN Setup
echo ============================================
echo.

:: Auto elevate
net session >nul 2>&1
if %errorlevel% neq 0 (
powershell -Command "Start-Process '%~f0' -Verb RunAs"
exit /b
)

:: URL ACL
echo [1/3] Checking URL ACL...

netsh http show urlacl | findstr /I "http://*:5000/" >nul

if errorlevel 1 (
netsh http add urlacl url=http://*:5000/ user=Everyone
) else (
echo [OK] URL ACL already exists.
)

:: Firewall
echo [2/3] Checking Firewall Rule...

netsh advfirewall firewall show rule name="WebPayment5000" >nul 2>&1

if errorlevel 1 (
netsh advfirewall firewall add rule ^
name="WebPayment5000" ^
dir=in ^
action=allow ^
protocol=TCP ^
localport=5000
) else (
echo [OK] Firewall rule already exists.
)

:: IIS Express config
echo [3/3] Updating applicationhost.config...

set "CONFIG=%~dp0.vs\thi_tay_nghe\config\applicationhost.config"

if not exist "%CONFIG%" (
echo.
echo [ERROR] applicationhost.config not found:
echo %CONFIG%
echo.
echo Open GUI_Web_Payment once in Visual Studio first.
pause
exit /b 1
)

copy /Y "%CONFIG%" "%CONFIG%.bak" >nul

powershell -NoProfile -ExecutionPolicy Bypass ^
-File "%~dp0add_binding.ps1" ^
"%CONFIG%"

if errorlevel 1 (
echo.
echo [ERROR] Failed to update applicationhost.config
pause
exit /b 1
)

echo.
echo ============================================
echo Completed Successfully
echo ============================================
echo.
echo Access from LAN:
echo http://YOUR-IP:5000
echo.
pause

@echo off
setlocal

echo ============================================
echo      Seoul Stay - IIS Express Installer
echo ============================================
echo.

:: =====================================================
:: 1. Kiem tra IIS Express da ton tai trong Registry chua
:: =====================================================

reg query "HKLM\SOFTWARE\Microsoft\IISExpress" >nul 2>&1
if %errorlevel%==0 goto INSTALLED

reg query "HKLM\SOFTWARE\WOW6432Node\Microsoft\IISExpress" >nul 2>&1
if %errorlevel%==0 goto INSTALLED

echo [INFO] Chua tim thay IIS Express trên Registry.
echo.

:: =====================================================
:: 2. Chon dung file MSI phu hop voi cau truc Windows
:: =====================================================

if defined ProgramFiles(x86) (
    set "MSI_FILE=%~dp0iisexpress_x64.msi"
    echo [INFO] Phat hien Windows 64-bit.
) else (
    set "MSI_FILE=%~dp0iisexpress_x86.msi"
    echo [INFO] Phat hien Windows 32-bit.
)

:: Neu thieu file msi, thoat ra va tra ma loi 1 de Inno Setup biet ma xu ly, ko xai pause
if not exist "%MSI_FILE%" (
    echo [ERROR] Khong tim thay file msi dat tai: %MSI_FILE%
    exit /b 1
)

echo.
echo [INFO] Dang cai dat am tham IIS Express...
echo.

:: =====================================================
:: 3. Thuc thi lenh cai dat ngam hoan toan (/qn)
:: =====================================================
start "" /wait msiexec /i "%MSI_FILE%" /qn /norestart /L*v "%TEMP%\iisexpress_install.log"

:: Bat truc tiep ma loi cua thuc thi gan nhat
if %errorlevel%==0 goto VERIFY
if %errorlevel%==3010 goto VERIFY

echo [ERROR] Cai dat that bai. MSI Exit Code = %errorlevel%
echo Chi tiet log ghi tai: %TEMP%\iisexpress_install.log
exit /b 1

:: =====================================================
:: 4. Xac minh lai su ton tai cua file sau khi cai
:: =====================================================
:VERIFY
if exist "%ProgramFiles%\IIS Express\iisexpress.exe" goto INSTALLED
if exist "%ProgramFiles(x86)%\IIS Express\iisexpress.exe" goto INSTALLED

echo [ERROR] Trinh cai dat bao thanh cong nhung khong tim thay file iisexpress.exe
exit /b 1

:INSTALLED
echo.
echo ============================================
echo    IIS Express da san sang hoat dong!
echo ============================================
echo.
exit /b 0
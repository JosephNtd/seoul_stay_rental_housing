@echo off
setlocal Enabledelayedexpansion

echo ============================================
echo   Seoul Stay - Kiểm tra & Cấu hình IIS Express
echo ============================================
echo.

:: 1. Xác định đường dẫn file iisexpress.exe mặc định của Windows để check xem cài chưa
set "IIS_PATH_64=%ProgramFiles%\IIS Express\iisexpress.exe"
set "IIS_PATH_32=%ProgramFiles(x86)%\IIS Express\iisexpress.exe"

echo [1/2] Đang kiểm tra hệ thống...

if exist "%IIS_PATH_64%" (
    echo [OK] Đã tìm thấy IIS Express (64-bit).
    goto :SUCCESS
)
if exist "%IIS_PATH_32%" (
    echo [OK] Đã tìm thấy IIS Express (32-bit).
    goto :SUCCESS
)

:: 2. Nếu chưa cài -> Tự động nhận diện cấu trúc Windows (x86 hay x64) để chọn file cài
echo [ALERT] Máy chưa cài đặt IIS Express^^!
echo [2/2] Đang kiểm tra cấu trúc hệ điều hành...

:: Kiểm tra biến PROCESSOR_ARCHITECTURE hoặc ProgramFiles(x86) để nhận diện Win 64-bit
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    set "MSI_FILE=%~dp0iisexpress_x64.msi"
    echo [INFO] Phát hiện Windows 64-bit. Sử dụng bản cài x64.
) else if "%PROCESSOR_ARCHITEW6432%"=="AMD64" (
    set "MSI_FILE=%~dp0iisexpress_x64.msi"
    echo [INFO] Phát hiện Windows 64-bit. Sử dụng bản cài x64.) 
else (
    set "MSI_FILE=%~dp0iisexpress_x86.msi"
    echo [INFO] Phát hiện Windows 32-bit (x86). Sử dụng bản cài x86.
)

if not exist "!MSI_FILE!" (
    echo [ERROR] Không tìm thấy file cấu hình cài đặt IIS Express (!MSI_FILE!) ^^!
    exit /b 1
)

echo Đang tiến hành cài đặt tự động ngầm...
:: Chạy lệnh cài đặt msi tương ứng ngầm (/qn)
msiexec /i "!MSI_FILE!" /qn /norestart

if !errorlevel! equ 0 (
    echo [OK] Đã cài đặt thành công IIS Express!
    goto :SUCCESS
) else (
    echo [ERROR] Quá trình cài đặt IIS Express gặp lỗi. Mã lỗi: !errorlevel!
    exit /b 1
)

:SUCCESS
echo.
echo ============================================
echo   Hệ thống IIS Express đã sẵn sàng hoạt động!
echo ============================================
echo.
exit /b 0
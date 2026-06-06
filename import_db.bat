@echo off
setlocal Enabledelayedexpansion

echo ============================================
echo   Seoul Stay - Tự động Cấu hình Database
echo ============================================
echo.

:: 1. Tự động tìm kiếm Server Name (Instance) đang chạy trên máy
echo [1/3] Đang dò tìm SQL Server trên máy cục bộ...
set "SQL_SERVER="

REM Cách 1: Thử kết nối với Server mặc định của SQLExpress
sqlcmd -S .\SQLExpress -Q "SELECT @@SERVERNAME" >nul 2>&1
if !errorlevel! equ 0 (
    set "SQL_SERVER=.\SQLExpress"
    goto :FOUND
)

REM Cách 2: Thử kết nối với Local mặc định
sqlcmd -S (local) -Q "SELECT @@SERVERNAME" >nul 2>&1
if !errorlevel! equ 0 (
    set "SQL_SERVER=(local)"
    goto :FOUND
)

REM Cách 3: Thử kết nối với dấu chấm cục bộ
sqlcmd -S . -Q "SELECT @@SERVERNAME" >nul 2>&1
if !errorlevel! equ 0 (
    set "SQL_SERVER=."
    goto :FOUND
)

:FOUND
if "%SQL_SERVER%"=="" (
    echo [ERROR] Không tìm thấy SQL Server Instance nào đang chạy trên máy này^^!
    exit /b 1
)

echo [OK] Đã kết nối tới Server: %SQL_SERVER%
echo.

:: 2. Kiểm tra xem Database [Seoul_Stay] đã tồn tại hay chưa
echo [2/3] Kiểm tra sự tồn tại của Database 'Seoul_Stay'...

:: Ép hàm DB_ID kiểm tra chính xác tên Seoul_Stay
sqlcmd -S %SQL_SERVER% -E -Q "IF DB_ID('Seoul_Stay') IS NOT NULL EXIT(99) ELSE EXIT(0)"
set "CHECK_RESULT=%errorlevel%"

if %CHECK_RESULT% equ 99 (
    echo [OK] Database 'Seoul_Stay' đã tồn tại trên máy. Bỏ qua bước Import để bảo toàn dữ liệu cũ.
    echo.
    goto :SUCCESS
)

:: 3. Thực thi Import file .sql nếu chưa có Database trên máy khách
echo [3/3] Chưa có dữ liệu. Đang tự động nạp Database 'Seoul_Stay'...
sqlcmd -S %SQL_SERVER% -E -i "%~dp0script_db.sql"

if !errorlevel! neq 0 (
    echo.
    echo [ERROR] Lỗi trong quá trình nạp database.
    exit /b 1
)

:SUCCESS
echo ============================================
echo   Khởi tạo Cơ sở dữ liệu thành công!
echo ============================================
echo.
exit /b 0
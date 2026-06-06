@echo off
setlocal Enabledelayedexpansion

echo ============================================
echo   Seoul Stay - IIS Express LAN Setup (Prod)
echo ============================================
echo.

:: Tự động xin quyền Admin (Auto elevate)
net session >nul 2>&1
if %errorlevel% neq 0 (
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

:: 1. Cấu hình URL ACL
echo [1/3] Checking URL ACL...
netsh http show urlacl | findstr /I "http://*:5000/" >nul
if errorlevel 1 (
    netsh http add urlacl url=http://*:5000/ user=Everyone
) else (
    echo [OK] URL ACL already exists.
)

:: 2. Cấu hình Tường lửa (Firewall)
echo [2/3] Checking Firewall Rule...
netsh advfirewall firewall show rule name="WebPayment5000" >nul 2>&1
if errorlevel 1 (
    netsh advfirewall firewall add rule name="WebPayment5000" dir=in action=allow protocol=TCP localport=5000
) else (
    echo [OK] Firewall rule already exists.
)

:: 3. Xác định vị trí file cấu hình IIS Express
echo [3/3] Locating applicationhost.config...

:: Hướng tìm 1: Tìm trong thư mục code .vs (Dành cho máy dev)
set "CONFIG=%~dp0.vs\thi_tay_nghe\config\applicationhost.config"

:: Hướng tìm 2: Nếu không thấy (Máy khách cài qua Inno), tìm trong Documents của Windows
if not exist "!CONFIG!" (
    set "CONFIG=%USERPROFILE%\Documents\IISExpress\config\applicationhost.config"
)

:: Kiểm tra cuối cùng xem máy khách đã từng cài/chạy IIS Express chưa
if not exist "!CONFIG!" (
    echo.
    echo [ERROR] Không tìm thấy file applicationhost.config trên hệ thống.
    echo Vui lòng đảm bảo máy tính đã cài đặt IIS Express.
    pause
    exit /b 1
)

echo [OK] Found config at: !CONFIG!

:: Tạo bản sao lưu dự phòng
copy /Y "!CONFIG!" "!CONFIG!.bak" >nul

:: Chèn cổng 5000 trực tiếp bằng PowerShell Inline
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "[xml]$xml = Get-Content '!CONFIG!';" ^
    "$site = $xml.SelectNodes('//site') | Where-Object { $_.name -eq 'GUI_Web_Payment' -or $_.id -eq '1' };" ^
    "if (-not $site) { " ^
    "  $site = $xml.SelectNodes('//site')[0];" ^
    "  Write-Host '[WARN] Không thấy site GUI_Web_Payment, cấu hình đè lên Site mặc định đầu tiên';" ^
    "}" ^
    "$exists = $site.bindings.binding | Where-Object { $_.bindingInformation -eq '*:5000:*' };" ^
    "if ($exists) { Write-Host '[OK] Binding *:5000:* đã tồn tại.' } else {" ^
    "  $newBinding = $xml.CreateElement('binding');" ^
    "  $newBinding.SetAttribute('protocol','http');" ^
    "  $newBinding.SetAttribute('bindingInformation','*:5000:*');" ^
    "  $site.bindings.AppendChild($newBinding) | Out-Null;" ^
    "  $xml.Save('!CONFIG!');" ^
    "  Write-Host '[OK] Đã cấu hình cổng LAN 5000 thành công!';" ^
    "}"

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Lỗi cập nhật cấu hình IIS XML.
    pause
    exit /b 1
)

echo.
echo ============================================
echo  Cấu hình hoàn tất thành công 100%!
echo ============================================
echo.
echo IP máy khách dùng để kết nối trong LAN:
for /f "tokens=2 delims=:" %%a in ('ipconfig ^| findstr /i "IPv4"') do (
    set "ip=%%a"
    echo http:!ip: =!:5000
)
echo.
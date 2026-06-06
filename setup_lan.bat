@echo off
setlocal Enabledelayedexpansion

echo ============================================
echo   Seoul Stay - IIS Express LAN Setup (Clean)
echo ============================================

:: 🛑 ĐÃ XÓA ĐOẠN AUTO ELEVATE (Vì Inno Setup đã cấp quyền Admin sẵn rồi)

:: 1. Cấu hình URL ACL
netsh http show urlacl | findstr /I "http://*:5000/" >nul
if errorlevel 1 (
    netsh http add urlacl url=http://*:5000/ user=Everyone
)

:: 2. Cấu hình Tường lửa (Firewall)
netsh advfirewall firewall show rule name="WebPayment5000" >nul 2>&1
if errorlevel 1 (
    netsh advfirewall firewall add rule name="WebPayment5000" dir=in action=allow protocol=TCP localport=5000
)

:: 3. Xác định vị trí file cấu hình IIS Express
set "CONFIG=%~dp0.vs\thi_tay_nghe\config\applicationhost.config"

if not exist "!CONFIG!" (
    set "CONFIG=%USERPROFILE%\Documents\IISExpress\config\applicationhost.config"
)

:: 🛑 ĐÃ SỬA: Nếu không có file thì thoát luôn (exit), TUYỆT ĐỐI không dùng lệnh 'pause' kẻo treo bộ cài
if not exist "!CONFIG!" (
    exit /b 0
)

:: Tạo bản sao lưu dự phòng
copy /Y "!CONFIG!" "!CONFIG!.bak" >nul

:: Chèn cổng 5000 bằng PowerShell Inline
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "[xml]$xml = Get-Content '!CONFIG!';" ^
    "$site = $xml.SelectNodes('//site') | Where-Object { $_.name -eq 'GUI_Web_Payment' -or $_.id -eq '1' };" ^
    "if (-not $site) { $site = $xml.SelectNodes('//site')[0]; }" ^
    "$exists = $site.bindings.binding | Where-Object { $_.bindingInformation -eq '*:5000:*' };" ^
    "if (-not $exists) {" ^
    "  $newBinding = $xml.CreateElement('binding');" ^
    "  $newBinding.SetAttribute('protocol','http');" ^
    "  $newBinding.SetAttribute('bindingInformation','*:5000:*');" ^
    "  $site.bindings.AppendChild($newBinding) | Out-Null;" ^
    "  $xml.Save('!CONFIG!');" ^
    "}"

exit /b 0
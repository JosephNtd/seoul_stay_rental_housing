param(
[string]$configPath
)

Write-Host "Loading: $configPath"

[xml]$xml = Get-Content $configPath

$sites = $xml.SelectNodes("//site")

$site = $sites | Where-Object {
$_.name -eq "GUI_Web_Payment"
}

if (-not $site)
{
Write-Host "[ERROR] Site GUI_Web_Payment not found."
Write-Host ""
Write-Host "Available sites:"

```
foreach($s in $sites)
{
    Write-Host " - $($s.name)"
}

exit 1
```

}

$exists = $site.bindings.binding |
Where-Object {
$_.bindingInformation -eq "*:5000:*"
}

if ($exists)
{
Write-Host "[OK] Binding already exists."
exit 0
}

$newBinding = $xml.CreateElement("binding")

$newBinding.SetAttribute("protocol","http")
$newBinding.SetAttribute("bindingInformation","*:5000:*")

$site.bindings.AppendChild($newBinding) | Out-Null

$xml.Save($configPath)

Write-Host "[OK] Added binding *:5000:*"

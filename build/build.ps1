# Save and restore working directory
$startFolder = (Get-Location).Path
Write-Host "Started in $startFolder"

$msbuildinfo = $null

try {
    ($msbuildinfo = get-command msbuild) | Out-Null
}
catch {
    
}

if($null -eq $msbuildinfo) {
    Invoke-WebRequest https://aka.ms/vs/17/release/vs_BuildTools.exe -OutFile .\build\vs_BuildTools.exe
    Start-Process -FilePath .\build\vs_BuildTools.exe -ArgumentList "install","--quiet","--add","Microsoft.Net.Component.4.8.SDK","--wait","--installPath","$startFolder\build\buildtools" -Wait
}

if(Test-Path "$startFolder\build\buildtools\Common7\Tools\Launch-VsDevShell.ps1") {
    . "$startFolder\build\buildtools\Common7\Tools\Launch-VsDevShell.ps1"
    try {
        ($msbuildinfo = get-command msbuild) | Out-Null
    }
    catch {
        
    }
}

if($msbuildinfo -ne $null) {
    Write-Output "MSBUILD found"
}
else {
    Write-Output "MSBUILD not found"
    Exit 1
}

# Go to solution directory
Set-Location $startFolder

# Start release build in solution directory
Start-Process msbuild -ArgumentList "-p:Configuration=Release"

Set-Location $startFolder
Start-Process -FilePath .\build\vs_BuildTools.exe -ArgumentList "uninstall","--quiet","--installPath","$startFolder\build\buildtools" -Wait

# Restore working directory
Set-Location $startFolder
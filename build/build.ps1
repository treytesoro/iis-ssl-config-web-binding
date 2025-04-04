# Save and restore working directory
$startFolder = Get-Location

# Locate visual studio's powershell developer shell loader
$obj = (Get-ChildItem -Path 'C:\Program Files\Microsoft Visual Studio' -Recurse | Where-Object Name -eq 'Launch-VsDevShell.ps1' | Select-Object -First 1)

# Exit if not found
if($null -eq $obj){
    Exit 1
}

# Dot source Launch-VsDevShell.ps1 script
. "$($obj.Directory.ToString())\$($obj.Name)"

# Go to solution directory
Set-Location $startFolder

# Start release build in solution directory
Start-Process msbuild -ArgumentList "-p:Configuration=Release"

Set-Location $startFolder
Start-Process -FilePath .\build\vs_BuildTools.exe -ArgumentList "uninstall","--quiet","--installPath","$startFolder\build\buildtools" -Wait

# Restore working directory
Set-Location $startFolder

& gh release delete v1.2.3 --cleanup-tag --yes
& gh release delete v1.0.0 --cleanup-tag --yes
& gh release create v1.0.0 .\SetWebBinding\bin\Release\sslbinding.exe
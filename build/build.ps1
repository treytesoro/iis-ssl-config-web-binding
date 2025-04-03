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
& msbuild -p:Configuration=Release

# Restore working directory
Set-Location $startFolder
# Save and restore working directory
$startFolder = (Get-Location).Path

# Locate visual studio's powershell developer shell loader
$obj = (Get-ChildItem -Path 'C:\Program Files\Microsoft Visual Studio' -Recurse | Where-Object Name -eq 'Launch-VsDevShell.ps1' | Select-Object -First 1)

# Exit if not found
if($null -eq $obj){
    Exit 1
}

# Dot source Launch-VsDevShell.ps1 script
. "$($obj.Directory.ToString())\$($obj.Name)"

# Restore working directory
Set-Location $startFolder
# IIS ssl binding tool

This is meant to assist in adding and configuring an IIS Site's SSL binding and certificate.

# Requirements

This references Microsoft.Web.Administration at path `C:\Windows\System32\inetsrv\Microsoft.Web.Administration.dll`.

If the assembly does not exist at that location the build will fail.

From the root of the solution, run `.\build\build.ps1` in a powershell terminal. This will compile the project to the .\bin\Release directory. Output executable name is `sslbinding.exe`.

## Usage

```
sslbinding.exe -thumb "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" -store "My" -site "Doogle" -port "443" -host "www.doogle.com"
```
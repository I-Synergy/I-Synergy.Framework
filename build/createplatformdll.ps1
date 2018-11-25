param($file)

Write-Host $file
 
$exe = Get-ChildItem -Path "C:\program files (x86)\Microsoft SDKs\Windows\v10.0A\bin" -Recurse -Include corflags.exe | select -First 1
Write-Host $exe
 
if ($file.Extension -eq ".dll")
{
    Write-Host $file.FullName
    &$exe corflags /32bitreq- $($file.FullName)
}
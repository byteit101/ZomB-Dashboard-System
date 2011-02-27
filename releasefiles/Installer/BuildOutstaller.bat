"C:\WINDOWS\Microsoft.NET\Framework\v3.5\msbuild.exe" outstaller\OutStaller08.sln /p:Configuration=Release
if %errorlevel% EQU 0 goto okay
exit %errorlevel%
:okay
copy outstaller\bin\Release\OutStaller.exe OutStaller.exe
@rem This needs to be run in a folder that has both trunk and releasefiles
copy trunk\releasefiles\source\zomb.sln trunk\ZomB.sln /Y

@rem switch to installer dir, patch version
cd trunk\releasefiles\Installer\

IF %InstallerBuildSVN% EQU true (
call BatSub.bat FULL_VERSION %InstallerBuildVersion%%SVN_REVISION% version.diff > tversion.diff
call BatSub.bat SHORT_VERSION "%InstallerBuildVersion%0" tversion.diff > zversion.diff
) ELSE (
call BatSub.bat FULL_VERSION %InstallerBuildDisplay% version.diff > tversion.diff
call BatSub.bat SHORT_VERSION %InstallerBuildVersion% tversion.diff > zversion.diff
)
cd ..\..\..\
patch -p0 < trunk\releasefiles\Installer\zversion.diff

@rem build
"C:\WINDOWS\Microsoft.NET\Framework\v3.5\msbuild.exe" trunk\zomb.sln /p:Configuration=Release
if %errorlevel% EQU 0 goto okay
exit %errorlevel%

:okay

@rem Copy Default Dash
del "trunk\releasefiles\Default Dashboard\.gitignore"
copy "trunk\Apps\DefaultDash\bin\Release\*.dll" "trunk\releasefiles\Default Dashboard\"
copy "trunk\Apps\DefaultDash\bin\Release\*.exe" "trunk\releasefiles\Default Dashboard\"
copy "trunk\Apps\DefaultDash\bin\Release\*.txt" "trunk\releasefiles\Default Dashboard\"

@rem Copy ZomB Eye
del "trunk\releasefiles\ZomB Eye\.gitignore"
copy "trunk\Apps\ZomBeye\bin\Release\*.dll" "trunk\releasefiles\ZomB Eye\"
copy "trunk\Apps\ZomBeye\bin\Release\*.exe" "trunk\releasefiles\ZomB Eye\"
copy "trunk\Apps\ZomBeye\bin\Release\*.txt" "trunk\releasefiles\ZomB Eye\"

@rem Copy ViZ
del "trunk\releasefiles\Visual ZomB\.gitignore"
copy "trunk\Apps\VisualZomB\bin\Release\*.dll" "trunk\releasefiles\Visual ZomB\"
copy "trunk\Apps\VisualZomB\bin\Release\*.exe" "trunk\releasefiles\Visual ZomB\"
copy "trunk\Apps\VisualZomB\bin\Release\*.txt" "trunk\releasefiles\Visual ZomB\"

@rem Copy ZomB.dll and Prerequisites
del "trunk\releasefiles\ZomB dll's\.gitignore"
copy "trunk\ZomBdll\bin\Release\*.dll" "trunk\releasefiles\ZomB dll's"
copy "trunk\ZomBdll\bin\Release\*.txt" "trunk\releasefiles\ZomB dll's"

IF %Installer% NEQ true goto clean
@rem Build the installer, Complex!
IF %InstallerBuildSVN% EQU true (
call trunk\releasefiles\Installer\BuildInstaller.bat "%InstallerBuildVersion%%SVN_REVISION%"
if %errorlevel% NEQ 0 exit %errorlevel%
) ELSE (
call trunk\releasefiles\Installer\BuildInstaller.bat "%InstallerBuildDisplay%"
if %errorlevel% NEQ 0 exit %errorlevel%
)
copy "Install ZomB*.exe" ..\

:clean
IF %dll% NEQ true (IF %bindings% NEQ true (IF %src% NEQ true (
@rem no zips, we are finished
goto finish
)))

@rem clean up
del trunk\Apps\DefaultDash\obj\ /F /S /Q
del trunk\Apps\ZomBeye\obj\ /F /S /Q
del trunk\Apps\VisualZomB\obj\ /F /S /Q
del trunk\Apps\NullGEN\obj\ /F /S /Q
del trunk\ZomBdll\obj\ /F /S /Q
del trunk\ZomB.DriverStation\obj\ /F /S /Q
del trunk\ZomB.xShake\obj\ /F /S /Q
del trunk\Apps\DefaultDash\bin\ /F /S /Q
del trunk\Apps\ZomBeye\bin\ /F /S /Q
del trunk\Apps\VisualZomB\bin\ /F /S /Q
del trunk\Apps\NullGEN\bin\ /F /S /Q
del trunk\ZomBdll\bin\ /F /S /Q
del trunk\ZomB.DriverStation\bin\ /F /S /Q
del trunk\ZomB.xShake\bin\ /F /S /Q

@rem Copy
mkdir trunk\releasefiles\source\Apps
xcopy trunk\Apps\* trunk\releasefiles\source\Apps\ /S
mkdir trunk\releasefiles\source\ZomBdll
xcopy trunk\ZomBdll\* trunk\releasefiles\source\ZomBdll\ /S
mkdir trunk\releasefiles\source\ZomB.DriverStation
xcopy trunk\ZomB.DriverStation\* trunk\releasefiles\source\ZomB.DriverStation\ /S
mkdir trunk\releasefiles\source\ZomB.xShake
xcopy trunk\ZomB.xShake\* trunk\releasefiles\source\ZomB.xShake\ /S

@rem De-SVN-ize
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S *.svn*') DO RMDIR /S /Q "%%G"
DEL /S /Q thumbs.db
DEL /S /Q *.orig

@rem clean up and exit
xcopy trunk\releasefiles /S /E /Y /H

@rem Zip up specified ones
IF %dll% EQU true "C:\Program Files\7-Zip\7z" a -r ../ZomBr%SVN_REVISION%dllBuild%BUILD_NUMBER%.7z "./ZomB dll's/*"
IF %bindings% EQU true "C:\Program Files\7-Zip\7z" a -r ../ZomBr%SVN_REVISION%BindingsBuild%BUILD_NUMBER%.7z "./Bindings/*"
IF %src% EQU true "C:\Program Files\7-Zip\7z" a -r ../ZomBr%SVN_REVISION%Source.7z "./Source/*"

:finish
@rem end!

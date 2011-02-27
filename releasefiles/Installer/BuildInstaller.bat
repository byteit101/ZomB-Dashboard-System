@rem Build Default.exe from Default.zaml
trunk\Apps\VisualZomB\bin\Release\ViZ.exe -build trunk\Apps\VisualZomB\bin\Release\Default.zaml releasefiles\Installer\Default.exe

@rem Get plugin dll's
copy "trunk\ZomB.DriverStation\bin\Release\*.dll" "releasefiles\Installer\"

@rem Get ZomB.dll
copy "trunk\ZomBdll\bin\Release\*.dll" "releasefiles\Installer\"
copy "trunk\ZomBdll\bin\Release\*.txt" "releasefiles\Installer\"
copy "trunk\ZomBdll\bin\Release\*.exe" "releasefiles\Installer\"

@rem Copy ViZ
copy "trunk\Apps\VisualZomB\bin\Release\*.exe" "releasefiles\Installer\"
copy "trunk\Apps\VisualZomB\bin\Release\*.ico" "releasefiles\Installer\"

@rem Copy Default Dash
copy "trunk\Apps\DefaultDash\bin\Release\*.exe" "releasefiles\Installer\"

@rem Copy ZomB Eye
copy "trunk\Apps\ZomBeye\bin\Release\*.exe" "releasefiles\Installer\"

@rem Copy NullGEN
copy "trunk\Apps\NullGEN\bin\Release\*.exe" "releasefiles\Installer\"

@rem Copy bindings
copy "releasefiles\Bindings\*.*" "releasefiles\Installer\"

@rem switch to installer dir, build outstaller
cd releasefiles\Installer\
call BuildOutstaller.bat

@rem Replace VERSION_NUMBER with our version, build, and copy to out
BatSub.bat VERSION_NUMBER %1 setup.nsi | "C:\Program Files\NSIS\makensis.exe" -
cd ..\..\
copy "releasefiles\Installer\Install ZomB %~1.exe" .\
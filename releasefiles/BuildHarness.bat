@rem Test harness, double click and go
@rem File system must look like this:
@rem somefolder/
@rem somefolder/trunk/releasefiles/
@rem run from releasefiles or somefolder
@echo off
set bhCWD=%CD%
echo Searching for trunk folder...
:BEGINBUILD
IF EXIST trunk GOTO TESTTrunk
CD ..\
GOTO BEGINBUILD
:TESTTrunk
echo Trunk found, CWD is: %CD%
:retest
set patchFOUND=
FOR %%X in (patch.exe) do ( set patchFOUND=%%~$PATH:X )
IF "x%patchFOUND%" NEQ "x" (
IF "x%patchFOUND%" NEQ "x " (
	echo patch.exe found in %patchFOUND%
	GOTO CONTSecOne
)
)
echo patch.exe not found in path, mofifying...
IF EXIST %bhCWD%\Installer\patch.exe GOTO tryadd

echo patch.exe not found in installer! Aborting!
pause
exit 1
:tryadd
SET PATH=%PATH%;%bhCWD%\Installer\
GOTO retest

:CONTSecOne
@rem get all our vars that hudson normally provides
SET dll=N
SET /P dll=Build the ZomB.dll [y/N]: 
IF /i %dll% EQU Y (SET dll=true) ELSE SET dll=false

SET src=N
SET /P src=Zip the source code [y/N]: 
IF /i %src% EQU Y (SET src=true) ELSE SET src=false

SET bindings=N
SET /P bindings=Zip the bindings [y/N]: 
IF /i %bindings% EQU Y (SET bindings=true) ELSE SET bindings=false

SET Installer=Y
SET /P Installer=Build the Installer [Y/n]: 
IF /i %Installer% EQU Y (SET Installer=true) ELSE SET Installer=false

SET InstallerBuildVersion=0.8.3.
SET /P InstallerBuildVersion=Version (Becomes Install ZomB (Version)(SVN).exe) [0.8.3.]:

SET InstallerBuildSVN=true

SET SVN_REVISION=451
SET /P SVN_REVISION=SVN Revision (don't set too high) [451]:

SET BUILD_NUMBER=70
SET InstallerBuildDisplay=0.8.3.0

@echo on
@call trunk\releasefiles\Build.bat
@echo Done!
@PAUSE
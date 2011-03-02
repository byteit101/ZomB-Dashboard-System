@rem Test harness, double click and go
@rem File system must look like this:
@rem somefolder/
@rem somefolder/trunk/releasefiles/
@rem run from releasefiles or somefolder
@echo off
:BEGINBUILD
IF EXIST trunk GOTO TESTTrunk
CD ..\
GOTO BEGINBUILD
:TESTTrunk
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

SET InstallerBuildVersion=0.6.1.
SET /P InstallerBuildVersion=Version (Becomes Install ZomB (Version).exe) [0.6.1.]:

SET InstallerBuildSVN=true

SET SVN_REVISION=310
SET /P SVN_REVISION=SVN Revision (don't set too high) [310]:

SET BUILD_NUMBER=50
SET /P BUILD_NUMBER=Build number [50]:

@echo on
@call trunk\releasefiles\Build.bat
@echo Done!
@PAUSE
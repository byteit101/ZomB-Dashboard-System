SetCompressor /SOLID lzma

Name "ZomB Dashboard System"

RequestExecutionLevel highest

# General Symbol Definitions
!define REGKEY "SOFTWARE\ZomB"
!define VERSION "VERSION_NUMBER" ;CONST VERSION
!define COMPANY "Team 451 and Patrick Plenefisch"
!define URL http://thecatattack.org/ZomB
!define VLCVERSION "1.1.10"
!define VLCVERSION2 "1.1.11"
!define VLCVERSION3 "1.1.12"
!define VLCVERSION4 "1.1.13"

# MUI Symbol Definitions
!define MUI_ICON "Installer.ico"
!define MUI_UNICON "Uninstaller.ico"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT HKLM
!define MUI_STARTMENUPAGE_REGISTRY_KEY ${REGKEY}
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME StartMenuGroup
!define MUI_STARTMENUPAGE_DEFAULTFOLDER ZomB

!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE
!define MUI_COMPONENTSPAGE_SMALLDESC

# Included files
!include Sections.nsh
!include MUI2.nsh
!include Library.nsh
!include FileAssociation.nsh

# Variables
Var StartMenuGroup
Var ZomBLibInstall
Var vlcVers

# Installer pages
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_STARTMENU Application $StartMenuGroup
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# Installer languages
!insertmacro MUI_LANGUAGE English

# Installer attributes
OutFile "Install ZomB VERSION_NUMBER.exe" ;CONST VERSION
InstallDir $PROGRAMFILES\ZomB
CRCCheck on
XPStyle on
ShowInstDetails show
VIProductVersion 1.5.0.0
VIAddVersionKey ProductName "ZomB Dashboard System"
VIAddVersionKey ProductVersion "${VERSION}"
VIAddVersionKey CompanyName "${COMPANY}"
VIAddVersionKey CompanyWebsite "${URL}"
VIAddVersionKey FileVersion "${VERSION}"
VIAddVersionKey FileDescription ""
VIAddVersionKey LegalCopyright ""
ShowUninstDetails show
BrandingText "ZomB Dashboard System ${VERSION}"

# Macros
!macro _DIREMPTY DIR D1 D2 D3
    Push "${DIR}"
    Call un.isEmptyDir
!macroend
!define IsDirEmpty '"" DIREMPTY'

!macro CREATE_SMGROUP_SHORTCUT NAME PATH
    Push "${NAME}"
    Push "${PATH}"
    Call CreateSMGroupShortcut
!macroend

!macro DELETE_SMGROUP_SHORTCUT NAME
    Push "${NAME}"
    Call un.DeleteSMGroupShortcut
!macroend

!macro NGEN NAME
    Push "v2.0"
	Call GetDotNetDir
	Pop $R0
	ExecWait '"$R0\ngen.exe" install "${NAME}" /silent'
!macroend

!macro UNGEN NAME
    Push "v2.0"
	Call un.GetDotNetDir
	Pop $R0
	ExecWait '"$R0\ngen.exe" uninstall "${NAME}" /silent'
!macroend

!macro IfKeyExists ROOT MAIN_KEY KEY
  Push $R0
  Push $R1
  Push $R2
 
  # XXX bug if ${ROOT}, ${MAIN_KEY} or ${KEY} use $R0 or $R1
 
  StrCpy $R1 "0" # loop index
  StrCpy $R2 "0" # not found
 
  ${Do}
    EnumRegValue $R0 ${ROOT} "${MAIN_KEY}" "$R1"
    ${If} '$R0' == '${KEY}'
      StrCpy $R2 "1" # found
      ${Break}
    ${EndIf}
    IntOp $R1 $R1 + 1
  ${LoopWhile} '$R0' != ""
 
  ClearErrors
 
  Exch 2
  Pop $R0
  Pop $R1
  Exch $R2
!macroend

# Functions
Function CreateSMGroupShortcut
    Exch $R0 ;PATH
    Exch 
    Exch $R1 ;NAME
    Push $R2
    StrCpy $R2 $StartMenuGroup 1
    StrCmp $R2 `>` no_smgroup
    CreateShortcut "$SMPROGRAMS\$StartMenuGroup\$R1.lnk" "$R0"
no_smgroup:
    Pop $R2
    Pop $R1
    Pop $R0
FunctionEnd

; based on http://nsis.sourceforge.net/Get_directory_of_installed_.NET_runtime
; Given a .NET version number, this function returns that .NET framework's
; install directory. Returns "" if the given .NET version is not installed.
; Params: [version] (eg. "v2.0")
; Return: [dir] (eg. "C:\WINNT\Microsoft.NET\Framework\v2.0.50727")
Function GetDotNetDir
	Exch $R0 ; Set R0 to .net version major
	Push $R1
	Push $R2
 
	; set R2 to .NET install dir root
	ReadRegStr $R2 HKLM "Software\Microsoft\.NETFramework" "InstallRoot"
 
	; set R0 to the .NET install dir full
	StrCpy $R0 "$R2v2.0.50727"
	
	Pop $R2
	Pop $R1
	Exch $R0 ; return .net install dir full
	Return
 FunctionEnd
 Function un.GetDotNetDir
	Exch $R0 ; Set R0 to .net version major
	Push $R1
	Push $R2
 
	; set R2 to .NET install dir root
	ReadRegStr $R2 HKLM "Software\Microsoft\.NETFramework" "InstallRoot"
 
	; set R0 to the .NET install dir full
	StrCpy $R0 "$R2v2.0.50727"
	
	Pop $R2
	Pop $R1
	Exch $R0 ; return .net install dir full
	Return
 FunctionEnd

Function un.DeleteSMGroupShortcut
    Exch $R1 ;NAME
    Push $R2
    StrCpy $R2 $StartMenuGroup 1
    StrCmp $R2 `>` no_smgroup
    Delete /REBOOTOK "$SMPROGRAMS\$StartMenuGroup\$R1.lnk"
no_smgroup:
    Pop $R2
    Pop $R1
FunctionEnd

Function un.isEmptyDir
  # Stack ->                    # Stack: <directory>
  Exch $0                       # Stack: $0
  Push $1                       # Stack: $1, $0
  FindFirst $0 $1 "$0\*.*"
  strcmp $1 "." 0 _notempty
    FindNext $0 $1
    strcmp $1 ".." 0 _notempty
      ClearErrors
      FindNext $0 $1
      IfErrors 0 _notempty
        FindClose $0
        Pop $1                  # Stack: $0
        StrCpy $0 1
        Exch $0                 # Stack: 1 (true)
        goto _end
     _notempty:
       FindClose $0
       ClearErrors
       Pop $1                   # Stack: $0
       StrCpy $0 0
       Exch $0                  # Stack: 0 (false)
  _end:
FunctionEnd

InstType "Default"
InstType "Default and VLC"
InstType "Full"
InstType "Minimal"

# Installer sections
Section "-ZomB Core" SEC0000
    SectionIn 1 2 3 4
    
    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application    
    CreateDirectory "$SMPROGRAMS\$StartMenuGroup\"    
    !insertmacro MUI_STARTMENU_WRITE_END

    SetOutPath $INSTDIR
    SetOverwrite on
    File ZomB.dll
    File SharpPG.dll
	File ICSharpCode.SharpZipLib.dll
    File /nonfatal LICENSE.txt
    File /nonfatal CREDITS.txt
    File NullGEN.exe
    ExecWait "$INSTDIR\NullGen.exe compile framework" $0
SectionEnd

Section -post SEC0001
    WriteRegStr HKLM "${REGKEY}" Path $INSTDIR
    WriteUninstaller "$INSTDIR\Uninstall ZomB VERSION_NUMBER.exe" ;CONST VERSION
    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    CreateShortcut "$SMPROGRAMS\$StartMenuGroup\Uninstall.lnk" "$INSTDIR\Uninstall ZomB VERSION_NUMBER.exe" ;CONST VERSION
    !insertmacro MUI_STARTMENU_WRITE_END
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZomB" DisplayName "ZomB VERSION_NUMBER" ;CONST VERSION
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZomB" DisplayIcon "$INSTDIR\Uninstall ZomB VERSION_NUMBER.exe" ;CONST VERSION
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZomB" UninstallString "$INSTDIR\Uninstall ZomB VERSION_NUMBER.exe" ;CONST VERSION
    WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZomB" EstimatedSize 25000
    WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZomB" NoModify 1
    WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZomB" NoRepair 1
SectionEnd

SectionGroup "Dependencies" SECGRP0001
    Section "FFmpeg" SEC0002
        SectionIn 1 2 3
        SetOverwrite on
		File "/oname=$WINDIR\ffmpeg.exe" ffmpeg.exe
    SectionEnd
    Section "VLC" SEC0003
        SectionIn 2 3
        SetOverwrite on
        StrCmp $vlcVers "${VLCVERSION}" 0 +2
            Return
		StrCmp $vlcVers "${VLCVERSION2}" 0 +2
            Return
		StrCmp $vlcVers "${VLCVERSION3}" 0 +2
            Return
		StrCmp $vlcVers "${VLCVERSION4}" 0 +2
            Return
        NSISdl::download "http://downloads.sourceforge.net/project/vlc/${VLCVERSION}/win32/vlc-${VLCVERSION}-win32.exe" "vlc-${VLCVERSION}-win32.exe"
        Pop $R0
        ${If} $R0 == "success"
            ExecWait '"vlc-${VLCVERSION}-win32.exe" /S' $0
        ${Else}
            MessageBox MB_OK "VLC Download failed: $R0"
        ${EndIf}
    SectionEnd
	Section "SlimDX" SEC0014
		SectionIn 2 3
        SetOverwrite on
		!insertmacro IfKeyExists "HKLM" "SOFTWARE\Classes\Installer\Assemblies\Global" 'SlimDX,version="2.0.12.43",culture="neutral",publicKeyToken="B1B0C32FD1FFE4F9",processorArchitecture="x86"'
		Pop $R0
		${If} $R0 == 0
			!insertmacro IfKeyExists "HKLM" "SOFTWARE\Classes\Installer\Assemblies\Global" 'SlimDX,version="2.0.12.43",culture="neutral",publicKeyToken="B1B0C32FD1FFE4F9",processorArchitecture="x64"'
			Pop $R0
			${If} $R0 == 0
				NSISdl::download "http://slimdx.googlecode.com/files/SlimDX%20Runtime%20for%20.NET%202.0%20%28September%202011%29.msi" "SlimDX Runtime for .NET 2.0 (September 2011).msi"
				Pop $R0
				${If} $R0 == "success"
					ExecWait "SlimDX Runtime for .NET 2.0 (September 2011).msi" $0
				${Else}
					MessageBox MB_OK "SlimDX Download failed: $R0"
				${EndIf}
			${EndIf}
		${EndIf}
		#http://slimdx.googlecode.com/files/SlimDX%20Runtime%20for%20.NET%202.0%20%28March%202011%29.msi
		#SlimDX,version="2.0.11.43",culture="neutral",publicKeyToken="B1B0C32FD1FFE4F9",processorArchitecture="x86"
	SectionEnd
SectionGroupEnd

Section "Visual ZomB" SEC0004
    SectionIn 1 2 3 4 RO
    SetOverwrite on
    File ViZ.exe
	File Zaml.ico
	File Dashboardexe.ico
	File EnableDriver.reg
	File DisableDriver.reg
    ExecWait "$INSTDIR\ViZ.exe -extract-install" $0
    ${registerExtension} "$INSTDIR\ViZ.exe" ".zaml" "Run Zaml File"
	Call RefreshShellIcons
    !insertmacro CREATE_SMGROUP_SHORTCUT "Visual ZomB" "$INSTDIR\ViZ.exe"
SectionEnd

SectionGroup "Bindings (obsolete)" SECGRP0002
	Section /o "Robot Binding (Non-Smart sources, obsolete)" SEC0013
        SectionIn 3
        SetOverwrite on
        ${IfNot} ${FileExists} `$INSTDIR\Bindings`
        CreateDirectory "$INSTDIR\Bindings"
        ${EndIf}
        File "/oname=Bindings\ZomB.out" ZomB.out
		File "/oname=Bindings\ZomB.a" ZomB.a
		File "/oname=Bindings\OutStaller.exe" OutStaller.exe
		!insertmacro CREATE_SMGROUP_SHORTCUT "Install bindings to robot" "$INSTDIR\Bindings\OutStaller.exe"
		Exec "$INSTDIR\Bindings\OutStaller.exe"
		File "/oname=Bindings\Bindings Help.pdf" "Bindings Help.pdf"
        !insertmacro CREATE_SMGROUP_SHORTCUT "Binding Help" "$INSTDIR\Bindings\Bindings Help.pdf"
    SectionEnd
	
    Section /o "C++ Bindings" SEC0005
        SectionIn 3
        SetOverwrite on
        ${IfNot} ${FileExists} `$INSTDIR\Bindings`
        CreateDirectory "$INSTDIR\Bindings"
        ${EndIf}
        File "/oname=Bindings\ZomBDashboard.h" ZomBDashboard.h
    SectionEnd
	
    Section /o "Java Bindings" SEC0006
        SectionIn 3
        SetOverwrite on
        ${IfNot} ${FileExists} `$INSTDIR\Bindings`
        CreateDirectory "$INSTDIR\Bindings"
        ${EndIf}
        File "/oname=Bindings\ZomBDashboard.java" ZomBDashboard.java
        File "/oname=Bindings\ZomBModes.java" ZomBModes.java
    SectionEnd
	
    Section /o "LabVIEW Bindings" SEC0007
        SectionIn 3
        SetOverwrite on
        ${IfNot} ${FileExists} `$INSTDIR\Bindings`
        CreateDirectory "$INSTDIR\Bindings"
        ${EndIf}
        File "/oname=Bindings\ZomB.llb" ZomB.llb
    SectionEnd
    
    Section  /o "Shortcut" SEC0008
        SectionIn 3
        ${IfNot} ${FileExists} `$INSTDIR\Bindings`
        CreateDirectory "$INSTDIR\Bindings"
        ${EndIf}
        !insertmacro CREATE_SMGROUP_SHORTCUT "Bindings" "$INSTDIR\Bindings"
    SectionEnd
SectionGroupEnd

SectionGroup "Others" SECGRP0003
    Section /o "Default Dashboard (WinForms)" SEC0009
        SectionIn 3
        SetOverwrite on
        File DefaultDash.exe
        !insertmacro CREATE_SMGROUP_SHORTCUT "Default Dashboard (WinForm)" "$INSTDIR\DefaultDash.exe"
		!insertmacro NGEN "$INSTDIR\DefaultDash.exe"
    SectionEnd

    Section "Default Dashboard (WPF)" SEC0010
        SectionIn 1 2 3
        SetOverwrite on
        File Default.exe
		!insertmacro NGEN "$INSTDIR\Default.exe"
        !insertmacro CREATE_SMGROUP_SHORTCUT "Default Dashboard" "$INSTDIR\Default.exe"
    SectionEnd

    Section "ZomB Eye" SEC0011
        SectionIn 1 2 3
        SetOverwrite on
        File "ZomB Eye.exe"
        !insertmacro CREATE_SMGROUP_SHORTCUT "ZomB Eye" "$INSTDIR\ZomB Eye.exe"
		!insertmacro NGEN "$INSTDIR\ZomB Eye.exe"
    SectionEnd

    Section "Driver Station Plugin" SEC0012
        SectionIn 1 2 3
        SetOverwrite on
		${IfNot} ${FileExists} `$INSTDIR\plugins`
        CreateDirectory "$INSTDIR\plugins"
        ${EndIf}
        File "/oname=plugins\ZomB.DriverStation.dll" "ZomB.DriverStation.dll"
		!insertmacro NGEN "$INSTDIR\plugins\ZomB.DriverStation.dll"
    SectionEnd
SectionGroupEnd

Section -un.post UNSEC0005
    DeleteRegKey HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZomB"
    Delete /REBOOTOK "$SMPROGRAMS\$StartMenuGroup\Uninstall.lnk"
    Delete /REBOOTOK "$INSTDIR\Uninstall ZomB VERSION_NUMBER.exe" ;CONST VERSION
    DeleteRegValue HKLM "${REGKEY}" StartMenuGroup
    DeleteRegValue HKLM "${REGKEY}" Path
    DeleteRegKey /IfEmpty HKLM "${REGKEY}\Components"
    DeleteRegKey /IfEmpty HKLM "${REGKEY}"
    RmDir /r /REBOOTOK $SMPROGRAMS\$StartMenuGroup
SectionEnd

# Uninstaller sections
Section "-un.ZomB" UNSEC0000
	ExecWait "$INSTDIR\NullGEN.exe -uninstall"
    Delete /REBOOTOK "ZomB.dll"
    Delete /REBOOTOK "ICSharpCode.SharpZipLib.dll"
    Delete /REBOOTOK "SharpPG.dll"
    Delete /REBOOTOK "32feetWidcomm.dll"
    Delete /REBOOTOK "InTheHand.Net.Personal.dll"
    Delete /REBOOTOK "SlimDX.dll"
    Delete /REBOOTOK "libvlc.dll"
    Delete /REBOOTOK "libvlccore.dll"
    Delete /REBOOTOK "Vlc.DotNet.Core.dll"
    Delete /REBOOTOK "Vlc.DotNet.Forms.dll"
	Delete /REBOOTOK "plugins\ZomB.DriverStation.dll"
    Delete /REBOOTOK "ffmpeg.exe"
    Delete /REBOOTOK "LICENSE.txt"
    Delete /REBOOTOK "CREDITS.txt"
    Delete /REBOOTOK "Bindings\ZomBDashboard.h"
    Delete /REBOOTOK "Bindings\ZomBDashboard.java"
    Delete /REBOOTOK "Bindings\ZomBModes.java"
    Delete /REBOOTOK "Bindings\ZomB.llb"
    Delete /REBOOTOK "Bindings\ZomB.out"
    Delete /REBOOTOK "Bindings\OutStaller.exe"
    Delete /REBOOTOK "DefaultDash.exe"
    Delete /REBOOTOK "Default.exe"
    Delete /REBOOTOK "ZomB Eye.exe"
    Delete /REBOOTOK "ViZ.exe"
    Delete /REBOOTOK "Zaml.ico"
    Delete /REBOOTOK "Dashboardexe.ico"
    ${UnRegisterExtension} ".zaml" "Zaml File"
	Call un.RefreshShellIcons
    
    ${If} ${IsDirEmpty} `$INSTDIR\Bindings`
    RmDir "$INSTDIR\Bindings"
    ${EndIf}
	${If} ${IsDirEmpty} `$INSTDIR\plugins`
    RmDir "$INSTDIR\plugins"
    ${EndIf}
	
	!insertmacro UNGEN "$INSTDIR\Default.exe"
	!insertmacro UNGEN "$INSTDIR\DefaultDash.exe"
	!insertmacro UNGEN "$INSTDIR\Bindings\OutStaller.exe"
	!insertmacro UNGEN "$INSTDIR\ZomB Eye.exe"
	!insertmacro UNGEN "$INSTDIR\plugins\ZomB.DriverStation.dll"
	!insertmacro UNGEN "$INSTDIR\ViZ.exe"
    
    !insertmacro DELETE_SMGROUP_SHORTCUT "Bindings"
	!insertmacro DELETE_SMGROUP_SHORTCUT "Install bindings to robot"
    !insertmacro DELETE_SMGROUP_SHORTCUT "Visual ZomB"
    !insertmacro DELETE_SMGROUP_SHORTCUT "Default Dashboard (WinForm)"
    !insertmacro DELETE_SMGROUP_SHORTCUT "Default Dashboard"
    !insertmacro DELETE_SMGROUP_SHORTCUT "ZomB Eye"
    RmDir /r /REBOOTOK $SMPROGRAMS\$StartMenuGroup
SectionEnd

# Installer functions
Function .onInit
    InitPluginsDir
    Push $0
    ReadRegStr $0 HKLM "${REGKEY}" Path
    ClearErrors
    StrCmp $0 "" +2
    StrCpy $ZomBLibInstall 1
    Pop $0
	#TODO, remove redundacy
    ReadRegStr $vlcVers HKLM "Software\VideoLAN\VLC\" "Version"
    ${If} $vlcVers == "${VLCVERSION}"
        SectionSetFlags ${SEC0003} 16
        SectionSetText ${SEC0003} "VLC (Already Installed)"
        SectionSetSize ${SEC0003} 0
    ${ElseIf} $vlcVers == "${VLCVERSION2}"
        SectionSetFlags ${SEC0003} 16
        SectionSetText ${SEC0003} "VLC (Already Installed)"
        SectionSetSize ${SEC0003} 0
    ${ElseIf} $vlcVers == "${VLCVERSION3}"
        SectionSetFlags ${SEC0003} 16
        SectionSetText ${SEC0003} "VLC (Already Installed)"
        SectionSetSize ${SEC0003} 0
    ${ElseIf} $vlcVers == "${VLCVERSION4}"
        SectionSetFlags ${SEC0003} 16
        SectionSetText ${SEC0003} "VLC (Already Installed)"
        SectionSetSize ${SEC0003} 0
    ${Else}
        SectionSetFlags ${SEC0003} 1
        SectionSetSize ${SEC0003} 78746
    ${EndIf}
	
	SectionSetFlags ${SEC0014} 16
	SectionSetText ${SEC0014} "SlimDX (Already Installed)"
	SectionSetSize ${SEC0014} 0
			
	!insertmacro IfKeyExists "HKLM" "SOFTWARE\Classes\Installer\Assemblies\Global" 'SlimDX,version="2.0.12.43",culture="neutral",publicKeyToken="B1B0C32FD1FFE4F9",processorArchitecture="x86"'
	Pop $R0
	${If} $R0 == 0
		!insertmacro IfKeyExists "HKLM" "SOFTWARE\Classes\Installer\Assemblies\Global" 'SlimDX,version="2.0.12.43",culture="neutral",publicKeyToken="B1B0C32FD1FFE4F9",processorArchitecture="x64"'
		Pop $R0
		${If} $R0 == 0
			SectionSetFlags ${SEC0014} 1
			SectionSetText ${SEC0014} "SlimDX"
			SectionSetSize ${SEC0014} 11470
		${EndIf}
	${EndIf}
FunctionEnd

# Uninstaller functions
Function un.onInit
    ReadRegStr $INSTDIR HKLM "${REGKEY}" Path
    !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuGroup
    SetOutPath $INSTDIR    
FunctionEnd

# Section Descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${SECGRP0001} "Mostly video stuff, you need to install for video saving and playing to work"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0002} "Advanced video encoding library for saving videos"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0003} "Epic video player that can play anything (used for video playback)"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0004} "ZomB Dashboard designer to quickly create dashboards"
!insertmacro MUI_DESCRIPTION_TEXT ${SECGRP0002} "Robot side bindings to the ZomB protocol (UDP and TCP, obsolete)"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0005} "C++ robot bindings (old, obsolete)"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0006} "Java robot bindings (old, obsolete)"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0007} "LabVIEW robot bindings (old, obsolete)"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0008} "Shortcut to the bindings folder (helpful, old, obsolete)"
!insertmacro MUI_DESCRIPTION_TEXT ${SECGRP0003} "Other programs"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0009} "Old default dashboard"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0010} "New default dashboard"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0011} "Pit display with BlueFinger and saved video playing capabilities (requires VLC)"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0012} "Enables ZomB to act as the driver station"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0013} "Enables the bindings to actually do stuff (old, obsolete)"
!insertmacro MUI_DESCRIPTION_TEXT ${SEC0014} "SlimDX runtime for shaking and joystick support (required)"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

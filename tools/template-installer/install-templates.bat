@echo off

cls
set DD4T_DATA_FOLDER=data
set DD4T_REMOVE_CONFIG=Y
set DD4T_EXTENSIONS=N
set DD4T_DLL=DD4T.Templates.merged.dll
set DD4T_TEMP_DLL=DD4T.Templates.merged.tmp.dll
title DD4T Template Installer
if not exist "%~dp0\dd4t-upload-config.xml" ( 
	TcmUploadAssembly.exe dd4t-upload-config.xml
	GoTo AskKeepConfig
)

GoTo AfterAskKeepConfig
:AskKeepConfig
echo Would you like to keep the configuration file for future uploads? [Y/N] 
set /p "DD4T_KEEP_CONFIG= "
if %DD4T_KEEP_CONFIG%==N (
	GoTo RemoveConfig
)
if %DD4T_KEEP_CONFIG%==n (
	GoTo RemoveConfig
)
:AfterAskKeepConfig


GoTo AfterRemoveConfig
:RemoveConfig
set DD4T_REMOVE_CONFIG=y
:AfterRemoveConfig

if not exist "%~dp0\dd4t-upload-config.xml" ( 
	echo Something must have gone wrong, there is no dd4t-upload-config.xml
	GoTo Exit
)

If "%1"=="" (
	GoTo AskFolder
)
set DD4T_FOLDER=%1%
GoTo CheckExtension
:AskFolder
echo Enter the URI of the folder where you want to store the DD4T template building blocks:
set /p "DD4T_FOLDER= "



:CheckExtension
If %2=="" (
	GoTo AskExtension
)
set DD4T_EXTENSIONS=%2%
GoTo AfterAskExtension
:AskExtension
echo Enter the path of the dll where you implemented the DD4T extensions (leave empty if no extensions were implemented):
set /p "DD4T_EXTENSIONS= "

If %DD4T_EXTENSIONS%==N (
	echo No extensions were implemented. 
	GoTo AfterAskFolder
)

If not exist %DD4T_EXTENSIONS% (
	echo Extension dll file not found.
	GoTo Exit
)
:AfterAskExtension
If not exist %DD4T_DATA_FOLDER%\%DD4T_TEMP_DLL% (	
	echo Renaming original dll from %DD4T_DLL% to %DD4T_DATA_FOLDER%\%DD4T_TEMP_DLL%
	rename %DD4T_DATA_FOLDER%\%DD4T_DLL% %DD4T_TEMP_DLL%
)

:MergeExtensions
echo Rebuilding (Merging) %DD4T_DLL% 
"%~dp0..\ILMerge.exe" /lib:"C:\Windows\Microsoft.NET\Framework\v4.0.30319" /lib:"%~dp0..\..\dependencies" /t:dll /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319 /out:"%~dp0%DD4T_DATA_FOLDER%\%DD4T_DLL%" "%~dp0%DD4T_DATA_FOLDER%\%DD4T_TEMP_DLL%" %DD4T_EXTENSIONS%
set DD4T_EXTENSIONS=Y

:AfterAskFolder
TcmUploadAssembly.exe dd4t-upload-config.xml %DD4T_DATA_FOLDER%\%DD4T_DLL% /folder:%DD4T_FOLDER% /verbose


if %DD4T_REMOVE_CONFIG%==y (
	echo Removing config file
	del dd4t-upload-config.xml
)

if %DD4T_EXTENSIONS%==Y (
	echo Removing temp file %DD4T_TEMP_DLL%
	del %DD4T_DATA_FOLDER%\%DD4T_DLL%
	rename %DD4T_DATA_FOLDER%\%DD4T_TEMP_DLL% %DD4T_DLL%
)
	
:Exit
set DD4T_FOLDER=
set DD4T_REMOVE_CONFIG=
set DD4T_KEEP_CONFIG=
set DD4T_ACCEPT_FOLDER=
set DD4T_EXTENSIONS=
set DD4T_DLL=
set DD4T_TEMP_DLL=
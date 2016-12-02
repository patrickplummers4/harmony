set PATH=%path%;%windir%\Microsoft.NET\Framework\v4.0.30319

REM Uninstall the Service if it exists already

InstallUtil /u .\bin\Debug\HarmonyService.exe

REM Install the Service

InstallUtil /i .\bin\Debug\HarmonyService.exe

REM Start the Service once we're done
net start HarmonyHubService
PAUSE
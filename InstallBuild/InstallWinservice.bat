cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

net stop "PowellLibraryService"

InstallUtil.exe -U "C:\DEPLOY\WinNestLibraryService\bin\Powell.Mfg.LibraryWinService.exe"


InstallUtil.exe "C:\DEPLOY\WinNestLibraryService\bin\Powell.Mfg.LibraryWinService.exe"

net start "PowellLibraryService"

@echo off
set steamAppPath=D:\SteamLibrary\steamapps
xcopy .\FirstPluginFiles\* %steamAppPath%\workshop\content\1189490\2824349934\BepInEx\plugins\FirstPlugin /Y /S
xcopy .\ResourceProj\AssetBundles\allneed %steamAppPath%\workshop\content\1189490\2824349934\BepInEx\plugins\FirstPlugin /Y
xcopy .\FirstBepinPlugin\bin\Debug\FirstBepinPlugin.dll %steamAppPath%\workshop\content\1189490\2824349934\BepInEx\plugins\FirstPlugin /Y

xcopy .\NextMod\* %steamAppPath%\common\�ٳ���\����Mod����\test\plugins\Next\modTest /Y /S


pause 
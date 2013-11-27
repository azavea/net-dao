echo off
if not exist nupkg mkdir nupkg
del /q nupkg\*
echo on
.nuget\NuGet.exe pack -Symbols -Prop Configuration=Release -OutputDirectory nupkg

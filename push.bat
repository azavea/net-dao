echo off
set pkg_name=%1
echo on
.nuget\NuGet.exe push %pkg_name%
.nuget\Nuget.exe push %pkg_name% -s "http://nupeek.internal.azavea.com/"

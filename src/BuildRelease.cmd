echo off
pushd "%~dp0\LagKiller"
dotnet msbuild -t:BuildReleaseZip -p:Configuration=Release -p:ReleaseDir=../Releases 
popd

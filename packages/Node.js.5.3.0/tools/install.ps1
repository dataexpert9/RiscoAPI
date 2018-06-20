param($installPath, $toolsPath, $package, $project)

. (Join-Path $toolsPath 'bin_tools.ps1')

$cmd = '.bin\node.cmd'
Set-BuildAction $cmd 'None'
Update-BinPaths $cmd
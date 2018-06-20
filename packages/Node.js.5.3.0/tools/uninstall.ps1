param($installPath, $toolsPath, $package, $project)

. (Join-Path $toolsPath 'bin_tools.ps1')

$cmd = '.bin\node.cmd'
Delete-Bin $cmd
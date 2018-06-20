Function Update-BinPaths($cmd) {
    $cmd_name = [IO.Path]::GetFileName($cmd)
    $bin_name = [IO.Path]::GetDirectoryName($cmd)

    $repo_rel_path = '..\..\packages'
    $repo_rel_regex = $repo_rel_path.Replace('\', '\\').Replace('.', '\.')
    $bin_dir = Join-Path (gi $project.FullName).Directory.FullName $bin_name
    $repo_dir = (gi $installPath).Parent.FullName

    $source = Join-Path $installPath "content\$cmd"
    $target = Join-Path $bin_dir $cmd_name

    cd $bin_dir

    if (($repo_dir | rvpa -relative) -eq $repo_rel_path) {
        return;
    }

    (gc $source) | % {$_ -replace $repo_rel_regex, ($repo_dir | rvpa -relative)} | sc $source
    cp $source $target
}

Function Add-BinToPath($cmd) {
    $bin_name = [IO.Path]::GetDirectoryName($cmd)
    $path = [Environment]::GetEnvironmentVariable('Path', [EnvironmentVariableTarget]::User)

    if ($path -notmatch ";$bin_name") {
        [Environment]::SetEnvironmentVariable('Path', "$path;$bin_name", [EnvironmentVariableTarget]::User)
    }
}

Function Set-BuildAction($cmd, $action) {
    $pi = Get-ProjectItem $cmd
    if ($pi) {
        $num = [int]2
        if ($action -eq 'None') {
            $num = [int]0
        } elseif ($action -eq 'Compile') {
            $num = [int]1
        } elseif ($action -eq 'Embedded Resource') {
            $num = [int]3
        }
        
        $pi.Properties.Item('BuildAction').Value = $num;
    }
}

Function Delete-Bin($cmd) {
    $pi = Get-ProjectItem $cmd
    if ($pi) {
        $pi.Delete()
    }
}

Function Get-ProjectItem($cmd) {
    $cmd_name = [IO.Path]::GetFileName($cmd)
    $bin_name = [IO.Path]::GetDirectoryName($cmd)

    try {
        $pi = $project.ProjectItems.Item($bin_name)
        if ($pi) {
            $pi = $pi.ProjectItems.Item($cmd_name)
        }
        return $pi
    } catch {
        return $null
    }
}
$packages = Get-ChildItem -Filter *.dll -Recurse |
    ForEach-Object {
    try {
        $_ | Add-Member NoteProperty FileVersion ($_.VersionInfo.FileVersion)
        $_ | Add-Member NoteProperty AssemblyVersion (
            [Reflection.AssemblyName]::GetAssemblyName($_.FullName).Version
        )
    }
    catch {}
} | Select-Object Name, FileVersion, AssemblyVersion

$groupPackage = $packages | group-object -property Name;

$errorText;

foreach ($group in $groupPackage) {
    $versions = $group.Group | sort-object -Property AssemblyVersion -Unique

    if ($versions.Count -gt 1) {
        $errorText = "These files have different versions"
        foreach ($version in $versions) {
            $errorText = "$($errorText)`n    $($version)"
        }
    }
}

if($errorText -ne $null) {
    throw $errorText
}

Write-Host "Check Consolidate Dlls successful" -ForegroundColor Green


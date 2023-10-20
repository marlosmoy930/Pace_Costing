$ErrorActionPreference = "Stop";

$packagesConfigs = Get-ChildItem packages.config -Recurse

$allVersions = New-Object System.Collections.ArrayList

foreach ($configPath in $packagesConfigs) {
    $projectName = $configPath.Directory.Name
    $config = [xml](Get-Content $configPath -Raw)
    $versions = $config.packages.package | Select-Object -Property id,version
    $versions | ForEach-Object { Add-Member -Type NoteProperty -Name project -Value $projectName -InputObject $_ }

    if ($versions.Count -gt 1) {
        $allVersions.AddRange($versions)
    } elseif ($versions.Count -eq 1) {
        $allVersions.Add($versions)
    }
}

$groupedById = $allVersions | Group-Object id

$anyPackageIsBroken = $false

foreach ($group in $groupedById) {
    $uniqueVersions = $group.Group | Select-Object -ExpandProperty version | Sort-Object -Unique
    if ($uniqueVersions.Count -gt 1) {
        if (-not $anyPackageIsBroken) {
            Write-Host "Broken packages:"
            Write-Host
        }
        $anyPackageIsBroken = $true
        Write-Host $group.Name
        Write-Host Referenced versions: $([string]::Join(", ", $uniqueVersions))
        Write-Host
    }
}

if($anyPackageIsBroken) {
    throw "Some project contains a reference to the same package with different versions."
}
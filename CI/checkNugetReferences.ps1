$ErrorActionPreference = "Stop";

function ParseHintPath
{
    [CmdletBinding()]
    Param([parameter(ValueFromPipeline)] $hintPath)

    process
    {
        $package = $hintPath.Substring($hintPath.IndexOf("packages\") + "packages\".Length)
        $package = $package.Substring(0, $package.IndexOf("\"))
        return $package
    }
}

function ParsePackageNode
{
    [CmdletBinding()]
    Param([parameter(ValueFromPipeline)] $packageNode)

    process
    {
        $package = [xml]$packageNode
        return $package.package.id + "." + $package.package.version
    }
}

$projects = Get-ChildItem -Recurse *.csproj

$throwError = $false

foreach ($project in $projects)
{
    $referencedPackages = Get-Content $project | Where-Object { $_.Contains("packages\") } | ParseHintPath
    if ($referencedPackages)
    {
        $declaredPackages = Get-Content $([System.IO.Path]::GetDirectoryName($project.FullName) + "\packages.config") | Where-Object { $_.Contains("<package ") } | ParsePackageNode
        $notDeclaredPackages = $referencedPackages | Sort-Object -Unique | Where-Object { -not $declaredPackages.Contains($_) }

        if ($notDeclaredPackages)
        {
            $throwError = $true
            Write-Host $project.Name
            Write-Host "Broken NuGet references:"
            $notDeclaredPackages | Write-Host
            Write-Host
        }
    }
}

if ($throwError)
{
    throw "Some projects has broken NuGet references"
}
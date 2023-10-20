param(
    [string]$SolutionDir = "$CI_PROJECT_DIR\",
    [string]$Project,
    [string]$OutDir,
    [string]$BranchName = $CI_COMMIT_REF_NAME,
    [string]$CommitHash = $CI_COMMIT_SHA,
    [string]$Configuration = "Release",
    [bool]$SonarAnalysis = $False
)
$ErrorActionPreference = "Stop";

. $PSScriptRoot\invokeMsbuild.ps1

$excludePbd = ""
if($Configuration -eq "Release" -And -Not $SonarAnalysis)
{
    Write-Host "Excluding PDBs"
    $excludePbd = "/p:DebugSymbols=false /p:DebugType=None"
}

$logicalCoresNumber = (Get-WmiObject Win32_ComputerSystem | Select NumberOfLogicalProcessors).NumberOfLogicalProcessors

nuget restore CscGet.sln

$msbuildParams = "$Project /nr:false /p:DeployOnBuild=True $excludePbd /m:$logicalCoresNumber /p:SolutionDir=$SolutionDir /p:OutDir=$OutDir /p:Configuration=$Configuration /p:build_number=$Version"
Invoke-MsBuild -Parameters $msbuildParams

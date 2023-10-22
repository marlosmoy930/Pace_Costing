$ErrorActionPreference = "Stop";
$sw = [Diagnostics.Stopwatch]::StartNew()

$projectDirectory = Resolve-Path "$($(Get-Location).Path)"
Write-host "Project dir is $projectDirectory"

$runner = Resolve-Path "$projectDirectory\packages\xunit.runner.console.2.2.0\tools\xunit.console.exe"
#$runner = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"

$reportPath = "$projectDirectory\coverage\dotCover.xml"
$testPath = "$projectDirectory\build"

$runnerArguments = $(Get-ChildItem -Path $testPath -Include **.Tests.dll -Recurse | Where { $_.FullName -notlike "*\obj\*" } | % { $_.FullName })

Write-Host "Found tests are $runnerArguments";
#$runnerArguments = "$runnerArguments /InIsolation /Enablecodecoverage /UseVsixExtensions:true /TestAdapterPath:C:\CODE\CAP-N-BackEnd\build\tests"
$runnerArguments = "$runnerArguments -verbose  -parallel none"

$testedScope = "$projectDirectory\CscGet.WebEntryPoint\bin\**\*.dll;$projectDirectory\build\**\*.dll"

$exclude = "-:*.Tests;-:*.DalTests;-:CscGet.Persistence.InMemoryDbFixtures;-:*ContextTests;-:*YamlDotNet*;-:*NHibernate*;-:*Fluent*;-:*xunit.core*;-:*HtmlAgilityPack*;-:*EntityFramework*;"

dotCover analyse /Filters=$exclude /ProcessFilters=-:sqlservr.exe /ReportType=XML /Output=$reportPath /TargetExecutable=$runner /TargetArguments=$runnerArguments /Scope=$testedScope

if (-not $?){
    Write-Host "Failed with $LASTEXITCODE"
}

$sw.Stop()

# parsing reuslts xml file
[xml]$results = Get-Content $reportPath
$resultCOverage = $results.Root.CoveredStatements / $results.Root.TotalStatements
Write-Host "Coverage is $resultCOverage, took $($sw.Elapsed)"

$ErrorActionPreference = "Stop";

# fix for maxpath more than 248 symbols
$path = Resolve-Path .\
$projects = cmd /c "cd $path && dir /s /b"

$projectsWithWrongRefs = $projects -Like "*.csproj" | Where-Object { $(Get-Content $_) -match "<HintPath>(?!(\.\.\\)+(packages|libs))" }

if ($projectsWithWrongRefs.Count -ne 0){
    Write-Host $projectsWithWrongRefs
    throw "Wrong references detected"
}

Write-Host "References are OK"


$projectsNotAttachedToSln = Get-ChildItem -Filter *.csproj -Recurse | Where-Object { -not $(Get-Content CscGet.sln -Raw).Contains($_.Name) }
if ($projectsNotAttachedToSln){
    throw "All projects should be attached to sln file, $projectsNotAttachedToSln"
}


$projectsWithGACReferences = Get-ChildItem -Filter *.csproj -Recurse | Where-Object { $(Get-Content $_.FullName) -match "<Reference Include=`"(?!(System(?!\.Web\.Http)(?!\.Net\.Http\.Formatting))|Microsoft|WindowsBase).*(?=/>)" }
if ($projectsWithGACReferences){
    throw "All external references should be specified with HintPath attributes, $projectsWithGACReferences"
}

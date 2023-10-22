param(
    [string] $IntegrationTestToken,
    [string] $IntegrationTestsUri,
    [string] $PrivateToken,
    [string] $Branch,
    [int] $Timeout = 300,
    [int] $Interval = 10
)
function Write-Status ($status, $id) {
    switch($status)
    {
        "failed" {
            Write-Host "$($status.ToUpper()). Please check details via http://vm199251.projects.local/DXC-Tools/DXC-Costing-IntegrationTests/pipelines/$id" -ForegroundColor "red"
            throw 
        }
        "success" {
            Write-Host "$($status.ToUpper()). Please check details via http://vm199251.projects.local/DXC-Tools/DXC-Costing-IntegrationTests/pipelines/$id" -ForegroundColor "green"
            exit 
        }
        "canceled" {
            Write-Host $status.ToUpper()
            throw 
        }
        "pending" {
            Write-Host "Status: $status"     
        }
        "running" {
            Write-Host "Status: $status"
        }

    }
}

if (($Timeout -eq 0) -or ($Timeout -eq "")){
    $Timeout = 300
}


$triggerUrl = $IntegrationTestsUri + "trigger/pipeline"
Write-Host "Trigerring integration tests pipeline..."
$job = curl -Method Post -Body @{"token"="$IntegrationTestToken";"ref"="$Branch"} -Uri "$triggerUrl" -UseBasicParsing

if (0 -eq $?){
    throw "CURL FAILED"
}

Write-Host "Waiting for response 40 seconds..."
Start-Sleep -seconds 40

$content = $job.Content | ConvertFrom-Json
$getUrl = $IntegrationTestsUri + "pipelines/" + $content.id

for ($time=40; $time -le $Timeout; $time+=$Interval){

    $response = curl -Headers @{"PRIVATE-TOKEN" = "$PrivateToken"} -Uri "$getUrl" -UseBasicParsing
    $result = $response.Content | ConvertFrom-Json
    Write-Status -status $result.status -id $content.id

    Write-Host "Waiting for response $Interval seconds..."

    Start-Sleep -seconds $Interval
}

throw "Integration tests are timed out. Please check details via http://vm199251.projects.local/DXC-Tools/DXC-Costing-IntegrationTests/pipelines/$($content.id)"
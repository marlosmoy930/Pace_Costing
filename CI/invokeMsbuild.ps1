function Invoke-MsBuild {
    param(
        $Parameters
    )

    $msbuildPath = "MsBuild.exe"
    $arguments = $Parameters.Split(" ")
    & "$msbuildPath" $arguments

    if (-not $?){
        throw "Msbuild failed"
    }
}
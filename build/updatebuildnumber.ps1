param($previousBuildNumber)

# Replace build number for nuget
$newBuildNumber = $previousBuildNumber.SubString(0, $previousBuildNumber.Length -2) 
   
# This will be what sets the build number
Write-Host "##vso[build.updatebuildnumber]$newBuildNumber" 
Write-Host "Updated  build number from '$previousBuildNumber' to '$newBuildNumber'"
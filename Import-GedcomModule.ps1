[CmdletBinding()]
param ()

$AppBaseFolder = "$PSScriptRoot/src/Stravaig.Gedcom.PowerShell"
$ProjectFile = "$AppBaseFolder/Stravaig.Gedcom.PowerShell.csproj";
$AppPublishFolder = "$AppBaseFolder/bin/psm";


$module = Get-Module -Name Stravaig.Gedcom.PowerShell
if ($null -ne $module)
{
    Write-Host "Module already loaded. Restart the powershell session if you want updates." -ForegroundColor Cyan
    Exit 1;
}

Write-Host "Cleaning destination..." -ForegroundColor Green
Get-ChildItem -Path $AppPublishFolder -Recurse -File | Remove-Item

Write-Host "Building..." -ForegroundColor Green
& dotnet build "$ProjectFile" --configuration Release

Write-Host "Publishing..." -ForegroundColor Green
& dotnet publish "$ProjectFile" --configuration Release  --output "$AppPublishFolder" --force

Import-Module "$AppPublishFolder\Stravaig.Gedcom.PowerShell.dll" -Verbose

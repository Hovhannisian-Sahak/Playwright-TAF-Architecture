$ErrorActionPreference = 'Stop'

$configPath = Join-Path $PSScriptRoot '..\PlaywrightTAF.Tests\ReportPortal.config.json'

if (Test-Path $configPath) {
    $config = Get-Content $configPath -Raw | ConvertFrom-Json
} else {
    $config = [PSCustomObject]@{
        enabled = $false
        server = [PSCustomObject]@{
            url = 'https://demo.reportportal.io'
            project = 'hovhannisian-sahak_personal'
            authentication = [PSCustomObject]@{
                uuid = ''
            }
        }
        launch = [PSCustomObject]@{
            name = 'Playwright Automation'
            description = 'Playwright Tests'
            debugMode = $false
            tags = @('playwright', 'nunit')
        }
    }
}

if ([string]::IsNullOrWhiteSpace($env:REPORTPORTAL_API_KEY)) {
    $config.enabled = $false
} else {
    $config.enabled = $true
    $config.server.authentication.uuid = $env:REPORTPORTAL_API_KEY
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_URL)) {
    $config.server.url = $env:REPORTPORTAL_URL
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_PROJECT)) {
    $config.server.project = $env:REPORTPORTAL_PROJECT
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_LAUNCH_NAME)) {
    $config.launch.name = $env:REPORTPORTAL_LAUNCH_NAME
} elseif (-not [string]::IsNullOrWhiteSpace($env:BUILD_NUMBER)) {
    $config.launch.name = "Playwright Automation #$env:BUILD_NUMBER"
}

$config | ConvertTo-Json -Depth 10 | Set-Content $configPath -Encoding UTF8

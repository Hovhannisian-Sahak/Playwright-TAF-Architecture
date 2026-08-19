$ErrorActionPreference = 'Stop'

$testProjectPath = Resolve-Path (Join-Path $PSScriptRoot '..\PlaywrightTAF.Tests')
$configPath = Join-Path $testProjectPath 'ReportPortal.config.json'

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

$hasReportPortalApiKey = -not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_API_KEY)

if ($hasReportPortalApiKey) {
    $config.enabled = $true
    $config.server.apiKey = $env:REPORTPORTAL_API_KEY
    $config.server.authentication.uuid = $env:REPORTPORTAL_API_KEY
} else {
    $config.enabled = $false
    $config.server.apiKey = ''
    $config.server.authentication.uuid = ''
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_URL)) {
    $reportPortalUrl = $env:REPORTPORTAL_URL.TrimEnd('/')

    if (-not $reportPortalUrl.EndsWith('/api/v1', [System.StringComparison]::OrdinalIgnoreCase)) {
        $reportPortalUrl = "$reportPortalUrl/api/v1"
    }

    $config.server.url = "$reportPortalUrl/"
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_PROJECT)) {
    $config.server.project = $env:REPORTPORTAL_PROJECT
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_LAUNCH_NAME)) {
    $config.launch.name = $env:REPORTPORTAL_LAUNCH_NAME
} elseif (-not [string]::IsNullOrWhiteSpace($env:BUILD_NUMBER)) {
    $config.launch.name = "Playwright Automation #$env:BUILD_NUMBER"
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_LAUNCH_DESCRIPTION)) {
    $config.launch.description = $env:REPORTPORTAL_LAUNCH_DESCRIPTION
}

if (-not [string]::IsNullOrWhiteSpace($env:REPORTPORTAL_LAUNCH_TAGS)) {
    $tags = $env:REPORTPORTAL_LAUNCH_TAGS -split ',' |
        ForEach-Object { $_.Trim() } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

    $config.launch.tags = @($tags)
}

$outputConfigPath = Join-Path $testProjectPath "bin\$env:CONFIGURATION\net8.0\ReportPortal.config.json"

if (Test-Path (Split-Path $outputConfigPath -Parent)) {
    $json = $config | ConvertTo-Json -Depth 10
    $json | Set-Content $outputConfigPath -Encoding UTF8
}

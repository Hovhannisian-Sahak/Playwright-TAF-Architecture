pipeline {
    agent any

    options {
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '20'))
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        CONFIGURATION = 'Release'
        REPORTPORTAL_URL = 'https://demo.reportportal.io'
        REPORTPORTAL_PROJECT = 'hovhannisian-sahak_personal'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore NuGet Packages') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build Solution') {
            steps {
                bat 'dotnet build --configuration %CONFIGURATION% --no-restore'
            }
        }

             stage('Configure ReportPortal') {
      
                 steps {
      
                     withCredentials([
                         string(
                             credentialsId: 'reportportal-api-key',
                             variable: 'REPORTPORTAL_API_KEY'
                         )
                     ]) {
      
                         bat '''
                         powershell -NoProfile -ExecutionPolicy Bypass -Command ^
                         "$path = 'PlaywrightTAF.Tests\\ReportPortal.config.json'; ^
                         $config = Get-Content $path -Raw | ConvertFrom-Json; ^
                         $config.enabled = $true; ^
                         $config.server.url = $env:REPORTPORTAL_URL; ^
                         $config.server.project = $env:REPORTPORTAL_PROJECT; ^
                         $config.server.authentication.uuid = $env:REPORTPORTAL_API_KEY; ^
                         $config.launch.name = 'Playwright Automation #' + $env:BUILD_NUMBER; ^
                         $config | ConvertTo-Json -Depth 10 | Set-Content $path -Encoding UTF8"
                         '''
                     }
                 }
             }

        stage('Install Playwright Browsers') {
             steps {
                 bat 'dotnet tool install --global Microsoft.Playwright.CLI'
                 bat 'playwright install'
             }
        }

       stage('Run Tests')
       {
           parallel
           {
               stage('API Tests')
               {
                   steps
                   {
                       bat '''
                       dotnet test PlaywrightTAF.Tests\\PlaywrightTAF.Tests.csproj ^
                       --filter TestCategory=API ^
                       --configuration Release
                       '''
                   }
               }
       
       
               stage('UI Tests')
               {
                   steps
                   {
                       bat '''
                       dotnet test PlaywrightTAF.Tests\\PlaywrightTAF.Tests.csproj ^
                       --filter TestCategory=UI ^
                       --configuration Release
                       '''
                   }
               }
           }
       }
    }

    post {
    
        always {
    
            archiveArtifacts artifacts: '**/TestResults/**/*',
                             fingerprint: true
    
            archiveArtifacts artifacts: '**/*.png',
                             allowEmptyArchive: true
    
            junit allowEmptyResults: true,
                  testResults: '**/*.trx'
    
            allure(
                includeProperties: false,
                jdk: '',
                results: [[path: 'PlaywrightTAF.Tests/bin/Release/net8.0/allure-results']]
            )
        }

        success {
            echo 'Build completed successfully.'
        }

        failure {
            echo 'Build failed.'
        }
    }
}

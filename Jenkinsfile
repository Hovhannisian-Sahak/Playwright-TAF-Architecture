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

        stage('Configure ReportPortal') {
            steps {
                withCredentials([
                    string(
                        credentialsId: 'reportportal-api-key',
                        variable: 'REPORTPORTAL_API_KEY'
                    )
                ]) {
                    bat 'powershell -NoProfile -ExecutionPolicy Bypass -File ci\\ConfigureReportPortal.ps1'
                }
            }
        }

        stage('Build Solution') {
            steps {
                bat 'dotnet build --configuration %CONFIGURATION% --no-restore'
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
                       --configuration Release ^
                       --no-build
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
                       --configuration Release ^
                       --no-build
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

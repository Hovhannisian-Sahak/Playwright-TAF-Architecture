pipeline {
    agent any

    options {
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '20'))
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        CONFIGURATION = 'Release'
        REPORTPORTAL_URL = 'https://demo.reportportal.io/api/v1/'
        REPORTPORTAL_PROJECT = 'hovhannisian-sahak_personal'
        PERF_VUS = '3'
        PERF_DURATION_SECONDS = '60'
        PERF_REQUEST_DELAY_SECONDS = '1'
        PERF_MAX_P95_MS = '1000'
        PERF_MAX_FAILURE_RATE = '0.01'
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
                       --no-build ^
                       --logger:ReportPortal ^
                       --logger "trx;LogFileName=api-tests.trx" ^
                       --results-directory TestResults
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
                       --no-build ^
                       --logger:ReportPortal ^
                       --logger "trx;LogFileName=ui-tests.trx" ^
                       --results-directory TestResults
                       '''
                   }
               }
           }
       }
       
       stage('Run Performance Tests') {
           steps {
               bat '''
               dotnet test PlaywrightTAF.Tests\\PlaywrightTAF.Tests.csproj ^
               --filter TestCategory=Performance ^
               --configuration Release ^
               --no-build ^
               --logger:ReportPortal ^
               --logger "trx;LogFileName=performance-tests.trx" ^
               --results-directory TestResults
               '''
           }
       }
    }

    post {
    
        always {
    
            archiveArtifacts artifacts: '**/TestResults/**/*',
                             fingerprint: true
    
            archiveArtifacts artifacts: '**/*.png',
                             allowEmptyArchive: true

            archiveArtifacts artifacts: '**/logs/**/*.log',
                             allowEmptyArchive: true

            junit allowEmptyResults: true,
                  testResults: '**/*.trx'
    
            allure(
                includeProperties: false,
                jdk: '',
                results: [[path: 'PlaywrightTAF.Tests/bin/Release/net8.0/allure-results']]
            )

            bat '''
            if exist PlaywrightTAF.Tests\\Authentication\\AuthStates\\adminState.json del PlaywrightTAF.Tests\\Authentication\\AuthStates\\adminState.json
            if exist PlaywrightTAF.Tests\\Authentication\\AuthStates\\userState.json del PlaywrightTAF.Tests\\Authentication\\AuthStates\\userState.json
            '''
        }

        success {
            echo 'Build completed successfully.'
        }

        failure {
            echo 'Build failed.'
        }
    }
}

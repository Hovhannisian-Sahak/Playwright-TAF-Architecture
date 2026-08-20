pipeline {
    agent any

    options {
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '20'))
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        CONFIGURATION = 'Release'
        TAF_Headless = 'true'
        TAF_Admin__Username = 'Admin'
        TAF_Admin__Password = 'admin123'
        TAF_User__Username = 'Users'
        TAF_User__Password = 'users123'
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

        stage('Clean Solution') {
            steps {
                bat 'dotnet clean --configuration %CONFIGURATION%'
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

       stage('Functional Tests') {
           parallel {
               stage('API Tests') {
                   steps {
                       withCredentials([
                           string(
                               credentialsId: 'reportportal-api-key',
                               variable: 'REPORTPORTAL_API_KEY'
                           )
                       ]) {
                           bat '''
                           set "REPORTPORTAL_LAUNCH_NAME=API Tests #%BUILD_NUMBER%"
                           set "REPORTPORTAL_LAUNCH_DESCRIPTION=Functional API tests for build #%BUILD_NUMBER% on %JOB_NAME%"
                           set "REPORTPORTAL_LAUNCH_TAGS=api,functional,playwright,nunit,dotnet,jenkins,build-%BUILD_NUMBER%"
                           if exist TestOutput\\api rmdir /s /q TestOutput\\api
                           xcopy PlaywrightTAF.Tests\\bin\\Release\\net8.0 TestOutput\\api /E /I /Y >nul
                           set "REPORTPORTAL_CONFIG_OUTPUT_PATH=TestOutput\\api\\ReportPortal.config.json"
                           powershell -NoProfile -ExecutionPolicy Bypass -File ci\\ConfigureReportPortal.ps1
                           dotnet vstest TestOutput\\api\\PlaywrightTAF.Tests.dll ^
                           --TestCaseFilter:TestCategory=API ^
                           --Logger:ReportPortal ^
                           --Logger:"trx;LogFileName=api-tests.trx" ^
                           --ResultsDirectory:TestResults
                           '''
                       }
                   }
               }

               stage('UI Tests') {
                   steps {
                       withCredentials([
                           string(
                               credentialsId: 'reportportal-api-key',
                               variable: 'REPORTPORTAL_API_KEY'
                           )
                       ]) {
                           bat '''
                           set "REPORTPORTAL_LAUNCH_NAME=UI Tests #%BUILD_NUMBER%"
                           set "REPORTPORTAL_LAUNCH_DESCRIPTION=Browser UI tests for build #%BUILD_NUMBER% on %JOB_NAME%"
                           set "REPORTPORTAL_LAUNCH_TAGS=ui,browser,playwright,nunit,dotnet,jenkins,build-%BUILD_NUMBER%"
                           if exist TestOutput\\ui rmdir /s /q TestOutput\\ui
                           xcopy PlaywrightTAF.Tests\\bin\\Release\\net8.0 TestOutput\\ui /E /I /Y >nul
                           set "REPORTPORTAL_CONFIG_OUTPUT_PATH=TestOutput\\ui\\ReportPortal.config.json"
                           powershell -NoProfile -ExecutionPolicy Bypass -File ci\\ConfigureReportPortal.ps1
                           dotnet vstest TestOutput\\ui\\PlaywrightTAF.Tests.dll ^
                           --TestCaseFilter:TestCategory=UI ^
                           --Logger:ReportPortal ^
                           --Logger:"trx;LogFileName=ui-tests.trx" ^
                           --ResultsDirectory:TestResults
                           '''
                       }
                   }
               }
           }
       }
       
       stage('Run Performance Tests') {
           steps {
               withCredentials([
                   string(
                       credentialsId: 'reportportal-api-key',
                       variable: 'REPORTPORTAL_API_KEY'
                   )
               ]) {
                   bat '''
                   set "REPORTPORTAL_LAUNCH_NAME=Performance Tests #%BUILD_NUMBER%"
                   set "REPORTPORTAL_LAUNCH_DESCRIPTION=API and UI performance thresholds for build #%BUILD_NUMBER% on %JOB_NAME%"
                   set "REPORTPORTAL_LAUNCH_TAGS=performance,api-performance,ui-performance,playwright,nunit,dotnet,jenkins,build-%BUILD_NUMBER%"
                   powershell -NoProfile -ExecutionPolicy Bypass -File ci\\ConfigureReportPortal.ps1
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
                results: [
                    [path: 'PlaywrightTAF.Tests/bin/Release/net8.0/allure-results'],
                    [path: 'TestOutput/api/allure-results'],
                    [path: 'TestOutput/ui/allure-results']
                ]
            )

            bat '''
            if exist PlaywrightTAF.Tests\\Authentication\\AuthStates\\adminState.json del PlaywrightTAF.Tests\\Authentication\\AuthStates\\adminState.json
            if exist PlaywrightTAF.Tests\\Authentication\\AuthStates\\userState.json del PlaywrightTAF.Tests\\Authentication\\AuthStates\\userState.json
            if exist PlaywrightTAF.Tests\\Authentication\\AuthStates\\adminState-*.json del PlaywrightTAF.Tests\\Authentication\\AuthStates\\adminState-*.json
            if exist PlaywrightTAF.Tests\\Authentication\\AuthStates\\userState-*.json del PlaywrightTAF.Tests\\Authentication\\AuthStates\\userState-*.json
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

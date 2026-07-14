pipeline {
    agent any

    options {
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '20'))
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        CONFIGURATION = 'Release'
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

            archiveArtifacts artifacts: '**/TestResults/**/*', fingerprint: true

            junit allowEmptyResults: true,
                  testResults: '**/*.trx'

            allure(
                includeProperties: false,
                jdk: '',
                results: [[path: 'allure-results']]
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
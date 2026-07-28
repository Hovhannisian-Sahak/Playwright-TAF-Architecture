# Playwright TAF

A .NET 8 test automation framework for UI, API, and lightweight performance testing.

The solution combines NUnit, Microsoft Playwright, RestSharp, Serilog, Allure, ReportPortal, and Jenkins. UI tests target the OrangeHRM demo site. API and performance tests target the Conduit API.

## Project Structure

```text
TAF-Playwright/
|-- PlaywrightTAF.sln
|-- PlaywrightTAF.Core/
|-- PlaywrightTAF.UI/
|-- PlaywrightTAF.API/
|-- PlaywrightTAF.Tests/
|-- Performance/
|-- ci/
|-- Jenkinsfile
`-- REPORTPORTAL.md
```

## Projects

### PlaywrightTAF.Core

Shared infrastructure used by UI, API, and test projects.

- `Configuration/AppConfiguration.cs` defines strongly typed settings such as UI URL, API URL, browser, headless mode, timeout, admin credentials, and user credentials.
- `Configuration/ConfigurationReader.cs` loads `appsettings.json`, applies `TAF_` environment variable overrides, validates required values, and exposes the cached configuration through `ConfigurationReader.Current`.
- `Authentication/Credentials.cs` stores username/password values.
- `Authentication/AuthStatePaths.cs` builds paths for generated Playwright storage-state files.
- `Authentication/UserRole.cs` defines `Admin` and `User` roles.
- `Logging/LogProvider.cs` configures Serilog console logging and rolling file logs.

### PlaywrightTAF.UI

Page Object Model layer for OrangeHRM UI automation.

- `Pages/BasePage.cs` is the common base page with navigation, page-load wait, title, current URL, and `IsLoadedAsync`.
- `Pages/LoginPage.cs` models the login screen.
- `Pages/MainPage.cs` models the logged-in main page and logout menu.
- `Pages/DropDown.cs` is a small reusable dropdown helper.
- `Pages/AdminPages/Base/BasePageAdmin.cs` opens the Admin area and Corporate Branding page.
- `Pages/AdminPages/CorporateBrandingPage.cs` handles the Corporate Branding color picker and publish action.
- `Pages/UserManagementPages/Base/UserManagementPageBase.cs` contains common user-management search and assertion behavior.
- `Pages/UserManagementPages/AddUserPage.cs` creates admin users.
- `Pages/UserManagementPages/EditUserPage.cs` edits the first searched user and can change username/password.
- `Pages/UserManagementPages/DeleteUserPage.cs` deletes the first searched user.
- `Pages/UserManagementPages/PersonalDetailsPage.cs` updates personal details and uploads attachments.

### PlaywrightTAF.API

API automation layer for the Conduit API.

- `Clients/ApiClient.cs` is the RestSharp base client. It sets the API base URL, logs requests/responses, validates successful responses, and throws useful errors.
- `Clients/AuthApiClient.cs` sends login and register requests.
- `Clients/ArticleApiClient.cs` creates, reads, updates, and deletes articles.
- `Clients/UserApiClient.cs` gets the current authenticated user.
- `Endpoints/ApiEndpoints.cs` centralizes API endpoint paths.
- `Services/AuthService.cs` wraps login/register into test-friendly methods.
- `Services/ArticleService.cs` wraps article operations and injects the current auth token.
- `Services/UserService.cs` wraps current-user retrieval.
- `Authentication/TokenProvider.cs` stores the current API token and expires it after 30 minutes by default.
- `RequestModels/` contains request DTOs for article, login, and registration payloads.
- `ResponseModels/` contains response DTOs for auth, article, and user responses.

Note: several request/response model files are physically in `PlaywrightTAF.API`, but their namespaces are `PlaywrightTAF.Core.RequestModels` and `PlaywrightTAF.Core.Models`.

### PlaywrightTAF.Tests

NUnit test project that references Core, UI, and API projects.

- `Base/UiBaseTest.cs` starts Playwright, launches the configured browser, creates context/page, opens `BaseUrl`, logs in through the UI by default, captures screenshots on UI failure, and closes Playwright resources.
- `Base/AuthenticatedUiBaseTest.cs` creates/reuses Playwright storage-state files so role-based UI tests can start authenticated.
- `Base/AdminTest.cs` runs UI tests with admin credentials and admin storage state.
- `Base/UserTest.cs` runs UI tests with user credentials and user storage state.
- `Base/ApiBaseTest.cs` creates a new Conduit test user before each API test, logs in, stores the token, and initializes API services.
- `Authentication/AuthSetup.cs` generates Playwright storage-state files by logging in through the UI in a headless browser.
- `Authentication/AuthStateGeneratorTests.cs` can generate admin and user auth-state files.
- `ApiTests/ArticleApiTests.cs` covers create, get, update, and delete article API flows.
- `ApiTests/UsersApiTests.cs` verifies current-user API data.
- `UiTests/DashboardTests.cs` verifies admin dashboard navigation.
- `UiTests/ProfileTests.cs` verifies user profile navigation.
- `UiTests/UserManagementTests.cs` covers add, delete, edit, personal details, and file upload flows.
- `UiTests/AdminCorporateBrandingTests.cs` covers Corporate Branding color selection and publishing.
- `UserPermissionTests.cs` verifies the user role does not navigate to an admin page.
- `PerformanceTests/CreateArticlePerformanceTests.cs` wraps the standalone performance runner in NUnit so Allure and ReportPortal can capture the performance gate as a normal test.

## Configuration

Main test configuration:

```text
PlaywrightTAF.Tests/appsettings.json
```

Current defaults:

```json
{
  "BaseUrl": "https://opensource-demo.orangehrmlive.com/",
  "ApiBaseUrl": "https://conduit-api.bondaracademy.com",
  "Browser": "chromium",
  "Headless": false,
  "DefaultTimeoutMilliseconds": 30000,
  "Admin": {
    "Username": "Admin",
    "Password": "admin123"
  },
  "User": {
    "Username": "Users",
    "Password": "users123"
  }
}
```

Environment variables with the `TAF_` prefix override values from `appsettings.json`.

Examples:

```powershell
$env:TAF_Browser = "firefox"
$env:TAF_Headless = "true"
$env:TAF_DefaultTimeoutMilliseconds = "45000"
$env:TAF_Admin__Username = "Admin"
$env:TAF_Admin__Password = "admin123"
```

Supported browser values:

```text
chromium
firefox
webkit
```

## Setup

Restore dependencies:

```powershell
dotnet restore
```

Build:

```powershell
dotnet build
```

Install Playwright browsers:

```powershell
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

If the Playwright CLI is already installed:

```powershell
playwright install
```

## Running Tests

Run all tests:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj
```

Run API category tests:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=API
```

Run UI category tests:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=UI
```

Run performance category tests:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=Performance
```

Run Release tests:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --configuration Release
```

Some tests currently do not have `[Category("UI")]` or `[Category("API")]`, so category-filtered runs may not execute every test class.

## Authentication State

Authenticated UI tests use Playwright storage state files under:

```text
PlaywrightTAF.Tests/Authentication/AuthStates/
```

To generate storage state manually:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter GenerateAuthStates
```

`AuthenticatedUiBaseTest` also creates missing storage states during fixture setup.

## Logging

Serilog is configured in:

```text
PlaywrightTAF.Core/Logging/LogProvider.cs
```

Logs are written to:

- console
- `logs/test-run-.log`

Change log level:

```powershell
$env:TAF_LOG_LEVEL = "Debug"
```

## Screenshots

When a UI test fails, `UiBaseTest` captures a full-page screenshot and attaches it to Allure.

Screenshot files are saved under:

```text
screenshots/
```

## Allure Reports

Allure configuration:

```text
PlaywrightTAF.Tests/allureConfig.json
```

Configured results directory:

```text
allure-results
```

Open Debug report:

```powershell
allure serve PlaywrightTAF.Tests\bin\Debug\net8.0\allure-results
```

Open Release report:

```powershell
allure serve PlaywrightTAF.Tests\bin\Release\net8.0\allure-results
```

## ReportPortal

ReportPortal configuration:

```text
PlaywrightTAF.Tests/ReportPortal.config.json
```

Local publishing is disabled by default:

```json
"enabled": false
```

The Jenkins pipeline enables ReportPortal when `REPORTPORTAL_API_KEY` is provided. The helper script is:

```text
ci/ConfigureReportPortal.ps1
```

Supported ReportPortal environment variables:

```text
REPORTPORTAL_API_KEY
REPORTPORTAL_URL
REPORTPORTAL_PROJECT
REPORTPORTAL_LAUNCH_NAME
```

See `REPORTPORTAL.md` for the focused ReportPortal notes.

## Performance Testing

`Performance` is a .NET 8 console app for a basic Conduit API load test and is included in `PlaywrightTAF.sln`.

`PlaywrightTAF.Tests` references the `Performance` project and exposes the scenario through an NUnit test:

```text
PlaywrightTAF.Tests/PerformanceTests/CreateArticlePerformanceTests.cs
```

That wrapper lets Allure and ReportPortal record the performance run like the rest of the NUnit suite.

The scenario:

1. Start the configured number of virtual users.
2. Register a unique Conduit user per virtual user.
3. Repeatedly create an article with `POST /api/articles`.
4. Measure create-article duration.
5. Delete the created article.
6. Wait for the configured delay.
7. Stop after the configured duration.
8. Print metrics and fail if thresholds are exceeded.

Files:

- `Program.cs` wires options, HTTP client, scenario, metrics, cancellation, output, and threshold checks.
- `PerformanceOptions.cs` parses command-line options.
- `PerformanceTestRunner.cs` contains the reusable run logic used by both the console app and the NUnit wrapper.
- `PerformanceRunResult.cs` stores options, results, pass/fail status, and threshold failure text.
- `Clients/ConduitApiClient.cs` sends register, create article, and delete article requests.
- `Clients/ConduitEndpoints.cs` stores endpoint constants.
- `Scenarios/CreateArticleScenario.cs` runs each virtual user loop.
- `Metrics/PerformanceMetrics.cs` records request count, failures, average duration, and P95 duration.
- `Metrics/PerformanceResults.cs` stores final metric values.
- `Models/TestUser.cs` stores registered username and token.

Run with defaults:

```powershell
dotnet run --project Performance\Performance.csproj
```

Run for 60 seconds with 3 virtual users:

```powershell
dotnet run --project Performance\Performance.csproj -- --vus 3 --duration-seconds 60
```

Run through NUnit, Allure, and ReportPortal:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=Performance
```

Run through NUnit with custom settings:

```powershell
$env:PERF_VUS = "3"
$env:PERF_DURATION_SECONDS = "60"
$env:PERF_REQUEST_DELAY_SECONDS = "1"
$env:PERF_MAX_P95_MS = "1000"
$env:PERF_MAX_FAILURE_RATE = "0.01"

dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=Performance
```

Options:

```text
--base-url                  API base URL. Default: https://conduit-api.bondaracademy.com
--vus                       Number of parallel virtual users. Default: 3
--duration-seconds          Test duration. Default: 30
--request-delay-seconds     Delay between iterations per virtual user. Default: 1
--max-p95-ms                Maximum allowed P95 POST duration. Default: 1000
--max-failure-rate          Maximum allowed failure rate. Default: 0.01
```

The NUnit wrapper reads these environment variables and passes them to the performance runner:

```text
PERF_BASE_URL
PERF_VUS
PERF_DURATION_SECONDS
PERF_REQUEST_DELAY_SECONDS
PERF_MAX_P95_MS
PERF_MAX_FAILURE_RATE
```

The process exits with code `1` when failure rate or P95 exceeds the configured thresholds.

## Jenkins CI

`Jenkinsfile` runs:

1. Checkout.
2. `dotnet restore`.
3. ReportPortal config update through `ci/ConfigureReportPortal.ps1`.
4. `dotnet build --configuration Release --no-restore`.
5. Playwright browser installation.
6. API and UI tests in parallel using category filters.
7. Performance tests through the NUnit `Performance` category.
8. Artifact publishing for TRX files, screenshots, logs, and Allure results.

Jenkins expects a string credential:

```text
reportportal-api-key
```

## Useful Commands

Clean and rebuild:

```powershell
dotnet clean
dotnet restore
dotnet build
```

Run API tests in Release:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=API --configuration Release
```

Run UI tests in Release:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=UI --configuration Release
```

Run performance tests in Release:

```powershell
dotnet test PlaywrightTAF.Tests\PlaywrightTAF.Tests.csproj --filter TestCategory=Performance --configuration Release
```

Run the performance test:

```powershell
dotnet run --project Performance\Performance.csproj -- --vus 3 --duration-seconds 60
```

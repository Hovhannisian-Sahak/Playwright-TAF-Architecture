# ReportPortal

ReportPortal is wired through the `ReportPortal.NUnit` addin.

Local runs are disabled by default in `PlaywrightTAF.Tests/ReportPortal.config.json` so tests do not
fail when no ReportPortal server is available.

Configure these Jenkins environment variables to publish results:

- `REPORTPORTAL_API_KEY`: API key or UUID token for ReportPortal. Required to enable publishing.
- `REPORTPORTAL_URL`: ReportPortal server URL. Defaults to the value in `ReportPortal.config.json`.
- `REPORTPORTAL_PROJECT`: ReportPortal project name. Defaults to the value in `ReportPortal.config.json`.
- `REPORTPORTAL_LAUNCH_NAME`: Optional launch name. Jenkins uses `Playwright Automation #<BUILD_NUMBER>`
  when this is not set.

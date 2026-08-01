using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Logging;
using PlaywrightTAF.Tests.Authentication;
using Serilog;

namespace PlaywrightTAF.Tests.Base;

public abstract class AuthenticatedUiBaseTest : UiBaseTest
{
    private static readonly ILogger Logger = LogProvider.ForContext<AuthenticatedUiBaseTest>();
    private static readonly SemaphoreSlim AuthStateLock = new(1, 1);
    private static readonly HashSet<string> CreatedAuthStates = [];

    protected abstract Credentials Credentials { get; }

    protected abstract string StorageStatePath { get; }

    protected override bool ShouldLoginThroughUi => false;

    protected override bool ShouldLogoutThroughUi => true;

    protected override string InitialUrl => new Uri(new Uri(Configuration.BaseUrl), "/web/index.php/dashboard/index").ToString();

    [OneTimeSetUp]
    public override async Task OneTimeSetUpAsync()
    {
        await AuthStateLock.WaitAsync();

        try
        {
            if (CreatedAuthStates.Add(StorageStatePath))
            {
                await AuthSetup.CreateAuthStateAsync(Credentials, StorageStatePath);
            }
        }
        finally
        {
            AuthStateLock.Release();
        }

        await base.OneTimeSetUpAsync();
    }

    protected override BrowserNewContextOptions CreateContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = Configuration.BaseUrl,
            StorageStatePath = StorageStatePath
        };
    }

    [OneTimeTearDown]
    public override async Task OneTimeTearDownAsync()
    {
        await base.OneTimeTearDownAsync();
        Logger.Information("Kept shared storage auth state {StorageStatePath}", StorageStatePath);
    }
}

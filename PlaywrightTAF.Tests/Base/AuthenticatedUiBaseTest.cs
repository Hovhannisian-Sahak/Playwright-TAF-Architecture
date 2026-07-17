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
        try
        {
            await base.OneTimeTearDownAsync();
        }
        finally
        {
            DeleteStorageState();
        }
    }

    private void DeleteStorageState()
    {
        if (!File.Exists(StorageStatePath))
        {
            return;
        }

        File.Delete(StorageStatePath);
        Logger.Information("Deleted storage auth state {StorageStatePath}", StorageStatePath);
    }
}

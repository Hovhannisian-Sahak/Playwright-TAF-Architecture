using System;
using System.IO;

namespace PlaywrightTAF.Tests.TestData;

public static class TestDataFactory
{
    public const string DefaultUploadFileName = "test.png";

    public static string UniqueUsername(string prefix)
    {
        return $"{prefix}{Guid.NewGuid():N}"[..(prefix.Length + 10)];
    }

    public static string UploadFilePath(string fileName = DefaultUploadFileName)
    {
        return Path.Combine(AppContext.BaseDirectory, fileName);
    }
}

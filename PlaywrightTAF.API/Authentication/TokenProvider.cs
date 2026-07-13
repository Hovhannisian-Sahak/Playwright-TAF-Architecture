namespace PlaywrightTAF.Core.Authentication;

public class TokenProvider
{
    private static readonly TimeSpan DefaultTokenLifetime = TimeSpan.FromMinutes(30);
    private DateTimeOffset? _expiresAt;
    private string? _token;

    public string GetToken()
    {
        if (IsTokenExpired())
        {
            Clear();
        }

        if (string.IsNullOrWhiteSpace(_token))
        {
            throw new InvalidOperationException("Token is not set. Login or register before making authenticated API calls.");
        }

        return _token;
    }

    public void SetToken(string token)
    {
        SetToken(token, DefaultTokenLifetime);
    }

    public void SetToken(string token, TimeSpan lifetime)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be empty.", nameof(token));
        }

        if (lifetime <= TimeSpan.Zero)
        {
            throw new ArgumentException("Token lifetime must be greater than zero.", nameof(lifetime));
        }

        _token = token;
        _expiresAt = DateTimeOffset.UtcNow.Add(lifetime);
    }

    public void Clear()
    {
        _token = null;
        _expiresAt = null;
    }

    private bool IsTokenExpired()
    {
        return _expiresAt.HasValue && DateTimeOffset.UtcNow >= _expiresAt.Value;
    }
}

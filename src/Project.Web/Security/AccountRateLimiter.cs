using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace Project.Web.Security;

public sealed class AccountRateLimiter : IDisposable
{
    private readonly PartitionedRateLimiter<HttpContext> _signup;
    private readonly PartitionedRateLimiter<HttpContext> _login;
    private readonly PartitionedRateLimiter<HttpContext> _passwordReset;
    private readonly PartitionedRateLimiter<HttpContext> _resendConfirmation;

    public AccountRateLimiter(IConfiguration configuration)
    {
        var section = configuration.GetSection("RateLimit");
        _signup = Create(section.GetValue("SignupPerHour", 5), TimeSpan.FromHours(1));
        _login = Create(section.GetValue("LoginPerMinute", 10), TimeSpan.FromMinutes(1));
        _passwordReset = Create(section.GetValue("PasswordResetPerHour", 3), TimeSpan.FromHours(1));
        _resendConfirmation = Create(section.GetValue("ResendConfirmationPerHour", 3), TimeSpan.FromHours(1));
    }

    public Task<bool> AllowSignupAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        return AllowAsync(_signup, httpContext, cancellationToken);
    }

    public Task<bool> AllowLoginAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        return AllowAsync(_login, httpContext, cancellationToken);
    }

    public Task<bool> AllowPasswordResetAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        return AllowAsync(_passwordReset, httpContext, cancellationToken);
    }

    public Task<bool> AllowResendConfirmationAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        return AllowAsync(_resendConfirmation, httpContext, cancellationToken);
    }

    public void Dispose()
    {
        _signup.Dispose();
        _login.Dispose();
        _passwordReset.Dispose();
        _resendConfirmation.Dispose();
    }

    private static PartitionedRateLimiter<HttpContext> Create(int permitLimit, TimeSpan window)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                ClientIp(context),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = window,
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));
    }

    private static async Task<bool> AllowAsync(
        PartitionedRateLimiter<HttpContext> limiter,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        using var lease = await limiter.AcquireAsync(httpContext, 1, cancellationToken);
        if (lease.IsAcquired)
        {
            return true;
        }

        if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            var seconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
            httpContext.Response.Headers.RetryAfter = seconds.ToString(CultureInfo.InvariantCulture);
        }

        httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return false;
    }

    private static string ClientIp(HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

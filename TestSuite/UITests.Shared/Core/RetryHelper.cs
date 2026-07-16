namespace UITests.Core;

/// <summary>
/// Retry-with-backoff for gestures/actions that are occasionally flaky for
/// reasons outside the test's control (e.g. a platform dialog stealing focus
/// for a moment). Use sparingly — prefer <see cref="WaitHelper"/> to wait for
/// a precondition instead of retrying an action blindly.
/// </summary>
public static class RetryHelper
{
    public static readonly TimeSpan DefaultDelayBetweenAttempts = TimeSpan.FromMilliseconds(500);

    public static T Retry<T>(Func<T> action, int maxAttempts = 3, TimeSpan? delayBetweenAttempts = null)
    {
        if (maxAttempts < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, "maxAttempts must be at least 1.");

        var delay = delayBetweenAttempts ?? DefaultDelayBetweenAttempts;
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (attempt < maxAttempts)
                    Thread.Sleep(delay);
            }
        }

        throw new InvalidOperationException(
            $"Action failed after {maxAttempts} attempt(s). See inner exception for the last failure.", lastException);
    }

    public static void Retry(Action action, int maxAttempts = 3, TimeSpan? delayBetweenAttempts = null)
    {
        Retry<object?>(
            () =>
            {
                action();
                return null;
            },
            maxAttempts,
            delayBetweenAttempts);
    }
}
